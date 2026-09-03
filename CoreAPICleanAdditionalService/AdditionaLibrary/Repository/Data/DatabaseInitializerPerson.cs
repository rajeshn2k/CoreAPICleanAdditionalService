namespace Core.Library.Clean.AdditionalService
{
    public static class DatabaseInitializerPerson
    {
        private static readonly List<Person> persons;
        private static readonly object personRepositoryLock = new object();

        static DatabaseInitializerPerson()
        {
            persons = new List<Person>
            {
                //SPORTS PEOPLE
                new Person() {firstName = "Pete", lastName = "Sampras", rank = 1, category = "Tennis", dateOfBirth = DateTime.Parse("11/11/1981"), isPlayCricket = false, dateCreated = DateTime.Now, Id="5f6730bd-98ae-43d8-b50d-80a2c9eddb8a"},
                new Person() {firstName = "Rahul", lastName = "Dravid",rank = 2, category = "Cricket", dateOfBirth = DateTime.Parse("11/11/1981"), isPlayCricket = true, dateCreated = DateTime.Now, Id="eb3167af-7765-4693-b809-5629539d6330"},
                new Person() {firstName = "Brain", lastName = "Lara",rank = 3, category = "Cricket", dateOfBirth = DateTime.Parse("11/11/1981"), isPlayCricket = true, dateCreated = DateTime.Now, Id="72bdea32-6025-49de-b1e4-a50c8bb6d161"},
                new Person() {firstName = "Roger", lastName = "Federer", rank = 4, category = "Tennis", dateOfBirth = DateTime.Parse("11/11/1981"), isPlayCricket = false, dateCreated = DateTime.Now, Id="0eb4316f-4348-45d7-9d65-06a48aff13a0"},
                new Person() {firstName = "Diego", lastName = "Maradona", rank = 5, category = "Socker", dateOfBirth = DateTime.Parse("11/11/1981"), isPlayCricket = false, dateCreated = DateTime.Now, Id="8c719885-8150-4ad6-9ec9-854affb611cc"},
                new Person() {firstName = "Michael", lastName = "Schumacher", rank = 6, category = "F1", dateOfBirth = DateTime.Parse("11/11/1981"), isPlayCricket = false, dateCreated = DateTime.Now, Id="f2c0b404-01b4-4a9a-aa46-b43e9e630612"},

                //AUTHORS
                new Person() {firstName = "Michelle", lastName = "Obama",rank = 1, category = "Lawyer", dateOfBirth = DateTime.Parse("17 January 1964"), isPlayCricket = false, dateCreated = DateTime.Now, Id="2d10dce3-9bbe-4e63-9fb5-d2ba23b05d48"},
                new Person() {firstName = "Khaled", lastName = "Hosseini", rank = 2, category = "Physician", dateOfBirth = DateTime.Parse("4 March 1965 "), isPlayCricket = false, dateCreated = DateTime.Now, Id="9aa456b2-c0b5-4b58-b831-4d2a042028fc"},
                new Person() {firstName = "Johnny", lastName = "B.Laughing", rank = 3, category = "", dateOfBirth = DateTime.Parse("11/11/1900"), isPlayCricket = false, dateCreated = DateTime.Now, Id="562c1468-64f1-49bc-9f60-a4ae3bd722fc"},
                new Person() {firstName = "Fredrik ", lastName = "Backman", rank = 4, category = "Columnist", dateOfBirth = DateTime.Parse("2 June 1981"), isPlayCricket = false, dateCreated = DateTime.Now, Id="1388f427-fe8e-4e87-b98d-c7ead5eaa2f0"},
                new Person() {firstName = "Chetan", lastName = "Bhagat", rank = 5, category = "Columnist", dateOfBirth = DateTime.Parse("22 April 1974 "), isPlayCricket = false, dateCreated = DateTime.Now, Id="fbde722e-cd91-4004-947e-5d400ca5d951"},
                new Person() {firstName = "Sheryl", lastName = "Sandberg", rank = 6, category = "billionaire technology executive", dateOfBirth = DateTime.Parse("28 August 1969"), isPlayCricket = false, dateCreated = DateTime.Now, Id="67b26599-5bdc-40f9-a5e6-b0c2fe625e9d"},
                new Person() {firstName = "Periyar", lastName = "EVR", rank = 6, category = "social activist", dateOfBirth = DateTime.Parse("17 September 1879"), isPlayCricket = false, dateCreated = DateTime.Now, Id="f62455c6-c79c-4e87-9cb3-5c7424444a7e"},

                //TEST
                new Person() {firstName = "firstName1", lastName = "lastName1", rank = 99, category = "Test Category", dateOfBirth = DateTime.Parse("17 September 1879"), isPlayCricket = false, dateCreated = DateTime.Now, Id="c7509c6f-b114-4f46-9279-3b07ad3995e4"},
                new Person() {firstName = "firstName2", lastName = "lastName2", rank = 99, category = "Test Category", dateOfBirth = DateTime.Parse("17 September 1879"), isPlayCricket = true, dateCreated = DateTime.Now, Id="fa2f9a05-796f-4df2-8d08-cb9c7a301680"},
                new Person() {firstName = "firstName3", lastName = "lastName3", rank = 99, category = "Test Category", dateOfBirth = DateTime.Parse("17 September 1879"), isPlayCricket = false, dateCreated = DateTime.Now, Id="5a07ec64-288b-4895-ac5f-450e55272404"},
                new Person() {firstName = "firstName4", lastName = "lastName4", rank = 99, category = "Test Category", dateOfBirth = DateTime.Parse("17 September 1879"), isPlayCricket = true, dateCreated = DateTime.Now, Id="bbabb461-7df4-4d1f-9104-b433986e2f31"},
                new Person() {firstName = "firstName5", lastName = "lastName5", rank = 99, category = "Test Category", dateOfBirth = DateTime.Parse("17 September 1879"), isPlayCricket = false, dateCreated = DateTime.Now, Id="fffd28e5-c312-4b36-ab47-4b1265ec5825"},
                new Person() {firstName = "firstName6", lastName = "lastName6", rank = 99, category = "Test Category", dateOfBirth = DateTime.Parse("17 September 1879"), isPlayCricket = true, dateCreated = DateTime.Now, Id="5324b5b0-acde-41c2-a740-992e33d5f0bc"},
                new Person() {firstName = "firstName7", lastName = "lastName7", rank = 99, category = "Test Category", dateOfBirth = DateTime.Parse("17 September 1879"), isPlayCricket = false, dateCreated = DateTime.Now, Id="97bf2836-9d1e-49bd-a28e-737c90c24c8e"},
                new Person() {firstName = "firstName8", lastName = "lastName8", rank = 99, category = "Test Category", dateOfBirth = DateTime.Parse("17 September 1879"), isPlayCricket = true, dateCreated = DateTime.Now, Id="80cca7ac-9da3-485b-9e62-ebfcd66a22e1"},
                new Person() {firstName = "firstName9", lastName = "lastName9", rank = 99, category = "Test Category", dateOfBirth = DateTime.Parse("17 September 1879"), isPlayCricket = false, dateCreated = DateTime.Now, Id="f0e6b09a-e6df-4d31-a10c-6acc094fd1d5"},
                new Person() {firstName = "firstName10", lastName = "lastName10", rank = 99, category = "Test Category", dateOfBirth = DateTime.Parse("17 September 1879"), isPlayCricket = true, dateCreated = DateTime.Now, Id="1ce2a126-e11d-4443-8c09-2fcfdf2bca1f"},
                new Person() {firstName = "firstName11", lastName = "lastName11", rank = 99, category = "Test Category", dateOfBirth = DateTime.Parse("17 September 1879"), isPlayCricket = false, dateCreated = DateTime.Now, Id="2c1ffe34-74e8-4ef4-98eb-c8235bd05b81"},
                new Person() {firstName = "firstName12", lastName = "lastName12", rank = 99, category = "Test Category", dateOfBirth = DateTime.Parse("17 September 1879"), isPlayCricket = true, dateCreated = DateTime.Now, Id="b462ebaa-d9a0-4f3e-91ed-e22eac97744b"}
             };
        }

        public static List<Person> GetPersons()
        {
            lock (personRepositoryLock)
            {
                return persons;
            }
        }
    }
}
