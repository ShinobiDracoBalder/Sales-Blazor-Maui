using Microsoft.EntityFrameworkCore;
using SBM.API.Data;
using SBM.API.Infrastructure.Interfaces;
using SBM.Shared.DTOs;
using SBM.Shared.Entities;
using SBM.Shared.Helpers;
using SBM.Shared.Responses;
using System.Collections.Generic;
using System.Net;

namespace SBM.API.Infrastructure.Implementations
{
    public class StateRepository : IStateRepository
    {
        private readonly ILogger<StateRepository> _logger;
        private readonly DataContext _dataContext;

        public StateRepository(ILogger<StateRepository> logger, DataContext dataContext)
        {
            _logger = logger;
            _dataContext = dataContext;
        }

        public async Task<Response<State>> AddAsync(StateDTO stateDTO)
        {
            State state = new State
            {
                Name = stateDTO.Name,
                CountryId = stateDTO.CountryId,    
            };

            _dataContext.States.Add(state);
            try
            {
                await _dataContext.SaveChangesAsync();

                return new Response<State>
                {
                    IsSuccess = true,
                    StatusCode = (int)HttpStatusCode.OK,
                };
            }
            catch (DbUpdateException dbUpdateException)
            {
                if (dbUpdateException.InnerException!.Message.Contains("duplicate"))
                {
                    return new Response<State>
                    {
                        IsSuccess = false,
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Message = "Ya existe un registro con el mismo nombre."
                    };
                }
                else
                {
                    return new Response<State>
                    {
                        IsSuccess = false,
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Message = dbUpdateException.InnerException.Message
                    };
                }
            }
            catch (Exception exception)
            {
                return new Response<State>
                {
                    IsSuccess = false,
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = exception.Message
                };
            }
        }

        public async Task<Response<State>> GetAsync(int id)
        {
            try
            {
                var state = await _dataContext.States
                    .Include(c => c.Cities)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (state == null)
                {
                    return new Response<State>
                    {
                        IsSuccess = false,
                        Message = "ERR001",
                        StatusCode = (int)HttpStatusCode.NotFound,
                    };
                }

                return new Response<State>
                {
                    IsSuccess = true,
                    StatusCode = (int)HttpStatusCode.OK,
                    Result = state
                };
            }
            catch (Exception ex)
            {
                return new Response<State>
                {
                    IsSuccess = false,
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Message = ex.Message,
                };
            }

        }

        public async Task<Response<IEnumerable<State>>> GetAsync()
        {
            try
            {
                var States = await _dataContext.States
                   .Include(c => c.Cities)
                   .OrderBy(c => c.Name)
                   .ToListAsync();

                if (States.Count == 0)
                {
                    return new Response<IEnumerable<State>>
                    {
                        IsSuccess = false,
                        StatusCode = (int)HttpStatusCode.NotFound,

                    };
                }

                return new Response<IEnumerable<State>>
                {
                    IsSuccess = true,
                    StatusCode = (int)HttpStatusCode.OK,
                    Result = States
                };
            }
            catch (Exception ex)
            {
                return new Response<IEnumerable<State>>
                {
                    IsSuccess = false,
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Message = ex.Message,
                };
            }
        }

        public async Task<Response<IEnumerable<State>>> GetAsync(PaginationDTO pagination)
        {
            try
            {
                var queryable = _dataContext.States
               .Include(x => x.Cities)
               .AsQueryable();

                if (!await queryable.AnyAsync())
                {
                    return new Response<IEnumerable<State>>
                    {
                        IsSuccess = false,
                        StatusCode = (int)HttpStatusCode.NotFound,
                    };
                }

                if (!string.IsNullOrWhiteSpace(pagination.Filter))
                {
                    queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
                }

                return new Response<IEnumerable<State>>
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

                return new Response<IEnumerable<State>>
                {
                    IsSuccess = false,
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Message = ex.Message,
                };
            }
        }

        public Task<Response<State>> GetDeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<Response<int>> GetTotalRecordsAsync(PaginationDTO pagination)
        {
            try
            {
                var queryable = _dataContext.States.AsQueryable();

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

        public async Task<Response<State>> UpdateAsync(StateDTO stateDTO)
        {
            try
            {

                var _state = await _dataContext.States.FirstOrDefaultAsync(c => c.Id.Equals(stateDTO.Id));
                if (_state == null)
                {
                    return new Response<State>
                    {
                        IsSuccess = false,
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Message = "No existe un registro con el mismo nombre."
                    };
                }
                _state!.Name = stateDTO.Name;
                _dataContext.Update(_state);

                await _dataContext.SaveChangesAsync();
                return new Response<State>
                {
                    IsSuccess = true,
                    StatusCode = (int)HttpStatusCode.OK,
                    Result = _state
                };
            }
            catch (DbUpdateException dbUpdateException)
            {
                if (dbUpdateException.InnerException!.Message.Contains("duplicate"))
                {
                    return new Response<State>
                    {
                        IsSuccess = false,
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Message = "Ya existe un registro con el mismo nombre."
                    };
                }
                else
                {
                    return new Response<State>
                    {
                        IsSuccess = false,
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Message = dbUpdateException.InnerException.Message
                    };
                }
            }
            catch (Exception exception)
            {
                return new Response<State>
                {
                    IsSuccess = false,
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = exception.Message
                };
            }
        }

        public async Task<Response<List<State>>> GetComboAsync(int countryId)
        {
            Response<List<State>> response = new Response<List<State>>();
            List <State> ListState = new List<State>();
            try
            {

                ListState =  await _dataContext.States
                    .Where(x => x.CountryId == countryId)
                    .ToListAsync();

                if (ListState == null) 
                {
                    response.IsSuccess = false;
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    response.Message = "NotFound";
                    return response;
                }

                response.IsSuccess = true;
                response.StatusCode = (int)HttpStatusCode.OK;
                response.Message = "OK";
                response.Result = ListState;
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
    }
}
