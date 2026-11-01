using System;
using System.Collections.Generic;

namespace HelpDesk.Domain.Models;

/// <summary>
/// La API inserta eventos en la misma transaccion del cambio; no permite editarlos ni borrarlos.
/// </summary>
public partial class HistorialTickets
{
    public Guid IdHistorial { get; set; }

    public Guid IdTicket { get; set; }

    public Guid? IdUsuario { get; set; }

    public string Accion { get; set; } = null!;

    public string? CampoModificado { get; set; }

    public string? ValorAnterior { get; set; }

    public string? ValorNuevo { get; set; }

    public string? Motivo { get; set; }

    public DateTime Fecha { get; set; }

    public virtual Tickets IdTicketNavigation { get; set; } = null!;

    public virtual Usuarios? IdUsuarioNavigation { get; set; }
}
