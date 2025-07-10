using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ferremas.Api.Models;

namespace Ferremas.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; } = null!;
        public DbSet<Categoria> Categorias { get; set; } = null!;
        public DbSet<Marca> Marcas { get; set; } = null!;
        public DbSet<Producto> Productos { get; set; } = null!;
        public DbSet<Pedido> Pedidos { get; set; } = null!;
        public DbSet<DetallePedido> DetallesPedido { get; set; } = null!;
        public DbSet<Pago> Pagos { get; set; } = null!;
        public DbSet<Direccion> Direcciones { get; set; } = null!;
        public DbSet<Log> Logs { get; set; } = null!;
        public DbSet<ComparadorPrecio> ComparadorPrecios { get; set; } = null!;
        public DbSet<Tienda> Tiendas { get; set; } = null!;
        public DbSet<Envio> Envios { get; set; } = null!;
        public DbSet<ConversionDivisa> ConversionDivisas { get; set; } = null!;
        public DbSet<Cliente> Clientes { get; set; } = null!;
        public DbSet<ComparacionHistorial> ComparacionesHistorial { get; set; } = null!;
        public DbSet<Proveedor> Proveedores { get; set; } = null!;
        public DbSet<Factura> Facturas { get; set; } = null!;
        public DbSet<Notificacion> Notificaciones { get; set; } = null!;
        public DbSet<Descuento> Descuentos { get; set; } = null!;
        public DbSet<Promocion> Promociones { get; set; } = null!;
        public DbSet<LogAuditoria> LogsAuditoria { get; set; } = null!;
        public DbSet<Carrito> Carrito { get; set; } = null!;
        public DbSet<DatosFacturaEmpresa> DatosFacturaEmpresa { get; set; }
        public DbSet<ListaDeseos> ListasDeseos { get; set; } = null!;
        public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de Usuario
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("usuarios");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.Rol).HasMaxLength(30); // Según tu BD
                entity.HasIndex(u => u.Email).IsUnique();
                entity.HasIndex(u => u.Rut).IsUnique();
            });

            // CONFIGURACIÓN CORREGIDA DE PRODUCTO - ¡ESTO ES LO MÁS IMPORTANTE!
            modelBuilder.Entity<Producto>(entity =>
            {
                entity.ToTable("productos");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(150);
                entity.Property(e => e.Codigo).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Descripcion).HasMaxLength(1000);
                entity.Property(e => e.Precio).HasColumnType("decimal(10,2)");
                entity.Property(e => e.ImagenUrl).HasMaxLength(200).HasDefaultValue("Sin imagen");
                
                // CONFIGURAR LAS RELACIONES EXPLÍCITAMENTE
                entity.HasOne(p => p.Categoria)
                      .WithMany(c => c.Productos)
                      .HasForeignKey(p => p.CategoriaId)
                      .OnDelete(DeleteBehavior.Restrict);
                      
                entity.HasOne(p => p.Marca)
                      .WithMany(m => m.Productos)
                      .HasForeignKey(p => p.MarcaId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración de Categoria
            modelBuilder.Entity<Categoria>(entity =>
            {
                entity.ToTable("categorias");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Descripcion).HasMaxLength(500);
                entity.Property(e => e.Codigo).IsRequired().HasMaxLength(10);
                
                entity.HasOne(e => e.CategoriaPadre)
                    .WithMany(e => e.Subcategorias)
                    .HasForeignKey(e => e.CategoriaPadreId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración de Marca
            modelBuilder.Entity<Marca>(entity =>
            {
                entity.ToTable("marcas");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Descripcion).HasMaxLength(1000);
                entity.Property(e => e.LogoUrl).HasMaxLength(255);
                entity.HasIndex(e => e.Nombre).IsUnique();
            });

            // Configuración de Pedido
            modelBuilder.Entity<Pedido>(entity =>
            {
                entity.ToTable("pedidos");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Total).HasColumnType("decimal(10,2)");
                
                // Agregar la relación con Usuario
                entity.HasOne(e => e.Usuario)
                    .WithMany(u => u.Pedidos)
                    .HasForeignKey(e => e.UsuarioId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración de DetallePedido
            modelBuilder.Entity<DetallePedido>(entity =>
            {
                entity.ToTable("detallespedido");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PrecioUnitario).HasColumnType("decimal(10,2)");
                entity.Property(e => e.Subtotal).HasColumnType("decimal(10,2)");
                
                entity.HasOne(e => e.Pedido)
                    .WithMany(p => p.Detalles)
                    .HasForeignKey(e => e.PedidoId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Producto)
                    .WithMany(p => p.DetallesPedido)
                    .HasForeignKey(e => e.ProductoId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración de Pago
            modelBuilder.Entity<Pago>(entity =>
            {
                entity.ToTable("pagos");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Monto).HasColumnType("decimal(10,2)");
                entity.Property(e => e.MetodoPago).IsRequired().HasMaxLength(50);
                entity.Property(e => e.TransaccionId).HasMaxLength(100);
                entity.Property(e => e.TokenPasarela).HasMaxLength(100);
                entity.Property(e => e.DatosRespuesta);
                entity.Property(e => e.FechaPago).IsRequired();
                entity.Property(e => e.Estado).IsRequired().HasMaxLength(50);

                // Agregar la relación con Pedido
                entity.HasOne(p => p.Pedido)
                    .WithMany()
                    .HasForeignKey(p => p.PedidoId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración de ComparacionHistorial
            modelBuilder.Entity<ComparacionHistorial>(entity =>
            {
                entity.ToTable("comparaciones_historial");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Producto).IsRequired().HasMaxLength(200);
                entity.Property(e => e.PrecioFerremas).HasColumnType("decimal(10,2)");
                entity.Property(e => e.Resultados).IsRequired();
                entity.Property(e => e.FechaComparacion).IsRequired();
                entity.HasOne(e => e.Usuario)
                    .WithMany()
                    .HasForeignKey(e => e.UsuarioId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración de ComparadorPrecio
            modelBuilder.Entity<ComparadorPrecio>(entity =>
            {
                entity.ToTable("comparadorprecios");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PrecioCompetidor).HasColumnType("decimal(10,2)");
                
                entity.HasOne(cp => cp.Producto)
                    .WithMany()
                    .HasForeignKey(cp => cp.ProductoId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuración de Cliente
            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.ToTable("clientes");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).HasMaxLength(100);
                entity.Property(e => e.Apellido).HasMaxLength(100);
                entity.Property(e => e.Rut).HasMaxLength(20);
                entity.Property(e => e.CorreoElectronico).HasMaxLength(100);
                entity.Property(e => e.Telefono).HasMaxLength(20);
                entity.Property(e => e.FechaCreacion);
                entity.Property(e => e.FechaModificacion);
                entity.Property(e => e.Activo);
            });

            // Configuración de Direccion
            modelBuilder.Entity<Direccion>(entity =>
            {
                entity.ToTable("direcciones");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Calle).HasMaxLength(200);
                entity.Property(e => e.Numero).HasMaxLength(20);
                entity.Property(e => e.Departamento).HasMaxLength(50);
                entity.Property(e => e.Comuna).HasMaxLength(100);
                entity.Property(e => e.Region).HasMaxLength(100);
                entity.Property(e => e.CodigoPostal).HasMaxLength(20);
                entity.Property(e => e.EsPrincipal);
                entity.Property(e => e.FechaCreacion);
                entity.Property(e => e.FechaModificacion);

                // Configurar la relación con Usuario
                entity.HasOne(d => d.Usuario)
                    .WithMany(u => u.Direcciones)
                    .HasForeignKey(d => d.UsuarioId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuración de Envio
            modelBuilder.Entity<Envio>(entity =>
            {
                entity.ToTable("envios");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.DireccionEnvio).IsRequired().HasMaxLength(200);
                entity.Property(e => e.EstadoEnvio).HasMaxLength(20);
                entity.Property(e => e.ProveedorTransporte).IsRequired().HasMaxLength(100);
                entity.Property(e => e.TrackingUrl).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Observaciones).HasMaxLength(500);
                entity.Property(e => e.ComunaDestino).IsRequired().HasMaxLength(50);
                entity.Property(e => e.RegionDestino).IsRequired().HasMaxLength(50);
                entity.Property(e => e.TelefonoContacto).IsRequired().HasMaxLength(20);
                entity.Property(e => e.NombreDestinatario).IsRequired().HasMaxLength(100);
                entity.Property(e => e.FechaCreacion).IsRequired();
                entity.Property(e => e.FechaActualizacion);
            });

            // Configuración de Log
            modelBuilder.Entity<Log>(entity =>
            {
                entity.ToTable("logs");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nivel).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Mensaje).IsRequired().HasMaxLength(1000);
                entity.Property(e => e.Excepcion).HasMaxLength(4000);
                entity.Property(e => e.Fecha).IsRequired();
            });

            // Configuración de Proveedor
            modelBuilder.Entity<Proveedor>(entity =>
            {
                entity.ToTable("proveedores");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Contacto).HasMaxLength(100);
                entity.Property(e => e.Correo).HasMaxLength(100);
                entity.Property(e => e.Telefono).HasMaxLength(20);
                entity.Property(e => e.Direccion).HasMaxLength(200);
                entity.Property(e => e.Activo).IsRequired();
            });

            // Configuración de Factura
            modelBuilder.Entity<Factura>(entity =>
            {
                entity.ToTable("facturas");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FechaEmision).IsRequired();
                entity.Property(e => e.MontoTotal).HasColumnType("decimal(10,2)");
                entity.Property(e => e.Anulada).IsRequired();
                
                entity.HasOne(f => f.Pedido)
                    .WithMany()
                    .HasForeignKey(f => f.PedidoId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración de Notificacion
            modelBuilder.Entity<Notificacion>(entity =>
            {
                entity.ToTable("notificaciones");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Mensaje).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Leida).IsRequired();
                entity.Property(e => e.FechaCreacion).IsRequired();
                
                entity.HasOne(n => n.Usuario)
                    .WithMany()
                    .HasForeignKey(n => n.UsuarioId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuración de Descuento
            modelBuilder.Entity<Descuento>(entity =>
            {
                entity.ToTable("descuentos");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Codigo).IsRequired().HasMaxLength(50);
                entity.Property(e => e.FechaInicio).IsRequired();
                entity.Property(e => e.FechaFin).IsRequired();
                entity.Property(e => e.Activo).IsRequired();
                
                entity.HasIndex(e => e.Codigo).IsUnique();
            });

            // Configuración de Carrito
            modelBuilder.Entity<Carrito>(entity =>
            {
                entity.ToTable("carrito");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Cantidad).IsRequired();
                entity.Property(e => e.FechaAgregado).IsRequired();
                entity.Property(e => e.Activo).IsRequired();
                
                // Relación con Usuario
                entity.HasOne(c => c.Usuario)
                    .WithMany()
                    .HasForeignKey(c => c.UsuarioId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                // Relación con Producto
                entity.HasOne(c => c.Producto)
                    .WithMany()
                    .HasForeignKey(c => c.ProductoId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuración de RefreshToken
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.ToTable("refresh_tokens");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Token).IsRequired().HasMaxLength(500);
                entity.Property(e => e.ExpiryDate).IsRequired();
                entity.Property(e => e.IsRevoked).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
                
                // Relación con Usuario
                entity.HasOne(rt => rt.Usuario)
                    .WithMany()
                    .HasForeignKey(rt => rt.UsuarioId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
