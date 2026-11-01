using System.ComponentModel.DataAnnotations;
namespace HelpDesk.Application.UseCases.Tickets.Dtos;
public class TicketsInputDto
{
    public Guid IdServicio { get; set; }
    [Required]
    [StringLength(10000)]
    public string Asunto { get; set; } = "";
    [Required]
    [StringLength(10000)]
    public string Descripcion { get; set; } = "";
    [Required]
    [StringLength(10000)]
    public string Tipo { get; set; } = "incidente";
    [Required]
    [StringLength(10000)]
    public string Prioridad { get; set; } = "media";
    public int? Version { get; set; }
}
public class TicketsResponseDto
{
    public Guid IdTicket { get; set; }
    public long Numero { get; set; }
    public Guid IdSolicitante { get; set; }
    public Guid IdCreador { get; set; }
    public Guid IdAreaSolicitante { get; set; }
    public Guid IdServicio { get; set; }
    public Guid IdEquipoAsignado { get; set; }
    public Guid? IdTecnicoAsignado { get; set; }
    public Guid IdPolitica { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Asunto { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string Prioridad { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string? MotivoEspera { get; set; }
    public string? Solucion { get; set; }
    public string? MotivoCancelacion { get; set; }
    public int MinutosPrimeraRespuesta { get; set; }
    public int MinutosResolucion { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime FechaActualizacion { get; set; }
    public DateTime FechaLimiteRespuesta { get; set; }
    public DateTime FechaLimiteResolucion { get; set; }
    public DateTime? FechaPrimeraRespuesta { get; set; }
    public DateTime? FechaResolucion { get; set; }
    public DateTime? FechaCierre { get; set; }
    public DateTime? FechaCancelacion { get; set; }
    public int Version { get; set; }
}
