using System;
using System.Collections.Generic;

namespace HelpDesk.Domain.Models;

/// <summary>
/// La API exige resolver pasos en orden. Rechazo/cancelacion termina el proceso; una nueva solicitud de negocio requiere definir su nuevo ciclo.
/// </summary>
public partial class Aprobaciones
{
    public Guid IdAprobacion { get; set; }

    public Guid? IdTicket { get; set; }

    public Guid? IdCambio { get; set; }

    public Guid IdSolicitante { get; set; }

    public Guid IdAprobador { get; set; }

    public short Orden { get; set; }

    public string Motivo { get; set; } = null!;

    public string Estado { get; set; } = null!;

    public string? ComentarioDecision { get; set; }

    public DateTime FechaSolicitud { get; set; }

    public DateTime? FechaDecision { get; set; }

    public virtual Usuarios IdAprobadorNavigation { get; set; } = null!;

    public virtual Cambios? IdCambioNavigation { get; set; }

    public virtual Usuarios IdSolicitanteNavigation { get; set; } = null!;

    public virtual Tickets? IdTicketNavigation { get; set; }
}
