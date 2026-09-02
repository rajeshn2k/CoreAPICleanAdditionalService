#nullable disable

namespace Core.Library.Clean.AdditionalService
{
    public class PersonDirector : IEntityDirector<PersonDTO, PersonCreateDTO>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMessagePublisher messagePublisher;

        public PersonDirector(IUnitOfWork unitOfWork, IMessagePublisher messagePublisher)
        {
            this.unitOfWork = unitOfWork;
            this.messagePublisher = messagePublisher;
        }

        public async Task<IEnumerable<PersonDTO>> GetEntitiesAsync(CancellationToken cancellationToken)
        {
            var persons = await unitOfWork.PersonRepository.GetEntitiesAsync(cancellationToken);

            var personDTOs = persons.Select(PersonMapper.PersonToPersonDTO);

            if (personDTOs == null)
            {
                return Enumerable.Empty<PersonDTO>();
            }

            return personDTOs.OrderBy(person => person.firstName);
        }

        public async Task<PersonDTO> GetEntityByIdAsync(string entityId, CancellationToken cancellationToken)
        {
            var result = await unitOfWork.PersonRepository.GetEntityByIdAsync(entityId, cancellationToken).ConfigureAwait(false);
            return PersonMapper.PersonToPersonDTO(result);
        }

        public async Task<IEnumerable<PersonDTO>> SearchEntitiesAsync(string searchValue, CancellationToken cancellationToken)
        {
            var results = await unitOfWork.PersonRepository.SearchEntitiesAsync(searchValue, cancellationToken).ConfigureAwait(false);
            return results.Select(PersonMapper.PersonToPersonDTO);
        }

        public async Task<long> UpdateEntityByIdAsync(string entityId, PersonDTO person, CancellationToken cancellationToken)
        {
            var personEntity = PersonMapper.PersonDTOToPerson(person);
            var result = await unitOfWork.PersonRepository.UpdateAsync(entityId, personEntity, cancellationToken).ConfigureAwait(false);
            return result;
        }

        public async Task<long> UpdateEntitiesAsync(string searchValue, IEnumerable<PersonDTO> persons, CancellationToken cancellationToken)
        {
            var personEntities = persons.Select(PersonMapper.PersonDTOToPerson);
            var result = await unitOfWork.PersonRepository.UpdateManyAsync(personEntities, cancellationToken).ConfigureAwait(false);
            return result;
        }

        public async Task<PersonDTO> CreateEntityAsync(PersonCreateDTO person, CancellationToken cancellationToken)
        {
            if (person != null)
            {
                var personEntity = PersonMapper.PersonCreateDTOToPerson(person);

                var result = await unitOfWork.PersonRepository.CreateEntityAsync(personEntity, cancellationToken).ConfigureAwait(false);

                await messagePublisher.PublishAsync(person, MessageTypeConstant.PersonType, MessageActionConstant.Create, cancellationToken).ConfigureAwait(false);

                return PersonMapper.PersonToPersonDTO(result);

                //await unitOfWork.CommitAsync(cancellationToken).ConfigureAwait(false); not required since save changes, but due to this intercepter not happening
            }

            return null;
        }

        public async Task<IEnumerable<PersonDTO>> CreateEntitiesAsync(IEnumerable<PersonCreateDTO> persons, CancellationToken cancellationToken)
        {
            if (persons != null)
            {
                var personEntities = persons.Select(PersonMapper.PersonCreateDTOToPerson);

                var result = await unitOfWork.PersonRepository.CreateEntitiesAsync(personEntities, cancellationToken).ConfigureAwait(false);

                await messagePublisher.PublishAsync(persons, MessageTypeConstant.PersonType, MessageActionConstant.Create, cancellationToken).ConfigureAwait(false);

                return result.Select(PersonMapper.PersonToPersonDTO);
            }

            return null;
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

        public async Task<IEnumerable<PersonDTO>> LoadAllEntityForNewDatabase(CancellationToken cancellationToken)
        {
            IEnumerable<Person> persons = DatabaseInitializerPerson.GetPersons();

            var result = await unitOfWork.PersonRepository.CreateEntitiesAsync(persons, cancellationToken).ConfigureAwait(false);

            return result.Select(PersonMapper.PersonToPersonDTO);
        }
    }
}
