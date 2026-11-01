using System;
using System.Collections.Generic;

namespace HelpDesk.Domain.Models;

public partial class Roles
{
    public Guid IdRol { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public virtual ICollection<UsuariosRoles> UsuariosRoles { get; set; } = new List<UsuariosRoles>();

    public virtual ICollection<Permisos> IdPermiso { get; set; } = new List<Permisos>();
}
