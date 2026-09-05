using Core.Library.Clean.AdditionalService;
using Microsoft.AspNetCore.Mvc;

namespace Core.API.Clean.AdditionalService.Controllers
{
    /// <summary>
    /// PersonController
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController(PersonDirector personDirector) : ControllerBase
    {
        private readonly PersonDirector personDirector = personDirector;

        [HttpGet]
        public async Task<IEnumerable<PersonDTO>> Get()
        {
            var result = await personDirector.GetEntitiesAsync(default).ConfigureAwait(false);
            return result;
        }

        [HttpGet("{personId}")]
        public async Task<PersonDTO> GetById(string personId)
        {
            var result = await personDirector.GetEntityByIdAsync(personId, default).ConfigureAwait(false);
            return result;
        }

        [HttpGet("SearchByPerson/{searchVale}")]
        public async Task<IEnumerable<PersonDTO>> SearchByPerson(string searchVale)
        {
            var result = await personDirector.SearchEntitiesAsync(searchVale, default).ConfigureAwait(false);
            return result;
        }

        [HttpGet("SearchByBookId/{bookId}")]
        public async Task<IEnumerable<PersonDTO>> SearchByBookId(string bookId)
        {
            var result = await personDirector.SearchEntitiesByForeignIdAsync(bookId, default).ConfigureAwait(false);
            return result;
        }

        [HttpPut("{personId}")]
        public async Task<long> Put(string personId, PersonDTO person)
        {
            var result = await personDirector.UpdateEntityByIdAsync(personId, person, default).ConfigureAwait(false);
            return result;
        }

        //[HttpPut("Many")]
        //public async Task<long> PutMany(IEnumerable<string> personsIds, IEnumerable<PersonDTO> persons)
        //{
        //    var result = await personDirector.UpdateEntitiesAsync(personsIds, persons, default).ConfigureAwait(false);
        //    return result;
        //}

        [HttpPost]
        public async Task<PersonDTO> Post(PersonCreateDTO person)
        {
            var personresult = await personDirector.CreateEntityAsync(person, default).ConfigureAwait(false);
            return personresult;
        }

        [HttpPost("Many")]
        public async Task<IEnumerable<PersonDTO>> PostMany(IEnumerable<PersonCreateDTO> persons)
        {
            var personresult = await personDirector.CreateEntitiesAsync(persons, default).ConfigureAwait(false);
            return personresult;
        }

        [HttpDelete("{personId}")]
        public async Task<long> Delete(string personId)
        {
            var result = await personDirector.DeleteEntityByIdAsync(personId, default).ConfigureAwait(false);
            return result;
        }

        [HttpDelete("Many")]
        public async Task<long> DeleteMany()
        {
            var result = await personDirector.DeleteEntitiesAsync(default).ConfigureAwait(false);
            return result;
        }

        [HttpGet("LoadAllPersonForNewDatabase")]
        public async Task<IEnumerable<PersonDTO>> LoadAllPersonForNewDatabase()
        {
            var result = await personDirector.LoadAllEntityForNewDatabase(default).ConfigureAwait(false);
            return result;
        }
    }
}
