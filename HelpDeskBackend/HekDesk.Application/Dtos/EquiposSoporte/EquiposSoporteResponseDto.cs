namespace HelpDesk.Application.Dtos.EquiposSoporte;

public class EquiposSoporteResponseDto
{
    public Guid IdEquipo
    { get; set; }
    public string Nombre
    { get; set; } = string.Empty;
    public string? Descripcion
    { get; set; }
    public bool Activo
    { get; set; }
}
