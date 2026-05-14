namespace SystemGym.Infrastructure.Persistence;

using SystemGym.Application.Abstractions;

/// <summary>
/// Implementación del patrón Unit of Work para coordinar cambios en múltiples repositorios
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly SystemGymDbContext _dbContext;
    private ISystemUserRepository? _systemUserRepository;
    private IClientRepository? _clientRepository;
    private IProductRepository? _productRepository;
    private IPlanRepository? _planRepository;
    private ISubscriptionRepository? _subscriptionRepository;
    private ISalesHistoryRepository? _salesHistoryRepository;
    private IInventoryLogRepository? _inventoryLogRepository;

    public UnitOfWork(SystemGymDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public ISystemUserRepository SystemUsers =>
        _systemUserRepository ??= new SystemUserRepository(_dbContext);

    public IClientRepository Clients =>
        _clientRepository ??= new ClientRepository(_dbContext);

    public IProductRepository Products =>
        _productRepository ??= new ProductRepository(_dbContext);

    public IPlanRepository Plans =>
        _planRepository ??= new PlanRepository(_dbContext);

    public ISubscriptionRepository Subscriptions =>
        _subscriptionRepository ??= new SubscriptionRepository(_dbContext);

    public ISalesHistoryRepository SalesHistory =>
        _salesHistoryRepository ??= new SalesHistoryRepository(_dbContext);

    public IInventoryLogRepository InventoryLogs =>
        _inventoryLogRepository ??= new InventoryLogRepository(_dbContext);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
            await _dbContext.Database.CommitTransactionAsync(cancellationToken);
        }
        catch
        {
            await _dbContext.Database.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.Database.RollbackTransactionAsync(cancellationToken);
    }

    public void Dispose()
    {
        _dbContext?.Dispose();
    }
}
