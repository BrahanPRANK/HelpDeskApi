using System;
using System.Collections.Generic;

namespace HelpDesk.Domain.Models;

/// <summary>
/// Crear junto al aviso en la misma transaccion. Worker reclama filas con bloqueo; correo puede duplicarse si el proveedor no ofrece idempotencia.
/// </summary>
public partial class EnviosNotificaciones
{
    public Guid IdEnvio { get; set; }

    public Guid IdNotificacion { get; set; }

    public string CorreoDestino { get; set; } = null!;

    public string Estado { get; set; } = null!;

    public int Intentos { get; set; }

    public int MaximoIntentos { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime FechaProximoIntento { get; set; }

    public DateTime? BloqueoHasta { get; set; }

    public DateTime? FechaEnvio { get; set; }

    public string? UltimoError { get; set; }

    public string? IdentificadorProveedor { get; set; }

    public virtual Notificaciones IdNotificacionNavigation { get; set; } = null!;
}
