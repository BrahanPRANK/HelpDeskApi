using System.ComponentModel.DataAnnotations;
namespace HelpDesk.Application.UseCases.TareasCambios.Dtos;
public class TareasCambiosInputDto
{
    public Guid IdCambio { get; set; }
    public Guid? IdResponsable { get; set; }
    public short Orden { get; set; } = 1;
    [Required]
    [StringLength(10000)]
    public string Titulo { get; set; } = "";
    public string? Descripcion { get; set; }
    [Required]
    [StringLength(10000)]
    public string Estado { get; set; } = "pendiente";
}
public class TareasCambiosResponseDto
{
    public Guid IdTareaCambio { get; set; }
    public Guid IdCambio { get; set; }
    public Guid? IdResponsable { get; set; }
    public short Orden { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string Estado { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaFinalizacion { get; set; }
}
