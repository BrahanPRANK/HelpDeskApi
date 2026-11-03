namespace HelpDesk.Application.Dtos.TiposActivos;

public class TiposActivosResponseDto
{
    public Guid IdTipoActivo
    { get; set; }
    public string Nombre
    { get; set; } = string.Empty;
    public string? Descripcion
    { get; set; }
    public bool Activo
    { get; set; }
}
