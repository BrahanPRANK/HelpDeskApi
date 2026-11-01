using System.ComponentModel.DataAnnotations;
namespace HelpDesk.Application.UseCases.Comentarios.Dtos;
public class ComentariosInputDto
{
    public Guid IdTicket { get; set; }
    [Required]
    [StringLength(10000)]
    public string Mensaje { get; set; } = "";
    public bool EsInterno { get; set; }
}
public class ComentariosResponseDto
{
    public Guid IdComentario { get; set; }
    public Guid IdTicket { get; set; }
    public Guid IdAutor { get; set; }
    public string Mensaje { get; set; } = string.Empty;
    public bool EsInterno { get; set; }
    public DateTime Fecha { get; set; }
}
