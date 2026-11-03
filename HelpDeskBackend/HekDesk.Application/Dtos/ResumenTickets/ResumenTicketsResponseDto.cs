namespace HelpDesk.Application.Dtos.ResumenTickets;

public class ResumenTicketsResponseDto
{
    public Guid? IdTicket
    { get; set; }
    public long? Numero
    { get; set; }
    public string? Asunto
    { get; set; }
    public string? Tipo
    { get; set; }
    public string? Estado
    { get; set; }
    public string? Prioridad
    { get; set; }
    public Guid? IdSolicitante
    { get; set; }
    public Guid? IdAreaSolicitante
    { get; set; }
    public Guid? IdEquipoAsignado
    { get; set; }
    public Guid? IdTecnicoAsignado
    { get; set; }
    public DateTime? FechaCreacion
    { get; set; }
    public DateTime? FechaLimiteRespuesta
    { get; set; }
    public DateTime? FechaLimiteResolucion
    { get; set; }
    public DateTime? FechaPrimeraRespuesta
    { get; set; }
    public DateTime? FechaResolucion
    { get; set; }
    public DateTime? FechaCierre
    { get; set; }
    public decimal? MinutosHastaResolucion
    { get; set; }
    public bool? RespuestaEnPlazo
    { get; set; }
    public bool? ResolucionEnPlazo
    { get; set; }
    public short? Calificacion
    { get; set; }
}
