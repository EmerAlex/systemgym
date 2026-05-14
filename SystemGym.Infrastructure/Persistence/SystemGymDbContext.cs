namespace SystemGym.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SystemGym.Domain.Entities;

/// <summary>
/// Contexto de base de datos para el sistema de gestión del gimnasio
/// </summary>
public class SystemGymDbContext : DbContext
{
    public DbSet<SystemUser> SystemUsers { get; set; }
    public DbSet<Client> Clients { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Plan> Plans { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<SalesHistory> SalesHistories { get; set; }
    public DbSet<InventoryLog> InventoryLogs { get; set; }

    public SystemGymDbContext(DbContextOptions<SystemGymDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configurar SystemUser
        modelBuilder.Entity<SystemUser>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.Habilitado).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();

            entity.HasIndex(e => e.Username).IsUnique();

            entity.OwnsOne(e => e.Role, ro =>
            {
                ro.Property(r => r.Value).HasColumnName("Role").IsRequired().HasMaxLength(20);
            });
        });

        // Configurar Client
        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.NumeroDocumento).IsRequired().HasMaxLength(20);
            entity.Property(e => e.NombreCompleto).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Habilitado).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();

            entity.HasIndex(e => e.NumeroDocumento).IsUnique();

            entity.OwnsOne(e => e.TipoDocumento, td =>
            {
                td.Property(d => d.Value).HasColumnName("TipoDocumento").IsRequired().HasMaxLength(10);
            });

            entity.OwnsOne(e => e.Celular, cel =>
            {
                cel.Property(c => c.Value).HasColumnName("Celular").HasMaxLength(20);
            });
        });

        // Configurar Product
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Descripcion).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Valor).IsRequired().HasColumnType("decimal(18,2)");
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
        });

        // Configurar Plan
        modelBuilder.Entity<Plan>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Descripcion).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Valor).IsRequired().HasColumnType("decimal(18,2)");
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();

            entity.OwnsOne(e => e.TipoPeriodo, tp =>
            {
                tp.Property(p => p.Value).HasColumnName("TipoPeriodo").IsRequired().HasMaxLength(10);
            });
        });

        // Configurar Subscription
        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ClientId).IsRequired();
            entity.Property(e => e.PlanId).IsRequired();
            entity.Property(e => e.InicioVigencia).IsRequired();
            entity.Property(e => e.FinVigencia).IsRequired(false);
            entity.Property(e => e.Valor).IsRequired().HasColumnType("decimal(18,2)");
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();

            entity.HasIndex(e => new { e.ClientId, e.PlanId });

            // Índice para consultas de suscripciones activas
            entity.HasIndex(e => e.Activa);
        });

        // Configurar SalesHistory
        modelBuilder.Entity<SalesHistory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ClientId).IsRequired();
            entity.Property(e => e.ProductId).IsRequired();
            entity.Property(e => e.UserId).IsRequired(false); // Nullable para operaciones del sistema
            entity.Property(e => e.Monto).IsRequired().HasColumnType("decimal(18,2)");
            entity.Property(e => e.Pagado).IsRequired();
            entity.Property(e => e.FechaVenta).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();

            entity.HasIndex(e => new { e.ClientId, e.FechaVenta });
            entity.HasIndex(e => e.FechaVenta);
        });

        // Configurar InventoryLog
        modelBuilder.Entity<InventoryLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ProductId).IsRequired();
            entity.Property(e => e.UserId).IsRequired();
            entity.Property(e => e.CantidadAnterior).IsRequired();
            entity.Property(e => e.CantidadNueva).IsRequired();
            entity.Property(e => e.Diferencia).IsRequired();
            entity.Property(e => e.Operacion).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Razon).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).IsRequired();

            // Índice para consultas de log de inventario
            entity.HasIndex(e => new { e.ProductId, e.CreatedAt });
        });

        // Aplicar conversión automática a UTC para TODOS los DateTime
        // IMPORTANTE: SIEMPRE normalizar a fecha-solo (medianoche) para consistencia
        var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
            v => DateTime.SpecifyKind(v.Date, DateTimeKind.Utc),  // SIEMPRE tomar .Date para garantizar medianoche
            v => v);  // La BD devuelve ya con Kind=Utc

        var nullableDateTimeConverter = new ValueConverter<DateTime?, DateTime?>(
            v => v.HasValue ? DateTime.SpecifyKind(v.Value.Date, DateTimeKind.Utc) : null,  // SIEMPRE tomar .Date
            v => v);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime))
                    property.SetValueConverter(dateTimeConverter);
                else if (property.ClrType == typeof(DateTime?))
                    property.SetValueConverter(nullableDateTimeConverter);
            }
        }
    }
}
