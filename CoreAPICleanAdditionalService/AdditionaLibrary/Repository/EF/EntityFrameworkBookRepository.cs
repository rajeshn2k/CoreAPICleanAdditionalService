using Microsoft.EntityFrameworkCore;

namespace Core.Library.Clean.AdditionalService
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
    public class EntityFrameworkBookRepository(DbContext sqlDbContext) :
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

        public async Task<Book> CreateEntityAsync(Book entity, CancellationToken cancellationToken)
        {
            try
            {
                entity.Id = string.IsNullOrEmpty(entity.Id) ? Guid.NewGuid().ToString() : entity.Id;
                entity.dateCreated = DateTime.Now;

                var result = await InsertAsync(entity, cancellationToken).ConfigureAwait(false);

                if (result > 0)
                    return entity;
                else
                    return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while creating the entity {entity.Id}: {ex.Message}");
                return null;
            }
        }

        public async Task<IEnumerable<Book>> CreateEntitiesAsync(IEnumerable<Book> entityies, CancellationToken cancellationToken)
        {
            try
            {
                var entityiesCopy = entityies.ToList();

                foreach (Book book in entityiesCopy)
                {
                    book.Id = string.IsNullOrEmpty(book.Id) ? Guid.NewGuid().ToString() : book.Id;
                    book.dateCreated = DateTime.Now;
                }

                var result = await InsertManyAsync(entityiesCopy, cancellationToken).ConfigureAwait(false);

                if (result > 0)
                    return entityiesCopy;
                else
                    return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while creating entities {string.Join(",", entityies.Select(b => b.Id))}: {ex.Message}");
                return null;
            }
        }

        public async Task<long> UpdateAsync(string entityId, Book entity, CancellationToken cancellationToken)
        {
            try
            {
                return await UpdateAsync(book => book.Id == entityId, entity, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while creating the entity {entity.Id}: {ex.Message}");
                return 0;
            }
        }

        public new async Task<long> UpdateManyAsync(IEnumerable<Book> entityies, CancellationToken cancellationToken)
        {
            try
            {
                return await base.UpdateManyAsync(entityies, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while creating entities {string.Join(",", entityies.Select(b => b.Id))}: {ex.Message}");
                return 0;
            }
        }
    }
}
