using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResumeDBTracker.Core.Models
{
	public class CategoryCandidateMapping
	{
		public string category_id { get; set; }
		public string candidate_id { get; set; }
		public string name { get; set; }
		public DateTime updated_at { get; set; }
		public string updated_by { get; set; }

		// Extra columns
		public string resume_id { get; set; }
		public string first_name { get; set; }
		public string last_name { get; set; }
		public string email { get; set; }
		public string phone_number { get; set; }
		public string location { get; set; }
		public string total_exp { get; set; }
		public string technical_skill { get; set; }
		




	}
}
