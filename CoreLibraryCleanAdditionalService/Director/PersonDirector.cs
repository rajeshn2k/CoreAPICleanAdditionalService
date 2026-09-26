using Microsoft.Extensions.Logging;
using Core.Library.Clean.AdditionalService.Messaging.Contracts;

namespace Core.Library.Clean.AdditionalService
{
    public class PersonDirector : IEntityDirector<PersonDTO, PersonCreateDTO>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMessagePublisher messagePublisher;
        private readonly ICacheService cacheService;
        private readonly ILogger<PersonDirector> logger;

        public PersonDirector(IUnitOfWork unitOfWork, IMessagePublisher messagePublisher, ICacheService cacheService, ILogger<PersonDirector> logger)
        {
            this.unitOfWork = unitOfWork;
            this.messagePublisher = messagePublisher;
            this.cacheService = cacheService;
            this.logger = logger;
        }

        public async Task<IEnumerable<PersonDTO>> GetEntitiesAsync(CancellationToken cancellationToken)
        {
            try
            {
                var cacheKey = "person:all";
                var cachedPersons = await cacheService.GetAsync<IEnumerable<PersonDTO>>(cacheKey, cancellationToken);
                
                if (cachedPersons != null)
                {
                    logger.LogDebug("Cache hit for key: {CacheKey}", cacheKey);
                    return cachedPersons;
                }

                var persons = await unitOfWork.PersonRepository.GetEntitiesAsync(cancellationToken);

                if (persons != null)
                {
                    var personDTOs = persons.Select(PersonMapper.PersonToPersonDTO);
                    await cacheService.SetAsync(cacheKey, personDTOs, TimeSpan.FromMinutes(30), cancellationToken);
                    return personDTOs;
                }

                return null;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in GetEntitiesAsync, falling back to database");
                var persons = await unitOfWork.PersonRepository.GetEntitiesAsync(cancellationToken);
                return persons?.Select(PersonMapper.PersonToPersonDTO);
            }
        }

        public async Task<PersonDTO> GetEntityByIdAsync(string entityId, CancellationToken cancellationToken)
        {
            try
            {
                var cacheKey = $"person:{entityId}";
                var cachedPerson = await cacheService.GetAsync<PersonDTO>(cacheKey, cancellationToken);
                
                if (cachedPerson != null)
                {
                    logger.LogDebug("Cache hit for key: {CacheKey}", cacheKey);
                    return cachedPerson;
                }

                var result = await unitOfWork.PersonRepository.GetEntityByIdAsync(entityId, cancellationToken).ConfigureAwait(false);

                if (result != null)
                {
                    var personDTO = PersonMapper.PersonToPersonDTO(result);
                    await cacheService.SetAsync(cacheKey, personDTO, TimeSpan.FromMinutes(60), cancellationToken);
                    return personDTO;
                }

                return null;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in GetEntityByIdAsync, falling back to database");
                var result = await unitOfWork.PersonRepository.GetEntityByIdAsync(entityId, cancellationToken).ConfigureAwait(false);
                return result != null ? PersonMapper.PersonToPersonDTO(result) : null;
            }
        }

        public async Task<IEnumerable<PersonDTO>> SearchEntitiesAsync(string searchValue, CancellationToken cancellationToken)
        {
            try
            {
                var cacheKey = $"person:search:{searchValue}";
                var cachedPersons = await cacheService.GetAsync<IEnumerable<PersonDTO>>(cacheKey, cancellationToken);
                
                if (cachedPersons != null)
                {
                    logger.LogDebug("Cache hit for key: {CacheKey}", cacheKey);
                    return cachedPersons;
                }

                var results = await unitOfWork.PersonRepository.SearchEntitiesAsync(searchValue, cancellationToken).ConfigureAwait(false);

                if (results != null)
                {
                    var personDTOs = results.Select(PersonMapper.PersonToPersonDTO);
                    await cacheService.SetAsync(cacheKey, personDTOs, TimeSpan.FromMinutes(15), cancellationToken);
                    return personDTOs;
                }

                return null;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in SearchEntitiesAsync, falling back to database");
                var results = await unitOfWork.PersonRepository.SearchEntitiesAsync(searchValue, cancellationToken).ConfigureAwait(false);
                return results?.Select(PersonMapper.PersonToPersonDTO);
            }
        }

        public async Task<IEnumerable<PersonDTO>> SearchEntitiesByForeignIdAsync(string bookId, CancellationToken cancellationToken)
        {
            try
            {
                var cacheKey = $"person:book:{bookId}";
                var cachedPersons = await cacheService.GetAsync<IEnumerable<PersonDTO>>(cacheKey, cancellationToken);
                
                if (cachedPersons != null)
                {
                    logger.LogDebug("Cache hit for key: {CacheKey}", cacheKey);
                    return cachedPersons;
                }

                var book = await unitOfWork.BookRepository.GetEntityByIdAsync(bookId, cancellationToken).ConfigureAwait(false);

                if (book != null)
                {
                    var result = await unitOfWork.PersonRepository.GetEntityByIdAsync(book.personId, cancellationToken).ConfigureAwait(false);

                    if (result != null)
                    {
                        var personDTOs = new[] { PersonMapper.PersonToPersonDTO(result) };
                        await cacheService.SetAsync(cacheKey, personDTOs, TimeSpan.FromMinutes(30), cancellationToken);
                        return personDTOs;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in SearchEntitiesByForeignIdAsync, falling back to database");
                var book = await unitOfWork.BookRepository.GetEntityByIdAsync(bookId, cancellationToken).ConfigureAwait(false);

                if (book != null)
                {
                    var result = await unitOfWork.PersonRepository.GetEntityByIdAsync(book.personId, cancellationToken).ConfigureAwait(false);

                    if (result != null)
                    {
                        return new[] { PersonMapper.PersonToPersonDTO(result) };
                    }
                }

                return null;
            }
        }

        public async Task<long> UpdateEntityByIdAsync(string entityId, PersonDTO person, CancellationToken cancellationToken)
        {
            if (person != null)
            {
                var personEntity = PersonMapper.PersonDTOToPerson(person);

                var result = await unitOfWork.PersonRepository.UpdateAsync(entityId, personEntity, cancellationToken).ConfigureAwait(false);

                if (result > 0)
                {
                    // Invalidate cache
                    await InvalidatePersonCacheAsync(entityId, cancellationToken);
                    
                    // Publish message
                    var message = new Messaging.Contracts.PersonUpdatedMessage
                    {
                        PersonId = entityId,
                        PersonData = person,
                        Timestamp = DateTime.UtcNow,
                        CorrelationId = Guid.NewGuid().ToString()
                    };
                    await messagePublisher.PublishAsync(message, cancellationToken).ConfigureAwait(false);
                }

                return result;
            }

            return 0;
        }

        public async Task<long> UpdateEntitiesAsync(IEnumerable<string> personsIds, IEnumerable<PersonDTO> persons, CancellationToken cancellationToken)
        {
            if (persons != null)
            {
                var personEntities = persons.Select(PersonMapper.PersonDTOToPerson);

                var result = await unitOfWork.PersonRepository.UpdateManyAsync(personEntities, cancellationToken).ConfigureAwait(false);

                if (result > 0)
                {
                    // Invalidate all person cache
                    await cacheService.RemoveByPatternAsync("person:*", cancellationToken);
                    
                    // Publish messages
                    var messages = personEntities.Select(p => new Messaging.Contracts.PersonUpdatedMessage
                    {
                        PersonId = p.Id,
                        PersonData = PersonMapper.PersonToPersonDTO(p),
                        Timestamp = DateTime.UtcNow,
                        CorrelationId = Guid.NewGuid().ToString()
                    });
                    await messagePublisher.PublishAsync(messages, cancellationToken).ConfigureAwait(false);

                    return result;
                }
            }

            return 0;
        }

        public async Task<PersonDTO> CreateEntityAsync(PersonCreateDTO person, CancellationToken cancellationToken)
        {
            if (person != null)
            {
                var personEntity = PersonMapper.PersonCreateDTOToPerson(person);

                var result = await unitOfWork.PersonRepository.CreateEntityAsync(personEntity, cancellationToken).ConfigureAwait(false);

                if (result != null)
                {
                    // Invalidate person list cache
                    await cacheService.RemoveAsync("person:all", cancellationToken);
                    
                    // Publish message
                    var message = new Messaging.Contracts.PersonCreatedMessage
                    {
                        PersonId = result.Id,
                        PersonData = PersonMapper.PersonToPersonDTO(result),
                        Timestamp = DateTime.UtcNow,
                        CorrelationId = Guid.NewGuid().ToString()
                    };
                    await messagePublisher.PublishAsync(message, cancellationToken).ConfigureAwait(false);

                    return PersonMapper.PersonToPersonDTO(result);
                }
            }

            return null;
        }

        public async Task<IEnumerable<PersonDTO>> CreateEntitiesAsync(IEnumerable<PersonCreateDTO> persons, CancellationToken cancellationToken)
        {
            if (persons != null)
            {
                var personEntities = persons.Select(PersonMapper.PersonCreateDTOToPerson);

                var result = await unitOfWork.PersonRepository.CreateEntitiesAsync(personEntities, cancellationToken).ConfigureAwait(false);

                if (result != null)
                {
                    // Invalidate all person cache
                    await cacheService.RemoveByPatternAsync("person:*", cancellationToken);
                    
                    // Publish messages
                    var messages = result.Select(p => new Messaging.Contracts.PersonCreatedMessage
                    {
                        PersonId = p.Id,
                        PersonData = PersonMapper.PersonToPersonDTO(p),
                        Timestamp = DateTime.UtcNow,
                        CorrelationId = Guid.NewGuid().ToString()
                    });
                    await messagePublisher.PublishAsync(messages, cancellationToken).ConfigureAwait(false);

                    return result.Select(PersonMapper.PersonToPersonDTO);
                }
            }

            return null;
        }

        public async Task<long> DeleteEntityByIdAsync(string entityId, CancellationToken cancellationToken)
        {
            var result = await unitOfWork.PersonRepository.DeleteEntityByIdAsync(entityId, cancellationToken).ConfigureAwait(false);

            if (result > 0)
            {
                // Invalidate cache
                await InvalidatePersonCacheAsync(entityId, cancellationToken);
            }

            return result;
        }

        public async Task<long> DeleteEntitiesAsync(CancellationToken cancellationToken)
        {
            var result = await unitOfWork.PersonRepository.DeleteEntitiesAsync(cancellationToken).ConfigureAwait(false);

            if (result > 0)
            {
                // Invalidate all person cache
                await cacheService.RemoveByPatternAsync("person:*", cancellationToken);
            }

            return result;
        }

        public async Task<IEnumerable<PersonDTO>> LoadAllEntityForNewDatabase(CancellationToken cancellationToken)
        {
            IEnumerable<Person> persons = DatabaseInitializerPerson.GetPersons();

            var result = await unitOfWork.PersonRepository.CreateEntitiesAsync(persons, cancellationToken).ConfigureAwait(false);

            if (result != null)
            {
                // Invalidate all person cache
                await cacheService.RemoveByPatternAsync("person:*", cancellationToken);
                return result.Select(PersonMapper.PersonToPersonDTO);
            }

            return null;
        }

        private async Task InvalidatePersonCacheAsync(string personId, CancellationToken cancellationToken)
        {
            await cacheService.RemoveAsync($"person:{personId}", cancellationToken);
            await cacheService.RemoveAsync("person:all", cancellationToken);
        }
    }
}
