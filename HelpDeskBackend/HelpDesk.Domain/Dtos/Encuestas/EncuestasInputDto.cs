namespace HelpDesk.Domain.Dtos.Encuestas;

public class EncuestasInputDto
{
    public Guid IdTicket
    { get; set; }
    public short Calificacion
    { get; set; }
    public string? Comentario
    { get; set; }
}
