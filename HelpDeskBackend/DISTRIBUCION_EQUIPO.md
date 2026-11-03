# Distribución del proyecto HelpDesk entre seis integrantes

**Actualización:** 3 de noviembre de 2026. **Alcance:** 36 tablas, 2 vistas, autenticación y reportes.

Este es el reparto vigente. Sustituye al de ocho integrantes. El equipo actual es Diego Medina, Gianfranco Paolo, Fernando Aquima, Stiven Yerai, Ederson Vilca y Brahan Quispe. Las responsabilidades que quedaron libres se redistribuyen a continuación.

**Aclaración sobre servicios:** Brahan se encarga de los servicios técnicos de `HelpDesk.Infrastructure/Services` y sus interfaces de Domain. La entidad de negocio `Servicios` (catálogo de atenciones) corresponde a Ederson. Son responsabilidades distintas.

**Repositorio de este documento:** plantilla completa `BrahanPRANK/HelpDeskApi`. El repositorio de trabajo del equipo es `brahanquispedev/HelpDeskApi`; esta simplificación se publica únicamente en la plantilla.

## Primera entrega simplificada — 3 de noviembre de 2026

Esta plantilla se adapta al estilo del laboratorio 11. El reparto de entidades entre seis integrantes se mantiene; cada uno incorpora solamente los archivos del alcance actual. Las funciones que se describían antes como publicación de artículos, recuperación de contraseña, programación de cambios y reapertura de tickets quedan para una segunda entrega.

- Los casos de uso están en `HekDesk.Application/UseCases/{Entidad}/Commands` y `Queries`.
- Todos los DTOs están en `HelpDesk.Domain/Dtos/{Entidad}/`, uno por archivo. Los compartidos están en `Dtos/Common/` y los de acceso en `Dtos/Autenticacion/`.
- Los handlers contienen su lógica y `try/catch`. `CancellationToken` solo se declara en `Handle` para cumplir la interfaz de MediatR; no se utiliza ni se pasa al repositorio.
- `IUnitOfWork` ofrece `Repository<T>()` y `Complete()`. Los repositorios exponen operaciones básicas; cada escritura se confirma desde el handler. No hay auditoría automática dentro de UnitOfWork.
- Se conserva el flujo de tickets: crear → asignar → iniciar → resolver → cerrar. No se implementa reasignación en esta entrega.

El inventario exacto de archivos que corresponde copiar está en [PRIMER_AVANCE.md](PRIMER_AVANCE.md). La instalación sigue en los pasos siguientes.

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

La referencia está en `https://github.com/BrahanPRANK/HelpDeskApi`. El trabajo del equipo se sube a `https://github.com/brahanquispedev/HelpDeskApi`; copiar allí solamente los archivos asignados y adaptar los nombres de los proyectos.

Entrar en la carpeta que contiene `HelpDeskBackend.sln`. Cambiar esta ruta si el proyecto se descargó en otro lugar:

```powershell
cd C:\Users\ASUS\Documents\HelpDesk\HelpDeskBackend
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
.\scripts\Actualizar-Modelo.ps1 -Sobrescribir
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
.\scripts\Crear-Administrador.ps1 -NombreUsuario admin -Correo tu-correo@dominio.com
```

Usar un correo propio de desarrollo. El script pedirá una contraseña oculta de al menos 10 caracteres y la guardará como hash mediante .NET. Si ya existe un administrador, el script no crea otro: se utilizan las operaciones administrativas de la API.

**¿Por qué se usa un comando?** La base empieza sin usuarios. Hace falta crear al primer administrador para que luego pueda configurar áreas, usuarios y roles. El script es un paso de instalación local: no se ejecuta cada vez que inicia la API ni para registrar a cada usuario. `AuthService.Bootstrap` agrega la cuenta y su relación con el rol administrador mediante el repositorio y un único `Complete()`.

**Después de crearlo:** iniciar sesión en `POST /api/auth/login` con esa cuenta y usar su JWT. El hash guarda una representación segura de la contraseña; el JWT identifica las peticiones posteriores. El script no entrega un JWT ni permite iniciar sesión directamente.

**Para los demás usuarios:** `POST /api/auth/registro` crea empleados. El administrador asigna el área mediante Usuarios y otros roles mediante UsuariosRoles. Las altas en `POST /api/Usuarios` también son administrativas y necesitan asignación de rol; no deben confundirse con el registro público. Nadie puede elegir ser administrador desde el registro público.

La contraseña de esta cuenta **no es la contraseña de PostgreSQL**. Una sirve para entrar a HelpDesk y la otra para que el backend se conecte al servidor.

Si PowerShell bloquea scripts descargados, revisar los archivos y desbloquear solo los scripts del proyecto:

```powershell
Unblock-File .\scripts\Actualizar-Modelo.ps1
Unblock-File .\scripts\Crear-Administrador.ps1
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

| Integrante | Grupo | Entidades / recursos |
|---|---|---|
| Diego Medina | Autenticación, usuarios y acceso | `Usuarios`, `Roles`, `UsuariosRoles`, `Permisos`, `RolesPermisos`, `RecuperacionesClave` |
| Gianfranco Paolo | Cambios, aprobaciones y problemas recurrentes | `Cambios`, `CambiosTickets`, `TareasCambios`, `Aprobaciones`, `Problemas`, `ProblemasTickets` |
| Fernando Aquima | Inventario y evidencias de atención | `Activos`, `TiposActivos`, `AsignacionesActivos`, `TicketsActivos`, `Comentarios`, `Adjuntos` |
| Stiven Yerai | Organización, equipos y notificaciones | `Sedes`, `Areas`, `EquiposSoporte`, `IntegrantesEquipo`, `Notificaciones`, `PreferenciasNotificaciones`, `EnviosNotificaciones` |
| Ederson Vilca | Servicios, calidad y conocimiento | `Servicios`, `CategoriasServicios`, `PoliticasAtencion`, `Encuestas`, `ArticulosConocimiento`, `CategoriasConocimiento`, `TicketsArticulos` |
| Brahan Quispe | Tickets, servicios técnicos, persistencia e integración | `Tickets`, `AsignacionesTickets`, `HistorialTickets`, `CargaTecnicos`, `ResumenTickets`, `Auditoria` |

Cada tabla o vista tiene un solo responsable funcional. `CargaTecnicos` y `ResumenTickets` se consultan; no tienen CRUD. `Autenticacion` corresponde a Diego y `Reportes` a Brahan.

## Qué archivos debe subir cada integrante

Por cada entidad asignada, el responsable incorpora al repositorio de trabajo:

1. `Application/UseCases/{Entidad}/`: comandos, consultas y handlers de su entidad. Además, subir `Domain/Dtos/{Entidad}/`, un DTO por archivo.
2. Los controladores CRUD y de acciones correspondientes en el proyecto API.
3. Pruebas y ejemplos de peticiones que demuestren su parte.
4. Ajustes de modelo necesarios, coordinados con Brahan; los modelos generados ya pertenecen a la base común.

En esta plantilla Application se llama `HekDesk.Application` y la API `HelpDesk.Api`. En la estructura del equipo del Escritorio se llaman `HelpDesk.Application` y `HelpDeskApi`. Adaptar las rutas, namespaces y referencias al proyecto destino; no reemplazar su solución completa con la de la plantilla.

Los modelos, contexto, repositorios y servicios técnicos compartidos los sube y coordina Brahan. Los otros integrantes los consumen mediante interfaces; no crean copias de Unit of Work por entidad.

## Detalle por integrante

### 1. Diego Medina — Autenticación, usuarios y acceso

**Rama sugerida:** `feature/diego-seguridad`.

**Responsabilidad funcional:**

- Registro e inicio de sesión y cambio de contraseña. Recuperación queda para una segunda versión.
- Administración de cuentas, roles y permisos; conservar un administrador activo.
- Consumir las interfaces de contraseña y JWT; coordinar con Brahan sus implementaciones de Infrastructure.

**Carpetas y controladores en la plantilla:**

| Entidad | Casos de uso completos | Controladores | Modelo de referencia |
|---|---|---|
| `Usuarios` | `HekDesk.Application/UseCases/Usuarios/` | `UsuariosController.cs`, `UsuariosAccionesController.cs` | `Usuarios.cs` |
| `Roles` | `HekDesk.Application/UseCases/Roles/` | `RolesController.cs` | `Roles.cs` |
| `UsuariosRoles` | `HekDesk.Application/UseCases/UsuariosRoles/` | `UsuariosRolesController.cs` | `UsuariosRoles.cs` |
| `Permisos` | `HekDesk.Application/UseCases/Permisos/` | `PermisosController.cs` | `Permisos.cs` |
| `RolesPermisos` | `HekDesk.Application/UseCases/RolesPermisos/` | `RolesPermisosController.cs` | Relación dentro del contexto; sin clase propia |
| `RecuperacionesClave` | `HekDesk.Application/UseCases/RecuperacionesClave/` | `RecuperacionesClaveController.cs` | `RecuperacionesClave.cs` |

**Además sube:**

- `HekDesk.Application/UseCases/Autenticacion/Commands/`.
- `HelpDesk.Domain/Dtos/Autenticacion/` y `HekDesk.Application/Security/AuthService.cs` (alta inicial por consola).
- `HelpDesk.Api/Controllers/AuthController.cs` y `HelpDesk.Api/Security/CurrentUser.cs`.
- `HekDesk.Application/Security/AccessRules.cs` y `scripts/Crear-Administrador.ps1`, coordinando su integración.
- Diego define y prueba el flujo de acceso. Brahan sube las implementaciones técnicas `PasswordService`, `JwtService` y sus interfaces; no se duplican esos archivos.

**Debe demostrar:**

- Registro solo como empleado, login correcto e incorrecto.
- Denegar cambios de roles por un empleado y comprobar cuenta inactiva.
- Cambiar la contraseña con la clave actual e invalidar el token de sesión anterior.

### 2. Gianfranco Paolo — Cambios, aprobaciones y problemas recurrentes

**Rama sugerida:** `feature/gianfranco-cambios`.

**Responsabilidad funcional:**

- Crear, consultar y actualizar cambios en borrador y sus tareas. La programación, ejecución y finalización quedan para v2.
- Decidir aprobaciones en orden y por el aprobador asignado.
- Investigar fallas recurrentes, documentar causa raíz y relacionar incidentes con problemas.

**Carpetas y controladores en la plantilla:**

| Entidad | Casos de uso completos | Controladores | Modelo de referencia |
|---|---|---|
| `Cambios` | `HekDesk.Application/UseCases/Cambios/` | `CambiosController.cs` | `Cambios.cs` |
| `CambiosTickets` | `HekDesk.Application/UseCases/CambiosTickets/` | `CambiosTicketsController.cs` | Relación dentro del contexto; sin clase propia |
| `TareasCambios` | `HekDesk.Application/UseCases/TareasCambios/` | `TareasCambiosController.cs` | `TareasCambios.cs` |
| `Aprobaciones` | `HekDesk.Application/UseCases/Aprobaciones/` | `AprobacionesController.cs`, `AprobacionesAccionesController.cs` | `Aprobaciones.cs` |
| `Problemas` | `HekDesk.Application/UseCases/Problemas/` | `ProblemasController.cs` | `Problemas.cs` |
| `ProblemasTickets` | `HekDesk.Application/UseCases/ProblemasTickets/` | `ProblemasTicketsController.cs` | Relación dentro del contexto; sin clase propia |

**Debe demostrar:**

- Crear y consultar cambios en borrador y actualizar sus tareas; el ciclo de ejecución se implementará en v2.
- Rechazar una segunda decisión sobre una aprobación resuelta.
- Exigir causa y solución al resolver un problema.

### 3. Fernando Aquima — Inventario y evidencias de atención

**Rama sugerida:** `feature/fernando-activos-evidencias`.

**Responsabilidad funcional:**

- Registrar y clasificar bienes tecnológicos, asignarlos y recibir devoluciones.
- Relacionar activos con tickets.
- Gestionar mensajes y metadatos de evidencias asociadas; aplicar visibilidad pública o interna.

**Carpetas y controladores en la plantilla:**

| Entidad | Casos de uso completos | Controladores | Modelo de referencia |
|---|---|---|
| `Activos` | `HekDesk.Application/UseCases/Activos/` | `ActivosController.cs` | `Activos.cs` |
| `TiposActivos` | `HekDesk.Application/UseCases/TiposActivos/` | `TiposActivosController.cs` | `TiposActivos.cs` |
| `AsignacionesActivos` | `HekDesk.Application/UseCases/AsignacionesActivos/` | `AsignacionesActivosController.cs`, `AsignacionesActivosAccionesController.cs` | `AsignacionesActivos.cs` |
| `TicketsActivos` | `HekDesk.Application/UseCases/TicketsActivos/` | `TicketsActivosController.cs` | Relación dentro del contexto; sin clase propia |
| `Comentarios` | `HekDesk.Application/UseCases/Comentarios/` | `ComentariosController.cs` | `Comentarios.cs` |
| `Adjuntos` | `HekDesk.Application/UseCases/Adjuntos/` | `AdjuntosController.cs` | `Adjuntos.cs` |

**Debe demostrar:**

- Asignar, devolver y volver a asignar un activo conservando el historial.
- Rechazar dos asignaciones vigentes de un activo.
- Impedir que el solicitante consulte comentarios o adjuntos internos.

### 4. Stiven Yerai — Organización, equipos y notificaciones

**Rama sugerida:** `feature/stiven-organizacion-avisos`.

**Responsabilidad funcional:**

- Configurar locales, departamentos, grupos de soporte e integrantes.
- Gestionar avisos destinados a usuarios, lectura y preferencias.
- Consultar el seguimiento de envíos; los reintentos y el proveedor de correo real quedan para v2.

**Carpetas y controladores en la plantilla:**

| Entidad | Casos de uso completos | Controladores | Modelo de referencia |
|---|---|---|
| `Sedes` | `HekDesk.Application/UseCases/Sedes/` | `SedesController.cs` | `Sedes.cs` |
| `Areas` | `HekDesk.Application/UseCases/Areas/` | `AreasController.cs` | `Areas.cs` |
| `EquiposSoporte` | `HekDesk.Application/UseCases/EquiposSoporte/` | `EquiposSoporteController.cs` | `EquiposSoporte.cs` |
| `IntegrantesEquipo` | `HekDesk.Application/UseCases/IntegrantesEquipo/` | `IntegrantesEquipoController.cs` | `IntegrantesEquipo.cs` |
| `Notificaciones` | `HekDesk.Application/UseCases/Notificaciones/` | `NotificacionesController.cs`, `NotificacionesAccionesController.cs` | `Notificaciones.cs` |
| `PreferenciasNotificaciones` | `HekDesk.Application/UseCases/PreferenciasNotificaciones/` | `PreferenciasNotificacionesController.cs` | `PreferenciasNotificaciones.cs` |
| `EnviosNotificaciones` | `HekDesk.Application/UseCases/EnviosNotificaciones/` | `EnviosNotificacionesController.cs` | `EnviosNotificaciones.cs` |

**Debe demostrar:**

- Crear sede, área y equipo; revisar pertenencia y disponibilidad de técnicos.
- Mostrar y marcar como leídos solo los avisos del destinatario.
- Consultar envíos existentes y verificar las preferencias del destinatario; los reintentos quedan para v2.

### 5. Ederson Vilca — Servicios, calidad y conocimiento

**Rama sugerida:** `feature/ederson-servicios-conocimiento`.

**Responsabilidad funcional:**

- Administrar el catálogo de servicios y los compromisos de respuesta y resolución.
- Evaluar la atención con encuestas y controlar quién puede responderlas.
- Crear y clasificar artículos en borrador. La publicación y las relaciones con artículos publicados quedan para v2.

**Carpetas y controladores en la plantilla:**

| Entidad | Casos de uso completos | Controladores | Modelo de referencia |
|---|---|---|
| `Servicios` | `HekDesk.Application/UseCases/Servicios/` | `ServiciosController.cs` | `Servicios.cs` |
| `CategoriasServicios` | `HekDesk.Application/UseCases/CategoriasServicios/` | `CategoriasServiciosController.cs` | `CategoriasServicios.cs` |
| `PoliticasAtencion` | `HekDesk.Application/UseCases/PoliticasAtencion/` | `PoliticasAtencionController.cs` | `PoliticasAtencion.cs` |
| `Encuestas` | `HekDesk.Application/UseCases/Encuestas/` | `EncuestasController.cs` | `Encuestas.cs` |
| `ArticulosConocimiento` | `HekDesk.Application/UseCases/ArticulosConocimiento/` | `ArticulosConocimientoController.cs` | `ArticulosConocimiento.cs` |
| `CategoriasConocimiento` | `HekDesk.Application/UseCases/CategoriasConocimiento/` | `CategoriasConocimientoController.cs` | `CategoriasConocimiento.cs` |
| `TicketsArticulos` | `HekDesk.Application/UseCases/TicketsArticulos/` | `TicketsArticulosController.cs` | `TicketsArticulos.cs` |

**Debe demostrar:**

- Crear un servicio de un equipo válido y una política por prioridad.
- Aceptar una encuesta propia al cierre y rechazar duplicados.
- Crear y editar un artículo en borrador y comprobar que otro empleado no pueda verlo. La publicación y asociación operativa quedan para v2.

### 6. Brahan Quispe — Tickets, servicios técnicos, persistencia e integración

**Rama sugerida:** `feature/brahan-tickets-infraestructura`.

**Responsabilidad funcional:**

- Implementar el ciclo de atención, asignaciones, historial y control de versión de tickets.
- Gestionar reportes operativos y auditoría.
- Implementar y subir servicios técnicos de Infrastructure, repositorios, Unit of Work y sus contratos en Domain.
- Coordinar configuración, registro de dependencias, middleware, scaffolding e integración general.

**Carpetas y controladores en la plantilla:**

| Entidad | Casos de uso completos | Controladores | Modelo de referencia |
|---|---|---|
| `Tickets` | `HekDesk.Application/UseCases/Tickets/` | `TicketsController.cs`, `TicketsAccionesController.cs` | `Tickets.cs` |
| `AsignacionesTickets` | `HekDesk.Application/UseCases/AsignacionesTickets/` | `AsignacionesTicketsController.cs` | `AsignacionesTickets.cs` |
| `HistorialTickets` | `HekDesk.Application/UseCases/HistorialTickets/` | `HistorialTicketsController.cs` | `HistorialTickets.cs` |
| `CargaTecnicos` | `HekDesk.Application/UseCases/CargaTecnicos/` | `CargaTecnicosController.cs` | `CargaTecnicos.cs` |
| `ResumenTickets` | `HekDesk.Application/UseCases/ResumenTickets/` | `ResumenTicketsController.cs` | `ResumenTickets.cs` |
| `Auditoria` | `HekDesk.Application/UseCases/Auditoria/` | `AuditoriaController.cs` | `Auditoria.cs` |

**Además sube y mantiene la base compartida:**

- `HekDesk.Application/UseCases/Reportes/Queries/` y `HelpDesk.Api/Controllers/ReportesController.cs`.
- `HelpDesk.Domain/Ports/IGenericRepository.cs`, `IUnitOfWork.cs`, `ICurrentUser.cs`, `IPasswordService.cs`, `ITokenService.cs` y `AccessToken.cs`.
- `HelpDesk.Domain/Models/` como scaffolding común; cada dueño de entidad revisa los cambios que le afectan.
- `HelpDesk.Domain/Exceptions/BusinessException.cs`.
- `HelpDesk.Infrastructure/Repositories/GenericRepository.cs` y `UnitOfWork.cs`.
- `HelpDesk.Infrastructure/Services/PasswordService.cs` y `JwtService.cs`.
- `HelpDesk.Infrastructure/Data/HelpDeskDbContext.cs` y `HelpDeskDbContext.Custom.cs`.
- `HelpDesk.Infrastructure/Configuration/InfrastructureServicesExtensions.cs`.
- `HekDesk.Application/Configuration/ApplicationServicesExtensions.cs` y los contratos de `Dtos/Common/`, además de las validaciones de `Common/`.
- `HelpDesk.Api/Program.cs`, `HelpDesk.Api/Middleware/`, solución, referencias y dependencias.
- `scripts/Actualizar-Modelo.ps1`, SQL, documentación y suite de integración compartida.
- Si se añade un servicio técnico adicional a Infrastructure, Brahan coordina su implementación, interfaz y registro. El responsable funcional correspondiente aporta sus requisitos y pruebas.

**Debe demostrar:**

- Completar registro, asignación, atención, resolución y cierre de un ticket.
- Rechazar modificaciones con una versión anterior.
- Comprobar guardado mediante Complete(), consultas de reportes, JWT y hash junto con Diego. Auditoría conserva sus consultas; el registro automático queda para v2.

## Código compartido: responsabilidad y revisión

| Componente | Responsable de subir / mantener | Coordinación |
|---|---|---|
| Repositorio genérico e interfaz | Brahan | Todos utilizan `Repository<T>()`. |
| Unit of Work e interfaz | Brahan | Todos confirman las escrituras con `Complete()`. |
| Interfaces de servicios en Domain | Brahan | Diego revisa los contratos de seguridad. |
| JWT y hash en Infrastructure/Services | Brahan | Diego consume los servicios desde autenticación y prueba el acceso. |
| Usuario actual y reglas de autorización | Diego | Brahan integra con la configuración de la API. |
| Contexto, modelos y correcciones del scaffolding | Brahan | Cada integrante revisa sus entidades; no regenerar independientemente. |
| Program, registro DI, middleware y Common | Brahan | Registrar contratos y handlers de todos sin reemplazar cambios ajenos. |
| SQL, `.sln`, `.csproj`, scripts y pruebas compartidas | Brahan | Los seis aportan escenarios; cambios de esquema coordinados. |

**Domain no lleva implementaciones de acceso a PostgreSQL ni generación de JWT.** Brahan mantiene allí entidades, excepciones y contratos; implementa persistencia y servicios en Infrastructure.

## Dependencias entre grupos

| Grupo | Necesita coordinar con | Información compartida |
|---|---|---|
| Todos | Brahan | Interfaces, repositorios, servicios y configuración común. |
| Todos | Diego | Identidades, roles y permisos. |
| Usuarios, inventario, servicios y tickets | Stiven | Sedes, áreas, equipos e integrantes. |
| Tickets | Ederson | Servicio y política activa antes de registrar un caso. |
| Tickets | Fernando | Comentarios, adjuntos y activos relacionados. |
| Tickets y cambios | Gianfranco | Aprobaciones, problemas y cambios relacionados. |
| Tickets | Stiven | Avisos y destinatarios. |
| Conocimiento y encuestas | Brahan | Estado y acceso al ticket relacionado. |
| Reportes | Los seis | Datos coherentes para indicadores. |

## Cómo incorporar los archivos al repositorio de trabajo

1. Partir de `main` actualizado de `brahanquispedev/HelpDeskApi`, no de una copia de la solución completa de referencia.
2. Brahan integra primero la persistencia, servicios técnicos, contratos, contexto y configuración común. La base debe seguir compilando.
3. Cada integrante crea su rama, adapta los archivos de sus entidades a los nombres del proyecto destino y agrega pruebas.
4. Subir solo los archivos de su responsabilidad. Los archivos compartidos se coordinan con Brahan; los de seguridad con Diego y Brahan.
5. Abrir un Pull Request hacia `main`, indicando qué se incorporó, qué se adaptó y cómo se probó. Cada persona usa su cuenta y autoría propia.
6. Integrar los grupos por dependencias y ejecutar las pruebas compartidas. Que haya más o menos entidades no significa igual carga; revisar avance entre los seis.

No se incorporan automáticamente contribuciones al repositorio de trabajo por actualizar este documento de la plantilla. Este documento define quién debe subirlas.

## Qué NO subir

- Secretos, contraseñas, tokens, cadenas de conexión con credenciales ni archivos de User Secrets.
- Carpetas `bin/`, `obj/`, `.idea/` o configuración personal.
- Copias completas de contexto o configuración que reemplacen cambios de otro integrante sin revisión.

## Verificación antes de integrar

Cada integrante compila y prueba sus endpoints. En esta plantilla:

```powershell
cd HelpDeskBackend
dotnet build HelpDeskBackend.sln
dotnet run --project tests/HelpDesk.IntegrationTests
```

En el repositorio del equipo se utilizan los nombres de su propia solución. La suite completa requiere una base temporal y permisos para crearla/eliminarla; su coordinación corresponde a Brahan.

## Cobertura y versión del reparto

- Seis integrantes activos.
- 36 tablas y 2 vistas: 38 recursos asignados una sola vez.
- Autenticación asignada a Diego; servicios técnicos y contratos asignados a Brahan.
- Reportes, repositorios, Unit of Work e integración asignados a Brahan.
- Se conserva la guía inicial de instalación y ejecución al principio del documento.
- El reparto anterior de ocho personas queda reemplazado por esta versión del 2 de noviembre de 2026.
