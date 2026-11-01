# Primera versión de HelpDesk: CQRS, JWT y CRUD

La solución conserva API, Application, Domain e Infrastructure. `HekDesk.Application` mantiene su nombre de proyecto; su código utiliza el namespace `HelpDesk.Application`.

## Cómo iniciar

1. Abrir `HelpDeskBackend.sln` en Rider.
2. La conexión `ConnectionStrings:DefaultConnection` y la clave JWT están configuradas en User Secrets de la API. No se incluyen credenciales en el código.
3. Crear el primer administrador desde la carpeta de la solución:

```powershell
.\scripts\Crear-Administrador.ps1 -NombreUsuario admin -Correo tu-correo@dominio.com
```

El script pide una clave oculta de al menos 10 caracteres. Solo permite inicializar el administrador cuando todavía no existe uno. No hay contraseña predeterminada ni endpoint público de administrador.

4. Ejecutar la API:

```powershell
dotnet run --project HelpDesk.Api --launch-profile http
```

5. Abrir `http://localhost:5014/` para Swagger. Iniciar sesión con `POST /api/auth/login` y pegar el token en **Authorize**.

## Orden para probar el negocio

Crear sede y área; asignar un área al empleado; crear equipo y sus integrantes; configurar las políticas de atención; crear un servicio. Después registrar un ticket.

El registro público asigna únicamente el rol empleado y no permite elegir área, roles ni permisos. El administrador completa el área desde Usuarios. Las altas administrativas de usuarios requieren asignar los roles mediante UsuariosRoles.

El administrador configura la plataforma. Para trabajar como técnico, coordinador o aprobador se le asigna también ese rol expresamente. Un técnico puede tener además el rol coordinador.

## Qué significa CQRS aquí

Cada recurso tiene comandos de escritura y consultas de lectura con sus handlers en Application. Los controladores llaman a MediatR y devuelven el resultado. Repository y Unit of Work implementan la persistencia en Infrastructure.

Ejemplo: `CrearSedesCommand -> CrearSedesCommandHandler -> IUnitOfWork -> GenericRepository -> PostgreSQL`.

Cada comando y consulta implementa su caso de uso en su propio handler: validaciones de negocio, filtros LINQ, proyección al DTO y guardado cuando corresponde. No hay CrudService ni clases Definition. La seguridad compartida sigue centralizada para mantener iguales los controles de acceso.

## DTOs y respuestas

Los DTOs están en `Application/UseCases/{Recurso}/Dtos`. Los de entrada no incluyen todos los campos de la tabla: autores, hashes, estados controlados y fechas se establecen en el servidor.

Formato de respuesta:

```json
{"statusCode": 200, "data": {"items": [], "pagina": 1, "tamano": 20, "total": 0}, "mensaje": null}
```

Los listados aceptan `pagina`, `tamano` (1 a 100), `buscar` y, cuando corresponde, `estado` o `idTicket`. Las rutas por clave compuesta incluyen ambas claves, en el orden de la tabla siguiente.

## Endpoints por recurso

El CRUD se ofrece donde tiene sentido. Las relaciones sin datos editables se agregan y quitan; no tienen un PUT ficticio. Historial y auditoría se conservan; las vistas se consultan.

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

Las rutas tienen el prefijo `/api/`. Los roles y permisos concretos se comprueban en el servidor. Las operaciones administrativas no se habilitan por tener cualquier token.

## Acciones adicionales

- Tickets: `asignar`, `iniciar`, `esperar`, `reanudar`, `resolver`, `cerrar`, `reabrir`, `cancelar`.
- Aprobaciones: `aprobar`, `rechazar`, en orden y solo por el aprobador asignado.
- AsignacionesActivos: `devolver`, conservando las asignaciones anteriores.
- ArticulosConocimiento: `publicar`, `archivar`.
- Cambios: `programar`, `ejecutar`, `finalizar`, con aprobación y tareas completas cuando corresponda.
- Usuarios: `activar`, `desactivar`; se conserva al menos un administrador activo.
- Notificaciones: `marcar-leida`, solo para el destinatario.
- EnviosNotificaciones: `reintentar`, dentro del límite de intentos.
- Reportes: `GET /api/Reportes/indicadores`, además de las dos vistas.

Ejemplo de acción: `POST /api/Tickets/{id}/resolver`:

```json
{"version": 3, "motivo": "Se corrigió la configuración de red."}
```

La versión debe ser la obtenida en la última consulta. Una modificación concurrente devuelve 409. Para asignar se incluyen también `idEquipo` e `idTecnico`. La asignación, el historial y el ticket se guardan juntos.

## Autenticación y middleware

- JWT firmado con HS256, duración de 30 minutos y validación de firma, emisor, audiencia y vencimiento.
- Contraseñas con `PasswordHasher`; no se admiten claves heredadas en texto plano.
- Cuenta activa, roles y permisos se vuelven a verificar en cada petición autenticada.
- Cambiar o restablecer la contraseña invalida los tokens anteriores de esa cuenta.
- Registro, login y restablecimiento tienen un límite de 20 peticiones por minuto por IP en esta instancia.
- Middleware central de excepciones: errores controlados 400/401/403/404/409 y errores inesperados 500 con referencia de seguimiento, sin detalles internos en la respuesta.
- Los DTOs generales no exponen hashes, resúmenes de tokens, rutas internas ni errores del proveedor de correo.

La recuperación se emite mediante `POST /api/auth/recuperacion/{idUsuario}`, solo para administración. Devuelve un token temporal una única vez para entregarlo por un canal verificado. `POST /api/auth/restablecer-clave` recibe `token` y `claveNueva`. No se afirma enviar un correo: todavía no hay proveedor de correo integrado.

## Límites de esta primera entrega

- No hay frontend, subida/descarga física de archivos ni proveedor de correo. Adjuntos maneja metadatos; los correos no se envían automáticamente.
- Los plazos SLA son minutos corridos. No hay calendarios laborales ni pausas que suspendan el reloj.
- Se permite reabrir un ticket resuelto antes de confirmar el cierre; no se reabren tickets ya cerrados. Los plazos originales se conservan.
- La administración de catálogos, relaciones e inventario usa permisos administrativos. El personal tiene las consultas y acciones operativas autorizadas; ampliar la delegación requiere definir permisos adicionales.
- No se implementó refresh token ni un proveedor OAuth/OIDC externo. Este login local con JWT corresponde al proyecto académico.
- La auditoría registra operación, entidad, actor y fecha sin copiar valores sensibles. No sustituye un sistema externo inmutable de auditoría.
- Los dos índices de usuario y correo con `lower(...)` siguen en PostgreSQL aunque EF no los represente.

## Regeneración del modelo

El esquema se administra con SQL y el scaffolding se regenera mediante `scripts/Actualizar-Modelo.ps1`. Las correcciones y tokens de concurrencia están en `HelpDeskDbContext.Custom.cs` para que no se pierdan al regenerar.

El scaffolding interpretó los índices únicos parciales de asignaciones como relaciones 1:1; el archivo parcial corrige esas relaciones a 1:N conservando la restricción SQL de una asignación vigente.

## Verificación reproducible

```powershell
dotnet build HelpDeskBackend.sln
dotnet run --project tests/HelpDesk.IntegrationTests
```

Las pruebas requieren permiso para crear una base temporal en el mismo servidor. Crean una base con prefijo `helpdesk_test_`, aplican únicamente el esquema y sus catálogos, levantan una API temporal en un puerto libre y eliminan esa base al terminar. No insertan ni modifican datos en `helpdeskdb`.

Incluyen consulta de los 38 recursos, CRUD, permisos, aislamiento por usuario, JWT inválido, contraseñas, recuperación, flujo de tickets, notas internas, concurrencia, aprobaciones, activos y cambios.

Total de rutas HTTP declaradas en esta versión: **178**.

## Organizacion de procesos actualizada

Se retiro la carpeta Workflows. Cada accion tiene su Command y Handler en UseCases/{Recurso}/Commands; por ejemplo ResolverTicketCommand contiene sus validaciones, actualizacion, historial y notificacion. Las consultas de indicadores estan en UseCases/Reportes/Queries. Las validaciones de negocio del CRUD estan en cada handler; se retiro Common/ReglasRecursos.cs. Los controladores de acciones envian comandos concretos y conservan las mismas rutas HTTP.

## Responsabilidad unica en los casos de uso

Cada handler tiene la responsabilidad de ejecutar un caso de uso concreto. CrearSedes no cambia cuando se modifica la forma de resolver un ticket. El controlador solo traduce la peticion HTTP y envia el comando; Infrastructure se ocupa del acceso a PostgreSQL. No es necesario que una clase tenga una sola linea o un solo metodo para cumplir responsabilidad unica. Se conserva codigo comun para contratos de respuesta, validacion basica y seguridad transversal; no para decidir ni ejecutar el CRUD de distintas entidades.

Los handlers de autenticacion tambien contienen sus propias operaciones. AuthService queda limitado al alta inicial de un administrador por consola. JWT, hashing y persistencia siguen detras de interfaces, sin copiar algoritmos criptograficos a los casos de uso.

## Confirmacion de cambios y errores por handler

Todos los comandos y consultas contienen try/catch en su Handle. BusinessException produce un ResponseDto con el estado esperado; OperationCanceledException se propaga; los errores inesperados se registran con ILogger y devuelven un mensaje generico 500. El middleware queda como respaldo para excepciones fuera de los handlers.

Las entidades se consultan y modifican mediante _unitOfWork.Repository<T>(). El handler confirma las escrituras con await _unitOfWork.Complete(ct); Complete ejecuta SaveChangesAsync internamente en Infrastructure, conserva la auditoria y traduce conflictos de persistencia. Las consultas no invocan Complete. Los procesos que necesitan varios guardados conservan una transaccion que se revierte antes de devolver un error.
