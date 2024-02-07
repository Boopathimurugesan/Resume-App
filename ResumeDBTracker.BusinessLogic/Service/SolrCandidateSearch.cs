using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ResumeDBTracker.Business.Interface;
using ResumeDBTracker.Business.ViewModel;
using ResumeDBTracker.Common.Helper;
using ResumeDBTracker.Core.Models;
using System.Net;
using System.Text;

namespace ResumeDBTracker.Business.Service
{
    public class SolrCandidateSearch : ICandidateSearch
    {
        private readonly string _candidateSearchURL;
        public SolrCandidateSearch(IConfiguration configuration)
        {
            _candidateSearchURL = configuration.GetValue<string>("SolrServer:CandidateSearchURL");
        }
        public SearchCandidateResponse SearchCandidate(SearchCandiateRequest searchCandiateRequest)
        {
            StringBuilder solrQuery = new StringBuilder();
            int recordsPerPage = searchCandiateRequest.RecordPerPage != 0 ? searchCandiateRequest.RecordPerPage : 10;
            int start = (searchCandiateRequest.CurrentPage - 1) * searchCandiateRequest.RecordPerPage;
            string sorting = "";
            sorting = SolrQueryGeneratorHelper.GenerateSolrDefaultSortQuery();
            string paging = SolrQueryGeneratorHelper.GenerateSolrPagingQuery(start.ToString(), recordsPerPage.ToString());
            string facetQuery = SolrQueryGeneratorHelper.GenerateSolrFacetQuery("location,technical_skill");
            solrQuery.Append(GenerateSolrSearchQuery(searchCandiateRequest));
            solrQuery.Append(sorting);
            solrQuery.Append(paging);
            solrQuery.Append(facetQuery);
            if (!string.IsNullOrEmpty(solrQuery.ToString()))
            {
                var solrWebRequest = HttpWebRequest.Create(_candidateSearchURL);
                var postData = solrQuery.ToString();
                var encodedData = Encoding.ASCII.GetBytes(postData);

                solrWebRequest.Method = "POST";
                solrWebRequest.ContentType = "application/x-www-form-urlencoded";
                solrWebRequest.ContentLength = encodedData.Length;

                using (var DataStream = solrWebRequest.GetRequestStream())
                {
                    DataStream.Write(encodedData, 0, encodedData.Length);
                }

                var solrWebResponse = (HttpWebResponse)solrWebRequest.GetResponse();
                string responseString = new StreamReader(solrWebResponse.GetResponseStream()).ReadToEnd();
                var deserializeResult = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(responseString);
                if (deserializeResult is null) return new SearchCandidateResponse();
                var searchResult = deserializeResult["response"]["docs"];
                var recordCount = deserializeResult["response"]["numFound"];
                var locationFacetResult = deserializeResult["facet_counts"]["facet_fields"]["location"];
                var skillFacetResult = deserializeResult["facet_counts"]["facet_fields"]["technical_skill"];

                SearchCandidateResponse solrCandidateResult = new SearchCandidateResponse();
                solrCandidateResult.TotalCount = Convert.ToInt32(recordCount);
                solrCandidateResult.TotalPages = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(recordCount) / Convert.ToDouble(recordsPerPage)));
                solrCandidateResult.CurrentPage = 1;
                solrCandidateResult.CandidateResult = searchResult.ToObject<List<Candidate>>();
                solrCandidateResult.LocationFacetResult = (locationFacetResult != null) ? JsonFilterToPageResultFilter(locationFacetResult) : new List<SolrFacetResult>();
                solrCandidateResult.SkillFacetResult = (skillFacetResult != null) ? JsonFilterToPageResultFilter(skillFacetResult) : new List<SolrFacetResult>();
                return solrCandidateResult;
            }
            return new SearchCandidateResponse();
        }
        public string StringJoin(string[] array)
        {
            if (array is null || array.Length == 0) return string.Empty;
            return string.Join(",", array);
        }

        private string GenerateSolrSearchQuery(SearchCandiateRequest searchCandiateRequest)
        {
            StringBuilder ResultQuery = new StringBuilder();
            string locationJoin = StringJoin(searchCandiateRequest.Locations);
            string locationQuery = SolrQueryGeneratorHelper.GenerateFilterQueryForMultipleValues("location", locationJoin, ",");
            string skillsJoin = StringJoin(searchCandiateRequest.Skills);
            string skillsQuery = SolrQueryGeneratorHelper.GenerateFilterQueryForMultipleValues("technical_skill", skillsJoin, ",");
            string SearchKeyword = SolrQueryGeneratorHelper.GenerateSolrSearchQuery(searchCandiateRequest.Keyword);
            string jobExperience = SolrQueryGeneratorHelper.GenerateFilterQueryForRangeValue("total_exp", searchCandiateRequest.ExperienceFrom, searchCandiateRequest.ExperienceTo);
            string emailAddress = SolrQueryGeneratorHelper.GenerateFilterQueryForSingleValue("email", searchCandiateRequest.EmailAddress);
            ResultQuery.Append(SearchKeyword);
            ResultQuery.Append(locationQuery);
            ResultQuery.Append(skillsQuery);
            ResultQuery.Append(jobExperience);
            ResultQuery.Append(emailAddress);
            return ResultQuery.ToString();
        }
        private List<SolrFacetResult> JsonFilterToPageResultFilter(dynamic values)
        {
            List<SolrFacetResult> facetResult = new List<SolrFacetResult>();
            if (values != null)
            {
                int j = 1;
                for (int i = 0; i < values.Count; i++)
                {
                    SolrFacetResult solrfacet = new SolrFacetResult();
                    solrfacet.Name = Convert.ToString(values[i]);
                    //solrfacet.TotalCount = Convert.ToInt32(values[i + 1]);
                    solrfacet.Id = j;
                    i = i + 1;
                    facetResult.Add(solrfacet);
                    j++;
                }
            }
            return facetResult;
        }
    }
}
