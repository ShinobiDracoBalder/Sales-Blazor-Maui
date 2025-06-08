using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SBM.API.Data;
using SBM.Shared.Entities;
using SBM.Shared.Responses;
using Serilog;
using System.Net;

namespace SBM.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController : ControllerBase
    {
        private readonly ILogger<CountriesController> _logger;
        private readonly DataContext _dataContext;

        public CountriesController(ILogger<CountriesController> logger, DataContext dataContext)
        {
            _logger = logger;
            _dataContext = dataContext;
        }

        [AllowAnonymous]
        [HttpGet("combo")]
        public async Task<ActionResult<Response<List<Country>>>> GetCombo()
        {
            try
            {
                _logger.LogInformation("Inicio del método GetCombo.");

                var countries = await _dataContext.Countries.ToListAsync();

                _logger.LogInformation($"Se recuperaron {countries.Count} países.");

                var response = new Response<List<Country>>
                {
                    IsSuccess = true,
                    StatusCode = (int)HttpStatusCode.OK,
                    Message = "Listado de países obtenido correctamente.",
                    Result = countries
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la lista de países.");

                var errorResponse = new Response<List<Country>>
                {
                    IsSuccess = false,
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Message = "Ocurrió un error interno al obtener los países.",
                    Result = null
                };

                return StatusCode((int)HttpStatusCode.InternalServerError, errorResponse);
            }
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            Response<List<Country>> response = new Response<List<Country>>();
            List<Country> countries = new List<Country>();
            _logger.LogInformation("Inicio del método Get -- Async.");
            try
            {
                countries = await _dataContext.Countries
                .Include(x => x.States)
                .ToListAsync();

                if (countries.Count == 0)
                {
                    response = new Response<List<Country>>
                    {
                        IsSuccess = true,
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Message = "Lista sin datos",
                        Result = countries
                    };
                    return Ok(response);
                }
                response = new Response<List<Country>>
                {
                    IsSuccess = true,
                    StatusCode = (int)HttpStatusCode.OK,
                    Message = "Lista completa de países",
                    Result = countries
                };
                return Ok(response);
            }
            catch (Exception ex)
            {

                _logger.LogInformation($"Error Inicio del método Exception {ex.Message}");
                return Ok(new Response<Country>
                {
                    IsSuccess = false,
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = ex.Message
                });
            }
        }


        [HttpGet("full")]
        public async Task<ActionResult<Response<List<Country>>>> GetFullAsync()
        {
            Response<List<Country>> response = new Response<List<Country>>();
            List<Country> countries = new List<Country>();    
            _logger.LogInformation("Inicio del método GetFullAsync.");
            countries = await _dataContext.Countries
                .Include(x => x.States!)
                .ThenInclude(x => x.Cities)
                .ToListAsync();
            if (countries.Count == 0)
            {
                response = new Response<List<Country>>
                {
                    IsSuccess = true,
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "Lista sin datos",
                    Result = countries  
                };
                return Ok(response);
            }

            response = new Response<List<Country>>
            {
                IsSuccess = true,
                StatusCode = (int)HttpStatusCode.OK,
                Message = "Lista completa de países",
                Result = countries
            };

            return Ok(response);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Response<Country>>> GetAsync(int id)
        {

            _logger.LogInformation("Inicio del método GetAsync.");
            var country = await _dataContext.Countries
                //.Include(x => x.States!)
                //.ThenInclude(x => x.Cities)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (country == null)
            {
                return NotFound(new Response<Country>
                {
                    IsSuccess = false,
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "País no encontrado",
                    Result = null
                });
            }

            return Ok(new Response<Country>
            {
                IsSuccess = true,
                StatusCode = (int)HttpStatusCode.OK,
                Message = "País encontrado",
                Result = country
            });
        }

        [HttpPost]
        public async Task<ActionResult<Response<Country>>> PostAsync(Country country)
        {
            _logger.LogInformation("Inicio del método PostAsync.");
            try
            {
                _dataContext.Add(country);
                await _dataContext.SaveChangesAsync();

                return Ok(new Response<Country>
                {
                    IsSuccess = true,
                    StatusCode = (int)HttpStatusCode.OK,
                    Message = "País creado correctamente",
                    Result = country
                });
            }
            catch (DbUpdateException dbUpdateException)
            {
                var isDuplicate = dbUpdateException.InnerException?.Message.Contains("duplicate") ?? false;
                _logger.LogInformation($"Inicio del método dbUpdateException {isDuplicate} : {dbUpdateException.Message}");
                return Ok(new Response<Country>
                {
                    IsSuccess = false,
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = isDuplicate ? "Ya existe un país con el mismo nombre." : dbUpdateException.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Error Inicio del método Exception {ex.Message}");
                return Ok(new Response<Country>
                {
                    IsSuccess = false,
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = ex.Message
                });
            }
        }

        [HttpPut]
        public async Task<ActionResult<Response<Country>>> PutAsync(Country country)
        {
            _logger.LogInformation("Inicio del método PutAsync.");
            try
            {
                _dataContext.Update(country);
                await _dataContext.SaveChangesAsync();

                return Ok(new Response<Country>
                {
                    IsSuccess = true,
                    StatusCode = (int)HttpStatusCode.OK,
                    Message = "País actualizado correctamente",
                    Result = country
                });
            }
            catch (DbUpdateException dbUpdateException)
            {
                var isDuplicate = dbUpdateException.InnerException?.Message.Contains("duplicate") ?? false;
                _logger.LogInformation($"Inicio del método dbUpdateException {isDuplicate} : {dbUpdateException.Message}");
                return BadRequest(new Response<Country>
                {
                    IsSuccess = false,
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = isDuplicate ? "Ya existe un país con el mismo nombre." : dbUpdateException.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Inicio del método dbUpdateException {ex.Message} ");
                return BadRequest(new Response<Country>
                {
                    IsSuccess = false,
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = ex.Message
                });
            }
        }
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<Response<object>>> DeleteAsync(int id)
        {
            _logger.LogInformation("Inicio del método DeleteAsync.");
            var country = await _dataContext.Countries.FirstOrDefaultAsync(x => x.Id == id);

            if (country == null)
            {
                return NotFound(new Response<object>
                {
                    IsSuccess = false,
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "País no encontrado"
                });
            }

            _dataContext.Remove(country);
            await _dataContext.SaveChangesAsync();

            return Ok(new Response<object>
            {
                IsSuccess = true,
                StatusCode = (int)HttpStatusCode.OK,
                Message = "País eliminado correctamente"
            });
        }

    }
}
