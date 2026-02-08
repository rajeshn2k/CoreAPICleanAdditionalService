using Core.API.AdditionalServiceLibrary;
using Core.Library.ArivuTharavuThalam;
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

        /// <summary>
        /// Get All Person(s)
        /// </summary>
        [HttpGet]
        public async Task<IEnumerable<Person>> Get()
        {
            var result = await personDirector.GetEntitiesAsync(default).ConfigureAwait(false);
            return result;
        }

        /// <summary>
        /// Get Single Person by Id
        /// Input - Id
        /// </summary>
        [HttpGet("{personId}")]
        public async Task<Person> Get(string personId)
        {
            var result = await personDirector.GetEntityByIdAsync(personId, default).ConfigureAwait(false);
            return result;
        }

        /// <summary>
        /// Update Single Person by Id
        /// Input - Person, Id
        /// </summary>
        [HttpPut("{personId}")]
        public async Task<long> Put(string personId, Person person)
        {
            var result = await personDirector.UpdateEntityByIdAsync(personId, person, default).ConfigureAwait(false);
            return result;
        }

        /// <summary>
        /// Update Multiple Person
        /// Input - Person(s)
        /// TODO Update based on searchValue 
        /// </summary>
        [HttpPut("Many")]
        public async Task<long> PutMany(string searchValue, IEnumerable<Person> persons)
        {
            var result = await personDirector.UpdateEntitiesAsync(searchValue, persons, default).ConfigureAwait(false);
            return result;
        }

        /// <summary>
        /// Create Single Person
        /// Input - Person
        /// </summary>
        [HttpPost]
        public async Task<Person> Post(Person person)
        {
            var personresult = await personDirector.CreateEntityAsync(person, default).ConfigureAwait(false);
            return personresult;
        }

        /// <summary>
        /// Create Multiple Person(s)
        /// Input - Person(s)
        /// </summary>
        [HttpPost("Many")]
        public async Task<IEnumerable<Person>> PostMany(IEnumerable<Person> persons)
        {
            var personresult = await personDirector.CreateEntitiesAsync(persons, default).ConfigureAwait(false);
            return personresult;
        }

        /// <summary>
        /// Delete Single Person
        /// Input - Id
        /// </summary>
        [HttpDelete("{personId}")]
        public async Task<long> Delete(string personId)
        {
            var result = await personDirector.DeleteEntityByIdAsync(personId, default).ConfigureAwait(false);
            return result;
        }

        /// <summary>
        /// Delete Multiple Person(s)
        /// </summary>
        [HttpDelete("Many")]
        public async Task<long> DeleteMany()
        {
            var result = await personDirector.DeleteEntitiesAsync(default).ConfigureAwait(false);
            return result;
        }

        /// <summary>
        /// Load and Create Multiple Person(s), Input - Static Collection
        /// </summary>
        [HttpGet("LoadAllPersonForNewDatabase")]
        public async Task<IEnumerable<Person>> LoadAllPersonForNewDatabase()
        {
            var result = await personDirector.LoadAllEntityForNewDatabase(default).ConfigureAwait(false);
            return result;
        }
    }
}
