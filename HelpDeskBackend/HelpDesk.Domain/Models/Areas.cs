using System;
using System.Collections.Generic;

namespace HelpDesk.Domain.Models;

public partial class Areas
{
    public Guid IdArea { get; set; }

    public Guid IdSede { get; set; }

    public string Nombre { get; set; } = null!;

    public bool Activo { get; set; }

    public virtual ICollection<Activos> Activos { get; set; } = new List<Activos>();

    public virtual Sedes IdSedeNavigation { get; set; } = null!;

    public virtual ICollection<Tickets> Tickets { get; set; } = new List<Tickets>();

    public virtual ICollection<Usuarios> Usuarios { get; set; } = new List<Usuarios>();
}
