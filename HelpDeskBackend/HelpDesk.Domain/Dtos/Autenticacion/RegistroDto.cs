using System.ComponentModel.DataAnnotations;

namespace HelpDesk.Domain.Dtos.Autenticacion;

public class RegistroDto
{
    [Required, StringLength(100)]
    public string Nombres
    { get; set; } = string.Empty;
    [Required, StringLength(150)]
    public string Apellidos
    { get; set; } = string.Empty;
    [Required, StringLength(100)]
    public string NombreUsuario
    { get; set; } = string.Empty;
    [Required, EmailAddress, StringLength(254)]
    public string Correo
    { get; set; } = string.Empty;
    [Required, StringLength(128, MinimumLength = 10)]
    public string Clave
    { get; set; } = string.Empty;
}
