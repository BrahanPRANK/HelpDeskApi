using System;
using System.Collections.Generic;

namespace HelpDesk.Domain.Models;

public partial class AsignacionesActivos
{
    public Guid IdAsignacion { get; set; }

    public Guid IdActivo { get; set; }

    public Guid IdUsuarioResponsable { get; set; }

    public Guid IdUsuarioAsignador { get; set; }

    public DateTime FechaAsignacion { get; set; }

    public DateTime? FechaDevolucion { get; set; }

    public string? Observaciones { get; set; }

    public virtual Activos IdActivoNavigation { get; set; } = null!;

    public virtual Usuarios IdUsuarioAsignadorNavigation { get; set; } = null!;

    public virtual Usuarios IdUsuarioResponsableNavigation { get; set; } = null!;
}
