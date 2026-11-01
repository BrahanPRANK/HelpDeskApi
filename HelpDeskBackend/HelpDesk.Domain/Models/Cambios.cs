using System;
using System.Collections.Generic;

namespace HelpDesk.Domain.Models;

public partial class Cambios
{
    public Guid IdCambio { get; set; }

    public Guid IdSolicitante { get; set; }

    public Guid IdResponsable { get; set; }

    public string Titulo { get; set; } = null!;

    public string Descripcion { get; set; } = null!;

    public string Riesgo { get; set; } = null!;

    public string PlanEjecucion { get; set; } = null!;

    public string PlanReversion { get; set; } = null!;

    public bool RequiereAprobacion { get; set; }

    public string Estado { get; set; } = null!;

    public DateTime? FechaProgramada { get; set; }

    public DateTime? FechaFinalizacion { get; set; }

    public string? Resultado { get; set; }

    public DateTime FechaCreacion { get; set; }

    public virtual ICollection<Aprobaciones> Aprobaciones { get; set; } = new List<Aprobaciones>();

    public virtual Usuarios IdResponsableNavigation { get; set; } = null!;

    public virtual Usuarios IdSolicitanteNavigation { get; set; } = null!;

    public virtual ICollection<TareasCambios> TareasCambios { get; set; } = new List<TareasCambios>();

    public virtual ICollection<Tickets> IdTicket { get; set; } = new List<Tickets>();
}
