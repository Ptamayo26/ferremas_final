using System;

namespace Ferremas.Api.Models
{
    public class Inventario { public int Id { get; set; } public int ProductoId { get; set; } public int? SucursalId { get; set; } public int Stock { get; set; } = 0; public DateTime? UltimaSalida { get; set; } public DateTime? UltimoIngreso { get; set; } }
}