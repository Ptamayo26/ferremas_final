namespace Ferremas.Api.DTOs
{
    public class ClienteDTO
    {
        public int Id { get; set; }
        public required string Nombre { get; set; }
        public required string Apellido { get; set; }
        public required string Rut { get; set; }
        public required string CorreoElectronico { get; set; }
        public required string Telefono { get; set; }
        public bool Activo { get; set; }
    }
}