using System.ComponentModel.DataAnnotations;
namespace HelpDesk.Application.UseCases.Aprobaciones.Dtos;
public class AprobacionesInputDto
{
    public Guid? IdTicket { get; set; }
    public Guid? IdCambio { get; set; }
    public Guid IdAprobador { get; set; }
    public short Orden { get; set; } = 1;
    [Required]
    [StringLength(10000)]
    public string Motivo { get; set; } = "";
    public string? ComentarioDecision { get; set; }
}
public class AprobacionesResponseDto
{
    public Guid IdAprobacion { get; set; }
    public Guid? IdTicket { get; set; }
    public Guid? IdCambio { get; set; }
    public Guid IdSolicitante { get; set; }
    public Guid IdAprobador { get; set; }
    public short Orden { get; set; }
    public string Motivo { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string? ComentarioDecision { get; set; }
    public DateTime FechaSolicitud { get; set; }
    public DateTime? FechaDecision { get; set; }
}
