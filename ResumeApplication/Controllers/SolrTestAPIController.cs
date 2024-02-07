using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ResumeDBTracker.Business.Interface;
using ResumeDBTracker.Business.ViewModel;

namespace ResumeApplication.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SolrTestAPIController : ControllerBase
    {
        private readonly ICandidateSearch _iCandidateSearch;
        public SolrTestAPIController(ICandidateSearch candidateSearch)
        {
            _iCandidateSearch=candidateSearch;
        }

        //[HttpPost]
        public SearchCandidateResponse GetCandidateInfo([FromBody]SearchCandiateRequest searchCandiateRequest)
        {
            SearchCandidateResponse searchCandidateResponse=_iCandidateSearch.SearchCandidate(searchCandiateRequest);
            return searchCandidateResponse;
        }

        [HttpGet]
        public ActionResult Get()
        {
            return Ok("good");
        }
    }
}
