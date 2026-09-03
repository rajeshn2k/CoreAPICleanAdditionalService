namespace Core.Library.Clean.AdditionalService
{
    public class PersonDTO
    {
        public string id { get; set; }

        public string firstName { get; set; }

        public string lastName { get; set; }

        public int rank { get; set; }

        public string category { get; set; }

        public DateTime dateOfBirth { get; set; }

        public bool isPlayCricket { get; set; }

        public DateTime dateCreated { get; set; }

        public ICollection<BookDTO>? Books { get; set; } = new List<BookDTO>();
    }
}