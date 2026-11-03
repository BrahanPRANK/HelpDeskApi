using HelpDesk.Domain.Ports;
using Microsoft.AspNetCore.Identity;

namespace HelpDesk.Infrastructure.Services;

public class PasswordService : IPasswordService
{
    private readonly PasswordHasher<object> _hasher = new();
    private readonly object _user = new();

    public string Hash(string password)
    {
        return _hasher.HashPassword(_user, password);
    }

    public bool Verify(string hash, string password)
    {
        try
        {
            return _hasher.VerifyHashedPassword(_user, hash, password)
                != PasswordVerificationResult.Failed;
        }
        catch (FormatException)
        {
            return false;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }
}
