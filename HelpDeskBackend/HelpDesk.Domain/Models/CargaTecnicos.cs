using System;
using System.Collections.Generic;

namespace HelpDesk.Domain.Models;

public partial class CargaTecnicos
{
    public Guid? IdEquipoAsignado { get; set; }

    public Guid? IdTecnicoAsignado { get; set; }

    public string? Prioridad { get; set; }

    public long? TicketsPendientes { get; set; }
}
