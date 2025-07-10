using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Ferremas.Api.Services;

namespace Ferremas.Api.Controllers
{
    [ApiController]
    [Route("api/shipit/health")]
    public class ShipitHealthController : ControllerBase
    {
        private readonly ShipitService _shipitService;

        public ShipitHealthController(ShipitService shipitService)
        {
            _shipitService = shipitService;
        }

        [HttpGet]
        public async Task<IActionResult> GetHealth()
        {
            var ok = await _shipitService.IsServiceAvailableAsync();
            if (ok)
            {
                return Ok(new { ok = true, mensaje = "Shipit conectado y funcionando" });
            }
            else
            {
                return StatusCode(503, new { ok = false, mensaje = "No se pudo conectar a Shipit" });
            }
        }
    }
} 