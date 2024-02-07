using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResumeDBTracker.Core.Models
{
	public class Category
	{
		public object category_id { get; set; }
		public string name { get; set;}
		public DateTime updated_at { get; set;}
		public string updated_by { get; set;}

		//this column comes from sp
		public string candidate_count { get; set; }
		
	}
}
