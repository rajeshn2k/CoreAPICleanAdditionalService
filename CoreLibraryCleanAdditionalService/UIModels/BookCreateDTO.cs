#nullable disable

namespace Core.Library.Clean.AdditionalService
{
    public class BookCreateDTO
    {
        public string bookCategory { get; set; }

        public string bookName { get; set; }

        public string edition { get; set; }

        public string image { get; set; }

        public double price { get; set; }
    }
}
