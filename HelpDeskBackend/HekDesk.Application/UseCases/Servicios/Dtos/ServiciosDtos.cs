using System.ComponentModel.DataAnnotations;
namespace HelpDesk.Application.UseCases.Servicios.Dtos;
public class ServiciosInputDto
{
    public Guid IdEquipoResponsable { get; set; }
    [Required]
    [StringLength(10000)]
    public string Nombre { get; set; } = "";
    public Guid IdCategoriaServicio { get; set; }
    [Required]
    [StringLength(10000)]
    public string Descripcion { get; set; } = "";
    public bool RequiereAprobacion { get; set; }
    public bool Activo { get; set; } = true;
}
public class ServiciosResponseDto
{
    public Guid IdServicio { get; set; }
    public Guid IdEquipoResponsable { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public Guid IdCategoriaServicio { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public bool RequiereAprobacion { get; set; }
    public bool Activo { get; set; }
}
