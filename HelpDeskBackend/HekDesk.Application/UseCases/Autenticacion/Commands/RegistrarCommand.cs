using System.ComponentModel.DataAnnotations;
using HelpDesk.Domain.Dtos.Common;
using HelpDesk.Domain.Dtos.Autenticacion;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.Security;

public class RegistrarCommand : IRequest<ResponseDto<UsuarioRegistradoDto>>
{
    [Required, StringLength(100)]
    public string Nombres { get; set; } = string.Empty;
    [Required, StringLength(150)]
    public string Apellidos { get; set; } = string.Empty;
    [Required, StringLength(100)]
    public string NombreUsuario { get; set; } = string.Empty;
    [Required, EmailAddress, StringLength(254)]
    public string Correo { get; set; } = string.Empty;
    [Required, StringLength(128, MinimumLength = 10)]
    public string Clave { get; set; } = string.Empty;
}
internal sealed class RegistrarCommandHandler(IUnitOfWork _unitOfWork, IPasswordService passwords, ILogger<RegistrarCommandHandler> _logger) : IRequestHandler<RegistrarCommand, ResponseDto<UsuarioRegistradoDto>>
{
    public async Task<ResponseDto<UsuarioRegistradoDto>> Handle(RegistrarCommand request, CancellationToken ct)
    {
        try
        {


            Validation.Check(request);
            var employee = (await (_unitOfWork.Repository<Roles>().Query().Where(x => x.Nombre == "empleado")).ToListAsync()).SingleOrDefault()
            ?? throw new BusinessException(409, "Falta el rol empleado del catalogo inicial.");
            var user = MakeUser(request);
            await _unitOfWork.Repository<global::HelpDesk.Domain.Models.Usuarios>().AddAsync(user);
            await _unitOfWork.Repository<UsuariosRoles>().AddAsync(new UsuariosRoles
            {
                IdUsuario = user.IdUsuario,
                IdRol = employee.IdRol,
                FechaAsignacion = DateTime.UtcNow
            });
            await _unitOfWork.Complete();
            return new(201, new(user.IdUsuario, user.NombreUsuario), "Cuenta creada con rol empleado.");
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en RegistrarCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error al ejecutar RegistrarCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }
    private Usuarios MakeUser(RegistrarCommand x) => new()
    {
        IdUsuario = Guid.NewGuid(),
        Nombres = x.Nombres.Trim(),
        Apellidos = x.Apellidos.Trim(),
        NombreUsuario = x.NombreUsuario.Trim(),
        Correo = x.Correo.Trim().ToLowerInvariant(),
        ClaveHash = passwords.Hash(x.Clave),
        Activo = true,
        FechaRegistro = DateTime.UtcNow
    };

}
