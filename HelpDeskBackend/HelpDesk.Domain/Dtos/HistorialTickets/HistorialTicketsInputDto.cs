using System.ComponentModel.DataAnnotations;

namespace HelpDesk.Domain.Dtos.HistorialTickets;

public class HistorialTicketsInputDto
{
    public Guid IdTicket
    { get; set; }
    public Guid? IdUsuario
    { get; set; }
    [Required]
    [StringLength(10000)]
    public string Accion
    { get; set; } = "";
    public string? CampoModificado
    { get; set; }
    public string? ValorAnterior
    { get; set; }
    public string? ValorNuevo
    { get; set; }
    public string? Motivo
    { get; set; }
}
