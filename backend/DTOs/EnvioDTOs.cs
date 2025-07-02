using System;
using System.ComponentModel.DataAnnotations;

namespace Ferremas.Api.DTOs
{
    public class EnvioCreateDTO
    {
        [Required]
        public int PedidoId { get; set; }

        [Required]
        [StringLength(200)]
        public string DireccionDestino { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string ComunaDestino { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string RegionDestino { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string TelefonoContacto { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string NombreDestinatario { get; set; } = string.Empty;

        public string? InstruccionesEspeciales { get; set; }
    }

    public class EnvioDTO
    {
        public int Id { get; set; }
        public int PedidoId { get; set; }
        public string DireccionDestino { get; set; }
        public string ComunaDestino { get; set; }
        public string RegionDestino { get; set; }
        public string TelefonoContacto { get; set; }
        public string NombreDestinatario { get; set; }
        public string Estado { get; set; }
        public string NumeroSeguimiento { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }
    }

    public class EnvioResponseDTO
    {
        public int Id { get; set; }
        public int PedidoId { get; set; }

        [Required]
        [StringLength(200)]
        public string DireccionDestino { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string ComunaDestino { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string RegionDestino { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string TelefonoContacto { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string NombreDestinatario { get; set; } = string.Empty;

        public string? InstruccionesEspeciales { get; set; }

        [Required]
        [StringLength(20)]
        public string Estado { get; set; } = "Pendiente";

        [Required]
        [StringLength(50)]
        public string NumeroSeguimiento { get; set; } = string.Empty;

        public DateTime FechaEnvio { get; set; }
        public DateTime? FechaEntrega { get; set; }
    }
} 