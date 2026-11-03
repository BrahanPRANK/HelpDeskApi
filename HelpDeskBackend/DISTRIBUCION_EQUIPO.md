# Distribución completa del primer avance de HelpDesk

**Equipo de seis integrantes. Actualización: 3 de noviembre de 2026.** La distribución por entidades se mantiene. Este documento también asigna todos los archivos compartidos y auxiliares de la primera versión para que ninguno quede sin responsable.

**Plantilla de referencia:** [BrahanPRANK/HelpDeskApi](https://github.com/BrahanPRANK/HelpDeskApi). **Repositorio de trabajo del equipo:** [brahanquispedev/HelpDeskApi](https://github.com/brahanquispedev/HelpDeskApi). Los compañeros incorporan sus archivos manualmente, mediante commits separados por entidad; no deben subir la plantilla entera de una sola vez.

## Qué ya hizo Brahan y qué se entrega al final

La revisión corresponde a la copia local del repositorio del equipo, rama `feature/brahan-avance`, commit `06fd3b0`. Los archivos que hoy existen allí son avance de Brahan; todavía no se atribuyen aportes a otros integrantes. Esta revisión no afirma que todos esos commits ya estén integrados en main.

- Brahan ya incorporó estructura, scaffolding, puertos de Domain, repositorios, UnitOfWork, servicios JWT/hash, configuración de Infrastructure, Common, AccessRules y DTOs compartidos.
- También incorporó los casos de uso y DTOs de respuesta de Tickets y AsignacionesTickets. Sus controladores siguen pendientes.
- Los DTOs `TicketsResponseDto` y `AsignacionesTicketsResponseDto` **ya están subidos** en Domain. No es necesario esperar al final para agregarlos: un handler que los utiliza los necesita para compilar. Al trabajar una entidad, incorporar primero su DTO o incluir DTO y casos de uso en el mismo commit.
- Aún faltan los módulos de los compañeros, sus controladores y la integración de la API. Program.cs del equipo conserva WeatherForecast y requiere la integración final que hace Brahan.

La meta es incorporar **todos los archivos versionados de esta primera versión**, incluidos proyectos, configuraciones de ejemplo, scripts y documentación. Bin, obj, configuración personal del IDE, contraseñas y User Secrets no forman parte de esa entrega. La base sigue siendo de **36 tablas y 2 vistas**; repartir más archivos no agrega tablas.

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

`dotnet restore` instala las dependencias indicadas en los `.csproj`: EF Core, PostgreSQL/Npgsql, MediatR, JWT y Swagger. **No hace falta agregar los paquetes uno por uno ni actualizar sus versiones.** Esta compilación no inicia la API ni necesita todavía consultar la base.

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

Iniciar la API y comprobar cada módulo manualmente desde Swagger. Las pruebas automáticas no forman parte de esta primera entrega.

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

---

## Distribución por entidades

| Integrante | Entidades o recursos a incorporar | Responsabilidad adicional |
|---|---|---|
| Diego Medina | `Usuarios`, `Roles`, `UsuariosRoles`, `Permisos`, `RolesPermisos`, `RecuperacionesClave` | Autenticación, AuthService de alta inicial, CurrentUser de API y script de administrador. Consume la seguridad compartida ya subida por Brahan. |
| Gianfranco Paolo | `Cambios`, `CambiosTickets`, `TareasCambios`, `Aprobaciones`, `Problemas`, `ProblemasTickets` | Decisiones de aprobaciones y tareas de cambios; ciclo avanzado de cambios queda fuera de esta versión. |
| Fernando Aquima | `Activos`, `TiposActivos`, `AsignacionesActivos`, `TicketsActivos`, `Comentarios`, `Adjuntos` | Devolución de activos y visibilidad de comentarios/adjuntos. |
| Stiven Yerai | `Sedes`, `Areas`, `EquiposSoporte`, `IntegrantesEquipo`, `Notificaciones`, `PreferenciasNotificaciones`, `EnviosNotificaciones` | Avisos propios, lectura y preferencias. Los envíos se consultan; no se añade un proveedor de correo. |
| Ederson Vilca | `Servicios`, `CategoriasServicios`, `PoliticasAtencion`, `Encuestas`, `ArticulosConocimiento`, `CategoriasConocimiento`, `TicketsArticulos` | Catálogo de servicios, políticas, encuestas y conocimiento en borrador. |
| Brahan Quispe | `Tickets`, `AsignacionesTickets`, `HistorialTickets`, `CargaTecnicos`, `ResumenTickets`, `Auditoria` | Reportes, DTOs compartidos, Domain común, Infrastructure, configuración, Program, middleware, scaffolding, comprobaciones manuales y documentación. |

Cada dueño de entidad incorpora sus Commands, Queries, DTOs de respuesta y todos sus controladores, incluido el controlador de acciones si existe. Los modelos generados, puertos, repositorios y servicios técnicos tienen un único responsable de subida: Brahan. El dueño funcional revisa su entidad, pero no vuelve a subir esos archivos comunes como aporte propio.

**Servicios no significa una sola cosa:** `Infrastructure/Services` son JWT y hash, a cargo de Brahan. El recurso de negocio `Servicios`, su DTO y sus casos de uso corresponden a Ederson.

**Archivos compartidos ya realizados por Brahan:** Common/Validation.cs, Security/AccessRules.cs, Dtos/Common y los tres DTOs de Dtos/Autenticacion. Diego los consume y coordina cambios; no tiene que volver a copiarlos como si estuvieran pendientes. Sus Commands de registro, login y cambio de clave contienen directamente los datos de entrada.

## Qué subir por cada entidad

1. `HelpDesk.Domain/Dtos/{Entidad}/`: sus DTOs de respuesta, uno por archivo.
2. `HekDesk.Application/UseCases/{Entidad}/Commands/` y `Queries/`: solamente los archivos existentes de la primera versión. Las entidades de solo consulta no requieren inventar Commands.
3. `HelpDesk.Api/Controllers/{Entidad}Controller.cs` y su controlador de acciones, cuando aparezca en el inventario.
4. Compilar, probar su parte y hacer un commit identificable de esa entidad. Si otra entidad aún falta, incorporar primero sus dependencias compartidas y coordinar la integración.

Ejemplos de mensajes: `feat: agregar DTO y casos de uso de Sedes`, `feat: agregar controlador de Sedes`, `feat: agregar aprobaciones`. No mezclar todas las entidades de una persona en un único commit. Las acciones pertenecen a su entidad; no cuentan como nuevas tablas.

La definición de los DTOs está en Domain y su construcción en el handler. Los Commands y las Queries que reciben datos declaran sus propiedades directamente. Mantener el estilo actual: try/catch, Complete() para las escrituras, sin ILogger y CancellationToken solamente declarado en Handle. No recuperar los Commands avanzados retirados de la plantilla.

## Archivos compartidos y quién termina cada parte

| Archivos o grupo | Responsable | Situación y acción |
|---|---|---|
| Domain/Models, excepciones y puertos | Brahan | Base ya incorporada. Revisar diferencias del scaffolding, sin borrar modelos existentes. |
| Infrastructure/Repositories, Services y Configuration | Brahan | Ya incorporados. Conservar una sola implementación para todos. |
| Infrastructure/Data/HelpDeskDbContext.cs y .Custom.cs | Brahan | Ya incorporados; preservar la corrección de relaciones. |
| Application/Common y Security/AccessRules.cs | Brahan | Ya incorporados. |
| Domain/Dtos/Common y Domain/Dtos/Autenticacion | Brahan | Ya incorporados; los usa el resto del equipo. |
| Application/Security/AuthService.cs | Diego | Pendiente: inicialización del administrador desde consola. |
| API/Security/CurrentUser.cs y Controllers/AuthController.cs | Diego | Pendientes: adaptar identidad y exponer acceso por HTTP. |
| scripts/Crear-Administrador.ps1 | Diego | Pendiente; coordinar su llamada desde Program con Brahan. |
| Application/Configuration/ApplicationServicesExtensions.cs | Brahan | Pendiente; integrar MediatR y los servicios de Application cuando existan sus clases. |
| API/Program.cs y Middleware/ExceptionMiddleware.cs | Brahan | Pendientes: DI, conexión, JWT, autorizaciones, middleware, Swagger y controladores. |
| .csproj, solución, appsettings de ejemplo, launchSettings y archivo HTTP | Brahan | Existen bases; adaptar las referencias y configuración de la plantilla a los nombres del equipo. |
| SQL y scripts/Actualizar-Modelo.ps1 | Brahan | Completar los archivos compartidos para instalación y regeneración. No cambiar las 36 tablas por copiar archivos. |
| .gitignore, .gitattributes, README y todos los MD | Brahan | Revisar o incorporar los que falten; también están asignados en el inventario. |

Los archivos Class1.cs que aún existan son restos de estructura, no módulos adicionales. Brahan los revisa al integrar; no requieren crear una funcionalidad ni un commit ficticio.

## Orden para incorporar toda la versión

1. Usar la base común ya subida por Brahan y actualizar la rama de cada integrante desde la base acordada.
2. Stiven incorpora primero Sedes, Areas, EquiposSoporte e IntegrantesEquipo; Diego incorpora usuarios, roles, permisos y autenticación. Ederson incorpora categorías, políticas y servicios. Coordinar pruebas cuando estas dependencias estén disponibles.
3. Cada integrante incorpora sus entidades restantes, con DTOs, casos de uso y controladores en commits identificables. Brahan completa sus DTOs/controladores y los recursos que le faltan: HistorialTickets, CargaTecnicos, ResumenTickets, Auditoria y Reportes.
4. Brahan integra Program, registro de servicios, configuración de ejemplo y middleware. Diego aporta CurrentUser y AuthService. No se sustituye Program en cada rama de entidad.
5. Incorporar SQL, scripts y documentos pendientes según el inventario. Revisar las diferencias en archivos que ya existen, en vez de sobrescribirlos automáticamente.
6. Revisar los PR por entidad e integrarlos; compilar la solución final y probar login → crear ticket → asignar → iniciar → resolver → cerrar, además de los módulos de cada integrante.

**La API final no se considera completa solo porque se copiaron los casos de uso.** Deben estar presentes y conectados DTOs, controladores, registros DI, configuración y los archivos auxiliares de instalación.

## Nombres de los proyectos al copiar

| Plantilla BrahanPRANK | Proyecto del equipo brahanquispedev |
|---|---|
| `HelpDeskBackend/` | `HelpDeskApi/` |
| `HekDesk.Application/` | `HelpDesk.Application/` |
| `HelpDesk.Api/` | `HelpDeskApi/` |
| `HelpDeskBackend.sln` | `HelpDeskApi.sln` |
| `InfrastructureServicesExtensions.cs` | Actualmente `InfrastuctureServicesExtensions.cs`; su clase es InfrastructureServicesExtensions. Coordinar cualquier cambio de nombre. |

Los namespaces de código siguen siendo HelpDesk.Application, HelpDesk.Domain y HelpDesk.Infrastructure según cada archivo. No renombrar namespaces basándose solo en el error tipográfico de la carpeta HekDesk. Conservar el UserSecretsId y las rutas de los proyectos del equipo; no copiar contraseñas de otra computadora.

## Inventario exhaustivo: ningún archivo sin responsable

Este inventario incluye cada archivo versionado de la plantilla al realizar esta revisión. No incluye archivos generados ignorados por Git. Una fila tiene un único responsable de incorporación. El responsable de un archivo no implica que lo haya escrito ya: la tabla distingue el avance existente.

Estados comparados contra la copia local del equipo:

- **Coincide:** el archivo está en su destino y su contenido coincide al normalizar finales de línea y BOM. No requiere otro commit vacío.
- **Existe: revisar:** ya existe un archivo en el destino, pero difiere. Brahan hizo la base existente; el responsable coordina su adaptación. No significa que esté ausente o incorrecto.
- **Pendiente:** todavía no se encontró el archivo en el destino esperado.

La columna Destino usa los nombres del proyecto del equipo. Los modelos/proyectos ya creados deben revisarse, no recrearse. El estado de configuración no confirma conectividad con PostgreSQL ni que el módulo funcione por HTTP.

**Cobertura: 337 archivos de plantilla, todos asignados una sola vez.** Comparación local: 76 coinciden, 7 existen con diferencias y 254 están pendientes.

| Responsable | Archivos asignados |
|---|---|
| Diego Medina | 47 |
| Gianfranco Paolo | 40 |
| Fernando Aquima | 40 |
| Stiven Yerai | 47 |
| Ederson Vilca | 47 |
| Brahan Quispe | 116 |

### Inventario de Diego Medina

| Archivo de plantilla | Destino en el equipo | Estado |
|---|---|---|
| `HelpDeskBackend/HekDesk.Application/Security/AuthService.cs` | `HelpDeskApi/HelpDesk.Application/Security/AuthService.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/Autenticacion/Commands/CambiarClaveCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Autenticacion/Commands/CambiarClaveCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/Autenticacion/Commands/IniciarSesionCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Autenticacion/Commands/IniciarSesionCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/Autenticacion/Commands/RegistrarCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Autenticacion/Commands/RegistrarCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/Permisos/Commands/ActualizarPermisosCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Permisos/Commands/ActualizarPermisosCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/Permisos/Commands/CrearPermisosCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Permisos/Commands/CrearPermisosCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/Permisos/Commands/EliminarPermisosCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Permisos/Commands/EliminarPermisosCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/Permisos/Queries/ListarPermisosQuery.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Permisos/Queries/ListarPermisosQuery.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/Permisos/Queries/ObtenerPermisosQuery.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Permisos/Queries/ObtenerPermisosQuery.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/RecuperacionesClave/Queries/ListarRecuperacionesClaveQuery.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/RecuperacionesClave/Queries/ListarRecuperacionesClaveQuery.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/RecuperacionesClave/Queries/ObtenerRecuperacionesClaveQuery.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/RecuperacionesClave/Queries/ObtenerRecuperacionesClaveQuery.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/Roles/Commands/ActualizarRolesCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Roles/Commands/ActualizarRolesCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/Roles/Commands/CrearRolesCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Roles/Commands/CrearRolesCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/Roles/Commands/EliminarRolesCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Roles/Commands/EliminarRolesCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/Roles/Queries/ListarRolesQuery.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Roles/Queries/ListarRolesQuery.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/Roles/Queries/ObtenerRolesQuery.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Roles/Queries/ObtenerRolesQuery.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/RolesPermisos/Commands/CrearRolesPermisosCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/RolesPermisos/Commands/CrearRolesPermisosCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/RolesPermisos/Commands/EliminarRolesPermisosCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/RolesPermisos/Commands/EliminarRolesPermisosCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/RolesPermisos/Queries/ListarRolesPermisosQuery.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/RolesPermisos/Queries/ListarRolesPermisosQuery.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/RolesPermisos/Queries/ObtenerRolesPermisosQuery.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/RolesPermisos/Queries/ObtenerRolesPermisosQuery.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/Usuarios/Commands/ActivarUsuarioCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Usuarios/Commands/ActivarUsuarioCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/Usuarios/Commands/ActualizarUsuariosCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Usuarios/Commands/ActualizarUsuariosCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/Usuarios/Commands/CrearUsuariosCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Usuarios/Commands/CrearUsuariosCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/Usuarios/Commands/DesactivarUsuarioCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Usuarios/Commands/DesactivarUsuarioCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/Usuarios/Queries/ListarUsuariosQuery.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Usuarios/Queries/ListarUsuariosQuery.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/Usuarios/Queries/ObtenerUsuariosQuery.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Usuarios/Queries/ObtenerUsuariosQuery.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/UsuariosRoles/Commands/ActualizarUsuariosRolesCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/UsuariosRoles/Commands/ActualizarUsuariosRolesCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/UsuariosRoles/Commands/CrearUsuariosRolesCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/UsuariosRoles/Commands/CrearUsuariosRolesCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/UsuariosRoles/Commands/EliminarUsuariosRolesCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/UsuariosRoles/Commands/EliminarUsuariosRolesCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/UsuariosRoles/Queries/ListarUsuariosRolesQuery.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/UsuariosRoles/Queries/ListarUsuariosRolesQuery.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/UsuariosRoles/Queries/ObtenerUsuariosRolesQuery.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/UsuariosRoles/Queries/ObtenerUsuariosRolesQuery.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Api/Controllers/AuthController.cs` | `HelpDeskApi/HelpDeskApi/Controllers/AuthController.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Api/Controllers/PermisosController.cs` | `HelpDeskApi/HelpDeskApi/Controllers/PermisosController.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Api/Controllers/RecuperacionesClaveController.cs` | `HelpDeskApi/HelpDeskApi/Controllers/RecuperacionesClaveController.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Api/Controllers/RolesController.cs` | `HelpDeskApi/HelpDeskApi/Controllers/RolesController.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Api/Controllers/RolesPermisosController.cs` | `HelpDeskApi/HelpDeskApi/Controllers/RolesPermisosController.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Api/Controllers/UsuariosAccionesController.cs` | `HelpDeskApi/HelpDeskApi/Controllers/UsuariosAccionesController.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Api/Controllers/UsuariosController.cs` | `HelpDeskApi/HelpDeskApi/Controllers/UsuariosController.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Api/Controllers/UsuariosRolesController.cs` | `HelpDeskApi/HelpDeskApi/Controllers/UsuariosRolesController.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Api/Security/CurrentUser.cs` | `HelpDeskApi/HelpDeskApi/Security/CurrentUser.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Domain/Dtos/Permisos/PermisosResponseDto.cs` | `HelpDeskApi/HelpDesk.Domain/Dtos/Permisos/PermisosResponseDto.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Domain/Dtos/RecuperacionesClave/RecuperacionesClaveResponseDto.cs` | `HelpDeskApi/HelpDesk.Domain/Dtos/RecuperacionesClave/RecuperacionesClaveResponseDto.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Domain/Dtos/Roles/RolesResponseDto.cs` | `HelpDeskApi/HelpDesk.Domain/Dtos/Roles/RolesResponseDto.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Domain/Dtos/RolesPermisos/RolesPermisosResponseDto.cs` | `HelpDeskApi/HelpDesk.Domain/Dtos/RolesPermisos/RolesPermisosResponseDto.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Domain/Dtos/Usuarios/UsuariosResponseDto.cs` | `HelpDeskApi/HelpDesk.Domain/Dtos/Usuarios/UsuariosResponseDto.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Domain/Dtos/UsuariosRoles/UsuariosRolesResponseDto.cs` | `HelpDeskApi/HelpDesk.Domain/Dtos/UsuariosRoles/UsuariosRolesResponseDto.cs` | Pendiente |
| `HelpDeskBackend/scripts/Crear-Administrador.ps1` | `HelpDeskApi/scripts/Crear-Administrador.ps1` | Pendiente |

### Inventario de Gianfranco Paolo

| Archivo de plantilla | Destino en el equipo | Estado |
|---|---|---|
| `HelpDeskBackend/HekDesk.Application/UseCases/Aprobaciones/Commands/ActualizarAprobacionesCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Aprobaciones/Commands/ActualizarAprobacionesCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/Aprobaciones/Commands/AprobarSolicitudCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Aprobaciones/Commands/AprobarSolicitudCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/Aprobaciones/Commands/CrearAprobacionesCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Aprobaciones/Commands/CrearAprobacionesCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/Aprobaciones/Commands/RechazarSolicitudCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Aprobaciones/Commands/RechazarSolicitudCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/Aprobaciones/Queries/ListarAprobacionesQuery.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Aprobaciones/Queries/ListarAprobacionesQuery.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/Aprobaciones/Queries/ObtenerAprobacionesQuery.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Aprobaciones/Queries/ObtenerAprobacionesQuery.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/Cambios/Commands/ActualizarCambiosCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Cambios/Commands/ActualizarCambiosCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/Cambios/Commands/CrearCambiosCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Cambios/Commands/CrearCambiosCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/Cambios/Queries/ListarCambiosQuery.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Cambios/Queries/ListarCambiosQuery.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/Cambios/Queries/ObtenerCambiosQuery.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Cambios/Queries/ObtenerCambiosQuery.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/CambiosTickets/Commands/CrearCambiosTicketsCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/CambiosTickets/Commands/CrearCambiosTicketsCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/CambiosTickets/Commands/EliminarCambiosTicketsCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/CambiosTickets/Commands/EliminarCambiosTicketsCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/CambiosTickets/Queries/ListarCambiosTicketsQuery.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/CambiosTickets/Queries/ListarCambiosTicketsQuery.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/CambiosTickets/Queries/ObtenerCambiosTicketsQuery.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/CambiosTickets/Queries/ObtenerCambiosTicketsQuery.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/Problemas/Commands/ActualizarProblemasCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Problemas/Commands/ActualizarProblemasCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/Problemas/Commands/CrearProblemasCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Problemas/Commands/CrearProblemasCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/Problemas/Queries/ListarProblemasQuery.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Problemas/Queries/ListarProblemasQuery.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/Problemas/Queries/ObtenerProblemasQuery.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Problemas/Queries/ObtenerProblemasQuery.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/ProblemasTickets/Commands/CrearProblemasTicketsCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/ProblemasTickets/Commands/CrearProblemasTicketsCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/ProblemasTickets/Commands/EliminarProblemasTicketsCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/ProblemasTickets/Commands/EliminarProblemasTicketsCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/ProblemasTickets/Queries/ListarProblemasTicketsQuery.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/ProblemasTickets/Queries/ListarProblemasTicketsQuery.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/ProblemasTickets/Queries/ObtenerProblemasTicketsQuery.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/ProblemasTickets/Queries/ObtenerProblemasTicketsQuery.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/TareasCambios/Commands/ActualizarTareasCambiosCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/TareasCambios/Commands/ActualizarTareasCambiosCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/TareasCambios/Commands/CrearTareasCambiosCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/TareasCambios/Commands/CrearTareasCambiosCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/TareasCambios/Commands/EliminarTareasCambiosCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/TareasCambios/Commands/EliminarTareasCambiosCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/TareasCambios/Queries/ListarTareasCambiosQuery.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/TareasCambios/Queries/ListarTareasCambiosQuery.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/TareasCambios/Queries/ObtenerTareasCambiosQuery.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/TareasCambios/Queries/ObtenerTareasCambiosQuery.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Api/Controllers/AprobacionesAccionesController.cs` | `HelpDeskApi/HelpDeskApi/Controllers/AprobacionesAccionesController.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Api/Controllers/AprobacionesController.cs` | `HelpDeskApi/HelpDeskApi/Controllers/AprobacionesController.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Api/Controllers/CambiosController.cs` | `HelpDeskApi/HelpDeskApi/Controllers/CambiosController.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Api/Controllers/CambiosTicketsController.cs` | `HelpDeskApi/HelpDeskApi/Controllers/CambiosTicketsController.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Api/Controllers/ProblemasController.cs` | `HelpDeskApi/HelpDeskApi/Controllers/ProblemasController.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Api/Controllers/ProblemasTicketsController.cs` | `HelpDeskApi/HelpDeskApi/Controllers/ProblemasTicketsController.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Api/Controllers/TareasCambiosController.cs` | `HelpDeskApi/HelpDeskApi/Controllers/TareasCambiosController.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Domain/Dtos/Aprobaciones/AprobacionesResponseDto.cs` | `HelpDeskApi/HelpDesk.Domain/Dtos/Aprobaciones/AprobacionesResponseDto.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Domain/Dtos/Cambios/CambiosResponseDto.cs` | `HelpDeskApi/HelpDesk.Domain/Dtos/Cambios/CambiosResponseDto.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Domain/Dtos/CambiosTickets/CambiosTicketsResponseDto.cs` | `HelpDeskApi/HelpDesk.Domain/Dtos/CambiosTickets/CambiosTicketsResponseDto.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Domain/Dtos/Problemas/ProblemasResponseDto.cs` | `HelpDeskApi/HelpDesk.Domain/Dtos/Problemas/ProblemasResponseDto.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Domain/Dtos/ProblemasTickets/ProblemasTicketsResponseDto.cs` | `HelpDeskApi/HelpDesk.Domain/Dtos/ProblemasTickets/ProblemasTicketsResponseDto.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Domain/Dtos/TareasCambios/TareasCambiosResponseDto.cs` | `HelpDeskApi/HelpDesk.Domain/Dtos/TareasCambios/TareasCambiosResponseDto.cs` | Pendiente |

### Inventario de Fernando Aquima

| Archivo de plantilla | Destino en el equipo | Estado |
|---|---|---|
| `HelpDeskBackend/HekDesk.Application/UseCases/Activos/Commands/ActualizarActivosCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Activos/Commands/ActualizarActivosCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/Activos/Commands/CrearActivosCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Activos/Commands/CrearActivosCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/Activos/Commands/EliminarActivosCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Activos/Commands/EliminarActivosCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/Activos/Queries/ListarActivosQuery.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Activos/Queries/ListarActivosQuery.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/Activos/Queries/ObtenerActivosQuery.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Activos/Queries/ObtenerActivosQuery.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/Adjuntos/Commands/ActualizarAdjuntosCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Adjuntos/Commands/ActualizarAdjuntosCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/Adjuntos/Commands/CrearAdjuntosCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Adjuntos/Commands/CrearAdjuntosCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/Adjuntos/Commands/EliminarAdjuntosCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Adjuntos/Commands/EliminarAdjuntosCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/Adjuntos/Queries/ListarAdjuntosQuery.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Adjuntos/Queries/ListarAdjuntosQuery.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/Adjuntos/Queries/ObtenerAdjuntosQuery.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Adjuntos/Queries/ObtenerAdjuntosQuery.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/AsignacionesActivos/Commands/CrearAsignacionesActivosCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/AsignacionesActivos/Commands/CrearAsignacionesActivosCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/AsignacionesActivos/Commands/DevolverActivoCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/AsignacionesActivos/Commands/DevolverActivoCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/AsignacionesActivos/Queries/ListarAsignacionesActivosQuery.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/AsignacionesActivos/Queries/ListarAsignacionesActivosQuery.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/AsignacionesActivos/Queries/ObtenerAsignacionesActivosQuery.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/AsignacionesActivos/Queries/ObtenerAsignacionesActivosQuery.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/Comentarios/Commands/ActualizarComentariosCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Comentarios/Commands/ActualizarComentariosCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/Comentarios/Commands/CrearComentariosCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Comentarios/Commands/CrearComentariosCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/Comentarios/Queries/ListarComentariosQuery.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Comentarios/Queries/ListarComentariosQuery.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/Comentarios/Queries/ObtenerComentariosQuery.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Comentarios/Queries/ObtenerComentariosQuery.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/TicketsActivos/Commands/CrearTicketsActivosCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/TicketsActivos/Commands/CrearTicketsActivosCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/TicketsActivos/Commands/EliminarTicketsActivosCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/TicketsActivos/Commands/EliminarTicketsActivosCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/TicketsActivos/Queries/ListarTicketsActivosQuery.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/TicketsActivos/Queries/ListarTicketsActivosQuery.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/TicketsActivos/Queries/ObtenerTicketsActivosQuery.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/TicketsActivos/Queries/ObtenerTicketsActivosQuery.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/TiposActivos/Commands/ActualizarTiposActivosCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/TiposActivos/Commands/ActualizarTiposActivosCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/TiposActivos/Commands/CrearTiposActivosCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/TiposActivos/Commands/CrearTiposActivosCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/TiposActivos/Commands/EliminarTiposActivosCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/TiposActivos/Commands/EliminarTiposActivosCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/TiposActivos/Queries/ListarTiposActivosQuery.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/TiposActivos/Queries/ListarTiposActivosQuery.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/TiposActivos/Queries/ObtenerTiposActivosQuery.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/TiposActivos/Queries/ObtenerTiposActivosQuery.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Api/Controllers/ActivosController.cs` | `HelpDeskApi/HelpDeskApi/Controllers/ActivosController.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Api/Controllers/AdjuntosController.cs` | `HelpDeskApi/HelpDeskApi/Controllers/AdjuntosController.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Api/Controllers/AsignacionesActivosAccionesController.cs` | `HelpDeskApi/HelpDeskApi/Controllers/AsignacionesActivosAccionesController.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Api/Controllers/AsignacionesActivosController.cs` | `HelpDeskApi/HelpDeskApi/Controllers/AsignacionesActivosController.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Api/Controllers/ComentariosController.cs` | `HelpDeskApi/HelpDeskApi/Controllers/ComentariosController.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Api/Controllers/TicketsActivosController.cs` | `HelpDeskApi/HelpDeskApi/Controllers/TicketsActivosController.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Api/Controllers/TiposActivosController.cs` | `HelpDeskApi/HelpDeskApi/Controllers/TiposActivosController.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Domain/Dtos/Activos/ActivosResponseDto.cs` | `HelpDeskApi/HelpDesk.Domain/Dtos/Activos/ActivosResponseDto.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Domain/Dtos/Adjuntos/AdjuntosResponseDto.cs` | `HelpDeskApi/HelpDesk.Domain/Dtos/Adjuntos/AdjuntosResponseDto.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Domain/Dtos/AsignacionesActivos/AsignacionesActivosResponseDto.cs` | `HelpDeskApi/HelpDesk.Domain/Dtos/AsignacionesActivos/AsignacionesActivosResponseDto.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Domain/Dtos/Comentarios/ComentariosResponseDto.cs` | `HelpDeskApi/HelpDesk.Domain/Dtos/Comentarios/ComentariosResponseDto.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Domain/Dtos/TicketsActivos/TicketsActivosResponseDto.cs` | `HelpDeskApi/HelpDesk.Domain/Dtos/TicketsActivos/TicketsActivosResponseDto.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Domain/Dtos/TiposActivos/TiposActivosResponseDto.cs` | `HelpDeskApi/HelpDesk.Domain/Dtos/TiposActivos/TiposActivosResponseDto.cs` | Pendiente |

### Inventario de Stiven Yerai

| Archivo de plantilla | Destino en el equipo | Estado |
|---|---|---|
| `HelpDeskBackend/HekDesk.Application/UseCases/Areas/Commands/ActualizarAreasCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Areas/Commands/ActualizarAreasCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/Areas/Commands/CrearAreasCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Areas/Commands/CrearAreasCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/Areas/Commands/EliminarAreasCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Areas/Commands/EliminarAreasCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/Areas/Queries/ListarAreasQuery.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Areas/Queries/ListarAreasQuery.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/Areas/Queries/ObtenerAreasQuery.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Areas/Queries/ObtenerAreasQuery.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/EnviosNotificaciones/Queries/ListarEnviosNotificacionesQuery.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/EnviosNotificaciones/Queries/ListarEnviosNotificacionesQuery.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/EnviosNotificaciones/Queries/ObtenerEnviosNotificacionesQuery.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/EnviosNotificaciones/Queries/ObtenerEnviosNotificacionesQuery.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/EquiposSoporte/Commands/ActualizarEquiposSoporteCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/EquiposSoporte/Commands/ActualizarEquiposSoporteCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/EquiposSoporte/Commands/CrearEquiposSoporteCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/EquiposSoporte/Commands/CrearEquiposSoporteCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/EquiposSoporte/Commands/EliminarEquiposSoporteCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/EquiposSoporte/Commands/EliminarEquiposSoporteCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/EquiposSoporte/Queries/ListarEquiposSoporteQuery.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/EquiposSoporte/Queries/ListarEquiposSoporteQuery.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/EquiposSoporte/Queries/ObtenerEquiposSoporteQuery.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/EquiposSoporte/Queries/ObtenerEquiposSoporteQuery.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/IntegrantesEquipo/Commands/ActualizarIntegrantesEquipoCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/IntegrantesEquipo/Commands/ActualizarIntegrantesEquipoCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/IntegrantesEquipo/Commands/CrearIntegrantesEquipoCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/IntegrantesEquipo/Commands/CrearIntegrantesEquipoCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/IntegrantesEquipo/Commands/EliminarIntegrantesEquipoCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/IntegrantesEquipo/Commands/EliminarIntegrantesEquipoCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/IntegrantesEquipo/Queries/ListarIntegrantesEquipoQuery.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/IntegrantesEquipo/Queries/ListarIntegrantesEquipoQuery.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/IntegrantesEquipo/Queries/ObtenerIntegrantesEquipoQuery.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/IntegrantesEquipo/Queries/ObtenerIntegrantesEquipoQuery.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/Notificaciones/Commands/ActualizarNotificacionesCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Notificaciones/Commands/ActualizarNotificacionesCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/Notificaciones/Commands/CrearNotificacionesCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Notificaciones/Commands/CrearNotificacionesCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/Notificaciones/Commands/MarcarNotificacionLeidaCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Notificaciones/Commands/MarcarNotificacionLeidaCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/Notificaciones/Queries/ListarNotificacionesQuery.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Notificaciones/Queries/ListarNotificacionesQuery.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/Notificaciones/Queries/ObtenerNotificacionesQuery.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Notificaciones/Queries/ObtenerNotificacionesQuery.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/PreferenciasNotificaciones/Commands/ActualizarPreferenciasNotificacionesCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/PreferenciasNotificaciones/Commands/ActualizarPreferenciasNotificacionesCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/PreferenciasNotificaciones/Commands/CrearPreferenciasNotificacionesCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/PreferenciasNotificaciones/Commands/CrearPreferenciasNotificacionesCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/PreferenciasNotificaciones/Commands/EliminarPreferenciasNotificacionesCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/PreferenciasNotificaciones/Commands/EliminarPreferenciasNotificacionesCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/PreferenciasNotificaciones/Queries/ListarPreferenciasNotificacionesQuery.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/PreferenciasNotificaciones/Queries/ListarPreferenciasNotificacionesQuery.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/PreferenciasNotificaciones/Queries/ObtenerPreferenciasNotificacionesQuery.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/PreferenciasNotificaciones/Queries/ObtenerPreferenciasNotificacionesQuery.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/Sedes/Commands/ActualizarSedesCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Sedes/Commands/ActualizarSedesCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/Sedes/Commands/CrearSedesCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Sedes/Commands/CrearSedesCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/Sedes/Commands/EliminarSedesCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Sedes/Commands/EliminarSedesCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/Sedes/Queries/ListarSedesQuery.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Sedes/Queries/ListarSedesQuery.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/Sedes/Queries/ObtenerSedesQuery.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Sedes/Queries/ObtenerSedesQuery.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Api/Controllers/AreasController.cs` | `HelpDeskApi/HelpDeskApi/Controllers/AreasController.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Api/Controllers/EnviosNotificacionesController.cs` | `HelpDeskApi/HelpDeskApi/Controllers/EnviosNotificacionesController.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Api/Controllers/EquiposSoporteController.cs` | `HelpDeskApi/HelpDeskApi/Controllers/EquiposSoporteController.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Api/Controllers/IntegrantesEquipoController.cs` | `HelpDeskApi/HelpDeskApi/Controllers/IntegrantesEquipoController.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Api/Controllers/NotificacionesAccionesController.cs` | `HelpDeskApi/HelpDeskApi/Controllers/NotificacionesAccionesController.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Api/Controllers/NotificacionesController.cs` | `HelpDeskApi/HelpDeskApi/Controllers/NotificacionesController.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Api/Controllers/PreferenciasNotificacionesController.cs` | `HelpDeskApi/HelpDeskApi/Controllers/PreferenciasNotificacionesController.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Api/Controllers/SedesController.cs` | `HelpDeskApi/HelpDeskApi/Controllers/SedesController.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Domain/Dtos/Areas/AreasResponseDto.cs` | `HelpDeskApi/HelpDesk.Domain/Dtos/Areas/AreasResponseDto.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Domain/Dtos/EnviosNotificaciones/EnviosNotificacionesResponseDto.cs` | `HelpDeskApi/HelpDesk.Domain/Dtos/EnviosNotificaciones/EnviosNotificacionesResponseDto.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Domain/Dtos/EquiposSoporte/EquiposSoporteResponseDto.cs` | `HelpDeskApi/HelpDesk.Domain/Dtos/EquiposSoporte/EquiposSoporteResponseDto.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Domain/Dtos/IntegrantesEquipo/IntegrantesEquipoResponseDto.cs` | `HelpDeskApi/HelpDesk.Domain/Dtos/IntegrantesEquipo/IntegrantesEquipoResponseDto.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Domain/Dtos/Notificaciones/NotificacionesResponseDto.cs` | `HelpDeskApi/HelpDesk.Domain/Dtos/Notificaciones/NotificacionesResponseDto.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Domain/Dtos/PreferenciasNotificaciones/PreferenciasNotificacionesResponseDto.cs` | `HelpDeskApi/HelpDesk.Domain/Dtos/PreferenciasNotificaciones/PreferenciasNotificacionesResponseDto.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Domain/Dtos/Sedes/SedesResponseDto.cs` | `HelpDeskApi/HelpDesk.Domain/Dtos/Sedes/SedesResponseDto.cs` | Pendiente |

### Inventario de Ederson Vilca

| Archivo de plantilla | Destino en el equipo | Estado |
|---|---|---|
| `HelpDeskBackend/HekDesk.Application/UseCases/ArticulosConocimiento/Commands/ActualizarArticulosConocimientoCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/ArticulosConocimiento/Commands/ActualizarArticulosConocimientoCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/ArticulosConocimiento/Commands/CrearArticulosConocimientoCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/ArticulosConocimiento/Commands/CrearArticulosConocimientoCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/ArticulosConocimiento/Commands/EliminarArticulosConocimientoCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/ArticulosConocimiento/Commands/EliminarArticulosConocimientoCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/ArticulosConocimiento/Queries/ListarArticulosConocimientoQuery.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/ArticulosConocimiento/Queries/ListarArticulosConocimientoQuery.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/ArticulosConocimiento/Queries/ObtenerArticulosConocimientoQuery.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/ArticulosConocimiento/Queries/ObtenerArticulosConocimientoQuery.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/CategoriasConocimiento/Commands/ActualizarCategoriasConocimientoCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/CategoriasConocimiento/Commands/ActualizarCategoriasConocimientoCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/CategoriasConocimiento/Commands/CrearCategoriasConocimientoCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/CategoriasConocimiento/Commands/CrearCategoriasConocimientoCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/CategoriasConocimiento/Commands/EliminarCategoriasConocimientoCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/CategoriasConocimiento/Commands/EliminarCategoriasConocimientoCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/CategoriasConocimiento/Queries/ListarCategoriasConocimientoQuery.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/CategoriasConocimiento/Queries/ListarCategoriasConocimientoQuery.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/CategoriasConocimiento/Queries/ObtenerCategoriasConocimientoQuery.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/CategoriasConocimiento/Queries/ObtenerCategoriasConocimientoQuery.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/CategoriasServicios/Commands/ActualizarCategoriasServiciosCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/CategoriasServicios/Commands/ActualizarCategoriasServiciosCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/CategoriasServicios/Commands/CrearCategoriasServiciosCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/CategoriasServicios/Commands/CrearCategoriasServiciosCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/CategoriasServicios/Commands/EliminarCategoriasServiciosCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/CategoriasServicios/Commands/EliminarCategoriasServiciosCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/CategoriasServicios/Queries/ListarCategoriasServiciosQuery.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/CategoriasServicios/Queries/ListarCategoriasServiciosQuery.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/CategoriasServicios/Queries/ObtenerCategoriasServiciosQuery.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/CategoriasServicios/Queries/ObtenerCategoriasServiciosQuery.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/Encuestas/Commands/CrearEncuestasCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Encuestas/Commands/CrearEncuestasCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/Encuestas/Queries/ListarEncuestasQuery.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Encuestas/Queries/ListarEncuestasQuery.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/Encuestas/Queries/ObtenerEncuestasQuery.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Encuestas/Queries/ObtenerEncuestasQuery.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/PoliticasAtencion/Commands/ActualizarPoliticasAtencionCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/PoliticasAtencion/Commands/ActualizarPoliticasAtencionCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/PoliticasAtencion/Commands/CrearPoliticasAtencionCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/PoliticasAtencion/Commands/CrearPoliticasAtencionCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/PoliticasAtencion/Commands/EliminarPoliticasAtencionCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/PoliticasAtencion/Commands/EliminarPoliticasAtencionCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/PoliticasAtencion/Queries/ListarPoliticasAtencionQuery.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/PoliticasAtencion/Queries/ListarPoliticasAtencionQuery.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/PoliticasAtencion/Queries/ObtenerPoliticasAtencionQuery.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/PoliticasAtencion/Queries/ObtenerPoliticasAtencionQuery.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/Servicios/Commands/ActualizarServiciosCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Servicios/Commands/ActualizarServiciosCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/Servicios/Commands/CrearServiciosCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Servicios/Commands/CrearServiciosCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/Servicios/Commands/EliminarServiciosCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Servicios/Commands/EliminarServiciosCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/Servicios/Queries/ListarServiciosQuery.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Servicios/Queries/ListarServiciosQuery.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/Servicios/Queries/ObtenerServiciosQuery.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Servicios/Queries/ObtenerServiciosQuery.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/TicketsArticulos/Commands/ActualizarTicketsArticulosCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/TicketsArticulos/Commands/ActualizarTicketsArticulosCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/TicketsArticulos/Commands/CrearTicketsArticulosCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/TicketsArticulos/Commands/CrearTicketsArticulosCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/TicketsArticulos/Commands/EliminarTicketsArticulosCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/TicketsArticulos/Commands/EliminarTicketsArticulosCommand.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/TicketsArticulos/Queries/ListarTicketsArticulosQuery.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/TicketsArticulos/Queries/ListarTicketsArticulosQuery.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/TicketsArticulos/Queries/ObtenerTicketsArticulosQuery.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/TicketsArticulos/Queries/ObtenerTicketsArticulosQuery.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Api/Controllers/ArticulosConocimientoController.cs` | `HelpDeskApi/HelpDeskApi/Controllers/ArticulosConocimientoController.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Api/Controllers/CategoriasConocimientoController.cs` | `HelpDeskApi/HelpDeskApi/Controllers/CategoriasConocimientoController.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Api/Controllers/CategoriasServiciosController.cs` | `HelpDeskApi/HelpDeskApi/Controllers/CategoriasServiciosController.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Api/Controllers/EncuestasController.cs` | `HelpDeskApi/HelpDeskApi/Controllers/EncuestasController.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Api/Controllers/PoliticasAtencionController.cs` | `HelpDeskApi/HelpDeskApi/Controllers/PoliticasAtencionController.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Api/Controllers/ServiciosController.cs` | `HelpDeskApi/HelpDeskApi/Controllers/ServiciosController.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Api/Controllers/TicketsArticulosController.cs` | `HelpDeskApi/HelpDeskApi/Controllers/TicketsArticulosController.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Domain/Dtos/ArticulosConocimiento/ArticulosConocimientoResponseDto.cs` | `HelpDeskApi/HelpDesk.Domain/Dtos/ArticulosConocimiento/ArticulosConocimientoResponseDto.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Domain/Dtos/CategoriasConocimiento/CategoriasConocimientoResponseDto.cs` | `HelpDeskApi/HelpDesk.Domain/Dtos/CategoriasConocimiento/CategoriasConocimientoResponseDto.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Domain/Dtos/CategoriasServicios/CategoriasServiciosResponseDto.cs` | `HelpDeskApi/HelpDesk.Domain/Dtos/CategoriasServicios/CategoriasServiciosResponseDto.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Domain/Dtos/Encuestas/EncuestasResponseDto.cs` | `HelpDeskApi/HelpDesk.Domain/Dtos/Encuestas/EncuestasResponseDto.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Domain/Dtos/PoliticasAtencion/PoliticasAtencionResponseDto.cs` | `HelpDeskApi/HelpDesk.Domain/Dtos/PoliticasAtencion/PoliticasAtencionResponseDto.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Domain/Dtos/Servicios/ServiciosResponseDto.cs` | `HelpDeskApi/HelpDesk.Domain/Dtos/Servicios/ServiciosResponseDto.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Domain/Dtos/TicketsArticulos/TicketsArticulosResponseDto.cs` | `HelpDeskApi/HelpDesk.Domain/Dtos/TicketsArticulos/TicketsArticulosResponseDto.cs` | Pendiente |

### Inventario de Brahan Quispe

| Archivo de plantilla | Destino en el equipo | Estado |
|---|---|---|
| `.gitattributes` | `.gitattributes` | Coincide |
| `.gitignore` | `.gitignore` | Coincide |
| `001_helpdesk_36_tablas_es.sql` | `001_helpdesk_36_tablas_es.sql` | Pendiente |
| `DOCUMENTACION_PROYECTO.md` | `DOCUMENTACION_PROYECTO.md` | Pendiente |
| `HelpDeskBackend/DISTRIBUCION_EQUIPO.md` | `HelpDeskApi/DISTRIBUCION_EQUIPO.md` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/Class1.cs` | `HelpDeskApi/HelpDesk.Application/Class1.cs` | Existe: revisar |
| `HelpDeskBackend/HekDesk.Application/Common/Validation.cs` | `HelpDeskApi/HelpDesk.Application/Common/Validation.cs` | Coincide |
| `HelpDeskBackend/HekDesk.Application/Configuration/ApplicationServicesExtensions.cs` | `HelpDeskApi/HelpDesk.Application/Configuration/ApplicationServicesExtensions.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/HekDesk.Application.csproj` | `HelpDeskApi/HelpDesk.Application/HekDesk.Application.csproj` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/Security/AccessRules.cs` | `HelpDeskApi/HelpDesk.Application/Security/AccessRules.cs` | Coincide |
| `HelpDeskBackend/HekDesk.Application/UseCases/AsignacionesTickets/Queries/ListarAsignacionesTicketsQuery.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/AsignacionesTickets/Queries/ListarAsignacionesTicketsQuery.cs` | Coincide |
| `HelpDeskBackend/HekDesk.Application/UseCases/AsignacionesTickets/Queries/ObtenerAsignacionesTicketsQuery.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/AsignacionesTickets/Queries/ObtenerAsignacionesTicketsQuery.cs` | Coincide |
| `HelpDeskBackend/HekDesk.Application/UseCases/Auditoria/Queries/ListarAuditoriaQuery.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Auditoria/Queries/ListarAuditoriaQuery.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/Auditoria/Queries/ObtenerAuditoriaQuery.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Auditoria/Queries/ObtenerAuditoriaQuery.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/CargaTecnicos/Queries/ListarCargaTecnicosQuery.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/CargaTecnicos/Queries/ListarCargaTecnicosQuery.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/HistorialTickets/Queries/ListarHistorialTicketsQuery.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/HistorialTickets/Queries/ListarHistorialTicketsQuery.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/HistorialTickets/Queries/ObtenerHistorialTicketsQuery.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/HistorialTickets/Queries/ObtenerHistorialTicketsQuery.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/Reportes/Queries/IndicadoresQuery.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Reportes/Queries/IndicadoresQuery.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/ResumenTickets/Queries/ListarResumenTicketsQuery.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/ResumenTickets/Queries/ListarResumenTicketsQuery.cs` | Pendiente |
| `HelpDeskBackend/HekDesk.Application/UseCases/Tickets/Commands/ActualizarTicketsCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Tickets/Commands/ActualizarTicketsCommand.cs` | Coincide |
| `HelpDeskBackend/HekDesk.Application/UseCases/Tickets/Commands/AsignarTicketCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Tickets/Commands/AsignarTicketCommand.cs` | Coincide |
| `HelpDeskBackend/HekDesk.Application/UseCases/Tickets/Commands/CerrarTicketCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Tickets/Commands/CerrarTicketCommand.cs` | Coincide |
| `HelpDeskBackend/HekDesk.Application/UseCases/Tickets/Commands/CrearTicketsCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Tickets/Commands/CrearTicketsCommand.cs` | Coincide |
| `HelpDeskBackend/HekDesk.Application/UseCases/Tickets/Commands/IniciarTicketCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Tickets/Commands/IniciarTicketCommand.cs` | Coincide |
| `HelpDeskBackend/HekDesk.Application/UseCases/Tickets/Commands/ResolverTicketCommand.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Tickets/Commands/ResolverTicketCommand.cs` | Coincide |
| `HelpDeskBackend/HekDesk.Application/UseCases/Tickets/Queries/ListarTicketsQuery.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Tickets/Queries/ListarTicketsQuery.cs` | Coincide |
| `HelpDeskBackend/HekDesk.Application/UseCases/Tickets/Queries/ObtenerTicketsQuery.cs` | `HelpDeskApi/HelpDesk.Application/UseCases/Tickets/Queries/ObtenerTicketsQuery.cs` | Coincide |
| `HelpDeskBackend/HelpDesk.Api/Controllers/AsignacionesTicketsController.cs` | `HelpDeskApi/HelpDeskApi/Controllers/AsignacionesTicketsController.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Api/Controllers/AuditoriaController.cs` | `HelpDeskApi/HelpDeskApi/Controllers/AuditoriaController.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Api/Controllers/CargaTecnicosController.cs` | `HelpDeskApi/HelpDeskApi/Controllers/CargaTecnicosController.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Api/Controllers/HistorialTicketsController.cs` | `HelpDeskApi/HelpDeskApi/Controllers/HistorialTicketsController.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Api/Controllers/ReportesController.cs` | `HelpDeskApi/HelpDeskApi/Controllers/ReportesController.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Api/Controllers/ResumenTicketsController.cs` | `HelpDeskApi/HelpDeskApi/Controllers/ResumenTicketsController.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Api/Controllers/TicketsAccionesController.cs` | `HelpDeskApi/HelpDeskApi/Controllers/TicketsAccionesController.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Api/Controllers/TicketsController.cs` | `HelpDeskApi/HelpDeskApi/Controllers/TicketsController.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Api/HelpDesk.Api.csproj` | `HelpDeskApi/HelpDeskApi/HelpDeskApi.csproj` | Existe: revisar |
| `HelpDeskBackend/HelpDesk.Api/HelpDeskBackend.http` | `HelpDeskApi/HelpDeskApi/HelpDeskBackend.http` | Pendiente |
| `HelpDeskBackend/HelpDesk.Api/Middleware/ExceptionMiddleware.cs` | `HelpDeskApi/HelpDeskApi/Middleware/ExceptionMiddleware.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Api/Program.cs` | `HelpDeskApi/HelpDeskApi/Program.cs` | Existe: revisar |
| `HelpDeskBackend/HelpDesk.Api/Properties/launchSettings.json` | `HelpDeskApi/HelpDeskApi/Properties/launchSettings.json` | Existe: revisar |
| `HelpDeskBackend/HelpDesk.Api/appsettings.Development.json` | `HelpDeskApi/HelpDeskApi/appsettings.Development.json` | Coincide |
| `HelpDeskBackend/HelpDesk.Api/appsettings.json` | `HelpDeskApi/HelpDeskApi/appsettings.json` | Coincide |
| `HelpDeskBackend/HelpDesk.Domain/Class1.cs` | `HelpDeskApi/HelpDesk.Domain/Class1.cs` | Coincide |
| `HelpDeskBackend/HelpDesk.Domain/Dtos/AsignacionesTickets/AsignacionesTicketsResponseDto.cs` | `HelpDeskApi/HelpDesk.Domain/Dtos/AsignacionesTickets/AsignacionesTicketsResponseDto.cs` | Coincide |
| `HelpDeskBackend/HelpDesk.Domain/Dtos/Auditoria/AuditoriaResponseDto.cs` | `HelpDeskApi/HelpDesk.Domain/Dtos/Auditoria/AuditoriaResponseDto.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Domain/Dtos/Autenticacion/RegistroDto.cs` | `HelpDeskApi/HelpDesk.Domain/Dtos/Autenticacion/RegistroDto.cs` | Coincide |
| `HelpDeskBackend/HelpDesk.Domain/Dtos/Autenticacion/SesionDto.cs` | `HelpDeskApi/HelpDesk.Domain/Dtos/Autenticacion/SesionDto.cs` | Coincide |
| `HelpDeskBackend/HelpDesk.Domain/Dtos/Autenticacion/UsuarioRegistradoDto.cs` | `HelpDeskApi/HelpDesk.Domain/Dtos/Autenticacion/UsuarioRegistradoDto.cs` | Coincide |
| `HelpDeskBackend/HelpDesk.Domain/Dtos/CargaTecnicos/CargaTecnicosResponseDto.cs` | `HelpDeskApi/HelpDesk.Domain/Dtos/CargaTecnicos/CargaTecnicosResponseDto.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Domain/Dtos/Common/AccionResultadoDto.cs` | `HelpDeskApi/HelpDesk.Domain/Dtos/Common/AccionResultadoDto.cs` | Coincide |
| `HelpDeskBackend/HelpDesk.Domain/Dtos/Common/OperacionDto.cs` | `HelpDeskApi/HelpDesk.Domain/Dtos/Common/OperacionDto.cs` | Coincide |
| `HelpDeskBackend/HelpDesk.Domain/Dtos/Common/PaginaDto.cs` | `HelpDeskApi/HelpDesk.Domain/Dtos/Common/PaginaDto.cs` | Coincide |
| `HelpDeskBackend/HelpDesk.Domain/Dtos/Common/ResponseDto.cs` | `HelpDeskApi/HelpDesk.Domain/Dtos/Common/ResponseDto.cs` | Coincide |
| `HelpDeskBackend/HelpDesk.Domain/Dtos/HistorialTickets/HistorialTicketsResponseDto.cs` | `HelpDeskApi/HelpDesk.Domain/Dtos/HistorialTickets/HistorialTicketsResponseDto.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Domain/Dtos/Reportes/IndicadoresDto.cs` | `HelpDeskApi/HelpDesk.Domain/Dtos/Reportes/IndicadoresDto.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Domain/Dtos/ResumenTickets/ResumenTicketsResponseDto.cs` | `HelpDeskApi/HelpDesk.Domain/Dtos/ResumenTickets/ResumenTicketsResponseDto.cs` | Pendiente |
| `HelpDeskBackend/HelpDesk.Domain/Dtos/Tickets/TicketsResponseDto.cs` | `HelpDeskApi/HelpDesk.Domain/Dtos/Tickets/TicketsResponseDto.cs` | Coincide |
| `HelpDeskBackend/HelpDesk.Domain/Exceptions/BusinessException.cs` | `HelpDeskApi/HelpDesk.Domain/Exceptions/BusinessException.cs` | Coincide |
| `HelpDeskBackend/HelpDesk.Domain/HelpDesk.Domain.csproj` | `HelpDeskApi/HelpDesk.Domain/HelpDesk.Domain.csproj` | Coincide |
| `HelpDeskBackend/HelpDesk.Domain/Models/Activos.cs` | `HelpDeskApi/HelpDesk.Domain/Models/Activos.cs` | Coincide |
| `HelpDeskBackend/HelpDesk.Domain/Models/Adjuntos.cs` | `HelpDeskApi/HelpDesk.Domain/Models/Adjuntos.cs` | Coincide |
| `HelpDeskBackend/HelpDesk.Domain/Models/Aprobaciones.cs` | `HelpDeskApi/HelpDesk.Domain/Models/Aprobaciones.cs` | Coincide |
| `HelpDeskBackend/HelpDesk.Domain/Models/Areas.cs` | `HelpDeskApi/HelpDesk.Domain/Models/Areas.cs` | Coincide |
| `HelpDeskBackend/HelpDesk.Domain/Models/ArticulosConocimiento.cs` | `HelpDeskApi/HelpDesk.Domain/Models/ArticulosConocimiento.cs` | Coincide |
| `HelpDeskBackend/HelpDesk.Domain/Models/AsignacionesActivos.cs` | `HelpDeskApi/HelpDesk.Domain/Models/AsignacionesActivos.cs` | Coincide |
| `HelpDeskBackend/HelpDesk.Domain/Models/AsignacionesTickets.cs` | `HelpDeskApi/HelpDesk.Domain/Models/AsignacionesTickets.cs` | Coincide |
| `HelpDeskBackend/HelpDesk.Domain/Models/Auditoria.cs` | `HelpDeskApi/HelpDesk.Domain/Models/Auditoria.cs` | Coincide |
| `HelpDeskBackend/HelpDesk.Domain/Models/Cambios.cs` | `HelpDeskApi/HelpDesk.Domain/Models/Cambios.cs` | Coincide |
| `HelpDeskBackend/HelpDesk.Domain/Models/CargaTecnicos.cs` | `HelpDeskApi/HelpDesk.Domain/Models/CargaTecnicos.cs` | Coincide |
| `HelpDeskBackend/HelpDesk.Domain/Models/CategoriasConocimiento.cs` | `HelpDeskApi/HelpDesk.Domain/Models/CategoriasConocimiento.cs` | Coincide |
| `HelpDeskBackend/HelpDesk.Domain/Models/CategoriasServicios.cs` | `HelpDeskApi/HelpDesk.Domain/Models/CategoriasServicios.cs` | Coincide |
| `HelpDeskBackend/HelpDesk.Domain/Models/Comentarios.cs` | `HelpDeskApi/HelpDesk.Domain/Models/Comentarios.cs` | Coincide |
| `HelpDeskBackend/HelpDesk.Domain/Models/Encuestas.cs` | `HelpDeskApi/HelpDesk.Domain/Models/Encuestas.cs` | Coincide |
| `HelpDeskBackend/HelpDesk.Domain/Models/EnviosNotificaciones.cs` | `HelpDeskApi/HelpDesk.Domain/Models/EnviosNotificaciones.cs` | Coincide |
| `HelpDeskBackend/HelpDesk.Domain/Models/EquiposSoporte.cs` | `HelpDeskApi/HelpDesk.Domain/Models/EquiposSoporte.cs` | Coincide |
| `HelpDeskBackend/HelpDesk.Domain/Models/HistorialTickets.cs` | `HelpDeskApi/HelpDesk.Domain/Models/HistorialTickets.cs` | Coincide |
| `HelpDeskBackend/HelpDesk.Domain/Models/IntegrantesEquipo.cs` | `HelpDeskApi/HelpDesk.Domain/Models/IntegrantesEquipo.cs` | Coincide |
| `HelpDeskBackend/HelpDesk.Domain/Models/Notificaciones.cs` | `HelpDeskApi/HelpDesk.Domain/Models/Notificaciones.cs` | Coincide |
| `HelpDeskBackend/HelpDesk.Domain/Models/Permisos.cs` | `HelpDeskApi/HelpDesk.Domain/Models/Permisos.cs` | Coincide |
| `HelpDeskBackend/HelpDesk.Domain/Models/PoliticasAtencion.cs` | `HelpDeskApi/HelpDesk.Domain/Models/PoliticasAtencion.cs` | Coincide |
| `HelpDeskBackend/HelpDesk.Domain/Models/PreferenciasNotificaciones.cs` | `HelpDeskApi/HelpDesk.Domain/Models/PreferenciasNotificaciones.cs` | Coincide |
| `HelpDeskBackend/HelpDesk.Domain/Models/Problemas.cs` | `HelpDeskApi/HelpDesk.Domain/Models/Problemas.cs` | Coincide |
| `HelpDeskBackend/HelpDesk.Domain/Models/RecuperacionesClave.cs` | `HelpDeskApi/HelpDesk.Domain/Models/RecuperacionesClave.cs` | Coincide |
| `HelpDeskBackend/HelpDesk.Domain/Models/ResumenTickets.cs` | `HelpDeskApi/HelpDesk.Domain/Models/ResumenTickets.cs` | Coincide |
| `HelpDeskBackend/HelpDesk.Domain/Models/Roles.cs` | `HelpDeskApi/HelpDesk.Domain/Models/Roles.cs` | Coincide |
| `HelpDeskBackend/HelpDesk.Domain/Models/Sedes.cs` | `HelpDeskApi/HelpDesk.Domain/Models/Sedes.cs` | Coincide |
| `HelpDeskBackend/HelpDesk.Domain/Models/Servicios.cs` | `HelpDeskApi/HelpDesk.Domain/Models/Servicios.cs` | Coincide |
| `HelpDeskBackend/HelpDesk.Domain/Models/TareasCambios.cs` | `HelpDeskApi/HelpDesk.Domain/Models/TareasCambios.cs` | Coincide |
| `HelpDeskBackend/HelpDesk.Domain/Models/Tickets.cs` | `HelpDeskApi/HelpDesk.Domain/Models/Tickets.cs` | Coincide |
| `HelpDeskBackend/HelpDesk.Domain/Models/TicketsArticulos.cs` | `HelpDeskApi/HelpDesk.Domain/Models/TicketsArticulos.cs` | Coincide |
| `HelpDeskBackend/HelpDesk.Domain/Models/TiposActivos.cs` | `HelpDeskApi/HelpDesk.Domain/Models/TiposActivos.cs` | Coincide |
| `HelpDeskBackend/HelpDesk.Domain/Models/Usuarios.cs` | `HelpDeskApi/HelpDesk.Domain/Models/Usuarios.cs` | Coincide |
| `HelpDeskBackend/HelpDesk.Domain/Models/UsuariosRoles.cs` | `HelpDeskApi/HelpDesk.Domain/Models/UsuariosRoles.cs` | Coincide |
| `HelpDeskBackend/HelpDesk.Domain/Ports/AccessToken.cs` | `HelpDeskApi/HelpDesk.Domain/Ports/AccessToken.cs` | Coincide |
| `HelpDeskBackend/HelpDesk.Domain/Ports/ICurrentUser.cs` | `HelpDeskApi/HelpDesk.Domain/Ports/ICurrentUser.cs` | Coincide |
| `HelpDeskBackend/HelpDesk.Domain/Ports/IGenericRepository.cs` | `HelpDeskApi/HelpDesk.Domain/Ports/IGenericRepository.cs` | Coincide |
| `HelpDeskBackend/HelpDesk.Domain/Ports/IPasswordService.cs` | `HelpDeskApi/HelpDesk.Domain/Ports/IPasswordService.cs` | Coincide |
| `HelpDeskBackend/HelpDesk.Domain/Ports/ITokenService.cs` | `HelpDeskApi/HelpDesk.Domain/Ports/ITokenService.cs` | Coincide |
| `HelpDeskBackend/HelpDesk.Domain/Ports/IUnitOfWork.cs` | `HelpDeskApi/HelpDesk.Domain/Ports/IUnitOfWork.cs` | Coincide |
| `HelpDeskBackend/HelpDesk.Infrastructure/Class1.cs` | `HelpDeskApi/HelpDesk.Infrastructure/Class1.cs` | Coincide |
| `HelpDeskBackend/HelpDesk.Infrastructure/Configuration/InfrastructureServicesExtensions.cs` | `HelpDeskApi/HelpDesk.Infrastructure/Configuration/InfrastuctureServicesExtensions.cs` | Coincide |
| `HelpDeskBackend/HelpDesk.Infrastructure/Data/HelpDeskDbContext.Custom.cs` | `HelpDeskApi/HelpDesk.Infrastructure/Data/HelpDeskDbContext.Custom.cs` | Coincide |
| `HelpDeskBackend/HelpDesk.Infrastructure/Data/HelpDeskDbContext.cs` | `HelpDeskApi/HelpDesk.Infrastructure/Data/HelpDeskDbContext.cs` | Coincide |
| `HelpDeskBackend/HelpDesk.Infrastructure/HelpDesk.Infrastructure.csproj` | `HelpDeskApi/HelpDesk.Infrastructure/HelpDesk.Infrastructure.csproj` | Existe: revisar |
| `HelpDeskBackend/HelpDesk.Infrastructure/Repositories/GenericRepository.cs` | `HelpDeskApi/HelpDesk.Infrastructure/Repositories/GenericRepository.cs` | Coincide |
| `HelpDeskBackend/HelpDesk.Infrastructure/Repositories/UnitOfWork.cs` | `HelpDeskApi/HelpDesk.Infrastructure/Repositories/UnitOfWork.cs` | Coincide |
| `HelpDeskBackend/HelpDesk.Infrastructure/Services/JwtService.cs` | `HelpDeskApi/HelpDesk.Infrastructure/Services/JwtService.cs` | Coincide |
| `HelpDeskBackend/HelpDesk.Infrastructure/Services/PasswordService.cs` | `HelpDeskApi/HelpDesk.Infrastructure/Services/PasswordService.cs` | Coincide |
| `HelpDeskBackend/HelpDeskBackend.sln` | `HelpDeskApi/HelpDeskApi.sln` | Existe: revisar |
| `HelpDeskBackend/PRIMERA_VERSION_API.md` | `HelpDeskApi/PRIMERA_VERSION_API.md` | Pendiente |
| `HelpDeskBackend/PRIMER_AVANCE.md` | `HelpDeskApi/PRIMER_AVANCE.md` | Pendiente |
| `HelpDeskBackend/SCAFFOLDING_HELPDESK.md` | `HelpDeskApi/SCAFFOLDING_HELPDESK.md` | Pendiente |
| `HelpDeskBackend/scripts/Actualizar-Modelo.ps1` | `HelpDeskApi/scripts/Actualizar-Modelo.ps1` | Pendiente |
| `LEEME_BASE_DATOS_36_TABLAS.md` | `LEEME_BASE_DATOS_36_TABLAS.md` | Pendiente |
| `LEEME_BASE_DATOS_ES.md` | `LEEME_BASE_DATOS_ES.md` | Pendiente |
| `README.md` | `README.md` | Existe: revisar |

## Criterio de entrega completa

Cada fila pendiente debe incorporarse y cada diferencia debe revisarse por su responsable. Si se decide retirar un archivo vacío, redundante o histórico, registrar esa decisión en un commit y actualizar el inventario; no dejarlo sin dueño. Los archivos de planificación histórica no habilitan funciones nuevas ni obligan a implementar una segunda versión.

Para revisar una nueva entrega, comparar el árbol del equipo con el inventario y comprobar las adaptaciones de nombres. Todos los archivos de esta primera versión deben quedar incorporados o con una decisión explícita documentada. La revisión final y el cierre de pendientes corresponden a Brahan.
