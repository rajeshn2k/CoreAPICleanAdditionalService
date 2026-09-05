using Core.Library.Clean.AdditionalService;
using Microsoft.AspNetCore.Mvc;

namespace Core.API.Clean.AdditionalService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController(BookDirector bookDirector) : ControllerBase
    {
        private readonly BookDirector bookDirector = bookDirector;

        [HttpGet]
        public async Task<IEnumerable<BookDTO>> Get()
        {
            IEnumerable<BookDTO> books = await bookDirector.GetEntitiesAsync(default).ConfigureAwait(false);
            return books;
        }

        [HttpGet("{bookId}")]
        public async Task<BookDTO> GetById(string bookId)
        {
            var result = await bookDirector.GetEntityByIdAsync(bookId, default).ConfigureAwait(false);
            return result;
        }

        [HttpGet("SearchByBook/{searchVale}")]
        public async Task<IEnumerable<BookDTO>> SearchByBook(string searchVale)
        {
            var result = await bookDirector.SearchEntitiesAsync(searchVale, default).ConfigureAwait(false);
            return result;
        }

        [HttpGet("SearchByPerson/{personId}")]
        public async Task<IEnumerable<BookDTO>> SearchByPerson(string personId)
        {
            var result = await bookDirector.SearchEntitiesByForeignIdAsync(personId, default).ConfigureAwait(false);
            return result;
        }

        [HttpPut("{bookId}")]
        public async Task<long> Put(string bookId, BookDTO book)
        {
            var result = await bookDirector.UpdateEntityByIdAsync(bookId, book, default).ConfigureAwait(false);
            return result;
        }

        //[HttpPut("Many")]
        //public async Task<long> PutMany(IEnumerable<string> bookIds, IEnumerable<BookDTO> books)
        //{
        //    var result = await bookDirector.UpdateEntitiesAsync(bookIds, books, default).ConfigureAwait(false);
        //    return result;
        //}

        [HttpPost]
        public async Task<BookDTO> Post(BookCreateDTO book)
        {
            var bookresult = await bookDirector.CreateEntityAsync(book, default).ConfigureAwait(false);
            return bookresult;
        }

        [HttpPost("Many")]
        public async Task<IEnumerable<BookDTO>> PostMany(IEnumerable<BookCreateDTO> books)
        {
            var bookresult = await bookDirector.CreateEntitiesAsync(books, default).ConfigureAwait(false);
            return bookresult;
        }

        [HttpDelete("{bookId}")]
        public async Task<long> Delete(string bookId)
        {
            var result = await bookDirector.DeleteEntityByIdAsync(bookId, default).ConfigureAwait(false);
            return result;
        }

        [HttpDelete("Many")]
        public async Task<long> DeleteAll()
        {
            var result = await bookDirector.DeleteEntitiesAsync(default).ConfigureAwait(false);
            return result;
        }

        [HttpGet("LoadAllBookForNewDatabase")]
        public async Task<IEnumerable<BookDTO>> LoadAllBookForNewDatabase()
        {
            var result = await bookDirector.LoadAllEntityForNewDatabase(default).ConfigureAwait(false);
            return result;
        }
    }
}
