using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
namespace HelpDesk.Application.Security;

public sealed class AccessRules(IUnitOfWork _unitOfWork, ICurrentUser actor)
{
    public bool Admin => actor.HasPermission("usuarios.administrar");
    public Guid UserId => actor.Id;
    public void Permission(string code)
    { if (!actor.IsAuthenticated || !actor.HasPermission(code)) throw new BusinessException(403,"No tiene permiso para esta operacion."); }
    public IQueryable<Tickets> TicketsScope()
    {
        var query = _unitOfWork.Repository<Tickets>().Query();
        if (actor.HasPermission("tickets.consultar_todos")) return query;
        var user = actor.Id;
        var teams = _unitOfWork.Repository<IntegrantesEquipo>().Query().Where(m => m.IdUsuario == user && m.Activo && m.EsCoordinador).Select(m=>m.IdEquipo);
        var own = actor.HasPermission("tickets.consultar_propios");
        var assigned = actor.HasPermission("tickets.consultar_asignados");
        var team = actor.HasPermission("tickets.consultar_equipo");
        return query.Where(t => (own && t.IdSolicitante == user) || (assigned && t.IdTecnicoAsignado == user) || (team && teams.Contains(t.IdEquipoAsignado)));
    }
    public bool Internal => actor.HasPermission("comentarios.internos");
    public IQueryable<T> Scope<T>(IQueryable<T> query) where T : class
    {
        if (!actor.IsAuthenticated) throw new BusinessException(401,"Inicie sesion.");
        var id=actor.Id;
        var visible=TicketsScope().Select(t=>t.IdTicket);
        // El administrador tampoco recibe tokens, hashes ni rutas: los DTO los excluyen.
        if (Admin) return query;
        object scoped = query;
        switch (query)
        {
            case IQueryable<Tickets> _: scoped=TicketsScope(); break;
            case IQueryable<Comentarios> q: scoped=q.Where(x=>visible.Contains(x.IdTicket) && (!x.EsInterno || Internal)); break;
            case IQueryable<Adjuntos> q:
                var publicComments=_unitOfWork.Repository<Comentarios>().Query().Where(c=>!c.EsInterno).Select(c=>c.IdComentario);
                scoped=q.Where(x=>visible.Contains(x.IdTicket) && (Internal || (!x.EsInterno && (x.IdComentario==null || publicComments.Contains(x.IdComentario.Value))))); break;
            case IQueryable<HistorialTickets> q: scoped=q.Where(x=>visible.Contains(x.IdTicket));break;
            case IQueryable<AsignacionesTickets> q: scoped=q.Where(x=>visible.Contains(x.IdTicket));break;
            case IQueryable<Encuestas> q: scoped=q.Where(x=>x.IdUsuario==id);break;
            case IQueryable<Notificaciones> q: scoped=q.Where(x=>x.IdUsuario==id);break;
            case IQueryable<PreferenciasNotificaciones> q: scoped=q.Where(x=>x.IdUsuario==id);break;
            case IQueryable<Usuarios> q: scoped=q.Where(x=>x.IdUsuario==id);break;
            case IQueryable<Aprobaciones> q: scoped=q.Where(x=>x.IdAprobador==id || x.IdSolicitante==id);break;
            case IQueryable<ArticulosConocimiento> q:
                scoped=q.Where(x=>(x.Estado=="publicado" && (!x.EsInterno || Internal)) || x.IdAutor==id);break;
            case IQueryable<ResumenTickets> q: scoped=q.Where(x=>x.IdTicket!=null && visible.Contains(x.IdTicket.Value));break;
            case IQueryable<CargaTecnicos> q:
                Permission("reportes.consultar_equipo");
                var teams=_unitOfWork.Repository<IntegrantesEquipo>().Query().Where(m=>m.IdUsuario==id && m.Activo && m.EsCoordinador).Select(m=>m.IdEquipo);
                scoped=q.Where(x=>x.IdEquipoAsignado!=null && teams.Contains(x.IdEquipoAsignado.Value));break;
            case IQueryable<Servicios> q: scoped=q.Where(x=>x.Activo);break;
            case IQueryable<Sedes> q: scoped=q.Where(x=>x.Activo);break;
            case IQueryable<Areas> q: scoped=q.Where(x=>x.Activo);break;
            case IQueryable<CategoriasServicios> q: scoped=q.Where(x=>x.Activo);break;
            case IQueryable<CategoriasConocimiento> q: scoped=q.Where(x=>x.Activo);break;
            case IQueryable<TiposActivos> q: scoped=q.Where(x=>x.Activo);break;
            case IQueryable<EquiposSoporte> q: scoped=q.Where(x=>x.Activo);break;
            case IQueryable<PoliticasAtencion> q: scoped=q.Where(x=>x.Activo);break;
            case IQueryable<Activos> _: Permission("activos.consultar");break;
            case IQueryable<Problemas> q: Permission("problemas.gestionar");scoped=q.Where(x=>x.IdResponsable==id);break;
            case IQueryable<Cambios> q: Permission("cambios.gestionar");scoped=q.Where(x=>x.IdResponsable==id || x.IdSolicitante==id);break;
            case IQueryable<TareasCambios> q: Permission("cambios.gestionar");scoped=q.Where(x=>x.IdResponsable==id);break;
            default: throw new BusinessException(403,"Este recurso requiere administracion.");
        }
        return (IQueryable<T>)scoped;
    }
    public void EnsureRead<T>(T entity) where T : class
    {
        if (Admin) return;
        // Para un registro individual se comprueba pertenencia mediante los mismos IDs del listado.
        var ticketId=entity switch { Tickets t=>t.IdTicket, Comentarios c=>c.IdTicket, Adjuntos a=>a.IdTicket,
            HistorialTickets h=>h.IdTicket, AsignacionesTickets a=>a.IdTicket, _=>Guid.Empty };
        if (ticketId!=Guid.Empty)
        {
            if (!TicketsScope().Any(t=>t.IdTicket==ticketId)) throw new BusinessException(404,"Registro no encontrado.");
            if (entity is Comentarios { EsInterno:true } && !Internal) throw new BusinessException(404,"Registro no encontrado.");
            if (entity is Adjuntos a && !Internal && (a.EsInterno || (a.IdComentario.HasValue && _unitOfWork.Repository<Comentarios>().Query().Any(c=>c.IdComentario==a.IdComentario && c.EsInterno))))
                throw new BusinessException(404,"Registro no encontrado.");
            return;
        }
        // Una consulta por clave ya materializada no debe abrir un recurso que el listado oculta.
        var allowed=entity switch
        {
            Usuarios x=>x.IdUsuario==actor.Id, Encuestas x=>x.IdUsuario==actor.Id,
            Notificaciones x=>x.IdUsuario==actor.Id, PreferenciasNotificaciones x=>x.IdUsuario==actor.Id,
            Aprobaciones x=>x.IdAprobador==actor.Id || x.IdSolicitante==actor.Id,
            ArticulosConocimiento x=>(x.Estado=="publicado" && (!x.EsInterno || Internal)) || x.IdAutor==actor.Id,
            Servicios x=>x.Activo, Sedes x=>x.Activo, Areas x=>x.Activo,
            CategoriasServicios x=>x.Activo, CategoriasConocimiento x=>x.Activo, TiposActivos x=>x.Activo,
            EquiposSoporte x=>x.Activo, PoliticasAtencion x=>x.Activo,
            Activos _=>actor.HasPermission("activos.consultar"),
            Problemas x=>actor.HasPermission("problemas.gestionar") && x.IdResponsable==actor.Id,
            Cambios x=>actor.HasPermission("cambios.gestionar") && (x.IdResponsable==actor.Id || x.IdSolicitante==actor.Id),
            TareasCambios x=>actor.HasPermission("cambios.gestionar") && x.IdResponsable==actor.Id,
            _=>false
        };
        if (!allowed) throw new BusinessException(404,"Registro no encontrado.");
    }
    public void EnsureWrite(string name,string operation)
    {
        if (Admin) return;
        var permission = (name,operation) switch
        {
            ("Tickets","create")=>"tickets.crear",
            ("Tickets","update")=>"tickets.crear",
            ("Comentarios","create")=>"comentarios.responder",
            ("Encuestas","create")=>"encuestas.responder_propias",
            ("PreferenciasNotificaciones",_)=>"notificaciones.propias",
            _=>null
        };
        if (permission is null) throw new BusinessException(403,"La administracion de este recurso requiere un administrador.");
        Permission(permission);
    }
    public void EnsureWork(Tickets ticket)
    { Permission("tickets.atender");EnsureRead(ticket); }
}
