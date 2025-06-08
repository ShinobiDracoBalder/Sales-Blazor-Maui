
using System.Net.Http;

namespace SBM.API.Services
{
    public class ApiVerificationService : IApiVerificationService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _tokenName;
        private readonly string _tokenValue;

        public ApiVerificationService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient("ApiDestino");
            _configuration = configuration;
            _tokenName = configuration["CoutriesAPI:tokenName"]!;
            _tokenValue = configuration["CoutriesAPI:tokenValue"]!;
        }

        public async Task<bool> IsApiActiveAsync()
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Add(_tokenName, _tokenValue);
                HttpResponseMessage response = await _httpClient.GetAsync("/v1/countries"); // o cualquier endpoint
                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error de red: {ex.Message}");
                return false;
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine("Tiempo de espera agotado");
                return false;
            }
        }
    }
}
