
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Library.Clean.AdditionalService
{
    public class Book
    {
        public string Id { get; set; }

        [ForeignKey("personId")]
        public string? personId { get; set; }

        public string bookCategory { get; set; }

        public string bookName { get; set; }

        public string edition { get; set; }

        public string image { get; set; }

        public double price { get; set; }

        public DateTime dateCreated { get; set; }
    }
}
