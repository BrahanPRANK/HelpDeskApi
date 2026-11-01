using System;
using System.Collections.Generic;

namespace HelpDesk.Domain.Models;

public partial class Comentarios
{
    public Guid IdComentario { get; set; }

    public Guid IdTicket { get; set; }

    public Guid IdAutor { get; set; }

    public string Mensaje { get; set; } = null!;

    public bool EsInterno { get; set; }

    public DateTime Fecha { get; set; }

    public virtual ICollection<Adjuntos> Adjuntos { get; set; } = new List<Adjuntos>();

    public virtual Usuarios IdAutorNavigation { get; set; } = null!;

    public virtual Tickets IdTicketNavigation { get; set; } = null!;
}
