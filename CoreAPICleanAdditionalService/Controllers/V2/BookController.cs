using Core.API.Clean.AdditionalService.Middleware;
using Core.Library.Clean.AdditionalService;
using Microsoft.AspNetCore.Mvc;

namespace Core.API.Clean.AdditionalService.Controllers.V2
{
    /// <summary>
    /// V2 Controller for managing books with standardized API response format   
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class BookController : ControllerBase
    {
        private readonly BookDirector _bookDirector;
        private readonly ILogger<BookController> _logger;

        public BookController(BookDirector bookDirector, ILogger<BookController> logger)
        {
            _bookDirector = bookDirector;
            _logger = logger;
        }

        /// <summary>
        /// Gets all books
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<BookDTO>>>> Get()
        {
            try
            {
                var correlationId = HttpContext.GetCorrelationId();
                var books = await _bookDirector.GetEntitiesAsync(default).ConfigureAwait(false);

                var response = ApiResponse<IEnumerable<BookDTO>>.CreateSuccess(
                    books,
                    "Books retrieved successfully",
                    correlationId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving books");
                var correlationId = HttpContext.GetCorrelationId();
                var errorResponse = ApiErrorResponse.CreateError(
                    ErrorCodes.INTERNAL_ERROR,
                    "An error occurred while retrieving books",
                    500,
                    correlationId,
                    HttpContext.Request.Path);

                return StatusCode(500, errorResponse);
            }
        }

        /// <summary>
        /// Gets a book by ID
        /// </summary>
        [HttpGet("{bookId}")]
        public async Task<ActionResult<ApiResponse<BookDTO>>> GetById(string bookId)
        {
            try
            {
                var correlationId = HttpContext.GetCorrelationId();
                var book = await _bookDirector.GetEntityByIdAsync(bookId, default).ConfigureAwait(false);

                if (book == null)
                {
                    var errorResponse = ApiErrorResponse.CreateError(
                        ErrorCodes.BOOK_NOT_FOUND,
                        $"Book with ID {bookId} not found",
                        404,
                        correlationId,
                        HttpContext.Request.Path);

                    return NotFound(errorResponse);
                }

                var response = ApiResponse<BookDTO>.CreateSuccess(
                    book,
                    "Book retrieved successfully",
                    correlationId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving book with ID: {BookId}", bookId);
                var correlationId = HttpContext.GetCorrelationId();
                var errorResponse = ApiErrorResponse.CreateError(
                    ErrorCodes.INTERNAL_ERROR,
                    "An error occurred while retrieving the book",
                    500,
                    correlationId,
                    HttpContext.Request.Path);

                return StatusCode(500, errorResponse);
            }
        }

        /// <summary>
        /// Search books by category or name
        /// </summary>
        [HttpGet("SearchByBook/{searchValue}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<BookDTO>>>> SearchByBook(string searchValue)
        {
            try
            {
                var correlationId = HttpContext.GetCorrelationId();
                var books = await _bookDirector.SearchEntitiesAsync(searchValue, default).ConfigureAwait(false);

                var response = ApiResponse<IEnumerable<BookDTO>>.CreateSuccess(
                    books,
                    "Books search completed successfully",
                    correlationId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching books with value: {SearchValue}", searchValue);
                var correlationId = HttpContext.GetCorrelationId();
                var errorResponse = ApiErrorResponse.CreateError(
                    ErrorCodes.INTERNAL_ERROR,
                    "An error occurred while searching books",
                    500,
                    correlationId,
                    HttpContext.Request.Path);

                return StatusCode(500, errorResponse);
            }
        }

        /// <summary>
        /// Search books by person ID
        /// </summary>
        [HttpGet("SearchByPersonId/{personId}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<BookDTO>>>> SearchByPerson(string personId)
        {
            try
            {
                var correlationId = HttpContext.GetCorrelationId();
                var books = await _bookDirector.SearchEntitiesByForeignIdAsync(personId, default).ConfigureAwait(false);

                var response = ApiResponse<IEnumerable<BookDTO>>.CreateSuccess(
                    books,
                    "Books search completed successfully",
                    correlationId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching books by person ID: {PersonId}", personId);
                var correlationId = HttpContext.GetCorrelationId();
                var errorResponse = ApiErrorResponse.CreateError(
                    ErrorCodes.INTERNAL_ERROR,
                    "An error occurred while searching books",
                    500,
                    correlationId,
                    HttpContext.Request.Path);

                return StatusCode(500, errorResponse);
            }
        }

        /// <summary>
        /// Updates a book
        /// </summary>
        [HttpPut("{bookId}")]
        public async Task<ActionResult<ApiResponse<long>>> Put(string bookId, BookDTO book)
        {
            try
            {
                var correlationId = HttpContext.GetCorrelationId();
                var result = await _bookDirector.UpdateEntityByIdAsync(bookId, book, default).ConfigureAwait(false);

                var response = ApiResponse<long>.CreateSuccess(
                    result,
                    "Book updated successfully",
                    correlationId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating book with ID: {BookId}", bookId);
                var correlationId = HttpContext.GetCorrelationId();
                var errorResponse = ApiErrorResponse.CreateError(
                    ErrorCodes.INTERNAL_ERROR,
                    "An error occurred while updating the book",
                    500,
                    correlationId,
                    HttpContext.Request.Path);

                return StatusCode(500, errorResponse);
            }
        }

        /// <summary>
        /// Creates a new book
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<BookDTO>>> Post(BookCreateDTO book)
        {
            try
            {
                var correlationId = HttpContext.GetCorrelationId();
                var bookResult = await _bookDirector.CreateEntityAsync(book, default).ConfigureAwait(false);

                if (bookResult == null)
                {
                    var errorResponse = ApiErrorResponse.CreateError(
                        ErrorCodes.INTERNAL_ERROR,
                        "Failed to create book",
                        500,
                        correlationId,
                        HttpContext.Request.Path);

                    return StatusCode(500, errorResponse);
                }

                var response = ApiResponse<BookDTO>.CreateSuccess(
                    bookResult,
                    "Book created successfully",
                    correlationId);

                return CreatedAtAction(nameof(GetById), new { bookId = bookResult.id }, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating book");
                var correlationId = HttpContext.GetCorrelationId();
                var errorResponse = ApiErrorResponse.CreateError(
                    ErrorCodes.INTERNAL_ERROR,
                    "An error occurred while creating the book",
                    500,
                    correlationId,
                    HttpContext.Request.Path);

                return StatusCode(500, errorResponse);
            }
        }

        /// <summary>
        /// Creates multiple books
        /// </summary>
        [HttpPost("Many")]
        public async Task<ActionResult<ApiResponse<IEnumerable<BookDTO>>>> PostMany(IEnumerable<BookCreateDTO> books)
        {
            try
            {
                var correlationId = HttpContext.GetCorrelationId();
                var bookResult = await _bookDirector.CreateEntitiesAsync(books, default).ConfigureAwait(false);

                var response = ApiResponse<IEnumerable<BookDTO>>.CreateSuccess(
                    bookResult,
                    "Books created successfully",
                    correlationId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating multiple books");
                var correlationId = HttpContext.GetCorrelationId();
                var errorResponse = ApiErrorResponse.CreateError(
                    ErrorCodes.INTERNAL_ERROR,
                    "An error occurred while creating books",
                    500,
                    correlationId,
                    HttpContext.Request.Path);

                return StatusCode(500, errorResponse);
            }
        }

        /// <summary>
        /// Deletes a book
        /// </summary>
        [HttpDelete("{bookId}")]
        public async Task<ActionResult<ApiResponse<long>>> Delete(string bookId)
        {
            try
            {
                var correlationId = HttpContext.GetCorrelationId();
                var result = await _bookDirector.DeleteEntityByIdAsync(bookId, default).ConfigureAwait(false);

                var response = ApiResponse<long>.CreateSuccess(
                    result,
                    "Book deleted successfully",
                    correlationId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting book with ID: {BookId}", bookId);
                var correlationId = HttpContext.GetCorrelationId();
                var errorResponse = ApiErrorResponse.CreateError(
                    ErrorCodes.INTERNAL_ERROR,
                    "An error occurred while deleting the book",
                    500,
                    correlationId,
                    HttpContext.Request.Path);

                return StatusCode(500, errorResponse);
            }
        }

        /// <summary>
        /// Deletes all books
        /// </summary>
        [HttpDelete("Many")]
        public async Task<ActionResult<ApiResponse<long>>> DeleteAll()
        {
            try
            {
                var correlationId = HttpContext.GetCorrelationId();
                var result = await _bookDirector.DeleteEntitiesAsync(default).ConfigureAwait(false);

                var response = ApiResponse<long>.CreateSuccess(
                    result,
                    "All books deleted successfully",
                    correlationId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting all books");
                var correlationId = HttpContext.GetCorrelationId();
                var errorResponse = ApiErrorResponse.CreateError(
                    ErrorCodes.INTERNAL_ERROR,
                    "An error occurred while deleting books",
                    500,
                    correlationId,
                    HttpContext.Request.Path);

                return StatusCode(500, errorResponse);
            }
        }

        /// <summary>
        /// Loads all books for new database initialization
        /// </summary>
        [HttpGet("LoadAllBookForNewDatabase")]
        public async Task<ActionResult<ApiResponse<IEnumerable<BookDTO>>>> LoadAllBookForNewDatabase()
        {
            try
            {
                var correlationId = HttpContext.GetCorrelationId();
                var result = await _bookDirector.LoadAllEntityForNewDatabase(default).ConfigureAwait(false);

                var response = ApiResponse<IEnumerable<BookDTO>>.CreateSuccess(
                    result,
                    "Books loaded successfully",
                    correlationId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading books for new database");
                var correlationId = HttpContext.GetCorrelationId();
                var errorResponse = ApiErrorResponse.CreateError(
                    ErrorCodes.INTERNAL_ERROR,
                    "An error occurred while loading books",
                    500,
                    correlationId,
                    HttpContext.Request.Path);

                return StatusCode(500, errorResponse);
            }
        }
    }
}