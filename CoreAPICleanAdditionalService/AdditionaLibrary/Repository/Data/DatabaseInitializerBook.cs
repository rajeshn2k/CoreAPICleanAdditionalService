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
                new Book() {personId = "2d10dce3-9bbe-4e63-9fb5-d2ba23b05d48", bookCategory = "Adventure", bookName = "Becoming", edition = "Kindle", price = 649, image = "/assets/img/150.png", dateCreated = DateTime.Now, Id="8fcad788-140d-44a6-8c5f-e673a1347c91" },
                new Book() {personId = "9aa456b2-c0b5-4b58-b831-4d2a042028fc", bookCategory = "History", bookName = "A Thousand Splendid Suns", edition = "Paperback", price = 349, image = "/assets/img/150.png", dateCreated = DateTime.Now, Id="3d7c2049-fc1c-48a4-b76f-3378dce3d1f4"  },
                new Book() {personId = "562c1468-64f1-49bc-9f60-a4ae3bd722fc", bookCategory = "Humour", bookName = "100+ Knock Knock Jokes", edition = "Paperback", price = 479, image = "/assets/img/150.png", dateCreated = DateTime.Now , Id="69fdf4b8-0e58-471f-8453-5fcfb0d63ecf"  },
                new Book() {personId = "1388f427-fe8e-4e87-b98d-c7ead5eaa2f0", bookCategory = "Humour", bookName = "A Man Called Ove", edition = "Paperback", price = 270, image = "/assets/img/150.png", dateCreated = DateTime.Now , Id="e53088ee-b0fa-436a-9f90-c20b2f8e77cc"  },
                new Book() {personId = "fbde722e-cd91-4004-947e-5d400ca5d951", bookCategory = "Crime", bookName = "The Girl in Room 105", edition = "Paperback", price = 89, image = "/assets/img/150.png", dateCreated = DateTime.Now , Id="16c0970c-0825-4c53-965c-5f9210d75e99"  },
                new Book() {personId = "67b26599-5bdc-40f9-a5e6-b0c2fe625e9d", bookCategory = "History", bookName = "Lean In= Women, Work, and the Will to Lead", edition = "Paperback", price = 370, image = "/assets/img/150.png", dateCreated = DateTime.Now , Id="665b561a-a76f-47b3-a6b6-28e92a19c06d"  },
                new Book() {personId = "f62455c6-c79c-4e87-9cb3-5c7424444a7e", bookCategory = "History", bookName = "Kudiarasu 1925 Periyarin Ezhuthum Pechum", edition = "Kindle", price = 99, image = "/assets/img/150.png", dateCreated = DateTime.Now , Id="f4e7a21e-5103-49b7-951e-3841bb0a0473"  },
                //
                new Book() {personId = "2d10dce3-9bbe-4e63-9fb5-d2ba23b05d48", bookCategory = "Adventure", bookName = "TestBook1", edition = "TestEdition", price = 12, image = "/assets/img/testimage1.png", dateCreated = DateTime.Now , Id="dfc775b5-4769-49c9-86fb-6e0b3451e119"  },
                new Book() {personId = "2d10dce3-9bbe-4e63-9fb5-d2ba23b05d48", bookCategory = "Adventure", bookName = "TestBook2", edition = "TestEdition", price = 13, image = "/assets/img/testimage2.png", dateCreated = DateTime.Now , Id="3943246e-5bef-40d5-a96d-db3388b3d414"  },
                new Book() { personId ="2d10dce3-9bbe-4e63-9fb5-d2ba23b05d48", bookCategory = "Adventure", bookName = "TestBook3", edition = "TestEdition", price = 14, image = "/assets/img/testimage3.png", dateCreated = DateTime.Now , Id="4c35788e-a426-421f-99bb-a00926bfc0ec"  },
                new Book() { personId ="9aa456b2-c0b5-4b58-b831-4d2a042028fc", bookCategory = "Adventure", bookName = "TestBook4", edition = "TestEdition", price = 15, image = "/assets/img/testimage4.png", dateCreated = DateTime.Now , Id="466e1f3a-7306-4604-9874-c79c9526d608"  },
                new Book() { personId ="9aa456b2-c0b5-4b58-b831-4d2a042028fc", bookCategory = "Adventure", bookName = "TestBook5", edition = "TestEdition", price = 16, image = "/assets/img/testimage5.png", dateCreated = DateTime.Now , Id="64171693-b53e-4e2f-8e07-d70da031bebe"  },
                new Book() { personId ="9aa456b2-c0b5-4b58-b831-4d2a042028fc", bookCategory = "Adventure", bookName = "TestBook6", edition = "TestEdition", price = 17, image = "/assets/img/testimage6.png", dateCreated = DateTime.Now , Id="27a2f003-903f-4f6a-b30d-65bb092bb260"  },
                new Book() { personId ="9aa456b2-c0b5-4b58-b831-4d2a042028fc", bookCategory = "Adventure", bookName = "TestBook7", edition = "TestEdition", price = 18, image = "/assets/img/testimage7.png", dateCreated = DateTime.Now, Id="5db192a9-0e0e-43b4-b210-03874669808e"   },
                new Book() { personId ="562c1468-64f1-49bc-9f60-a4ae3bd722fc", bookCategory = "Humour", bookName = "TestBook8", edition = "TestEdition", price = 19, image = "/assets/img/testimage8.png", dateCreated = DateTime.Now  , Id="25d509e1-ad56-4095-8506-2b020220cdf4" },
                new Book() { personId ="1388f427-fe8e-4e87-b98d-c7ead5eaa2f0", bookCategory = "Humour", bookName = "TestBook9", edition = "TestEdition", price = 211, image = "/assets/img/testimage9.png", dateCreated = DateTime.Now , Id="d9e3913f-b333-44b5-9da8-0004cc248efa"  },
                new Book() { personId ="1388f427-fe8e-4e87-b98d-c7ead5eaa2f0", bookCategory = "Humour", bookName = "TestBook10", edition = "TestEdition", price = 221, image = "/assets/img/testimage10.png", dateCreated = DateTime.Now , Id="a1aab07c-f655-4271-ab94-edeb7ac4fe54"  },
                new Book() { personId ="fbde722e-cd91-4004-947e-5d400ca5d951", bookCategory = "Humour", bookName = "TestBook11", edition = "TestEdition", price = 231, image = "/assets/img/testimage11.png", dateCreated = DateTime.Now , Id="6d51c487-986f-452d-b11c-be4d897f1e72"  },
                new Book() { personId ="fbde722e-cd91-4004-947e-5d400ca5d951", bookCategory = "Humour", bookName = "TestBook12", edition = "TestEdition", price = 241, image = "/assets/img/testimage12.png", dateCreated = DateTime.Now , Id="d73c7820-6a54-4abe-b65d-ab324e5ae366"  },
            };
        }

        public static List<Book> GetBooks()
        {
            lock (bookRepositoryLock)
            {
                return books;
            }
        }
    }
}
