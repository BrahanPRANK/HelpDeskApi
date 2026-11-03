namespace HelpDesk.Application.Dtos.Common;

public record ListOptions(int Pagina = 1, int Tamano = 20, string? Buscar = null, string? Estado = null, Guid? IdTicket = null);
