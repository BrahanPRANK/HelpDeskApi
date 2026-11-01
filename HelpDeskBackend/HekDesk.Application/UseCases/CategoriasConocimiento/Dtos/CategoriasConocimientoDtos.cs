using System.ComponentModel.DataAnnotations;
namespace HelpDesk.Application.UseCases.CategoriasConocimiento.Dtos;
public class CategoriasConocimientoInputDto
{
    [Required]
    [StringLength(10000)]
    public string Nombre { get; set; } = "";
    public string? Descripcion { get; set; }
    public bool Activo { get; set; } = true;
}
public class CategoriasConocimientoResponseDto
{
    public Guid IdCategoriaConocimiento { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public bool Activo { get; set; }
}
