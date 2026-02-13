using Core.API.AdditionalServiceLibrary;
using Core.Library.ArivuTharavuThalam;
using Microsoft.AspNetCore.Mvc;

namespace Core.API.Clean.AdditionalService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController(BookDirector bookDirector) : ControllerBase
    {
        private readonly BookDirector bookDirector = bookDirector;

        /// <summary>
        /// Get All Book(s)
        /// </summary>
        [HttpGet]
        public async Task<IEnumerable<BookDTO>> Get()
        {
            IEnumerable<BookDTO> books = await bookDirector.GetEntitiesAsync(default).ConfigureAwait(false);
            return books;
        }

        /// <summary>
        /// Get Single Book by Id
        /// Input - Id
        /// </summary>
        [HttpGet("{bookId}")]
        public async Task<BookDTO> Get(string bookId)
        {
            var result = await bookDirector.GetEntityByIdAsync(bookId, default).ConfigureAwait(false);
            return result;
        }

        /// <summary>
        /// Update Single Book by Id
        /// Input - Book, Id
        /// </summary>
        [HttpPut("{bookId}")]
        public async Task<long> Put(string bookId, BookDTO book)
        {
            var result = await bookDirector.UpdateEntityByIdAsync(bookId, book, default).ConfigureAwait(false);
            return result;
        }

        /// <summary>
        /// Update Multiple Book
        /// Input - Book(s)
        /// TODO Update based on searchValue 
        /// </summary>
        [HttpPut("Many")]
        public async Task<long> PutMany(string searchValue, IEnumerable<BookDTO> books)
        {
            var result = await bookDirector.UpdateEntitiesAsync(searchValue, books, default).ConfigureAwait(false);
            return result;
        }

        /// <summary>
        /// Create Single Book
        /// Input - Book
        /// </summary>
        [HttpPost]
        public async Task<BookDTO> Post(BookCreateDTO book)
        {
            var bookresult = await bookDirector.CreateEntityAsync(book, default).ConfigureAwait(false);
            return bookresult;
        }

        /// <summary>
        /// Create Multiple Book(s)
        /// Input - Book(s)
        /// </summary>
        [HttpPost("Many")]
        public async Task<IEnumerable<BookDTO>> PostMany(IEnumerable<BookCreateDTO> books)
        {
            var bookresult = await bookDirector.CreateEntitiesAsync(books, default).ConfigureAwait(false);
            return bookresult;
        }

        /// <summary>
        /// Delete Single Book
        /// Input - Id
        /// </summary>
        [HttpDelete("{bookId}")]
        public async Task<long> Delete(string bookId)
        {
            var result = await bookDirector.DeleteEntityByIdAsync(bookId, default).ConfigureAwait(false);
            return result;
        }

        /// <summary>
        /// Delete Multiple Book(s)
        /// </summary>
        [HttpDelete("Many")]
        public async Task<long> DeleteAll()
        {
            var result = await bookDirector.DeleteEntitiesAsync(default).ConfigureAwait(false);
            return result;
        }

        /// <summary>
        /// Load and Create Multiple Book(s), Input - Static Collection
        /// </summary>
        [HttpGet("LoadAllBookForNewDatabase")]
        public async Task<IEnumerable<BookDTO>> LoadAllBookForNewDatabase()
        {
            var result = await bookDirector.LoadAllEntityForNewDatabase(default).ConfigureAwait(false);
            return result;
        }
    }
}
