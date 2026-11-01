using System;
using System.Collections.Generic;

namespace HelpDesk.Domain.Models;

public partial class Sedes
{
    public Guid IdSede { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Direccion { get; set; }

    public bool Activo { get; set; }

    public virtual ICollection<Activos> Activos { get; set; } = new List<Activos>();

    public virtual ICollection<Areas> Areas { get; set; } = new List<Areas>();
}
