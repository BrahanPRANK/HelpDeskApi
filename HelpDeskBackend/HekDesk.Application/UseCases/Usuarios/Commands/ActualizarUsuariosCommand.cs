using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using HelpDesk.Domain.Dtos.Common;
using Microsoft.EntityFrameworkCore;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.Usuarios;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Usuarios;

public class ActualizarUsuariosCommand : IRequest<ResponseDto<UsuariosResponseDto>>
{
    [JsonIgnore]
    public object[] Keys { get; set; } = [];

    public Guid? IdArea { get; set; }
    [Required]
    [StringLength(10000)]
    public string Nombres { get; set; } = "";
    [Required]
    [StringLength(10000)]
    public string Apellidos { get; set; } = "";
    [Required]
    [StringLength(10000)]
    public string NombreUsuario { get; set; } = "";
    [Required, EmailAddress, StringLength(254)]
    public string Correo { get; set; } = "";
    public string? Telefono { get; set; }
    public bool Activo { get; set; } = true;
    [StringLength(128, MinimumLength = 10)]
    public string? Clave { get; set; }
}

internal sealed class ActualizarUsuariosCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, IPasswordService passwords) : IRequestHandler<ActualizarUsuariosCommand, ResponseDto<UsuariosResponseDto>>
{
    public async Task<ResponseDto<UsuariosResponseDto>> Handle(ActualizarUsuariosCommand request, CancellationToken ct)
    {
        try
        {

            Validation.Check(request);
            Validation.Require(access.Admin, "Esta operacion requiere administracion.", 403);
            var entity = await _unitOfWork.Repository<global::HelpDesk.Domain.Models.Usuarios>().GetByIdAsync(request.Keys)
            ?? throw new BusinessException(404, "Registro no encontrado.");
            access.EnsureRead(entity);
            if (!request.Activo) await EnsureNotLastAdmin(entity.IdUsuario);
            entity.IdArea = request.IdArea;
            entity.Nombres = request.Nombres;
            entity.Apellidos = request.Apellidos;
            entity.NombreUsuario = request.NombreUsuario;
            entity.Correo = request.Correo;
            entity.Telefono = request.Telefono;
            entity.Activo = request.Activo;
            var userInput = request;
            entity.NombreUsuario = entity.NombreUsuario.Trim();
            entity.Correo = entity.Correo.Trim().ToLowerInvariant();

            if (!string.IsNullOrWhiteSpace(userInput.Clave)) entity.ClaveHash = passwords.Hash(userInput.Clave);

            await _unitOfWork.Complete();
            return new ResponseDto<UsuariosResponseDto>(200, new UsuariosResponseDto
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
            }, "Registro actualizado.");
        }
        catch (BusinessException ex)
        {
            return new ResponseDto<UsuariosResponseDto>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<UsuariosResponseDto>(500, default, "No se pudo completar la operacion.");
        }
    }
    private async Task EnsureNotLastAdmin(Guid userId)
    {
        var roleIds = _unitOfWork.Repository<global::HelpDesk.Domain.Models.Roles>().Query().Where(r => r.Nombre == "administrador").Select(r => r.IdRol);
        if (!_unitOfWork.Repository<global::HelpDesk.Domain.Models.UsuariosRoles>().Query().Any(r => r.IdUsuario == userId && roleIds.Contains(r.IdRol))) return;
        var others = _unitOfWork.Repository<global::HelpDesk.Domain.Models.UsuariosRoles>().Query().Where(r => roleIds.Contains(r.IdRol) && r.IdUsuario != userId).Select(r => r.IdUsuario);
        Validation.Require(await (_unitOfWork.Repository<global::HelpDesk.Domain.Models.Usuarios>().Query().Where(u => u.Activo && others.Contains(u.IdUsuario))).CountAsync() > 0,
            "Debe conservarse al menos un administrador activo.", 409);
    }

}
