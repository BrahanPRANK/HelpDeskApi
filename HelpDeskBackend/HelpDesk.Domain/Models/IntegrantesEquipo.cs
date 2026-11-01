using System;
using System.Collections.Generic;

namespace HelpDesk.Domain.Models;

public partial class IntegrantesEquipo
{
    public Guid IdEquipo { get; set; }

    public Guid IdUsuario { get; set; }

    public bool EsCoordinador { get; set; }

    public bool Disponible { get; set; }

    public bool Activo { get; set; }

    public virtual ICollection<AsignacionesTickets> AsignacionesTickets { get; set; } = new List<AsignacionesTickets>();

    public virtual EquiposSoporte IdEquipoNavigation { get; set; } = null!;

    public virtual Usuarios IdUsuarioNavigation { get; set; } = null!;

    public virtual ICollection<Tickets> Tickets { get; set; } = new List<Tickets>();
}
