namespace Core.Library.Clean.AdditionalService
{
    public class BookDTO
    {
        public string id { get; set; }

        public string? personName { get; set; }

        public string? personId { get; set; }

        public string bookCategory { get; set; }

        public string bookName { get; set; }

        public string edition { get; set; }

        public string image { get; set; }

        public double price { get; set; }

        public DateTime dateCreated { get; set; }

        public PersonDTO? Author { get; set; }
    }
}
