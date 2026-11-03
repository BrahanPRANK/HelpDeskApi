namespace HelpDesk.Domain.Ports;

public interface ICurrentUser
{
    Guid Id { get; }
    bool IsAuthenticated { get; }
    bool IsInRole(string role);
    bool HasPermission(string permission);
}
