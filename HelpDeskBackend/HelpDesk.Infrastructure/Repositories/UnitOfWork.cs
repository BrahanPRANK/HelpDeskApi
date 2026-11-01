using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using HelpDesk.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;
namespace HelpDesk.Infrastructure.Repositories;

public sealed class UnitOfWork(HelpDeskDbContext context, ICurrentUser actor) : IUnitOfWork
{
    private readonly Dictionary<(Type, string?), object> repositories = [];
    public IGenericRepository<T> Repository<T>(string? name = null) where T : class
    {
        if (!repositories.TryGetValue((typeof(T), name), out var repo))
            repositories[(typeof(T), name)] = repo = new GenericRepository<T>(context, name);
        return (IGenericRepository<T>)repo;
    }
    public IQueryable<T> Query<T>(string? setName = null) where T : class => Repository<T>(setName).Query();
    public Task<List<T>> ListAsync<T>(IQueryable<T> query, CancellationToken ct) => query.ToListAsync(ct);
    public Task<int> CountAsync<T>(IQueryable<T> query, CancellationToken ct) => query.CountAsync(ct);
    public Task<T?> FindAsync<T>(object[] keys, CancellationToken ct, string? setName = null) where T : class => Repository<T>(setName).FindAsync(keys, ct);
    public void Add<T>(T entity, string? setName = null) where T : class => Repository<T>(setName).Add(entity);
    public void Remove<T>(T entity, string? setName = null) where T : class => Repository<T>(setName).Remove(entity);
    public async Task<T> InTransactionAsync<T>(Func<Task<T>> operation, CancellationToken ct)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(ct);
        var result = await operation(); await transaction.CommitAsync(ct); return result;
    }
    public async Task<int> Complete(CancellationToken ct)
    {
        var entries = context.ChangeTracker.Entries().Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted).ToList();
        foreach (var entry in entries)
        {
            if (entry.Entity is Tickets ticket && entry.State == EntityState.Modified)
            { ticket.Version = entry.Property(nameof(Tickets.Version)).OriginalValue is int v ? v + 1 : ticket.Version + 1; ticket.FechaActualizacion = DateTime.UtcNow; }
            if (entry.Entity is Auditoria or HistorialTickets) continue;
            var key = entry.Metadata.FindPrimaryKey();
            var id = key?.Properties.Count == 1 ? entry.Property(key.Properties[0].Name).CurrentValue as Guid? : null;
            context.Auditoria.Add(new Auditoria
            {
                IdAuditoria = Guid.NewGuid(),
                IdUsuario = actor.IsAuthenticated ? actor.Id : null,
                Accion = entry.State.ToString(),
                TablaAfectada = entry.Metadata.GetTableName() ?? entry.Metadata.Name,
                IdRegistro = id,
                Descripcion = "Operacion sobre " + entry.Metadata.ClrType.Name + ". No se registran valores sensibles.",
                Fecha = DateTime.UtcNow
            });
        }
        try { return await context.SaveChangesAsync(ct); }
        catch (DbUpdateConcurrencyException) { throw new BusinessException(409, "El registro fue modificado por otra operacion. Vuelva a consultarlo."); }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pg)
        {
            throw pg.SqlState switch
            {
                "23505" => new BusinessException(409, "Ya existe un registro con estos datos."),
                "23503" => new BusinessException(409, "El registro esta relacionado o una referencia no existe."),
                "23514" or "23502" or "22001" => new BusinessException(400, "Los datos no cumplen las restricciones del recurso."),
                _ => new BusinessException(500, "No se pudo guardar la operacion.")
            };
        }
    }
}
