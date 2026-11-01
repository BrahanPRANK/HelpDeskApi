using System;
using System.Collections.Generic;

namespace HelpDesk.Domain.Models;

public partial class Notificaciones
{
    public Guid IdNotificacion { get; set; }

    public Guid IdUsuario { get; set; }

    public Guid? IdTicket { get; set; }

    public string Titulo { get; set; } = null!;

    public string Mensaje { get; set; } = null!;

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaLectura { get; set; }

    public string ClaveEvento { get; set; } = null!;

    /// <summary>
    /// Codigo estable que relaciona el aviso con las preferencias del usuario.
    /// </summary>
    public string TipoEvento { get; set; } = null!;

    public virtual EnviosNotificaciones? EnviosNotificaciones { get; set; }

    public virtual Tickets? IdTicketNavigation { get; set; }

    public virtual Usuarios IdUsuarioNavigation { get; set; } = null!;
}
