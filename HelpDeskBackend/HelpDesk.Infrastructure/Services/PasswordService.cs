using HelpDesk.Domain.Ports;
using Microsoft.AspNetCore.Identity;
namespace HelpDesk.Infrastructure.Services;
public sealed class PasswordService:IPasswordService
{
    private readonly PasswordHasher<object> hasher=new();
    private readonly object context=new();
    public string Hash(string password)=>hasher.HashPassword(context,password);
    public bool Verify(string hash,string password)
    {
        try{return hasher.VerifyHashedPassword(context,hash,password)!=PasswordVerificationResult.Failed;}
        catch(FormatException){return false;}
        catch(ArgumentException){return false;}
    }
}
