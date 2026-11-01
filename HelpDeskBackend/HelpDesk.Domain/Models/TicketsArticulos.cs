using System;
using System.Collections.Generic;

namespace HelpDesk.Domain.Models;

public partial class TicketsArticulos
{
    public Guid IdTicket { get; set; }

    public Guid IdArticulo { get; set; }

    public Guid IdUsuario { get; set; }

    public DateTime FechaVinculacion { get; set; }

    public virtual ArticulosConocimiento IdArticuloNavigation { get; set; } = null!;

    public virtual Tickets IdTicketNavigation { get; set; } = null!;

    public virtual Usuarios IdUsuarioNavigation { get; set; } = null!;
}
