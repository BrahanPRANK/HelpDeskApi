using System.ComponentModel.DataAnnotations;

namespace HelpDesk.Application.Dtos.EquiposSoporte;

public class EquiposSoporteInputDto
{
    [Required]
    [StringLength(10000)]
    public string Nombre
    { get; set; } = "";
    public string? Descripcion
    { get; set; }
    public bool Activo
    { get; set; } = true;
}
