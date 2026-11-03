namespace HelpDesk.Domain.Dtos.Common;

public record PaginaDto<T>(IReadOnlyList<T> Items, int Pagina, int Tamano, int Total);
