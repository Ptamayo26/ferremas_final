namespace Ferremas.Api.Constants
{
    public static class EstadosPedido
    {
        public const string Pendiente = "Pendiente";
        public const string EnProceso = "En Proceso";
        public const string Enviado = "Enviado";
        public const string Entregado = "Entregado";
        public const string Cancelado = "Cancelado";

        public static class Permissions
        {
            public static readonly string[] Repartidor = new[] { EnProceso, Entregado };
            public static readonly string[] AdministradorVendedor = new[] { Pendiente, EnProceso, Enviado, Entregado, Cancelado };
        }
    }
} 