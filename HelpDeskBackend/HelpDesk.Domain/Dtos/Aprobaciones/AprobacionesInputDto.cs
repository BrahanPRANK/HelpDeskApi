using System.ComponentModel.DataAnnotations;

namespace HelpDesk.Domain.Dtos.Aprobaciones;

public class AprobacionesInputDto
{
    public Guid? IdTicket
    { get; set; }
    public Guid? IdCambio
    { get; set; }
    public Guid IdAprobador
    { get; set; }
    public short Orden
    { get; set; } = 1;
    [Required]
    [StringLength(10000)]
    public string Motivo
    { get; set; } = "";
    public string? ComentarioDecision
    { get; set; }
}
