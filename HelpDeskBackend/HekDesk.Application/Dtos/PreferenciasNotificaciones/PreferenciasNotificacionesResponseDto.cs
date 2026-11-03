namespace HelpDesk.Application.Dtos.PreferenciasNotificaciones;

public class PreferenciasNotificacionesResponseDto
{
    public Guid IdUsuario
    { get; set; }
    public string TipoEvento
    { get; set; } = string.Empty;
    public bool RecibirEnSistema
    { get; set; }
    public bool RecibirPorCorreo
    { get; set; }
}
