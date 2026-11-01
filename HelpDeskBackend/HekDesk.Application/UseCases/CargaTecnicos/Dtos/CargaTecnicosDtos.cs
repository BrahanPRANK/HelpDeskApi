using System.ComponentModel.DataAnnotations;
namespace HelpDesk.Application.UseCases.CargaTecnicos.Dtos;
public class CargaTecnicosInputDto
{
    public Guid? IdTecnicoAsignado { get; set; }
    public string? Prioridad { get; set; }
    public long? TicketsPendientes { get; set; }
}
public class CargaTecnicosResponseDto
{
    public Guid? IdEquipoAsignado { get; set; }
    public Guid? IdTecnicoAsignado { get; set; }
    public string? Prioridad { get; set; }
    public long? TicketsPendientes { get; set; }
}
