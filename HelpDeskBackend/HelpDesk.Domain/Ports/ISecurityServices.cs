namespace HelpDesk.Domain.Ports;

public interface ICurrentUser
{
    Guid Id { get; }
    bool IsAuthenticated { get; }
    bool IsInRole(string role);
    bool HasPermission(string permission);
}
public interface IPasswordService
{
    string Hash(string password);
    bool Verify(string hash, string password);
}
public record AccessToken(string Token, DateTime ExpiresAt);
public interface ITokenService
{
    AccessToken Create(Guid userId, string username, IEnumerable<string> roles, string credentialStamp);
}
