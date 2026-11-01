using System;
using System.Collections.Generic;

namespace HelpDesk.Domain.Models;

/// <summary>
/// Solo insercion desde la API. No guardar claves, tokens ni datos sensibles. id_registro es una referencia informativa para distintas tablas.
/// </summary>
public partial class Auditoria
{
    public Guid IdAuditoria { get; set; }

    public Guid? IdUsuario { get; set; }

    public string Accion { get; set; } = null!;

    public string TablaAfectada { get; set; } = null!;

    public Guid? IdRegistro { get; set; }

    public string Descripcion { get; set; } = null!;

    public DateTime Fecha { get; set; }

    public virtual Usuarios? IdUsuarioNavigation { get; set; }
}
