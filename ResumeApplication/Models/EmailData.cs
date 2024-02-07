using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ResumeApplication.Models
{
    public class EmailData
    {
        public static string email { get; set; }
        public static string username { get; set; }
        public static int CandidateCount { get; set; }
		public static string first_name { get; set; }
		public static string last_name { get; set; }
        public static int TechnicalSkillCount { get; set; }
        public static int CategoryCount { get; set; }
    }
}