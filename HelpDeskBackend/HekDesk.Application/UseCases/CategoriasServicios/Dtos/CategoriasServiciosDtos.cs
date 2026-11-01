using System.ComponentModel.DataAnnotations;
namespace HelpDesk.Application.UseCases.CategoriasServicios.Dtos;
public class CategoriasServiciosInputDto
{
    [Required]
    [StringLength(10000)]
    public string Nombre { get; set; } = "";
    public string? Descripcion { get; set; }
    public bool Activo { get; set; } = true;
}
public class CategoriasServiciosResponseDto
{
    public Guid IdCategoriaServicio { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public bool Activo { get; set; }
}
