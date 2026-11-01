using System.ComponentModel.DataAnnotations;
namespace HelpDesk.Application.UseCases.EquiposSoporte.Dtos;
public class EquiposSoporteInputDto
{
    [Required]
    [StringLength(10000)]
    public string Nombre { get; set; } = "";
    public string? Descripcion { get; set; }
    public bool Activo { get; set; } = true;
}
public class EquiposSoporteResponseDto
{
    public Guid IdEquipo { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public bool Activo { get; set; }
}
