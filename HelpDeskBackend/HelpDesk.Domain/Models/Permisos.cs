using System;
using System.Collections.Generic;

namespace HelpDesk.Domain.Models;

public partial class Permisos
{
    public Guid IdPermiso { get; set; }

    public string Codigo { get; set; } = null!;

    public string Modulo { get; set; } = null!;

    public string Descripcion { get; set; } = null!;

    public virtual ICollection<Roles> IdRol { get; set; } = new List<Roles>();
}
