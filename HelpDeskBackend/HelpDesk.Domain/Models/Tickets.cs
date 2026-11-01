using System;
using System.Collections.Generic;

namespace HelpDesk.Domain.Models;

public partial class Tickets
{
    public Guid IdTicket { get; set; }

    /// <summary>
    /// Numero visible automatico. .NET puede mostrarlo como HD-000123. Puede tener saltos.
    /// </summary>
    public long Numero { get; set; }

    public Guid IdSolicitante { get; set; }

    /// <summary>
    /// Usuario autenticado que registra; puede ser distinto del solicitante si tiene permiso.
    /// </summary>
    public Guid IdCreador { get; set; }

    public Guid IdAreaSolicitante { get; set; }

    public Guid IdServicio { get; set; }

    public Guid IdEquipoAsignado { get; set; }

    public Guid? IdTecnicoAsignado { get; set; }

    public Guid IdPolitica { get; set; }

    public string Tipo { get; set; } = null!;

    public string Asunto { get; set; } = null!;

    public string Descripcion { get; set; } = null!;

    public string Prioridad { get; set; } = null!;

    public string Estado { get; set; } = null!;

    public string? MotivoEspera { get; set; }

    public string? Solucion { get; set; }

    public string? MotivoCancelacion { get; set; }

    public int MinutosPrimeraRespuesta { get; set; }

    /// <summary>
    /// Copia del objetivo al crear el ticket; cambiar una politica no cambia el compromiso historico.
    /// </summary>
    public int MinutosResolucion { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime FechaActualizacion { get; set; }

    public DateTime FechaLimiteRespuesta { get; set; }

    public DateTime FechaLimiteResolucion { get; set; }

    public DateTime? FechaPrimeraRespuesta { get; set; }

    public DateTime? FechaResolucion { get; set; }

    public DateTime? FechaCierre { get; set; }

    public DateTime? FechaCancelacion { get; set; }

    /// <summary>
    /// La API incrementa este valor y comprueba la version anterior para evitar sobrescrituras simultaneas.
    /// </summary>
    public int Version { get; set; }

    public virtual ICollection<Adjuntos> Adjuntos { get; set; } = new List<Adjuntos>();

    public virtual ICollection<Aprobaciones> Aprobaciones { get; set; } = new List<Aprobaciones>();

    public virtual AsignacionesTickets? AsignacionesTickets { get; set; }

    public virtual ICollection<Comentarios> Comentarios { get; set; } = new List<Comentarios>();

    public virtual Encuestas? Encuestas { get; set; }

    public virtual ICollection<HistorialTickets> HistorialTickets { get; set; } = new List<HistorialTickets>();

    public virtual Areas IdAreaSolicitanteNavigation { get; set; } = null!;

    public virtual Usuarios IdCreadorNavigation { get; set; } = null!;

    public virtual EquiposSoporte IdEquipoAsignadoNavigation { get; set; } = null!;

    public virtual Servicios IdServicioNavigation { get; set; } = null!;

    public virtual Usuarios IdSolicitanteNavigation { get; set; } = null!;

    public virtual Usuarios? IdTecnicoAsignadoNavigation { get; set; }

    public virtual IntegrantesEquipo? IntegrantesEquipo { get; set; }

    public virtual ICollection<Notificaciones> Notificaciones { get; set; } = new List<Notificaciones>();

    public virtual PoliticasAtencion PoliticasAtencion { get; set; } = null!;

    public virtual ICollection<TicketsArticulos> TicketsArticulos { get; set; } = new List<TicketsArticulos>();

    public virtual ICollection<Activos> IdActivo { get; set; } = new List<Activos>();

    public virtual ICollection<Cambios> IdCambio { get; set; } = new List<Cambios>();

    public virtual ICollection<Problemas> IdProblema { get; set; } = new List<Problemas>();
}
