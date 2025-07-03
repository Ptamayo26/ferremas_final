using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Ferremas.Api.Models;
using Ferremas.Api.Data;
using System.Linq;

namespace Ferremas.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriasController : ControllerBase
    {
        private readonly AppDbContext _context;
        public CategoriasController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetCategorias()
        {
            var categorias = _context.Categorias
                .Where(c => c.Activo)
                .Select(c => new {
                    c.Id,
                    c.Nombre,
                    c.Descripcion,
                    c.Codigo
                })
                .ToList();
            return Ok(categorias);
        }
    }
} 