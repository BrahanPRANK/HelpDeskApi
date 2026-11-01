using System;
using System.Collections.Generic;
using HelpDesk.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Infrastructure.Data;

public partial class HelpDeskDbContext : DbContext
{
    public HelpDeskDbContext(DbContextOptions<HelpDeskDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Activos> Activos { get; set; }

    public virtual DbSet<Adjuntos> Adjuntos { get; set; }

    public virtual DbSet<Aprobaciones> Aprobaciones { get; set; }

    public virtual DbSet<Areas> Areas { get; set; }

    public virtual DbSet<ArticulosConocimiento> ArticulosConocimiento { get; set; }

    public virtual DbSet<AsignacionesActivos> AsignacionesActivos { get; set; }

    public virtual DbSet<AsignacionesTickets> AsignacionesTickets { get; set; }

    public virtual DbSet<Auditoria> Auditoria { get; set; }

    public virtual DbSet<Cambios> Cambios { get; set; }

    public virtual DbSet<CargaTecnicos> CargaTecnicos { get; set; }

    public virtual DbSet<CategoriasConocimiento> CategoriasConocimiento { get; set; }

    public virtual DbSet<CategoriasServicios> CategoriasServicios { get; set; }

    public virtual DbSet<Comentarios> Comentarios { get; set; }

    public virtual DbSet<Encuestas> Encuestas { get; set; }

    public virtual DbSet<EnviosNotificaciones> EnviosNotificaciones { get; set; }

    public virtual DbSet<EquiposSoporte> EquiposSoporte { get; set; }

    public virtual DbSet<HistorialTickets> HistorialTickets { get; set; }

    public virtual DbSet<IntegrantesEquipo> IntegrantesEquipo { get; set; }

    public virtual DbSet<Notificaciones> Notificaciones { get; set; }

    public virtual DbSet<Permisos> Permisos { get; set; }

    public virtual DbSet<PoliticasAtencion> PoliticasAtencion { get; set; }

    public virtual DbSet<PreferenciasNotificaciones> PreferenciasNotificaciones { get; set; }

    public virtual DbSet<Problemas> Problemas { get; set; }

    public virtual DbSet<RecuperacionesClave> RecuperacionesClave { get; set; }

    public virtual DbSet<ResumenTickets> ResumenTickets { get; set; }

    public virtual DbSet<Roles> Roles { get; set; }

    public virtual DbSet<Sedes> Sedes { get; set; }

    public virtual DbSet<Servicios> Servicios { get; set; }

    public virtual DbSet<TareasCambios> TareasCambios { get; set; }

    public virtual DbSet<Tickets> Tickets { get; set; }

    public virtual DbSet<TicketsArticulos> TicketsArticulos { get; set; }

    public virtual DbSet<TiposActivos> TiposActivos { get; set; }

    public virtual DbSet<Usuarios> Usuarios { get; set; }

    public virtual DbSet<UsuariosRoles> UsuariosRoles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Activos>(entity =>
        {
            entity.HasKey(e => e.IdActivo).HasName("activos_pkey");

            entity.ToTable("activos", "helpdesk");

            entity.HasIndex(e => e.CodigoPatrimonial, "activos_codigo_patrimonial_key").IsUnique();

            entity.HasIndex(e => new { e.IdArea, e.IdSede }, "ix_activos_area_sede");

            entity.HasIndex(e => e.IdSede, "ix_activos_sede");

            entity.HasIndex(e => e.IdTipoActivo, "ix_activos_tipo");

            entity.Property(e => e.IdActivo)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id_activo");
            entity.Property(e => e.CodigoPatrimonial)
                .HasMaxLength(80)
                .HasColumnName("codigo_patrimonial");
            entity.Property(e => e.Estado)
                .HasMaxLength(30)
                .HasDefaultValueSql("'disponible'::character varying")
                .HasColumnName("estado");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("fecha_registro");
            entity.Property(e => e.IdArea).HasColumnName("id_area");
            entity.Property(e => e.IdSede).HasColumnName("id_sede");
            entity.Property(e => e.IdTipoActivo).HasColumnName("id_tipo_activo");
            entity.Property(e => e.Marca)
                .HasMaxLength(100)
                .HasColumnName("marca");
            entity.Property(e => e.Modelo)
                .HasMaxLength(100)
                .HasColumnName("modelo");
            entity.Property(e => e.Nombre)
                .HasMaxLength(150)
                .HasColumnName("nombre");
            entity.Property(e => e.NumeroSerie)
                .HasMaxLength(150)
                .HasColumnName("numero_serie");

            entity.HasOne(d => d.IdSedeNavigation).WithMany(p => p.Activos)
                .HasForeignKey(d => d.IdSede)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("activos_id_sede_fkey");

            entity.HasOne(d => d.IdTipoActivoNavigation).WithMany(p => p.Activos)
                .HasForeignKey(d => d.IdTipoActivo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_activos_tipo");

            entity.HasOne(d => d.Areas).WithMany(p => p.Activos)
                .HasPrincipalKey(p => new { p.IdArea, p.IdSede })
                .HasForeignKey(d => new { d.IdArea, d.IdSede })
                .HasConstraintName("activos_id_area_id_sede_fkey");
        });

        modelBuilder.Entity<Adjuntos>(entity =>
        {
            entity.HasKey(e => e.IdAdjunto).HasName("adjuntos_pkey");

            entity.ToTable("adjuntos", "helpdesk", tb => tb.HasComment("La API comprueba permiso y visibilidad del comentario antes de descargar. Nunca exponer rutas directas del servidor."));

            entity.HasIndex(e => e.UbicacionArchivo, "adjuntos_ubicacion_archivo_key").IsUnique();

            entity.HasIndex(e => new { e.IdComentario, e.IdTicket }, "ix_adjuntos_comentario");

            entity.HasIndex(e => e.IdTicket, "ix_adjuntos_ticket");

            entity.Property(e => e.IdAdjunto)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id_adjunto");
            entity.Property(e => e.EsInterno).HasColumnName("es_interno");
            entity.Property(e => e.Fecha)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("fecha");
            entity.Property(e => e.IdComentario).HasColumnName("id_comentario");
            entity.Property(e => e.IdTicket).HasColumnName("id_ticket");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.NombreArchivo)
                .HasMaxLength(255)
                .HasColumnName("nombre_archivo");
            entity.Property(e => e.TamanoBytes).HasColumnName("tamano_bytes");
            entity.Property(e => e.TipoArchivo)
                .HasMaxLength(150)
                .HasColumnName("tipo_archivo");
            entity.Property(e => e.UbicacionArchivo).HasColumnName("ubicacion_archivo");

            entity.HasOne(d => d.IdTicketNavigation).WithMany(p => p.Adjuntos)
                .HasForeignKey(d => d.IdTicket)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("adjuntos_id_ticket_fkey");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Adjuntos)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("adjuntos_id_usuario_fkey");

            entity.HasOne(d => d.Comentarios).WithMany(p => p.Adjuntos)
                .HasPrincipalKey(p => new { p.IdComentario, p.IdTicket })
                .HasForeignKey(d => new { d.IdComentario, d.IdTicket })
                .HasConstraintName("adjuntos_id_comentario_id_ticket_fkey");
        });

        modelBuilder.Entity<Aprobaciones>(entity =>
        {
            entity.HasKey(e => e.IdAprobacion).HasName("aprobaciones_pkey");

            entity.ToTable("aprobaciones", "helpdesk", tb => tb.HasComment("La API exige resolver pasos en orden. Rechazo/cancelacion termina el proceso; una nueva solicitud de negocio requiere definir su nuevo ciclo."));

            entity.HasIndex(e => new { e.IdAprobador, e.Estado }, "ix_aprobaciones_aprobador_estado");

            entity.HasIndex(e => new { e.IdCambio, e.Orden }, "uq_aprobaciones_cambio_orden")
                .IsUnique()
                .HasFilter("(id_cambio IS NOT NULL)");

            entity.HasIndex(e => new { e.IdTicket, e.Orden }, "uq_aprobaciones_ticket_orden")
                .IsUnique()
                .HasFilter("(id_ticket IS NOT NULL)");

            entity.Property(e => e.IdAprobacion)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id_aprobacion");
            entity.Property(e => e.ComentarioDecision).HasColumnName("comentario_decision");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .HasDefaultValueSql("'pendiente'::character varying")
                .HasColumnName("estado");
            entity.Property(e => e.FechaDecision).HasColumnName("fecha_decision");
            entity.Property(e => e.FechaSolicitud)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("fecha_solicitud");
            entity.Property(e => e.IdAprobador).HasColumnName("id_aprobador");
            entity.Property(e => e.IdCambio).HasColumnName("id_cambio");
            entity.Property(e => e.IdSolicitante).HasColumnName("id_solicitante");
            entity.Property(e => e.IdTicket).HasColumnName("id_ticket");
            entity.Property(e => e.Motivo).HasColumnName("motivo");
            entity.Property(e => e.Orden)
                .HasDefaultValue((short)1)
                .HasColumnName("orden");

            entity.HasOne(d => d.IdAprobadorNavigation).WithMany(p => p.AprobacionesIdAprobadorNavigation)
                .HasForeignKey(d => d.IdAprobador)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("aprobaciones_id_aprobador_fkey");

            entity.HasOne(d => d.IdCambioNavigation).WithMany(p => p.Aprobaciones)
                .HasForeignKey(d => d.IdCambio)
                .HasConstraintName("aprobaciones_id_cambio_fkey");

            entity.HasOne(d => d.IdSolicitanteNavigation).WithMany(p => p.AprobacionesIdSolicitanteNavigation)
                .HasForeignKey(d => d.IdSolicitante)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("aprobaciones_id_solicitante_fkey");

            entity.HasOne(d => d.IdTicketNavigation).WithMany(p => p.Aprobaciones)
                .HasForeignKey(d => d.IdTicket)
                .HasConstraintName("aprobaciones_id_ticket_fkey");
        });

        modelBuilder.Entity<Areas>(entity =>
        {
            entity.HasKey(e => e.IdArea).HasName("areas_pkey");

            entity.ToTable("areas", "helpdesk");

            entity.HasIndex(e => new { e.IdArea, e.IdSede }, "areas_id_area_id_sede_key").IsUnique();

            entity.HasIndex(e => new { e.IdSede, e.Nombre }, "areas_id_sede_nombre_key").IsUnique();

            entity.Property(e => e.IdArea)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id_area");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.IdSede).HasColumnName("id_sede");
            entity.Property(e => e.Nombre)
                .HasMaxLength(150)
                .HasColumnName("nombre");

            entity.HasOne(d => d.IdSedeNavigation).WithMany(p => p.Areas)
                .HasForeignKey(d => d.IdSede)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("areas_id_sede_fkey");
        });

        modelBuilder.Entity<ArticulosConocimiento>(entity =>
        {
            entity.HasKey(e => e.IdArticulo).HasName("articulos_conocimiento_pkey");

            entity.ToTable("articulos_conocimiento", "helpdesk");

            entity.HasIndex(e => e.IdCategoriaConocimiento, "ix_articulos_categoria");

            entity.HasIndex(e => e.Estado, "ix_articulos_estado");

            entity.Property(e => e.IdArticulo)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id_articulo");
            entity.Property(e => e.Contenido).HasColumnName("contenido");
            entity.Property(e => e.EsInterno).HasColumnName("es_interno");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .HasDefaultValueSql("'borrador'::character varying")
                .HasColumnName("estado");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaPublicacion).HasColumnName("fecha_publicacion");
            entity.Property(e => e.IdAutor).HasColumnName("id_autor");
            entity.Property(e => e.IdCategoriaConocimiento).HasColumnName("id_categoria_conocimiento");
            entity.Property(e => e.IdRevisor).HasColumnName("id_revisor");
            entity.Property(e => e.Titulo)
                .HasMaxLength(255)
                .HasColumnName("titulo");

            entity.HasOne(d => d.IdAutorNavigation).WithMany(p => p.ArticulosConocimientoIdAutorNavigation)
                .HasForeignKey(d => d.IdAutor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("articulos_conocimiento_id_autor_fkey");

            entity.HasOne(d => d.IdCategoriaConocimientoNavigation).WithMany(p => p.ArticulosConocimiento)
                .HasForeignKey(d => d.IdCategoriaConocimiento)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_articulos_categoria");

            entity.HasOne(d => d.IdRevisorNavigation).WithMany(p => p.ArticulosConocimientoIdRevisorNavigation)
                .HasForeignKey(d => d.IdRevisor)
                .HasConstraintName("articulos_conocimiento_id_revisor_fkey");
        });

        modelBuilder.Entity<AsignacionesActivos>(entity =>
        {
            entity.HasKey(e => e.IdAsignacion).HasName("asignaciones_activos_pkey");

            entity.ToTable("asignaciones_activos", "helpdesk");

            entity.HasIndex(e => e.IdUsuarioResponsable, "ix_asignaciones_usuario");

            entity.HasIndex(e => e.IdActivo, "uq_activo_asignacion_vigente")
                .IsUnique()
                .HasFilter("(fecha_devolucion IS NULL)");

            entity.Property(e => e.IdAsignacion)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id_asignacion");
            entity.Property(e => e.FechaAsignacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("fecha_asignacion");
            entity.Property(e => e.FechaDevolucion).HasColumnName("fecha_devolucion");
            entity.Property(e => e.IdActivo).HasColumnName("id_activo");
            entity.Property(e => e.IdUsuarioAsignador).HasColumnName("id_usuario_asignador");
            entity.Property(e => e.IdUsuarioResponsable).HasColumnName("id_usuario_responsable");
            entity.Property(e => e.Observaciones).HasColumnName("observaciones");

            entity.HasOne(d => d.IdActivoNavigation).WithOne(p => p.AsignacionesActivos)
                .HasForeignKey<AsignacionesActivos>(d => d.IdActivo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("asignaciones_activos_id_activo_fkey");

            entity.HasOne(d => d.IdUsuarioAsignadorNavigation).WithMany(p => p.AsignacionesActivosIdUsuarioAsignadorNavigation)
                .HasForeignKey(d => d.IdUsuarioAsignador)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("asignaciones_activos_id_usuario_asignador_fkey");

            entity.HasOne(d => d.IdUsuarioResponsableNavigation).WithMany(p => p.AsignacionesActivosIdUsuarioResponsableNavigation)
                .HasForeignKey(d => d.IdUsuarioResponsable)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("asignaciones_activos_id_usuario_responsable_fkey");
        });

        modelBuilder.Entity<AsignacionesTickets>(entity =>
        {
            entity.HasKey(e => e.IdAsignacionTicket).HasName("asignaciones_tickets_pkey");

            entity.ToTable("asignaciones_tickets", "helpdesk", tb => tb.HasComment("Al reasignar: cerrar la anterior, crear la nueva y actualizar tickets en una sola transaccion. Mantener coherencia desde .NET."));

            entity.HasIndex(e => new { e.IdEquipo, e.IdTecnico }, "ix_asignaciones_ticket_equipo");

            entity.HasIndex(e => new { e.IdTicket, e.FechaAsignacion }, "ix_asignaciones_ticket_fecha");

            entity.HasIndex(e => e.IdTecnico, "ix_asignaciones_ticket_tecnico");

            entity.HasIndex(e => e.IdTicket, "uq_ticket_asignacion_vigente")
                .IsUnique()
                .HasFilter("(fecha_fin IS NULL)");

            entity.Property(e => e.IdAsignacionTicket)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id_asignacion_ticket");
            entity.Property(e => e.FechaAsignacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("fecha_asignacion");
            entity.Property(e => e.FechaFin).HasColumnName("fecha_fin");
            entity.Property(e => e.IdAsignador).HasColumnName("id_asignador");
            entity.Property(e => e.IdEquipo).HasColumnName("id_equipo");
            entity.Property(e => e.IdTecnico).HasColumnName("id_tecnico");
            entity.Property(e => e.IdTicket).HasColumnName("id_ticket");
            entity.Property(e => e.Motivo).HasColumnName("motivo");

            entity.HasOne(d => d.IdAsignadorNavigation).WithMany(p => p.AsignacionesTicketsIdAsignadorNavigation)
                .HasForeignKey(d => d.IdAsignador)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("asignaciones_tickets_id_asignador_fkey");

            entity.HasOne(d => d.IdEquipoNavigation).WithMany(p => p.AsignacionesTickets)
                .HasForeignKey(d => d.IdEquipo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("asignaciones_tickets_id_equipo_fkey");

            entity.HasOne(d => d.IdTecnicoNavigation).WithMany(p => p.AsignacionesTicketsIdTecnicoNavigation)
                .HasForeignKey(d => d.IdTecnico)
                .HasConstraintName("asignaciones_tickets_id_tecnico_fkey");

            entity.HasOne(d => d.IdTicketNavigation).WithOne(p => p.AsignacionesTickets)
                .HasForeignKey<AsignacionesTickets>(d => d.IdTicket)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("asignaciones_tickets_id_ticket_fkey");

            entity.HasOne(d => d.IntegrantesEquipo).WithMany(p => p.AsignacionesTickets)
                .HasForeignKey(d => new { d.IdEquipo, d.IdTecnico })
                .HasConstraintName("asignaciones_tickets_id_equipo_id_tecnico_fkey");
        });

        modelBuilder.Entity<Auditoria>(entity =>
        {
            entity.HasKey(e => e.IdAuditoria).HasName("auditoria_pkey");

            entity.ToTable("auditoria", "helpdesk", tb => tb.HasComment("Solo insercion desde la API. No guardar claves, tokens ni datos sensibles. id_registro es una referencia informativa para distintas tablas."));

            entity.HasIndex(e => new { e.TablaAfectada, e.IdRegistro }, "ix_auditoria_registro");

            entity.HasIndex(e => new { e.IdUsuario, e.Fecha }, "ix_auditoria_usuario_fecha");

            entity.Property(e => e.IdAuditoria)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id_auditoria");
            entity.Property(e => e.Accion)
                .HasMaxLength(100)
                .HasColumnName("accion");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.Fecha)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("fecha");
            entity.Property(e => e.IdRegistro).HasColumnName("id_registro");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.TablaAfectada)
                .HasMaxLength(80)
                .HasColumnName("tabla_afectada");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Auditoria)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("auditoria_id_usuario_fkey");
        });

        modelBuilder.Entity<Cambios>(entity =>
        {
            entity.HasKey(e => e.IdCambio).HasName("cambios_pkey");

            entity.ToTable("cambios", "helpdesk");

            entity.Property(e => e.IdCambio)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id_cambio");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.Estado)
                .HasMaxLength(30)
                .HasDefaultValueSql("'borrador'::character varying")
                .HasColumnName("estado");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaFinalizacion).HasColumnName("fecha_finalizacion");
            entity.Property(e => e.FechaProgramada).HasColumnName("fecha_programada");
            entity.Property(e => e.IdResponsable).HasColumnName("id_responsable");
            entity.Property(e => e.IdSolicitante).HasColumnName("id_solicitante");
            entity.Property(e => e.PlanEjecucion).HasColumnName("plan_ejecucion");
            entity.Property(e => e.PlanReversion).HasColumnName("plan_reversion");
            entity.Property(e => e.RequiereAprobacion)
                .HasDefaultValue(true)
                .HasColumnName("requiere_aprobacion");
            entity.Property(e => e.Resultado).HasColumnName("resultado");
            entity.Property(e => e.Riesgo)
                .HasMaxLength(10)
                .HasColumnName("riesgo");
            entity.Property(e => e.Titulo)
                .HasMaxLength(255)
                .HasColumnName("titulo");

            entity.HasOne(d => d.IdResponsableNavigation).WithMany(p => p.CambiosIdResponsableNavigation)
                .HasForeignKey(d => d.IdResponsable)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("cambios_id_responsable_fkey");

            entity.HasOne(d => d.IdSolicitanteNavigation).WithMany(p => p.CambiosIdSolicitanteNavigation)
                .HasForeignKey(d => d.IdSolicitante)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("cambios_id_solicitante_fkey");

            entity.HasMany(d => d.IdTicket).WithMany(p => p.IdCambio)
                .UsingEntity<Dictionary<string, object>>(
                    "CambiosTickets",
                    r => r.HasOne<Tickets>().WithMany()
                        .HasForeignKey("IdTicket")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("cambios_tickets_id_ticket_fkey"),
                    l => l.HasOne<Cambios>().WithMany()
                        .HasForeignKey("IdCambio")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("cambios_tickets_id_cambio_fkey"),
                    j =>
                    {
                        j.HasKey("IdCambio", "IdTicket").HasName("cambios_tickets_pkey");
                        j.ToTable("cambios_tickets", "helpdesk");
                        j.HasIndex(new[] { "IdTicket" }, "ix_cambios_tickets_ticket");
                        j.IndexerProperty<Guid>("IdCambio").HasColumnName("id_cambio");
                        j.IndexerProperty<Guid>("IdTicket").HasColumnName("id_ticket");
                    });
        });

        modelBuilder.Entity<CargaTecnicos>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("carga_tecnicos", "helpdesk");

            entity.Property(e => e.IdEquipoAsignado).HasColumnName("id_equipo_asignado");
            entity.Property(e => e.IdTecnicoAsignado).HasColumnName("id_tecnico_asignado");
            entity.Property(e => e.Prioridad)
                .HasMaxLength(20)
                .HasColumnName("prioridad");
            entity.Property(e => e.TicketsPendientes).HasColumnName("tickets_pendientes");
        });

        modelBuilder.Entity<CategoriasConocimiento>(entity =>
        {
            entity.HasKey(e => e.IdCategoriaConocimiento).HasName("categorias_conocimiento_pkey");

            entity.ToTable("categorias_conocimiento", "helpdesk");

            entity.HasIndex(e => e.Nombre, "categorias_conocimiento_nombre_key").IsUnique();

            entity.Property(e => e.IdCategoriaConocimiento)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id_categoria_conocimiento");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(255)
                .HasColumnName("descripcion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<CategoriasServicios>(entity =>
        {
            entity.HasKey(e => e.IdCategoriaServicio).HasName("categorias_servicios_pkey");

            entity.ToTable("categorias_servicios", "helpdesk");

            entity.HasIndex(e => e.Nombre, "categorias_servicios_nombre_key").IsUnique();

            entity.Property(e => e.IdCategoriaServicio)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id_categoria_servicio");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(255)
                .HasColumnName("descripcion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Comentarios>(entity =>
        {
            entity.HasKey(e => e.IdComentario).HasName("comentarios_pkey");

            entity.ToTable("comentarios", "helpdesk");

            entity.HasIndex(e => new { e.IdComentario, e.IdTicket }, "comentarios_id_comentario_id_ticket_key").IsUnique();

            entity.HasIndex(e => new { e.IdTicket, e.Fecha }, "ix_comentarios_ticket_fecha");

            entity.Property(e => e.IdComentario)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id_comentario");
            entity.Property(e => e.EsInterno).HasColumnName("es_interno");
            entity.Property(e => e.Fecha)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("fecha");
            entity.Property(e => e.IdAutor).HasColumnName("id_autor");
            entity.Property(e => e.IdTicket).HasColumnName("id_ticket");
            entity.Property(e => e.Mensaje).HasColumnName("mensaje");

            entity.HasOne(d => d.IdAutorNavigation).WithMany(p => p.Comentarios)
                .HasForeignKey(d => d.IdAutor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("comentarios_id_autor_fkey");

            entity.HasOne(d => d.IdTicketNavigation).WithMany(p => p.Comentarios)
                .HasForeignKey(d => d.IdTicket)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("comentarios_id_ticket_fkey");
        });

        modelBuilder.Entity<Encuestas>(entity =>
        {
            entity.HasKey(e => e.IdEncuesta).HasName("encuestas_pkey");

            entity.ToTable("encuestas", "helpdesk");

            entity.HasIndex(e => e.IdTicket, "encuestas_id_ticket_key").IsUnique();

            entity.Property(e => e.IdEncuesta)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id_encuesta");
            entity.Property(e => e.Calificacion).HasColumnName("calificacion");
            entity.Property(e => e.Comentario).HasColumnName("comentario");
            entity.Property(e => e.FechaRespuesta)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("fecha_respuesta");
            entity.Property(e => e.IdTicket).HasColumnName("id_ticket");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");

            entity.HasOne(d => d.IdTicketNavigation).WithOne(p => p.Encuestas)
                .HasForeignKey<Encuestas>(d => d.IdTicket)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("encuestas_id_ticket_fkey");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Encuestas)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("encuestas_id_usuario_fkey");
        });

        modelBuilder.Entity<EnviosNotificaciones>(entity =>
        {
            entity.HasKey(e => e.IdEnvio).HasName("envios_notificaciones_pkey");

            entity.ToTable("envios_notificaciones", "helpdesk", tb => tb.HasComment("Crear junto al aviso en la misma transaccion. Worker reclama filas con bloqueo; correo puede duplicarse si el proveedor no ofrece idempotencia."));

            entity.HasIndex(e => e.IdNotificacion, "envios_notificaciones_id_notificacion_key").IsUnique();

            entity.HasIndex(e => e.FechaProximoIntento, "ix_envios_pendientes").HasFilter("((estado)::text = ANY ((ARRAY['pendiente'::character varying, 'procesando'::character varying])::text[]))");

            entity.Property(e => e.IdEnvio)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id_envio");
            entity.Property(e => e.BloqueoHasta).HasColumnName("bloqueo_hasta");
            entity.Property(e => e.CorreoDestino)
                .HasMaxLength(254)
                .HasColumnName("correo_destino");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .HasDefaultValueSql("'pendiente'::character varying")
                .HasColumnName("estado");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaEnvio).HasColumnName("fecha_envio");
            entity.Property(e => e.FechaProximoIntento)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("fecha_proximo_intento");
            entity.Property(e => e.IdNotificacion).HasColumnName("id_notificacion");
            entity.Property(e => e.IdentificadorProveedor)
                .HasMaxLength(255)
                .HasColumnName("identificador_proveedor");
            entity.Property(e => e.Intentos).HasColumnName("intentos");
            entity.Property(e => e.MaximoIntentos)
                .HasDefaultValue(3)
                .HasColumnName("maximo_intentos");
            entity.Property(e => e.UltimoError).HasColumnName("ultimo_error");

            entity.HasOne(d => d.IdNotificacionNavigation).WithOne(p => p.EnviosNotificaciones)
                .HasForeignKey<EnviosNotificaciones>(d => d.IdNotificacion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("envios_notificaciones_id_notificacion_fkey");
        });

        modelBuilder.Entity<EquiposSoporte>(entity =>
        {
            entity.HasKey(e => e.IdEquipo).HasName("equipos_soporte_pkey");

            entity.ToTable("equipos_soporte", "helpdesk");

            entity.HasIndex(e => e.Nombre, "equipos_soporte_nombre_key").IsUnique();

            entity.Property(e => e.IdEquipo)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id_equipo");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(150)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<HistorialTickets>(entity =>
        {
            entity.HasKey(e => e.IdHistorial).HasName("historial_tickets_pkey");

            entity.ToTable("historial_tickets", "helpdesk", tb => tb.HasComment("La API inserta eventos en la misma transaccion del cambio; no permite editarlos ni borrarlos."));

            entity.HasIndex(e => new { e.IdTicket, e.Fecha }, "ix_historial_ticket_fecha");

            entity.Property(e => e.IdHistorial)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id_historial");
            entity.Property(e => e.Accion)
                .HasMaxLength(50)
                .HasColumnName("accion");
            entity.Property(e => e.CampoModificado)
                .HasMaxLength(80)
                .HasColumnName("campo_modificado");
            entity.Property(e => e.Fecha)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("fecha");
            entity.Property(e => e.IdTicket).HasColumnName("id_ticket");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.Motivo).HasColumnName("motivo");
            entity.Property(e => e.ValorAnterior).HasColumnName("valor_anterior");
            entity.Property(e => e.ValorNuevo).HasColumnName("valor_nuevo");

            entity.HasOne(d => d.IdTicketNavigation).WithMany(p => p.HistorialTickets)
                .HasForeignKey(d => d.IdTicket)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("historial_tickets_id_ticket_fkey");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.HistorialTickets)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("historial_tickets_id_usuario_fkey");
        });

        modelBuilder.Entity<IntegrantesEquipo>(entity =>
        {
            entity.HasKey(e => new { e.IdEquipo, e.IdUsuario }).HasName("integrantes_equipo_pkey");

            entity.ToTable("integrantes_equipo", "helpdesk");

            entity.HasIndex(e => e.IdUsuario, "ix_integrantes_usuario");

            entity.Property(e => e.IdEquipo).HasColumnName("id_equipo");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Disponible)
                .HasDefaultValue(true)
                .HasColumnName("disponible");
            entity.Property(e => e.EsCoordinador).HasColumnName("es_coordinador");

            entity.HasOne(d => d.IdEquipoNavigation).WithMany(p => p.IntegrantesEquipo)
                .HasForeignKey(d => d.IdEquipo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("integrantes_equipo_id_equipo_fkey");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.IntegrantesEquipo)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("integrantes_equipo_id_usuario_fkey");
        });

        modelBuilder.Entity<Notificaciones>(entity =>
        {
            entity.HasKey(e => e.IdNotificacion).HasName("notificaciones_pkey");

            entity.ToTable("notificaciones", "helpdesk");

            entity.HasIndex(e => e.IdTicket, "ix_notificaciones_ticket");

            entity.HasIndex(e => new { e.IdUsuario, e.FechaCreacion }, "ix_notificaciones_usuario_fecha").IsDescending(false, true);

            entity.HasIndex(e => new { e.IdUsuario, e.ClaveEvento }, "notificaciones_id_usuario_clave_evento_key").IsUnique();

            entity.Property(e => e.IdNotificacion)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id_notificacion");
            entity.Property(e => e.ClaveEvento)
                .HasMaxLength(200)
                .HasColumnName("clave_evento");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaLectura).HasColumnName("fecha_lectura");
            entity.Property(e => e.IdTicket).HasColumnName("id_ticket");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.Mensaje).HasColumnName("mensaje");
            entity.Property(e => e.TipoEvento)
                .HasMaxLength(80)
                .HasComment("Codigo estable que relaciona el aviso con las preferencias del usuario.")
                .HasColumnName("tipo_evento");
            entity.Property(e => e.Titulo)
                .HasMaxLength(255)
                .HasColumnName("titulo");

            entity.HasOne(d => d.IdTicketNavigation).WithMany(p => p.Notificaciones)
                .HasForeignKey(d => d.IdTicket)
                .HasConstraintName("notificaciones_id_ticket_fkey");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Notificaciones)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("notificaciones_id_usuario_fkey");
        });

        modelBuilder.Entity<Permisos>(entity =>
        {
            entity.HasKey(e => e.IdPermiso).HasName("permisos_pkey");

            entity.ToTable("permisos", "helpdesk");

            entity.HasIndex(e => e.Codigo, "permisos_codigo_key").IsUnique();

            entity.Property(e => e.IdPermiso)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id_permiso");
            entity.Property(e => e.Codigo)
                .HasMaxLength(100)
                .HasColumnName("codigo");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(255)
                .HasColumnName("descripcion");
            entity.Property(e => e.Modulo)
                .HasMaxLength(50)
                .HasColumnName("modulo");
        });

        modelBuilder.Entity<PoliticasAtencion>(entity =>
        {
            entity.HasKey(e => e.IdPolitica).HasName("politicas_atencion_pkey");

            entity.ToTable("politicas_atencion", "helpdesk");

            entity.HasIndex(e => new { e.IdPolitica, e.Prioridad }, "politicas_atencion_id_politica_prioridad_key").IsUnique();

            entity.HasIndex(e => e.Prioridad, "politicas_atencion_prioridad_key").IsUnique();

            entity.Property(e => e.IdPolitica)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id_politica");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.MinutosPrimeraRespuesta).HasColumnName("minutos_primera_respuesta");
            entity.Property(e => e.MinutosResolucion).HasColumnName("minutos_resolucion");
            entity.Property(e => e.Prioridad)
                .HasMaxLength(20)
                .HasColumnName("prioridad");
        });

        modelBuilder.Entity<PreferenciasNotificaciones>(entity =>
        {
            entity.HasKey(e => new { e.IdUsuario, e.TipoEvento }).HasName("preferencias_notificaciones_pkey");

            entity.ToTable("preferencias_notificaciones", "helpdesk", tb => tb.HasComment("La API define codigos de evento estables. Los correos de seguridad no dependen de preferencias opcionales."));

            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.TipoEvento)
                .HasMaxLength(80)
                .HasColumnName("tipo_evento");
            entity.Property(e => e.RecibirEnSistema)
                .HasDefaultValue(true)
                .HasColumnName("recibir_en_sistema");
            entity.Property(e => e.RecibirPorCorreo)
                .HasDefaultValue(true)
                .HasColumnName("recibir_por_correo");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.PreferenciasNotificaciones)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("preferencias_notificaciones_id_usuario_fkey");
        });

        modelBuilder.Entity<Problemas>(entity =>
        {
            entity.HasKey(e => e.IdProblema).HasName("problemas_pkey");

            entity.ToTable("problemas", "helpdesk");

            entity.Property(e => e.IdProblema)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id_problema");
            entity.Property(e => e.CausaRaiz).HasColumnName("causa_raiz");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.Estado)
                .HasMaxLength(30)
                .HasDefaultValueSql("'abierto'::character varying")
                .HasColumnName("estado");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.IdResponsable).HasColumnName("id_responsable");
            entity.Property(e => e.SolucionDefinitiva).HasColumnName("solucion_definitiva");
            entity.Property(e => e.SolucionTemporal).HasColumnName("solucion_temporal");
            entity.Property(e => e.Titulo)
                .HasMaxLength(255)
                .HasColumnName("titulo");

            entity.HasOne(d => d.IdResponsableNavigation).WithMany(p => p.Problemas)
                .HasForeignKey(d => d.IdResponsable)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("problemas_id_responsable_fkey");

            entity.HasMany(d => d.IdTicket).WithMany(p => p.IdProblema)
                .UsingEntity<Dictionary<string, object>>(
                    "ProblemasTickets",
                    r => r.HasOne<Tickets>().WithMany()
                        .HasForeignKey("IdTicket")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("problemas_tickets_id_ticket_fkey"),
                    l => l.HasOne<Problemas>().WithMany()
                        .HasForeignKey("IdProblema")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("problemas_tickets_id_problema_fkey"),
                    j =>
                    {
                        j.HasKey("IdProblema", "IdTicket").HasName("problemas_tickets_pkey");
                        j.ToTable("problemas_tickets", "helpdesk");
                        j.HasIndex(new[] { "IdTicket" }, "ix_problemas_tickets_ticket");
                        j.IndexerProperty<Guid>("IdProblema").HasColumnName("id_problema");
                        j.IndexerProperty<Guid>("IdTicket").HasColumnName("id_ticket");
                    });
        });

        modelBuilder.Entity<RecuperacionesClave>(entity =>
        {
            entity.HasKey(e => e.IdRecuperacion).HasName("recuperaciones_clave_pkey");

            entity.ToTable("recuperaciones_clave", "helpdesk");

            entity.HasIndex(e => new { e.IdUsuario, e.FechaVencimiento }, "ix_recuperaciones_usuario_fecha");

            entity.HasIndex(e => e.ResumenToken, "recuperaciones_clave_resumen_token_key").IsUnique();

            entity.Property(e => e.IdRecuperacion)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id_recuperacion");
            entity.Property(e => e.FechaSolicitud)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("fecha_solicitud");
            entity.Property(e => e.FechaUso).HasColumnName("fecha_uso");
            entity.Property(e => e.FechaVencimiento).HasColumnName("fecha_vencimiento");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.ResumenToken)
                .HasMaxLength(128)
                .HasComment("Hash de un token aleatorio temporal enviado al usuario; nunca el token original.")
                .HasColumnName("resumen_token");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.RecuperacionesClave)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("recuperaciones_clave_id_usuario_fkey");
        });

        modelBuilder.Entity<ResumenTickets>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("resumen_tickets", "helpdesk");

            entity.Property(e => e.Asunto)
                .HasMaxLength(255)
                .HasColumnName("asunto");
            entity.Property(e => e.Calificacion).HasColumnName("calificacion");
            entity.Property(e => e.Estado)
                .HasMaxLength(30)
                .HasColumnName("estado");
            entity.Property(e => e.FechaCierre).HasColumnName("fecha_cierre");
            entity.Property(e => e.FechaCreacion).HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaLimiteResolucion).HasColumnName("fecha_limite_resolucion");
            entity.Property(e => e.FechaLimiteRespuesta).HasColumnName("fecha_limite_respuesta");
            entity.Property(e => e.FechaPrimeraRespuesta).HasColumnName("fecha_primera_respuesta");
            entity.Property(e => e.FechaResolucion).HasColumnName("fecha_resolucion");
            entity.Property(e => e.IdAreaSolicitante).HasColumnName("id_area_solicitante");
            entity.Property(e => e.IdEquipoAsignado).HasColumnName("id_equipo_asignado");
            entity.Property(e => e.IdSolicitante).HasColumnName("id_solicitante");
            entity.Property(e => e.IdTecnicoAsignado).HasColumnName("id_tecnico_asignado");
            entity.Property(e => e.IdTicket).HasColumnName("id_ticket");
            entity.Property(e => e.MinutosHastaResolucion).HasColumnName("minutos_hasta_resolucion");
            entity.Property(e => e.Numero).HasColumnName("numero");
            entity.Property(e => e.Prioridad)
                .HasMaxLength(20)
                .HasColumnName("prioridad");
            entity.Property(e => e.ResolucionEnPlazo).HasColumnName("resolucion_en_plazo");
            entity.Property(e => e.RespuestaEnPlazo).HasColumnName("respuesta_en_plazo");
            entity.Property(e => e.Tipo)
                .HasMaxLength(20)
                .HasColumnName("tipo");
        });

        modelBuilder.Entity<Roles>(entity =>
        {
            entity.HasKey(e => e.IdRol).HasName("roles_pkey");

            entity.ToTable("roles", "helpdesk");

            entity.HasIndex(e => e.Nombre, "roles_nombre_key").IsUnique();

            entity.Property(e => e.IdRol)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id_rol");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(255)
                .HasColumnName("descripcion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");

            entity.HasMany(d => d.IdPermiso).WithMany(p => p.IdRol)
                .UsingEntity<Dictionary<string, object>>(
                    "RolesPermisos",
                    r => r.HasOne<Permisos>().WithMany()
                        .HasForeignKey("IdPermiso")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("roles_permisos_id_permiso_fkey"),
                    l => l.HasOne<Roles>().WithMany()
                        .HasForeignKey("IdRol")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("roles_permisos_id_rol_fkey"),
                    j =>
                    {
                        j.HasKey("IdRol", "IdPermiso").HasName("roles_permisos_pkey");
                        j.ToTable("roles_permisos", "helpdesk");
                        j.HasIndex(new[] { "IdPermiso" }, "ix_roles_permisos_permiso");
                        j.IndexerProperty<Guid>("IdRol").HasColumnName("id_rol");
                        j.IndexerProperty<Guid>("IdPermiso").HasColumnName("id_permiso");
                    });
        });

        modelBuilder.Entity<Sedes>(entity =>
        {
            entity.HasKey(e => e.IdSede).HasName("sedes_pkey");

            entity.ToTable("sedes", "helpdesk");

            entity.Property(e => e.IdSede)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id_sede");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Direccion)
                .HasMaxLength(255)
                .HasColumnName("direccion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(150)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Servicios>(entity =>
        {
            entity.HasKey(e => e.IdServicio).HasName("servicios_pkey");

            entity.ToTable("servicios", "helpdesk");

            entity.HasIndex(e => e.IdCategoriaServicio, "ix_servicios_categoria");

            entity.HasIndex(e => e.IdEquipoResponsable, "ix_servicios_equipo");

            entity.HasIndex(e => e.Nombre, "servicios_nombre_key").IsUnique();

            entity.Property(e => e.IdServicio)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id_servicio");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.IdCategoriaServicio).HasColumnName("id_categoria_servicio");
            entity.Property(e => e.IdEquipoResponsable).HasColumnName("id_equipo_responsable");
            entity.Property(e => e.Nombre)
                .HasMaxLength(150)
                .HasColumnName("nombre");
            entity.Property(e => e.RequiereAprobacion).HasColumnName("requiere_aprobacion");

            entity.HasOne(d => d.IdCategoriaServicioNavigation).WithMany(p => p.Servicios)
                .HasForeignKey(d => d.IdCategoriaServicio)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_servicios_categoria");

            entity.HasOne(d => d.IdEquipoResponsableNavigation).WithMany(p => p.Servicios)
                .HasForeignKey(d => d.IdEquipoResponsable)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("servicios_id_equipo_responsable_fkey");
        });

        modelBuilder.Entity<TareasCambios>(entity =>
        {
            entity.HasKey(e => e.IdTareaCambio).HasName("tareas_cambios_pkey");

            entity.ToTable("tareas_cambios", "helpdesk");

            entity.HasIndex(e => new { e.IdResponsable, e.Estado }, "ix_tareas_cambios_responsable");

            entity.HasIndex(e => new { e.IdCambio, e.Orden }, "tareas_cambios_id_cambio_orden_key").IsUnique();

            entity.Property(e => e.IdTareaCambio)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id_tarea_cambio");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .HasDefaultValueSql("'pendiente'::character varying")
                .HasColumnName("estado");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaFinalizacion).HasColumnName("fecha_finalizacion");
            entity.Property(e => e.IdCambio).HasColumnName("id_cambio");
            entity.Property(e => e.IdResponsable).HasColumnName("id_responsable");
            entity.Property(e => e.Orden).HasColumnName("orden");
            entity.Property(e => e.Titulo)
                .HasMaxLength(255)
                .HasColumnName("titulo");

            entity.HasOne(d => d.IdCambioNavigation).WithMany(p => p.TareasCambios)
                .HasForeignKey(d => d.IdCambio)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tareas_cambios_id_cambio_fkey");

            entity.HasOne(d => d.IdResponsableNavigation).WithMany(p => p.TareasCambios)
                .HasForeignKey(d => d.IdResponsable)
                .HasConstraintName("tareas_cambios_id_responsable_fkey");
        });

        modelBuilder.Entity<Tickets>(entity =>
        {
            entity.HasKey(e => e.IdTicket).HasName("tickets_pkey");

            entity.ToTable("tickets", "helpdesk");

            entity.HasIndex(e => e.IdAreaSolicitante, "ix_tickets_area");

            entity.HasIndex(e => e.IdCreador, "ix_tickets_creador");

            entity.HasIndex(e => new { e.IdEquipoAsignado, e.Estado }, "ix_tickets_equipo_estado");

            entity.HasIndex(e => new { e.Estado, e.FechaCreacion }, "ix_tickets_estado_fecha");

            entity.HasIndex(e => e.IdPolitica, "ix_tickets_politica");

            entity.HasIndex(e => e.IdServicio, "ix_tickets_servicio");

            entity.HasIndex(e => new { e.IdSolicitante, e.FechaCreacion }, "ix_tickets_solicitante").IsDescending(false, true);

            entity.HasIndex(e => new { e.IdTecnicoAsignado, e.Estado }, "ix_tickets_tecnico_estado");

            entity.HasIndex(e => e.Numero, "tickets_numero_key").IsUnique();

            entity.Property(e => e.IdTicket)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id_ticket");
            entity.Property(e => e.Asunto)
                .HasMaxLength(255)
                .HasColumnName("asunto");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.Estado)
                .HasMaxLength(30)
                .HasDefaultValueSql("'nuevo'::character varying")
                .HasColumnName("estado");
            entity.Property(e => e.FechaActualizacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("fecha_actualizacion");
            entity.Property(e => e.FechaCancelacion).HasColumnName("fecha_cancelacion");
            entity.Property(e => e.FechaCierre).HasColumnName("fecha_cierre");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaLimiteResolucion).HasColumnName("fecha_limite_resolucion");
            entity.Property(e => e.FechaLimiteRespuesta).HasColumnName("fecha_limite_respuesta");
            entity.Property(e => e.FechaPrimeraRespuesta).HasColumnName("fecha_primera_respuesta");
            entity.Property(e => e.FechaResolucion).HasColumnName("fecha_resolucion");
            entity.Property(e => e.IdAreaSolicitante).HasColumnName("id_area_solicitante");
            entity.Property(e => e.IdCreador)
                .HasComment("Usuario autenticado que registra; puede ser distinto del solicitante si tiene permiso.")
                .HasColumnName("id_creador");
            entity.Property(e => e.IdEquipoAsignado).HasColumnName("id_equipo_asignado");
            entity.Property(e => e.IdPolitica).HasColumnName("id_politica");
            entity.Property(e => e.IdServicio).HasColumnName("id_servicio");
            entity.Property(e => e.IdSolicitante).HasColumnName("id_solicitante");
            entity.Property(e => e.IdTecnicoAsignado).HasColumnName("id_tecnico_asignado");
            entity.Property(e => e.MinutosPrimeraRespuesta).HasColumnName("minutos_primera_respuesta");
            entity.Property(e => e.MinutosResolucion)
                .HasComment("Copia del objetivo al crear el ticket; cambiar una politica no cambia el compromiso historico.")
                .HasColumnName("minutos_resolucion");
            entity.Property(e => e.MotivoCancelacion).HasColumnName("motivo_cancelacion");
            entity.Property(e => e.MotivoEspera).HasColumnName("motivo_espera");
            entity.Property(e => e.Numero)
                .ValueGeneratedOnAdd()
                .HasComment("Numero visible automatico. .NET puede mostrarlo como HD-000123. Puede tener saltos.")
                .UseIdentityAlwaysColumn()
                .HasColumnName("numero");
            entity.Property(e => e.Prioridad)
                .HasMaxLength(20)
                .HasColumnName("prioridad");
            entity.Property(e => e.Solucion).HasColumnName("solucion");
            entity.Property(e => e.Tipo)
                .HasMaxLength(20)
                .HasColumnName("tipo");
            entity.Property(e => e.Version)
                .HasDefaultValue(1)
                .HasComment("La API incrementa este valor y comprueba la version anterior para evitar sobrescrituras simultaneas.")
                .HasColumnName("version");

            entity.HasOne(d => d.IdAreaSolicitanteNavigation).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.IdAreaSolicitante)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tickets_id_area_solicitante_fkey");

            entity.HasOne(d => d.IdCreadorNavigation).WithMany(p => p.TicketsIdCreadorNavigation)
                .HasForeignKey(d => d.IdCreador)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tickets_id_creador_fkey");

            entity.HasOne(d => d.IdEquipoAsignadoNavigation).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.IdEquipoAsignado)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tickets_id_equipo_asignado_fkey");

            entity.HasOne(d => d.IdServicioNavigation).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.IdServicio)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tickets_id_servicio_fkey");

            entity.HasOne(d => d.IdSolicitanteNavigation).WithMany(p => p.TicketsIdSolicitanteNavigation)
                .HasForeignKey(d => d.IdSolicitante)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tickets_id_solicitante_fkey");

            entity.HasOne(d => d.IdTecnicoAsignadoNavigation).WithMany(p => p.TicketsIdTecnicoAsignadoNavigation)
                .HasForeignKey(d => d.IdTecnicoAsignado)
                .HasConstraintName("tickets_id_tecnico_asignado_fkey");

            entity.HasOne(d => d.IntegrantesEquipo).WithMany(p => p.Tickets)
                .HasForeignKey(d => new { d.IdEquipoAsignado, d.IdTecnicoAsignado })
                .HasConstraintName("tickets_id_equipo_asignado_id_tecnico_asignado_fkey");

            entity.HasOne(d => d.PoliticasAtencion).WithMany(p => p.Tickets)
                .HasPrincipalKey(p => new { p.IdPolitica, p.Prioridad })
                .HasForeignKey(d => new { d.IdPolitica, d.Prioridad })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tickets_id_politica_prioridad_fkey");

            entity.HasMany(d => d.IdActivo).WithMany(p => p.IdTicket)
                .UsingEntity<Dictionary<string, object>>(
                    "TicketsActivos",
                    r => r.HasOne<Activos>().WithMany()
                        .HasForeignKey("IdActivo")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("tickets_activos_id_activo_fkey"),
                    l => l.HasOne<Tickets>().WithMany()
                        .HasForeignKey("IdTicket")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("tickets_activos_id_ticket_fkey"),
                    j =>
                    {
                        j.HasKey("IdTicket", "IdActivo").HasName("tickets_activos_pkey");
                        j.ToTable("tickets_activos", "helpdesk");
                        j.HasIndex(new[] { "IdActivo" }, "ix_tickets_activos_activo");
                        j.IndexerProperty<Guid>("IdTicket").HasColumnName("id_ticket");
                        j.IndexerProperty<Guid>("IdActivo").HasColumnName("id_activo");
                    });
        });

        modelBuilder.Entity<TicketsArticulos>(entity =>
        {
            entity.HasKey(e => new { e.IdTicket, e.IdArticulo }).HasName("tickets_articulos_pkey");

            entity.ToTable("tickets_articulos", "helpdesk");

            entity.HasIndex(e => e.IdArticulo, "ix_tickets_articulos_articulo");

            entity.Property(e => e.IdTicket).HasColumnName("id_ticket");
            entity.Property(e => e.IdArticulo).HasColumnName("id_articulo");
            entity.Property(e => e.FechaVinculacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("fecha_vinculacion");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");

            entity.HasOne(d => d.IdArticuloNavigation).WithMany(p => p.TicketsArticulos)
                .HasForeignKey(d => d.IdArticulo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tickets_articulos_id_articulo_fkey");

            entity.HasOne(d => d.IdTicketNavigation).WithMany(p => p.TicketsArticulos)
                .HasForeignKey(d => d.IdTicket)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tickets_articulos_id_ticket_fkey");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.TicketsArticulos)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tickets_articulos_id_usuario_fkey");
        });

        modelBuilder.Entity<TiposActivos>(entity =>
        {
            entity.HasKey(e => e.IdTipoActivo).HasName("tipos_activos_pkey");

            entity.ToTable("tipos_activos", "helpdesk");

            entity.HasIndex(e => e.Nombre, "tipos_activos_nombre_key").IsUnique();

            entity.Property(e => e.IdTipoActivo)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id_tipo_activo");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(255)
                .HasColumnName("descripcion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Usuarios>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("usuarios_pkey");

            entity.ToTable("usuarios", "helpdesk");

            entity.HasIndex(e => e.IdArea, "ix_usuarios_area");

            entity.Property(e => e.IdUsuario)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id_usuario");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Apellidos)
                .HasMaxLength(150)
                .HasColumnName("apellidos");
            entity.Property(e => e.ClaveHash)
                .HasComment("Contrasena transformada con un algoritmo seguro desde .NET; nunca texto plano ni cifrado reversible.")
                .HasColumnName("clave_hash");
            entity.Property(e => e.Correo)
                .HasMaxLength(254)
                .HasColumnName("correo");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("fecha_registro");
            entity.Property(e => e.IdArea).HasColumnName("id_area");
            entity.Property(e => e.NombreUsuario)
                .HasMaxLength(100)
                .HasColumnName("nombre_usuario");
            entity.Property(e => e.Nombres)
                .HasMaxLength(100)
                .HasColumnName("nombres");
            entity.Property(e => e.Telefono)
                .HasMaxLength(40)
                .HasColumnName("telefono");

            entity.HasOne(d => d.IdAreaNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdArea)
                .HasConstraintName("usuarios_id_area_fkey");
        });

        modelBuilder.Entity<UsuariosRoles>(entity =>
        {
            entity.HasKey(e => new { e.IdUsuario, e.IdRol }).HasName("usuarios_roles_pkey");

            entity.ToTable("usuarios_roles", "helpdesk");

            entity.HasIndex(e => e.IdRol, "ix_usuarios_roles_rol");

            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.IdRol).HasColumnName("id_rol");
            entity.Property(e => e.FechaAsignacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("fecha_asignacion");

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.UsuariosRoles)
                .HasForeignKey(d => d.IdRol)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("usuarios_roles_id_rol_fkey");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.UsuariosRoles)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("usuarios_roles_id_usuario_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
