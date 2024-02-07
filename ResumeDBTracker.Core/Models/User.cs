using Newtonsoft.Json;

namespace ResumeDBTracker.Core.Models
{
    public class User
    {
        [JsonProperty(PropertyName = "user_id")]
        public object? user_id { get; set; }

        [JsonProperty(PropertyName = "first_name")]
        public string? first_name { get; set; }

        [JsonProperty(PropertyName = "last_name")]
        public string? last_name { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string? email { get; set; }

        [JsonProperty(PropertyName = "password")]
        public string? password { get; set; }
    }
}