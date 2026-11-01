using System;
using System.Collections.Generic;

namespace HelpDesk.Domain.Models;

/// <summary>
/// Al reasignar: cerrar la anterior, crear la nueva y actualizar tickets en una sola transaccion. Mantener coherencia desde .NET.
/// </summary>
public partial class AsignacionesTickets
{
    public Guid IdAsignacionTicket { get; set; }

    public Guid IdTicket { get; set; }

    public Guid IdEquipo { get; set; }

    public Guid? IdTecnico { get; set; }

    public Guid IdAsignador { get; set; }

    public DateTime FechaAsignacion { get; set; }

    public DateTime? FechaFin { get; set; }

    public string Motivo { get; set; } = null!;

    public virtual Usuarios IdAsignadorNavigation { get; set; } = null!;

    public virtual EquiposSoporte IdEquipoNavigation { get; set; } = null!;

    public virtual Usuarios? IdTecnicoNavigation { get; set; }

    public virtual Tickets IdTicketNavigation { get; set; } = null!;

    public virtual IntegrantesEquipo? IntegrantesEquipo { get; set; }
}
