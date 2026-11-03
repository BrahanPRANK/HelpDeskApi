using System.ComponentModel.DataAnnotations;

namespace HelpDesk.Domain.Dtos.EnviosNotificaciones;

public class EnviosNotificacionesInputDto
{
    public Guid IdNotificacion
    { get; set; }
    [Required]
    [StringLength(10000)]
    public string CorreoDestino
    { get; set; } = "";
    [Required]
    [StringLength(10000)]
    public string Estado
    { get; set; } = "";
    public int Intentos
    { get; set; }
    public int MaximoIntentos
    { get; set; }
    public DateTime FechaProximoIntento
    { get; set; }
    public DateTime? BloqueoHasta
    { get; set; }
    public string? IdentificadorProveedor
    { get; set; }
}
