
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Library.Clean.AdditionalService
{
    public class Book
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("personId")]
        [ForeignKey("personId")]
        public string? personId { get; set; }

        [JsonProperty("bookCategory")]
        public string bookCategory { get; set; }

        [JsonProperty("bookName")]
        public string bookName { get; set; }

        [JsonProperty("edition")]
        public string edition { get; set; }

        [JsonProperty("image")]
        public string image { get; set; }

        [JsonProperty("price")]
        public double price { get; set; }

        [JsonProperty("dateCreated")]
        public DateTime dateCreated { get; set; }
    }
}
