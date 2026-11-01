using System.ComponentModel.DataAnnotations;
namespace HelpDesk.Application.UseCases.Auditoria.Dtos;
public class AuditoriaInputDto
{
    public Guid? IdUsuario { get; set; }
    [Required]
    [StringLength(10000)]
    public string Accion { get; set; } = "";
    [Required]
    [StringLength(10000)]
    public string TablaAfectada { get; set; } = "";
    public Guid? IdRegistro { get; set; }
    [Required]
    [StringLength(10000)]
    public string Descripcion { get; set; } = "";
}
public class AuditoriaResponseDto
{
    public Guid IdAuditoria { get; set; }
    public Guid? IdUsuario { get; set; }
    public string Accion { get; set; } = string.Empty;
    public string TablaAfectada { get; set; } = string.Empty;
    public Guid? IdRegistro { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
}
