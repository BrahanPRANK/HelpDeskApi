using System.ComponentModel.DataAnnotations;
namespace HelpDesk.Application.UseCases.Permisos.Dtos;
public class PermisosInputDto
{
    [Required]
    [StringLength(10000)]
    public string Codigo { get; set; } = "";
    [Required]
    [StringLength(10000)]
    public string Modulo { get; set; } = "";
    [Required]
    [StringLength(10000)]
    public string Descripcion { get; set; } = "";
}
public class PermisosResponseDto
{
    public Guid IdPermiso { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Modulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
}
