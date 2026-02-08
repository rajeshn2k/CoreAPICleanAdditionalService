using Core.Library.ArivuTharavuThalam;
using System.Linq.Expressions;

namespace Core.API.AdditionalServiceLibrary
{
    /// <summary>
    /// EntityFrameworkBookRepository (this class) is the implementation of IEntityGenericRepository
    /// EntityFrameworkBookRepository (this class) is to provide a wrapper for EntityFrameworkGenericRepository for BOOK MODEL
    /// EntityFrameworkGenericRepository has all the actual LOGIC implementation
    /// EntityFrameworkBookRepository (this class)  ONLY should be used in DIRECTOR
    /// TO Do - SearchEntitiesAsync
    /// TO DO - UpdateManyAsync
    /// </summary>
    /// <param name="sqlDbContext"></param>
    public class EntityFrameworkBookRepository(SqlDataBaseDataContext sqlDbContext) :
        EntityFrameworkGenericRepository<Book>(sqlDbContext), IEntityGenericRepository<Book>
    {
        public async Task<long> DeleteEntitiesAsync(CancellationToken cancellationToken)
        {
            return await DeleteAllAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<long> DeleteEntityByIdAsync(string entityId, CancellationToken cancellationToken)
        {
            return await DeleteAsync(book => book.Id == entityId, cancellationToken).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Book>> SearchEntitiesAsync(string searchValue, CancellationToken cancellationToken)
        {
            return await FindAllAsync(book => book.bookCategory == searchValue, cancellationToken).ConfigureAwait(false);
        }

        public async Task<Book> GetEntityByIdAsync(string entityId, CancellationToken cancellationToken)
        {
            return await FindOneAsync(book => book.Id == entityId, cancellationToken).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Book>> GetEntitiesAsync(CancellationToken cancellationToken)
        {
            return await GetAllAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<long> CreateEntityAsync(Book entity, CancellationToken cancellationToken)
        {
            entity.Id = Guid.NewGuid().ToString();
            entity.dateCreated = DateTime.Now;

            return await InsertAsync(entity, cancellationToken).ConfigureAwait(false);
        }

        public async Task<long> CreateEntitiesAsync(IEnumerable<Book> entityies, CancellationToken cancellationToken)
        {
            foreach (Book book in entityies)
            {
                book.Id = Guid.NewGuid().ToString();
                book.dateCreated = DateTime.Now;
            }

            return await InsertManyAsync(entityies, cancellationToken).ConfigureAwait(false);
        }

        public async Task<long> UpdateAsync(string entityId, Book entity, CancellationToken cancellationToken)
        {
            return await UpdateAsync(book => book.Id == entityId, entity, cancellationToken).ConfigureAwait(false);
        }

        public new async Task<long> UpdateManyAsync(Expression<Func<Book, bool>> predicate, IEnumerable<Book> entityies, CancellationToken cancellationToken)
        {
            return await UpdateManyAsync(predicate, entityies, cancellationToken).ConfigureAwait(false);
        }
    }
}
