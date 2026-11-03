namespace HelpDesk.Application.Dtos.Common;

public record ResponseDto<T>(int StatusCode, T? Data, string? Mensaje = null);
