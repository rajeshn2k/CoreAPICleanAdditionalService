namespace Core.Library.Clean.AdditionalService
{
    public class PersonCreateDTO
    {
        public string firstName { get; set; }

        public string lastName { get; set; }

        public int rank { get; set; }

        public string category { get; set; }

        public DateTime dateOfBirth { get; set; }

        public bool isPlaySports { get; set; }
    }
}