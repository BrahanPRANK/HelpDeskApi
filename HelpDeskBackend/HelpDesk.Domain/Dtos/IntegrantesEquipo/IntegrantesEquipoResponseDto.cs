namespace HelpDesk.Domain.Dtos.IntegrantesEquipo;

public class IntegrantesEquipoResponseDto
{
    public Guid IdEquipo
    { get; set; }
    public Guid IdUsuario
    { get; set; }
    public bool EsCoordinador
    { get; set; }
    public bool Disponible
    { get; set; }
    public bool Activo
    { get; set; }
}
