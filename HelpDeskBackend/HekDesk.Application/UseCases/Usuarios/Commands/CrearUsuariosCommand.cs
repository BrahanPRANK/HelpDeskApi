using HelpDesk.Application.Dtos.Common;
using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.Dtos.Usuarios;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Usuarios;

public record CrearUsuariosCommand(UsuariosInputDto Datos) : IRequest<ResponseDto<UsuariosResponseDto>>;

internal sealed class CrearUsuariosCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, IPasswordService passwords, ILogger<CrearUsuariosCommandHandler> _logger) : IRequestHandler<CrearUsuariosCommand, ResponseDto<UsuariosResponseDto>>
{
    public async Task<ResponseDto<UsuariosResponseDto>> Handle(CrearUsuariosCommand request, CancellationToken ct)
    {
        try
        {
            var input = request.Datos;
            Validation.Check(input);
            Validation.Require(access.Admin, "Esta operacion requiere administracion.", 403);
            var entity = new global::HelpDesk.Domain.Models.Usuarios()
            {
                IdUsuario = Guid.NewGuid(),
                IdArea = input.IdArea,
                Nombres = input.Nombres,
                Apellidos = input.Apellidos,
                NombreUsuario = input.NombreUsuario,
                Correo = input.Correo,
                Telefono = input.Telefono,
                Activo = input.Activo,
                FechaRegistro = DateTime.UtcNow
            };
            var userInput = input;
            entity.NombreUsuario = entity.NombreUsuario.Trim();
            entity.Correo = entity.Correo.Trim().ToLowerInvariant();
            Validation.Require(!string.IsNullOrWhiteSpace(userInput.Clave), "Clave es obligatoria al crear un usuario.");
            if (!string.IsNullOrWhiteSpace(userInput.Clave)) entity.ClaveHash = passwords.Hash(userInput.Clave);

            await _unitOfWork.Repository<global::HelpDesk.Domain.Models.Usuarios>().AddAsync(entity);
            await _unitOfWork.Complete();
            return new(201, new UsuariosResponseDto
            {
                IdUsuario = entity.IdUsuario,
                IdArea = entity.IdArea,
                Nombres = entity.Nombres,
                Apellidos = entity.Apellidos,
                NombreUsuario = entity.NombreUsuario,
                Correo = entity.Correo,
                Telefono = entity.Telefono,
                Activo = entity.Activo,
                FechaRegistro = entity.FechaRegistro
            }, "Registro creado.");
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en CrearUsuariosCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error al ejecutar CrearUsuariosCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
