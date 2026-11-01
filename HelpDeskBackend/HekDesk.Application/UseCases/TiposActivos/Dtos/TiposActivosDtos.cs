using System.ComponentModel.DataAnnotations;
namespace HelpDesk.Application.UseCases.TiposActivos.Dtos;
public class TiposActivosInputDto
{
    [Required]
    [StringLength(10000)]
    public string Nombre { get; set; } = "";
    public string? Descripcion { get; set; }
    public bool Activo { get; set; } = true;
}
public class TiposActivosResponseDto
{
    public Guid IdTipoActivo { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public bool Activo { get; set; }
}
