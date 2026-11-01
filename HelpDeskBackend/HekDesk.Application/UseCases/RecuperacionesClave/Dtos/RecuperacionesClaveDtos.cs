using System.ComponentModel.DataAnnotations;
namespace HelpDesk.Application.UseCases.RecuperacionesClave.Dtos;
public class RecuperacionesClaveInputDto
{
    public Guid IdUsuario { get; set; }
    public DateTime FechaVencimiento { get; set; }
    public DateTime? FechaUso { get; set; }
}
public class RecuperacionesClaveResponseDto
{
    public Guid IdRecuperacion { get; set; }
    public Guid IdUsuario { get; set; }
    public DateTime FechaSolicitud { get; set; }
    public DateTime FechaVencimiento { get; set; }
    public DateTime? FechaUso { get; set; }
}
