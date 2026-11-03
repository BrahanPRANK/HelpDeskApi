using System.ComponentModel.DataAnnotations;

namespace HelpDesk.Domain.Dtos.Usuarios;

public class UsuariosInputDto
{
    public Guid? IdArea
    { get; set; }
    [Required]
    [StringLength(10000)]
    public string Nombres
    { get; set; } = "";
    [Required]
    [StringLength(10000)]
    public string Apellidos
    { get; set; } = "";
    [Required]
    [StringLength(10000)]
    public string NombreUsuario
    { get; set; } = "";
    [Required, EmailAddress, StringLength(254)]
    public string Correo
    { get; set; } = "";
    public string? Telefono
    { get; set; }
    public bool Activo
    { get; set; } = true;
    [StringLength(128, MinimumLength = 10)]
    public string? Clave
    { get; set; }
}
