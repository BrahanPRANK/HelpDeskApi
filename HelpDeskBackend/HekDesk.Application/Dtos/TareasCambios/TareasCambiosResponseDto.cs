namespace HelpDesk.Application.Dtos.TareasCambios;

public class TareasCambiosResponseDto
{
    public Guid IdTareaCambio
    { get; set; }
    public Guid IdCambio
    { get; set; }
    public Guid? IdResponsable
    { get; set; }
    public short Orden
    { get; set; }
    public string Titulo
    { get; set; } = string.Empty;
    public string? Descripcion
    { get; set; }
    public string Estado
    { get; set; } = string.Empty;
    public DateTime FechaCreacion
    { get; set; }
    public DateTime? FechaFinalizacion
    { get; set; }
}
