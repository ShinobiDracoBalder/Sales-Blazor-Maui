using SBM.Shared.Responses;

namespace SBM.API.Services
{
    public interface IApiService
    {
        Task<GenericResponse> GetListAsync<T>(string servicePrefix, string controller);
    }
}
