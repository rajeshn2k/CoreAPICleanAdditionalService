using Microsoft.Extensions.Logging;
using Core.Library.Clean.AdditionalService.Messaging.Contracts;

namespace Core.Library.Clean.AdditionalService
{
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
            try
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
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in GetEntitiesAsync, falling back to database");
                var books = await unitOfWork.BookRepository.GetEntitiesAsync(cancellationToken);
                return books?.Select(BookMapper.BookToBookDTO);
            }
        }

        public async Task<BookDTO> GetEntityByIdAsync(string entityId, CancellationToken cancellationToken)
        {
            try
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
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in GetEntityByIdAsync, falling back to database");
                var book = await unitOfWork.BookRepository.GetEntityByIdAsync(entityId, cancellationToken).ConfigureAwait(false);
                return book != null ? BookMapper.BookToBookDTO(book) : null;
            }
        }

        public async Task<IEnumerable<BookDTO>> SearchEntitiesAsync(string searchValue, CancellationToken cancellationToken)
        {
            try
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
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in SearchEntitiesAsync, falling back to database");
                var books = await unitOfWork.BookRepository.SearchEntitiesAsync(searchValue, cancellationToken).ConfigureAwait(false);
                return books?.Select(BookMapper.BookToBookDTO);
            }
        }
        public async Task<IEnumerable<BookDTO>> SearchEntitiesByForeignIdAsync(string personId, CancellationToken cancellationToken)
        {
            try
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
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in SearchEntitiesByForeignIdAsync, falling back to database");
                var books = await unitOfWork.BookRepository.SearchEntitiesByForeignIdAsync(personId, cancellationToken).ConfigureAwait(false);
                return books?.Select(BookMapper.BookToBookDTO);
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
                    // Invalidate cache
                    await InvalidateBookCacheAsync(entityId, cancellationToken);
                    
                    // Publish message
                    var message = new Messaging.Contracts.BookUpdatedMessage
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
                    await cacheService.RemoveByPatternAsync("book:*", cancellationToken);
                    
                    // Publish messages
                    var messages = bookEntities.Select(b => new Messaging.Contracts.BookUpdatedMessage
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
                    await cacheService.RemoveAsync("book:all", cancellationToken);
                    
                    // Publish message
                    var message = new Messaging.Contracts.BookCreatedMessage
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
                    await cacheService.RemoveByPatternAsync("book:*", cancellationToken);
                    
                    // Publish messages
                    var messages = result.Select(b => new Messaging.Contracts.BookCreatedMessage
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
                await InvalidateBookCacheAsync(entityId, cancellationToken);
            }

            return result;
        }

        public async Task<long> DeleteEntitiesAsync(CancellationToken cancellationToken)
        {
            var result = await unitOfWork.BookRepository.DeleteEntitiesAsync(cancellationToken).ConfigureAwait(false);

            if (result > 0)
            {
                // Invalidate all book cache
                await cacheService.RemoveByPatternAsync("book:*", cancellationToken);
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
                    await cacheService.RemoveByPatternAsync("book:*", cancellationToken);
                    return result.Select(BookMapper.BookToBookDTO);
                }
            }

            return null;
        }

        private async Task InvalidateBookCacheAsync(string bookId, CancellationToken cancellationToken)
        {
            await cacheService.RemoveAsync($"book:{bookId}", cancellationToken);
            await cacheService.RemoveAsync("book:all", cancellationToken);
        }
    }
}
