namespace HelpDesk.Domain.Ports;

public interface IUnitOfWork
{
    IGenericRepository<T> Repository<T>(string? name = null) where T : class;
    IQueryable<T> Query<T>(string? setName = null) where T : class;
    Task<List<T>> ListAsync<T>(IQueryable<T> query, CancellationToken ct);
    Task<int> CountAsync<T>(IQueryable<T> query, CancellationToken ct);
    Task<T?> FindAsync<T>(object[] keys, CancellationToken ct, string? setName = null) where T : class;
    void Add<T>(T entity, string? setName = null) where T : class;
    void Remove<T>(T entity, string? setName = null) where T : class;
    Task<int> Complete(CancellationToken ct);
    Task<T> InTransactionAsync<T>(Func<Task<T>> operation, CancellationToken ct);
}
