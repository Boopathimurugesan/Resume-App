using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResumeDBTracker.Business.ViewModel
{
    public class SearchCandiateRequest
    {
        public string? Keyword { get; set; }
        public string[]? Locations { get; set; }=new string[0];
        public string[]? Skills { get; set; } = new string[0];
        public string? ExperienceFrom { get; set; }
        public string? ExperienceTo { get; set; }
        public string? EmailAddress { get; set; }
        public Int32 RecordPerPage { get; set; }
        public Int32 CurrentPage { get; set; }
    }
}
