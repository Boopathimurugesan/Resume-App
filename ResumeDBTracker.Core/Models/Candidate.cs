using Newtonsoft.Json;

namespace ResumeDBTracker.Core.Models
{
    public class Candidate
    {
        [JsonProperty(PropertyName = "candidate_id")]
        public object? candidate_id { get; set; }

        [JsonProperty(PropertyName = "resume_id")]
        public object? resume_id { get; set; }

        [JsonProperty(PropertyName = "first_name")]
        public string? first_name { get; set; }

        [JsonProperty(PropertyName = "last_name")]
        public string? last_name { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string? email { get; set; }

        [JsonProperty(PropertyName = "phone_number")]
        public string? phone_number { get; set; }

        [JsonProperty(PropertyName = "location")]
        public string? location { get; set; }

        [JsonProperty(PropertyName = "total_exp")]
        public string? total_exp { get; set; }

        [JsonProperty(PropertyName = "technical_skill")]
        public string? technical_skill { get; set; }

        [JsonProperty(PropertyName = "created_at")]
        public DateTime? created_at { get; set; }

        [JsonProperty(PropertyName = "resume_file")]
        public string resume_file { get; set; }

        [JsonProperty(PropertyName = "file_name")]
        public string file_name { get; set; }
    }
}