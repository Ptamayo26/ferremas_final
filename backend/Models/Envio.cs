using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ferremas.Api.Models
{
    public class Envio
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [Column("pedido_id")]
        public int PedidoId { get; set; }
        
        [Column("repartidor_id")]
        public int? RepartidorId { get; set; }
        
        [Column("fecha_envio")]
        public DateTime FechaEnvio { get; set; }
        
        [Column("estado_envio")]
        public string EstadoEnvio { get; set; } = "EN_PREPARACION";
        
        [Column("direccion_envio")]
        public string DireccionEnvio { get; set; } = string.Empty;
        
        [Column("proveedor_transporte")]
        public string ProveedorTransporte { get; set; } = "Chilexpress";
        
        [Column("tracking_url")]
        public string TrackingUrl { get; set; } = string.Empty;
        
        [Column("observaciones")]
        public string? Observaciones { get; set; }
        
        [Column("comuna_destino")]
        public string ComunaDestino { get; set; } = string.Empty;
        
        [Column("region_destino")]
        public string RegionDestino { get; set; } = string.Empty;
        
        [Column("telefono_contacto")]
        public string TelefonoContacto { get; set; } = string.Empty;
        
        [Column("nombre_destinatario")]
        public string NombreDestinatario { get; set; } = string.Empty;
        
        [Column("fecha_creacion")]
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        
        [Column("fecha_actualizacion")]
        public DateTime? FechaActualizacion { get; set; }
        
        // Relaciones
        [ForeignKey("PedidoId")]
        public required Pedido Pedido { get; set; }
    }
}