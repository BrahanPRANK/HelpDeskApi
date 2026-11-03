namespace HelpDesk.Domain.Dtos.Activos;

public class ActivosResponseDto
{
    public Guid IdActivo
    { get; set; }
    public Guid IdSede
    { get; set; }
    public Guid? IdArea
    { get; set; }
    public string CodigoPatrimonial
    { get; set; } = string.Empty;
    public string Nombre
    { get; set; } = string.Empty;
    public Guid IdTipoActivo
    { get; set; }
    public string? NumeroSerie
    { get; set; }
    public string? Marca
    { get; set; }
    public string? Modelo
    { get; set; }
    public string Estado
    { get; set; } = string.Empty;
    public DateTime FechaRegistro
    { get; set; }
}
