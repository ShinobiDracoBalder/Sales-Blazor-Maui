using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SBM.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController : ControllerBase
    {
        private readonly ILogger<CountriesController> _logger;

        public CountriesController(ILogger<CountriesController> logger)
        {
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpGet("combo")]
        public async Task<ActionResult> GetCombo()
        {
            _logger.LogInformation("Esto es información");
            _logger.LogWarning("Esto es una advertencia");
            _logger.LogError("Esto es un error");
            _logger.LogCritical("Esto es crítico");
            return Ok();
        }
    }
}
