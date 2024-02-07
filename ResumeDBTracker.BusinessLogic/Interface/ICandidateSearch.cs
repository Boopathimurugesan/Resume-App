using ResumeDBTracker.Business.ViewModel;
using ResumeDBTracker.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResumeDBTracker.Business.Interface
{
    public interface ICandidateSearch
    {
        SearchCandidateResponse SearchCandidate(SearchCandiateRequest searchCandiateRequest);
    }
}
