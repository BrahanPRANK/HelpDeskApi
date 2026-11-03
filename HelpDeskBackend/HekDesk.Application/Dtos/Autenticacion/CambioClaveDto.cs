using System.ComponentModel.DataAnnotations;

namespace HelpDesk.Application.Dtos.Autenticacion;

public class CambioClaveDto
{
    [Required]
    public string ClaveActual
    { get; set; } = string.Empty;
    [Required, StringLength(128, MinimumLength = 10)]
    public string ClaveNueva
    { get; set; } = string.Empty;
}
