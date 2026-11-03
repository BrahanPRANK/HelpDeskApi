# Primera versión simplificada de HelpDesk

Esta entrega sigue el estilo del laboratorio 11 y conserva las cuatro capas. `HekDesk.Application` es el nombre físico del proyecto; el namespace es `HelpDesk.Application`. La base PostgreSQL sigue usando las mismas 36 tablas y 2 vistas; no hace falta recrearla por esta reorganización.

## Instalación y administrador inicial

Seguir los pasos de [DISTRIBUCION_EQUIPO.md](DISTRIBUCION_EQUIPO.md): PostgreSQL, SQL inicial, User Secrets y compilación. Con la API detenida y desde `HelpDeskBackend`, crear una sola vez por base el administrador:

```powershell
.\scripts\Crear-Administrador.ps1 -NombreUsuario admin -Correo tu-correo@dominio.com
dotnet run --project HelpDesk.Api --launch-profile http
```

El script pide una contraseña oculta, la guarda como hash y asigna el rol administrador. No es el procedimiento para cada usuario ni debe ejecutarse en cada arranque. Si ya existe un administrador, se utilizan las operaciones administrativas de la API.

Abrir `http://localhost:5014/`, iniciar sesión en `POST /api/auth/login` y pegar `data.token` en Authorize. El registro público `POST /api/auth/registro` solo crea empleados. El administrador completa su área en Usuarios y asigna otros roles en UsuariosRoles. Las altas administrativas de Usuarios no asignan roles automáticamente.

## Organización y persistencia

- `Application/UseCases/{Entidad}/Commands`: escrituras y sus handlers, con lógica propia.
- `Application/UseCases/{Entidad}/Queries`: consultas y sus handlers, con filtros LINQ y proyección.
- `Domain/Dtos/{Entidad}`: DTOs de salida, cada uno en su propio archivo. Los campos de entrada están directamente en los Commands, con sus atributos de validación, como en Lab11.
- `Domain/Dtos/Common`: ResponseDto, PaginaDto, ListOptions y respuestas compartidas, separados.
- `Application/Common`: validación básica; `Application/Security`: reglas de acceso y alta inicial.
- `Domain/Ports`: interfaces de repositorio, UnitOfWork y seguridad.
- `Infrastructure`: contexto, repositorios, UnitOfWork, JWT y hash de contraseñas.

El repositorio ofrece GetAllAsync, GetByIdAsync, FirstOrDefaultAsync, AddAsync, Update, Delete y Query. Query permite escribir consultas LINQ en el caso de uso. Application utiliza las extensiones ToListAsync y CountAsync de EF Core para ejecutar esas consultas. Las interfaces y los modelos siguen en Domain.

UnitOfWork solo obtiene repositorios y confirma cambios con `await _unitOfWork.Complete()`. EF ejecuta SaveChangesAsync internamente y guarda las modificaciones de esa llamada en una transacción. No hay un motor de auditoría ni envoltorios de transacciones en UnitOfWork. Las entidades cargadas mediante GetByIdAsync quedan seguidas por EF, por lo que sus modificaciones se guardan con Complete() sin llamar obligatoriamente a Update().

La excepción al repositorio sin parámetros son RolesPermisos, TicketsActivos, ProblemasTickets y CambiosTickets: el scaffolding las representa como diccionarios compartidos y por eso Repository recibe el nombre del conjunto en esos casos.

En los handlers, CancellationToken aparece únicamente como parámetro obligatorio de Handle. No se comprueba ni se pasa a las operaciones. Cada Handle maneja BusinessException y devuelve un mensaje genérico ante errores inesperados; el middleware sirve como respaldo. No se utiliza ILogger en el código de la plantilla. No hay un servicio CRUD genérico que ejecute los casos de uso.

## CRUD por recurso

Los listados usan pagina, tamano (1–100), buscar y los filtros aplicables. Las relaciones con clave compuesta reciben ambas claves. Las restricciones de PostgreSQL y las reglas de acceso siguen vigentes.

| Recurso | Claves de ruta | Métodos |
|---|---|---|
| Activos | IdActivo | GET listado, GET por clave, POST, PUT, DELETE |
| Adjuntos | IdAdjunto | GET listado, GET por clave, POST, PUT, DELETE |
| Aprobaciones | IdAprobacion | GET listado, GET por clave, POST, PUT |
| Areas | IdArea | GET listado, GET por clave, POST, PUT, DELETE |
| ArticulosConocimiento | IdArticulo | GET listado, GET por clave, POST, PUT, DELETE |
| AsignacionesActivos | IdAsignacion | GET listado, GET por clave, POST |
| AsignacionesTickets | IdAsignacionTicket | GET listado, GET por clave |
| Auditoria | IdAuditoria | GET listado, GET por clave |
| Cambios | IdCambio | GET listado, GET por clave, POST, PUT |
| CargaTecnicos | IdEquipoAsignado | GET listado |
| CategoriasConocimiento | IdCategoriaConocimiento | GET listado, GET por clave, POST, PUT, DELETE |
| CategoriasServicios | IdCategoriaServicio | GET listado, GET por clave, POST, PUT, DELETE |
| Comentarios | IdComentario | GET listado, GET por clave, POST, PUT |
| Encuestas | IdEncuesta | GET listado, GET por clave, POST |
| EnviosNotificaciones | IdEnvio | GET listado, GET por clave |
| EquiposSoporte | IdEquipo | GET listado, GET por clave, POST, PUT, DELETE |
| HistorialTickets | IdHistorial | GET listado, GET por clave |
| IntegrantesEquipo | IdEquipo, IdUsuario | GET listado, GET por clave, POST, PUT, DELETE |
| Notificaciones | IdNotificacion | GET listado, GET por clave, POST, PUT |
| Permisos | IdPermiso | GET listado, GET por clave, POST, PUT, DELETE |
| PoliticasAtencion | IdPolitica | GET listado, GET por clave, POST, PUT, DELETE |
| PreferenciasNotificaciones | IdUsuario, TipoEvento | GET listado, GET por clave, POST, PUT, DELETE |
| Problemas | IdProblema | GET listado, GET por clave, POST, PUT |
| RecuperacionesClave | IdRecuperacion | GET listado, GET por clave |
| ResumenTickets | IdTicket | GET listado |
| Roles | IdRol | GET listado, GET por clave, POST, PUT, DELETE |
| Sedes | IdSede | GET listado, GET por clave, POST, PUT, DELETE |
| Servicios | IdServicio | GET listado, GET por clave, POST, PUT, DELETE |
| TareasCambios | IdTareaCambio | GET listado, GET por clave, POST, PUT, DELETE |
| Tickets | IdTicket | GET listado, GET por clave, POST, PUT |
| TicketsArticulos | IdTicket, IdArticulo | GET listado, GET por clave, POST, PUT, DELETE |
| TiposActivos | IdTipoActivo | GET listado, GET por clave, POST, PUT, DELETE |
| Usuarios | IdUsuario | GET listado, GET por clave, POST, PUT |
| UsuariosRoles | IdUsuario, IdRol | GET listado, GET por clave, POST, DELETE |
| RolesPermisos | IdRol, IdPermiso | GET listado, GET por clave, POST, DELETE |
| TicketsActivos | IdTicket, IdActivo | GET listado, GET por clave, POST, DELETE |
| ProblemasTickets | IdProblema, IdTicket | GET listado, GET por clave, POST, DELETE |
| CambiosTickets | IdCambio, IdTicket | GET listado, GET por clave, POST, DELETE |


## Operaciones adicionales conservadas

- Tickets: asignar, iniciar, resolver y cerrar.
- Usuarios: activar y desactivar, conservando un administrador activo.
- Aprobaciones: aprobar y rechazar por el aprobador asignado; se conserva el orden de decisión.
- AsignacionesActivos: devolver un activo antes de volver a asignarlo.
- Notificaciones: marcar-leida por su destinatario.
- Autenticación: registro, login y cambiar-clave.
- Reportes: GET /api/Reportes/indicadores y consultas de las dos vistas.

Flujo básico: configurar sede, área, equipo e integrantes, servicio y política de atención; asignar área al empleado; crear ticket → asignar técnico → iniciar → resolver → cerrar. Para atender como técnico o decidir como aprobador se necesita el rol y los permisos correspondientes.

Cada acción de ticket recibe la última version consultada. Ejemplo de resolver:

```json
{"version": 3, "motivo": "Se corrigió la configuración de red."}
```

La primera asignación completa el registro inicial del ticket. Esta versión no ofrece reasignación. Las acciones actualizan el historial y la versión del ticket, sin generar avisos automáticamente. El cierre finaliza la asignación vigente. Los tiempos se calculan en minutos corridos.

## Seguridad y límites

Se conservan JWT HS256 de 30 minutos, contraseñas con PasswordHasher, comprobación de cuenta activa, roles y permisos, aislamiento de tickets por usuario y protección de datos sensibles mediante DTOs. Cambiar la contraseña invalida los tokens anteriores. Registro y login tienen un límite de intentos por IP.

Para una segunda versión quedan: recuperación de contraseña, espera/reanudación/reapertura/cancelación y reasignación de tickets, programación/ejecución/finalización de cambios, publicación/archivo de conocimiento, reintentos de correo y auditoría automática. Las tablas, modelos y consultas correspondientes se conservan para evolucionar después; pueden estar vacíos. Los artículos y cambios nuevos se administran como borradores. Vincular un artículo a un ticket exige que ya esté publicado; ese flujo completo queda para v2.

Tampoco se incluye frontend, proveedor de correo, subida física de archivos, calendario laboral ni refresh tokens. Adjuntos administra metadatos. El flujo básico de tickets no se bloquea por el motor avanzado de aprobaciones, que queda para otra entrega; las aprobaciones se administran por separado.

## Scaffolding y pruebas

Los modelos ya están incluidos. Regenerar solo si cambia el SQL, mediante scripts/Actualizar-Modelo.ps1, conservando HelpDeskDbContext.Custom.cs con las correcciones de relaciones y concurrencia.

```powershell
dotnet build HelpDeskBackend.sln
dotnet run --project tests/HelpDesk.IntegrationTests
```

La suite prueba las consultas de recursos, CRUD, permisos, aislamiento, JWT, hash, cambio de contraseña, concurrencia y el flujo básico de tickets. Crea una base temporal helpdesk_test_* y la elimina al terminar; necesita permiso para crearla y no modifica helpdeskdb.
