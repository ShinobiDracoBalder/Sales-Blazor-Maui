using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SBM.API.Infrastructure.Interfaces;
using SBM.Shared.DTOs;
using SBM.Shared.Entities;
using SBM.Shared.Responses;
using System.Net;

namespace SBM.API.Controllers
{
    [Route("/api/categories")]
    [ApiController]
    public class categoriesController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;

        public categoriesController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        [HttpGet]
        public async Task<ActionResult> Get([FromQuery] PaginationDTO pagination)
        {
            var queryable = await _categoryRepository.GetAsync(pagination);

            if (!queryable.IsSuccess)
            {
                return Ok(queryable);
            }
            return Ok(queryable);
        }


        [HttpGet("totalPages")]
        public async Task<ActionResult> GetPages([FromQuery] PaginationDTO pagination)
        {
            var queryable = await _categoryRepository.GetAsync(pagination);

            var queryableCount = await _categoryRepository.GetTotalRecordsAsync(pagination);
            if (!queryable.IsSuccess)
            {
                return Ok(queryable);
            }

            double count = queryableCount.Result;
            double totalPages = Math.Ceiling(count / pagination.RecordsNumber);

            Response<object> response = new Response<object> { 
                TotalPages = (int)totalPages,
                IsSuccess = true,
                StatusCode = (int)HttpStatusCode.OK
            }; 

            return Ok(response);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> Get(int id)
        {
            var category = await _categoryRepository.GetAsync(id);
            if (!category.IsSuccess && category.StatusCode != (int)HttpStatusCode.OK)
            {
                return Ok(category);
            }

            return Ok(category);
        }


        [HttpPost]
        public async Task<ActionResult> Post(CategoryDTO category)
        {
           
            try
            {
                var categoryRepository = await _categoryRepository.AddAsync(category);
                return Ok(categoryRepository);
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        [HttpPut]
        public async Task<ActionResult> Put(CategoryDTO category)
        {
            try
            {
                var categoryRepository = await _categoryRepository.UpdateAsync(category);

                if (!categoryRepository.IsSuccess)
                {
                    return Ok(categoryRepository);
                }

                return Ok(category);
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

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var category = await _categoryRepository.GetDeleteAsync(id);
            if (!category.IsSuccess)
            {
                return NotFound(category);
            }
            //return NoContent();
            return Ok(category);
        }
    }
}
