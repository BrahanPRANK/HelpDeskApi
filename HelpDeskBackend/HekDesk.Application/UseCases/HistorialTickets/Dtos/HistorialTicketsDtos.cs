using System.ComponentModel.DataAnnotations;
namespace HelpDesk.Application.UseCases.HistorialTickets.Dtos;
public class HistorialTicketsInputDto
{
    public Guid IdTicket { get; set; }
    public Guid? IdUsuario { get; set; }
    [Required]
    [StringLength(10000)]
    public string Accion { get; set; } = "";
    public string? CampoModificado { get; set; }
    public string? ValorAnterior { get; set; }
    public string? ValorNuevo { get; set; }
    public string? Motivo { get; set; }
}
public class HistorialTicketsResponseDto
{
    public Guid IdHistorial { get; set; }
    public Guid IdTicket { get; set; }
    public Guid? IdUsuario { get; set; }
    public string Accion { get; set; } = string.Empty;
    public string? CampoModificado { get; set; }
    public string? ValorAnterior { get; set; }
    public string? ValorNuevo { get; set; }
    public string? Motivo { get; set; }
    public DateTime Fecha { get; set; }
}
