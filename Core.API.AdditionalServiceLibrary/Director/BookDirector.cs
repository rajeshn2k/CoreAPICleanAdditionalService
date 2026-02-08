using Core.Library.ArivuTharavuThalam;
#nullable disable

namespace Core.API.AdditionalServiceLibrary
{
    public class BookDirector : IEntityDirector<Book>
    {
        private readonly IUnitOfWork unitOfWork;

        public BookDirector(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Book>> GetEntitiesAsync(CancellationToken cancellationToken)
        {
            var books = await unitOfWork.BookRepository.GetEntitiesAsync(cancellationToken);

            return books.OrderBy(book => book.bookName);
        }

        public async Task<Book> GetEntityByIdAsync(string entityId, CancellationToken cancellationToken)
        {
            var book = await unitOfWork.BookRepository.GetEntityByIdAsync(entityId, cancellationToken).ConfigureAwait(false);
            return book;
        }

        public async Task<IEnumerable<Book>> SearchEntitiesAsync(string searchValue, CancellationToken cancellationToken)
        {
            var books = await unitOfWork.BookRepository.SearchEntitiesAsync(searchValue, cancellationToken).ConfigureAwait(false);
            return books;
        }

        public async Task<long> UpdateEntityByIdAsync(string entityId, Book book, CancellationToken cancellationToken)
        {
            var result = await unitOfWork.BookRepository.UpdateAsync(entityId, book, cancellationToken).ConfigureAwait(false);
            return result;
        }

        public async Task<long> UpdateEntitiesAsync(string searchValue, IEnumerable<Book> books, CancellationToken cancellationToken)
        {
            //TO DO - persons => true,
            var result = await unitOfWork.BookRepository.UpdateManyAsync(books => true, books, cancellationToken).ConfigureAwait(false);
            return result;
        }

        public async Task<Book> CreateEntityAsync(Book book, CancellationToken cancellationToken)
        {
            if (book != null)
            {
                await unitOfWork.BookRepository.CreateEntityAsync(book, cancellationToken).ConfigureAwait(false);

                //if (!configuration.IsCurrentMessageTypeEmpty())
                //{
                //    await messagePublisher.PublishAsync(book, MessageTypeConstant.BookType, MessageActionConstant.Create, cancellationToken).ConfigureAwait(false);
                //}
            }

            return book;
        }

        public async Task<IEnumerable<Book>> CreateEntitiesAsync(IEnumerable<Book> books, CancellationToken cancellationToken)
        {
            if (books != null)
            {
                await unitOfWork.BookRepository.CreateEntitiesAsync(books, cancellationToken).ConfigureAwait(false);

                //if (!configuration.IsCurrentMessageTypeEmpty())
                //{
                //    await messagePublisher.PublishAsync(books, MessageTypeConstant.BookType, MessageActionConstant.Create, cancellationToken).ConfigureAwait(false);
                //}
            }

            return books;
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

        public async Task<IEnumerable<Book>> LoadAllEntityForNewDatabase(CancellationToken cancellationToken)
        {
            IEnumerable<Book> books = DatabaseInitializerBook.GetBooks();

            var result = await CreateEntitiesAsync(books, cancellationToken).ConfigureAwait(false);

            return result;
        }
    }
}
