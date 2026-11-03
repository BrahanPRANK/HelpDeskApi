namespace HelpDesk.Application.Dtos.CargaTecnicos;

public class CargaTecnicosInputDto
{
    public Guid? IdTecnicoAsignado
    { get; set; }
    public string? Prioridad
    { get; set; }
    public long? TicketsPendientes
    { get; set; }
}
