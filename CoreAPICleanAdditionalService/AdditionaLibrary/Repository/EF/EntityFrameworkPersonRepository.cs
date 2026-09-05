using Microsoft.EntityFrameworkCore;

namespace Core.Library.Clean.AdditionalService
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
    public class EntityFrameworkPersonRepository(DbContext sqlDbContext)
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
            searchValue = searchValue.ToLower();
            return await FindAllAsync(person => person.category.ToLower().Contains(searchValue) || person.firstName.ToLower().Contains(searchValue)
            || person.lastName.ToLower().Contains(searchValue), cancellationToken).ConfigureAwait(false);
        }


        public async Task<Person> GetEntityByIdAsync(string entityId, CancellationToken cancellationToken)
        {
            return await FindOneAsync(person => person.Id == entityId, cancellationToken).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Person>> GetEntitiesAsync(CancellationToken cancellationToken)
        {
            return await GetAllAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<Person> CreateEntityAsync(Person entity, CancellationToken cancellationToken)
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

        public async Task<IEnumerable<Person>> CreateEntitiesAsync(IEnumerable<Person> entityies, CancellationToken cancellationToken)
        {
            try
            {
                var entityiesCopy = entityies.ToList();

                foreach (Person person in entityiesCopy)
                {
                    person.Id = string.IsNullOrEmpty(person.Id) ? Guid.NewGuid().ToString() : person.Id;
                    person.dateCreated = DateTime.Now;
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

        public async Task<long> UpdateAsync(string entityId, Person entity, CancellationToken cancellationToken)
        {
            try
            {
                var entityToBeUpdated = await GetEntityByIdAsync(entityId, cancellationToken).ConfigureAwait(false);

                if (entityToBeUpdated != null)
                {
                    entityToBeUpdated.dateCreated = DateTime.Now;
                    entityToBeUpdated.dateOfBirth = entity.dateOfBirth;
                    entityToBeUpdated.firstName = entity.firstName;
                    entityToBeUpdated.lastName = entity.lastName;
                    entityToBeUpdated.isPlaySports = entity.isPlaySports;
                    entityToBeUpdated.category = entity.category;

                    return await UpdateAsync(entityToBeUpdated, cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    Console.WriteLine($"Unable to find entity in database {entity.Id}");
                    return 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while creating the entity {entity.Id}: {ex.Message}");
                return 0;
            }
        }

        public async Task<long> UpdateManyAsync(IEnumerable<string> entityIds, IEnumerable<Person> entityies, CancellationToken cancellationToken)
        {
            try
            {
                List<Person> entitiesToBeUpdated = new List<Person>();

                foreach (var entityId in entityIds)
                {
                    var entity = entityies.Where(b => b.Id == entityId).FirstOrDefault();

                    if (entity != null)
                    {
                        var entityToBeUpdated = await GetEntityByIdAsync(entityId, cancellationToken).ConfigureAwait(false);

                        if (entityToBeUpdated != null)
                        {
                            entityToBeUpdated.dateCreated = DateTime.Now;
                            entityToBeUpdated.dateOfBirth = entity.dateOfBirth;
                            entityToBeUpdated.firstName = entity.firstName;
                            entityToBeUpdated.lastName = entity.lastName;
                            entityToBeUpdated.isPlaySports = entity.isPlaySports;
                            entityToBeUpdated.category = entity.category;

                            entitiesToBeUpdated.Add(entityToBeUpdated);
                        }
                        else
                        {
                            Console.WriteLine($"Unable to find entity in database {entity.Id}");
                        }
                    }
                }

                if (entitiesToBeUpdated.Count > 0)
                {
                    return await UpdateManyAsync(entitiesToBeUpdated, cancellationToken).ConfigureAwait(false);
                }
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while creating entities {string.Join(",", entityies.Select(b => b.Id))}: {ex.Message}");
                return 0;
            }
        }

        public Task<IEnumerable<Person>> SearchEntitiesByForeignIdAsync(string foreignId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("THIS IS NOT REQUIRED");
        }
    }
}
