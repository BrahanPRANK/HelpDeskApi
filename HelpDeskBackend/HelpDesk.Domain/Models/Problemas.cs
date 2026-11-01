using System;
using System.Collections.Generic;

namespace HelpDesk.Domain.Models;

public partial class Problemas
{
    public Guid IdProblema { get; set; }

    public Guid IdResponsable { get; set; }

    public string Titulo { get; set; } = null!;

    public string Descripcion { get; set; } = null!;

    public string Estado { get; set; } = null!;

    public string? CausaRaiz { get; set; }

    public string? SolucionTemporal { get; set; }

    public string? SolucionDefinitiva { get; set; }

    public DateTime FechaCreacion { get; set; }

    public virtual Usuarios IdResponsableNavigation { get; set; } = null!;

    public virtual ICollection<Tickets> IdTicket { get; set; } = new List<Tickets>();
}
