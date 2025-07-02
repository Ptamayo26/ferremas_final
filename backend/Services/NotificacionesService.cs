using Ferremas.Api.DTOs;
using Ferremas.Api.Models;
using Ferremas.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ferremas.Api.Data;

namespace Ferremas.Api.Services
{
    public class NotificacionesService : INotificacionesService
    {
        private readonly AppDbContext _context;

        public NotificacionesService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<NotificacionResponseDTO>> ObtenerPorUsuario(int usuarioId)
        {
            return await _context.Notificaciones
                .Where(n => n.UsuarioId == usuarioId)
                .OrderByDescending(n => n.FechaCreacion)
                .Select(n => new NotificacionResponseDTO
                {
                    Id = n.Id,
                    Mensaje = n.Mensaje,
                    FechaCreacion = n.FechaCreacion,
                    Leida = n.Leida
                }).ToListAsync();
        }

        public async Task<NotificacionResponseDTO> Crear(NotificacionCreateDTO dto)
        {
            var notificacion = new Notificacion
            {
                UsuarioId = dto.UsuarioId,
                Mensaje = dto.Mensaje,
                Leida = false,
                FechaCreacion = DateTime.Now
            };

            _context.Notificaciones.Add(notificacion);
            await _context.SaveChangesAsync();

            return new NotificacionResponseDTO
            {
                Id = notificacion.Id,
                Mensaje = notificacion.Mensaje,
                FechaCreacion = notificacion.FechaCreacion,
                Leida = false
            };
        }

        public async Task<bool> MarcarComoLeida(int id)
        {
            var notificacion = await _context.Notificaciones.FindAsync(id);
            if (notificacion == null) return false;

            notificacion.Leida = true;
            await _context.SaveChangesAsync();
            return true;
        }
    }
} 