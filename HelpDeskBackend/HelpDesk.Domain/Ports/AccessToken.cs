namespace HelpDesk.Domain.Ports;

public record AccessToken(string Token, DateTime ExpiresAt);
