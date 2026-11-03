namespace HelpDesk.Domain.Dtos.Servicios;

public class ServiciosResponseDto
{
    public Guid IdServicio
    { get; set; }
    public Guid IdEquipoResponsable
    { get; set; }
    public string Nombre
    { get; set; } = string.Empty;
    public Guid IdCategoriaServicio
    { get; set; }
    public string Descripcion
    { get; set; } = string.Empty;
    public bool RequiereAprobacion
    { get; set; }
    public bool Activo
    { get; set; }
}
