using System;
using System.Collections.Generic;

namespace HelpDesk.Domain.Models;

public partial class TiposActivos
{
    public Guid IdTipoActivo { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public bool Activo { get; set; }

    public virtual ICollection<Activos> Activos { get; set; } = new List<Activos>();
}
