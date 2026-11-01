# HelpDesk: propuesta actual de 36 tablas en español

Esta versión recupera funciones útiles de la propuesta grande y mantiene nombres que el equipo puede entender. Contiene **36 tablas y 2 vistas de reportes**, para los 16 módulos del proyecto.

## Archivo que debemos utilizar

**`001_helpdesk_36_tablas_es.sql`** es la propuesta actual. Las variantes anteriores de 25 y 52 tablas se conservan como referencia y sus guías ya no describen la opción elegida.

Ejecutar el archivo completo conectado a una base vacía en PostgreSQL 16 o superior. El script crea el esquema `helpdesk`. No crea la base, no contiene usuarios ni contraseñas y no elimina objetos existentes.

**No ejecutar los tres archivos en la misma base.** Esta versión no convierte una instalación anterior: si ya existe el esquema `helpdesk`, falla sin sobrescribirlo. Una migración de datos existentes requeriría un trabajo separado.

## Las 36 tablas

| N.º | Tabla | Explicación |
|---|---|---|
| 1 | sedes | Locales de la empresa. |
| 2 | areas | Departamentos que pertenecen a cada sede. |
| 3 | usuarios | Personas que utilizan el sistema. |
| 4 | roles | Responsabilidades: empleado, técnico, coordinador, aprobador y administrador. |
| 5 | usuarios_roles | Roles asignados a cada persona. |
| 6 | equipos_soporte | Grupos de técnicos; no son computadoras. |
| 7 | integrantes_equipo | Personas que forman cada grupo de soporte. |
| 8 | servicios | Atenciones disponibles, como instalación de programas. |
| 9 | politicas_atencion | Plazos de primera respuesta y resolución por prioridad. |
| 10 | tickets | Incidentes y solicitudes registrados. |
| 11 | historial_tickets | Cambios de estado, prioridad y otros eventos del caso. |
| 12 | comentarios | Respuestas al usuario y notas internas. |
| 13 | adjuntos | Datos de capturas y documentos; los archivos se guardan fuera de PostgreSQL. |
| 14 | notificaciones | Avisos para cada usuario. |
| 15 | articulos_conocimiento | Instrucciones y soluciones documentadas. |
| 16 | activos | Computadoras, impresoras y otros bienes tecnológicos. |
| 17 | asignaciones_activos | Responsable actual y anteriores de cada bien. |
| 18 | tickets_activos | Activos involucrados en cada ticket. |
| 19 | problemas | Investigación de causas de fallas recurrentes. |
| 20 | problemas_tickets | Casos relacionados con una investigación. |
| 21 | cambios | Modificaciones planificadas de equipos o servicios. |
| 22 | cambios_tickets | Casos relacionados con un cambio. |
| 23 | aprobaciones | Autorizaciones ordenadas para tickets o cambios. |
| 24 | encuestas | Calificación de la atención, de 1 a 5. |
| 25 | auditoria | Acciones importantes en toda la plataforma. |
| 26 | permisos | Acciones que el sistema reconoce, como asignar tickets. |
| 27 | roles_permisos | Qué acciones puede realizar cada rol. |
| 28 | categorias_servicios | Clasificación uniforme de los servicios. |
| 29 | categorias_conocimiento | Clasificación de artículos y guías. |
| 30 | tipos_activos | Clases de bienes: laptop, monitor, impresora, etc. |
| 31 | asignaciones_tickets | Historial estructurado de equipos y técnicos responsables. |
| 32 | tickets_articulos | Soluciones documentadas relacionadas con cada caso. |
| 33 | recuperaciones_clave | Solicitudes de recuperación con un token temporal protegido. |
| 34 | preferencias_notificaciones | Avisos que cada persona desea recibir. |
| 35 | envios_notificaciones | Estado e intentos de envío de cada correo. |
| 36 | tareas_cambios | Actividades necesarias para ejecutar un cambio. |

## Los 16 módulos y sus tablas

| Módulo | Tablas principales |
|---|---|
| 1. Usuarios y acceso | usuarios, roles, usuarios_roles, permisos, roles_permisos, recuperaciones_clave |
| 2. Organización | sedes, areas, equipos_soporte, integrantes_equipo |
| 3. Catálogo de servicios | servicios, categorias_servicios |
| 4. Tickets | tickets, historial_tickets |
| 5. Asignación y carga | asignaciones_tickets, responsables actuales en tickets |
| 6. Comunicación | comentarios, adjuntos |
| 7. Tiempos de atención | politicas_atencion y compromisos copiados en tickets |
| 8. Notificaciones | notificaciones, preferencias_notificaciones, envios_notificaciones |
| 9. Conocimiento | articulos_conocimiento, categorias_conocimiento, tickets_articulos |
| 10. Activos | activos, tipos_activos, asignaciones_activos, tickets_activos |
| 11. Problemas | problemas, problemas_tickets |
| 12. Cambios TI | cambios, cambios_tickets, tareas_cambios |
| 13. Aprobaciones | aprobaciones |
| 14. Satisfacción | encuestas |
| 15. Reportes | Consultas y vistas sobre los datos existentes |
| 16. Auditoría | auditoria |

## Relaciones que conviene entender

- Una persona puede tener varios roles y cada rol varios permisos. Por eso existen tablas de relación.
- El responsable actual del ticket se consulta en `tickets`; sus responsables anteriores se consultan en `asignaciones_tickets`. El backend guarda ambos cambios en la misma transacción.
- `historial_tickets` explica lo ocurrido en un caso. `auditoria` registra acciones de toda la plataforma, como cambiar permisos de un usuario.
- Un incidente es una falla concreta; un problema es la investigación de su causa recurrente; un cambio es una modificación planificada para corregir o mejorar algo.
- La aprobación se relaciona con un ticket o con un cambio mediante claves reales. Cada fila es un paso y solo su aprobador puede decidirlo, según las validaciones de la API.

## Qué sigue simplificado

- SLA significa compromiso de atención: se cuentan minutos corridos, incluyendo noches y fines de semana. No hay calendario laboral ni pausas del reloj.
- Las aprobaciones tienen un proceso por ticket o cambio, sin plantillas, delegaciones ni nuevas rondas. Si se necesita repetir un proceso, se ampliará el modelo.
- Las notificaciones tienen seguimiento de correo con reintentos limitados; el worker y la integración con correo se programan en .NET. No se garantiza entrega exactamente una vez.
- No hay versiones de artículos ni formularios configurables por servicio.
- La base no autoriza a los usuarios del producto por sí sola: .NET debe comprobar permisos y acceso por propietario/equipo.
- Historial, auditoría, versión del ticket y coherencia de asignaciones los mantiene .NET mediante transacciones. No hay triggers ocultos.

## Qué incluye el script y qué no

Incluye relaciones, restricciones, índices, roles, permisos iniciales, categorías, tipos de activo y las vistas `resumen_tickets` y `carga_tecnicos`.

No incluye cuentas, contraseñas ni plazos de atención ficticios. Después de instalar, se configuran sedes, áreas, usuarios, equipos, servicios y las cuatro políticas de atención.

Para generar clases con EF Core se debe seleccionar el esquema `helpdesk`. Las clases del negocio pueden conservar nombres en español; las palabras de C#, SQL y librerías mantienen sus nombres originales.

## Orden de desarrollo

1. Usuarios, roles, permisos y organización.
2. Catálogo, equipos y políticas de atención.
3. Un recorrido completo: registrar, asignar, responder, resolver y cerrar.
4. Historial y auditoría desde ese primer recorrido.
5. Notificaciones, conocimiento, activos y aprobaciones.
6. Problemas, cambios, encuestas y reportes.

## Documentación y validación

Esta guía sustituye las decisiones de base de datos de las propuestas anteriores. El documento general conserva valor como contexto, pero debe leerse con estos límites de alcance antes de repartir tareas.

La sintaxis SQL y las referencias se comprobaron estáticamente. El archivo no se ejecutó, no se abrió una conexión y no se probaron operaciones en un servidor PostgreSQL.
