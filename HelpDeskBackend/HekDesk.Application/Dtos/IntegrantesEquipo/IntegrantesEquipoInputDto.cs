namespace HelpDesk.Application.Dtos.IntegrantesEquipo;

public class IntegrantesEquipoInputDto
{
    public Guid IdEquipo
    { get; set; }
    public Guid IdUsuario
    { get; set; }
    public bool EsCoordinador
    { get; set; }
    public bool Disponible
    { get; set; } = true;
    public bool Activo
    { get; set; } = true;
}
