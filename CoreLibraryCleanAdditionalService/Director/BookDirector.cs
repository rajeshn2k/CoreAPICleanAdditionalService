using Microsoft.Extensions.Logging;

namespace Core.Library.Clean.AdditionalService
{
    /// BookDirector NEED NOT TO HANDLE EXCEPTION
    /// ALL OTHER SERVICES SUCH AS DBCONTEXT, CACHE, MESSAGING, MUST HANDLE EXCEPTION and CANNOT FAIL DIRECTOR
    public class BookDirector : IEntityDirector<BookDTO, BookCreateDTO>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMessagePublisher messagePublisher;
        private readonly ICacheService cacheService;
        private readonly ILogger<BookDirector> logger;

        public BookDirector(IUnitOfWork unitOfWork, IMessagePublisher messagePublisher, ICacheService cacheService, ILogger<BookDirector> logger)
        {
            this.unitOfWork = unitOfWork;
            this.messagePublisher = messagePublisher;
            this.cacheService = cacheService;
            this.logger = logger;
        }

        public async Task<IEnumerable<BookDTO>> GetEntitiesAsync(CancellationToken cancellationToken)
        {
            var cacheKey = "book:all";
            var cachedBooks = await cacheService.GetAsync<IEnumerable<BookDTO>>(cacheKey, cancellationToken);

            if (cachedBooks != null)
            {
                logger.LogDebug("Cache hit for key: {CacheKey}", cacheKey);
                return cachedBooks;
            }

            var books = await unitOfWork.BookRepository.GetEntitiesAsync(cancellationToken);

            if (books == null)
            {
                return null;
            }
            else
            {
                var bookDTOs = books.Select(BookMapper.BookToBookDTO);
                await cacheService.SetAsync(cacheKey, bookDTOs, TimeSpan.FromMinutes(30), cancellationToken);
                return bookDTOs;
            }
        }

        public async Task<BookDTO> GetEntityByIdAsync(string entityId, CancellationToken cancellationToken)
        {
            var cacheKey = $"book:{entityId}";
            var cachedBook = await cacheService.GetAsync<BookDTO>(cacheKey, cancellationToken);

            if (cachedBook != null)
            {
                logger.LogDebug("Cache hit for key: {CacheKey}", cacheKey);
                return cachedBook;
            }

            var book = await unitOfWork.BookRepository.GetEntityByIdAsync(entityId, cancellationToken).ConfigureAwait(false);

            if (book == null)
            {
                return null;
            }
            else
            {
                var bookDTO = BookMapper.BookToBookDTO(book);
                await cacheService.SetAsync(cacheKey, bookDTO, TimeSpan.FromMinutes(60), cancellationToken);
                return bookDTO;
            }
        }

        public async Task<IEnumerable<BookDTO>> SearchEntitiesAsync(string searchValue, CancellationToken cancellationToken)
        {
            var cacheKey = $"book:search:{searchValue}";
            var cachedBooks = await cacheService.GetAsync<IEnumerable<BookDTO>>(cacheKey, cancellationToken);

            if (cachedBooks != null)
            {
                logger.LogDebug("Cache hit for key: {CacheKey}", cacheKey);
                return cachedBooks;
            }

            var books = await unitOfWork.BookRepository.SearchEntitiesAsync(searchValue, cancellationToken).ConfigureAwait(false);

            if (books == null)
            {
                return null;
            }
            else
            {
                var bookDTOs = books.Select(BookMapper.BookToBookDTO);
                await cacheService.SetAsync(cacheKey, bookDTOs, TimeSpan.FromMinutes(15), cancellationToken);
                return bookDTOs;
            }
        }
        public async Task<IEnumerable<BookDTO>> SearchEntitiesByForeignIdAsync(string personId, CancellationToken cancellationToken)
        {
            var cacheKey = $"book:person:{personId}";
            var cachedBooks = await cacheService.GetAsync<IEnumerable<BookDTO>>(cacheKey, cancellationToken);

            if (cachedBooks != null)
            {
                logger.LogDebug("Cache hit for key: {CacheKey}", cacheKey);
                return cachedBooks;
            }

            var books = await unitOfWork.BookRepository.SearchEntitiesByForeignIdAsync(personId, cancellationToken).ConfigureAwait(false);

            if (books == null)
            {
                return null;
            }
            else
            {
                var bookDTOs = books.Select(BookMapper.BookToBookDTO);
                await cacheService.SetAsync(cacheKey, bookDTOs, TimeSpan.FromMinutes(30), cancellationToken);
                return bookDTOs;
            }
        }

        public async Task<long> UpdateEntityByIdAsync(string entityId, BookDTO book, CancellationToken cancellationToken)
        {
            if (book != null)
            {
                var bookEntity = BookMapper.BookDTOToBook(book);

                var result = await unitOfWork.BookRepository.UpdateAsync(entityId, bookEntity, cancellationToken).ConfigureAwait(false);

                if (result > 0)
                {
                    // This may also required for search case implementation
                    // Invalidate cache
                    await InvalidateBookCacheForSingleEntityAsync(entityId, cancellationToken);
                    
                    // Publish message
                    var message = new BookUpdatedMessage
                    {
                        BookId = entityId,
                        BookData = book,
                        Timestamp = DateTime.UtcNow,
                        CorrelationId = Guid.NewGuid().ToString()
                    };

                    await messagePublisher.PublishAsync(message, cancellationToken).ConfigureAwait(false);
                }

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
                {
                    // Invalidate all book cache
                    await InvalidateBookCacheForMultipleEntityAsync(cancellationToken);

                    // Publish messages
                    var messages = bookEntities.Select(b => new BookUpdatedMessage
                    {
                        BookId = b.Id,
                        BookData = BookMapper.BookToBookDTO(b),
                        Timestamp = DateTime.UtcNow,
                        CorrelationId = Guid.NewGuid().ToString()
                    });

                    await messagePublisher.PublishAsync(messages, cancellationToken).ConfigureAwait(false);
                }

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
                    // Invalidate book list cache
                    // This may also required for search case implementation, but individual items
                    await cacheService.RemoveAsync("book:all", cancellationToken);
                    
                    // Publish message
                    var message = new BookCreatedMessage
                    {
                        BookId = result.Id,
                        BookData = BookMapper.BookToBookDTO(result),
                        Timestamp = DateTime.UtcNow,
                        CorrelationId = Guid.NewGuid().ToString()
                    };
                    await messagePublisher.PublishAsync(message, cancellationToken).ConfigureAwait(false);

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
                    // Invalidate all book cache
                    await InvalidateBookCacheForMultipleEntityAsync(cancellationToken);

                    // Publish messages
                    var messages = result.Select(b => new BookCreatedMessage
                    {
                        BookId = b.Id,
                        BookData = BookMapper.BookToBookDTO(b),
                        Timestamp = DateTime.UtcNow,
                        CorrelationId = Guid.NewGuid().ToString()
                    });
                    await messagePublisher.PublishAsync(messages, cancellationToken).ConfigureAwait(false);

                    return result.Select(BookMapper.BookToBookDTO);
                }
            }

            return null;
        }

        public async Task<long> DeleteEntityByIdAsync(string entityId, CancellationToken cancellationToken)
        {
            var result = await unitOfWork.BookRepository.DeleteEntityByIdAsync(entityId, cancellationToken).ConfigureAwait(false);

            if (result > 0)
            {
                // Invalidate cache
                await InvalidateBookCacheForSingleEntityAsync(entityId, cancellationToken);
            }

            return result;
        }

        public async Task<long> DeleteEntitiesAsync(CancellationToken cancellationToken)
        {
            var result = await unitOfWork.BookRepository.DeleteEntitiesAsync(cancellationToken).ConfigureAwait(false);

            if (result > 0)
            {
                // Invalidate all book cache
                await InvalidateBookCacheForMultipleEntityAsync(cancellationToken);
            }

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
                    // Invalidate all book cache
                    await InvalidateBookCacheForMultipleEntityAsync(cancellationToken);
                    return result.Select(BookMapper.BookToBookDTO);
                }
            }

            return null;
        }

        /// <summary>
        /// This should be used to Invalidate Book Cache For a SINGLE Entity UPDATE, DELETE, CREATE
        /// When book with id 123 UPDATED 1. book:123  → now stale 2. book:all  → may contain the old version of book 123
        /// When book with id 123 DELETED 1. book:123  → now deleted 2. book:all  → list could still contain the deleted book 123
        /// When book with id 123 CREATED 1. book:all  → list will NOT contain the new book 123, so needs to be removed for book referesh 
        /// </summary>
        private async Task InvalidateBookCacheForSingleEntityAsync(string bookId, CancellationToken cancellationToken)
        {
            await cacheService.RemoveAsync($"book:{bookId}", cancellationToken);
            await cacheService.RemoveAsync("book:all", cancellationToken);
        }

        /// <summary>
        /// This should be used to Invalidate Book Cache For a MULTIPLE Entity UPDATE, DELETE, CREATE
        /// Cache entries could become stale after ACTION
        /// Something has changed across the Book collection. I don't know which individual cached Book keys exist, so invalidate all Book-related cache entries.
        /// </summary>
        private async Task InvalidateBookCacheForMultipleEntityAsync(CancellationToken cancellationToken)
        {
            //book:* => book:serach:key + book:all + book:bookId
            await cacheService.RemoveByPatternAsync("book:*", cancellationToken);
        }
    }
}
