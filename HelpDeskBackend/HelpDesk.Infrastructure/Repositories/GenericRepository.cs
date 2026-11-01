using HelpDesk.Domain.Ports;
using HelpDesk.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
namespace HelpDesk.Infrastructure.Repositories;
public sealed class GenericRepository<T>(HelpDeskDbContext context,string? setName=null):IGenericRepository<T> where T:class
{
    private DbSet<T> Set=>setName is null?context.Set<T>():context.Set<T>(setName);
    public IQueryable<T> Query()=>Set;
    public async Task<T?> FindAsync(object[] keys,CancellationToken ct)=>await Set.FindAsync(keys,ct);
    public void Add(T entity)=>Set.Add(entity);
    public void Remove(T entity)=>Set.Remove(entity);
}
