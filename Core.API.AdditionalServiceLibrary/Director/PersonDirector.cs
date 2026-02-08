using Core.Library.ArivuTharavuThalam;
#nullable disable

namespace Core.API.AdditionalServiceLibrary
{
    public class PersonDirector : IEntityDirector<Person>
    {
        private readonly IUnitOfWork unitOfWork;

        public PersonDirector(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Person>> GetEntitiesAsync(CancellationToken cancellationToken)
        {
            var cacheData = await unitOfWork.PersonRepository.GetEntitiesAsync(cancellationToken);

            return cacheData.OrderBy(person => person.firstName);
        }

        public async Task<Person> GetEntityByIdAsync(string entityId, CancellationToken cancellationToken)
        {
            var result = await unitOfWork.PersonRepository.GetEntityByIdAsync(entityId, cancellationToken).ConfigureAwait(false);
            return result;
        }

        public async Task<IEnumerable<Person>> SearchEntitiesAsync(string searchValue, CancellationToken cancellationToken)
        {
            var results = await unitOfWork.PersonRepository.SearchEntitiesAsync(searchValue, cancellationToken).ConfigureAwait(false);
            return results;
        }

        public async Task<long> UpdateEntityByIdAsync(string entityId, Person person, CancellationToken cancellationToken)
        {
            var result = await unitOfWork.PersonRepository.UpdateAsync(entityId, person, cancellationToken).ConfigureAwait(false);
            return result;
        }

        public async Task<long> UpdateEntitiesAsync(string searchValue, IEnumerable<Person> persons, CancellationToken cancellationToken)
        {
            var result = await unitOfWork.PersonRepository.UpdateManyAsync(persons => true, persons, cancellationToken).ConfigureAwait(false);
            return result;
        }

        public async Task<Person> CreateEntityAsync(Person person, CancellationToken cancellationToken)
        {
            if (person != null)
            {
                await unitOfWork.PersonRepository.CreateEntityAsync(person, cancellationToken).ConfigureAwait(false);
                //await unitOfWork.CommitAsync(cancellationToken).ConfigureAwait(false); not required since save changes, but due to this intercepter not happening

                //if (!configuration.IsCurrentMessageTypeEmpty())
                //{
                //    await messagePublisher.PublishAsync(person, MessageTypeConstant.PersonType, MessageActionConstant.Create, cancellationToken).ConfigureAwait(false);
                //}
            }

            return person;
        }

        public async Task<IEnumerable<Person>> CreateEntitiesAsync(IEnumerable<Person> persons, CancellationToken cancellationToken)
        {
            if (persons != null)
            {
                await unitOfWork.PersonRepository.CreateEntitiesAsync(persons, cancellationToken).ConfigureAwait(false);

                //MessageActionConstant.CreateMany,
                //if (!configuration.IsCurrentMessageTypeEmpty())
                //{
                //    await messagePublisher.PublishAsync(persons, MessageTypeConstant.PersonType, MessageActionConstant.Create, cancellationToken).ConfigureAwait(false);
                //}
            }

            return persons;
        }

        public async Task<long> DeleteEntityByIdAsync(string entityId, CancellationToken cancellationToken)
        {
            var result = await unitOfWork.PersonRepository.DeleteEntityByIdAsync(entityId, cancellationToken).ConfigureAwait(false);

            return result;
        }

        public async Task<long> DeleteEntitiesAsync(CancellationToken cancellationToken)
        {
            var result = await unitOfWork.PersonRepository.DeleteEntitiesAsync(cancellationToken).ConfigureAwait(false);

            return result;
        }

        public async Task<IEnumerable<Person>> LoadAllEntityForNewDatabase(CancellationToken cancellationToken)
        {
            IEnumerable<Person> persons = DatabaseInitializerPerson.GetPersons();

            var result = await CreateEntitiesAsync(persons, cancellationToken).ConfigureAwait(false);

            return result;
        }
    }
}
