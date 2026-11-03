namespace HelpDesk.Domain.Dtos.Permisos;

public class PermisosResponseDto
{
    public Guid IdPermiso
    { get; set; }
    public string Codigo
    { get; set; } = string.Empty;
    public string Modulo
    { get; set; } = string.Empty;
    public string Descripcion
    { get; set; } = string.Empty;
}
