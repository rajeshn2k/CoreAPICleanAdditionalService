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
                // SPORTS
                new Person() {firstName = "Pete", lastName = "Sampras", rank = 1, category = "Tennis", dateOfBirth = DateTime.Parse("11/11/1981"), isPlaySports = true, dateCreated = DateTime.Now, Id="5f6730bd-98ae-43d8-b50d-80a2c9eddb8a"},
                new Person() {firstName = "Rahul", lastName = "Dravid",rank = 2, category = "Cricket", dateOfBirth = DateTime.Parse("11/11/1981"), isPlaySports = true, dateCreated = DateTime.Now, Id="eb3167af-7765-4693-b809-5629539d6330"},
                new Person() {firstName = "Brain", lastName = "Lara",rank = 3, category = "Cricket", dateOfBirth = DateTime.Parse("11/11/1981"), isPlaySports = true, dateCreated = DateTime.Now, Id="72bdea32-6025-49de-b1e4-a50c8bb6d161"},
                new Person() {firstName = "Roger", lastName = "Federer", rank = 4, category = "Tennis", dateOfBirth = DateTime.Parse("11/11/1981"), isPlaySports = true, dateCreated = DateTime.Now, Id="0eb4316f-4348-45d7-9d65-06a48aff13a0"},
                new Person() {firstName = "Diego", lastName = "Maradona", rank = 5, category = "Soccer", dateOfBirth = DateTime.Parse("11/11/1981"), isPlaySports = true, dateCreated = DateTime.Now, Id="8c719885-8150-4ad6-9ec9-854affb611cc"},
                new Person() {firstName = "Michael", lastName = "Schumacher", rank = 6, category = "F1", dateOfBirth = DateTime.Parse("11/11/1981"), isPlaySports = true, dateCreated = DateTime.Now, Id="f2c0b404-01b4-4a9a-aa46-b43e9e630612"},
                new Person() {firstName = "Novak", lastName = "Djokovic", rank = 7, category = "Tennis", dateOfBirth = DateTime.Parse("22 May 1987"), isPlaySports = true, dateCreated = DateTime.Now, Id="a1c2d3e4-f567-4890-ab12-34567890c001"},
                new Person() {firstName = "Rafael", lastName = "Nadal", rank = 8, category = "Tennis", dateOfBirth = DateTime.Parse("3 June 1986"), isPlaySports = true, dateCreated = DateTime.Now, Id="b2d3e4f5-a678-4901-bc23-45678901c002"},
                new Person() {firstName = "Serena", lastName = "Williams", rank = 9, category = "Tennis", dateOfBirth = DateTime.Parse("26 September 1981"), isPlaySports = true, dateCreated = DateTime.Now, Id="c3e4f5a6-b789-4012-cd34-56789012c003"},
                new Person() {firstName = "Sachin", lastName = "Tendulkar", rank = 10, category = "Cricket", dateOfBirth = DateTime.Parse("24 April 1973"), isPlaySports = true, dateCreated = DateTime.Now, Id="d4f5a6b7-c890-4123-de45-67890123c004"},
                new Person() {firstName = "Virat", lastName = "Kohli", rank = 11, category = "Cricket", dateOfBirth = DateTime.Parse("5 November 1988"), isPlaySports = true, dateCreated = DateTime.Now, Id="e5a6b7c8-d901-4234-ef56-78901234c005"},
                new Person() {firstName = "MS", lastName = "Dhoni", rank = 12, category = "Cricket", dateOfBirth = DateTime.Parse("7 July 1981"), isPlaySports = true, dateCreated = DateTime.Now, Id="f6b7c8d9-e012-4345-f067-89012345c006"},
                new Person() {firstName = "Brian", lastName = "Lara", rank = 13, category = "Cricket", dateOfBirth = DateTime.Parse("2 May 1969"), isPlaySports = true, dateCreated = DateTime.Now, Id="07c8d9e0-f123-4456-0178-90123456c007"},
                new Person() {firstName = "Wasim", lastName = "Akram", rank = 14, category = "Cricket", dateOfBirth = DateTime.Parse("3 June 1966"), isPlaySports = true, dateCreated = DateTime.Now, Id="18d9e0f1-0234-4567-1289-01234567c008"},
                new Person() {firstName = "Jacques", lastName = "Kallis", rank = 15, category = "Cricket", dateOfBirth = DateTime.Parse("16 October 1975"), isPlaySports = true, dateCreated = DateTime.Now, Id="29e0f1a2-1345-4678-2390-12345678c009"},
                new Person() {firstName = "Shane", lastName = "Warne", rank = 16, category = "Cricket", dateOfBirth = DateTime.Parse("13 September 1969"), isPlaySports = true, dateCreated = DateTime.Now, Id="3af1a2b3-2456-4789-3401-23456789c010"},
                new Person() {firstName = "Lionel", lastName = "Messi", rank = 17, category = "Soccer", dateOfBirth = DateTime.Parse("24 June 1987"), isPlaySports = true, dateCreated = DateTime.Now, Id="4ba2b3c4-3567-4890-4512-34567890c011"},
                new Person() {firstName = "Cristiano", lastName = "Ronaldo", rank = 18, category = "Soccer", dateOfBirth = DateTime.Parse("5 February 1985"), isPlaySports = true, dateCreated = DateTime.Now, Id="5cb3c4d5-4678-4901-5623-45678901c012"},
                new Person() {firstName = "Pele", lastName = "Nascimento", rank = 19, category = "Soccer", dateOfBirth = DateTime.Parse("23 October 1940"), isPlaySports = true, dateCreated = DateTime.Now, Id="6dc4d5e6-5789-4012-6734-56789012c013"},
                new Person() {firstName = "Zinedine", lastName = "Zidane", rank = 20, category = "Soccer", dateOfBirth = DateTime.Parse("23 June 1972"), isPlaySports = true, dateCreated = DateTime.Now, Id="7ed5e6f7-6890-4123-7845-67890123c014"},
                new Person() {firstName = "David", lastName = "Beckham", rank = 21, category = "Soccer", dateOfBirth = DateTime.Parse("2 May 1975"), isPlaySports = true, dateCreated = DateTime.Now, Id="8fe6f708-7901-4234-8956-78901234c015"},
                new Person() {firstName = "Michael", lastName = "Jordan", rank = 22, category = "Basketball", dateOfBirth = DateTime.Parse("17 February 1963"), isPlaySports = true, dateCreated = DateTime.Now, Id="90f70819-8012-4345-9067-89012345c016"},
                new Person() {firstName = "LeBron", lastName = "James", rank = 23, category = "Basketball", dateOfBirth = DateTime.Parse("30 December 1984"), isPlaySports = true, dateCreated = DateTime.Now, Id="a108192a-9123-4456-a178-90123456c017"},
                new Person() {firstName = "Kobe", lastName = "Bryant", rank = 24, category = "Basketball", dateOfBirth = DateTime.Parse("23 August 1978"), isPlaySports = true, dateCreated = DateTime.Now, Id="b2192a3b-0234-4567-b289-01234567c018"},
                new Person() {firstName = "Usain", lastName = "Bolt", rank = 25, category = "Athletics", dateOfBirth = DateTime.Parse("21 August 1986"), isPlaySports = true, dateCreated = DateTime.Now, Id="c32a3b4c-1345-4678-c390-12345678c019"},
                new Person() {firstName = "Lewis", lastName = "Hamilton", rank = 26, category = "F1", dateOfBirth = DateTime.Parse("7 January 1985"), isPlaySports = true, dateCreated = DateTime.Now, Id="d43b4c5d-2456-4789-d401-23456789c020"},
                new Person() {firstName = "Ayrton", lastName = "Senna", rank = 27, category = "F1", dateOfBirth = DateTime.Parse("21 March 1960"), isPlaySports = true, dateCreated = DateTime.Now, Id="e54c5d6e-3567-4890-e512-34567890c021"},
                new Person() {firstName = "Max", lastName = "Verstappen", rank = 28, category = "F1", dateOfBirth = DateTime.Parse("30 September 1997"), isPlaySports = true, dateCreated = DateTime.Now, Id="f65d6e7f-4678-4901-f623-45678901c022"},
                new Person() {firstName = "Tiger", lastName = "Woods", rank = 29, category = "Golf", dateOfBirth = DateTime.Parse("30 December 1975"), isPlaySports = true, dateCreated = DateTime.Now, Id="076e7f80-5789-4012-0734-56789012c023"},
                new Person() {firstName = "Roger", lastName = "Bannister", rank = 30, category = "Athletics", dateOfBirth = DateTime.Parse("23 March 1929"), isPlaySports = true, dateCreated = DateTime.Now, Id="187f8091-6890-4123-1845-67890123c024"},
                new Person() {firstName = "Jack", lastName = "Nicklaus", rank = 31, category = "Golf", dateOfBirth = DateTime.Parse("21 January 1940"), isPlaySports = true, dateCreated = DateTime.Now, Id="298091a2-7901-4234-2956-78901234c025"},
                new Person() {firstName = "Muhammad", lastName = "Ali", rank = 32, category = "Boxing", dateOfBirth = DateTime.Parse("17 January 1942"), isPlaySports = true, dateCreated = DateTime.Now, Id="3a91a2b3-8012-4345-3a67-89012345c026"},
                new Person() {firstName = "Mike", lastName = "Tyson", rank = 33, category = "Boxing", dateOfBirth = DateTime.Parse("30 June 1966"), isPlaySports = true, dateCreated = DateTime.Now, Id="4ba2b3c4-9123-4456-4b78-90123456c027"},
                new Person() {firstName = "Manny", lastName = "Pacquiao", rank = 34, category = "Boxing", dateOfBirth = DateTime.Parse("17 December 1978"), isPlaySports = true, dateCreated = DateTime.Now, Id="5bc3c4d5-0234-4567-5c89-01234567c028"},
                new Person() {firstName = "Tom", lastName = "Brady", rank = 35, category = "American Football", dateOfBirth = DateTime.Parse("3 August 1977"), isPlaySports = true, dateCreated = DateTime.Now, Id="6cd4d5e6-1345-4678-6d90-12345678c029"},
                new Person() {firstName = "Simone", lastName = "Biles", rank = 36, category = "Gymnastics", dateOfBirth = DateTime.Parse("14 March 1997"), isPlaySports = true, dateCreated = DateTime.Now, Id="7de5e6f7-2456-4789-7e01-23456789c030"},
                new Person() {firstName = "Michael", lastName = "Phelps", rank = 37, category = "Swimming", dateOfBirth = DateTime.Parse("30 June 1985"), isPlaySports = true, dateCreated = DateTime.Now, Id="8ef6f708-3567-4890-8f12-34567890c031"},
                new Person() {firstName = "Mary", lastName = "Kom", rank = 38, category = "Boxing", dateOfBirth = DateTime.Parse("24 November 1982"), isPlaySports = true, dateCreated = DateTime.Now, Id="9f070819-4678-4901-9023-45678901c032"},
                new Person() {firstName = "Neeraj", lastName = "Chopra", rank = 39, category = "Athletics", dateOfBirth = DateTime.Parse("24 December 1997"), isPlaySports = true, dateCreated = DateTime.Now, Id="a018192a-5789-4012-a134-56789012c033"},
                new Person() {firstName = "Kapil", lastName = "Dev", rank = 40, category = "Cricket", dateOfBirth = DateTime.Parse("6 January 1959"), isPlaySports = true, dateCreated = DateTime.Now, Id="b1292a3b-6890-4123-b245-67890123c034"},

                // AUTHORS
                new Person() {firstName = "Michelle", lastName = "Obama",rank = 1, category = "Lawyer", dateOfBirth = DateTime.Parse("17 January 1964"), isPlaySports = false, dateCreated = DateTime.Now, Id="2d10dce3-9bbe-4e63-9fb5-d2ba23b05d48"},
                new Person() {firstName = "Khaled", lastName = "Hosseini", rank = 2, category = "Physician", dateOfBirth = DateTime.Parse("4 March 1965 "), isPlaySports = false, dateCreated = DateTime.Now, Id="9aa456b2-c0b5-4b58-b831-4d2a042028fc"},
                new Person() {firstName = "Johnny", lastName = "B.Laughing", rank = 3, category = "", dateOfBirth = DateTime.Parse("11/11/1900"), isPlaySports = false, dateCreated = DateTime.Now, Id="562c1468-64f1-49bc-9f60-a4ae3bd722fc"},
                new Person() {firstName = "Fredrik ", lastName = "Backman", rank = 4, category = "Columnist", dateOfBirth = DateTime.Parse("2 June 1981"), isPlaySports = false, dateCreated = DateTime.Now, Id="1388f427-fe8e-4e87-b98d-c7ead5eaa2f0"},
                new Person() {firstName = "Chetan", lastName = "Bhagat", rank = 5, category = "Columnist", dateOfBirth = DateTime.Parse("22 April 1974 "), isPlaySports = false, dateCreated = DateTime.Now, Id="fbde722e-cd91-4004-947e-5d400ca5d951"},
                new Person() {firstName = "Sheryl", lastName = "Sandberg", rank = 6, category = "billionaire technology executive", dateOfBirth = DateTime.Parse("28 August 1969"), isPlaySports = false, dateCreated = DateTime.Now, Id="67b26599-5bdc-40f9-a5e6-b0c2fe625e9d"},
                new Person() {firstName = "Periyar", lastName = "EVR", rank = 6, category = "social activist", dateOfBirth = DateTime.Parse("17 September 1879"), isPlaySports = false, dateCreated = DateTime.Now, Id="f62455c6-c79c-4e87-9cb3-5c7424444a7e"},
                new Person() {firstName = "J.K.", lastName = "Rowling", rank = 7, category = "Author", dateOfBirth = DateTime.Parse("31 July 1965"), isPlaySports = false, dateCreated = DateTime.Now, Id="c23a3b4c-7901-4234-c356-78901234c035"},
                new Person() {firstName = "George", lastName = "Orwell", rank = 8, category = "Author", dateOfBirth = DateTime.Parse("25 June 1903"), isPlaySports = false, dateCreated = DateTime.Now, Id="d34b4c5d-8012-4345-d467-89012345c036"},
                new Person() {firstName = "Mark", lastName = "Twain", rank = 9, category = "Author", dateOfBirth = DateTime.Parse("30 November 1835"), isPlaySports = false, dateCreated = DateTime.Now, Id="e45c5d6e-9123-4456-e578-90123456c037"},
                new Person() {firstName = "Ernest", lastName = "Hemingway", rank = 10, category = "Author", dateOfBirth = DateTime.Parse("21 July 1899"), isPlaySports = false, dateCreated = DateTime.Now, Id="f56d6e7f-0234-4567-f689-01234567c038"},
                new Person() {firstName = "Leo", lastName = "Tolstoy", rank = 11, category = "Author", dateOfBirth = DateTime.Parse("9 September 1828"), isPlaySports = false, dateCreated = DateTime.Now, Id="067e7f80-1345-4678-0780-12345678c039"},
                new Person() {firstName = "Fyodor", lastName = "Dostoevsky", rank = 12, category = "Author", dateOfBirth = DateTime.Parse("11 November 1821"), isPlaySports = false, dateCreated = DateTime.Now, Id="178f8091-2456-4789-1891-23456789c040"},
                new Person() {firstName = "William", lastName = "Shakespeare", rank = 13, category = "Playwright", dateOfBirth = DateTime.Parse("23 April 1564"), isPlaySports = false, dateCreated = DateTime.Now, Id="289091a2-3567-4890-2902-34567890c041"},
                new Person() {firstName = "Jane", lastName = "Austen", rank = 14, category = "Author", dateOfBirth = DateTime.Parse("16 December 1775"), isPlaySports = false, dateCreated = DateTime.Now, Id="39a1a2b3-4678-4901-3a13-45678901c042"},
                new Person() {firstName = "Charles", lastName = "Dickens", rank = 15, category = "Author", dateOfBirth = DateTime.Parse("7 February 1812"), isPlaySports = false, dateCreated = DateTime.Now, Id="4ab2b3c4-5789-4012-4b24-56789012c043"},
                new Person() {firstName = "Victor", lastName = "Hugo", rank = 16, category = "Author", dateOfBirth = DateTime.Parse("26 February 1802"), isPlaySports = false, dateCreated = DateTime.Now, Id="5bc3c4d5-6890-4123-5c35-67890123c044"},
                new Person() {firstName = "Gabriel", lastName = "Marquez", rank = 17, category = "Author", dateOfBirth = DateTime.Parse("6 March 1927"), isPlaySports = false, dateCreated = DateTime.Now, Id="6cd4d5e6-7901-4234-6d46-78901234c045"},
                new Person() {firstName = "Paulo", lastName = "Coelho", rank = 18, category = "Author", dateOfBirth = DateTime.Parse("24 August 1947"), isPlaySports = false, dateCreated = DateTime.Now, Id="7de5e6f7-8012-4345-7e57-89012345c046"},
                new Person() {firstName = "Dan", lastName = "Brown", rank = 19, category = "Author", dateOfBirth = DateTime.Parse("22 June 1964"), isPlaySports = false, dateCreated = DateTime.Now, Id="8ef6f708-9123-4456-8f68-90123456c047"},
                new Person() {firstName = "Stephen", lastName = "King", rank = 20, category = "Author", dateOfBirth = DateTime.Parse("21 September 1947"), isPlaySports = false, dateCreated = DateTime.Now, Id="9f070819-0234-4567-9079-01234567c048"},
                new Person() {firstName = "Agatha", lastName = "Christie", rank = 21, category = "Author", dateOfBirth = DateTime.Parse("15 September 1890"), isPlaySports = false, dateCreated = DateTime.Now, Id="a018192a-1345-4678-a180-12345678c049"},
                new Person() {firstName = "Arthur", lastName = "Doyle", rank = 22, category = "Author", dateOfBirth = DateTime.Parse("22 May 1859"), isPlaySports = false, dateCreated = DateTime.Now, Id="b1292a3b-2456-4789-b291-23456789c050"},
                new Person() {firstName = "R.K.", lastName = "Narayan", rank = 23, category = "Author", dateOfBirth = DateTime.Parse("10 October 1906"), isPlaySports = false, dateCreated = DateTime.Now, Id="c23a3b4c-3567-4890-c3a2-34567890c051"},
                new Person() {firstName = "Rabindranath", lastName = "Tagore", rank = 24, category = "Poet", dateOfBirth = DateTime.Parse("7 May 1861"), isPlaySports = false, dateCreated = DateTime.Now, Id="d34b4c5d-4678-4901-d4b3-45678901c052"},
                new Person() {firstName = "Arundhati", lastName = "Roy", rank = 25, category = "Author", dateOfBirth = DateTime.Parse("24 November 1961"), isPlaySports = false, dateCreated = DateTime.Now, Id="e45c5d6e-5789-4012-e5c4-56789012c053"},
                new Person() {firstName = "Ruskin", lastName = "Bond", rank = 26, category = "Author", dateOfBirth = DateTime.Parse("19 May 1934"), isPlaySports = false, dateCreated = DateTime.Now, Id="f56d6e7f-6890-4123-f6d5-67890123c054"},
                new Person() {firstName = "Amish", lastName = "Tripathi", rank = 27, category = "Author", dateOfBirth = DateTime.Parse("15 October 1974"), isPlaySports = false, dateCreated = DateTime.Now, Id="067e7f80-7901-4234-07e6-78901234c055"},
                new Person() {firstName = "Vikram", lastName = "Seth", rank = 28, category = "Author", dateOfBirth = DateTime.Parse("20 June 1952"), isPlaySports = false, dateCreated = DateTime.Now, Id="178f8091-8012-4345-18f7-89012345c056"},
                new Person() {firstName = "Jhumpa", lastName = "Lahiri", rank = 29, category = "Author", dateOfBirth = DateTime.Parse("11 July 1967"), isPlaySports = false, dateCreated = DateTime.Now, Id="289091a2-9123-4456-2908-90123456c057"},
                new Person() {firstName = "Salman", lastName = "Rushdie", rank = 30, category = "Author", dateOfBirth = DateTime.Parse("19 June 1947"), isPlaySports = false, dateCreated = DateTime.Now, Id="39a1a2b3-0234-4567-3a19-01234567c058"},
                new Person() {firstName = "Neil", lastName = "Gaiman", rank = 31, category = "Author", dateOfBirth = DateTime.Parse("10 November 1960"), isPlaySports = false, dateCreated = DateTime.Now, Id="4ab2b3c4-1345-4678-4b20-12345678c059"},
                new Person() {firstName = "Haruki", lastName = "Murakami", rank = 32, category = "Author", dateOfBirth = DateTime.Parse("12 January 1949"), isPlaySports = false, dateCreated = DateTime.Now, Id="5bc3c4d5-2456-4789-5c31-23456789c060"},
                new Person() {firstName = "Kazuo", lastName = "Ishiguro", rank = 33, category = "Author", dateOfBirth = DateTime.Parse("8 November 1954"), isPlaySports = false, dateCreated = DateTime.Now, Id="6cd4d5e6-3567-4890-6d42-34567890c061"},
                new Person() {firstName = "Margaret", lastName = "Atwood", rank = 34, category = "Author", dateOfBirth = DateTime.Parse("18 November 1939"), isPlaySports = false, dateCreated = DateTime.Now, Id="7de5e6f7-4678-4901-7e53-45678901c062"},
                new Person() {firstName = "Toni", lastName = "Morrison", rank = 35, category = "Author", dateOfBirth = DateTime.Parse("18 February 1931"), isPlaySports = false, dateCreated = DateTime.Now, Id="8ef6f708-5789-4012-8f64-56789012c063"},
                new Person() {firstName = "Maya", lastName = "Angelou", rank = 36, category = "Poet", dateOfBirth = DateTime.Parse("4 April 1928"), isPlaySports = false, dateCreated = DateTime.Now, Id="9f070819-6890-4123-9f75-67890123c064"},
                new Person() {firstName = "Sylvia", lastName = "Plath", rank = 37, category = "Poet", dateOfBirth = DateTime.Parse("27 October 1932"), isPlaySports = false, dateCreated = DateTime.Now, Id="a018192a-7901-4234-a086-78901234c065"},
                new Person() {firstName = "Oscar", lastName = "Wilde", rank = 38, category = "Author", dateOfBirth = DateTime.Parse("16 October 1854"), isPlaySports = false, dateCreated = DateTime.Now, Id="b1292a3b-8012-4345-b197-89012345c066"},
                new Person() {firstName = "Virginia", lastName = "Woolf", rank = 39, category = "Author", dateOfBirth = DateTime.Parse("25 January 1882"), isPlaySports = false, dateCreated = DateTime.Now, Id="c23a3b4c-9123-4456-c2a8-90123456c067"},
                new Person() {firstName = "F. Scott", lastName = "Fitzgerald", rank = 40, category = "Author", dateOfBirth = DateTime.Parse("24 September 1896"), isPlaySports = false, dateCreated = DateTime.Now, Id="d34b4c5d-0234-4567-d3b9-01234567c068"},

                // PUBLIC FIGURES
                //new Person() {firstName = "Albert", lastName = "Einstein", rank = 41, category = "Scientist", dateOfBirth = DateTime.Parse("14 March 1879"), isPlaySports = false, dateCreated = DateTime.Now, Id="e45c5d6e-1345-4678-e4ca-12345678c069"},
                //new Person() {firstName = "Isaac", lastName = "Newton", rank = 42, category = "Scientist", dateOfBirth = DateTime.Parse("4 January 1643"), isPlaySports = false, dateCreated = DateTime.Now, Id="f56d6e7f-2456-4789-f5db-23456789c070"},
                //new Person() {firstName = "Nikola", lastName = "Tesla", rank = 43, category = "Inventor", dateOfBirth = DateTime.Parse("10 July 1856"), isPlaySports = false, dateCreated = DateTime.Now, Id="067e7f80-3567-4890-06ec-34567890c071"},
                //new Person() {firstName = "Thomas", lastName = "Edison", rank = 44, category = "Inventor", dateOfBirth = DateTime.Parse("11 February 1847"), isPlaySports = false, dateCreated = DateTime.Now, Id="178f8091-4678-4901-17fd-45678901c072"},
                //new Person() {firstName = "Leonardo", lastName = "DaVinci", rank = 45, category = "Artist", dateOfBirth = DateTime.Parse("15 April 1452"), isPlaySports = false, dateCreated = DateTime.Now, Id="289091a2-5789-4012-280e-56789012c073"},
                //new Person() {firstName = "Pablo", lastName = "Picasso", rank = 46, category = "Artist", dateOfBirth = DateTime.Parse("25 October 1881"), isPlaySports = false, dateCreated = DateTime.Now, Id="39a1a2b3-6890-4123-391f-67890123c074"},
                //new Person() {firstName = "Vincent", lastName = "VanGogh", rank = 47, category = "Artist", dateOfBirth = DateTime.Parse("30 March 1853"), isPlaySports = false, dateCreated = DateTime.Now, Id="4ab2b3c4-7901-4234-4a20-78901234c075"},
                //new Person() {firstName = "Steve", lastName = "Jobs", rank = 48, category = "Entrepreneur", dateOfBirth = DateTime.Parse("24 February 1955"), isPlaySports = false, dateCreated = DateTime.Now, Id="5bc3c4d5-8012-4345-5b31-89012345c076"},
                //new Person() {firstName = "Bill", lastName = "Gates", rank = 49, category = "Entrepreneur", dateOfBirth = DateTime.Parse("28 October 1955"), isPlaySports = false, dateCreated = DateTime.Now, Id="6cd4d5e6-9123-4456-6c42-90123456c077"},
                //new Person() {firstName = "Elon", lastName = "Musk", rank = 50, category = "Entrepreneur", dateOfBirth = DateTime.Parse("28 June 1971"), isPlaySports = false, dateCreated = DateTime.Now, Id="7de5e6f7-0234-4567-7d53-01234567c078"},
                //new Person() {firstName = "Jeff", lastName = "Bezos", rank = 51, category = "Entrepreneur", dateOfBirth = DateTime.Parse("12 January 1964"), isPlaySports = false, dateCreated = DateTime.Now, Id="8ef6f708-1345-4678-8e64-12345678c079"},
                //new Person() {firstName = "Warren", lastName = "Buffett", rank = 52, category = "Investor", dateOfBirth = DateTime.Parse("30 August 1930"), isPlaySports = false, dateCreated = DateTime.Now, Id="9f070819-2456-4789-9f75-23456789c080"},
                //new Person() {firstName = "Oprah", lastName = "Winfrey", rank = 53, category = "Media", dateOfBirth = DateTime.Parse("29 January 1954"), isPlaySports = false, dateCreated = DateTime.Now, Id="a018192a-3567-4890-a086-34567890c081"},
                //new Person() {firstName = "Walt", lastName = "Disney", rank = 54, category = "Animator", dateOfBirth = DateTime.Parse("5 December 1901"), isPlaySports = false, dateCreated = DateTime.Now, Id="b1292a3b-4678-4901-b197-45678901c082"},
                //new Person() {firstName = "Charlie", lastName = "Chaplin", rank = 55, category = "Actor", dateOfBirth = DateTime.Parse("16 April 1889"), isPlaySports = false, dateCreated = DateTime.Now, Id="c23a3b4c-5789-4012-c2a8-56789012c083"},
                //new Person() {firstName = "Marilyn", lastName = "Monroe", rank = 56, category = "Actor", dateOfBirth = DateTime.Parse("1 June 1926"), isPlaySports = false, dateCreated = DateTime.Now, Id="d34b4c5d-6890-4123-d3b9-67890123c084"},
                //new Person() {firstName = "Audrey", lastName = "Hepburn", rank = 57, category = "Actor", dateOfBirth = DateTime.Parse("4 May 1929"), isPlaySports = false, dateCreated = DateTime.Now, Id="e45c5d6e-7901-4234-e4ca-78901234c085"},
                //new Person() {firstName = "Amitabh", lastName = "Bachchan", rank = 58, category = "Actor", dateOfBirth = DateTime.Parse("11 October 1942"), isPlaySports = false, dateCreated = DateTime.Now, Id="f56d6e7f-8012-4345-f5db-89012345c086"},
                //new Person() {firstName = "Rajinikanth", lastName = "Shivaji", rank = 59, category = "Actor", dateOfBirth = DateTime.Parse("12 December 1950"), isPlaySports = false, dateCreated = DateTime.Now, Id="067e7f80-9123-4456-06ec-90123456c087"},
                //new Person() {firstName = "A.R.", lastName = "Rahman", rank = 60, category = "Musician", dateOfBirth = DateTime.Parse("6 January 1967"), isPlaySports = false, dateCreated = DateTime.Now, Id="178f8091-0234-4567-17fd-01234567c088"},
                //new Person() {firstName = "Lata", lastName = "Mangeshkar", rank = 61, category = "Singer", dateOfBirth = DateTime.Parse("28 September 1929"), isPlaySports = false, dateCreated = DateTime.Now, Id="289091a2-1345-4678-280e-12345678c089"},
                //new Person() {firstName = "Michael", lastName = "Jackson", rank = 62, category = "Singer", dateOfBirth = DateTime.Parse("29 August 1958"), isPlaySports = false, dateCreated = DateTime.Now, Id="39a1a2b3-2456-4789-391f-23456789c090"},
                //new Person() {firstName = "Elvis", lastName = "Presley", rank = 63, category = "Singer", dateOfBirth = DateTime.Parse("8 January 1935"), isPlaySports = false, dateCreated = DateTime.Now, Id="4ab2b3c4-3567-4890-4a20-34567890c091"},
                //new Person() {firstName = "Freddie", lastName = "Mercury", rank = 64, category = "Singer", dateOfBirth = DateTime.Parse("5 September 1946"), isPlaySports = false, dateCreated = DateTime.Now, Id="5bc3c4d5-4678-4901-5b31-45678901c092"},
                //new Person() {firstName = "Bob", lastName = "Dylan", rank = 65, category = "Musician", dateOfBirth = DateTime.Parse("24 May 1941"), isPlaySports = false, dateCreated = DateTime.Now, Id="6cd4d5e6-5789-4012-6c42-56789012c093"},
                //new Person() {firstName = "John", lastName = "Lennon", rank = 66, category = "Musician", dateOfBirth = DateTime.Parse("9 October 1940"), isPlaySports = false, dateCreated = DateTime.Now, Id="7de5e6f7-6890-4123-7d53-67890123c094"},
                //new Person() {firstName = "Paul", lastName = "McCartney", rank = 67, category = "Musician", dateOfBirth = DateTime.Parse("18 June 1942"), isPlaySports = false, dateCreated = DateTime.Now, Id="8ef6f708-7901-4234-8e64-78901234c095"},
                //new Person() {firstName = "Martin", lastName = "LutherKing", rank = 68, category = "Activist", dateOfBirth = DateTime.Parse("15 January 1929"), isPlaySports = false, dateCreated = DateTime.Now, Id="9f070819-8012-4345-9f75-89012345c096"},
                //new Person() {firstName = "Nelson", lastName = "Mandela", rank = 69, category = "Activist", dateOfBirth = DateTime.Parse("18 July 1918"), isPlaySports = false, dateCreated = DateTime.Now, Id="a018192a-9123-4456-a086-90123456c097"},
                //new Person() {firstName = "Mother", lastName = "Teresa", rank = 70, category = "Humanitarian", dateOfBirth = DateTime.Parse("26 August 1910"), isPlaySports = false, dateCreated = DateTime.Now, Id="b1292a3b-0234-4567-b197-01234567c098"},
                //new Person() {firstName = "Mahatma", lastName = "Gandhi", rank = 71, category = "Activist", dateOfBirth = DateTime.Parse("2 October 1869"), isPlaySports = false, dateCreated = DateTime.Now, Id="c23a3b4c-1345-4678-c2a8-12345678c099"},
                //new Person() {firstName = "Abraham", lastName = "Lincoln", rank = 72, category = "President", dateOfBirth = DateTime.Parse("12 February 1809"), isPlaySports = false, dateCreated = DateTime.Now, Id="d34b4c5d-2456-4789-d3b9-23456789c100"},
                //new Person() {firstName = "George", lastName = "Washington", rank = 73, category = "President", dateOfBirth = DateTime.Parse("22 February 1732"), isPlaySports = false, dateCreated = DateTime.Now, Id="e45c5d6e-3567-4890-e4ca-34567890c101"},
                //new Person() {firstName = "Winston", lastName = "Churchill", rank = 74, category = "Politician", dateOfBirth = DateTime.Parse("30 November 1874"), isPlaySports = false, dateCreated = DateTime.Now, Id="f56d6e7f-4678-4901-f5db-45678901c102"},
                //new Person() {firstName = "Barack", lastName = "Obama", rank = 75, category = "Politician", dateOfBirth = DateTime.Parse("4 August 1961"), isPlaySports = false, dateCreated = DateTime.Now, Id="067e7f80-5789-4012-06ec-56789012c103"},
                //new Person() {firstName = "Bill", lastName = "Clinton", rank = 76, category = "Politician", dateOfBirth = DateTime.Parse("19 August 1946"), isPlaySports = false, dateCreated = DateTime.Now, Id="178f8091-6890-4123-17fd-67890123c104"},
                //new Person() {firstName = "Abdul", lastName = "Kalam", rank = 77, category = "Scientist", dateOfBirth = DateTime.Parse("15 October 1931"), isPlaySports = false, dateCreated = DateTime.Now, Id="289091a2-7901-4234-280e-78901234c105"},
                //new Person() {firstName = "C.V.", lastName = "Raman", rank = 78, category = "Scientist", dateOfBirth = DateTime.Parse("7 November 1888"), isPlaySports = false, dateCreated = DateTime.Now, Id="39a1a2b3-8012-4345-391f-89012345c106"},
                //new Person() {firstName = "Srinivasa", lastName = "Ramanujan", rank = 79, category = "Mathematician", dateOfBirth = DateTime.Parse("22 December 1887"), isPlaySports = false, dateCreated = DateTime.Now, Id="4ab2b3c4-9123-4456-4a20-90123456c107"},
                //new Person() {firstName = "Stephen", lastName = "Hawking", rank = 80, category = "Scientist", dateOfBirth = DateTime.Parse("8 January 1942"), isPlaySports = false, dateCreated = DateTime.Now, Id="5bc3c4d5-0234-4567-5b31-01234567c108"},
                //new Person() {firstName = "Richard", lastName = "Feynman", rank = 81, category = "Scientist", dateOfBirth = DateTime.Parse("11 May 1918"), isPlaySports = false, dateCreated = DateTime.Now, Id="6cd4d5e6-1345-4678-6c42-12345678c109"},
                //new Person() {firstName = "Marie", lastName = "Curie", rank = 82, category = "Scientist", dateOfBirth = DateTime.Parse("7 November 1867"), isPlaySports = false, dateCreated = DateTime.Now, Id="7de5e6f7-2456-4789-7d53-23456789c110"},
                //new Person() {firstName = "Charles", lastName = "Darwin", rank = 83, category = "Scientist", dateOfBirth = DateTime.Parse("12 February 1809"), isPlaySports = false, dateCreated = DateTime.Now, Id="8ef6f708-3567-4890-8e64-34567890c111"},
                //new Person() {firstName = "Galileo", lastName = "Galilei", rank = 84, category = "Scientist", dateOfBirth = DateTime.Parse("15 February 1564"), isPlaySports = false, dateCreated = DateTime.Now, Id="9f070819-4678-4901-9f75-45678901c112"},
                //new Person() {firstName = "Plato", lastName = "Philosopher", rank = 85, category = "Philosopher", dateOfBirth = DateTime.Parse("1 January 428"), isPlaySports = false, dateCreated = DateTime.Now, Id="a018192a-5789-4012-a086-56789012c113"},
                //new Person() {firstName = "Socrates", lastName = "Philosopher", rank = 86, category = "Philosopher", dateOfBirth = DateTime.Parse("1 January 470"), isPlaySports = false, dateCreated = DateTime.Now, Id="b1292a3b-6890-4123-b197-67890123c114"},
                //new Person() {firstName = "Aristotle", lastName = "Philosopher", rank = 87, category = "Philosopher", dateOfBirth = DateTime.Parse("1 January 384"), isPlaySports = false, dateCreated = DateTime.Now, Id="c23a3b4c-7901-4234-c2a8-78901234c115"},
                //new Person() {firstName = "Confucius", lastName = "Philosopher", rank = 88, category = "Philosopher", dateOfBirth = DateTime.Parse("28 September 551"), isPlaySports = false, dateCreated = DateTime.Now, Id="d34b4c5d-8012-4345-d3b9-89012345c116"},
                //new Person() {firstName = "Dalai", lastName = "Lama", rank = 89, category = "Spiritual Leader", dateOfBirth = DateTime.Parse("6 July 1935"), isPlaySports = false, dateCreated = DateTime.Now, Id="e45c5d6e-9123-4456-e4ca-90123456c117"},
                //new Person() {firstName = "Sadhguru", lastName = "Vasudev", rank = 90, category = "Spiritual Leader", dateOfBirth = DateTime.Parse("3 September 1957"), isPlaySports = false, dateCreated = DateTime.Now, Id="f56d6e7f-0234-4567-f5db-01234567c118"},
                //new Person() {firstName = "Ratan", lastName = "Tata", rank = 91, category = "Businessman", dateOfBirth = DateTime.Parse("28 December 1937"), isPlaySports = false, dateCreated = DateTime.Now, Id="067e7f80-1345-4678-06ec-12345678c119"},
                //new Person() {firstName = "Dhirubhai", lastName = "Ambani", rank = 92, category = "Businessman", dateOfBirth = DateTime.Parse("28 December 1932"), isPlaySports = false, dateCreated = DateTime.Now, Id="178f8091-2456-4789-17fd-23456789c120"},
                //new Person() {firstName = "Indra", lastName = "Nooyi", rank = 93, category = "Businesswoman", dateOfBirth = DateTime.Parse("28 October 1955"), isPlaySports = false, dateCreated = DateTime.Now, Id="289091a2-3567-4890-280e-34567890c121"},
                //new Person() {firstName = "Sundar", lastName = "Pichai", rank = 94, category = "Technology Executive", dateOfBirth = DateTime.Parse("10 July 1972"), isPlaySports = false, dateCreated = DateTime.Now, Id="39a1a2b3-4678-4901-391f-45678901c122"},
                //new Person() {firstName = "Satya", lastName = "Nadella", rank = 95, category = "Technology Executive", dateOfBirth = DateTime.Parse("19 August 1967"), isPlaySports = false, dateCreated = DateTime.Now, Id="4ab2b3c4-5789-4012-4a20-56789012c123"},
                //new Person() {firstName = "Mark", lastName = "Zuckerberg", rank = 96, category = "Technology Executive", dateOfBirth = DateTime.Parse("14 May 1984"), isPlaySports = false, dateCreated = DateTime.Now, Id="5bc3c4d5-6890-4123-5b31-67890123c124"},
                //new Person() {firstName = "Larry", lastName = "Page", rank = 97, category = "Technology Executive", dateOfBirth = DateTime.Parse("26 March 1973"), isPlaySports = false, dateCreated = DateTime.Now, Id="6cd4d5e6-7901-4234-6c42-78901234c125"},
                //new Person() {firstName = "Sergey", lastName = "Brin", rank = 98, category = "Technology Executive", dateOfBirth = DateTime.Parse("21 August 1973"), isPlaySports = false, dateCreated = DateTime.Now, Id="7de5e6f7-8012-4345-7d53-89012345c126"},
                //new Person() {firstName = "Tim", lastName = "BernersLee", rank = 99, category = "Computer Scientist", dateOfBirth = DateTime.Parse("8 June 1955"), isPlaySports = false, dateCreated = DateTime.Now, Id="8ef6f708-9123-4456-8e64-90123456c127"},
                //new Person() {firstName = "Dennis", lastName = "Ritchie", rank = 100, category = "Computer Scientist", dateOfBirth = DateTime.Parse("9 September 1941"), isPlaySports = false, dateCreated = DateTime.Now, Id="9f070819-0234-4567-9f75-01234567c128"},
                ////TEST
                new Person() {firstName = "firstName1", lastName = "lastName1", rank = 99, category = "Test Category", dateOfBirth = DateTime.Parse("17 September 1879"), isPlaySports = false, dateCreated = DateTime.Now, Id="2d10dce3-9bbe-4e11-9fb5-d2ba23b05d44"},
                new Person() {firstName = "firstName2", lastName = "lastName2", rank = 99, category = "Test Category", dateOfBirth = DateTime.Parse("17 September 1879"), isPlaySports = true, dateCreated = DateTime.Now, Id="2d10dce3-9bbe-4e22-9fb5-d2ba23b05d55"},
             };
        }

        public static List<Person> GetPersons()
        {
            lock (personRepositoryLock)
            {
                return persons;
            }
        }

        public static void Initialize(SqlDataBaseDataContext context)
        {
            context.Database.EnsureCreated();

            // Seed Persons
            if (!context.Persons.Any())
            {
                context.Persons.AddRange(persons);
                context.SaveChanges();
            }
        }
    }
}
