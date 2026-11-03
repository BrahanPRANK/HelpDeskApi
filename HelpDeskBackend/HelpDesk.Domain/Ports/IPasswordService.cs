namespace HelpDesk.Domain.Ports;

public interface IPasswordService
{
    string Hash(string password);
    bool Verify(string hash, string password);
}
