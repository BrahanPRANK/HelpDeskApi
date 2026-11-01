using System.ComponentModel.DataAnnotations;
namespace HelpDesk.Application.Common;
public class AccionDto
{
    [StringLength(4000)] public string Motivo {get;set;}=string.Empty;
    public DateTime? FechaProgramada {get;set;}
}
public record AccionResultadoDto(Guid Id,string Estado);
