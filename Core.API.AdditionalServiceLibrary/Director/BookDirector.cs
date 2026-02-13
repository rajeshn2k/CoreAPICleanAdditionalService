using Core.Library.ArivuTharavuThalam;
#nullable disable

namespace Core.API.AdditionalServiceLibrary
{
    public class BookDirector : IEntityDirector<BookDTO, BookCreateDTO>
    {
        private readonly IUnitOfWork unitOfWork;

        public BookDirector(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<BookDTO>> GetEntitiesAsync(CancellationToken cancellationToken)
        {
            var books = await unitOfWork.BookRepository.GetEntitiesAsync(cancellationToken);

            var bookDTOs = books.Select(BookMapper.BookToBookDTO);

            if (bookDTOs == null)
            {
                return Enumerable.Empty<BookDTO>();
            }

            return bookDTOs.OrderBy(book => book.bookName);
        }

        public async Task<BookDTO> GetEntityByIdAsync(string entityId, CancellationToken cancellationToken)
        {
            var book = await unitOfWork.BookRepository.GetEntityByIdAsync(entityId, cancellationToken).ConfigureAwait(false);
            return BookMapper.BookToBookDTO(book);
        }

        public async Task<IEnumerable<BookDTO>> SearchEntitiesAsync(string searchValue, CancellationToken cancellationToken)
        {
            var books = await unitOfWork.BookRepository.SearchEntitiesAsync(searchValue, cancellationToken).ConfigureAwait(false);

            var bookDTOs = books.Select(BookMapper.BookToBookDTO);

            if (bookDTOs == null)
            {
                return Enumerable.Empty<BookDTO>();
            }

            return bookDTOs.OrderBy(book => book.bookName);
        }

        public async Task<long> UpdateEntityByIdAsync(string entityId, BookDTO book, CancellationToken cancellationToken)
        {
            var bookEntity = BookMapper.BookDTOToBook(book);
            var result = await unitOfWork.BookRepository.UpdateAsync(entityId, bookEntity, cancellationToken).ConfigureAwait(false);
            return result;
        }

        public async Task<long> UpdateEntitiesAsync(string searchValue, IEnumerable<BookDTO> books, CancellationToken cancellationToken)
        {
            //TO DO - persons => true,
            var bookEntities = books.Select(BookMapper.BookDTOToBook);
            var result = await unitOfWork.BookRepository.UpdateManyAsync(bookEntities, cancellationToken).ConfigureAwait(false);
            return result;
        }

        public async Task<BookDTO> CreateEntityAsync(BookCreateDTO book, CancellationToken cancellationToken)
        {
            if (book != null)
            {
                var bookEntity = BookMapper.BookCreateDTOToBook(book);
                var result = await unitOfWork.BookRepository.CreateEntityAsync(bookEntity, cancellationToken).ConfigureAwait(false);

                //if (!configuration.IsCurrentMessageTypeEmpty())
                //{
                //    await messagePublisher.PublishAsync(book, MessageTypeConstant.BookType, MessageActionConstant.Create, cancellationToken).ConfigureAwait(false);
                //}
                return BookMapper.BookToBookDTO(result);
            }

            return null;
        }

        public async Task<IEnumerable<BookDTO>> CreateEntitiesAsync(IEnumerable<BookCreateDTO> books, CancellationToken cancellationToken)
        {
            if (books != null)
            {
                var bookEntities = books.Select(book => BookMapper.BookCreateDTOToBook(book));

                var result = await unitOfWork.BookRepository.CreateEntitiesAsync(bookEntities, cancellationToken).ConfigureAwait(false);

                return result.Select(BookMapper.BookToBookDTO);

                //if (!configuration.IsCurrentMessageTypeEmpty())
                //{
                //    await messagePublisher.PublishAsync(books, MessageTypeConstant.BookType, MessageActionConstant.Create, cancellationToken).ConfigureAwait(false);
                //}
            }

            return null;
        }

        public async Task<long> DeleteEntityByIdAsync(string entityId, CancellationToken cancellationToken)
        {
            var result = await unitOfWork.BookRepository.DeleteEntityByIdAsync(entityId, cancellationToken).ConfigureAwait(false);

            return result;
        }

        public async Task<long> DeleteEntitiesAsync(CancellationToken cancellationToken)
        {
            var result = await unitOfWork.BookRepository.DeleteEntitiesAsync(cancellationToken).ConfigureAwait(false);
            return result;
        }

        public async Task<IEnumerable<BookDTO>> LoadAllEntityForNewDatabase(CancellationToken cancellationToken)
        {
            IEnumerable<Book> books = DatabaseInitializerBook.GetBooks();

            var result = await unitOfWork.BookRepository.CreateEntitiesAsync(books, cancellationToken).ConfigureAwait(false);

            return result.Select(BookMapper.BookToBookDTO);
        }
    }
}
