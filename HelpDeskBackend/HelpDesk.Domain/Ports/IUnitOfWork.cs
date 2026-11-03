namespace HelpDesk.Domain.Ports;

public interface IUnitOfWork
{
    IGenericRepository<T> Repository<T>(string? setName = null) where T : class;
    Task<int> Complete();
}
