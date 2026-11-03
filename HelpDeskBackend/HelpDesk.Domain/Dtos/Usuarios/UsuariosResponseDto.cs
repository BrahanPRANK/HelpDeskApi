namespace HelpDesk.Domain.Dtos.Usuarios;

public class UsuariosResponseDto
{
    public Guid IdUsuario
    { get; set; }
    public Guid? IdArea
    { get; set; }
    public string Nombres
    { get; set; } = string.Empty;
    public string Apellidos
    { get; set; } = string.Empty;
    public string NombreUsuario
    { get; set; } = string.Empty;
    public string Correo
    { get; set; } = string.Empty;
    public string? Telefono
    { get; set; }
    public bool Activo
    { get; set; }
    public DateTime FechaRegistro
    { get; set; }
}
