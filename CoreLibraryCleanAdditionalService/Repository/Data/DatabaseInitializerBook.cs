namespace Core.Library.Clean.AdditionalService
{
    public static class DatabaseInitializerBook
    {
        private static readonly List<Book> books;
        private static readonly object bookRepositoryLock = new object();
        static DatabaseInitializerBook()
        {
            books = new List<Book>
            {
                new Book() {personId = "2d10dce3-9bbe-4e63-9fb5-d2ba23b05d48", bookCategory = "Adventure", bookName = "Becoming", edition = "Kindle", price = 649, image = "/assets/img/default.png", dateCreated = DateTime.Now, Id="8fcad788-140d-44a6-8c5f-e673a1347c91" },
                new Book() {personId = "9aa456b2-c0b5-4b58-b831-4d2a042028fc", bookCategory = "History", bookName = "A Thousand Splendid Suns", edition = "Paperback", price = 349, image = "/assets/img/default.png", dateCreated = DateTime.Now, Id="3d7c2049-fc1c-48a4-b76f-3378dce3d1f4"  },
                new Book() {personId = "562c1468-64f1-49bc-9f60-a4ae3bd722fc", bookCategory = "Humour", bookName = "100+ Knock Knock Jokes", edition = "Paperback", price = 479, image = "/assets/img/default.png", dateCreated = DateTime.Now , Id="69fdf4b8-0e58-471f-8453-5fcfb0d63ecf"  },
                new Book() {personId = "1388f427-fe8e-4e87-b98d-c7ead5eaa2f0", bookCategory = "Humour", bookName = "A Man Called Ove", edition = "Paperback", price = 270, image = "/assets/img/default.png", dateCreated = DateTime.Now , Id="e53088ee-b0fa-436a-9f90-c20b2f8e77cc"  },
                new Book() {personId = "fbde722e-cd91-4004-947e-5d400ca5d951", bookCategory = "Crime", bookName = "The Girl in Room 105", edition = "Paperback", price = 89, image = "/assets/img/default.png", dateCreated = DateTime.Now , Id="16c0970c-0825-4c53-965c-5f9210d75e99"  },
                new Book() {personId = "67b26599-5bdc-40f9-a5e6-b0c2fe625e9d", bookCategory = "History", bookName = "Lean In= Women, Work, and the Will to Lead", edition = "Paperback", price = 370, image = "/assets/img/default.png", dateCreated = DateTime.Now , Id="665b561a-a76f-47b3-a6b6-28e92a19c06d"  },
                new Book() {personId = "f62455c6-c79c-4e87-9cb3-5c7424444a7e", bookCategory = "History", bookName = "Kudiarasu 1925 Periyarin Ezhuthum Pechum", edition = "Kindle", price = 99, image = "/assets/img/default.png", dateCreated = DateTime.Now , Id="f4e7a21e-5103-49b7-951e-3841bb0a0473"  },
                new Book() {personId = "a1c2d3e4-f567-4890-ab12-34567890c001", bookCategory = "Sports", bookName = "Serve to Win", edition = "Paperback", price = 499, image = "/assets/img/default.png", dateCreated = DateTime.Now, Id="10a1b2c3-d4e5-4678-9012-34567890d001" },
                new Book() {personId = "b2d3e4f5-a678-4901-bc23-45678901c002", bookCategory = "Sports", bookName = "Rafa", edition = "Hardcover", price = 799, image = "/assets/img/default.png", dateCreated = DateTime.Now, Id="20b1c2d3-e4f5-4789-0123-45678901d002" },
                new Book() {personId = "c3e4f5a6-b789-4012-cd34-56789012c003", bookCategory = "Biography", bookName = "My Life", edition = "Paperback", price = 599, image = "/assets/img/default.png", dateCreated = DateTime.Now, Id="30c1d2e3-f4a5-4890-1234-56789012d003" },
                new Book() {personId = "d4f5a6b7-c890-4123-de45-67890123c004", bookCategory = "Sports", bookName = "Playing It My Way", edition = "Kindle", price = 399, image = "/assets/img/default.png", dateCreated = DateTime.Now, Id="40d1e2f3-a4b5-4901-2345-67890123d004" },
                new Book() {personId = "e5a6b7c8-d901-4234-ef56-78901234c005", bookCategory = "Biography", bookName = "Driven", edition = "Paperback", price = 449, image = "/assets/img/default.png", dateCreated = DateTime.Now, Id="50e1f2a3-b4c5-4012-3456-78901234d005" },
                new Book() {personId = "f6b7c8d9-e012-4345-f067-89012345c006", bookCategory = "Sports", bookName = "The Dhoni Touch", edition = "Paperback", price = 299, image = "/assets/img/default.png", dateCreated = DateTime.Now, Id="60f1a2b3-c4d5-4123-4567-89012345d006" },
                new Book() {personId = "07c8d9e0-f123-4456-0178-90123456c007", bookCategory = "Sports", bookName = "Cricketing Genius", edition = "Hardcover", price = 699, image = "/assets/img/default.png", dateCreated = DateTime.Now, Id="7012a3b4-c5d6-4234-5678-90123456d007" },
                new Book() {personId = "18d9e0f1-0234-4567-1289-01234567c008", bookCategory = "Biography", bookName = "Sultan of Swing", edition = "Paperback", price = 549, image = "/assets/img/default.png", dateCreated = DateTime.Now, Id="8123b4c5-d6e7-4345-6789-01234567d008" },
                new Book() {personId = "29e0f1a2-1345-4678-2390-12345678c009", bookCategory = "Sports", bookName = "Greatest All Rounder", edition = "Kindle", price = 349, image = "/assets/img/default.png", dateCreated = DateTime.Now, Id="9234c5d6-e7f8-4456-7890-12345678d009" },
                new Book() {personId = "3af1a2b3-2456-4789-3401-23456789c010", bookCategory = "Biography", bookName = "No Spin", edition = "Paperback", price = 499, image = "/assets/img/default.png", dateCreated = DateTime.Now, Id="a345d6e7-f809-4567-8901-23456789d010" },
                new Book() {personId = "4ba2b3c4-3567-4890-4512-34567890c011", bookCategory = "Biography", bookName = "Messi: The Inside Story", edition = "Hardcover", price = 899, image = "/assets/img/default.png", dateCreated = DateTime.Now, Id="b456e7f8-091a-4678-9012-34567890d011" },
                new Book() {personId = "5cb3c4d5-4678-4901-5623-45678901c012", bookCategory = "Sports", bookName = "Ronaldo", edition = "Paperback", price = 649, image = "/assets/img/default.png", dateCreated = DateTime.Now, Id="c567f809-1a2b-4789-0123-45678901d012" },
                new Book() {personId = "6dc4d5e6-5789-4012-6734-56789012c013", bookCategory = "History", bookName = "Pele: The Autobiography", edition = "Paperback", price = 599, image = "/assets/img/default.png", dateCreated = DateTime.Now, Id="d678091a-2b3c-4890-1234-56789012d013" },
                new Book() {personId = "7ed5e6f7-6890-4123-7845-67890123c014", bookCategory = "Sports", bookName = "Zidane: A Biography", edition = "Kindle", price = 379, image = "/assets/img/default.png", dateCreated = DateTime.Now, Id="e7891a2b-3c4d-4901-2345-67890123d014" },
                new Book() {personId = "8fe6f708-7901-4234-8956-78901234c015", bookCategory = "Biography", bookName = "Beckham", edition = "Hardcover", price = 899, image = "/assets/img/default.png", dateCreated = DateTime.Now, Id="f89a2b3c-4d5e-4012-3456-78901234d015" },
                new Book() {personId = "90f70819-8012-4345-9067-89012345c016", bookCategory = "Sports", bookName = "Michael Jordan: The Life", edition = "Paperback", price = 749, image = "/assets/img/default.png", dateCreated = DateTime.Now, Id="09ab3c4d-5e6f-4123-4567-89012345d016" },
                new Book() {personId = "a108192a-9123-4456-a178-90123456c017", bookCategory = "Biography", bookName = "LeBron", edition = "Paperback", price = 599, image = "/assets/img/default.png", dateCreated = DateTime.Now, Id="1abc4d5e-6f70-4234-5678-90123456d017" },
                new Book() {personId = "b2192a3b-0234-4567-b289-01234567c018", bookCategory = "Biography", bookName = "Mamba Mentality", edition = "Hardcover", price = 999, image = "/assets/img/default.png", dateCreated = DateTime.Now, Id="2bcd5e6f-7081-4345-6789-01234567d018" },
                new Book() {personId = "c32a3b4c-1345-4678-c390-12345678c019", bookCategory = "Sports", bookName = "Faster", edition = "Paperback", price = 449, image = "/assets/img/default.png", dateCreated = DateTime.Now, Id="3cde6f70-8192-4456-7890-12345678d019" },
                new Book() {personId = "d43b4c5d-2456-4789-d401-23456789c020", bookCategory = "Sports", bookName = "Lewis Hamilton: The Biography", edition = "Kindle", price = 399, image = "/assets/img/default.png", dateCreated = DateTime.Now, Id="4def7081-92a3-4567-8901-23456789d020" },
                new Book() {personId = "e54c5d6e-3567-4890-e512-34567890c021", bookCategory = "Biography", bookName = "Ayrton Senna", edition = "Hardcover", price = 799, image = "/assets/img/default.png", dateCreated = DateTime.Now, Id="5ef08192-a3b4-4678-9012-34567890d021" },
                new Book() {personId = "f65d6e7f-4678-4901-f623-45678901c022", bookCategory = "Sports", bookName = "Max Verstappen: Unstoppable", edition = "Paperback", price = 499, image = "/assets/img/default.png", dateCreated = DateTime.Now, Id="6f012a3b-4c5d-4789-0123-45678901d022" },
                new Book() {personId = "076e7f80-5789-4012-0734-56789012c023", bookCategory = "Sports", bookName = "How I Play Golf", edition = "Paperback", price = 549, image = "/assets/img/default.png", dateCreated = DateTime.Now, Id="70123b4c-5d6e-4890-1234-56789012d023" },
                new Book() {personId = "187f8091-6890-4123-1845-67890123c024", bookCategory = "History", bookName = "The Four Minute Mile", edition = "Kindle", price = 299, image = "/assets/img/default.png", dateCreated = DateTime.Now, Id="81234c5d-6e7f-4901-2345-67890123d024" },
                new Book() {personId = "298091a2-7901-4234-2956-78901234c025", bookCategory = "Sports", bookName = "Golf My Way", edition = "Hardcover", price = 699, image = "/assets/img/default.png", dateCreated = DateTime.Now, Id="92345d6e-7f80-4012-3456-78901234d025" },
                new Book() {personId = "3a91a2b3-8012-4345-3a67-89012345c026", bookCategory = "Biography", bookName = "The Greatest", edition = "Paperback", price = 649, image = "/assets/img/default.png", dateCreated = DateTime.Now, Id="a3456e7f-8091-4123-4567-89012345d026" },
                new Book() {personId = "4ba2b3c4-9123-4456-4b78-90123456c027", bookCategory = "Sports", bookName = "Undisputed Truth", edition = "Paperback", price = 499, image = "/assets/img/default.png", dateCreated = DateTime.Now, Id="b4567f80-91a2-4234-5678-90123456d027" },
                new Book() {personId = "5bc3c4d5-0234-4567-5c89-01234567c028", bookCategory = "Biography", bookName = "Pacquiao: The Fighting Life", edition = "Kindle", price = 329, image = "/assets/img/default.png", dateCreated = DateTime.Now, Id="c5678091-a2b3-4345-6789-01234567d028" },
                new Book() {personId = "6cd4d5e6-1345-4678-6d90-12345678c029", bookCategory = "Sports", bookName = "The TB12 Method", edition = "Paperback", price = 599, image = "/assets/img/default.png", dateCreated = DateTime.Now, Id="d67891a2-b3c4-4456-7890-12345678d029" },
                new Book() {personId = "7de5e6f7-2456-4789-7e01-23456789c030", bookCategory = "Sports", bookName = "Courage to Soar", edition = "Hardcover", price = 649, image = "/assets/img/default.png", dateCreated = DateTime.Now, Id="e789a2b3-c4d5-4567-8901-23456789d030" },
                new Book() {personId = "8ef6f708-3567-4890-8f12-34567890c031", bookCategory = "Sports", bookName = "No Limits", edition = "Paperback", price = 549, image = "/assets/img/default.png", dateCreated = DateTime.Now, Id="f89ab3c4-d5e6-4678-9012-34567890d031" },
                new Book() {personId = "9f070819-4678-4901-9023-45678901c032", bookCategory = "Biography", bookName = "Unbreakable", edition = "Kindle", price = 399, image = "/assets/img/default.png", dateCreated = DateTime.Now, Id="09abc4d5-e6f7-4789-0123-45678901d032" },
                new Book() {personId = "a018192a-5789-4012-a134-56789012c033", bookCategory = "Sports", bookName = "Believe", edition = "Paperback", price = 449, image = "/assets/img/default.png", dateCreated = DateTime.Now, Id="1bcd5e6f-7081-4890-1234-56789012d033" },
                new Book() {personId = "b1292a3b-6890-4123-b245-67890123c034", bookCategory = "Sports", bookName = "By Gods Decree", edition = "Hardcover", price = 599, image = "/assets/img/default.png", dateCreated = DateTime.Now, Id="2cde6f70-8192-4901-2345-67890123d034" },
                new Book() {personId = "c23a3b4c-7901-4234-c356-78901234c035", bookCategory = "Fantasy", bookName = "Harry Potter and the Sorcerer's Stone", edition = "Paperback", price = 399, image = "/assets/img/default.png", dateCreated = DateTime.Now, Id="3def7081-92a3-4012-3456-78901234d035" },
                new Book() {personId = "d34b4c5d-8012-4345-d467-89012345c036", bookCategory = "Dystopian", bookName = "1984", edition = "Kindle", price = 199, image = "/assets/img/default.png", dateCreated = DateTime.Now, Id="4ef08192-a3b4-4123-4567-89012345d036" },
                new Book() {personId = "e45c5d6e-9123-4456-e578-90123456c037", bookCategory = "Adventure", bookName = "The Adventures of Tom Sawyer", edition = "Paperback", price = 299, image = "/assets/img/default.png", dateCreated = DateTime.Now, Id="5f012a3b-4c5d-4234-5678-90123456d037" },
                new Book() {personId = "f56d6e7f-0234-4567-f689-01234567c038", bookCategory = "Adventure", bookName = "The Old Man and the Sea", edition = "Paperback", price = 249, image = "/assets/img/default.png", dateCreated = DateTime.Now, Id="60123b4c-5d6e-4345-6789-01234567d038" },
                new Book() {personId = "067e7f80-1345-4678-0780-12345678c039", bookCategory = "Classic", bookName = "War and Peace", edition = "Hardcover", price = 899, image = "/assets/img/default.png", dateCreated = DateTime.Now, Id="71234c5d-6e7f-4456-7890-12345678d039" },
                new Book() {personId = "178f8091-2456-4789-1891-23456789c040", bookCategory = "Classic", bookName = "Crime and Punishment", edition = "Paperback", price = 499, image = "/assets/img/default.png", dateCreated = DateTime.Now, Id="82345d6e-7f80-4567-8901-23456789d040" },
                new Book() {personId = "289091a2-3567-4890-2902-34567890c041", bookCategory = "Drama", bookName = "Hamlet", edition = "Kindle", price = 149, image = "/assets/img/default.png", dateCreated = DateTime.Now, Id="93456e7f-8091-4678-9012-34567890d041" },
                new Book() {personId = "39a1a2b3-4678-4901-3a13-45678901c042", bookCategory = "Romance", bookName = "Pride and Prejudice", edition = "Paperback", price = 299, image = "/assets/img/default.png", dateCreated = DateTime.Now, Id="a4567f80-91a2-4789-0123-45678901d042" },
                new Book() {personId = "4ab2b3c4-5789-4012-4b24-56789012c043", bookCategory = "Classic", bookName = "Great Expectations", edition = "Hardcover", price = 549, image = "/assets/img/default.png", dateCreated = DateTime.Now, Id="b5678091-a2b3-4890-1234-56789012d043" },
                new Book() {personId = "5bc3c4d5-6890-4123-5c35-67890123c044", bookCategory = "Classic", bookName = "Les Miserables", edition = "Paperback", price = 699, image = "/assets/img/default.png", dateCreated = DateTime.Now, Id="c67891a2-b3c4-4901-2345-67890123d044" },
                new Book() {personId = "6cd4d5e6-7901-4234-6d46-78901234c045", bookCategory = "Fiction", bookName = "One Hundred Years of Solitude", edition = "Paperback", price = 599, image = "/assets/img/default.png", dateCreated = DateTime.Now, Id="d789a2b3-c4d5-4012-3456-78901234d045" },
                new Book() {personId = "7de5e6f7-8012-4345-7e57-89012345c046", bookCategory = "Adventure", bookName = "The Alchemist", edition = "Kindle", price = 249, image = "/assets/img/default.png", dateCreated = DateTime.Now, Id="e89ab3c4-d5e6-4123-4567-89012345d046" },
                new Book() {personId = "8ef6f708-9123-4456-8f68-90123456c047", bookCategory = "Mystery", bookName = "The Da Vinci Code", edition = "Paperback", price = 399, image = "/assets/img/default.png", dateCreated = DateTime.Now, Id="f9abc4d5-e6f7-4234-5678-90123456d047" },
                new Book() {personId = "9f070819-0234-4567-9079-01234567c048", bookCategory = "Horror", bookName = "The Shining", edition = "Paperback", price = 449, image = "/assets/img/default.png", dateCreated = DateTime.Now, Id="0abcd5e6-f708-4345-6789-01234567d048" },
                new Book() {personId = "a018192a-1345-4678-a180-12345678c049", bookCategory = "Mystery", bookName = "And Then There Were None", edition = "Kindle", price = 199, image = "/assets/img/default.png", dateCreated = DateTime.Now, Id="1bcde6f7-8091-4456-7890-12345678d049" },
                new Book() {personId = "b1292a3b-2456-4789-b291-23456789c050", bookCategory = "Mystery", bookName = "The Hound of the Baskervilles", edition = "Paperback", price = 299, image = "/assets/img/default.png", dateCreated = DateTime.Now, Id="2cdef708-91a2-4567-8901-23456789d050" },
                new Book() {personId = "c23a3b4c-3567-4890-c3a2-34567890c051", bookCategory = "Fiction", bookName = "Swami and Friends", edition = "Paperback", price = 249, image = "/assets/img/default.png", dateCreated = DateTime.Now, Id="3def0819-a2b3-4678-9012-34567890d051" },
                new Book() {personId = "d34b4c5d-4678-4901-d4b3-45678901c052", bookCategory = "Poetry", bookName = "Gitanjali", edition = "Hardcover", price = 399, image = "/assets/img/default.png", dateCreated = DateTime.Now, Id="4ef192a3-b4c5-4789-0123-45678901d052" },
                new Book() {personId = "e45c5d6e-5789-4012-e5c4-56789012c053", bookCategory = "Fiction", bookName = "The God of Small Things", edition = "Paperback", price = 349, image = "/assets/img/default.png", dateCreated = DateTime.Now, Id="5f02a3b4-c5d6-4890-1234-56789012d053" },
                new Book() {personId = "f56d6e7f-6890-4123-f6d5-67890123c054", bookCategory = "Children", bookName = "The Room on the Roof", edition = "Paperback", price = 199, image = "/assets/img/default.png", dateCreated = DateTime.Now, Id="6013b4c5-d6e7-4901-2345-67890123d054" },
                new Book() {personId = "067e7f80-7901-4234-07e6-78901234c055", bookCategory = "Mythology", bookName = "The Immortals of Meluha", edition = "Kindle", price = 299, image = "/assets/img/default.png", dateCreated = DateTime.Now, Id="7124c5d6-e7f8-4012-3456-78901234d055" },
                new Book() {personId = "178f8091-8012-4345-18f7-89012345c056", bookCategory = "Fiction", bookName = "A Suitable Boy", edition = "Hardcover", price = 899, image = "/assets/img/default.png", dateCreated = DateTime.Now, Id="8235d6e7-f809-4123-4567-89012345d056" },
                new Book() {personId = "289091a2-9123-4456-2908-90123456c057", bookCategory = "Fiction", bookName = "Interpreter of Maladies", edition = "Paperback", price = 349, image = "/assets/img/default.png", dateCreated = DateTime.Now, Id="9346e7f8-091a-4234-5678-90123456d057" },
                new Book() {personId = "39a1a2b3-0234-4567-3a19-01234567c058", bookCategory = "Fiction", bookName = "Midnight's Children", edition = "Paperback", price = 499, image = "/assets/img/default.png", dateCreated = DateTime.Now, Id="a457f809-1a2b-4345-6789-01234567d058" },
                new Book() {personId = "4ab2b3c4-1345-4678-4b20-12345678c059", bookCategory = "Fantasy", bookName = "American Gods", edition = "Kindle", price = 399, image = "/assets/img/default.png", dateCreated = DateTime.Now, Id="b568091a-2b3c-4456-7890-12345678d059" },
                new Book() {personId = "5bc3c4d5-2456-4789-5c31-23456789c060", bookCategory = "Fiction", bookName = "Norwegian Wood", edition = "Paperback", price = 449, image = "/assets/img/default.png", dateCreated = DateTime.Now, Id="c6791a2b-3c4d-4567-8901-23456789d060" },
                //
                new Book() {personId = "2d10dce3-9bbe-4e11-9fb5-d2ba23b05d44", bookCategory = "Adventure", bookName = "TestBook1", edition = "TestEdition", price = 12, image = "/assets/img/testimage1.png", dateCreated = DateTime.Now , Id="dfc775b5-4769-49c9-86fb-6e0b3451e119"  },
                new Book() {personId = "2d10dce3-9bbe-4e22-9fb5-d2ba23b05d55", bookCategory = "Adventure", bookName = "TestBook2", edition = "TestEdition", price = 13, image = "/assets/img/testimage2.png", dateCreated = DateTime.Now , Id="3943246e-5bef-40d5-a96d-db3388b3d414"  },
                new Book() { personId ="2d10dce3-9bbe-4e22-9fb5-d2ba23b05d55", bookCategory = "Adventure", bookName = "TestBook3", edition = "TestEdition", price = 14, image = "/assets/img/testimage3.png", dateCreated = DateTime.Now , Id="4c35788e-a426-421f-99bb-a00926bfc0ec"  },
            };
        }

        public static List<Book> GetBooks()
        {
            lock (bookRepositoryLock)
            {
                return books;
            }
        }

        public static void Initialize(SqlDataBaseDataContext context)
        {
            context.Database.EnsureCreated();

            // Seed Persons
            if (!context.Books.Any())
            {
                context.Books.AddRange(books);
                context.SaveChanges();
            }
        }
    }
}
