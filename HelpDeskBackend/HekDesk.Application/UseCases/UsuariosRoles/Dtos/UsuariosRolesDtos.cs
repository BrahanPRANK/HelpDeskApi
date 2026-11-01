using System.ComponentModel.DataAnnotations;
namespace HelpDesk.Application.UseCases.UsuariosRoles.Dtos;
public class UsuariosRolesInputDto
{
    public Guid IdUsuario { get; set; }
    public Guid IdRol { get; set; }
}
public class UsuariosRolesResponseDto
{
    public Guid IdUsuario { get; set; }
    public Guid IdRol { get; set; }
    public DateTime FechaAsignacion { get; set; }
}
