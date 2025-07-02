using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ferremas.Api.Models;
using Ferremas.Api.Data;

namespace Ferremas.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FavoritosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FavoritosController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("mis")]
        public async Task<IActionResult> GetMisFavoritos()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized();
            }

            var favoritos = await _context.Set<ListaDeseos>()
                .Where(ld => ld.UsuarioId == userId)
                .Include(ld => ld.Producto)
                .Select(ld => new {
                    ld.Producto.Id,
                    ld.Producto.Nombre,
                    ld.Producto.Descripcion,
                    ld.Producto.Precio,
                    ld.Producto.Stock,
                    ld.Producto.ImagenUrl
                })
                .ToListAsync();

            return Ok(favoritos);
        }
    }
} 