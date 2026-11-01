using System;
using System.Collections.Generic;

namespace HelpDesk.Domain.Models;

/// <summary>
/// La API define codigos de evento estables. Los correos de seguridad no dependen de preferencias opcionales.
/// </summary>
public partial class PreferenciasNotificaciones
{
    public Guid IdUsuario { get; set; }

    public string TipoEvento { get; set; } = null!;

    public bool RecibirEnSistema { get; set; }

    public bool RecibirPorCorreo { get; set; }

    public virtual Usuarios IdUsuarioNavigation { get; set; } = null!;
}
