# HelpDesk Enterprise

## Documento de definición y planificación del proyecto

| Dato | Valor |
|---|---|
| Versión | 1.0 |
| Fecha | 1 de noviembre de 2026 |
| Estado | Propuesta general acordada; especificación técnica inicial para revisión |
| Curso | Desarrollo de aplicaciones empresariales con .NET; confirmar denominación oficial |
| Equipo | 8 integrantes; nombres y responsables pendientes |
| Directorio del proyecto | `C:\Users\ASUS\Documents\HelpDesk` |
| Arquitectura de referencia | `C:\Users\ASUS\RiderProjects\Lab11-BrahanQuispe` |

Este documento es la referencia inicial compartida para organizar el desarrollo. Distingue acuerdos de la conversación, propuestas de diseño y decisiones pendientes. No acredita que existan código, tablas, despliegues o funcionalidades implementadas.

## Índice

1. [Definición y objetivos](#1-definición-y-objetivos)
2. [Alcance y decisiones](#2-alcance-y-decisiones)
3. [Actores y autorización](#3-actores-y-autorización)
4. [Especificación de los 16 módulos](#4-especificación-de-los-16-módulos)
5. [Procesos y reglas de negocio](#5-procesos-y-reglas-de-negocio)
6. [Arquitectura y organización del código](#6-arquitectura-y-organización-del-código)
7. [Diseño inicial de datos](#7-diseño-inicial-de-datos)
8. [Contratos de API e interfaz](#8-contratos-de-api-e-interfaz)
9. [Requisitos no funcionales](#9-requisitos-no-funcionales)
10. [Plan de ejecución y equipo](#10-plan-de-ejecución-y-equipo)
11. [Pruebas y aceptación](#11-pruebas-y-aceptación)
12. [Riesgos, pendientes y próximos pasos](#12-riesgos-pendientes-y-próximos-pasos)

## 1. Definición y objetivos

### 1.1 Descripción

HelpDesk Enterprise será una plataforma web de soporte TI para una empresa con múltiples sedes y departamentos. Permitirá a los empleados registrar incidentes y solicitudes de servicio, y a los equipos técnicos gestionar su atención, responsables, tiempos, evidencias y resultados.

El sistema conectará la atención diaria con el inventario tecnológico, el conocimiento reutilizable, las aprobaciones, la investigación de problemas recurrentes y los cambios TI.

### 1.2 Problema que busca resolver

La atención mediante conversaciones dispersas, correo o registros manuales dificulta conocer qué solicitudes están pendientes, quién las atiende, cuánto tardan y si la solución fue satisfactoria. También impide conservar un historial consistente y analizar las fallas recurrentes.

### 1.3 Objetivo general

Desarrollar una plataforma empresarial de gestión de soporte TI que centralice incidentes y solicitudes, controle el proceso de atención y permita evaluar la calidad del servicio mediante trazabilidad, compromisos de atención e indicadores.

### 1.4 Objetivos específicos

- Implementar acceso por roles, permisos y ámbito de información.
- Gestionar el ciclo de vida de incidentes y solicitudes de servicio.
- Organizar equipos, colas de atención y asignaciones.
- Controlar tiempos de primera respuesta y resolución mediante SLA.
- Relacionar tickets con activos, artículos, problemas y cambios.
- Incorporar aprobaciones y encuestas de satisfacción.
- Proporcionar reportes y auditoría de las acciones relevantes.
- Aplicar una arquitectura consistente con el laboratorio de referencia.

## 2. Alcance y decisiones

### 2.1 Base acordada

| Aspecto | Definición |
|---|---|
| Tipo de producto | Aplicación web empresarial de soporte TI |
| Módulos | 16 módulos funcionales |
| Equipo previsto | 8 integrantes |
| Organización técnica | API, Application, Domain e Infrastructure |
| Patrón de casos de uso | Commands, Queries y Handlers con MediatR |
| Persistencia | Entity Framework Core, Repository y Unit of Work |
| Base de datos | PostgreSQL |
| Autenticación | JWT |
| Gestión del esquema | Database First, scripts SQL versionados y scaffolding |
| Referencia | Lab11-BrahanQuispe, con ajustes para el alcance empresarial |

### 2.2 Supuestos de planificación

- Una sola empresa; varias sedes y departamentos. Requiere confirmación funcional.
- Una aplicación backend desplegable con organización interna por módulos.
- Una base relacional compartida con límites de responsabilidad por módulo.
- Frontend web separado; framework todavía pendiente.
- Ocho semanas como escenario ilustrativo; no es una fecha de entrega comprometida.
- .NET 10 como punto de partida porque el laboratorio apunta a `net10.0`; confirmar herramientas y alojamiento antes de fijar versiones del proyecto.

### 2.3 Límites de la primera entrega

La primera entrega completa incluirá una versión delimitada y demostrable de los 16 módulos. Los módulos complejos tendrán flujos sencillos y explícitos.

Quedan fuera del alcance inicial: múltiples empresas independientes, facturación, compras y depreciación de activos, descubrimiento automático de equipos, control remoto, chatbot con IA, aplicación móvil nativa y diseñador visual de flujos. Estas funciones podrán evaluarse como ampliaciones.

## 3. Actores y autorización

### 3.1 Roles iniciales

| Rol | Responsabilidad | Ámbito inicial propuesto |
|---|---|---|
| Empleado | Solicitar soporte, responder y evaluar | Sus propios tickets y contenido publicado |
| Técnico | Diagnosticar, atender y resolver | Tickets asignados y los de su equipo según permiso |
| Coordinador | Distribuir trabajo y supervisar atención | Equipos que tenga a cargo |
| Aprobador | Autorizar o rechazar solicitudes | Aprobaciones asignadas; información mínima relacionada |
| Administrador | Configurar plataforma y acceso | Administración global según permisos |

Una persona puede tener varios roles. El rol aprobador no concede acceso general a tickets ni facultades de administración. El acceso global de administración no sustituye las reglas que exigen una acción técnica o una aprobación.

### 3.2 Matriz inicial de permisos

| Acción | Empleado | Técnico | Coordinador | Administrador |
|---|---|---|---|---|
| Crear ticket propio | Sí | Sí | Sí | Sí |
| Consultar tickets | Propios | Según asignación/equipo | Equipos a cargo | Global con permiso |
| Asignar o reasignar | No | Solo con permiso específico | Sí, en su ámbito | Con permiso |
| Registrar diagnóstico y solución | No | En casos autorizados | Con permiso de atención | Con permiso de atención |
| Confirmar solución | Como solicitante | Como solicitante | Como solicitante | Como solicitante |
| Gestionar usuarios y roles | No | No | No | Sí |
| Consultar auditoría global | No | No | No | Con permiso |

Los controles se ejecutan en el servidor. Ocultar botones en la interfaz no reemplaza la autorización. Para cada operación se comprobarán tanto el permiso como el acceso al registro concreto.

## 4. Especificación de los 16 módulos

Los identificadores MOD-01 a MOD-16 permiten relacionar requisitos, tareas y pruebas. Los nombres de entidades son propuestas iniciales.

### MOD-01. Usuarios y control de acceso

- **Objetivo:** administrar identidades y capacidades de acceso.
- **Funciones:** inicio de sesión, gestión y desactivación de cuentas, roles, permisos y recuperación de contraseña.
- **Reglas:** no exponer hashes; desactivar una cuenta sin borrar su historial; asignar roles solo con permiso administrativo; recuperación mediante token temporal de un solo uso.
- **Datos:** User, Role, UserRole, Permission, RolePermission, PasswordResetToken.
- **Dependencias:** correo para recuperación; auditoría para acciones administrativas.
- **Aceptación:** una cuenta válida accede; una cuenta inactiva no puede iniciar sesión; un empleado no puede asignarse roles. La estrategia para invalidar tokens ya emitidos debe definirse antes de cerrar este módulo.

### MOD-02. Estructura organizacional

- **Objetivo:** representar sedes, departamentos y equipos de soporte.
- **Funciones:** administrar estructura, integrantes, coordinadores y pertenencia de usuarios.
- **Reglas:** desactivar elementos sin perder referencias históricas; comprobar que el técnico pertenece al equipo al asignarlo.
- **Datos:** Site, Department, SupportTeam, TeamMember.
- **Dependencias:** usuarios.
- **Aceptación:** un coordinador consulta y administra las colas de sus equipos sin acceder a equipos ajenos.

### MOD-03. Catálogo de servicios

- **Objetivo:** organizar lo que el área TI ofrece a los empleados.
- **Funciones:** categorías, servicios, descripción, requisitos, equipo responsable, política SLA y necesidad de aprobación.
- **Reglas:** impedir nuevas solicitudes de servicios inactivos; conservar los tickets existentes; fijar inicialmente requisitos mediante campos definidos, sin constructor dinámico de formularios.
- **Datos:** ServiceCategory, Service; referencias a equipos y políticas.
- **Dependencias:** estructura, SLA y aprobaciones según servicio.
- **Aceptación:** elegir un servicio permite identificar equipo responsable y condiciones de atención.

### MOD-04. Gestión de tickets

- **Objetivo:** administrar incidentes y solicitudes durante todo su ciclo de vida.
- **Funciones:** crear, consultar, filtrar, actualizar información permitida, iniciar atención, resolver, cerrar, reabrir y cancelar.
- **Reglas:** número único; transiciones controladas; identidad del actor desde la sesión; solución obligatoria al resolver; conservación de historial.
- **Datos:** Ticket, TicketHistory.
- **Dependencias:** usuarios, organización y servicios.
- **Aceptación:** un empleado registra un caso, el equipo lo atiende y el solicitante confirma su cierre con trazabilidad completa.

### MOD-05. Asignación y carga de trabajo

- **Objetivo:** distribuir y supervisar el trabajo técnico.
- **Funciones:** colas por equipo, asignación manual, reasignación, disponibilidad básica y conteo de tickets activos.
- **Reglas:** solo asignar técnicos activos del equipo autorizado; registrar responsable anterior, nuevo responsable, actor, fecha y motivo.
- **Datos:** TicketAssignment y referencias actuales en Ticket.
- **Dependencias:** tickets, equipos y usuarios.
- **Aceptación:** una reasignación actualiza la cola y conserva el historial en la misma operación de guardado.

### MOD-06. Comunicación y evidencias

- **Objetivo:** concentrar conversación y documentación del caso.
- **Funciones:** respuestas públicas, notas internas y archivos adjuntos.
- **Reglas:** el solicitante no accede a notas internas; autor derivado de la sesión; descarga con autorización; restricciones de tamaño y tipo de archivo.
- **Datos:** TicketComment, Attachment.
- **Dependencias:** tickets, usuarios y almacenamiento de archivos.
- **Aceptación:** una nota interna no aparece en respuestas, descargas ni vistas del empleado.

### MOD-07. SLA y escalamiento

- **Objetivo:** controlar compromisos de primera respuesta y resolución.
- **Funciones:** políticas por servicio/prioridad, calendario laboral, excepciones, pausas y alertas de vencimiento.
- **Reglas:** conservar compromisos del ticket aunque cambie la política; documentar pausas; evitar alertas duplicadas; calcular conforme a zona horaria y horario configurados.
- **Datos:** SlaPolicy, SlaTarget, BusinessCalendar, BusinessCalendarException, TicketSla, SlaPause, SlaEscalation.
- **Dependencias:** tickets, catálogo, equipos y notificaciones.
- **Aceptación:** casos con fines de semana, feriados y pausas generan fechas y tiempos esperados según ejemplos aprobados.

### MOD-08. Notificaciones

- **Objetivo:** informar eventos que requieren conocimiento o acción.
- **Funciones:** bandeja interna, correo, lectura, preferencias y seguimiento de envíos.
- **Reglas:** una falla de correo no debe deshacer la resolución del ticket; limitar reintentos; evitar duplicación por evento y destinatario; respetar visibilidad del contenido.
- **Datos:** Notification, NotificationDelivery, NotificationPreference, OutboxMessage.
- **Dependencias:** usuarios, proveedor de correo y procesamiento en segundo plano.
- **Aceptación:** una asignación genera aviso; un fallo de envío queda registrado y puede reintentarse.

### MOD-09. Base de conocimientos

- **Objetivo:** reutilizar soluciones y facilitar el autoservicio.
- **Funciones:** categorías, búsqueda por texto, borradores, revisión, publicación y relación con tickets.
- **Reglas:** empleados consultan artículos publicados y autorizados; publicación por permiso; conservar autor y fechas.
- **Datos:** KnowledgeCategory, KnowledgeArticle, TicketKnowledgeArticle.
- **Dependencias:** usuarios y tickets para vinculación.
- **Aceptación:** un técnico propone una solución, un responsable la publica y un empleado la encuentra.

### MOD-10. Activos tecnológicos

- **Objetivo:** identificar los equipos involucrados en el soporte.
- **Funciones:** registro, tipo, código patrimonial, serie, ubicación, estado, asignación y tickets relacionados.
- **Reglas:** código patrimonial único; historial de responsables; activos retirados no se asignan; política de múltiples responsables pendiente de confirmación.
- **Datos:** AssetType, Asset, AssetAssignment, TicketAsset.
- **Dependencias:** usuarios, organización y tickets.
- **Aceptación:** un activo muestra su responsable actual y los incidentes asociados sin perder asignaciones anteriores.

### MOD-11. Gestión de problemas

- **Objetivo:** investigar causas de incidentes recurrentes.
- **Funciones:** agrupar tickets, documentar análisis, causa raíz, solución temporal y solución definitiva.
- **Reglas:** cerrar un problema exige conclusión documentada; cerrar un problema no cierra automáticamente los tickets relacionados.
- **Datos:** Problem, ProblemTicket.
- **Dependencias:** tickets; conocimiento y cambios cuando corresponda.
- **Aceptación:** varios incidentes se vinculan a una investigación y se conserva el resultado de esta.

### MOD-12. Gestión de cambios TI

- **Objetivo:** planificar modificaciones que afecten servicios o equipos.
- **Funciones:** solicitud, impacto, riesgo, planificación, aprobación, tareas, implementación, reversión y revisión final.
- **Reglas:** cambios que requieren aprobación no se ejecutan antes de obtenerla; registrar resultado y plan de reversión; ejecución manual en la primera entrega.
- **Datos:** ChangeRequest, ChangeTask, ChangeTicket.
- **Dependencias:** usuarios, tickets y aprobaciones.
- **Aceptación:** un cambio aprobado se ejecuta y termina con resultado verificable y responsable identificado.

### MOD-13. Solicitudes y aprobaciones

- **Objetivo:** autorizar acciones sensibles mediante decisiones registradas.
- **Funciones:** solicitudes de aprobación para tickets o cambios, pasos secuenciales, aprobar y rechazar con motivo.
- **Reglas:** solo decide el aprobador asignado o un reemplazo autorizado; impedir doble decisión; prohibir autoaprobación por defecto; un rechazo detiene el flujo inicial.
- **Datos:** ApprovalRequest, ApprovalStep.
- **Dependencias:** usuarios, catálogo, tickets y cambios.
- **Aceptación:** una solicitud con dos pasos solo queda aprobada después de ambas decisiones en orden.

### MOD-14. Encuestas de satisfacción

- **Objetivo:** evaluar la experiencia del solicitante.
- **Funciones:** calificación de 1 a 5, comentario opcional y consulta de valoraciones bajas.
- **Reglas:** una respuesta por ticket en la primera entrega; solo responde su solicitante después del cierre; definir vigencia de la encuesta.
- **Datos:** SatisfactionSurvey.
- **Dependencias:** tickets y notificaciones.
- **Aceptación:** no se permite responder por otro usuario ni registrar una segunda calificación.

### MOD-15. Reportes e indicadores

- **Objetivo:** evaluar operación y calidad con definiciones consistentes.
- **Funciones:** tickets por estado, prioridad, servicio y equipo; carga activa; tiempos de atención; cumplimiento SLA; satisfacción; exportación CSV inicial.
- **Reglas:** filtros por intervalo y ámbito; distinguir tiempo calendario de tiempo laboral; los datos agregados respetan permisos.
- **Datos:** consultas sobre tablas existentes; sin duplicar información operativa inicialmente.
- **Dependencias:** tickets, SLA, organización y encuestas.
- **Aceptación:** los totales coinciden con un conjunto de datos de prueba conocido.

### MOD-16. Auditoría y trazabilidad

- **Objetivo:** registrar acciones administrativas y de negocio relevantes.
- **Funciones:** búsqueda por actor, fecha, acción y entidad; revisión de cambios de permisos, asignaciones, estados y aprobaciones.
- **Reglas:** registros no editables desde funciones ordinarias; excluir contraseñas y tokens; acceso restringido; establecer retención.
- **Datos:** AuditEvent.
- **Dependencias:** integración transversal con los otros módulos.
- **Aceptación:** una modificación relevante identifica actor, instante, acción, entidad y cambios permitidos, sin registrar secretos.

## 5. Procesos y reglas de negocio

### 5.1 Flujo de atención

```text
Nuevo -> Asignado -> En atención -> Resuelto -> Cerrado
                         |
                         +-> En espera del solicitante -> En atención
                         +-> En espera de aprobación -> En atención

Resuelto -> En atención: reapertura con motivo
Estados activos -> Cancelado: según permiso y motivo
```

La asignación al equipo puede existir desde la creación. El estado Asignado indicará que existe un técnico responsable. Se deberá confirmar este significado al cerrar el diagrama definitivo.

| Operación | Condiciones propuestas | Efectos |
|---|---|---|
| Crear | Servicio activo, datos válidos, actor autorizado | Número, equipo, prioridad, historial y compromisos SLA |
| Asignar | Técnico activo y perteneciente al equipo | Responsable, historial y notificación |
| Iniciar | Ticket asignado y actor autorizado | Inicio de atención registrado |
| Esperar solicitante | Pregunta pública y motivo | Estado de espera y pausa solo si la política la permite |
| Solicitar aprobación | Servicio o acción que la requiere | Solicitud y pasos de aprobación |
| Resolver | Estado permitido y resumen de solución | Resolución, historial y aviso |
| Cerrar | Solución confirmada según política | Cierre y habilitación de encuesta |
| Reabrir | Solicitante autorizado y plazo vigente | Retorno a atención, motivo y ajuste SLA según política |
| Cancelar | Permiso, estado permitido y motivo | Cancelación conservando historial |

El cierre automático, la ventana de reapertura y el comportamiento SLA al reabrir siguen pendientes. No se implementarán con valores arbitrarios.

### 5.2 Impacto, urgencia y prioridad

Matriz inicial propuesta, pendiente de validación:

| Impacto / Urgencia | Alta | Media | Baja |
|---|---|---|---|
| Alto: servicio crítico o varias áreas | Crítica | Alta | Media |
| Medio: departamento o grupo | Alta | Media | Baja |
| Bajo: usuario individual | Media | Baja | Baja |

Modificar manualmente la prioridad requerirá permiso y motivo. Los objetivos de tiempo de cada prioridad se acordarán con ejemplos antes de desarrollar SLA.

### 5.3 Definiciones para indicadores

- **Carga activa:** tickets asignados al técnico que no estén resueltos, cerrados o cancelados; revisar si los resueltos deben contarse antes de aprobar la definición.
- **Primera respuesta:** primera respuesta pública de un técnico autorizado; una nota interna no cuenta.
- **Cumplimiento SLA:** compromisos cumplidos dividido entre compromisos evaluables del periodo. Primera respuesta y resolución se reportan por separado.
- **Tiempo de resolución:** desde creación hasta resolución según el reloj definido por la política; documentar pausas y reaperturas.
- **Satisfacción:** promedio de calificaciones válidas y tasa de respuesta. Mostrar cantidad de respuestas junto al promedio.

## 6. Arquitectura y organización del código

### 6.1 Referencia examinada

La solución Lab11 tiene cuatro proyectos. Application referencia a Domian; Infrastructure referencia a Domian; la API referencia a Application e Infrastructure. Domain no declara dependencias de otros proyectos. Los handlers utilizan MediatR y los puertos; Infrastructure implementa persistencia y servicios.

La revisión realizada fue estática. No se ejecutaron compilaciones ni pruebas HTTP y no se modificó el laboratorio.

### 6.2 Dependencias del proyecto nuevo

```text
HelpDesk.Api ------------> HelpDesk.Application ----> HelpDesk.Domain
     |
     +------------------> HelpDesk.Infrastructure --> HelpDesk.Domain
```

La API compone las implementaciones mediante inyección de dependencias. Los controladores no consultan el DbContext. Application no referencia Infrastructure. Domain no incorpora detalles HTTP, EF Core o del proveedor PostgreSQL.

La separación de Commands y Queries usará una misma base de datos. El despliegue inicial será un monolito organizado por módulos, sin separar cada módulo en un servicio independiente.

### 6.3 Estructura propuesta

```text
HelpDesk/
|-- README.md
|-- docs/
|   |-- requirements/
|   |-- architecture/
|   |-- database/
|   `-- decisions/
|-- database/
|   |-- scripts/
|   `-- seeds/
|-- src/
|   |-- HelpDesk.Api/
|   |   |-- Controllers/
|   |   |-- Configuration/
|   |   |-- Middleware/
|   |   `-- Program.cs
|   |-- HelpDesk.Application/
|   |   |-- Common/Results/
|   |   |-- Common/Behaviors/
|   |   |-- Common/Interfaces/
|   |   `-- UseCases/{Modulo}/Commands|Queries|Dtos/
|   |-- HelpDesk.Domain/
|   |   |-- Models/
|   |   |-- Enums/
|   |   |-- Exceptions/
|   |   `-- Ports/Repositories|Services/
|   `-- HelpDesk.Infrastructure/
|       |-- Data/Generated/
|       |-- Data/Customizations/
|       |-- Repositories/
|       |-- Services/
|       `-- Configuration/
|-- tests/
`-- deployment/
```

La estructura es una propuesta; estas carpetas y proyectos todavía no se han creado. Las barras verticales dentro de rutas representan subcarpetas alternativas, no nombres literales.

### 6.4 Patrón por caso de uso

```text
HTTP -> Controller -> MediatR -> Handler -> Puerto
                                         -> Implementación -> PostgreSQL
```

- **Command:** representa una intención de cambio, por ejemplo ResolveTicketCommand.
- **Query:** representa una lectura, por ejemplo GetMyTicketsQuery.
- **Handler:** coordina autorización, validación, reglas, persistencia y resultado.
- **Domain:** concentra reglas independientes del transporte, especialmente transiciones y condiciones de entidades.
- **Repository:** encapsula acceso persistente. Habrá consultas específicas cuando el repositorio genérico sea insuficiente.
- **Unit of Work:** guarda cambios relacionados de una operación. Se definirán transacciones explícitas para operaciones que requieran varios guardados atómicos.

Comandos iniciales de tickets: CreateTicket, AssignTicket, StartTicket, PutTicketOnHold, ResolveTicket, CloseTicket, ReopenTicket y CancelTicket. La modificación del asunto no permitirá cambiar libremente el estado.

### 6.5 Adaptaciones frente al laboratorio

1. Corregir nombres Domian y Repostories en la solución nueva.
2. Ubicar DTOs y resultados de casos de uso en Application, separados del dominio.
3. Traducir resultados a códigos HTTP en la API, sin StatusCode en Domain.
4. Incorporar identidad del usuario actual y autorización por recurso.
5. Centralizar errores inesperados, correlación y logging; conservar validaciones de negocio explícitas.
6. Propagar CancellationToken hasta consultas y guardado.
7. Agregar paginación y proyección de consultas.
8. Sustituir actualizaciones libres del estado por operaciones de negocio.
9. Excluir la compatibilidad del laboratorio con contraseñas en texto plano.
10. Incorporar concurrencia, auditoría e historial desde las primeras operaciones.

### 6.6 Trabajos en segundo plano

El envío de correo y la evaluación periódica de vencimientos necesitarán ejecución en segundo plano. Se definirá su hospedaje antes de implementarlos. Una bandeja de salida transaccional propuesta permitirá guardar el evento pendiente junto con el cambio del ticket y procesarlo después, con reintentos e idempotencia.

## 7. Diseño inicial de datos

### 7.1 Método Database First

1. Aprobar modelo conceptual y reglas.
2. Definir tablas, claves, relaciones, restricciones e índices.
3. Escribir scripts SQL numerados e inmutables una vez aplicados.
4. Registrar qué scripts se aplicaron a cada entorno.
5. Aplicar y verificar en desarrollo.
6. Ejecutar scaffolding con alcance controlado.
7. Revisar diferencias y comprobar compilación y comportamiento.

Los scripts SQL serán la fuente de evolución del esquema. No se alternará informalmente entre scripts y migraciones EF para modificar las mismas tablas.

Las entidades y el contexto generados se mantendrán identificables. Las extensiones manuales se escribirán en archivos separados mediante clases parciales y configuraciones compatibles. No se regenerará código sin revisar qué archivos se sobrescriben. Las invariantes tendrán métodos y validaciones de aplicación, además de restricciones de datos cuando correspondan.

### 7.2 Inventario conceptual

| Área | Entidades propuestas |
|---|---|
| Identidad | User, Role, UserRole, Permission, RolePermission, PasswordResetToken |
| Organización | Site, Department, SupportTeam, TeamMember |
| Servicios | ServiceCategory, Service |
| Tickets | Ticket, TicketHistory, TicketAssignment |
| Comunicación | TicketComment, Attachment |
| SLA | SlaPolicy, SlaTarget, BusinessCalendar, BusinessCalendarException, TicketSla, SlaPause, SlaEscalation |
| Notificaciones | Notification, NotificationDelivery, NotificationPreference, OutboxMessage |
| Conocimiento | KnowledgeCategory, KnowledgeArticle, TicketKnowledgeArticle |
| Activos | AssetType, Asset, AssetAssignment, TicketAsset |
| Problemas | Problem, ProblemTicket |
| Cambios | ChangeRequest, ChangeTask, ChangeTicket |
| Aprobaciones | ApprovalRequest, ApprovalStep |
| Satisfacción | SatisfactionSurvey |
| Auditoría | AuditEvent |

Es un inventario lógico preliminar. No constituye un esquema físico aprobado ni obliga a crear todas las tablas de inmediato. El número final dependerá de cardinalidades y alcance.

### 7.3 Entidad central Ticket

| Campo propuesto | Significado | Condición |
|---|---|---|
| Id | Identificador interno UUID | Clave primaria |
| Number | Código visible, por ejemplo HD-2026-000001 | Único, generado con mecanismo seguro ante concurrencia |
| Type | Incidente o solicitud | Valores controlados |
| Subject | Asunto | Obligatorio, longitud máxima definida |
| Description | Detalle inicial | Política de contenido por definir |
| RequesterId | Persona que necesita soporte | FK a User |
| CreatedById | Actor que registró el caso | FK a User; derivado de la sesión |
| ServiceId | Servicio solicitado | FK a Service |
| DepartmentId | Contexto organizacional del caso | FK; definir conservación histórica |
| Impact | Alcance del caso | Valores controlados |
| Urgency | Necesidad temporal | Valores controlados |
| Priority | Prioridad resultante | Derivada de matriz; cambios justificados |
| Status | Estado actual | Transiciones controladas |
| AssignedTeamId | Equipo responsable | FK a SupportTeam |
| AssignedTechnicianId | Técnico actual | FK opcional a User |
| ResolutionSummary | Solución registrada | Obligatoria al resolver |
| CreatedAt | Creación | Instante UTC |
| UpdatedAt | Último cambio | Instante UTC |
| ResolvedAt | Resolución | Opcional antes de resolver |
| ClosedAt | Cierre | Opcional antes de cerrar |
| Version | Control de concurrencia | Estrategia concreta pendiente |

### 7.4 Relaciones y restricciones

- Un usuario puede solicitar muchos tickets; cada ticket tiene un solicitante y un creador.
- Un usuario puede tener varios roles; RolePermission relaciona roles y permisos.
- Un equipo tiene varios integrantes; TeamMember debe impedir membresías duplicadas.
- Un servicio pertenece a una categoría y tiene un equipo responsable inicial.
- Un ticket tiene comentarios, asignaciones y eventos de historial.
- TicketAsset, ProblemTicket y ChangeTicket resuelven relaciones de varios a varios.
- TicketKnowledgeArticle relaciona soluciones y tickets sin duplicar el contenido del artículo.
- Un cambio o ticket puede generar solicitudes de aprobación. ApprovalRequest tendrá FK explícitas y una restricción que exija exactamente un destino válido; no se dependerá de un EntityId genérico sin integridad referencial.
- Una aprobación tiene pasos ordenados y decisiones con fecha y actor.
- Una encuesta referencia un ticket con restricción única para la versión inicial.
- Una asignación activa de activo deberá cumplir la política acordada de responsables.

### 7.5 Convenciones de datos

- PostgreSQL: tablas y columnas en snake_case; C#: propiedades en PascalCase.
- UUID para identificadores internos, salvo justificación documentada de otra clave.
- Instantes en UTC usando tipos compatibles con zona horaria; calendarios conservan identificador de zona y horarios locales por separado.
- Claves foráneas y restricciones para integridad. Revisar eliminación en cascada individualmente.
- Desactivar usuarios, servicios y catálogos cuando ya tengan uso histórico.
- Tickets conservados con cancelación o archivo según política; no aplicar borrado lógico indiscriminado a todas las tablas.
- Índices iniciales en filtros frecuentes y FK necesarias, revisados con consultas reales.
- Unicidad de número de ticket, código de activo y valores de identidad normalizados.
- Archivos fuera de la base; guardar metadatos y clave de almacenamiento, sin exponer rutas del servidor.
- SLA conserva objetivos y vencimientos del caso; el historial permite explicar cambios y reaperturas.

### 7.6 Orden inicial de implementación del esquema

1. Usuarios, roles, permisos, sedes, departamentos y equipos.
2. Categorías y servicios; definir referencias SLA antes de consolidar el esquema.
3. Tickets, historial, asignaciones y comunicación.
4. SLA, notificaciones y aprobaciones.
5. Activos, conocimiento, problemas, cambios y satisfacción.

El modelo conceptual completo se revisará antes de generar el primer bloque. Auditoría se incorpora desde el primer flujo persistido.

## 8. Contratos de API e interfaz

### 8.1 Recursos iniciales propuestos

| Método y ruta | Propósito |
|---|---|
| POST /api/auth/login | Iniciar sesión |
| POST /api/auth/forgot-password | Solicitar recuperación con respuesta que no revele si la cuenta existe |
| POST /api/auth/reset-password | Restablecer contraseña mediante token válido |
| GET /api/users/me | Obtener perfil y capacidades actuales |
| GET /api/services | Consultar servicios disponibles |
| POST /api/tickets | Crear ticket |
| GET /api/tickets | Listar con filtros, paginación y ámbito del actor |
| GET /api/tickets/{id} | Consultar detalle autorizado |
| POST /api/tickets/{id}/assignments | Asignar o reasignar |
| POST /api/tickets/{id}/start | Iniciar atención |
| POST /api/tickets/{id}/holds | Registrar espera justificada |
| POST /api/tickets/{id}/resume | Reanudar atención |
| POST /api/tickets/{id}/resolve | Resolver |
| POST /api/tickets/{id}/close | Cerrar |
| POST /api/tickets/{id}/reopen | Reabrir |
| POST /api/tickets/{id}/cancel | Cancelar |
| POST /api/tickets/{id}/comments | Agregar respuesta o nota permitida |
| GET /api/tickets/{id}/history | Consultar historial visible |

Son contratos preliminares. Cada endpoint tendrá DTO de entrada y salida, permisos, validaciones, errores y ejemplos. Un comando no recibirá campos administrativos que el actor no deba controlar.

### 8.2 Acuerdos de respuesta

- Listas con items, página, tamaño y total cuando su cálculo sea adecuado.
- Límite máximo de página y orden estable; valores concretos por acordar.
- Errores estructurados con código, mensaje, errores de campos y correlación; sin detalles internos de la base.
- 401 para autenticación requerida; 403 para operación no permitida. Definir respuesta para recursos fuera del ámbito sin revelar información.
- 409 para conflictos de concurrencia o estado cuando corresponda.
- Fechas con formato inequívoco y zona; interfaz adaptada al usuario.

### 8.3 Pantallas prioritarias

Inicio de sesión; recuperación; portal del empleado; catálogo; creación de ticket; mis tickets; detalle y conversación; cola técnica; panel del coordinador; usuarios y permisos; administración del catálogo.

Después: activos, conocimiento, aprobaciones, problemas, cambios, SLA, encuestas, reportes y auditoría. Cada pantalla debe contemplar carga, vacío, validación, error, falta de permiso y operación exitosa.

## 9. Requisitos no funcionales

| Área | Requisito verificable inicial |
|---|---|
| Seguridad | Autorización en servidor por operación y recurso; pruebas de acceso cruzado |
| Credenciales | Hash de contraseñas; secretos fuera del repositorio; recuperación temporal |
| JWT | Validar firma, emisor, audiencia y vigencia; definir expiración e invalidación |
| Archivos | Lista de tipos permitidos, límite de tamaño y descarga protegida; valores pendientes |
| Integridad | Una operación no deja asignación actual e historial contradictorios |
| Concurrencia | Dos resoluciones simultáneas no sobrescriben datos silenciosamente |
| Rendimiento | Listados paginados y consultas filtradas; establecer presupuesto de latencia y volumen antes de medir |
| Observabilidad | Errores con correlación y logs sin secretos |
| Recuperación | Copia de base y archivos; demostrar restauración en entorno de prueba |
| Usabilidad | Formularios claros, navegación por teclado y mensajes de validación comprensibles |
| Mantenibilidad | Dependencias por capas y casos de uso consistentes |
| Operación | Entornos separados de desarrollo, prueba y demostración/despliegue |

No se fijan porcentajes de disponibilidad ni tiempos de respuesta sin conocer alojamiento, volumen y condiciones de medición.

## 10. Plan de ejecución y equipo

### 10.1 Cronograma ilustrativo de ocho semanas

| Semana | Trabajo | Entregable o condición de salida |
|---|---|---|
| 1 | Alcance, permisos, reglas, prototipos y modelo conceptual | Decisiones funcionales revisadas y backlog inicial |
| 2 | Esquema base, scaffolding, solución, identidad, organización y catálogo | Usuario autenticado consulta servicios con permisos correctos |
| 3 | Tickets, asignación y comunicación | Flujo integrado desde registro hasta resolución y cierre |
| 4 | SLA, notificaciones y aprobaciones | Plazos verificables y solicitud autorizada por pasos |
| 5 | Activos y conocimiento | Equipos y soluciones relacionados con tickets |
| 6 | Problemas, cambios y satisfacción | Recurrencia investigada, cambio aprobado y encuesta respondida |
| 7 | Reportes e integración de los 16 módulos | Indicadores consistentes y escenarios integrados |
| 8 | Pruebas finales, correcciones, despliegue y documentación | Instalación reproducible y demostración completa |

Seguridad, auditoría, pruebas y documentación se trabajan en cada semana. La viabilidad de este calendario se revisará al conocer fecha de entrega, horas disponibles y experiencia del equipo.

### 10.2 Distribución preliminar

| Integrante | Responsabilidad funcional |
|---|---|
| 1 | Usuarios y acceso; estructura organizacional |
| 2 | Catálogo; solicitudes y aprobaciones |
| 3 | Tickets; comunicación y evidencias |
| 4 | Asignación; SLA y escalamiento |
| 5 | Notificaciones; satisfacción |
| 6 | Conocimiento; problemas |
| 7 | Activos; cambios TI |
| 8 | Reportes; auditoría |

Esta distribución no implica cargas equivalentes. SLA, acceso y tickets pueden necesitar apoyo compartido. Cada responsable entregará backend, integración de interfaz, pruebas y documentación de su alcance, con colaboración según habilidades.

Se designarán además responsables transversales de arquitectura e integración, esquema SQL, frontend y despliegue. No son cuatro integrantes adicionales.

### 10.3 Flujo de trabajo

- Backlog con tarea, requisito, responsable, dependencias y criterio de aceptación.
- Cambios pequeños en ramas por funcionalidad y revisión antes de integrar.
- Rama principal ejecutable; integración frecuente.
- Coordinación de scripts SQL por una persona responsable.
- Revisión semanal con una demostración funcional y actualización de riesgos.
- Decisiones técnicas registradas con contexto, decisión y consecuencias.

## 11. Pruebas y aceptación

### 11.1 Estrategia

- Pruebas unitarias para reglas de transición, prioridades y cálculos SLA.
- Pruebas de integración con PostgreSQL para restricciones, transacciones, filtros y concurrencia.
- Pruebas de API para autenticación, permisos, validación y contratos.
- Pruebas de interfaz de los recorridos principales.
- Datos ficticios en entornos aislados; limpieza controlada y sin usar información real del equipo.

El script HTTP del laboratorio es una referencia para comprobaciones básicas. HelpDesk necesitará también casos de denegación por rol, por propietario y por equipo.

### 11.2 Escenarios mínimos de demostración

| ID | Escenario | Resultado esperado |
|---|---|---|
| E2E-01 | Empleado crea incidente; técnico atiende; empleado confirma | Ticket cerrado con historial |
| E2E-02 | Empleado intenta consultar ticket ajeno | Acceso denegado sin filtrar contenido |
| E2E-03 | Empleado intenta asignarse un rol | Operación rechazada y evento auditable |
| E2E-04 | Solicitud requiere dos aprobaciones | No continúa antes de completar ambos pasos |
| E2E-05 | Ticket atraviesa horario no laboral y pausa | Cálculo SLA coincide con ejemplo aprobado |
| E2E-06 | Técnico registra nota interna | Empleado no puede verla ni descargar sus archivos |
| E2E-07 | Dos actores modifican la misma versión | Conflicto detectado sin pérdida silenciosa |
| E2E-08 | Falla el correo al resolver | Ticket resuelto y envío pendiente de reintento |
| E2E-09 | Incidentes vinculados a problema y cambio | Relaciones y decisiones trazables |
| E2E-10 | Solicitante responde encuesta | Una respuesta válida; reportes consistentes |
| E2E-11 | Restauración de respaldo en entorno de prueba | Base y archivos relacionados recuperables |

### 11.3 Definición de terminado

Una funcionalidad estará terminada cuando cumpla su criterio de aceptación, persista correctamente, aplique permisos, maneje errores, conserve la trazabilidad requerida, esté integrada con su interfaz y tenga verificaciones pertinentes y documentación actualizada.

### 11.4 Entregables finales

Código fuente; scripts de esquema y datos ficticios; diagrama entidad-relación; diccionario de datos; especificación funcional; documentación de API; pruebas y resultados; instrucciones de instalación y configuración; manual de usuario; guía de demostración; registro de decisiones y limitaciones conocidas.

## 12. Riesgos, pendientes y próximos pasos

### 12.1 Riesgos principales

| Riesgo | Tratamiento |
|---|---|
| Alcance excesivo para el plazo | Delimitar cada módulo y revisar avance semanal |
| Integración tardía de ocho personas | Primer flujo vertical integrado en semana 3 o antes |
| Conflictos del esquema | Scripts numerados y responsable de coordinación |
| Sobrescritura por scaffolding | Separar generado de personalizaciones y revisar diferencias |
| Autenticación confundida con autorización | Pruebas específicas por permiso y recurso |
| Cálculos SLA ambiguos | Aprobar ejemplos de calendario, pausa y reapertura antes de implementar |
| Dependencia de correo o alojamiento | Configuración por entorno y proveedores sustituibles |
| Reportes contradictorios | Definir fórmulas y validar con un conjunto conocido |

### 12.2 Registro de decisiones pendientes

| ID | Decisión | Debe resolverse antes de |
|---|---|---|
| DEC-01 | Fecha real, rúbrica y alcance exigido por el curso | Comprometer cronograma |
| DEC-02 | Nombres, disponibilidad y experiencia de integrantes | Repartir tareas definitivas |
| DEC-03 | Framework frontend y estrategia de autenticación en navegador | Construir frontend |
| DEC-04 | Confirmar una sola empresa y modelo de sedes/departamentos | Aprobar esquema organizacional |
| DEC-05 | Registro de cuentas: administrativo o autoservicio controlado | Cerrar identidad |
| DEC-06 | Matriz completa de permisos y delegaciones | Implementar operaciones protegidas |
| DEC-07 | Horario laboral, feriados, zona y objetivos por prioridad | Desarrollar SLA |
| DEC-08 | Cierre automático, plazo de reapertura y SLA al reabrir | Completar ciclo de tickets |
| DEC-09 | Reglas de aprobación, reemplazos y autoaprobación | Completar aprobaciones |
| DEC-10 | Proveedor de correo y ubicación de archivos | Integrar notificaciones y adjuntos |
| DEC-11 | Límites de archivos y retención de datos/auditoría | Completar comunicación y operación |
| DEC-12 | Vigencia de JWT, invalidación y necesidad de renovación | Completar seguridad |
| DEC-13 | Hospedaje, trabajos en segundo plano y respaldos | Preparar despliegue |
| DEC-14 | Versiones exactas de SDK y paquetes compatibles | Crear solución definitiva |
| DEC-15 | Volumen esperado y objetivos medibles de rendimiento | Pruebas de rendimiento |

### 12.3 Primera sesión de trabajo

1. Revisar este documento con el equipo y la rúbrica del curso.
2. Confirmar actores, contexto organizacional y alcance de los 16 módulos.
3. Aprobar el flujo de tickets y la matriz de permisos.
4. Dibujar el modelo conceptual completo y revisar dependencias.
5. Elaborar el diccionario de datos del primer bloque y los scripts SQL.
6. Elegir frontend y fijar versiones de herramientas.
7. Crear solución, repositorio y entorno compartido de desarrollo.
8. Implementar el primer recorrido: iniciar sesión, consultar servicios, registrar ticket y visualizarlo con acceso restringido.

### 12.4 Mantenimiento de esta documentación

Actualizar la versión y el registro de cambios cuando se aprueben decisiones relevantes. Los requisitos nuevos deben indicar qué módulo afectan, su prioridad y el impacto en el plazo. Evitar presentar propuestas como funcionalidades existentes.

| Versión | Fecha | Cambio |
|---|---|---|
| 1.0 | 2026-11-01 | Definición inicial consolidada, arquitectura de referencia y planificación |
