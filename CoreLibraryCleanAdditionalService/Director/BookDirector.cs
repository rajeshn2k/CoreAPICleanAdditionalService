namespace Core.Library.Clean.AdditionalService
{
    public class BookDirector : IEntityDirector<BookDTO, BookCreateDTO>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMessagePublisher messagePublisher;

        public BookDirector(IUnitOfWork unitOfWork, IMessagePublisher messagePublisher)
        {
            this.unitOfWork = unitOfWork;
            this.messagePublisher = messagePublisher;
        }

        public async Task<IEnumerable<BookDTO>> GetEntitiesAsync(CancellationToken cancellationToken)
        {
            var books = await unitOfWork.BookRepository.GetEntitiesAsync(cancellationToken);

            if (books == null)
            {
                return null;
            }
            else
            {
                return books.Select(BookMapper.BookToBookDTO);
            }
        }

        public async Task<BookDTO> GetEntityByIdAsync(string entityId, CancellationToken cancellationToken)
        {
            var book = await unitOfWork.BookRepository.GetEntityByIdAsync(entityId, cancellationToken).ConfigureAwait(false);

            if (book == null)
            {
                return null;
            }
            else
            {
                return BookMapper.BookToBookDTO(book);
            }
        }

        public async Task<IEnumerable<BookDTO>> SearchEntitiesAsync(string searchValue, CancellationToken cancellationToken)
        {
            var books = await unitOfWork.BookRepository.SearchEntitiesAsync(searchValue, cancellationToken).ConfigureAwait(false);

            if (books == null)
            {
                return null;
            }
            else
            {
                return books.Select(BookMapper.BookToBookDTO);
            }
        }
        public async Task<IEnumerable<BookDTO>> SearchEntitiesByForeignIdAsync(string personId, CancellationToken cancellationToken)
        {
            var books = await unitOfWork.BookRepository.SearchEntitiesByForeignIdAsync(personId, cancellationToken).ConfigureAwait(false);

            if (books == null)
            {
                return null;
            }
            else
            {
                return books.Select(BookMapper.BookToBookDTO);
            }
        }

        public async Task<long> UpdateEntityByIdAsync(string entityId, BookDTO book, CancellationToken cancellationToken)
        {
            if (book != null)
            {
                var bookEntity = BookMapper.BookDTOToBook(book);

                var result = await unitOfWork.BookRepository.UpdateAsync(entityId, bookEntity, cancellationToken).ConfigureAwait(false);

                if (result > 0)
                    await messagePublisher.PublishAsync(bookEntity, MessageTypeConstant.BookType, MessageActionConstant.Update, cancellationToken).ConfigureAwait(false);

                return result;
            }

            return 0;
        }

        public async Task<long> UpdateEntitiesAsync(IEnumerable<string> bookIds, IEnumerable<BookDTO> books, CancellationToken cancellationToken)
        {
            if (books != null)
            {
                var bookEntities = books.Select(book => BookMapper.BookDTOToBook(book));

                var result = await unitOfWork.BookRepository.UpdateManyAsync(bookEntities, cancellationToken).ConfigureAwait(false);

                if (result > 0)
                    await messagePublisher.PublishAsync(books, MessageTypeConstant.BookType, MessageActionConstant.Update, cancellationToken).ConfigureAwait(false);

                return result;
            }

            return 0;
        }

        public async Task<BookDTO> CreateEntityAsync(BookCreateDTO book, CancellationToken cancellationToken)
        {
            if (book != null)
            {
                var bookEntity = BookMapper.BookCreateDTOToBook(book);

                var result = await unitOfWork.BookRepository.CreateEntityAsync(bookEntity, cancellationToken).ConfigureAwait(false);

                if (result != null)
                {
                    await messagePublisher.PublishAsync(book, MessageTypeConstant.BookType, MessageActionConstant.Create, cancellationToken).ConfigureAwait(false);

                    return BookMapper.BookToBookDTO(result);
                }
            }

            return null;
        }

        public async Task<IEnumerable<BookDTO>> CreateEntitiesAsync(IEnumerable<BookCreateDTO> books, CancellationToken cancellationToken)
        {
            if (books != null)
            {
                var bookEntities = books.Select(book => BookMapper.BookCreateDTOToBook(book));

                var result = await unitOfWork.BookRepository.CreateEntitiesAsync(bookEntities, cancellationToken).ConfigureAwait(false);

                if (result != null)
                {
                    await messagePublisher.PublishAsync(books, MessageTypeConstant.BookType, MessageActionConstant.Create, cancellationToken).ConfigureAwait(false);

                    return result.Select(BookMapper.BookToBookDTO);
                }
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

            var personCount = await unitOfWork.PersonRepository.GetEntitiesAsync(cancellationToken);

            if (personCount?.Count() > 0)
            {
                var result = await unitOfWork.BookRepository.CreateEntitiesAsync(books, cancellationToken).ConfigureAwait(false);

                if (result != null)
                {
                    return result.Select(BookMapper.BookToBookDTO);
                }
            }

            return null;
        }
    }
}
