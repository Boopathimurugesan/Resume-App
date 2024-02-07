using System.Web;

namespace ResumeDBTracker.Common.Helper
{
    public sealed class SolrQueryGeneratorHelper
    {
        /// <summary>
        /// Generates solr search query using keyword
        /// </summary>
        /// <param name="keywords">Search keyword</param>
        /// <returns>Solr search query</returns>
        public static String GenerateSolrSearchQuery(String keywords)
        {
            if (string.IsNullOrEmpty(keywords)) return "q=*:*";
            //return "q=" + HttpUtility.UrlEncode(keywords);
            if (keywords.Contains('@'))
            {
                string str = '"' + keywords + '"';
                return "q=@" + str;
            }
            else
				return "q=" + keywords;
		}

        /// <summary>
        /// Generates solr filter query for multi valued field
        /// </summary>
        /// <param name="fieldName">Name of the field</param>
        /// <param name="filterValue">Value of the field</param>
        /// <returns>Solr filter query</returns>
        public static String GenerateFilterQueryForMultipleValues(String fieldName, String? filterValue, String splitType = "")
        {
            if (String.IsNullOrEmpty(filterValue)) return string.Empty;
            String result = "";
            String[]? resultArray = null;

            if (splitType == "")
                resultArray = filterValue.Split(' ');
            else
                resultArray = filterValue.Split(',');

            if (resultArray != null)
            {
                if (resultArray.Count() > 1)
                {
                    foreach (var Item in resultArray)
                    {
                        if (result != "")
                            result += HttpUtility.UrlEncode(" OR " + fieldName + ":\"" + Item + "\"");
                        else
                            result = "&fq=" + HttpUtility.UrlEncode(fieldName + ":\"" + Item + "\"");
                    }
                }
                else
                {
                    result = "&fq=" + HttpUtility.UrlEncode(fieldName + ":\"" + filterValue + "\"");
                }
            }

            return result;
        }


        /// <summary>
        /// Generates solr filter query for single valued field
        /// </summary>
        /// <param name="fieldName">Name of the field</param>
        /// <param name="filterValue">Value of the field</param>
        /// <returns>Solr filter query</returns>
        public static String GenerateFilterQueryForSingleValue(String fieldName, String? filterValue)
        {
            if (String.IsNullOrEmpty(filterValue)) return string.Empty;
            return "&fq=" + HttpUtility.UrlEncode(fieldName + ":\"" + filterValue + "\"");
        }

        /// <summary>
        /// Generates solr filter query for range valued field
        /// </summary>
        /// <param name="fieldName">Name of the field</param>
        /// <param name="fromValue">From value</param>
        /// <param name="toValue">To value</param>
        /// <returns>Solr filter query</returns>
        public static String GenerateFilterQueryForRangeValue(String fieldName, String? fromValue, String? toValue)
        {
            if (String.IsNullOrEmpty(fromValue) && String.IsNullOrEmpty(toValue)) return string.Empty;
            return "&fq=" + HttpUtility.UrlEncode(fieldName + ":[" + (!string.IsNullOrEmpty(fromValue) ? fromValue : "*") + " TO " + (!string.IsNullOrEmpty(toValue) ? toValue : "*") + "]");
        }

        /// <summary>
        /// Generates solr facet query for given values
        /// </summary>
        /// <param name="fieldName">Facet field name in comma seperated format</param>
        /// <returns>Solr facet query</returns>
        public static String GenerateSolrFacetQuery(String fieldName)
        {
            String result = "&facet=true&facet.mincount=1";
            String[]? resultArray = null;

            resultArray = fieldName.Split(',');

            if (resultArray != null)
            {
                if (resultArray.Count() > 1)
                {
                    foreach (var Item in resultArray)
                    {
                        result += "&facet.field=" + Item;
                    }
                }
                else
                {
                    result += "&facet.field=" + fieldName;
                }
            }

            return result;
        }

        /// <summary>
        /// Generates default solr sorting query with score as base sorting and followed by corresponding field sorting
        /// </summary>
        /// <param name="fieldName">Sorting field name</param>
        /// <param name="sortOrder">Sorting order asc or desc</param>
        /// <returns>Solr sorting query</returns>
        public static String GenerateSolrDefaultSortQuery(String fieldName="", String sortOrder = "desc")
        {
            if (string.IsNullOrEmpty(fieldName))
                return "&sort=" + HttpUtility.UrlEncode("score desc");
            return "&sort=" + HttpUtility.UrlEncode("score desc, " + fieldName + " " + sortOrder);

        }

        /// <summary>
        /// Generates solr paging query
        /// </summary>
        /// <param name="pageNo">Page No</param>
        /// <param name="rowsPerPage">Records per page</param>
        /// <returns>Solr paging query</returns>
        public static String GenerateSolrPagingQuery(String pageNo, String rowsPerPage)
        {
            return "&start=" + pageNo + "&rows=" + rowsPerPage + "&wt=json&indent=true";
        }
    }
}
