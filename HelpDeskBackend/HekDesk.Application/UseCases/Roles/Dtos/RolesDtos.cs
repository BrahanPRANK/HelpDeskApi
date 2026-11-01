using System.ComponentModel.DataAnnotations;
namespace HelpDesk.Application.UseCases.Roles.Dtos;
public class RolesInputDto
{
    [Required]
    [StringLength(10000)]
    public string Nombre { get; set; } = "";
    public string? Descripcion { get; set; }
}
public class RolesResponseDto
{
    public Guid IdRol { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
}
