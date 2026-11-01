using System.ComponentModel.DataAnnotations;
namespace HelpDesk.Application.UseCases.Encuestas.Dtos;
public class EncuestasInputDto
{
    public Guid IdTicket { get; set; }
    public short Calificacion { get; set; }
    public string? Comentario { get; set; }
}
public class EncuestasResponseDto
{
    public Guid IdEncuesta { get; set; }
    public Guid IdTicket { get; set; }
    public Guid IdUsuario { get; set; }
    public short Calificacion { get; set; }
    public string? Comentario { get; set; }
    public DateTime FechaRespuesta { get; set; }
}
