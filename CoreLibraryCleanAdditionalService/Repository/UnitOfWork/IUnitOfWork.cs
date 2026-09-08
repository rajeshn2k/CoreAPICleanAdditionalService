namespace Core.Library.Clean.AdditionalService
{
    public interface IUnitOfWork
    {
        IEntityGenericRepository<Book> BookRepository { get; }

        IEntityGenericRepository<Person> PersonRepository { get; }

        /// <summary>
        /// Commits all work to data store.
        /// </summary>
        /// <param name="cancellationToken">Cancellation Token.</param>
        /// <returns>Number of rows.</returns>
        Task<int> CommitAsync(CancellationToken cancellationToken);
    }
}
