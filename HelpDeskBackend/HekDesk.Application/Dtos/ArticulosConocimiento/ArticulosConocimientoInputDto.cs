using System.ComponentModel.DataAnnotations;

namespace HelpDesk.Application.Dtos.ArticulosConocimiento;

public class ArticulosConocimientoInputDto
{
    public Guid IdCategoriaConocimiento
    { get; set; }
    [Required]
    [StringLength(10000)]
    public string Titulo
    { get; set; } = "";
    [Required]
    [StringLength(10000)]
    public string Contenido
    { get; set; } = "";
    public bool EsInterno
    { get; set; }
}
