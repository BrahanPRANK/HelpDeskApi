using System.ComponentModel.DataAnnotations;

namespace HelpDesk.Application.Dtos.Servicios;

public class ServiciosInputDto
{
    public Guid IdEquipoResponsable
    { get; set; }
    [Required]
    [StringLength(10000)]
    public string Nombre
    { get; set; } = "";
    public Guid IdCategoriaServicio
    { get; set; }
    [Required]
    [StringLength(10000)]
    public string Descripcion
    { get; set; } = "";
    public bool RequiereAprobacion
    { get; set; }
    public bool Activo
    { get; set; } = true;
}
