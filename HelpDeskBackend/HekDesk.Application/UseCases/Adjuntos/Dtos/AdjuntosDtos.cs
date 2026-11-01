using System.ComponentModel.DataAnnotations;
namespace HelpDesk.Application.UseCases.Adjuntos.Dtos;
public class AdjuntosInputDto
{
    public Guid IdTicket { get; set; }
    public Guid? IdComentario { get; set; }
    [Required]
    [StringLength(10000)]
    public string NombreArchivo { get; set; } = "";
    [Required]
    [StringLength(10000)]
    public string TipoArchivo { get; set; } = "";
    public long TamanoBytes { get; set; }
    public bool EsInterno { get; set; }
}
public class AdjuntosResponseDto
{
    public Guid IdAdjunto { get; set; }
    public Guid IdTicket { get; set; }
    public Guid? IdComentario { get; set; }
    public Guid IdUsuario { get; set; }
    public string NombreArchivo { get; set; } = string.Empty;
    public string TipoArchivo { get; set; } = string.Empty;
    public long TamanoBytes { get; set; }
    public bool EsInterno { get; set; }
    public DateTime Fecha { get; set; }
}
