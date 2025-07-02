using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System;

namespace Ferremas.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UploadController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;

        public UploadController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        [HttpPost("producto")]
        [Authorize(Roles = "administrador,bodeguero")]
        public async Task<IActionResult> SubirImagenProducto(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { mensaje = "No se ha seleccionado ningún archivo" });
                }

                // Validar tipo de archivo
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                
                if (!allowedExtensions.Contains(fileExtension))
                {
                    return BadRequest(new { mensaje = "Solo se permiten archivos de imagen (jpg, jpeg, png, gif, webp)" });
                }

                // Validar tamaño (máximo 5MB)
                if (file.Length > 5 * 1024 * 1024)
                {
                    return BadRequest(new { mensaje = "El archivo es demasiado grande. Máximo 5MB" });
                }

                // Generar nombre único para el archivo
                var fileName = $"{Guid.NewGuid()}{fileExtension}";
                var uploadPath = Path.Combine(_environment.WebRootPath, "images", "productos");
                
                // Crear directorio si no existe
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                var filePath = Path.Combine(uploadPath, fileName);

                // Guardar archivo
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Retornar URL de la imagen
                var imageUrl = $"/images/productos/{fileName}";
                
                return Ok(new { 
                    mensaje = "Imagen subida exitosamente",
                    url = imageUrl,
                    nombreArchivo = fileName
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al subir la imagen", error = ex.Message });
            }
        }

        [HttpDelete("producto/{fileName}")]
        [Authorize(Roles = "administrador,bodeguero")]
        public IActionResult EliminarImagenProducto(string fileName)
        {
            try
            {
                var filePath = Path.Combine(_environment.WebRootPath, "images", "productos", fileName);
                
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                    return Ok(new { mensaje = "Imagen eliminada exitosamente" });
                }
                
                return NotFound(new { mensaje = "Archivo no encontrado" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al eliminar la imagen", error = ex.Message });
            }
        }

        [HttpGet("producto/{fileName}")]
        [AllowAnonymous]
        public IActionResult ObtenerImagenProducto(string fileName)
        {
            try
            {
                var filePath = Path.Combine(_environment.WebRootPath, "images", "productos", fileName);
                
                if (System.IO.File.Exists(filePath))
                {
                    var fileBytes = System.IO.File.ReadAllBytes(filePath);
                    var contentType = GetContentType(fileName);
                    return File(fileBytes, contentType);
                }
                
                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener la imagen", error = ex.Message });
            }
        }

        private string GetContentType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };
        }
    }
} 