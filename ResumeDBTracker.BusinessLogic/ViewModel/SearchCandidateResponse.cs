using ResumeDBTracker.Core.Models;

namespace ResumeDBTracker.Business.ViewModel
{
    public class SearchCandidateResponse
    {
        public Int32 TotalCount { get; set; }
        public Int32 TotalPages { get; set; }
        public Int32? SearchType { get; set; }
        public string? SearchOption { get; set; }
        public Int32 CurrentPage { get; set; }
        public List<Candidate>? CandidateResult { get; set; }
        public List<SolrFacetResult> LocationFacetResult { get; set; }
        public List<SolrFacetResult> SkillFacetResult { get; set; }
    }
}
