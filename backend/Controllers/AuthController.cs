using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Ferremas.Api.DTOs;
using Ferremas.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Ferremas.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            var response = await _authService.LoginAsync(loginDto);

            if (!response.Exito)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] UsuarioCreateDTO dto)
        {
            // Log de los datos recibidos
            _logger.LogInformation("Registro solicitado para: {Email}", dto.Email);
            _logger.LogInformation("Datos completos: {@UsuarioCreateDTO}", dto);
            
            // Verificar si el modelo es válido
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Modelo inválido: {Errors}", string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                return BadRequest(new { exito = false, mensaje = "Datos inválidos", errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
            }
            
            var response = await _authService.RegisterAsync(dto);
            if (!response.Exito)
                return BadRequest(response);
            return Ok(response);
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO resetDto)
        {
            var response = await _authService.ResetPasswordAsync(resetDto);
            if (!response.Exito)
                return BadRequest(response);
            return Ok(response);
        }

        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDTO model)
        {
            var response = await _authService.RefreshTokenAsync(model);
            if (!response.Exito)
                return BadRequest(response);
            return Ok(response);
        }
    }
} 