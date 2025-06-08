using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SBM.API.Data;
using SBM.API.Infrastructure.Implementations;
using SBM.API.Infrastructure.Interfaces;
using SBM.Shared.DTOs;
using SBM.Shared.Responses;
using System.Net;

namespace SBM.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CitiesController : ControllerBase
    {
        private readonly ICityRepository _cityRepository;
        private readonly ILogger<CitiesController> _logger;

        public CitiesController(ICityRepository cityRepository, ILogger<CitiesController> logger)
        {
            _cityRepository = cityRepository;
            _logger = logger;
        }
        [AllowAnonymous]
        [HttpGet("combo/{countryId:int}")]
        public async Task<ActionResult> GetCombo(int countryId)
        {
            return Ok(await _cityRepository.GetComboAsync(countryId));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            return Ok(await _cityRepository.GetAsync(id));
        }
        [HttpPost]
        public async Task<ActionResult> Post(CityDTO cityDTO)
        {

            try
            {
                return Ok(await _cityRepository.AddAsync(cityDTO));
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        [HttpPut]
        public async Task<ActionResult> Put(CityDTO cityDTO)
        {
            try
            {
                return Ok(await _cityRepository.UpdateAsync(cityDTO));
            }
            catch (DbUpdateException dbUpdateException)
            {
                if (dbUpdateException.InnerException!.Message.Contains("duplicate"))
                {
                    return BadRequest("Ya existe un registro con el mismo nombre.");
                }
                else
                {
                    return BadRequest(dbUpdateException.InnerException.Message);
                }
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        [HttpGet]
        public async Task<ActionResult> Get([FromQuery] PaginationDTO pagination)
        {
            var queryable = await _cityRepository.GetAsync(pagination);

            if (!queryable.IsSuccess)
            {
                return Ok(queryable);
            }
            return Ok(queryable);
        }


        [HttpGet("totalPages")]
        public async Task<ActionResult> GetPages([FromQuery] PaginationDTO pagination)
        {
            var queryable = await _cityRepository.GetAsync(pagination);

            var queryableCount = await _cityRepository.GetTotalRecordsAsync(pagination);
            if (!queryable.IsSuccess)
            {
                return Ok(queryable);
            }

            double count = queryableCount.Result;
            double totalPages = Math.Ceiling(count / pagination.RecordsNumber);

            Response<object> response = new Response<object>
            {
                TotalPages = (int)totalPages,
                IsSuccess = true,
                StatusCode = (int)HttpStatusCode.OK
            };

            return Ok(response);
        }

    }
}
