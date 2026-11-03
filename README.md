# HelpDesk API

Proyecto académico de soporte TI desarrollado con .NET 10, PostgreSQL y Entity Framework Core. Primera versión del backend con CQRS mediante MediatR, DTOs, Repository, Unit of Work, autenticación JWT, hash de contraseñas y autorización por roles y permisos.

Esta plantilla se simplificó siguiendo Lab11. Ver [el inventario del primer avance](HelpDeskBackend/PRIMER_AVANCE.md) antes de copiar casos de uso al repositorio del equipo.

## Empezar

**Leer primero [la guía de instalación y el reparto del equipo](HelpDeskBackend/DISTRIBUCION_EQUIPO.md).** Explica PostgreSQL, creación de la base, User Secrets, scaffolding, administrador inicial, Swagger y responsabilidades de los seis integrantes.

Requisitos: .NET SDK 10.0, PostgreSQL 16 o superior y una herramienta como pgAdmin. Para regenerar modelos se utiliza `dotnet-ef` 10.0.13.

1. Crear una base local vacía llamada `helpdeskdb`.
2. Ejecutar **[001_helpdesk_36_tablas_es.sql](001_helpdesk_36_tablas_es.sql)** completo en esa base. Crea el esquema `helpdesk` con 36 tablas y 2 vistas. Se aplica una sola vez; no es un script para reemplazar una base existente.
3. Configurar en User Secrets de `HelpDesk.Api` la conexión y los valores JWT, siguiendo la guía. Los secretos no están incluidos en Git.
4. Desde la carpeta de la solución:

```powershell
cd HelpDeskBackend
dotnet restore HelpDeskBackend.sln
dotnet build HelpDeskBackend.sln
.\scripts\Crear-Administrador.ps1 -NombreUsuario admin -Correo tu-correo@dominio.com
dotnet run --project HelpDesk.Api --launch-profile http
```

El script de administrador pide una contraseña oculta y se utiliza una sola vez por base. No hay una cuenta con contraseña predeterminada.

Swagger: [http://localhost:5014/](http://localhost:5014/). Iniciar sesión en `POST /api/auth/login` y pegar `data.token` en **Authorize** para probar endpoints protegidos.

## Organización

```text
HelpDeskBackend/
├── HelpDesk.Api/             Controladores, middleware, Swagger y arranque
├── HekDesk.Application/      Comandos, consultas y handlers
├── HelpDesk.Domain/          Modelos, interfaces y Dtos/{Entidad}
├── HelpDesk.Infrastructure/  EF Core, repositorios, Unit of Work y seguridad
├── scripts/                 Scaffolding y administrador inicial
└── tests/                   Pruebas de integración
```

`HekDesk.Application` conserva el nombre que tiene actualmente el proyecto. Cada handler implementa su caso de uso, maneja errores con `try/catch` y confirma las escrituras mediante `IUnitOfWork.Complete()`.

Los modelos y el contexto ya se incluyen. **No hace falta ejecutar scaffolding al clonar si la base usa el esquema actual.** Se regenera únicamente tras cambios de esquema coordinados.

## Documentación

- [Instalación y distribución de responsabilidades](HelpDeskBackend/DISTRIBUCION_EQUIPO.md).
- [Inventario de código del primer avance](HelpDeskBackend/PRIMER_AVANCE.md).
- [Endpoints, acciones y límites de la primera versión](HelpDeskBackend/PRIMERA_VERSION_API.md).
- [Scaffolding y configuración de modelos](HelpDeskBackend/SCAFFOLDING_HELPDESK.md).
- [Explicación de las 36 tablas](LEEME_BASE_DATOS_36_TABLAS.md).
- [Planificación general del proyecto](DOCUMENTACION_PROYECTO.md).

`DOCUMENTACION_PROYECTO.md` conserva propuestas de alcance más avanzadas. `LEEME_BASE_DATOS_ES.md` es una referencia histórica de la alternativa de 25 tablas; **no describe el esquema elegido**. La implementación actual se basa en el SQL de 36 tablas y en la guía de primera versión.

## Verificación

```powershell
dotnet build HelpDeskBackend.sln
dotnet run --project tests/HelpDesk.IntegrationTests
```

Ejecutar desde `HelpDeskBackend`. La suite comprueba el alcance simplificado y requiere permiso para crear y eliminar una base temporal `helpdesk_test_*`. Usa el SQL de la carpeta superior y no escribe en `helpdeskdb`. La API debe estar detenida al compilar para evitar archivos bloqueados.

## Alcance actual

CRUD, flujo básico de tickets, aprobaciones, devolución de activos, usuarios y consultas de reportes. La auditoría automática, la publicación de conocimiento, el ciclo avanzado de cambios y la recuperación de contraseña quedan para otra entrega.

Todavía no incluye frontend, envío real de correo ni carga física de archivos. Los adjuntos manejan metadatos y los tiempos de atención se cuentan en minutos corridos. Los detalles y restricciones se describen en la guía de primera versión.

Las credenciales, claves JWT y tokens pertenecen a la configuración local o al entorno de despliegue. No subirlos al repositorio.
