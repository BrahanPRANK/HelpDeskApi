namespace HelpDesk.Application.Dtos.Sedes;

public class SedesResponseDto
{
    public Guid IdSede
    { get; set; }
    public string Nombre
    { get; set; } = string.Empty;
    public string? Direccion
    { get; set; }
    public bool Activo
    { get; set; }
}
