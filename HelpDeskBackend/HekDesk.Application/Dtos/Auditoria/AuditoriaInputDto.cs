using System.ComponentModel.DataAnnotations;

namespace HelpDesk.Application.Dtos.Auditoria;

public class AuditoriaInputDto
{
    public Guid? IdUsuario
    { get; set; }
    [Required]
    [StringLength(10000)]
    public string Accion
    { get; set; } = "";
    [Required]
    [StringLength(10000)]
    public string TablaAfectada
    { get; set; } = "";
    public Guid? IdRegistro
    { get; set; }
    [Required]
    [StringLength(10000)]
    public string Descripcion
    { get; set; } = "";
}
