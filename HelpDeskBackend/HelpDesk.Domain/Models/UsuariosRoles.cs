using System;
using System.Collections.Generic;

namespace HelpDesk.Domain.Models;

public partial class UsuariosRoles
{
    public Guid IdUsuario { get; set; }

    public Guid IdRol { get; set; }

    public DateTime FechaAsignacion { get; set; }

    public virtual Roles IdRolNavigation { get; set; } = null!;

    public virtual Usuarios IdUsuarioNavigation { get; set; } = null!;
}
