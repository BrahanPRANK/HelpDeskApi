namespace HelpDesk.Application.Dtos.Areas;

public class AreasResponseDto
{
    public Guid IdArea
    { get; set; }
    public Guid IdSede
    { get; set; }
    public string Nombre
    { get; set; } = string.Empty;
    public bool Activo
    { get; set; }
}
