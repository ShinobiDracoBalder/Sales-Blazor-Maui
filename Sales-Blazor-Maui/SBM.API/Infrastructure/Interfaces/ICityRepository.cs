using SBM.Shared.DTOs;
using SBM.Shared.Entities;
using SBM.Shared.Responses;

namespace SBM.API.Infrastructure.Interfaces
{
    public interface ICityRepository
    {
        Task<Response<City>> GetAsync(int id);

        Task<Response<IEnumerable<City>>> GetAsync();

        Task<Response<IEnumerable<City>>> GetAsync(PaginationDTO pagination);

        Task<Response<int>> GetTotalRecordsAsync(PaginationDTO pagination);

        Task<Response<List<City>>> GetComboAsync(int StateId);

        Task<Response<City>> AddAsync(CityDTO city);

        Task<Response<City>> UpdateAsync(CityDTO city);

        Task<Response<CityDTO>> GetDeleteAsync(int id);
    }
}
