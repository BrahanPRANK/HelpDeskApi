namespace HelpDesk.Domain.Ports;

public interface ITokenService
{
    AccessToken Create(Guid userId, string username, IEnumerable<string> roles, string credentialStamp);
}
