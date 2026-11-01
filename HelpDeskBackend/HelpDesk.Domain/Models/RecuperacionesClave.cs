using System;
using System.Collections.Generic;

namespace HelpDesk.Domain.Models;

public partial class RecuperacionesClave
{
    public Guid IdRecuperacion { get; set; }

    public Guid IdUsuario { get; set; }

    /// <summary>
    /// Hash de un token aleatorio temporal enviado al usuario; nunca el token original.
    /// </summary>
    public string ResumenToken { get; set; } = null!;

    public DateTime FechaSolicitud { get; set; }

    public DateTime FechaVencimiento { get; set; }

    public DateTime? FechaUso { get; set; }

    public virtual Usuarios IdUsuarioNavigation { get; set; } = null!;
}
