namespace HelpDesk.Domain.Exceptions;
public sealed class BusinessException(int status, string message) : Exception(message)
{
    public int Status { get; } = status;
}
