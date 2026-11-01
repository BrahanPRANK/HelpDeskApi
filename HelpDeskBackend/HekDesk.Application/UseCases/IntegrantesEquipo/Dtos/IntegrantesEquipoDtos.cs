using System.ComponentModel.DataAnnotations;
namespace HelpDesk.Application.UseCases.IntegrantesEquipo.Dtos;
public class IntegrantesEquipoInputDto
{
    public Guid IdEquipo { get; set; }
    public Guid IdUsuario { get; set; }
    public bool EsCoordinador { get; set; }
    public bool Disponible { get; set; } = true;
    public bool Activo { get; set; } = true;
}
public class IntegrantesEquipoResponseDto
{
    public Guid IdEquipo { get; set; }
    public Guid IdUsuario { get; set; }
    public bool EsCoordinador { get; set; }
    public bool Disponible { get; set; }
    public bool Activo { get; set; }
}
