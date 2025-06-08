namespace SBM.API.Services
{
    public interface IApiVerificationService
    {
        Task<bool> IsApiActiveAsync();
    }
}
