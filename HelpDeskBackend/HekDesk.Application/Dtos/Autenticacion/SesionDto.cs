namespace HelpDesk.Application.Dtos.Autenticacion;

public record SesionDto(string Token, DateTime FechaVencimiento, Guid IdUsuario, string NombreUsuario, IReadOnlyList<string> Roles);
