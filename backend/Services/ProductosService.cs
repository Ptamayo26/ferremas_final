using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ferremas.Api.Models;
using Ferremas.Api.DTOs;
using Ferremas.Api.Data;
using Ferremas.Api.Services.Interfaces;

namespace Ferremas.Api.Services
{
    public class ProductosService : IProductosService
    {
        private readonly AppDbContext _context;

        public ProductosService(AppDbContext context)
        {
            _context = context;
        }

        // Obtener todos los productos
        public async Task<IEnumerable<ProductoResponseDTO>> GetAll()
        {
            var productos = await _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Marca)
                .Where(p => p.Activo)
                .OrderBy(p => p.Nombre)
                .ToListAsync();

            return productos.Select(p => new ProductoResponseDTO
            {
                Id = p.Id,
                Codigo = p.Codigo ?? "Sin Código",
                Nombre = p.Nombre ?? "Sin Nombre",
                Descripcion = p.Descripcion ?? "Sin Descripción",
                Precio = p.Precio,
                Stock = p.Stock,
                // Corregir conversiones implícitas usando GetValueOrDefault()
                CategoriaId = p.CategoriaId.GetValueOrDefault(),
                CategoriaNombre = p.Categoria?.Nombre ?? "Sin Categoría",
                MarcaId = p.MarcaId.GetValueOrDefault(),
                MarcaNombre = p.Marca?.Nombre ?? "Sin Marca",
                ImagenUrl = p.ImagenUrl ?? "Sin Imagen",
                Especificaciones = p.Especificaciones ?? "No Especificadas",
                FechaCreacion = p.FechaCreacion,
                FechaModificacion = p.FechaModificacion,
                Activo = p.Activo
            });
        }

        // Obtener un producto por ID
        public async Task<ProductoResponseDTO?> GetById(int id)
        {
            var producto = await _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Marca)
                .FirstOrDefaultAsync(p => p.Id == id && p.Activo);

            return producto == null ? null : new ProductoResponseDTO
            {
                Id = producto.Id,
                Codigo = producto.Codigo ?? "Sin Código",
                Nombre = producto.Nombre ?? "Sin Nombre",
                Descripcion = producto.Descripcion ?? "Sin Descripción",
                Precio = producto.Precio,
                Stock = producto.Stock,
                // Corregir conversiones implícitas
                CategoriaId = producto.CategoriaId.GetValueOrDefault(),
                CategoriaNombre = producto.Categoria?.Nombre ?? "Sin Categoría",
                MarcaId = producto.MarcaId.GetValueOrDefault(),
                MarcaNombre = producto.Marca?.Nombre ?? "Sin Marca",
                ImagenUrl = producto.ImagenUrl ?? "Sin Imagen",
                Especificaciones = producto.Especificaciones ?? "No Especificadas",
                FechaCreacion = producto.FechaCreacion,
                FechaModificacion = producto.FechaModificacion,
                Activo = producto.Activo
            };
        }

        // Crear un nuevo producto
        public async Task<ProductoResponseDTO> Create(ProductoCreateDTO productoDto)
        {
            var producto = new Producto
            {
                Codigo = productoDto.Codigo,
                Nombre = productoDto.Nombre,
                Descripcion = productoDto.Descripcion,
                // Corregir problema con operador ?? en tipos no nulos
                // Asumiendo que productoDto.Precio y productoDto.Stock son nullable
                Precio = productoDto.Precio,
                Stock = productoDto.Stock,   // Asumiendo que Stock es int?
                CategoriaId = productoDto.CategoriaId,
                MarcaId = productoDto.MarcaId,
                ImagenUrl = productoDto.ImagenUrl,
                Especificaciones = productoDto.Especificaciones,
                FechaCreacion = DateTime.Now,
                Activo = true
            };

            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();
            return await GetById(producto.Id) ?? throw new Exception("Producto no encontrado después de la creación.");
        }

        // Actualizar un producto
        public async Task<ProductoResponseDTO?> Update(int id, ProductoUpdateDTO productoDto)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null) return null;

            producto.Nombre = productoDto.Nombre ?? producto.Nombre;
            producto.Descripcion = productoDto.Descripcion ?? producto.Descripcion;
            
            // Si los tipos son nullable, usa GetValueOrDefault() en lugar de ??
            if (productoDto.Precio.HasValue)
                producto.Precio = productoDto.Precio.Value;
                
            if (productoDto.Stock.HasValue)
                producto.Stock = productoDto.Stock.Value;
                
            producto.ImagenUrl = productoDto.ImagenUrl ?? producto.ImagenUrl;
            producto.Especificaciones = productoDto.Especificaciones ?? producto.Especificaciones;
            producto.FechaModificacion = DateTime.Now;

            await _context.SaveChangesAsync();
            return await GetById(id);
        }

        // Actualizar el stock de un producto
        public async Task<ProductoResponseDTO?> UpdateStock(int id, int cantidad)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null) return null;

            producto.Stock = producto.Stock + cantidad;
            producto.FechaModificacion = DateTime.Now;

            await _context.SaveChangesAsync();
            return await GetById(id);
        }

        // Eliminar un producto
        public async Task<bool> Delete(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null) return false;

            producto.Activo = false;
            producto.FechaModificacion = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

        // Buscar productos por término
        public async Task<IEnumerable<ProductoResponseDTO>> Search(string termino)
        {
            var productos = await _context.Productos
                .Where(p => p.Activo && 
                       ((p.Nombre != null && p.Nombre.Contains(termino)) || 
                        (p.Descripcion != null && p.Descripcion.Contains(termino))))
                .ToListAsync();

            return productos.Select(p => new ProductoResponseDTO
            {
                Id = p.Id,
                Codigo = p.Codigo ?? "Sin Código",
                Nombre = p.Nombre ?? "Sin Nombre",
                Descripcion = p.Descripcion ?? "Sin Descripción",
                Precio = p.Precio,
                Stock = p.Stock
            });
        }

        public async Task<IEnumerable<ProductoResponseDTO>> GetByCategoria(int categoriaId)
        {
            var productos = await _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Marca)
                .Where(p => p.CategoriaId == categoriaId)
                .ToListAsync();

            return productos.Select(p => new ProductoResponseDTO
            {
                Id = p.Id,
                Codigo = p.Codigo ?? "Sin Código",
                Nombre = p.Nombre ?? "Desconocido",
                Descripcion = p.Descripcion ?? "No disponible",
                Precio = p.Precio,
                // Corregir conversiones de int? a int
                CategoriaId = p.CategoriaId.GetValueOrDefault(),
                CategoriaNombre = p.Categoria?.Nombre ?? "Sin categoría",
                MarcaId = p.MarcaId.GetValueOrDefault(),
                MarcaNombre = p.Marca?.Nombre ?? "Sin marca",
                ImagenUrl = p.ImagenUrl ?? "Sin imagen",
                Especificaciones = p.Especificaciones ?? "No especificadas",
                FechaCreacion = p.FechaCreacion,
                FechaModificacion = p.FechaModificacion,
                Activo = p.Activo
            });
        }

        public async Task<IEnumerable<ProductoResponseDTO>> GetByMarca(int marcaId)
        {
            var productos = await _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Marca)
                .Where(p => p.MarcaId == marcaId)
                .ToListAsync();

            return productos.Select(p => new ProductoResponseDTO
            {
                Id = p.Id,
                Codigo = p.Codigo ?? "Sin Código",
                Nombre = p.Nombre ?? "Desconocido",
                Descripcion = p.Descripcion ?? "No disponible",
                Precio = p.Precio,
                // Corregir conversiones de int? a int
                CategoriaId = p.CategoriaId.GetValueOrDefault(),
                CategoriaNombre = p.Categoria?.Nombre ?? "Sin categoría",
                MarcaId = p.MarcaId.GetValueOrDefault(),
                MarcaNombre = p.Marca?.Nombre ?? "Sin marca",
                ImagenUrl = p.ImagenUrl ?? "Sin imagen",
                Especificaciones = p.Especificaciones ?? "No especificadas",
                FechaCreacion = p.FechaCreacion,
                FechaModificacion = p.FechaModificacion,
                Activo = p.Activo
            });
        }
    }
}