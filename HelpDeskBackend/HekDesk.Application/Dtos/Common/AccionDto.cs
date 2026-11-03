using System.ComponentModel.DataAnnotations;

namespace HelpDesk.Application.Dtos.Common;

public class AccionDto
{
    [StringLength(4000)]
    public string Motivo
    { get; set; } = string.Empty;
}
