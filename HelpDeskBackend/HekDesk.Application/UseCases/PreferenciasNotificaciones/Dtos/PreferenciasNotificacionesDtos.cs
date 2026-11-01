using System.ComponentModel.DataAnnotations;
namespace HelpDesk.Application.UseCases.PreferenciasNotificaciones.Dtos;
public class PreferenciasNotificacionesInputDto
{
    public Guid IdUsuario { get; set; }
    [Required]
    [StringLength(10000)]
    public string TipoEvento { get; set; } = "";
    public bool RecibirEnSistema { get; set; }
    public bool RecibirPorCorreo { get; set; }
}
public class PreferenciasNotificacionesResponseDto
{
    public Guid IdUsuario { get; set; }
    public string TipoEvento { get; set; } = string.Empty;
    public bool RecibirEnSistema { get; set; }
    public bool RecibirPorCorreo { get; set; }
}
