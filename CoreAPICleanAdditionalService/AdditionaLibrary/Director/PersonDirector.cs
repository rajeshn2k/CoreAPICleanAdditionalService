using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

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

            if (persons != null)
            {
                return persons.Select(PersonMapper.PersonToPersonDTO);
            }

            return null;
        }

        public async Task<PersonDTO> GetEntityByIdAsync(string entityId, CancellationToken cancellationToken)
        {
            var result = await unitOfWork.PersonRepository.GetEntityByIdAsync(entityId, cancellationToken).ConfigureAwait(false);

            if (result != null)
            {
                return PersonMapper.PersonToPersonDTO(result);
            }

            return null;
        }

        public async Task<IEnumerable<PersonDTO>> SearchEntitiesAsync(string searchValue, CancellationToken cancellationToken)
        {
            var results = await unitOfWork.PersonRepository.SearchEntitiesAsync(searchValue, cancellationToken).ConfigureAwait(false);

            if (results != null)
            {
                return results.Select(PersonMapper.PersonToPersonDTO);
            }

            return null;
        }

        public async Task<IEnumerable<PersonDTO>> SearchEntitiesByForeignIdAsync(string bookId, CancellationToken cancellationToken)
        {
            var book = await unitOfWork.BookRepository.GetEntityByIdAsync(bookId, cancellationToken).ConfigureAwait(false);

            if (book != null)
            {
                var result = await unitOfWork.PersonRepository.GetEntityByIdAsync(book.personId, cancellationToken).ConfigureAwait(false);

                if (result != null)
                {
                    return [PersonMapper.PersonToPersonDTO(result)];
                }
            }

            return null;
        }

        public async Task<long> UpdateEntityByIdAsync(string entityId, PersonDTO person, CancellationToken cancellationToken)
        {
            if (person != null)
            {
                var personEntity = PersonMapper.PersonDTOToPerson(person);

                var result = await unitOfWork.PersonRepository.UpdateAsync(entityId, personEntity, cancellationToken).ConfigureAwait(false);

                await messagePublisher.PublishAsync(person, MessageTypeConstant.PersonType, MessageActionConstant.Update, cancellationToken).ConfigureAwait(false);

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
                    await messagePublisher.PublishAsync(personEntities, MessageTypeConstant.PersonType, MessageActionConstant.Update, cancellationToken).ConfigureAwait(false);

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
                    await messagePublisher.PublishAsync(person, MessageTypeConstant.PersonType, MessageActionConstant.Create, cancellationToken).ConfigureAwait(false);

                    return PersonMapper.PersonToPersonDTO(result);
                }

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

                if (result != null)
                {
                    await messagePublisher.PublishAsync(persons, MessageTypeConstant.PersonType, MessageActionConstant.Create, cancellationToken).ConfigureAwait(false);

                    return result.Select(PersonMapper.PersonToPersonDTO);
                }
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

            if (result != null)
            {
                return result.Select(PersonMapper.PersonToPersonDTO);
            }

            return null;
        }
    }
}
