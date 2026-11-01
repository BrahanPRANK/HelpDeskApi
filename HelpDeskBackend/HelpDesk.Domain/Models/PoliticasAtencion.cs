using System;
using System.Collections.Generic;

namespace HelpDesk.Domain.Models;

public partial class PoliticasAtencion
{
    public Guid IdPolitica { get; set; }

    public string Prioridad { get; set; } = null!;

    public int MinutosPrimeraRespuesta { get; set; }

    public int MinutosResolucion { get; set; }

    public bool Activo { get; set; }

    public virtual ICollection<Tickets> Tickets { get; set; } = new List<Tickets>();
}
