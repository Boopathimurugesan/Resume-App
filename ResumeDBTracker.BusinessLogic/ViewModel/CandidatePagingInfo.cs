using ResumeDBTracker.Core.Models;
using X.PagedList;

namespace ResumeDBTracker.Business.ViewModel
{
    public class CandidatePagingInfo
    {
        public int? page { get; set; }
        public int? pageSize { get; set; }
        public StaticPagedList<Candidate>? CandidateList { get; set; }
        public SearchCandiateRequest searchCandiateRequest { get; set; } = new SearchCandiateRequest();
        public List<Category>? Categories { get; set; } = new List<Category>();
    }
}