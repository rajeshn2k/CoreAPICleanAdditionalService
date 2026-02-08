using Core.Library.ArivuTharavuThalam;
using System.Linq.Expressions;

namespace Core.API.AdditionalServiceLibrary
{
    /// <summary>
    /// EntityFrameworkPersonRepository (this class) is the implementation of IEntityGenericRepository
    /// EntityFrameworkPersonRepository (this class) is to provide a wrapper for EntityFrameworkGenericRepository for PERSON MODEL
    /// EntityFrameworkGenericRepository has all the actual LOGIC implementation
    /// EntityFrameworkPersonRepository (this class)  ONLY should be used in DIRECTOR
    /// TO Do - SearchEntitiesAsync
    /// TO DO - UpdateManyAsync
    /// </summary>
    /// <param name="sqlDbContext"></param>
    public class EntityFrameworkPersonRepository(SqlDataBaseDataContext sqlDbContext)
        : EntityFrameworkGenericRepository<Person>(sqlDbContext), IEntityGenericRepository<Person>
    {
        public async Task<long> DeleteEntitiesAsync(CancellationToken cancellationToken)
        {
            return await DeleteAllAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<long> DeleteEntityByIdAsync(string entityId, CancellationToken cancellationToken)
        {
            return await DeleteAsync(person => person.Id == entityId, cancellationToken).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Person>> SearchEntitiesAsync(string searchValue, CancellationToken cancellationToken)
        {
            return await FindAllAsync(person => person.category == searchValue, cancellationToken).ConfigureAwait(false);
        }

        public async Task<Person> GetEntityByIdAsync(string entityId, CancellationToken cancellationToken)
        {
            return await FindOneAsync(person => person.Id == entityId, cancellationToken).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Person>> GetEntitiesAsync(CancellationToken cancellationToken)
        {
            return await GetAllAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<long> CreateEntityAsync(Person entity, CancellationToken cancellationToken)
        {
            entity.Id = Guid.NewGuid().ToString();
            entity.dateCreated = DateTime.Now;

            return await InsertAsync(entity, cancellationToken).ConfigureAwait(false);
        }

        public async Task<long> CreateEntitiesAsync(IEnumerable<Person> entityies, CancellationToken cancellationToken)
        {
            foreach (Person person in entityies)
            {
                person.Id = Guid.NewGuid().ToString();
                person.dateCreated = DateTime.Now;
            }

            return await InsertManyAsync(entityies, cancellationToken).ConfigureAwait(false);
        }

        public async Task<long> UpdateAsync(string entityId, Person entity, CancellationToken cancellationToken)
        {
            return await UpdateAsync(person => person.Id == entityId, entity, cancellationToken).ConfigureAwait(false);
        }

        public new async Task<long> UpdateManyAsync(Expression<Func<Person, bool>> predicate, IEnumerable<Person> entityies, CancellationToken cancellationToken)
        {
            return await UpdateManyAsync(predicate, entityies, cancellationToken).ConfigureAwait(false);
        }
    }
}
