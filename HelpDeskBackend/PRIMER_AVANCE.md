# Guía del primer avance: estilo Lab11

Consultar primero la instalación en [DISTRIBUCION_EQUIPO.md](DISTRIBUCION_EQUIPO.md). El reparto entre integrantes sigue vigente. Este documento define el alcance de código que deben pasar desde la plantilla al repositorio del equipo.

## Qué copiar por entidad

1. `HelpDesk.Domain/Dtos/{Entidad}/`: cada DTO de salida tiene su archivo. Los datos de entrada y sus validaciones están directamente en el Command.
2. `HekDesk.Application/UseCases/{Entidad}/Commands/` y `Queries/`.
3. Su controlador y, si existe, el controlador de acciones indicado en el reparto.
4. Adaptar los nombres de los proyectos del equipo. No reemplazar Program ni la configuración compartida sin coordinar con Brahan.

Cada integrante incorpora los DTOs de sus entidades en Domain. Brahan incorpora los modelos y contratos compartidos de Domain, contexto, repositorio, UnitOfWork, servicios técnicos y configuración común. Diego incorpora autenticación y coordina sus contratos de seguridad. Los DTOs compartidos están en HelpDesk.Domain/Dtos/Common; los de autenticación en HelpDesk.Domain/Dtos/Autenticacion. Los DTOs pertenecen a Domain, como en Lab11; Application los utiliza mediante su referencia al proyecto Domain. Common ya no contiene DTOs.

## Datos de entrada directamente en el Command

Como en Lab11, cada Command es una clase que implementa IRequest y declara los campos que necesita. Ejemplo:

```csharp
public class CrearRolesCommand : IRequest<ResponseDto<RolesResponseDto>>
{
    [Required]
    [StringLength(10000)]
    public string Nombre { get; set; } = "";
    public string? Descripcion { get; set; }
}
```

El controlador recibe el Command y lo envía a MediatR. En actualizaciones y acciones con una clave en la URL, el controlador asigna esa clave antes de enviar la solicitud; la propiedad se marca JsonIgnore para que el cuerpo no pueda reemplazarla. El handler valida el propio request y devuelve el DTO de salida.

RegistroDto se conserva exclusivamente como entrada del alta inicial de administrador por consola. Los Commands de registro y login tienen sus propiedades directamente.

## Cómo se escribe un handler

El Command recibe los datos, el handler valida, consulta el repositorio, modifica la entidad, llama a Complete() y devuelve un ResponseDto tipado. El Query solo consulta y devuelve su DTO. Mantener try/catch en cada Handle. CancellationToken solo se declara en la firma que exige MediatR.

Ejemplo de persistencia usado en los Commands:

```csharp
var rol = await _unitOfWork.Repository<Roles>().GetByIdAsync(request.Keys);
if (rol is null)
    throw new BusinessException(404, "Registro no encontrado.");

rol.Descripcion = request.Descripcion;
await _unitOfWork.Complete();
```

El ejemplo es un fragmento del cuerpo del handler: conservar también validación, autorización, respuesta y manejo de errores del archivo real. No llamar a SaveChangesAsync desde Application.

## Inventario vigente

La entrega tiene **91 Commands, 75 Queries y 47 archivos de DTOs**. El total de comandos incluye el CRUD de las 36 tablas donde corresponde, más las acciones básicas; no significa que deban escribirse todos para cada entidad.

| Entidad | Commands incluidos | Queries |
|---|---|---|
| Activos | `ActualizarActivosCommand`, `CrearActivosCommand`, `EliminarActivosCommand` | 2 |
| Adjuntos | `ActualizarAdjuntosCommand`, `CrearAdjuntosCommand`, `EliminarAdjuntosCommand` | 2 |
| Aprobaciones | `ActualizarAprobacionesCommand`, `AprobarSolicitudCommand`, `CrearAprobacionesCommand`, `RechazarSolicitudCommand` | 2 |
| Areas | `ActualizarAreasCommand`, `CrearAreasCommand`, `EliminarAreasCommand` | 2 |
| ArticulosConocimiento | `ActualizarArticulosConocimientoCommand`, `CrearArticulosConocimientoCommand`, `EliminarArticulosConocimientoCommand` | 2 |
| AsignacionesActivos | `CrearAsignacionesActivosCommand`, `DevolverActivoCommand` | 2 |
| AsignacionesTickets | Solo consultas | 2 |
| Auditoria | Solo consultas | 2 |
| Autenticacion | `CambiarClaveCommand`, `IniciarSesionCommand`, `RegistrarCommand` | 0 |
| Cambios | `ActualizarCambiosCommand`, `CrearCambiosCommand` | 2 |
| CambiosTickets | `CrearCambiosTicketsCommand`, `EliminarCambiosTicketsCommand` | 2 |
| CargaTecnicos | Solo consultas | 1 |
| CategoriasConocimiento | `ActualizarCategoriasConocimientoCommand`, `CrearCategoriasConocimientoCommand`, `EliminarCategoriasConocimientoCommand` | 2 |
| CategoriasServicios | `ActualizarCategoriasServiciosCommand`, `CrearCategoriasServiciosCommand`, `EliminarCategoriasServiciosCommand` | 2 |
| Comentarios | `ActualizarComentariosCommand`, `CrearComentariosCommand` | 2 |
| Encuestas | `CrearEncuestasCommand` | 2 |
| EnviosNotificaciones | Solo consultas | 2 |
| EquiposSoporte | `ActualizarEquiposSoporteCommand`, `CrearEquiposSoporteCommand`, `EliminarEquiposSoporteCommand` | 2 |
| HistorialTickets | Solo consultas | 2 |
| IntegrantesEquipo | `ActualizarIntegrantesEquipoCommand`, `CrearIntegrantesEquipoCommand`, `EliminarIntegrantesEquipoCommand` | 2 |
| Notificaciones | `ActualizarNotificacionesCommand`, `CrearNotificacionesCommand`, `MarcarNotificacionLeidaCommand` | 2 |
| Permisos | `ActualizarPermisosCommand`, `CrearPermisosCommand`, `EliminarPermisosCommand` | 2 |
| PoliticasAtencion | `ActualizarPoliticasAtencionCommand`, `CrearPoliticasAtencionCommand`, `EliminarPoliticasAtencionCommand` | 2 |
| PreferenciasNotificaciones | `ActualizarPreferenciasNotificacionesCommand`, `CrearPreferenciasNotificacionesCommand`, `EliminarPreferenciasNotificacionesCommand` | 2 |
| Problemas | `ActualizarProblemasCommand`, `CrearProblemasCommand` | 2 |
| ProblemasTickets | `CrearProblemasTicketsCommand`, `EliminarProblemasTicketsCommand` | 2 |
| RecuperacionesClave | Solo consultas | 2 |
| Reportes | Solo consultas | 1 |
| ResumenTickets | Solo consultas | 1 |
| Roles | `ActualizarRolesCommand`, `CrearRolesCommand`, `EliminarRolesCommand` | 2 |
| RolesPermisos | `CrearRolesPermisosCommand`, `EliminarRolesPermisosCommand` | 2 |
| Sedes | `ActualizarSedesCommand`, `CrearSedesCommand`, `EliminarSedesCommand` | 2 |
| Servicios | `ActualizarServiciosCommand`, `CrearServiciosCommand`, `EliminarServiciosCommand` | 2 |
| TareasCambios | `ActualizarTareasCambiosCommand`, `CrearTareasCambiosCommand`, `EliminarTareasCambiosCommand` | 2 |
| Tickets | `ActualizarTicketsCommand`, `AsignarTicketCommand`, `CerrarTicketCommand`, `CrearTicketsCommand`, `IniciarTicketCommand`, `ResolverTicketCommand` | 2 |
| TicketsActivos | `CrearTicketsActivosCommand`, `EliminarTicketsActivosCommand` | 2 |
| TicketsArticulos | `ActualizarTicketsArticulosCommand`, `CrearTicketsArticulosCommand`, `EliminarTicketsArticulosCommand` | 2 |
| TiposActivos | `ActualizarTiposActivosCommand`, `CrearTiposActivosCommand`, `EliminarTiposActivosCommand` | 2 |
| Usuarios | `ActivarUsuarioCommand`, `ActualizarUsuariosCommand`, `CrearUsuariosCommand`, `DesactivarUsuarioCommand` | 2 |
| UsuariosRoles | `ActualizarUsuariosRolesCommand`, `CrearUsuariosRolesCommand`, `EliminarUsuariosRolesCommand` | 2 |

## Qué queda para después

No copiar los Commands retirados de versiones anteriores. Espera, reapertura, cancelación, reasignación, recuperación de contraseña, publicación de conocimiento, ciclo de ejecución de cambios, reintentos de correo y auditoría automática quedan para otra entrega. Los modelos de base de datos se mantienen para no cambiar el esquema compartido.

La guía de administrador explica el único paso de consola inicial. Para crear empleados se usa el registro de la API; para asignar otros roles, UsuariosRoles con una cuenta administrativa.
