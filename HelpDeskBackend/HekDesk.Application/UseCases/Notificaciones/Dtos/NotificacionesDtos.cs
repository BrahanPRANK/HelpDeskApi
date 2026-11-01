using System.ComponentModel.DataAnnotations;
namespace HelpDesk.Application.UseCases.Notificaciones.Dtos;
public class NotificacionesInputDto
{
    public Guid IdUsuario { get; set; }
    public Guid? IdTicket { get; set; }
    [Required]
    [StringLength(10000)]
    public string Titulo { get; set; } = "";
    [Required]
    [StringLength(10000)]
    public string Mensaje { get; set; } = "";
    [Required]
    [StringLength(10000)]
    public string ClaveEvento { get; set; } = "";
    [Required]
    [StringLength(10000)]
    public string TipoEvento { get; set; } = "";
}
public class NotificacionesResponseDto
{
    public Guid IdNotificacion { get; set; }
    public Guid IdUsuario { get; set; }
    public Guid? IdTicket { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Mensaje { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaLectura { get; set; }
    public string ClaveEvento { get; set; } = string.Empty;
    public string TipoEvento { get; set; } = string.Empty;
}
