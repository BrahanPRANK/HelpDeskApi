using System.ComponentModel.DataAnnotations;
namespace HelpDesk.Application.UseCases.Usuarios.Dtos;
public class UsuariosInputDto
{
    public Guid? IdArea { get; set; }
    [Required]
    [StringLength(10000)]
    public string Nombres { get; set; } = "";
    [Required]
    [StringLength(10000)]
    public string Apellidos { get; set; } = "";
    [Required]
    [StringLength(10000)]
    public string NombreUsuario { get; set; } = "";
    [Required, EmailAddress, StringLength(254)]
    public string Correo { get; set; } = "";
    public string? Telefono { get; set; }
    public bool Activo { get; set; } = true;
    [StringLength(128, MinimumLength = 10)]
    public string? Clave { get; set; }
}
public class UsuariosResponseDto
{
    public Guid IdUsuario { get; set; }
    public Guid? IdArea { get; set; }
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string NombreUsuario { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public bool Activo { get; set; }
    public DateTime FechaRegistro { get; set; }
}
