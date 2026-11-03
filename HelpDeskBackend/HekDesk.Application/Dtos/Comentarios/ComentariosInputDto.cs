using System.ComponentModel.DataAnnotations;

namespace HelpDesk.Application.Dtos.Comentarios;

public class ComentariosInputDto
{
    public Guid IdTicket
    { get; set; }
    [Required]
    [StringLength(10000)]
    public string Mensaje
    { get; set; } = "";
    public bool EsInterno
    { get; set; }
}
