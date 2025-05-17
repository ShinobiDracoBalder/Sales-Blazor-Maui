using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SBM.API.Data;
using SBM.Shared.Entities;
using SBM.Shared.Responses;
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

    }
}
