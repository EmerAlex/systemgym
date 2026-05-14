namespace SystemGym.Application.Abstractions;

using SystemGym.Domain.Entities;

/// <summary>
/// Interfaz para el repositorio de usuarios del sistema
/// </summary>
public interface ISystemUserRepository
{
    Task<SystemUser?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<SystemUser?> FirstOrDefaultAsync(System.Linq.Expressions.Expression<Func<SystemUser, bool>> predicate, CancellationToken cancellationToken = default);
    Task<IEnumerable<SystemUser>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<(IEnumerable<SystemUser> Items, int Total)> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    void Add(SystemUser entity);
    void Update(SystemUser entity);
    void Delete(SystemUser entity);
}

/// <summary>
/// Interfaz para hash y verificación de contraseñas.
/// </summary>
public interface IPasswordHasher
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string passwordHash);
}

/// <summary>
/// Interfaz para el repositorio de clientes
/// </summary>
public interface IClientRepository
{
    Task<Client?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Client?> FirstOrDefaultAsync(System.Linq.Expressions.Expression<Func<Client, bool>> predicate, CancellationToken cancellationToken = default);
    Task<Client?> GetByNumeroDocumentoAsync(string numeroDocumento, string? tipoDocumento, CancellationToken cancellationToken = default);
    Task<IEnumerable<Client>> SearchByNombreAsync(string nombre, CancellationToken cancellationToken = default);
    Task<IEnumerable<Client>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<(IEnumerable<Client> Items, int Total)> GetPagedAsync(int pageNumber, int pageSize, string? searchTerm = null, CancellationToken cancellationToken = default);
    void Add(Client entity);
    void Update(Client entity);
    void Delete(Client entity);
}

/// <summary>
/// Interfaz para el repositorio de productos
/// </summary>
public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Product>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<(IEnumerable<Product> Items, int Total)> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    void Add(Product entity);
    void Update(Product entity);
    void Delete(Product entity);
}

/// <summary>
/// Interfaz para el repositorio de planes
/// </summary>
public interface IPlanRepository
{
    Task<Plan?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Plan>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<(IEnumerable<Plan> Items, int Total)> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    void Add(Plan entity);
    void Update(Plan entity);
    void Delete(Plan entity);
}

/// <summary>
/// Interfaz para el repositorio de suscripciones
/// </summary>
public interface ISubscriptionRepository
{
    Task<Subscription?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Subscription?> FirstOrDefaultAsync(System.Linq.Expressions.Expression<Func<Subscription, bool>> predicate, CancellationToken cancellationToken = default);
    Task<IEnumerable<Subscription>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<(IEnumerable<Subscription> Items, int Total)> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<(IEnumerable<Subscription> Items, int Total)> GetPagedByClientAsync(Guid clientId, int pageNumber, int pageSize, bool? activeOnly = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<Subscription>> GetAllByClientIdAsync(Guid clientId, CancellationToken cancellationToken = default);
    void Add(Subscription entity);
    void Update(Subscription entity);
    void Delete(Subscription entity);
}

/// <summary>
/// Interfaz para el repositorio de historial de ventas
/// </summary>
public interface ISalesHistoryRepository
{
    Task<SalesHistory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<SalesHistory>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<(IEnumerable<SalesHistory> Items, int Total)> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    void Add(SalesHistory entity);
}

/// <summary>
/// Interfaz para el repositorio de log de inventario
/// </summary>
public interface IInventoryLogRepository
{
    Task<IEnumerable<InventoryLog>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<(IEnumerable<InventoryLog> Items, int Total)> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    void Add(InventoryLog entity);
}

/// <summary>
/// Interfaz para el patrón Unit of Work
/// </summary>
public interface IUnitOfWork : IDisposable
{
    ISystemUserRepository SystemUsers { get; }
    IClientRepository Clients { get; }
    IProductRepository Products { get; }
    IPlanRepository Plans { get; }
    ISubscriptionRepository Subscriptions { get; }
    ISalesHistoryRepository SalesHistory { get; }
    IInventoryLogRepository InventoryLogs { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
