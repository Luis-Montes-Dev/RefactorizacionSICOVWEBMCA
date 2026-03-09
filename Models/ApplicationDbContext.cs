using Microsoft.EntityFrameworkCore;

namespace SICOVWEB_MCA.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        
        public DbSet<Proveedor> Proveedores { get; set; }
        public DbSet<Cotizacion_proveedor> Cotizaciones_proveedores { get; set; }
        public DbSet<DetalleCotizacionProveedor> DetalleCotizacionProveedor { get; set; }
        public DbSet<Compra> Compras { get; set; }
        public DbSet<DetalleCompra> Detalles_compra { get; set; }
        public DbSet<Evaluacion_compra> Evaluaciones_compra { get; set; }
        public DbSet<Empleado> Empleados { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Contacto_cliente> Contactos_cliente { get; set; }
        public DbSet<Cotizacion> Cotizaciones { get; set; }
        public DbSet<DetalleCotizacionCliente> DetalleCotizacionCliente { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<DetalleVenta> DetalleVentas { get; set; }
        public DbSet<Evaluacion_Venta> Evaluaciones_Venta { get; set; }
        public DbSet<Campania_Marketing> Campanias_Marketing { get; set; }
        public DbSet<Promocion> Promociones { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Almacen> Almacenes { get; set; }
        public DbSet<Inventario> Inventarios { get; set; }
        public DbSet<Movimiento_Almacen> Movimientos_Almacen { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Establece la relación entre Cotizacion y DetalleCotizacionCliente
            modelBuilder.Entity<DetalleCotizacionCliente>()
                .HasOne(d => d.Cotizacion)
                .WithMany(c => c.Detalles)
                .HasForeignKey(d => d.IdCotizacion);
            modelBuilder.Entity<DetalleCotizacionCliente>()
                .HasKey(d => d.IdDetalle);

            modelBuilder.Entity<DetalleCotizacionCliente>()
                .Property(d => d.IdDetalle)
                .ValueGeneratedOnAdd();

            // Establece la relación entre Cotizacion_proveedor y DetalleCotizacionProveedor
            modelBuilder.Entity<DetalleCotizacionProveedor>()
                .HasOne(d => d.Cotizacion_proveedor)
                .WithMany(c => c.Detalles)
                .HasForeignKey(d => d.Id_Cotizacion);
            modelBuilder.Entity<DetalleCotizacionProveedor>()
                .HasKey(d => d.Id_Detalle);

            modelBuilder.Entity<DetalleCotizacionProveedor>()
                .Property(d => d.Id_Detalle)
                .ValueGeneratedOnAdd();

            // Establece la relación entre Compra y DetalleCompra
            modelBuilder.Entity<DetalleCompra>()
                .HasOne(d => d.Compra)
                .WithMany(c => c.Detalles)
                .HasForeignKey(d => d.Id_Compra);
            modelBuilder.Entity<DetalleCompra>()
                .HasKey(d => d.Id_Detalle);
            modelBuilder.Entity<DetalleCompra>()
                .Property(d => d.Id_Detalle)
                .ValueGeneratedOnAdd();

            base.OnModelCreating(modelBuilder);
        }
    }
}

