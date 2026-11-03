namespace HelpDesk.Domain.Dtos.ArticulosConocimiento;

public class ArticulosConocimientoResponseDto
{
    public Guid IdArticulo
    { get; set; }
    public Guid IdAutor
    { get; set; }
    public Guid? IdRevisor
    { get; set; }
    public Guid IdCategoriaConocimiento
    { get; set; }
    public string Titulo
    { get; set; } = string.Empty;
    public string Contenido
    { get; set; } = string.Empty;
    public string Estado
    { get; set; } = string.Empty;
    public bool EsInterno
    { get; set; }
    public DateTime FechaCreacion
    { get; set; }
    public DateTime? FechaPublicacion
    { get; set; }
}
