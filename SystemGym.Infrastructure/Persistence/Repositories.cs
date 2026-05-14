namespace SystemGym.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using SystemGym.Application.Abstractions;
using SystemGym.Domain.Abstractions;
using SystemGym.Domain.Entities;

/// <summary>
/// Repositorio base genérico para todas las entidades.
/// TEntity : Entity garantiza que siempre existan Id y CreatedAt para ordenamiento y búsqueda.
/// </summary>
public abstract class BaseRepository<TEntity> where TEntity : Entity
{
    protected readonly SystemGymDbContext DbContext;
    protected readonly DbSet<TEntity> DbSet;

    protected BaseRepository(SystemGymDbContext dbContext)
    {
        DbContext = dbContext;
        DbSet = dbContext.Set<TEntity>();
    }

    public virtual async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet.FindAsync(new object[] { id }, cancellationToken: cancellationToken);
    }

    public virtual async Task<TEntity?> FirstOrDefaultAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Paginación estándar ordenada por CreatedAt DESC.
    /// Los repositorios con criterio de ordenación distinto deben sobreescribir este método.
    /// </summary>
    public virtual async Task<(IEnumerable<TEntity> Items, int Total)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var total = await DbSet.CountAsync(cancellationToken);
        var items = await DbSet
            .OrderByDescending(x => x.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public virtual void Add(TEntity entity)
    {
        DbSet.Add(entity);
    }

    public virtual void Update(TEntity entity)
    {
        DbSet.Update(entity);
    }

    public virtual void Delete(TEntity entity)
    {
        DbSet.Remove(entity);
    }
}

/// <summary>
/// Repositorio para SystemUser
/// </summary>
public class SystemUserRepository : BaseRepository<SystemUser>, ISystemUserRepository
{
    public SystemUserRepository(SystemGymDbContext dbContext) : base(dbContext)
    {
    }
}

/// <summary>
/// Repositorio para Client
/// </summary>
public class ClientRepository : BaseRepository<Client>, IClientRepository
{
    public ClientRepository(SystemGymDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Client?> GetByNumeroDocumentoAsync(
        string numeroDocumento,
        string? tipoDocumento,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet.Where(c => c.NumeroDocumento == numeroDocumento);

        if (!string.IsNullOrWhiteSpace(tipoDocumento))
            query = query.Where(c => c.TipoDocumento.Value == tipoDocumento);

        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<Client>> SearchByNombreAsync(
        string nombre,
        CancellationToken cancellationToken = default)
    {
        var busqueda = nombre.Trim().ToLower();
        return await DbSet
            .Where(c => c.NombreCompleto.ToLower().Contains(busqueda))
            .OrderBy(c => c.NombreCompleto)
            .Take(50)
            .ToListAsync(cancellationToken);
    }

    public override async Task<(IEnumerable<Client> Items, int Total)> GetPagedAsync(
        int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        => await GetPagedAsync(pageNumber, pageSize, null, cancellationToken);

    public async Task<(IEnumerable<Client> Items, int Total)> GetPagedAsync(
        int pageNumber, int pageSize, string? searchTerm, CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = query.Where(c =>
                c.NombreCompleto.ToLower().Contains(term) ||
                c.NumeroDocumento.ToLower().Contains(term));
        }

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }
}

/// <summary>
/// Repositorio para Product
/// </summary>
public class ProductRepository : BaseRepository<Product>, IProductRepository
{
    public ProductRepository(SystemGymDbContext dbContext) : base(dbContext)
    {
    }
}

/// <summary>
/// Repositorio para Plan
/// </summary>
public class PlanRepository : BaseRepository<Plan>, IPlanRepository
{
    public PlanRepository(SystemGymDbContext dbContext) : base(dbContext)
    {
    }
}

/// <summary>
/// Repositorio para Subscription
/// </summary>
public class SubscriptionRepository : BaseRepository<Subscription>, ISubscriptionRepository
{
    public SubscriptionRepository(SystemGymDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<(IEnumerable<Subscription> Items, int Total)> GetPagedByClientAsync(
        Guid clientId,
        int pageNumber,
        int pageSize,
        bool? activeOnly = null,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet.Where(s => s.ClientId == clientId);

        if (activeOnly == true)
            query = query.Where(s => s.Activa);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public async Task<IEnumerable<Subscription>> GetAllByClientIdAsync(
        Guid clientId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(s => s.ClientId == clientId)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}

/// <summary>
/// Repositorio para SalesHistory
/// </summary>
public class SalesHistoryRepository : BaseRepository<SalesHistory>, ISalesHistoryRepository
{
    public SalesHistoryRepository(SystemGymDbContext dbContext) : base(dbContext)
    {
    }

    public override async Task<(IEnumerable<SalesHistory> Items, int Total)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsQueryable();
        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(x => x.FechaVenta)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }
}

/// <summary>
/// Repositorio para InventoryLog
/// </summary>
public class InventoryLogRepository : BaseRepository<InventoryLog>, IInventoryLogRepository
{
    public InventoryLogRepository(SystemGymDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IEnumerable<InventoryLog>> GetByProductIdAsync(
        Guid productId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(x => x.ProductId == productId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public override async Task<(IEnumerable<InventoryLog> Items, int Total)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsQueryable();
        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }
}
