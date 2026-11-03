using System.ComponentModel.DataAnnotations;

namespace HelpDesk.Domain.Dtos.Activos;

public class ActivosInputDto
{
    public Guid IdSede
    { get; set; }
    public Guid? IdArea
    { get; set; }
    [Required]
    [StringLength(10000)]
    public string CodigoPatrimonial
    { get; set; } = "";
    [Required]
    [StringLength(10000)]
    public string Nombre
    { get; set; } = "";
    public Guid IdTipoActivo
    { get; set; }
    public string? NumeroSerie
    { get; set; }
    public string? Marca
    { get; set; }
    public string? Modelo
    { get; set; }
    [Required]
    [StringLength(10000)]
    public string Estado
    { get; set; } = "disponible";
}
