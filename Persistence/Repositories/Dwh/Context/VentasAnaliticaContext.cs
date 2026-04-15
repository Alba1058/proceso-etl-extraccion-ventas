using Microsoft.EntityFrameworkCore;
using SistemaAnalisisVentas.Domain.Entities.Dwh.Dimensions;
using SistemaAnalisisVentas.Domain.Entities.Dwh.Facts;

namespace SistemaAnalisisVentas.Persistence.Repositories.Dwh.Context
{
    public class VentasAnaliticaContext : DbContext
    {
        public VentasAnaliticaContext(DbContextOptions<VentasAnaliticaContext> options) : base(options)
        {
        }

        public DbSet<DimProducto> Dim_Producto { get; set; } = null!;
        public DbSet<DimCliente> Dim_Cliente { get; set; } = null!;
        public DbSet<DimTiempo> Dim_Tiempo { get; set; } = null!;
        public DbSet<DimSucursal> Dim_Sucursal { get; set; } = null!;
        public DbSet<DimUbicacion> Dim_Ubicacion { get; set; } = null!;
        public DbSet<DimFuenteDatos> Dim_Fuente_Datos { get; set; } = null!;
        public DbSet<FactVenta> Fact_Ventas { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // DimProducto
            modelBuilder.Entity<DimProducto>(entity =>
            {
                entity.HasKey(e => e.IdProducto);
                entity.HasIndex(e => e.ProductoOrigenId).IsUnique();
                entity.Property(e => e.NombreProducto).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Categoria).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Marca).HasMaxLength(100).IsRequired();
                entity.Property(e => e.PrecioBase).HasColumnType("decimal(18,2)");
                entity.Property(e => e.EstadoProducto).HasMaxLength(50).IsRequired();
            });

            // DimCliente
            modelBuilder.Entity<DimCliente>(entity =>
            {
                entity.HasKey(e => e.IdCliente);
                entity.HasIndex(e => e.ClienteOrigenId).IsUnique();
                entity.Property(e => e.NombreCliente).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Telefono).HasMaxLength(50).IsRequired();
                entity.Property(e => e.SegmentoCliente).HasMaxLength(100).IsRequired();
                entity.Property(e => e.FechaRegistro).HasColumnType("date");
            });

            // DimTiempo
            modelBuilder.Entity<DimTiempo>(entity =>
            {
                entity.HasKey(e => e.IdTiempo);
                entity.Property(e => e.IdTiempo).ValueGeneratedNever(); // Natural key (YYYYMMDD)
                entity.HasIndex(e => e.Fecha).IsUnique();
                entity.Property(e => e.Fecha).HasColumnType("date");
                entity.Property(e => e.NombreMes).HasMaxLength(30).IsRequired();
            });

            // DimSucursal
            modelBuilder.Entity<DimSucursal>(entity =>
            {
                entity.HasKey(e => e.IdSucursal);
                entity.HasIndex(e => new { e.NombreSucursal, e.Direccion }).IsUnique();
                entity.Property(e => e.NombreSucursal).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Direccion).HasMaxLength(300).IsRequired();
            });

            // DimUbicacion
            modelBuilder.Entity<DimUbicacion>(entity =>
            {
                entity.HasKey(e => e.IdUbicacion);
                entity.HasIndex(e => new { e.Pais, e.Region, e.Ciudad }).IsUnique();
                entity.Property(e => e.Pais).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Region).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Ciudad).HasMaxLength(100).IsRequired();
            });

            // DimFuenteDatos
            modelBuilder.Entity<DimFuenteDatos>(entity =>
            {
                entity.HasKey(e => e.IdFuente);
                entity.HasIndex(e => new { e.NombreFuente, e.TipoFuente }).IsUnique();
                entity.Property(e => e.NombreFuente).HasMaxLength(100).IsRequired();
                entity.Property(e => e.TipoFuente).HasMaxLength(50).IsRequired();
                entity.Property(e => e.DescripcionFuente).HasMaxLength(300).IsRequired();
            });

            // FactVenta
            modelBuilder.Entity<FactVenta>(entity =>
            {
                entity.HasKey(e => e.IdVenta);
                entity.HasIndex(e => new { 
                    e.NumeroOrdenOrigen, e.IdProducto, e.IdCliente, e.IdTiempo, e.IdSucursal, e.IdUbicacion, e.IdFuente 
                }).IsUnique().HasDatabaseName("IX_FactVentas_NaturalKey");

                entity.Property(e => e.PrecioUnitario).HasColumnType("decimal(18,2)");
                entity.Property(e => e.TotalVenta).HasColumnType("decimal(18,2)");

                entity.HasOne(d => d.Producto)
                    .WithMany()
                    .HasForeignKey(d => d.IdProducto)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_FactVentas_DimProducto");

                entity.HasOne(d => d.Cliente)
                    .WithMany()
                    .HasForeignKey(d => d.IdCliente)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_FactVentas_DimCliente");

                entity.HasOne(d => d.Tiempo)
                    .WithMany()
                    .HasForeignKey(d => d.IdTiempo)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_FactVentas_DimTiempo");

                entity.HasOne(d => d.Sucursal)
                    .WithMany()
                    .HasForeignKey(d => d.IdSucursal)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_FactVentas_DimSucursal");

                entity.HasOne(d => d.Ubicacion)
                    .WithMany()
                    .HasForeignKey(d => d.IdUbicacion)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_FactVentas_DimUbicacion");

                entity.HasOne(d => d.FuenteDatos)
                    .WithMany()
                    .HasForeignKey(d => d.IdFuente)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_FactVentas_DimFuente");
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
