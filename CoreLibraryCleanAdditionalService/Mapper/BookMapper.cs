namespace Core.Library.Clean.AdditionalService
{
    /// <summary>
    /// BookMapper - Works AFTER data fecthed from Database Repository completed
    /// BookMapper - Will NOT have impact on SQL query exectuted
    /// </summary>
    public static class BookMapper
    {
        public static BookDTO BookToBookDTO(Book book)
        {
            if (book == null) return null;

            return new BookDTO
            {
                id = book.Id,
                personId = book.personId,
                bookCategory = book.bookCategory,
                bookName = book.bookName,
                edition = book.edition,
                image = book.image,
                price = book.price,
                dateCreated = book.dateCreated
            };
        }

        public static Book BookDTOToBook(BookDTO dto)
        {
            if (dto == null) return null;

            return new Book
            {
                Id = dto.id,
                personId = dto.personId,
                bookCategory = dto.bookCategory,
                bookName = dto.bookName,
                edition = dto.edition,
                image = dto.image,
                price = dto.price,
                dateCreated = dto.dateCreated
            };
        }

        public static BookCreateDTO BookToBookCreateDTO(Book book)
        {
            if (book == null) return null;

            return new BookCreateDTO
            {
                bookCategory = book.bookCategory,
                bookName = book.bookName,
                edition = book.edition,
                image = book.image,
                price = book.price,
            };
        }

        public static Book BookCreateDTOToBook(BookCreateDTO dto)
        {
            if (dto == null) return null;

            return new Book
            {
                bookCategory = dto.bookCategory,
                bookName = dto.bookName,
                edition = dto.edition,
                image = dto.image,
                price = dto.price,
            };
        }
    }
}
