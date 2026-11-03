using System.ComponentModel.DataAnnotations;

namespace HelpDesk.Application.Dtos.CategoriasServicios;

public class CategoriasServiciosInputDto
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
