namespace HelpDesk.Domain.Dtos.Comentarios;

public class ComentariosResponseDto
{
    public Guid IdComentario
    { get; set; }
    public Guid IdTicket
    { get; set; }
    public Guid IdAutor
    { get; set; }
    public string Mensaje
    { get; set; } = string.Empty;
    public bool EsInterno
    { get; set; }
    public DateTime Fecha
    { get; set; }
}
