using System.ComponentModel.DataAnnotations;

namespace HelpDesk.Application.Dtos.Roles;

public class RolesInputDto
{
    [Required]
    [StringLength(10000)]
    public string Nombre
    { get; set; } = "";
    public string? Descripcion
    { get; set; }
}
