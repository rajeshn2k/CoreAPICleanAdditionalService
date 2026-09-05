using Newtonsoft.Json;

namespace Core.Library.Clean.AdditionalService
{
    public class Person
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("firstName")]
        public string firstName { get; set; }

        [JsonProperty("lastName")]
        public string lastName { get; set; }

        [JsonProperty("rank")]
        public int rank { get; set; }

        [JsonProperty("category")]
        public string category { get; set; }

        [JsonProperty("dateOfBirth")]
        public DateTime dateOfBirth { get; set; }

        [JsonProperty("isPlaySports")]
        public bool isPlaySports { get; set; }

        [JsonProperty("dateCreated")]
        public DateTime dateCreated { get; set; }
    }
}