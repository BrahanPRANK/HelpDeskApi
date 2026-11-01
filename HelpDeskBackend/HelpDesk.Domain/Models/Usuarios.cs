using System;
using System.Collections.Generic;

namespace HelpDesk.Domain.Models;

public partial class Usuarios
{
    public Guid IdUsuario { get; set; }

    public Guid? IdArea { get; set; }

    public string Nombres { get; set; } = null!;

    public string Apellidos { get; set; } = null!;

    public string NombreUsuario { get; set; } = null!;

    public string Correo { get; set; } = null!;

    /// <summary>
    /// Contrasena transformada con un algoritmo seguro desde .NET; nunca texto plano ni cifrado reversible.
    /// </summary>
    public string ClaveHash { get; set; } = null!;

    public string? Telefono { get; set; }

    public bool Activo { get; set; }

    public DateTime FechaRegistro { get; set; }

    public virtual ICollection<Adjuntos> Adjuntos { get; set; } = new List<Adjuntos>();

    public virtual ICollection<Aprobaciones> AprobacionesIdAprobadorNavigation { get; set; } = new List<Aprobaciones>();

    public virtual ICollection<Aprobaciones> AprobacionesIdSolicitanteNavigation { get; set; } = new List<Aprobaciones>();

    public virtual ICollection<ArticulosConocimiento> ArticulosConocimientoIdAutorNavigation { get; set; } = new List<ArticulosConocimiento>();

    public virtual ICollection<ArticulosConocimiento> ArticulosConocimientoIdRevisorNavigation { get; set; } = new List<ArticulosConocimiento>();

    public virtual ICollection<AsignacionesActivos> AsignacionesActivosIdUsuarioAsignadorNavigation { get; set; } = new List<AsignacionesActivos>();

    public virtual ICollection<AsignacionesActivos> AsignacionesActivosIdUsuarioResponsableNavigation { get; set; } = new List<AsignacionesActivos>();

    public virtual ICollection<AsignacionesTickets> AsignacionesTicketsIdAsignadorNavigation { get; set; } = new List<AsignacionesTickets>();

    public virtual ICollection<AsignacionesTickets> AsignacionesTicketsIdTecnicoNavigation { get; set; } = new List<AsignacionesTickets>();

    public virtual ICollection<Auditoria> Auditoria { get; set; } = new List<Auditoria>();

    public virtual ICollection<Cambios> CambiosIdResponsableNavigation { get; set; } = new List<Cambios>();

    public virtual ICollection<Cambios> CambiosIdSolicitanteNavigation { get; set; } = new List<Cambios>();

    public virtual ICollection<Comentarios> Comentarios { get; set; } = new List<Comentarios>();

    public virtual ICollection<Encuestas> Encuestas { get; set; } = new List<Encuestas>();

    public virtual ICollection<HistorialTickets> HistorialTickets { get; set; } = new List<HistorialTickets>();

    public virtual Areas? IdAreaNavigation { get; set; }

    public virtual ICollection<IntegrantesEquipo> IntegrantesEquipo { get; set; } = new List<IntegrantesEquipo>();

    public virtual ICollection<Notificaciones> Notificaciones { get; set; } = new List<Notificaciones>();

    public virtual ICollection<PreferenciasNotificaciones> PreferenciasNotificaciones { get; set; } = new List<PreferenciasNotificaciones>();

    public virtual ICollection<Problemas> Problemas { get; set; } = new List<Problemas>();

    public virtual ICollection<RecuperacionesClave> RecuperacionesClave { get; set; } = new List<RecuperacionesClave>();

    public virtual ICollection<TareasCambios> TareasCambios { get; set; } = new List<TareasCambios>();

    public virtual ICollection<TicketsArticulos> TicketsArticulos { get; set; } = new List<TicketsArticulos>();

    public virtual ICollection<Tickets> TicketsIdCreadorNavigation { get; set; } = new List<Tickets>();

    public virtual ICollection<Tickets> TicketsIdSolicitanteNavigation { get; set; } = new List<Tickets>();

    public virtual ICollection<Tickets> TicketsIdTecnicoAsignadoNavigation { get; set; } = new List<Tickets>();

    public virtual ICollection<UsuariosRoles> UsuariosRoles { get; set; } = new List<UsuariosRoles>();
}
