using System.ComponentModel.DataAnnotations;

namespace HelpDesk.Application.Dtos.Autenticacion;

public class LoginDto
{
    [Required, StringLength(100)]
    public string NombreUsuario
    { get; set; } = string.Empty;
    [Required, StringLength(128)]
    public string Clave
    { get; set; } = string.Empty;
}
