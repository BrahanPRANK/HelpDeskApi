using HelpDesk.Domain.Dtos.Common;
using HelpDesk.Application.Common;
using HelpDesk.Domain.Exceptions;
namespace HelpDesk.Api.Middleware;

public sealed class ExceptionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try { await next(context); }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested) { }
        catch (Exception ex)
        {
            if (context.Response.HasStarted) throw;
            var status = ex is BusinessException business ? business.Status : 500;
            var message = status < 500 ? ex.Message : "Ocurrio un error al procesar la solicitud.";
            context.Response.StatusCode = status;
            await context.Response.WriteAsJsonAsync(new ResponseDto<object>(status, null, message));
        }
    }
}
