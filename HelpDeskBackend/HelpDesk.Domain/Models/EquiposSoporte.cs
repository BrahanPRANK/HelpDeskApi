using System;
using System.Collections.Generic;

namespace HelpDesk.Domain.Models;

public partial class EquiposSoporte
{
    public Guid IdEquipo { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public bool Activo { get; set; }

    public virtual ICollection<AsignacionesTickets> AsignacionesTickets { get; set; } = new List<AsignacionesTickets>();

    public virtual ICollection<IntegrantesEquipo> IntegrantesEquipo { get; set; } = new List<IntegrantesEquipo>();

    public virtual ICollection<Servicios> Servicios { get; set; } = new List<Servicios>();

    public virtual ICollection<Tickets> Tickets { get; set; } = new List<Tickets>();
}
