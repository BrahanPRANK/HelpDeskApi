using System;
using System.Collections.Generic;

namespace HelpDesk.Domain.Models;

/// <summary>
/// La API comprueba permiso y visibilidad del comentario antes de descargar. Nunca exponer rutas directas del servidor.
/// </summary>
public partial class Adjuntos
{
    public Guid IdAdjunto { get; set; }

    public Guid IdTicket { get; set; }

    public Guid? IdComentario { get; set; }

    public Guid IdUsuario { get; set; }

    public string NombreArchivo { get; set; } = null!;

    public string UbicacionArchivo { get; set; } = null!;

    public string TipoArchivo { get; set; } = null!;

    public long TamanoBytes { get; set; }

    public bool EsInterno { get; set; }

    public DateTime Fecha { get; set; }

    public virtual Comentarios? Comentarios { get; set; }

    public virtual Tickets IdTicketNavigation { get; set; } = null!;

    public virtual Usuarios IdUsuarioNavigation { get; set; } = null!;
}
