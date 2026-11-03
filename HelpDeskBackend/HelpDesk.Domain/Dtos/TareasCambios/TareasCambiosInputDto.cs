using System.ComponentModel.DataAnnotations;

namespace HelpDesk.Domain.Dtos.TareasCambios;

public class TareasCambiosInputDto
{
    public Guid IdCambio
    { get; set; }
    public Guid? IdResponsable
    { get; set; }
    public short Orden
    { get; set; } = 1;
    [Required]
    [StringLength(10000)]
    public string Titulo
    { get; set; } = "";
    public string? Descripcion
    { get; set; }
    [Required]
    [StringLength(10000)]
    public string Estado
    { get; set; } = "pendiente";
}
