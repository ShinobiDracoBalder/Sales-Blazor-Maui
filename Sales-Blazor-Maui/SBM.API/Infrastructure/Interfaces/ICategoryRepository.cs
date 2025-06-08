using SBM.Shared.DTOs;
using SBM.Shared.Entities;
using SBM.Shared.Responses;

namespace SBM.API.Infrastructure.Interfaces
{
    public interface ICategoryRepository
    {
        Task<Response<Category>> GetAsync(int id);

        Task<Response<IEnumerable<Category>>> GetAsync();

        Task<Response<IEnumerable<Category>>> GetAsync(PaginationDTO pagination);

        Task<Response<int>> GetTotalRecordsAsync(PaginationDTO pagination);

        Task<IEnumerable<Category>> GetComboAsync();

        Task<Response<Category>> AddAsync(CategoryDTO categoryDTO);

        Task<Response<Category>> UpdateAsync(CategoryDTO categoryDTO);

        Task<Response<Category>> GetDeleteAsync(int id);

    }
}
