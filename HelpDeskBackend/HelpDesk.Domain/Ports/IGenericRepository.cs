using System.Linq.Expressions;

namespace HelpDesk.Domain.Ports;

public interface IGenericRepository<T> where T : class
{
    IQueryable<T> Query();
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(params object[] ids);
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
}
