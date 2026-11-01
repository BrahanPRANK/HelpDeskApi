# Distribución del proyecto HelpDesk entre los ocho integrantes

**Fecha:** 1 de noviembre de 2026. **Ubicación:** `HelpDeskBackend`. **Alcance:** 36 tablas, 2 vistas y sus casos de uso.

Esta asignación propone responsables por grupos relacionados. Cada integrante revisa, comprende, completa y sube el código de su grupo. No se distribuyen tablas al azar ni se promete la misma carga por número de entidades: autenticación y tickets requieren más coordinación.

## Guía inicial: instalar y ejecutar el proyecto en otra computadora

**Leer esta sección antes del reparto de entidades.** Cada integrante configura su propia base local y sus propios secretos. Los ejemplos de terminal son para PowerShell en Windows.

### 1. Qué instalar

| Requisito | Qué necesitamos | Para qué sirve |
|---|---|---|
| .NET SDK | Versión 10.0, incluyendo herramientas de desarrollo; no basta instalar el Runtime | Compilar y ejecutar los cuatro proyectos. |
| PostgreSQL | Versión 16 o superior, con el servicio iniciado | Ejecutar la base local `helpdeskdb`. |
| pgAdmin 4 | Incluido como opción del instalador de PostgreSQL, o instalado aparte | Crear la base y ejecutar el SQL con interfaz gráfica. |
| Rider o Visual Studio | Con soporte de .NET 10; también sirve una terminal | Abrir `HelpDeskBackend.sln` y trabajar en el código. |
| Git | Cliente instalado | Clonar, crear ramas y subir cambios. |
| dotnet-ef | Herramienta 10.0.13; necesaria para scaffolding | Generar modelos y contexto desde PostgreSQL. |

Descargas oficiales: [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0), [PostgreSQL para Windows](https://www.postgresql.org/download/windows/), [pgAdmin](https://www.pgadmin.org/download/) y [Git](https://git-scm.com/downloads).

En el instalador de PostgreSQL, anotar el usuario, la contraseña y el puerto elegidos. `localhost`, puerto `5432` y usuario `postgres` son valores habituales, pero deben coincidir con la instalación de cada uno. La contraseña maestra de pgAdmin no necesariamente es la contraseña de PostgreSQL.

Comprobar en una terminal nueva:

```powershell
dotnet --list-sdks
git --version
```

Debe aparecer un SDK `10.0.xxx`. Si no aparece, instalar el SDK y volver a abrir la terminal.

### 2. Abrir la solución y restaurar las librerías

Obtener el proyecto del repositorio o de la carpeta compartida acordada. No se fija una URL de Git porque el remoto aún debe confirmarse con el equipo.

Entrar en la carpeta que contiene `HelpDeskBackend.sln`. Cambiar esta ruta si el proyecto se descargó en otro lugar:

```powershell
cd C:UsersASUSDocumentsHelpDeskHelpDeskBackend
dotnet restore HelpDeskBackend.sln
dotnet build HelpDeskBackend.sln
```

`dotnet restore` instala las dependencias indicadas en los `.csproj`: EF Core, PostgreSQL/Npgsql, MediatR, JWT, Swagger y logging. **No hace falta agregar los paquetes uno por uno ni actualizar sus versiones.** Esta compilación no inicia la API ni necesita todavía consultar la base.

La carpeta `HekDesk.Application` está escrita así en la solución actual. No renombrarla individualmente: afectaría referencias del equipo.

### 3. Crear la base de datos local

1. Abrir pgAdmin y conectarse al servidor PostgreSQL instalado en la computadora.
2. En **Databases**, elegir **Create → Database** y crear `helpdeskdb`.
3. Seleccionar esa base, abrir **Query Tool** y cargar `001_helpdesk_36_tablas_es.sql`.
4. Ejecutar **todo el archivo**. El script crea el esquema `helpdesk`, sus 36 tablas, 2 vistas y catálogos iniciales.
5. Actualizar el árbol de pgAdmin: **Schemas → helpdesk → Tables / Views**.

El archivo SQL actualmente está en la carpeta superior `HelpDesk`, junto a `HelpDeskBackend`. Si el repositorio solo incluye la solución, pedirle a Brahan ese mismo archivo SQL; no reconstruir tablas a mano ni ejecutar las versiones anteriores de 25 o 52 tablas.

Estructura de referencia:

```text
HelpDesk/
├── 001_helpdesk_36_tablas_es.sql
└── HelpDeskBackend/
    ├── HelpDeskBackend.sln
    ├── HelpDesk.Api/
    ├── HekDesk.Application/
    ├── HelpDesk.Domain/
    ├── HelpDesk.Infrastructure/
    └── scripts/
```

**Este SQL se ejecuta una sola vez en una base vacía.** Si el esquema ya existe, no volver a ejecutarlo ni eliminarlo para corregir un error. Consultar al responsable del esquema. Los cambios posteriores se coordinan mediante nuevos scripts.

Comprobar desde Query Tool, conectado a `helpdeskdb`:

```sql
SELECT table_type, COUNT(*) AS cantidad
FROM information_schema.tables
WHERE table_schema = 'helpdesk'
GROUP BY table_type;
```

Resultado esperado: `BASE TABLE = 36` y `VIEW = 2`. Crear solamente la base vacía no es suficiente para el scaffolding ni para probar endpoints.

### 4. Configurar conexión y JWT en User Secrets

**Este paso es obligatorio en cada computadora.** Los secretos de Brahan no viajan por Git. No copiar contraseñas al código ni al documento.

Desde la carpeta de la solución:

```powershell
dotnet user-secrets init --project HelpDesk.Api
```

El proyecto ya contiene un identificador de User Secrets; el comando lo conserva si existe. Los valores se guardan fuera del repositorio, en el perfil local del usuario. Es configuración de desarrollo; no es un almacén cifrado para producción.

Ejecutar este bloque completo para introducir la contraseña de PostgreSQL de forma oculta y generar una clave JWT local aleatoria:

```powershell
$servidorLocal = Read-Host 'Servidor PostgreSQL (por ejemplo localhost)'
$puertoLocal = Read-Host 'Puerto PostgreSQL (por ejemplo 5432)'
$usuarioLocal = Read-Host 'Usuario PostgreSQL (por ejemplo postgres)'
$claveLocal = Read-Host 'Contrasena PostgreSQL' -AsSecureString
$punteroClave = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($claveLocal)

try {
    $conexionLocal = [System.Data.Common.DbConnectionStringBuilder]::new()
    $conexionLocal.set_Item('Host', $servidorLocal)
    $conexionLocal.set_Item('Port', $puertoLocal)
    $conexionLocal.set_Item('Database', 'helpdeskdb')
    $conexionLocal.set_Item('Username', $usuarioLocal)
    $conexionLocal.set_Item('Password', [Runtime.InteropServices.Marshal]::PtrToStringBSTR($punteroClave))

    $bytesJwtLocal = New-Object byte[] 64
    $generadorLocal = [System.Security.Cryptography.RandomNumberGenerator]::Create()
    try { $generadorLocal.GetBytes($bytesJwtLocal) }
    finally { $generadorLocal.Dispose() }

    $configuracionLocal = @{
        'ConnectionStrings:DefaultConnection' = $conexionLocal.get_ConnectionString()
        'Jwt:Key' = [Convert]::ToBase64String($bytesJwtLocal)
        'Jwt:Issuer' = 'HelpDesk.Api'
        'Jwt:Audience' = 'HelpDesk.Web'
    }

    $configuracionLocal | ConvertTo-Json -Compress |
        dotnet user-secrets set --project HelpDesk.Api

    if ($LASTEXITCODE -ne 0) { throw 'No se pudo guardar la configuracion local.' }
}
finally {
    [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($punteroClave)
    $claveLocal.Dispose()
    Remove-Variable configuracionLocal, conexionLocal -ErrorAction SilentlyContinue
}
```

Introducir valores reales en las preguntas; no escribir literalmente los ejemplos si la instalación usa otros datos. Repetir el bloque genera una clave JWT nueva e invalida los tokens firmados con la anterior. No hace falta repetirlo cada vez que se inicia el proyecto.

La conexión selecciona `helpdeskdb`. Dentro de ella, EF usa el esquema `helpdesk` configurado en el modelo. Base y esquema son conceptos distintos.

### 5. Scaffolding: cuándo hacerlo y cómo

**Scaffolding significa leer las tablas y generar las clases C# y el DbContext. No crea tablas ni ejecuta el SQL.** La solución que se comparte ya incluye los modelos y el contexto: si la base coincide con el SQL actual, se puede continuar al paso 6 sin regenerar nada.

Solo regenerar cuando cambie el esquema o cuando el coordinador lo indique. No borrar modelos para probarlo, porque los handlers actuales necesitan esas clases para compilar.

Instalar la herramienta si no existe:

```powershell
dotnet tool install --global dotnet-ef --version 10.0.13
dotnet ef --version
```

Si indica que la herramienta ya está instalada, consultar `dotnet ef --version`. Si es anterior a `10.0.13`, actualizarla:

```powershell
dotnet tool update --global dotnet-ef --version 10.0.13
```

Si es una versión superior usada por otros proyectos, coordinar una herramienta local con Brahan en vez de cambiarla sin revisar compatibilidad. Si `dotnet ef` no se reconoce tras instalarlo, cerrar y abrir la terminal y comprobar que `%USERPROFILE%\.dotnet\tools` esté en PATH.

**Detener la API antes de regenerar.** Desde la carpeta de la solución:

```powershell
.scriptsActualizar-Modelo.ps1 -Sobrescribir
```

El script activa Development temporalmente y utiliza la conexión por nombre. Equivale a seleccionar:

- Proveedor: `Npgsql.EntityFrameworkCore.PostgreSQL`.
- Proyecto destino del contexto: `HelpDesk.Infrastructure`.
- Proyecto de arranque/configuración: `HelpDesk.Api`.
- Esquema: `helpdesk`.
- Entidades: `HelpDesk.Domain/Models/`.
- Contexto: `HelpDesk.Infrastructure/Data/HelpDeskDbContext.cs`.
- Opción `--no-onconfiguring`: evita incrustar credenciales en el código generado.

`-Sobrescribir` reemplaza archivos generados. Guardar los cambios propios antes, revisar el diff y coordinar la subida del scaffolding con Brahan. Las personalizaciones de concurrencia y relaciones están en `HelpDeskDbContext.Custom.cs` y se conservan separadas del contexto generado.

Es normal que el generador avise que no representa los índices de usuario y correo basados en `lower(...)`: siguen funcionando en PostgreSQL. También es normal tener 34 clases generadas y no 38: cuatro tablas de relación se representan dentro del contexto, sin archivo propio.

### 6. Crear el primer administrador local

El SQL crea los nombres de roles y permisos, **pero no crea cuentas ni contraseñas**. Ejecutar una vez por base, con la API detenida:

```powershell
.scriptsCrear-Administrador.ps1 -NombreUsuario admin -Correo tu-correo@dominio.com
```

Usar un correo propio de desarrollo. El script pedirá una contraseña oculta de al menos 10 caracteres y la guardará como hash mediante .NET. Si ya existe un administrador, el script no crea otro: se utilizan las operaciones administrativas de la API.

La contraseña de esta cuenta **no es la contraseña de PostgreSQL**. Una sirve para entrar a HelpDesk y la otra para que el backend se conecte al servidor.

Si PowerShell bloquea scripts descargados, revisar los archivos y desbloquear solo los scripts del proyecto:

```powershell
Unblock-File .scriptsActualizar-Modelo.ps1
Unblock-File .scriptsCrear-Administrador.ps1
```

Si la política de la computadora sigue impidiendo scripts, coordinarlo con el administrador del equipo. No hace falta cambiar permanentemente la política global para compilar el proyecto.

### 7. Ejecutar la API y abrir Swagger

Desde la carpeta de la solución:

```powershell
dotnet run --project HelpDesk.Api --launch-profile http
```

Debe aparecer `Now listening on: http://localhost:5014` y `Hosting environment: Development`. Dejar esa terminal abierta; `Ctrl+C` detiene el servidor.

Abrir [Swagger local](http://localhost:5014/index.html). En Rider también se puede ejecutar el perfil `http` de `launchSettings.json`. No lanzar el `.exe` directamente como forma habitual de desarrollo: ese arranque no aplica el perfil de la solución.

### 8. Iniciar sesión y probar los endpoints

1. En Swagger, abrir **POST `/api/auth/login`** y pulsar **Try it out**.
2. Enviar el usuario y la clave de la cuenta creada en el paso 6:

```json
{
  "nombreUsuario": "admin",
  "clave": "TU_CLAVE_DE_HELPDESK"
}
```

3. Copiar el valor `data.token` de la respuesta.
4. Pulsar **Authorize** en Swagger y pegar solo el token. El esquema HTTP Bearer agrega el prefijo correspondiente.
5. Probar primero `GET /api/Roles` o `GET /api/Sedes`.

Orden de carga para poder registrar un ticket: sedes → áreas → usuarios con área y roles → equipos e integrantes → categorías/servicios → políticas de atención → ticket. Los usuarios nuevos no tienen todas las funciones por poseer un JWT: se comprueban sus roles, permisos y acceso al registro.

El administrador configura la plataforma. Para atender como técnico o decidir como aprobador necesita además el rol correspondiente; no recibe esas funciones automáticamente.

### 9. Comprobaciones y pruebas

Para una revisión normal:

```powershell
dotnet build HelpDeskBackend.sln
```

Para la suite completa, con la API detenida antes de compilar:

```powershell
dotnet run --project tests/HelpDesk.IntegrationTests
```

La suite necesita permisos de PostgreSQL para crear y eliminar una base temporal. Usa `helpdesk_test_*`, levanta una API temporal y elimina esa base al terminar; no inserta datos en `helpdeskdb`. También necesita `001_helpdesk_36_tablas_es.sql` en la carpeta superior a `HelpDeskBackend`, como en el paso 3. Si la cuenta no puede crear bases, Brahan ejecuta la suite y cada integrante hace sus pruebas de endpoints sobre su base local.

### 10. Errores frecuentes

| Mensaje o síntoma | Qué revisar |
|---|---|
| `Configure ConnectionStrings:DefaultConnection` | Ejecutar el paso 4 para el proyecto API correcto y arrancar con el perfil `http`. Los secretos de otra computadora no se transfieren por Git. |
| `Configure Jwt:Key`, `Jwt:Issuer` o `Jwt:Audience` | Falta completar el paso 4; la clave necesita al menos 32 bytes. |
| Conexión rechazada o tiempo agotado | Servicio PostgreSQL iniciado, servidor y puerto correctos. |
| Error de autenticación PostgreSQL | Usuario y contraseña de la base; no usar la clave maestra de pgAdmin ni la del administrador de HelpDesk. |
| Tabla o relación no encontrada | SQL aplicado en `helpdeskdb` y esquema `helpdesk`, no en otra base. |
| El puerto 5014 ya está ocupado | Detener la instancia anterior desde su terminal o desde Rider antes de iniciar otra. |
| DLL en uso al compilar o generar modelos | Detener primero la API y volver a compilar. |
| Swagger no carga la definición | Probar `/swagger/v1/swagger.json`, recargar con Ctrl+F5 y usar la configuración actual de `Program.cs`. |
| Respuesta 401 | Token ausente, inválido, vencido o emitido antes de cambiar la contraseña o la clave JWT. Iniciar sesión otra vez. |
| Respuesta 403 | Cuenta autenticada sin permiso; comprobar roles y pertenencia al equipo cuando corresponda. |
| Respuesta 409 | Duplicado, registro relacionado, versión anterior del ticket o estado incompatible. Leer el mensaje de respuesta. |
| No se puede crear un ticket | Configurar área del solicitante, servicio activo y política activa para esa prioridad. |

**No subir User Secrets, credenciales, tokens, `bin/` ni `obj/` al repositorio.** Se comparten código, `.csproj`, scripts y documentación; cada integrante conserva su configuración local.

Referencias oficiales para ampliar: [herramientas y scaffolding de EF Core](https://learn.microsoft.com/ef/core/miscellaneous/cli/dotnet), [User Secrets en ASP.NET Core](https://learn.microsoft.com/aspnet/core/security/app-secrets?view=aspnetcore-10.0) e [instalación PostgreSQL en Windows](https://www.postgresql.org/download/windows/).

---

## Resumen de responsables

| Integrante | Grupo | Entidades o recursos asignados |
|---|---|---|
| Diego Medina | Seguridad y cuentas | `Usuarios`, `Roles`, `UsuariosRoles`, `Permisos`, `RolesPermisos`, `RecuperacionesClave` |
| Gianfranco Paolo | Cambios y autorizaciones | `Cambios`, `CambiosTickets`, `TareasCambios`, `Aprobaciones` |
| Fernando Aquima | Inventario tecnológico | `Activos`, `TiposActivos`, `AsignacionesActivos`, `TicketsActivos` |
| Leticia Maldonado | Conocimiento y problemas recurrentes | `ArticulosConocimiento`, `CategoriasConocimiento`, `TicketsArticulos`, `Problemas`, `ProblemasTickets` |
| Frank Oviedo | Comunicación y notificaciones | `Comentarios`, `Adjuntos`, `Notificaciones`, `PreferenciasNotificaciones`, `EnviosNotificaciones` |
| Stiven Yerai | Organización y equipos de soporte | `Sedes`, `Areas`, `EquiposSoporte`, `IntegrantesEquipo` |
| Ederson Vilca | Servicios y calidad de atención | `Servicios`, `CategoriasServicios`, `PoliticasAtencion`, `Encuestas` |
| Brahan Quispe | Atención, seguimiento e integración | `Tickets`, `AsignacionesTickets`, `HistorialTickets`, `CargaTecnicos`, `ResumenTickets`, `Auditoria` |

`CargaTecnicos` y `ResumenTickets` son vistas de consulta, no tablas con CRUD. `Autenticacion` pertenece a Diego; `Reportes` pertenece a Brahan.

## Qué sube cada responsable por entidad

Para cada entidad asignada, el paquete funcional comprende:

1. Su carpeta `HekDesk.Application/UseCases/{Entidad}/`: comandos, consultas, handlers y DTOs.
2. `HelpDesk.Api/Controllers/{Entidad}Controller.cs` y, si existe, `{Entidad}AccionesController.cs`.
3. Revisión y cambios necesarios de `HelpDesk.Domain/Models/{Entidad}.cs`, respetando que es código generado por scaffolding.
4. Pruebas y ejemplos de solicitudes de su grupo; documentación de lo que funciona y de sus límites.

Las tablas de relación `RolesPermisos`, `TicketsActivos`, `ProblemasTickets` y `CambiosTickets` no tienen archivo de modelo independiente: EF las representa dentro del contexto. Sus carpetas de casos de uso y controladores sí existen.

## Detalle por integrante

### 1. Diego Medina — Seguridad y cuentas

**Rama sugerida:** `feature/diego-seguridad`.

**Responsabilidad funcional:**

- Inicio de sesión, registro, hash de contraseñas y recuperación de acceso.
- Asignación de roles y permisos; activación y desactivación de cuentas.
- Revisar que las respuestas nunca expongan hashes o tokens almacenados.

**Archivos y carpetas de su grupo:**

| Entidad | Casos de uso completos | Controladores | Modelo |
|---|---|---|---|
| `Usuarios` | `HekDesk.Application/UseCases/Usuarios/` | `UsuariosController.cs`, `UsuariosAccionesController.cs` | `Usuarios.cs` |
| `Roles` | `HekDesk.Application/UseCases/Roles/` | `RolesController.cs` | `Roles.cs` |
| `UsuariosRoles` | `HekDesk.Application/UseCases/UsuariosRoles/` | `UsuariosRolesController.cs` | `UsuariosRoles.cs` |
| `Permisos` | `HekDesk.Application/UseCases/Permisos/` | `PermisosController.cs` | `Permisos.cs` |
| `RolesPermisos` | `HekDesk.Application/UseCases/RolesPermisos/` | `RolesPermisosController.cs` | Relación mapeada en el contexto; sin clase propia |
| `RecuperacionesClave` | `HekDesk.Application/UseCases/RecuperacionesClave/` | `RecuperacionesClaveController.cs` | `RecuperacionesClave.cs` |

**Además:**

- Carpeta completa `HekDesk.Application/UseCases/Autenticacion/`.
- `HelpDesk.Api/Controllers/AuthController.cs`.
- `HekDesk.Application/Security/AutenticacionDtos.cs` y `AuthService.cs` para el alta inicial por consola.
- `HelpDesk.Infrastructure/Services/PasswordService.cs` y `JwtService.cs`.
- `HelpDesk.Api/Security/CurrentUser.cs` y `scripts/Crear-Administrador.ps1`.
- Coordinar cambios de JWT y autorización compartida con Brahan; no subir claves ni secretos.

**Qué debe demostrar antes de integrar:**

- Registrar un empleado y comprobar su rol inicial.
- Iniciar sesión, cambiar/restablecer clave y verificar que el token anterior deja de servir.
- Rechazar asignación de roles por un empleado y conservar un administrador activo.

**Ejemplo de mensaje de commit:** `feat: completar seguridad y cuentas`.

### 2. Gianfranco Paolo — Cambios y autorizaciones

**Rama sugerida:** `feature/gianfranco-cambios`.

**Responsabilidad funcional:**

- Planificación, programación, ejecución y finalización de cambios.
- Tareas de cada cambio y relación con los tickets que lo justifican.
- Aprobaciones por pasos: orden, responsable y decisión.

**Archivos y carpetas de su grupo:**

| Entidad | Casos de uso completos | Controladores | Modelo |
|---|---|---|---|
| `Cambios` | `HekDesk.Application/UseCases/Cambios/` | `CambiosController.cs`, `CambiosAccionesController.cs` | `Cambios.cs` |
| `CambiosTickets` | `HekDesk.Application/UseCases/CambiosTickets/` | `CambiosTicketsController.cs` | Relación mapeada en el contexto; sin clase propia |
| `TareasCambios` | `HekDesk.Application/UseCases/TareasCambios/` | `TareasCambiosController.cs` | `TareasCambios.cs` |
| `Aprobaciones` | `HekDesk.Application/UseCases/Aprobaciones/` | `AprobacionesController.cs`, `AprobacionesAccionesController.cs` | `Aprobaciones.cs` |

**Qué debe demostrar antes de integrar:**

- Crear un cambio con sus tareas.
- Impedir programación sin las aprobaciones requeridas.
- Aprobar en orden y finalizar solo cuando las tareas estén completadas o canceladas.

**Ejemplo de mensaje de commit:** `feat: completar cambios y autorizaciones`.

### 3. Fernando Aquima — Inventario tecnológico

**Rama sugerida:** `feature/fernando-activos`.

**Responsabilidad funcional:**

- Registro y clasificación de computadoras, impresoras y otros activos.
- Asignación, devolución y conservación de responsables anteriores.
- Relación de los activos con sus incidentes y solicitudes.

**Archivos y carpetas de su grupo:**

| Entidad | Casos de uso completos | Controladores | Modelo |
|---|---|---|---|
| `Activos` | `HekDesk.Application/UseCases/Activos/` | `ActivosController.cs` | `Activos.cs` |
| `TiposActivos` | `HekDesk.Application/UseCases/TiposActivos/` | `TiposActivosController.cs` | `TiposActivos.cs` |
| `AsignacionesActivos` | `HekDesk.Application/UseCases/AsignacionesActivos/` | `AsignacionesActivosController.cs`, `AsignacionesActivosAccionesController.cs` | `AsignacionesActivos.cs` |
| `TicketsActivos` | `HekDesk.Application/UseCases/TicketsActivos/` | `TicketsActivosController.cs` | Relación mapeada en el contexto; sin clase propia |

**Qué debe demostrar antes de integrar:**

- Crear un activo de un tipo y sede válidos.
- Asignarlo, devolverlo y asignarlo nuevamente sin perder historial.
- Vincular el activo a un ticket y rechazar dos asignaciones vigentes.

**Ejemplo de mensaje de commit:** `feat: completar inventario tecnológico`.

### 4. Leticia Maldonado — Conocimiento y problemas recurrentes

**Rama sugerida:** `feature/leticia-conocimiento`.

**Responsabilidad funcional:**

- Borradores, publicación, archivo y clasificación de soluciones documentadas.
- Vincular artículos de solución con tickets.
- Agrupar fallas recurrentes e investigar su causa y solución.

**Archivos y carpetas de su grupo:**

| Entidad | Casos de uso completos | Controladores | Modelo |
|---|---|---|---|
| `ArticulosConocimiento` | `HekDesk.Application/UseCases/ArticulosConocimiento/` | `ArticulosConocimientoController.cs`, `ArticulosConocimientoAccionesController.cs` | `ArticulosConocimiento.cs` |
| `CategoriasConocimiento` | `HekDesk.Application/UseCases/CategoriasConocimiento/` | `CategoriasConocimientoController.cs` | `CategoriasConocimiento.cs` |
| `TicketsArticulos` | `HekDesk.Application/UseCases/TicketsArticulos/` | `TicketsArticulosController.cs` | `TicketsArticulos.cs` |
| `Problemas` | `HekDesk.Application/UseCases/Problemas/` | `ProblemasController.cs` | `Problemas.cs` |
| `ProblemasTickets` | `HekDesk.Application/UseCases/ProblemasTickets/` | `ProblemasTicketsController.cs` | Relación mapeada en el contexto; sin clase propia |

**Qué debe demostrar antes de integrar:**

- Crear y publicar un artículo y comprobar su visibilidad.
- Relacionar una solución con un ticket.
- Agrupar tickets en un problema y exigir causa y solución al resolverlo.

**Ejemplo de mensaje de commit:** `feat: completar conocimiento y problemas recurrentes`.

### 5. Frank Oviedo — Comunicación y notificaciones

**Rama sugerida:** `feature/frank-comunicacion`.

**Responsabilidad funcional:**

- Respuestas públicas, notas internas y metadatos de archivos asociados.
- Avisos para destinatarios, preferencias y lectura de notificaciones.
- Seguimiento y reintento de correos; no confundirlo con un proveedor de envío ya implementado.

**Archivos y carpetas de su grupo:**

| Entidad | Casos de uso completos | Controladores | Modelo |
|---|---|---|---|
| `Comentarios` | `HekDesk.Application/UseCases/Comentarios/` | `ComentariosController.cs` | `Comentarios.cs` |
| `Adjuntos` | `HekDesk.Application/UseCases/Adjuntos/` | `AdjuntosController.cs` | `Adjuntos.cs` |
| `Notificaciones` | `HekDesk.Application/UseCases/Notificaciones/` | `NotificacionesController.cs`, `NotificacionesAccionesController.cs` | `Notificaciones.cs` |
| `PreferenciasNotificaciones` | `HekDesk.Application/UseCases/PreferenciasNotificaciones/` | `PreferenciasNotificacionesController.cs` | `PreferenciasNotificaciones.cs` |
| `EnviosNotificaciones` | `HekDesk.Application/UseCases/EnviosNotificaciones/` | `EnviosNotificacionesController.cs`, `EnviosNotificacionesAccionesController.cs` | `EnviosNotificaciones.cs` |

**Qué debe demostrar antes de integrar:**

- Comprobar que el empleado no vea notas ni adjuntos internos.
- Consultar avisos propios y marcarlos como leídos.
- Reintentar solo correos fallidos que no superaron el límite.

**Ejemplo de mensaje de commit:** `feat: completar comunicación y notificaciones`.

### 6. Stiven Yerai — Organización y equipos de soporte

**Rama sugerida:** `feature/stiven-organizacion`.

**Responsabilidad funcional:**

- Locales y departamentos de la empresa.
- Grupos de soporte y sus integrantes, coordinadores y disponibilidad.
- Validar las relaciones que necesitan usuarios, servicios y asignación de tickets.

**Archivos y carpetas de su grupo:**

| Entidad | Casos de uso completos | Controladores | Modelo |
|---|---|---|---|
| `Sedes` | `HekDesk.Application/UseCases/Sedes/` | `SedesController.cs` | `Sedes.cs` |
| `Areas` | `HekDesk.Application/UseCases/Areas/` | `AreasController.cs` | `Areas.cs` |
| `EquiposSoporte` | `HekDesk.Application/UseCases/EquiposSoporte/` | `EquiposSoporteController.cs` | `EquiposSoporte.cs` |
| `IntegrantesEquipo` | `HekDesk.Application/UseCases/IntegrantesEquipo/` | `IntegrantesEquipoController.cs` | `IntegrantesEquipo.cs` |

**Qué debe demostrar antes de integrar:**

- Crear sede, área y equipo.
- Agregar un integrante y actualizar su disponibilidad.
- Comprobar que la pertenencia al equipo sea válida para asignar un técnico.

**Ejemplo de mensaje de commit:** `feat: completar organización y equipos de soporte`.

### 7. Ederson Vilca — Servicios y calidad de atención

**Rama sugerida:** `feature/ederson-servicios`.

**Responsabilidad funcional:**

- Catálogo de atenciones disponibles y sus categorías.
- Plazos de primera respuesta y resolución por prioridad, en tiempo corrido.
- Evaluación de la atención: solo solicitante, ticket cerrado y una respuesta por ticket.

**Archivos y carpetas de su grupo:**

| Entidad | Casos de uso completos | Controladores | Modelo |
|---|---|---|---|
| `Servicios` | `HekDesk.Application/UseCases/Servicios/` | `ServiciosController.cs` | `Servicios.cs` |
| `CategoriasServicios` | `HekDesk.Application/UseCases/CategoriasServicios/` | `CategoriasServiciosController.cs` | `CategoriasServicios.cs` |
| `PoliticasAtencion` | `HekDesk.Application/UseCases/PoliticasAtencion/` | `PoliticasAtencionController.cs` | `PoliticasAtencion.cs` |
| `Encuestas` | `HekDesk.Application/UseCases/Encuestas/` | `EncuestasController.cs` | `Encuestas.cs` |

**Qué debe demostrar antes de integrar:**

- Crear categoría y servicio vinculado a un equipo.
- Configurar los plazos de una prioridad y comprobar que el ticket los copie.
- Aceptar una encuesta del solicitante al cierre y rechazar duplicados.

**Ejemplo de mensaje de commit:** `feat: completar servicios y calidad de atención`.

### 8. Brahan Quispe — Atención, seguimiento e integración

**Rama sugerida:** `feature/brahan-tickets`.

**Responsabilidad funcional:**

- Creación, asignación, atención, espera, resolución, cierre, reapertura y cancelación de tickets.
- Historial, versiones de concurrencia y cambios de responsables.
- Consultas de carga, resumen e indicadores y coordinación de la integración técnica.

**Archivos y carpetas de su grupo:**

| Entidad | Casos de uso completos | Controladores | Modelo |
|---|---|---|---|
| `Tickets` | `HekDesk.Application/UseCases/Tickets/` | `TicketsController.cs`, `TicketsAccionesController.cs` | `Tickets.cs` |
| `AsignacionesTickets` | `HekDesk.Application/UseCases/AsignacionesTickets/` | `AsignacionesTicketsController.cs` | `AsignacionesTickets.cs` |
| `HistorialTickets` | `HekDesk.Application/UseCases/HistorialTickets/` | `HistorialTicketsController.cs` | `HistorialTickets.cs` |
| `CargaTecnicos` | `HekDesk.Application/UseCases/CargaTecnicos/` | `CargaTecnicosController.cs` | `CargaTecnicos.cs` |
| `ResumenTickets` | `HekDesk.Application/UseCases/ResumenTickets/` | `ResumenTicketsController.cs` | `ResumenTickets.cs` |
| `Auditoria` | `HekDesk.Application/UseCases/Auditoria/` | `AuditoriaController.cs` | `Auditoria.cs` |

**Además:**

- Carpeta completa `HekDesk.Application/UseCases/Reportes/`.
- `HelpDesk.Api/Controllers/ReportesController.cs`, incluyendo la consulta de indicadores.

**Qué debe demostrar antes de integrar:**

- Completar el ciclo de creación, asignación, atención, resolución y cierre.
- Rechazar cambios con una versión antigua del ticket.
- Comprobar historial, auditoría, indicadores y visibilidad por usuario/equipo.

**Ejemplo de mensaje de commit:** `feat: completar atención, seguimiento e integración`.

## Archivos compartidos: quién coordina

Estos archivos afectan a varios grupos. Brahan coordina su integración; Diego revisa seguridad. Cada integrante solicita los cambios necesarios en lugar de reemplazarlos con una copia antigua.

| Archivo o área | Coordinación | Motivo |
|---|---|---|
| `HelpDeskBackend.sln` y archivos `.csproj` | Brahan | Referencias, dependencias y compilación de la solución. |
| `HelpDesk.Infrastructure/Data/HelpDeskDbContext.cs` | Brahan | Contexto generado que contiene relaciones de todas las entidades. |
| `HelpDesk.Infrastructure/Data/HelpDeskDbContext.Custom.cs` | Brahan, con responsable afectado | Relaciones corregidas y concurrencia sin editar a mano el contexto generado. |
| `scripts/Actualizar-Modelo.ps1` | Brahan | Regeneración coordinada del scaffolding. |
| `HekDesk.Application/Configuration/ApplicationServicesExtensions.cs` | Brahan | Registro de handlers y dependencias comunes. |
| `HelpDesk.Api/Program.cs` | Brahan + Diego | Arranque, JWT, middleware, Swagger y configuración. |
| `HelpDesk.Infrastructure/Configuration/InfrastructureServicesExtensions.cs` | Diego + Brahan | Persistencia, JWT y comprobación de identidad. |
| `HekDesk.Application/Security/AccessRules.cs` | Diego + Brahan | Acceso transversal por propietario, rol y equipo. |
| `HelpDesk.Domain/Ports/` y `HelpDesk.Infrastructure/Repositories/` | Brahan | Contratos, repositorio y Unit of Work. |
| `HekDesk.Application/Common/` | Brahan | Respuestas, paginación, validación básica y contratos compartidos. |
| `HelpDesk.Api/Middleware/` | Brahan | Respaldo de errores fuera de los handlers. |
| `tests/HelpDesk.IntegrationTests/` | Brahan integra; todos aportan casos | Suite compartida para prevenir regresiones. |
| SQL y cambios de esquema | Brahan coordina; dueño de entidad revisa | Evitar versiones incompatibles de la base. |

Los responsables indicados coordinan estos archivos; eso no cambia la propiedad funcional de las entidades de los otros integrantes.

## Dependencias entre grupos

| Grupo que necesita información | Coordina con | Dependencia |
|---|---|---|
| Todos | Diego | Usuarios autenticados, roles y permisos. |
| Usuarios, servicios, inventario y tickets | Stiven | Sedes, áreas, equipos e integrantes. |
| Brahan | Ederson | Servicio y política activa antes de crear un ticket. |
| Brahan | Frank | Comentarios, avisos y evidencias del proceso de atención. |
| Brahan | Gianfranco | Aprobaciones necesarias para continuar una atención. |
| Fernando, Leticia y Gianfranco | Brahan | Identificador del ticket para sus relaciones. |
| Ederson | Brahan | Ticket cerrado para permitir una encuesta. |
| Reportes | Todos | Datos consistentes para consultas e indicadores. |

## Cómo organizar la primera subida

1. Brahan integra un **commit base compilable**: solución, cuatro proyectos, paquetes, puertos, repositorios, configuración sin secretos, contexto y todos los modelos generados por scaffolding. Así los ocho trabajan sobre la misma base y el contexto no referencia clases que faltan.
2. Los modelos del commit base quedan bajo revisión de sus responsables por entidad; no hace falta volver a subirlos sin cambios. El trabajo funcional de cada integrante se concentra en sus casos de uso, DTOs, controladores, pruebas y ajustes de modelo necesarios.
3. Cada integrante parte de esa base común y usa la rama indicada. Los casos de uso compartidos ya existentes pueden formar parte de una base más completa si el equipo lo acuerda; se conservarán y cada integrante identificará sus cambios reales sobre esa base.
4. Cada integrante revisa únicamente sus rutas, valida su grupo y prepara commits con archivos explícitos. Evitar `git add .` cuando hay cambios de otros compañeros en la misma carpeta de trabajo.
5. Crear una solicitud de integración por grupo, describiendo entidades, endpoints y verificaciones. La cuenta, el repositorio remoto y la rama principal se confirman cuando se configure Git.
6. Integrar primero organización y seguridad, después catálogo y tickets; luego comunicación, activos, conocimiento, problemas y cambios. Cerrar con pruebas completas de los ocho grupos.

## Qué NO subir

- Carpetas `bin/`, `obj/`, `.idea/` ni archivos de preferencias personales.
- Contraseñas, claves JWT, cadenas de conexión con credenciales o archivos de User Secrets.
- Bases de datos locales, copias con información personal o registros de pruebas con secretos.
- Archivos compartidos reemplazados sin coordinar, ni una segunda versión del esquema incompatible.

## Verificación antes de integrar

```powershell
dotnet build HelpDeskBackend.sln
```

Brahan ejecuta además la suite compartida en la integración:

```powershell
dotnet run --project tests/HelpDesk.IntegrationTests
```

La suite usa una base temporal y necesita permisos para crearla y eliminarla. No debe apuntarse manualmente a una base con datos reales. Cada integrante debe documentar también qué escenarios de su grupo verificó; la suite general no reemplaza revisar todos sus endpoints.

## Cobertura del reparto

- **36 tablas:** cada una tiene un responsable único.
- **2 vistas:** `CargaTecnicos` y `ResumenTickets`, a cargo de Brahan.
- **Autenticación:** Diego.
- **Reportes e integración compartida:** Brahan.
- Los DTOs mantienen su ubicación actual dentro de `UseCases/{Entidad}/Dtos`; este reparto no mueve archivos de código.

Esta distribución no publica commits ni modifica el repositorio remoto. Es una guía de responsabilidades que el equipo puede ajustar según disponibilidad y experiencia.
