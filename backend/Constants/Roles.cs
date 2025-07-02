namespace Ferremas.Api.Constants
{
    /// <summary>
    /// Constantes para los roles del sistema
    /// </summary>
    public static class Roles
    {
        /// <summary>
        /// Rol de administrador del sistema
        /// </summary>
        public const string Administrador = "administrador";

        /// <summary>
        /// Rol de vendedor
        /// </summary>
        public const string Vendedor = "vendedor";

        /// <summary>
        /// Rol de bodeguero
        /// </summary>
        public const string Bodeguero = "bodeguero";

        /// <summary>
        /// Rol de contador
        /// </summary>
        public const string Contador = "contador";

        /// <summary>
        /// Rol de repartidor
        /// </summary>
        public const string Repartidor = "repartidor";

        /// <summary>
        /// Rol de cliente
        /// </summary>
        public const string Cliente = "cliente";

        /// <summary>
        /// Obtiene todos los roles disponibles
        /// </summary>
        public static string[] Todos => new[]
        {
            Administrador,
            Vendedor,
            Bodeguero,
            Contador,
            Repartidor,
            Cliente
        };

        /// <summary>
        /// Obtiene los roles que pueden gestionar pedidos
        /// </summary>
        public static string[] RolesGestionPedidos => new[]
        {
            Administrador,
            Vendedor,
            Repartidor
        };

        /// <summary>
        /// Obtiene los roles que pueden gestionar inventario
        /// </summary>
        public static string[] RolesGestionInventario => new[]
        {
            Administrador,
            Bodeguero
        };

        /// <summary>
        /// Obtiene los roles que pueden gestionar pagos
        /// </summary>
        public static string[] RolesGestionPagos => new[]
        {
            Administrador,
            Vendedor,
            Contador
        };
    }
} 