using System.Linq.Expressions;
using HelpDesk.Domain.Ports;
using HelpDesk.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Infrastructure.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly DbSet<T> _entities;

    public GenericRepository(HelpDeskDbContext context, string? setName = null)
    {
        // Solo las cuatro tablas puente del scaffolding necesitan un nombre de conjunto.
        _entities = setName is null ? context.Set<T>() : context.Set<T>(setName);
    }

    public IQueryable<T> Query() => _entities;

    public async Task<IEnumerable<T>> GetAllAsync()
        => await _entities.AsNoTracking().ToListAsync();

    public async Task<T?> GetByIdAsync(params object[] ids)
        => await _entities.FindAsync(ids);

    public Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        => _entities.AsNoTracking().FirstOrDefaultAsync(predicate);

    public async Task AddAsync(T entity) => await _entities.AddAsync(entity);

    public void Update(T entity) => _entities.Update(entity);

    public void Delete(T entity) => _entities.Remove(entity);
}
