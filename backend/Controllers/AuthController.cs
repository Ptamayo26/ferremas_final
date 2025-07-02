using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Ferremas.Api.DTOs;
using Ferremas.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Ferremas.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
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