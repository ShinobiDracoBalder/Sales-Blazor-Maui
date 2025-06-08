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
    public class StatesController : ControllerBase
    {
        private readonly ILogger<StatesController> _logger;
        private readonly DataContext _dataContext;
        private readonly IStateRepository _stateRepository;

        public StatesController(ILogger<StatesController> logger, DataContext dataContext, IStateRepository stateRepository)
        {
            _logger = logger;
            _dataContext = dataContext;
            _stateRepository = stateRepository;
        }
        [AllowAnonymous]
        [HttpGet("combo/{countryId:int}")]
        public async Task<ActionResult> GetCombo(int countryId)
        {
            return Ok(await _stateRepository.GetComboAsync(countryId));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            var state = await _dataContext.States
                .Include(x => x.Cities)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (state == null)
            {
                return NotFound();
            }

            return Ok(state);
        }
        [HttpPost]
        public async Task<ActionResult> Post(StateDTO stateDTO)
        {

            try
            {
                return Ok(await _stateRepository.AddAsync(stateDTO));
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        [HttpPut]
        public async Task<ActionResult> Put(StateDTO state)
        {
            try
            {
                return Ok(await _stateRepository.UpdateAsync(state));
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
            var queryable = await _stateRepository.GetAsync(pagination);

            if (!queryable.IsSuccess)
            {
                return Ok(queryable);
            }
            return Ok(queryable);
        }


        [HttpGet("totalPages")]
        public async Task<ActionResult> GetPages([FromQuery] PaginationDTO pagination)
        {
            var queryable = await _stateRepository.GetAsync(pagination);

            var queryableCount = await _stateRepository.GetTotalRecordsAsync(pagination);
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
