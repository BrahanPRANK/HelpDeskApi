using System;
using System.Collections.Generic;

namespace HelpDesk.Domain.Models;

public partial class Encuestas
{
    public Guid IdEncuesta { get; set; }

    public Guid IdTicket { get; set; }

    public Guid IdUsuario { get; set; }

    public short Calificacion { get; set; }

    public string? Comentario { get; set; }

    public DateTime FechaRespuesta { get; set; }

    public virtual Tickets IdTicketNavigation { get; set; } = null!;

    public virtual Usuarios IdUsuarioNavigation { get; set; } = null!;
}
