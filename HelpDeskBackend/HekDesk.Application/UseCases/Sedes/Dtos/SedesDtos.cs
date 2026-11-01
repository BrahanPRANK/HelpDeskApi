using System.ComponentModel.DataAnnotations;
namespace HelpDesk.Application.UseCases.Sedes.Dtos;
public class SedesInputDto
{
    [Required]
    [StringLength(10000)]
    public string Nombre { get; set; } = "";
    public string? Direccion { get; set; }
    public bool Activo { get; set; } = true;
}
public class SedesResponseDto
{
    public Guid IdSede { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Direccion { get; set; }
    public bool Activo { get; set; }
}
