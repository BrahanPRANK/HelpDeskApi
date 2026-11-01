namespace HelpDesk.Application.Common;
public record ResponseDto<T>(int StatusCode, T? Data, string? Mensaje = null);
public record PaginaDto<T>(IReadOnlyList<T> Items, int Pagina, int Tamano, int Total);
public record ListOptions(int Pagina = 1, int Tamano = 20, string? Buscar = null, string? Estado = null, Guid? IdTicket = null);
public record OperacionDto(bool Completado);
