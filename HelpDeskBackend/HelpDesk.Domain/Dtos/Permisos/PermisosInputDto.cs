using System.ComponentModel.DataAnnotations;

namespace HelpDesk.Domain.Dtos.Permisos;

public class PermisosInputDto
{
    [Required]
    [StringLength(10000)]
    public string Codigo
    { get; set; } = "";
    [Required]
    [StringLength(10000)]
    public string Modulo
    { get; set; } = "";
    [Required]
    [StringLength(10000)]
    public string Descripcion
    { get; set; } = "";
}
