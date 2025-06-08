using Microsoft.EntityFrameworkCore;
using SBM.API.Data;
using SBM.API.Infrastructure.Interfaces;
using SBM.Shared.DTOs;
using SBM.Shared.Helpers;
using SBM.Shared.Responses;

namespace SBM.API.Infrastructure.Implementations
{
    public partial class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly DataContext _context;
        private readonly DbSet<T> _entity;

        public GenericRepository(DataContext context)
        {
            _context = context;
            _entity = context.Set<T>();
        }

        public virtual async Task<Response<T>> AddAsync(T entity)
        {
            _context.Add(entity);
            try
            {
                await _context.SaveChangesAsync();
                return new Response<T>
                {
                    IsSuccess = true,
                    Result = entity
                };
            }
            catch (DbUpdateException)
            {
                return DbUpdateExceptionResponse();
            }
            catch (Exception exception)
            {
                return ExceptionResponse(exception);
            }
        }

        public virtual async Task<Response<T>> DeleteAsync(int id)
        {
            var row = await _entity.FindAsync(id);
            if (row == null)
            {
                return new Response<T>
                {
                    IsSuccess = false,
                    Message = "ERR001"
                };
            }

            try
            {
                _entity.Remove(row);
                await _context.SaveChangesAsync();
                return new Response<T>
                {
                    IsSuccess = true,
                };
            }
            catch
            {
                return new Response<T>
                {
                    IsSuccess = false,
                    Message = "ERR002"
                };
            }
        }

        public virtual async Task<Response<T>> GetAsync(int id)
        {
            var row = await _entity.FindAsync(id);
            if (row != null)
            {
                return new Response<T>
                {
                    IsSuccess = true,
                    Result = row
                };
            }
            return new Response<T>
            {
                IsSuccess = false,
                Message = "ERR001"
            };
        }

        public virtual async Task<Response<IEnumerable<T>>> GetAsync()
        {
            return new Response<IEnumerable<T>>
            {
                IsSuccess = true,
                Result = await _entity.ToListAsync()
            };
        }

        public virtual async Task<Response<T>> UpdateAsync(T entity)
        {
            try
            {
                _context.Update(entity);
                await _context.SaveChangesAsync();
                return new Response<T>
                {
                    IsSuccess = true,
                    Result = entity
                };
            }
            catch (DbUpdateException)
            {
                return DbUpdateExceptionResponse();
            }
            catch (Exception exception)
            {
                return ExceptionResponse(exception);
            }
        }

        public virtual async Task<Response<IEnumerable<T>>> GetAsync(PaginationDTO pagination)
        {
            var queryable = _entity.AsQueryable();

            return new Response<IEnumerable<T>>
            {
                IsSuccess = true,
                Result = await queryable
                    .Paginate(pagination)
                    .ToListAsync()
            };
        }

        public virtual async Task<Response<int>> GetTotalRecordsAsync()
        {
            var queryable = _entity.AsQueryable();
            double count = await queryable.CountAsync();
            return new Response<int>
            {
                IsSuccess = true,
                Result = (int)count
            };
        }

        private Response<T> ExceptionResponse(Exception exception)
        {
            return new Response<T>
            {
                IsSuccess = false,
                Message = exception.Message
            };
        }

        private Response<T> DbUpdateExceptionResponse()
        {
            return new Response<T>
            {
                IsSuccess = false,
                Message = "ERR003"
            };
        }
    }
}
