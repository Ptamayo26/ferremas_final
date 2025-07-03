using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Ferremas.Api.Models;
using Ferremas.Api.Data;
using System.Linq;

namespace Ferremas.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MarcasController : ControllerBase
    {
        private readonly AppDbContext _context;
        public MarcasController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetMarcas()
        {
            var marcas = _context.Marcas
                .Where(m => m.Activo)
                .Select(m => new {
                    m.Id,
                    m.Nombre,
                    m.Descripcion,
                    m.LogoUrl
                })
                .ToList();
            return Ok(marcas);
        }
    }
} 