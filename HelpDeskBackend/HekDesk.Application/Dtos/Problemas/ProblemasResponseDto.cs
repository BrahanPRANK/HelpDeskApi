namespace HelpDesk.Application.Dtos.Problemas;

public class ProblemasResponseDto
{
    public Guid IdProblema
    { get; set; }
    public Guid IdResponsable
    { get; set; }
    public string Titulo
    { get; set; } = string.Empty;
    public string Descripcion
    { get; set; } = string.Empty;
    public string Estado
    { get; set; } = string.Empty;
    public string? CausaRaiz
    { get; set; }
    public string? SolucionTemporal
    { get; set; }
    public string? SolucionDefinitiva
    { get; set; }
    public DateTime FechaCreacion
    { get; set; }
}
