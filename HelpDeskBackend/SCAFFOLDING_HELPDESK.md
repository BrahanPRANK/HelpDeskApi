# Scaffolding de HelpDesk

Se generó el modelo desde la base `helpdeskdb`, esquema `helpdesk`, utilizando EF Core Database First. La conexión se reutilizó del laboratorio cambiando la base destino. No se aplicaron scripts SQL ni se modificaron datos durante el scaffolding.

## Archivos del proyecto

- `HelpDesk.Domain/Models/`: clases generadas en español.
- `HelpDesk.Infrastructure/Data/HelpDeskDbContext.cs`: relaciones, columnas, restricciones representables e índices compatibles.
- `HelpDesk.Infrastructure/Configuration/InfrastructureServicesExtensions.cs`: registro del contexto para PostgreSQL.
- `HelpDesk.Api/Program.cs`: invoca el registro de Infrastructure.
- `scripts/Actualizar-Modelo.ps1`: comando reproducible de generación y compilación.

Se conservó el nombre existente `HekDesk.Application`. No se implementaron controladores de negocio, JWT, repositorios ni casos de uso en esta etapa.

## Paquetes y herramientas

| Ubicación | Dependencia | Versión |
|---|---|---|
| Infrastructure | Microsoft.EntityFrameworkCore | 10.0.13 |
| Infrastructure | Microsoft.EntityFrameworkCore.Relational | 10.0.13 |
| Infrastructure | Microsoft.EntityFrameworkCore.Design | 10.0.13, privada |
| Infrastructure | Npgsql.EntityFrameworkCore.PostgreSQL | 10.0.3 |
| API | Microsoft.EntityFrameworkCore.Design | 10.0.13, privada |
| Herramienta global existente | dotnet-ef | 10.0.13 |

Se fijó Relational explícitamente para que Infrastructure y sus consumidores utilicen la misma versión en ejecución.

## Configuración local

La conexión está en **User Secrets de la API**, con la clave `ConnectionStrings:DefaultConnection`. No está en los modelos, en `OnConfiguring` ni en el script. User Secrets es almacenamiento local de desarrollo, no un almacén cifrado de producción.

La API carga los secretos en el entorno Development. En otros entornos se debe proporcionar la conexión mediante la configuración del despliegue; por ejemplo, la variable `ConnectionStrings__DefaultConnection`.

## Cobertura del modelo

La base tiene **36 tablas y 2 vistas**. Se generaron **34 clases**: 32 de tablas y 2 de vistas. Las otras cuatro tablas son relaciones de varios a varios que EF representa automáticamente en el contexto:

- `roles_permisos`
- `tickets_activos`
- `problemas_tickets`
- `cambios_tickets`

Estas tablas están mapeadas; no faltan en el modelo aunque no tengan un archivo C# independiente. Las vistas `resumen_tickets` y `carga_tecnicos` son modelos sin clave destinados a consulta.

## Avisos del generador

EF Core no representa los índices de expresión `uq_usuarios_nombre` y `uq_usuarios_correo`, que aplican unicidad mediante `lower(...)`. Los índices **siguen existiendo y funcionando en PostgreSQL**. Su definición permanece en el script SQL; no se deben eliminar por no aparecer como índices del modelo C#.

El esquema se administra mediante scripts SQL, no mediante migraciones de EF. La lógica de permisos, estados, historial y concurrencia sigue siendo responsabilidad del backend.

## Regenerar después de un cambio de esquema

Desde la carpeta de la solución:

```powershell
.\scripts\Actualizar-Modelo.ps1 -Sobrescribir
```

El script establece Development durante su ejecución, usa la conexión por nombre, selecciona el esquema `helpdesk` y evita escribir credenciales mediante `--no-onconfiguring`.

`-Sobrescribir` reemplaza archivos generados. Revisar los cambios antes de usarlo y mantener reglas manuales en archivos parciales separados. El script no elimina clases antiguas cuando desaparece una tabla; revisar esos cambios manualmente.

## Verificación

- Compilación de la solución.
- Comprobación de cobertura del modelo EF: 36 tablas y 2 vistas.
- Consultas de solo lectura sobre usuarios, tickets, roles, permisos y ambas vistas.
- Verificación de que el contexto no contiene `OnConfiguring` ni una conexión incrustada.

Estas verificaciones no comprueban todavía los procesos de negocio, que no se han implementado.
