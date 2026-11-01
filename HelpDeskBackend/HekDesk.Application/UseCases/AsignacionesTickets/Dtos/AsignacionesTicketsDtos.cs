using System.ComponentModel.DataAnnotations;
namespace HelpDesk.Application.UseCases.AsignacionesTickets.Dtos;
public class AsignacionesTicketsInputDto
{
    public Guid IdTicket { get; set; }
    public Guid IdEquipo { get; set; }
    public Guid? IdTecnico { get; set; }
    [Required]
    [StringLength(10000)]
    public string Motivo { get; set; } = "";
}
public class AsignacionesTicketsResponseDto
{
    public Guid IdAsignacionTicket { get; set; }
    public Guid IdTicket { get; set; }
    public Guid IdEquipo { get; set; }
    public Guid? IdTecnico { get; set; }
    public Guid IdAsignador { get; set; }
    public DateTime FechaAsignacion { get; set; }
    public DateTime? FechaFin { get; set; }
    public string Motivo { get; set; } = string.Empty;
}
