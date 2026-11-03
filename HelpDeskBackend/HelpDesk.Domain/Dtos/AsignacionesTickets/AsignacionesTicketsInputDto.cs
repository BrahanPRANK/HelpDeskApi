using System.ComponentModel.DataAnnotations;

namespace HelpDesk.Domain.Dtos.AsignacionesTickets;

public class AsignacionesTicketsInputDto
{
    public Guid IdTicket
    { get; set; }
    public Guid IdEquipo
    { get; set; }
    public Guid? IdTecnico
    { get; set; }
    [Required]
    [StringLength(10000)]
    public string Motivo
    { get; set; } = "";
}
