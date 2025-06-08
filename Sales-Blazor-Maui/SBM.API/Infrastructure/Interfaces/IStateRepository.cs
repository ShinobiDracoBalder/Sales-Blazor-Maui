using SBM.Shared.DTOs;
using SBM.Shared.Entities;
using SBM.Shared.Responses;

namespace SBM.API.Infrastructure.Interfaces
{
    public interface IStateRepository
    {
        Task<Response<State>> GetAsync(int id);

        Task<Response<IEnumerable<State>>> GetAsync();

        Task<Response<List<State>>> GetComboAsync(int countryId);

        Task<Response<IEnumerable<State>>> GetAsync(PaginationDTO pagination);

        Task<Response<int>> GetTotalRecordsAsync(PaginationDTO pagination);

        Task<Response<State>> AddAsync(StateDTO stateDTO);

        Task<Response<State>> UpdateAsync(StateDTO stateDTO);

        Task<Response<State>> GetDeleteAsync(int id);
    }
}
