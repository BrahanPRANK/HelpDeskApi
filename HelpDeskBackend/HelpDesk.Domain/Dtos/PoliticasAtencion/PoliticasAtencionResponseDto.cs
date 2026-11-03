namespace HelpDesk.Domain.Dtos.PoliticasAtencion;

public class PoliticasAtencionResponseDto
{
    public Guid IdPolitica
    { get; set; }
    public string Prioridad
    { get; set; } = string.Empty;
    public int MinutosPrimeraRespuesta
    { get; set; }
    public int MinutosResolucion
    { get; set; }
    public bool Activo
    { get; set; }
}
