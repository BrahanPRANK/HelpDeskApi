using System.ComponentModel.DataAnnotations;

namespace HelpDesk.Application.Dtos.Notificaciones;

public class NotificacionesInputDto
{
    public Guid IdUsuario
    { get; set; }
    public Guid? IdTicket
    { get; set; }
    [Required]
    [StringLength(10000)]
    public string Titulo
    { get; set; } = "";
    [Required]
    [StringLength(10000)]
    public string Mensaje
    { get; set; } = "";
    [Required]
    [StringLength(10000)]
    public string ClaveEvento
    { get; set; } = "";
    [Required]
    [StringLength(10000)]
    public string TipoEvento
    { get; set; } = "";
}
