namespace HelpDesk.Domain.Dtos.ResumenTickets;

public class ResumenTicketsInputDto
{
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
    public decimal? MinutosHastaResolucion
    { get; set; }
    public bool? RespuestaEnPlazo
    { get; set; }
    public bool? ResolucionEnPlazo
    { get; set; }
    public short? Calificacion
    { get; set; }
}
