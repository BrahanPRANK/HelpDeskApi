using System.ComponentModel.DataAnnotations;

namespace HelpDesk.Application.Dtos.Tickets;

public class TicketsInputDto
{
    public Guid IdServicio
    { get; set; }
    [Required]
    [StringLength(10000)]
    public string Asunto
    { get; set; } = "";
    [Required]
    [StringLength(10000)]
    public string Descripcion
    { get; set; } = "";
    [Required]
    [StringLength(10000)]
    public string Tipo
    { get; set; } = "incidente";
    [Required]
    [StringLength(10000)]
    public string Prioridad
    { get; set; } = "media";
    public int? Version
    { get; set; }
}
