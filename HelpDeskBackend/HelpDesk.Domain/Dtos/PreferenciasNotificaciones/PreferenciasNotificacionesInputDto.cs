using System.ComponentModel.DataAnnotations;

namespace HelpDesk.Domain.Dtos.PreferenciasNotificaciones;

public class PreferenciasNotificacionesInputDto
{
    public Guid IdUsuario
    { get; set; }
    [Required]
    [StringLength(10000)]
    public string TipoEvento
    { get; set; } = "";
    public bool RecibirEnSistema
    { get; set; }
    public bool RecibirPorCorreo
    { get; set; }
}
