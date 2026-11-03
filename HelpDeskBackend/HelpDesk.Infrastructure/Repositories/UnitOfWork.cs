using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Ports;
using HelpDesk.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace HelpDesk.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly HelpDeskDbContext _context;
    private readonly Dictionary<(Type, string?), object> _repositories = new();

    public UnitOfWork(HelpDeskDbContext context)
    {
        _context = context;
    }

    public IGenericRepository<T> Repository<T>(string? setName = null) where T : class
    {
        var key = (typeof(T), setName);
        if (!_repositories.TryGetValue(key, out var repository))
        {
            repository = new GenericRepository<T>(_context, setName);
            _repositories.Add(key, repository);
        }
        return (IGenericRepository<T>)repository;
    }

    public async Task<int> Complete()
    {
        try
        {
            // EF guarda los cambios de esta llamada en una transaccion automaticamente.
            return await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new BusinessException(409, "El registro cambio. Vuelva a consultarlo.");
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: "23505" or "23503" })
        {
            throw new BusinessException(409, "Existe un registro duplicado o una relacion impide guardar los cambios.");
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: "23514" or "23502" or "22001" })
        {
            throw new BusinessException(400, "Los datos no cumplen las restricciones de la base de datos.");
        }
    }
}
