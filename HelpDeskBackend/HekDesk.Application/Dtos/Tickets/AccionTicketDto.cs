using System.ComponentModel.DataAnnotations;

namespace HelpDesk.Application.Dtos.Tickets;

public class AccionTicketDto
{
    [Range(1, int.MaxValue)]
    public int Version
    { get; set; }
    [Required, StringLength(4000)]
    public string Motivo
    { get; set; } = string.Empty;
    public Guid? IdEquipo
    { get; set; }
    public Guid? IdTecnico
    { get; set; }
}
