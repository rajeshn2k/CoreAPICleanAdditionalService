using Core.API.Clean.AdditionalService.Middleware;
using Core.Library.Clean.AdditionalService;
using Microsoft.AspNetCore.Mvc;

namespace Core.API.Clean.AdditionalService.Controllers.V2
{
    /// <summary>
    /// V2 Controller for managing persons with standardized API response format
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class PersonController : ControllerBase
    {
        private readonly PersonDirector _personDirector;
        private readonly ILogger<PersonController> _logger;

        public PersonController(PersonDirector personDirector, ILogger<PersonController> logger)
        {
            _personDirector = personDirector;
            _logger = logger;
        }

        /// <summary>
        /// Gets all persons
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<PersonDTO>>>> Get()
        {
            try
            {
                var correlationId = HttpContext.GetCorrelationId();
                var persons = await _personDirector.GetEntitiesAsync(default).ConfigureAwait(false);

                var response = ApiResponse<IEnumerable<PersonDTO>>.CreateSuccess(
                    persons,
                    "Persons retrieved successfully",
                    correlationId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving persons");
                var correlationId = HttpContext.GetCorrelationId();
                var errorResponse = ApiErrorResponse.CreateError(
                    ErrorCodes.INTERNAL_ERROR,
                    "An error occurred while retrieving persons",
                    500,
                    correlationId,
                    HttpContext.Request.Path);

                return StatusCode(500, errorResponse);
            }
        }

        /// <summary>
        /// Gets a person by ID
        /// </summary>
        [HttpGet("{personId}")]
        public async Task<ActionResult<ApiResponse<PersonDTO>>> GetById(string personId)
        {
            try
            {
                var correlationId = HttpContext.GetCorrelationId();
                var person = await _personDirector.GetEntityByIdAsync(personId, default).ConfigureAwait(false);

                if (person == null)
                {
                    var errorResponse = ApiErrorResponse.CreateError(
                        ErrorCodes.PERSON_NOT_FOUND,
                        $"Person with ID {personId} not found",
                        404,
                        correlationId,
                        HttpContext.Request.Path);

                    return NotFound(errorResponse);
                }

                var response = ApiResponse<PersonDTO>.CreateSuccess(
                    person,
                    "Person retrieved successfully",
                    correlationId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving person with ID: {PersonId}", personId);
                var correlationId = HttpContext.GetCorrelationId();
                var errorResponse = ApiErrorResponse.CreateError(
                    ErrorCodes.INTERNAL_ERROR,
                    "An error occurred while retrieving the person",
                    500,
                    correlationId,
                    HttpContext.Request.Path);

                return StatusCode(500, errorResponse);
            }
        }

        /// <summary>
        /// Search persons by category, first name, or last name
        /// </summary>
        [HttpGet("SearchByPerson/{searchValue}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<PersonDTO>>>> SearchByPerson(string searchValue)
        {
            try
            {
                var correlationId = HttpContext.GetCorrelationId();
                var persons = await _personDirector.SearchEntitiesAsync(searchValue, default).ConfigureAwait(false);

                var response = ApiResponse<IEnumerable<PersonDTO>>.CreateSuccess(
                    persons,
                    "Persons search completed successfully",
                    correlationId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching persons with value: {SearchValue}", searchValue);
                var correlationId = HttpContext.GetCorrelationId();
                var errorResponse = ApiErrorResponse.CreateError(
                    ErrorCodes.INTERNAL_ERROR,
                    "An error occurred while searching persons",
                    500,
                    correlationId,
                    HttpContext.Request.Path);

                return StatusCode(500, errorResponse);
            }
        }

        /// <summary>
        /// Search persons by book ID
        /// </summary>
        [HttpGet("SearchByBookId/{bookId}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<PersonDTO>>>> SearchByBookId(string bookId)
        {
            try
            {
                var correlationId = HttpContext.GetCorrelationId();
                var persons = await _personDirector.SearchEntitiesByForeignIdAsync(bookId, default).ConfigureAwait(false);

                var response = ApiResponse<IEnumerable<PersonDTO>>.CreateSuccess(
                    persons,
                    "Persons search completed successfully",
                    correlationId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching persons by book ID: {BookId}", bookId);
                var correlationId = HttpContext.GetCorrelationId();
                var errorResponse = ApiErrorResponse.CreateError(
                    ErrorCodes.INTERNAL_ERROR,
                    "An error occurred while searching persons",
                    500,
                    correlationId,
                    HttpContext.Request.Path);

                return StatusCode(500, errorResponse);
            }
        }

        /// <summary>
        /// Updates a person
        /// </summary>
        [HttpPut("{personId}")]
        public async Task<ActionResult<ApiResponse<long>>> Put(string personId, PersonDTO person)
        {
            try
            {
                var correlationId = HttpContext.GetCorrelationId();
                var result = await _personDirector.UpdateEntityByIdAsync(personId, person, default).ConfigureAwait(false);

                var response = ApiResponse<long>.CreateSuccess(
                    result,
                    "Person updated successfully",
                    correlationId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating person with ID: {PersonId}", personId);
                var correlationId = HttpContext.GetCorrelationId();
                var errorResponse = ApiErrorResponse.CreateError(
                    ErrorCodes.INTERNAL_ERROR,
                    "An error occurred while updating the person",
                    500,
                    correlationId,
                    HttpContext.Request.Path);

                return StatusCode(500, errorResponse);
            }
        }

        /// <summary>
        /// Creates a new person
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<PersonDTO>>> Post(PersonCreateDTO person)
        {
            try
            {
                var correlationId = HttpContext.GetCorrelationId();
                var personResult = await _personDirector.CreateEntityAsync(person, default).ConfigureAwait(false);

                if (personResult == null)
                {
                    var errorResponse = ApiErrorResponse.CreateError(
                        ErrorCodes.INTERNAL_ERROR,
                        "Failed to create person",
                        500,
                        correlationId,
                        HttpContext.Request.Path);

                    return StatusCode(500, errorResponse);
                }

                var response = ApiResponse<PersonDTO>.CreateSuccess(
                    personResult,
                    "Person created successfully",
                    correlationId);

                return CreatedAtAction(nameof(GetById), new { personId = personResult.id }, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating person");
                var correlationId = HttpContext.GetCorrelationId();
                var errorResponse = ApiErrorResponse.CreateError(
                    ErrorCodes.INTERNAL_ERROR,
                    "An error occurred while creating the person",
                    500,
                    correlationId,
                    HttpContext.Request.Path);

                return StatusCode(500, errorResponse);
            }
        }

        /// <summary>
        /// Creates multiple persons
        /// </summary>
        [HttpPost("Many")]
        public async Task<ActionResult<ApiResponse<IEnumerable<PersonDTO>>>> PostMany(IEnumerable<PersonCreateDTO> persons)
        {
            try
            {
                var correlationId = HttpContext.GetCorrelationId();
                var personResult = await _personDirector.CreateEntitiesAsync(persons, default).ConfigureAwait(false);

                var response = ApiResponse<IEnumerable<PersonDTO>>.CreateSuccess(
                    personResult,
                    "Persons created successfully",
                    correlationId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating multiple persons");
                var correlationId = HttpContext.GetCorrelationId();
                var errorResponse = ApiErrorResponse.CreateError(
                    ErrorCodes.INTERNAL_ERROR,
                    "An error occurred while creating persons",
                    500,
                    correlationId,
                    HttpContext.Request.Path);

                return StatusCode(500, errorResponse);
            }
        }

        /// <summary>
        /// Deletes a person
        /// </summary>
        [HttpDelete("{personId}")]
        public async Task<ActionResult<ApiResponse<long>>> Delete(string personId)
        {
            try
            {
                var correlationId = HttpContext.GetCorrelationId();
                var result = await _personDirector.DeleteEntityByIdAsync(personId, default).ConfigureAwait(false);

                var response = ApiResponse<long>.CreateSuccess(
                    result,
                    "Person deleted successfully",
                    correlationId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting person with ID: {PersonId}", personId);
                var correlationId = HttpContext.GetCorrelationId();
                var errorResponse = ApiErrorResponse.CreateError(
                    ErrorCodes.INTERNAL_ERROR,
                    "An error occurred while deleting the person",
                    500,
                    correlationId,
                    HttpContext.Request.Path);

                return StatusCode(500, errorResponse);
            }
        }

        /// <summary>
        /// Deletes all persons
        /// </summary>
        [HttpDelete("Many")]
        public async Task<ActionResult<ApiResponse<long>>> DeleteMany()
        {
            try
            {
                var correlationId = HttpContext.GetCorrelationId();
                var result = await _personDirector.DeleteEntitiesAsync(default).ConfigureAwait(false);

                var response = ApiResponse<long>.CreateSuccess(
                    result,
                    "All persons deleted successfully",
                    correlationId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting all persons");
                var correlationId = HttpContext.GetCorrelationId();
                var errorResponse = ApiErrorResponse.CreateError(
                    ErrorCodes.INTERNAL_ERROR,
                    "An error occurred while deleting persons",
                    500,
                    correlationId,
                    HttpContext.Request.Path);

                return StatusCode(500, errorResponse);
            }
        }

        /// <summary>
        /// Loads all persons for new database initialization
        /// </summary>
        [HttpGet("LoadAllPersonForNewDatabase")]
        public async Task<ActionResult<ApiResponse<IEnumerable<PersonDTO>>>> LoadAllPersonForNewDatabase()
        {
            try
            {
                var correlationId = HttpContext.GetCorrelationId();
                var result = await _personDirector.LoadAllEntityForNewDatabase(default).ConfigureAwait(false);

                var response = ApiResponse<IEnumerable<PersonDTO>>.CreateSuccess(
                    result,
                    "Persons loaded successfully",
                    correlationId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading persons for new database");
                var correlationId = HttpContext.GetCorrelationId();
                var errorResponse = ApiErrorResponse.CreateError(
                    ErrorCodes.INTERNAL_ERROR,
                    "An error occurred while loading persons",
                    500,
                    correlationId,
                    HttpContext.Request.Path);

                return StatusCode(500, errorResponse);
            }
        }
    }
}