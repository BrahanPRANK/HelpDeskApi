using System;
using System.Collections.Generic;

namespace HelpDesk.Domain.Models;

public partial class TareasCambios
{
    public Guid IdTareaCambio { get; set; }

    public Guid IdCambio { get; set; }

    public Guid? IdResponsable { get; set; }

    public short Orden { get; set; }

    public string Titulo { get; set; } = null!;

    public string? Descripcion { get; set; }

    public string Estado { get; set; } = null!;

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaFinalizacion { get; set; }

    public virtual Cambios IdCambioNavigation { get; set; } = null!;

    public virtual Usuarios? IdResponsableNavigation { get; set; }
}
