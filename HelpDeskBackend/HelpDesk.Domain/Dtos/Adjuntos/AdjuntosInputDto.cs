using System.ComponentModel.DataAnnotations;

namespace HelpDesk.Domain.Dtos.Adjuntos;

public class AdjuntosInputDto
{
    public Guid IdTicket
    { get; set; }
    public Guid? IdComentario
    { get; set; }
    [Required]
    [StringLength(10000)]
    public string NombreArchivo
    { get; set; } = "";
    [Required]
    [StringLength(10000)]
    public string TipoArchivo
    { get; set; } = "";
    public long TamanoBytes
    { get; set; }
    public bool EsInterno
    { get; set; }
}
