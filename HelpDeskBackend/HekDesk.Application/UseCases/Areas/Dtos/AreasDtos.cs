using System.ComponentModel.DataAnnotations;
namespace HelpDesk.Application.UseCases.Areas.Dtos;
public class AreasInputDto
{
    public Guid IdSede { get; set; }
    [Required]
    [StringLength(10000)]
    public string Nombre { get; set; } = "";
    public bool Activo { get; set; } = true;
}
public class AreasResponseDto
{
    public Guid IdArea { get; set; }
    public Guid IdSede { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public bool Activo { get; set; }
}
