namespace HelpDesk.Domain.Dtos.CargaTecnicos;

public class CargaTecnicosResponseDto
{
    public Guid? IdEquipoAsignado
    { get; set; }
    public Guid? IdTecnicoAsignado
    { get; set; }
    public string? Prioridad
    { get; set; }
    public long? TicketsPendientes
    { get; set; }
}
