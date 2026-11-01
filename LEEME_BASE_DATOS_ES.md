# Base de datos HelpDesk: versión simplificada

Esta es una **alternativa de 25 tablas con nombres en español**. Sustituye la propuesta de 52 tablas para comenzar con un alcance más fácil de comprender. No convierte ni modifica una instalación anterior.

## Qué archivo elegir

- `001_helpdesk_simplificado_es.sql`: propuesta recomendada para el equipo en esta etapa.
- `001_helpdesk_postgresql.sql`: propuesta anterior, conservada como referencia.

**Elegir solo una versión para una base vacía.** Ambas crean el esquema `helpdesk`. Si ya ejecutaste la anterior, no ejecutes la nueva en esa misma base esperando que la transforme. No se incluye ninguna instrucción para borrar tus datos.

## ¿Conviene usar español?

Sí, si es el idioma que comprende el equipo. La calidad depende de relaciones, reglas y funcionamiento, no del idioma de los nombres. Mantener una convención consistente: `usuarios`, `id_usuario`, `fecha_creacion`; sin espacios ni tildes.

Las palabras propias de SQL siguen en inglés: `CREATE TABLE`, `SELECT`, `REFERENCES`. C# y las librerías también mantienen sus palabras y nombres originales. Las tablas y los campos del negocio pueden estar en español.

## Las 25 tablas explicadas

| Tabla | Para qué sirve |
|---|---|
| sedes | Locales o establecimientos de la empresa. |
| areas | Departamentos, como contabilidad o recursos humanos. |
| usuarios | Personas que utilizan el sistema. |
| roles | Responsabilidades: empleado, técnico, coordinador, aprobador y administrador. |
| usuarios_roles | Relaciona cada persona con uno o varios roles. |
| equipos_soporte | Grupos de técnicos responsables de atender solicitudes. |
| integrantes_equipo | Indica qué personas pertenecen a cada grupo. |
| servicios | Tipos de atención que ofrece TI. |
| politicas_atencion | Plazos de respuesta y solución por prioridad. |
| tickets | Solicitudes e incidentes registrados. |
| historial_tickets | Cambios de estado, técnico, prioridad y sus motivos. |
| comentarios | Conversación y notas internas de un ticket. |
| adjuntos | Datos de capturas y documentos asociados. |
| notificaciones | Avisos destinados a cada usuario. |
| articulos_conocimiento | Instrucciones y soluciones reutilizables. |
| activos | Computadoras, impresoras y otros bienes tecnológicos. |
| asignaciones_activos | Quién tiene cada activo y sus responsables anteriores. |
| tickets_activos | Qué activos están involucrados en un ticket. |
| problemas | Investigación de fallas recurrentes. |
| problemas_tickets | Qué tickets están asociados a una investigación. |
| cambios | Modificaciones planificadas de equipos o servicios. |
| cambios_tickets | Qué tickets justifican o se relacionan con un cambio. |
| aprobaciones | Autorizaciones solicitadas para tickets o cambios. |
| encuestas | Calificación y opinión del solicitante sobre la atención. |
| auditoria | Registro de acciones relevantes en toda la plataforma. |

## Por qué hay tablas que parecen repetidas

`usuarios_roles` es necesaria porque una persona puede ser técnico y aprobador a la vez. `tickets_activos` permite que un incidente involucre varias computadoras y que una computadora acumule varios tickets. Son tablas de relación: evitan guardar listas de identificadores dentro de un solo campo.

Un equipo de soporte es un **grupo de personas**. Un activo es un **bien físico**. Los nombres distintos evitan confundirlos.

## Cómo quedan los 16 módulos

| Módulo | Tablas principales |
|---|---|
| 1. Usuarios y acceso | usuarios, roles, usuarios_roles |
| 2. Organización | sedes, areas, equipos_soporte, integrantes_equipo |
| 3. Catálogo | servicios |
| 4. Tickets | tickets, historial_tickets |
| 5. Asignación | tickets, integrantes_equipo, historial_tickets |
| 6. Comunicación | comentarios, adjuntos |
| 7. Tiempos de atención | politicas_atencion y fechas de tickets |
| 8. Notificaciones | notificaciones |
| 9. Conocimiento | articulos_conocimiento |
| 10. Activos | activos, asignaciones_activos, tickets_activos |
| 11. Problemas | problemas, problemas_tickets |
| 12. Cambios | cambios, cambios_tickets |
| 13. Aprobaciones | aprobaciones |
| 14. Satisfacción | encuestas |
| 15. Reportes | Consultas sobre las tablas existentes |
| 16. Auditoría | auditoria |

## Qué se simplificó

- Los permisos se programan por rol en .NET. No se ofrece una pantalla para inventar permisos nuevos.
- Los tiempos de atención se cuentan de manera continua, incluso de noche y fines de semana. No hay calendarios laborales ni pausas que suspendan el reloj.
- Los cambios de responsable se guardan en el historial del ticket, sin otra tabla de asignaciones de tickets.
- Las aprobaciones se registran directamente por paso. No hay plantillas ni delegaciones. Se admite un proceso por ticket o cambio; repetirlo requerirá ampliar el modelo.
- Los avisos tienen un estado básico de correo; la entrega requiere código .NET. No hay infraestructura avanzada de reintentos.
- Las categorías de servicios y artículos son campos de texto. El frontend debe ofrecer opciones consistentes.
- No hay tablas específicas para tokens de recuperación, versiones de artículos o relación entre artículos y tickets. Esas funciones quedan fuera de esta primera versión o requieren una ampliación posterior.
- El historial y la auditoría se insertan desde .NET, junto con el cambio correspondiente. No se generan automáticamente desde PostgreSQL.

Las tablas mantienen claves foráneas, restricciones y los índices principales. La reducción es de alcance y complejidad; no significa que se pueda omitir la validación de permisos y reglas.

## Qué desarrollar primero

1. Usuarios, roles y organización.
2. Equipos de soporte y servicios.
3. Definir los plazos de las cuatro prioridades en `politicas_atencion`.
4. Registrar un ticket, asignarlo y responderlo.
5. Resolverlo y cerrarlo, guardando el historial.
6. Agregar progresivamente los demás módulos.

## Relación con la documentación anterior

`DOCUMENTACION_PROYECTO.md` conserva la planificación original y contiene funcionalidades avanzadas que **no están incluidas en este esquema simplificado**. Para empezar con esta alternativa, este LEEME describe sus límites. Antes de repartir tareas finales, ajustar el alcance funcional del documento principal a esta versión.

## Estado de validación

Se revisó la sintaxis con un analizador PostgreSQL y las referencias de tablas y columnas de forma estática. No se ejecutó el archivo, no se abrió una conexión a la base y no se comprobaron resultados en un servidor real.
