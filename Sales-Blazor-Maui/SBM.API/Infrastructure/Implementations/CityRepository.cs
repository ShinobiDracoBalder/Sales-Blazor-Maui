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
    public class CityRepository : ICityRepository
    {
        private readonly ILogger<StateRepository> _logger;
        private readonly DataContext _dataContext;

        public CityRepository(ILogger<StateRepository> logger, DataContext dataContext)
        {
            _logger = logger;
            _dataContext = dataContext;
        }

        public async Task<Response<City>> AddAsync(CityDTO city)
        {
            City _city = new City
            {
                Name = city.Name,
                StateId = city.StateId,
            };

            _dataContext.Cities.Add(_city);
            try
            {
                await _dataContext.SaveChangesAsync();

                return new Response<City>
                {
                    IsSuccess = true,
                    StatusCode = (int)HttpStatusCode.OK,
                };
            }
            catch (DbUpdateException dbUpdateException)
            {
                if (dbUpdateException.InnerException!.Message.Contains("duplicate"))
                {
                    return new Response<City>
                    {
                        IsSuccess = false,
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Message = "Ya existe un registro con el mismo nombre."
                    };
                }
                else
                {
                    return new Response<City>
                    {
                        IsSuccess = false,
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Message = dbUpdateException.InnerException.Message
                    };
                }
            }
            catch (Exception exception)
            {
                return new Response<City>
                {
                    IsSuccess = false,
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = exception.Message
                };
            }
        }

        public async Task<Response<City>> GetAsync(int id)
        {
            try
            {
                var city = await _dataContext.Cities
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (city == null)
                {
                    return new Response<City>
                    {
                        IsSuccess = false,
                        Message = "ERR001",
                        StatusCode = (int)HttpStatusCode.NotFound,
                    };
                }

                return new Response<City>
                {
                    IsSuccess = true,
                    StatusCode = (int)HttpStatusCode.OK,
                    Result = city
                };
            }
            catch (Exception ex)
            {
                return new Response<City>
                {
                    IsSuccess = false,
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Message = ex.Message,
                };
            }
        }

        public async Task<Response<IEnumerable<City>>> GetAsync()
        {
            try
            {
                var States = await _dataContext.Cities
                   .OrderBy(c => c.Name)
                   .ToListAsync();

                if (States.Count == 0)
                {
                    return new Response<IEnumerable<City>>
                    {
                        IsSuccess = false,
                        StatusCode = (int)HttpStatusCode.NotFound,

                    };
                }

                return new Response<IEnumerable<City>>
                {
                    IsSuccess = true,
                    StatusCode = (int)HttpStatusCode.OK,
                    Result = States
                };
            }
            catch (Exception ex)
            {
                return new Response<IEnumerable<City>>
                {
                    IsSuccess = false,
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Message = ex.Message,
                };
            }
        }

        public async Task<Response<IEnumerable<City>>> GetAsync(PaginationDTO pagination)
        {
            try
            {
                var queryable = _dataContext.Cities
               .AsQueryable();

                if (!await queryable.AnyAsync())
                {
                    return new Response<IEnumerable<City>>
                    {
                        IsSuccess = false,
                        StatusCode = (int)HttpStatusCode.NotFound,
                    };
                }

                if (!string.IsNullOrWhiteSpace(pagination.Filter))
                {
                    queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
                }

                return new Response<IEnumerable<City>>
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

                return new Response<IEnumerable<City>>
                {
                    IsSuccess = false,
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Message = ex.Message,
                };
            }
        }

        public async Task<Response<List<City>>> GetComboAsync(int StateId)
        {
            Response<List<City>> response = new Response<List<City>>();
            List<City> ListCity = new List<City>();
            try
            {

                ListCity = await _dataContext.Cities
                    .Where(x => x.StateId == StateId)
                    .ToListAsync();

                if (ListCity == null)
                {
                    response.IsSuccess = false;
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    response.Message = "NotFound";
                    return response;
                }

                response.IsSuccess = true;
                response.StatusCode = (int)HttpStatusCode.OK;
                response.Message = "OK";
                response.Result = ListCity;
                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Message = ex.InnerException?.Message;
                return response;
            }
        }

        public Task<Response<CityDTO>> GetDeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<Response<int>> GetTotalRecordsAsync(PaginationDTO pagination)
        {
            try
            {
                var queryable = _dataContext.Cities.AsQueryable();

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

        public async Task<Response<City>> UpdateAsync(CityDTO city)
        {
            try
            {

                var _city = await _dataContext.Cities.FirstOrDefaultAsync(c => c.Id.Equals(city.Id));
                if (_city == null)
                {
                    return new Response<City>
                    {
                        IsSuccess = false,
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Message = "No existe un registro con el mismo nombre."
                    };
                }
                _city!.Name = city.Name;
                _dataContext.Update(_city);

                await _dataContext.SaveChangesAsync();
                return new Response<City>
                {
                    IsSuccess = true,
                    StatusCode = (int)HttpStatusCode.OK,
                    Result = _city
                };
            }
            catch (DbUpdateException dbUpdateException)
            {
                if (dbUpdateException.InnerException!.Message.Contains("duplicate"))
                {
                    return new Response<City>
                    {
                        IsSuccess = false,
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Message = "Ya existe un registro con el mismo nombre."
                    };
                }
                else
                {
                    return new Response<City>
                    {
                        IsSuccess = false,
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Message = dbUpdateException.InnerException.Message
                    };
                }
            }
            catch (Exception exception)
            {
                return new Response<City>
                {
                    IsSuccess = false,
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = exception.Message
                };
            }
        }
    }
}
