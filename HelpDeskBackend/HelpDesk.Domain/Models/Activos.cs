using System;
using System.Collections.Generic;

namespace HelpDesk.Domain.Models;

public partial class Activos
{
    public Guid IdActivo { get; set; }

    public Guid IdSede { get; set; }

    public Guid? IdArea { get; set; }

    public string CodigoPatrimonial { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public Guid IdTipoActivo { get; set; }

    public string? NumeroSerie { get; set; }

    public string? Marca { get; set; }

    public string? Modelo { get; set; }

    public string Estado { get; set; } = null!;

    public DateTime FechaRegistro { get; set; }

    public virtual Areas? Areas { get; set; }

    public virtual AsignacionesActivos? AsignacionesActivos { get; set; }

    public virtual Sedes IdSedeNavigation { get; set; } = null!;

    public virtual TiposActivos IdTipoActivoNavigation { get; set; } = null!;

    public virtual ICollection<Tickets> IdTicket { get; set; } = new List<Tickets>();
}
