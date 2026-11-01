namespace HelpDesk.Domain.Ports;
public interface IGenericRepository<T> where T:class
{
    IQueryable<T> Query();
    Task<T?> FindAsync(object[] keys,CancellationToken ct);
    void Add(T entity);
    void Remove(T entity);
}
