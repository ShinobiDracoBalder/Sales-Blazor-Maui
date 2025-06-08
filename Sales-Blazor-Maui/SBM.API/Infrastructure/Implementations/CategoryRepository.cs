using Microsoft.EntityFrameworkCore;
using SBM.API.Data;
using SBM.API.Infrastructure.Interfaces;
using SBM.Shared.DTOs;
using SBM.Shared.Entities;
using SBM.Shared.Helpers;
using SBM.Shared.Responses;
using System.Net;

namespace SBM.API.Infrastructure.Implementations
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ILogger<CategoryRepository> _logger;
        private readonly DataContext _dataContext;

        public CategoryRepository(ILogger<CategoryRepository> logger, DataContext dataContext)
        {
            _logger = logger;
            _dataContext = dataContext;
        }

        public async Task<Response<Category>> AddAsync(CategoryDTO categoryDTO)
        {

            Category category = new Category {
                Name = categoryDTO.Name,    
            };
            
            _dataContext.Add(category);
            try
            {
                await _dataContext.SaveChangesAsync();

                return new Response<Category>
                {
                    IsSuccess = true,
                    StatusCode = (int)HttpStatusCode.OK,
                };
            }
            catch (DbUpdateException dbUpdateException)
            {
                if (dbUpdateException.InnerException!.Message.Contains("duplicate"))
                {
                    return new Response<Category>
                    {
                        IsSuccess = false,
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Message = "Ya existe un registro con el mismo nombre."
                    };
                }
                else
                {
                    return new Response<Category>
                    {
                        IsSuccess = false,
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Message = dbUpdateException.InnerException.Message
                    };
                }
            }
            catch (Exception exception)
            {
                return new Response<Category>
                {
                    IsSuccess = false,
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = exception.Message
                };
            }
        }

        public async Task<Response<Category>> GetAsync(int id)
        {
            try
            {
                var category = await _dataContext.Categories
                    .Include(c => c.ProductCategories)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (category == null)
                {
                    return new Response<Category>
                    {
                        IsSuccess = false,
                        Message = "ERR001",
                        StatusCode = (int)HttpStatusCode.NotFound,
                    };
                }

                return new Response<Category>
                {
                    IsSuccess = true,
                    StatusCode = (int)HttpStatusCode.OK,
                    Result = category
                };
            }
            catch (Exception ex)
            {
                return new Response<Category>
                {
                    IsSuccess = false,
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Message = ex.Message,
                };
            }
        }

        public async Task<Response<IEnumerable<Category>>> GetAsync()
        {
            try
            {
                var categories = await _dataContext.Categories
                   .Include(c => c.ProductCategories)
                   .OrderBy(c => c.Name)
                   .ToListAsync();

                if (categories.Count == 0)
                {
                    return new Response<IEnumerable<Category>>
                    {
                        IsSuccess = false,
                        StatusCode = (int)HttpStatusCode.NotFound,

                    };
                }

                return new Response<IEnumerable<Category>>
                {
                    IsSuccess = true,
                    StatusCode = (int)HttpStatusCode.OK,
                    Result = categories
                };
            }
            catch (Exception ex)
            {
                return new Response<IEnumerable<Category>>
                {
                    IsSuccess = false,
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Message = ex.Message,
                };
            }
        }

        public async Task<Response<IEnumerable<Category>>> GetAsync(PaginationDTO pagination)
        {
            try
            {
                var queryable = _dataContext.Categories
               .Include(x => x.ProductCategories)
               .AsQueryable();

                if (!await queryable.AnyAsync())
                {
                    return new Response<IEnumerable<Category>>
                    {
                        IsSuccess = false,
                        StatusCode = (int)HttpStatusCode.NotFound,
                    };
                }

                if (!string.IsNullOrWhiteSpace(pagination.Filter))
                {
                    queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
                }

                return new Response<IEnumerable<Category>>
                {
                    IsSuccess = true,
                    Result = await queryable
                        .OrderBy(x => x.Name)
                        .Paginate(pagination)
                        .ToListAsync()
                };
            }
            catch (Exception ex)
            {

                return new Response<IEnumerable<Category>>
                {
                    IsSuccess = false,
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Message = ex.Message,
                };
            }
        }

        public async Task<Response<List<Category>>> GetComboAsync()
        {
            try
            {
                _logger.LogInformation("Inicio del método GetComboAsync.");

                var categories = await _dataContext.Categories.ToListAsync();

                _logger.LogInformation($"Se recuperaron {categories.Count} categorías.");

                return new Response<List<Category>>
                {
                    IsSuccess = true,
                    StatusCode = (int)HttpStatusCode.OK,
                    Message = "Listado de categorías obtenido correctamente.",
                    Result = categories
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la lista de categorías.");

                return new Response<List<Category>>
                {
                    IsSuccess = false,
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Message = "Ocurrió un error interno al obtener las categorías.",
                    Result = null
                };
            }
        }
        public Task<Response<Category>> GetDeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<Response<int>> GetTotalRecordsAsync(PaginationDTO pagination)
        {
            try
            {
                var queryable = _dataContext.Countries.AsQueryable();

                if (!await queryable.AnyAsync())
                {
                    return new Response<int>
                    {
                        IsSuccess = false,
                        Result = (int)0,
                        StatusCode = (int)HttpStatusCode.NotFound,
                    };
                }

                if (!string.IsNullOrWhiteSpace(pagination.Filter))
                {
                    queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
                }

                double count = await queryable.CountAsync();
                return new Response<int>
                {
                    IsSuccess = true,
                    Result = (int)count
                };
            }
            catch (Exception ex)
            {
                return new Response<int>
                {
                    IsSuccess = false,
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Message = ex.Message,
                    Result = (int)-1
                };
            }
        }

        public async Task<Response<Category>> UpdateAsync(CategoryDTO categoryDTO)
        {
            try
            {

                var _category = await _dataContext.Categories.FirstOrDefaultAsync(c => c.Id.Equals(categoryDTO.Id));
                if (_category == null)
                {
                    return new Response<Category>
                    {
                        IsSuccess = false,
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Message = "No existe un registro con el mismo nombre."
                    };
                }
                _category!.Name = categoryDTO.Name;
                _dataContext.Update(_category);

                await _dataContext.SaveChangesAsync();
                return new Response<Category>
                {
                    IsSuccess = true,
                    StatusCode = (int)HttpStatusCode.OK,
                    Result = _category
                };
            }
            catch (DbUpdateException dbUpdateException)
            {
                if (dbUpdateException.InnerException!.Message.Contains("duplicate"))
                {
                    return new Response<Category>
                    {
                        IsSuccess = false,
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Message = "Ya existe un registro con el mismo nombre."
                    };
                }
                else
                {
                    return new Response<Category>
                    {
                        IsSuccess = false,
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Message = dbUpdateException.InnerException.Message
                    };
                }
            }
            catch (Exception exception)
            {
                return new Response<Category>
                {
                    IsSuccess = false,
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = exception.Message
                };
            }
        }

        Task<IEnumerable<Category>> ICategoryRepository.GetComboAsync()
        {
            throw new NotImplementedException();
        }
    }
}
