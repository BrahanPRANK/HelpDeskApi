namespace HelpDesk.Application.Dtos.EnviosNotificaciones;

public class EnviosNotificacionesResponseDto
{
    public Guid IdEnvio
    { get; set; }
    public Guid IdNotificacion
    { get; set; }
    public string CorreoDestino
    { get; set; } = string.Empty;
    public string Estado
    { get; set; } = string.Empty;
    public int Intentos
    { get; set; }
    public int MaximoIntentos
    { get; set; }
    public DateTime FechaCreacion
    { get; set; }
    public DateTime FechaProximoIntento
    { get; set; }
    public DateTime? BloqueoHasta
    { get; set; }
    public DateTime? FechaEnvio
    { get; set; }
    public string? IdentificadorProveedor
    { get; set; }
}
