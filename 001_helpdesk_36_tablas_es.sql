/*
 HELPDESK - VERSION EQUILIBRADA EN ESPANOL: 36 TABLAS / 16 MODULOS
 Fecha: 2026-11-01. PostgreSQL 16 o superior.

 ESTA es la propuesta actual; reemplaza como propuesta las variantes de 25 y 52 tablas.
 No es una migracion: NO convierte bases existentes. Conectarse a una BASE VACIA
 y ejecutar TODO el archivo. Crea el esquema helpdesk y falla si ya existe.
 No elimina datos ni objetos, no crea bases ni usuarios del servidor.
 Elegir una sola version. EF Core Database First: incluir --schema helpdesk.

 NOMBRES: espanol, minusculas, sin tildes, separados con guion bajo.
 UUID = identificador unico; FK/REFERENCES = relacion entre tablas.
 timestamptz = fecha y hora que representan un instante con zona horaria.
 Hash = resultado de un algoritmo seguro; no es una contrasena recuperable.

 ALCANCE INTERMEDIO AVANZADO
 - Permisos por rol; categorias independientes para servicios y conocimiento.
 - Asignaciones de tickets con historial y un responsable actual en tickets.
 - Activos, aprobaciones por pasos, problemas, cambios y tareas de cambios.
 - Recuperacion de claves mediante tokens temporales guardados SOLO como hash.
 - Preferencias de avisos y seguimiento de correos con reintentos limitados.
 - Relacion entre tickets y articulos de solucion; vistas de reportes.
 - SLA BASICO: tiempo corrido 24/7 por prioridad. No incluye calendario laboral,
   feriados ni pausas que detengan el reloj. No se inventan plazos iniciales.

 RESPONSABILIDADES DE .NET (no implementadas por crear tablas)
 - Autenticar, obtener actor desde sesion y verificar permisos y ambito del registro.
 - Verificar estados, aprobadores, pertenencia activa de tecnicos y campos editables.
 - Guardar ticket, asignacion, historial y auditoria en la misma transaccion.
 - Incrementar version y usar la version esperada en el WHERE para concurrencia.
 - Copiar objetivos SLA al ticket y calcular fechas usando tiempo corrido.
 - Mantener inmutables numero, autor y fecha original del ticket; registrar motivos.
 - Validar solicitante y cierre antes de aceptar una encuesta.
 - Proteger notas y adjuntos internos; no exponer rutas fisicas.
 - Ejecutar correos con bloqueo de trabajo, idempotencia y reintentos limitados.
 - Consumir tokens de recuperacion de forma atomica una sola vez; comprobar vigencia.
 - Historial y auditoria son de solo insercion desde las funciones ordinarias de API.
 - No hay RLS ni triggers: no exponer acceso SQL al usuario final.

 SEGURIDAD
 Sin cuentas, claves, tokens ni credenciales de ejemplo. Roles de la aplicacion,
 no roles del servidor. Usar una cuenta limitada para el backend, distinta del
 propietario. No registrar secretos en historial, auditoria ni mensajes de error.

 VERIFICACION
 Archivo revisado mediante analisis estatico de sintaxis SQL y referencias.
 No ejecutado en un servidor. Fuente de restricciones PostgreSQL:
 https://www.postgresql.org/docs/16/ddl-constraints.html
*/

BEGIN;
SET LOCAL TIME ZONE 'UTC';
CREATE SCHEMA helpdesk;
SET LOCAL search_path = helpdesk, pg_catalog;

-- MODULO 02: ORGANIZACION. Sede = local de la empresa; area = departamento.
-- 01
CREATE TABLE sedes (
    id_sede uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    nombre varchar(150) NOT NULL CHECK (btrim(nombre) <> ''),
    direccion varchar(255),
    activo boolean NOT NULL DEFAULT true
);

-- 02
CREATE TABLE areas (
    id_area uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    id_sede uuid NOT NULL REFERENCES sedes(id_sede),
    nombre varchar(150) NOT NULL CHECK (btrim(nombre) <> ''),
    activo boolean NOT NULL DEFAULT true,
    UNIQUE (id_sede, nombre),
    UNIQUE (id_area, id_sede)
);

-- MODULO 01: USUARIOS Y ACCESO.
-- 03
CREATE TABLE usuarios (
    id_usuario uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    id_area uuid REFERENCES areas(id_area),
    nombres varchar(100) NOT NULL CHECK (btrim(nombres) <> ''),
    apellidos varchar(150) NOT NULL CHECK (btrim(apellidos) <> ''),
    nombre_usuario varchar(100) NOT NULL CHECK (btrim(nombre_usuario) <> '' AND nombre_usuario = btrim(nombre_usuario)),
    correo varchar(254) NOT NULL CHECK (btrim(correo) <> '' AND correo = btrim(correo)),
    clave_hash text NOT NULL CHECK (btrim(clave_hash) <> ''),
    telefono varchar(40),
    activo boolean NOT NULL DEFAULT true,
    fecha_registro timestamptz NOT NULL DEFAULT CURRENT_TIMESTAMP
);
CREATE UNIQUE INDEX uq_usuarios_nombre ON usuarios(lower(nombre_usuario));
CREATE UNIQUE INDEX uq_usuarios_correo ON usuarios(lower(correo));
COMMENT ON COLUMN usuarios.clave_hash IS 'Contrasena transformada con un algoritmo seguro desde .NET; nunca texto plano ni cifrado reversible.';

-- 04: Un rol es un conjunto de responsabilidades (empleado, tecnico, etc.).
CREATE TABLE roles (
    id_rol uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    nombre varchar(50) NOT NULL UNIQUE,
    descripcion varchar(255)
);

-- 05: Una persona puede tener mas de un rol.
CREATE TABLE usuarios_roles (
    id_usuario uuid NOT NULL REFERENCES usuarios(id_usuario),
    id_rol uuid NOT NULL REFERENCES roles(id_rol),
    fecha_asignacion timestamptz NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (id_usuario, id_rol)
);

-- MODULO 02: EQUIPOS DE SOPORTE.
-- 06: Grupo de tecnicos; no confundir con un equipo fisico del inventario.
CREATE TABLE equipos_soporte (
    id_equipo uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    nombre varchar(150) NOT NULL UNIQUE,
    descripcion text,
    activo boolean NOT NULL DEFAULT true
);

-- 07: Esta tabla permite asignar una persona a varios equipos de soporte.
CREATE TABLE integrantes_equipo (
    id_equipo uuid NOT NULL REFERENCES equipos_soporte(id_equipo),
    id_usuario uuid NOT NULL REFERENCES usuarios(id_usuario),
    es_coordinador boolean NOT NULL DEFAULT false,
    disponible boolean NOT NULL DEFAULT true,
    activo boolean NOT NULL DEFAULT true,
    PRIMARY KEY (id_equipo, id_usuario)
);

-- MODULO 03: CATALOGO DE SERVICIOS.
-- 08: Ejemplo de servicio: soporte de internet o instalacion de programas.
CREATE TABLE servicios (
    id_servicio uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    id_equipo_responsable uuid NOT NULL REFERENCES equipos_soporte(id_equipo),
    nombre varchar(150) NOT NULL UNIQUE,
    id_categoria_servicio uuid NOT NULL,
    descripcion text NOT NULL CHECK (btrim(descripcion) <> ''),
    requiere_aprobacion boolean NOT NULL DEFAULT false,
    activo boolean NOT NULL DEFAULT true
);

-- MODULO 07: TIEMPOS DE ATENCION (SLA BASICO).
-- 09: Una politica por prioridad. Se completan al acordar los plazos del proyecto.
CREATE TABLE politicas_atencion (
    id_politica uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    prioridad varchar(20) NOT NULL UNIQUE CHECK (prioridad IN ('baja','media','alta','critica')),
    minutos_primera_respuesta integer NOT NULL CHECK (minutos_primera_respuesta > 0),
    minutos_resolucion integer NOT NULL CHECK (minutos_resolucion > 0),
    activo boolean NOT NULL DEFAULT true,
    CHECK (minutos_resolucion >= minutos_primera_respuesta),
    UNIQUE (id_politica, prioridad)
);

-- MODULOS 04 Y 05: TICKETS Y ASIGNACIONES.
-- 10: Ticket = registro de un incidente o solicitud de soporte.
CREATE TABLE tickets (
    id_ticket uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    numero bigint GENERATED ALWAYS AS IDENTITY UNIQUE,
    id_solicitante uuid NOT NULL REFERENCES usuarios(id_usuario),
    id_creador uuid NOT NULL REFERENCES usuarios(id_usuario),
    id_area_solicitante uuid NOT NULL REFERENCES areas(id_area),
    id_servicio uuid NOT NULL REFERENCES servicios(id_servicio),
    id_equipo_asignado uuid NOT NULL REFERENCES equipos_soporte(id_equipo),
    id_tecnico_asignado uuid REFERENCES usuarios(id_usuario),
    id_politica uuid NOT NULL,
    tipo varchar(20) NOT NULL CHECK (tipo IN ('incidente','solicitud')),
    asunto varchar(255) NOT NULL CHECK (btrim(asunto) <> ''),
    descripcion text NOT NULL CHECK (btrim(descripcion) <> ''),
    prioridad varchar(20) NOT NULL CHECK (prioridad IN ('baja','media','alta','critica')),
    estado varchar(30) NOT NULL DEFAULT 'nuevo'
        CHECK (estado IN ('nuevo','asignado','en_atencion','en_espera','resuelto','cerrado','cancelado')),
    motivo_espera text,
    solucion text,
    motivo_cancelacion text,
    minutos_primera_respuesta integer NOT NULL CHECK (minutos_primera_respuesta > 0),
    minutos_resolucion integer NOT NULL CHECK (minutos_resolucion > 0),
    fecha_creacion timestamptz NOT NULL DEFAULT CURRENT_TIMESTAMP,
    fecha_actualizacion timestamptz NOT NULL DEFAULT CURRENT_TIMESTAMP,
    fecha_limite_respuesta timestamptz NOT NULL,
    fecha_limite_resolucion timestamptz NOT NULL,
    fecha_primera_respuesta timestamptz,
    fecha_resolucion timestamptz,
    fecha_cierre timestamptz,
    fecha_cancelacion timestamptz,
    version integer NOT NULL DEFAULT 1 CHECK (version > 0),
    FOREIGN KEY (id_politica, prioridad) REFERENCES politicas_atencion(id_politica, prioridad),
    FOREIGN KEY (id_equipo_asignado, id_tecnico_asignado)
        REFERENCES integrantes_equipo(id_equipo, id_usuario),
    CHECK (minutos_resolucion >= minutos_primera_respuesta),
    CHECK (fecha_limite_respuesta >= fecha_creacion),
    CHECK (fecha_limite_resolucion >= fecha_limite_respuesta),
    CHECK (estado NOT IN ('asignado','en_atencion','resuelto','cerrado') OR id_tecnico_asignado IS NOT NULL),
    CHECK (estado <> 'en_espera' OR (motivo_espera IS NOT NULL AND btrim(motivo_espera) <> '')),
    CHECK (estado NOT IN ('resuelto','cerrado') OR
        (solucion IS NOT NULL AND btrim(solucion) <> '' AND fecha_resolucion IS NOT NULL)),
    CHECK ((estado = 'cerrado') = (fecha_cierre IS NOT NULL)),
    CHECK ((estado = 'cancelado') = (fecha_cancelacion IS NOT NULL)),
    CHECK (estado <> 'cancelado' OR (motivo_cancelacion IS NOT NULL AND btrim(motivo_cancelacion) <> '')),
    CHECK (fecha_primera_respuesta IS NULL OR fecha_primera_respuesta >= fecha_creacion),
    CHECK (fecha_resolucion IS NULL OR fecha_resolucion >= fecha_creacion),
    CHECK (fecha_cierre IS NULL OR (fecha_resolucion IS NOT NULL AND fecha_cierre >= fecha_resolucion)),
    CHECK (fecha_cancelacion IS NULL OR fecha_cancelacion >= fecha_creacion)
);
COMMENT ON COLUMN tickets.numero IS 'Numero visible automatico. .NET puede mostrarlo como HD-000123. Puede tener saltos.';
COMMENT ON COLUMN tickets.id_creador IS 'Usuario autenticado que registra; puede ser distinto del solicitante si tiene permiso.';
COMMENT ON COLUMN tickets.minutos_resolucion IS 'Copia del objetivo al crear el ticket; cambiar una politica no cambia el compromiso historico.';
COMMENT ON COLUMN tickets.version IS 'La API incrementa este valor y comprueba la version anterior para evitar sobrescrituras simultaneas.';

-- 11: Conserva cambios de estado, prioridad, responsable y motivos.
CREATE TABLE historial_tickets (
    id_historial uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    id_ticket uuid NOT NULL REFERENCES tickets(id_ticket),
    id_usuario uuid REFERENCES usuarios(id_usuario),
    accion varchar(50) NOT NULL,
    campo_modificado varchar(80),
    valor_anterior text,
    valor_nuevo text,
    motivo text,
    fecha timestamptz NOT NULL DEFAULT CURRENT_TIMESTAMP
);
COMMENT ON TABLE historial_tickets IS 'La API inserta eventos en la misma transaccion del cambio; no permite editarlos ni borrarlos.';

-- MODULO 06: COMUNICACION Y EVIDENCIAS.
-- 12
CREATE TABLE comentarios (
    id_comentario uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    id_ticket uuid NOT NULL REFERENCES tickets(id_ticket),
    id_autor uuid NOT NULL REFERENCES usuarios(id_usuario),
    mensaje text NOT NULL CHECK (btrim(mensaje) <> ''),
    es_interno boolean NOT NULL DEFAULT false,
    fecha timestamptz NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UNIQUE (id_comentario, id_ticket)
);

-- 13: El archivo fisico se guarda fuera de PostgreSQL; aqui van sus datos.
CREATE TABLE adjuntos (
    id_adjunto uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    id_ticket uuid NOT NULL REFERENCES tickets(id_ticket),
    id_comentario uuid,
    id_usuario uuid NOT NULL REFERENCES usuarios(id_usuario),
    nombre_archivo varchar(255) NOT NULL,
    ubicacion_archivo text NOT NULL UNIQUE,
    tipo_archivo varchar(150) NOT NULL,
    tamano_bytes bigint NOT NULL CHECK (tamano_bytes > 0),
    es_interno boolean NOT NULL DEFAULT false,
    fecha timestamptz NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (id_comentario, id_ticket) REFERENCES comentarios(id_comentario, id_ticket)
);
COMMENT ON TABLE adjuntos IS 'La API comprueba permiso y visibilidad del comentario antes de descargar. Nunca exponer rutas directas del servidor.';

-- MODULO 08: NOTIFICACIONES BASICAS.
-- 14: Una fila por destinatario y aviso. No es una conversacion de tickets.
CREATE TABLE notificaciones (
    id_notificacion uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    id_usuario uuid NOT NULL REFERENCES usuarios(id_usuario),
    id_ticket uuid REFERENCES tickets(id_ticket),
    titulo varchar(255) NOT NULL,
    mensaje text NOT NULL,
    fecha_creacion timestamptz NOT NULL DEFAULT CURRENT_TIMESTAMP,
    fecha_lectura timestamptz,
    clave_evento varchar(200) NOT NULL,
    UNIQUE (id_usuario, clave_evento),
    CHECK (fecha_lectura IS NULL OR fecha_lectura >= fecha_creacion)
);

-- MODULO 09: BASE DE CONOCIMIENTOS.
-- 15: Articulos con instrucciones o soluciones reutilizables.
CREATE TABLE articulos_conocimiento (
    id_articulo uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    id_autor uuid NOT NULL REFERENCES usuarios(id_usuario),
    id_revisor uuid REFERENCES usuarios(id_usuario),
    id_categoria_conocimiento uuid NOT NULL,
    titulo varchar(255) NOT NULL CHECK (btrim(titulo) <> ''),
    contenido text NOT NULL CHECK (btrim(contenido) <> ''),
    estado varchar(20) NOT NULL DEFAULT 'borrador'
        CHECK (estado IN ('borrador','en_revision','publicado','archivado')),
    es_interno boolean NOT NULL DEFAULT false,
    fecha_creacion timestamptz NOT NULL DEFAULT CURRENT_TIMESTAMP,
    fecha_publicacion timestamptz,
    CHECK (estado <> 'publicado' OR (id_revisor IS NOT NULL AND fecha_publicacion IS NOT NULL))
);

-- MODULO 10: ACTIVOS TECNOLOGICOS.
-- 16: Activo = computadora, impresora u otro bien tecnologico inventariado.
CREATE TABLE activos (
    id_activo uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    id_sede uuid NOT NULL REFERENCES sedes(id_sede),
    id_area uuid,
    codigo_patrimonial varchar(80) NOT NULL UNIQUE CHECK (btrim(codigo_patrimonial) <> ''),
    nombre varchar(150) NOT NULL,
    id_tipo_activo uuid NOT NULL,
    numero_serie varchar(150),
    marca varchar(100),
    modelo varchar(100),
    estado varchar(30) NOT NULL DEFAULT 'disponible'
        CHECK (estado IN ('disponible','asignado','mantenimiento','retirado','perdido')),
    fecha_registro timestamptz NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (id_area, id_sede) REFERENCES areas(id_area, id_sede)
);

-- 17: Permite conocer responsables anteriores y el actual de cada activo.
CREATE TABLE asignaciones_activos (
    id_asignacion uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    id_activo uuid NOT NULL REFERENCES activos(id_activo),
    id_usuario_responsable uuid NOT NULL REFERENCES usuarios(id_usuario),
    id_usuario_asignador uuid NOT NULL REFERENCES usuarios(id_usuario),
    fecha_asignacion timestamptz NOT NULL DEFAULT CURRENT_TIMESTAMP,
    fecha_devolucion timestamptz,
    observaciones text,
    CHECK (fecha_devolucion IS NULL OR fecha_devolucion >= fecha_asignacion)
);
CREATE UNIQUE INDEX uq_activo_asignacion_vigente ON asignaciones_activos(id_activo)
    WHERE fecha_devolucion IS NULL;

-- 18: Un ticket puede involucrar varios activos y viceversa.
CREATE TABLE tickets_activos (
    id_ticket uuid NOT NULL REFERENCES tickets(id_ticket),
    id_activo uuid NOT NULL REFERENCES activos(id_activo),
    PRIMARY KEY (id_ticket, id_activo)
);

-- MODULO 11: PROBLEMAS RECURRENTES.
-- 19: Problema = investigacion de la causa que origina uno o varios incidentes.
CREATE TABLE problemas (
    id_problema uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    id_responsable uuid NOT NULL REFERENCES usuarios(id_usuario),
    titulo varchar(255) NOT NULL,
    descripcion text NOT NULL,
    estado varchar(30) NOT NULL DEFAULT 'abierto'
        CHECK (estado IN ('abierto','en_investigacion','resuelto','cerrado')),
    causa_raiz text,
    solucion_temporal text,
    solucion_definitiva text,
    fecha_creacion timestamptz NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CHECK (estado NOT IN ('resuelto','cerrado') OR
        (causa_raiz IS NOT NULL AND btrim(causa_raiz) <> ''
         AND solucion_definitiva IS NOT NULL AND btrim(solucion_definitiva) <> ''))
);

-- 20
CREATE TABLE problemas_tickets (
    id_problema uuid NOT NULL REFERENCES problemas(id_problema),
    id_ticket uuid NOT NULL REFERENCES tickets(id_ticket),
    PRIMARY KEY (id_problema, id_ticket)
);

-- MODULO 12: CAMBIOS TI.
-- 21: Cambio = modificacion planificada, por ejemplo actualizar un servidor.
CREATE TABLE cambios (
    id_cambio uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    id_solicitante uuid NOT NULL REFERENCES usuarios(id_usuario),
    id_responsable uuid NOT NULL REFERENCES usuarios(id_usuario),
    titulo varchar(255) NOT NULL,
    descripcion text NOT NULL,
    riesgo varchar(10) NOT NULL CHECK (riesgo IN ('bajo','medio','alto')),
    plan_ejecucion text NOT NULL CHECK (btrim(plan_ejecucion) <> ''),
    plan_reversion text NOT NULL CHECK (btrim(plan_reversion) <> ''),
    requiere_aprobacion boolean NOT NULL DEFAULT true,
    estado varchar(30) NOT NULL DEFAULT 'borrador'
        CHECK (estado IN ('borrador','pendiente_aprobacion','aprobado','programado','en_ejecucion','finalizado','rechazado','cancelado')),
    fecha_programada timestamptz,
    fecha_finalizacion timestamptz,
    resultado text,
    fecha_creacion timestamptz NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CHECK (estado <> 'finalizado' OR (fecha_finalizacion IS NOT NULL AND resultado IS NOT NULL AND btrim(resultado) <> '')),
    CHECK (fecha_finalizacion IS NULL OR fecha_finalizacion >= fecha_creacion)
);

-- 22
CREATE TABLE cambios_tickets (
    id_cambio uuid NOT NULL REFERENCES cambios(id_cambio),
    id_ticket uuid NOT NULL REFERENCES tickets(id_ticket),
    PRIMARY KEY (id_cambio, id_ticket)
);

-- MODULO 13: APROBACIONES.
-- 23: Cada fila es un paso. Puede pertenecer a un ticket O a un cambio, no ambos.
CREATE TABLE aprobaciones (
    id_aprobacion uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    id_ticket uuid REFERENCES tickets(id_ticket),
    id_cambio uuid REFERENCES cambios(id_cambio),
    id_solicitante uuid NOT NULL REFERENCES usuarios(id_usuario),
    id_aprobador uuid NOT NULL REFERENCES usuarios(id_usuario),
    orden smallint NOT NULL DEFAULT 1 CHECK (orden > 0),
    motivo text NOT NULL CHECK (btrim(motivo) <> ''),
    estado varchar(20) NOT NULL DEFAULT 'pendiente'
        CHECK (estado IN ('pendiente','aprobado','rechazado','cancelado')),
    comentario_decision text,
    fecha_solicitud timestamptz NOT NULL DEFAULT CURRENT_TIMESTAMP,
    fecha_decision timestamptz,
    CHECK (num_nonnulls(id_ticket, id_cambio) = 1),
    CHECK (id_solicitante <> id_aprobador),
    CHECK ((estado = 'pendiente') = (fecha_decision IS NULL)),
    CHECK (estado <> 'rechazado' OR (comentario_decision IS NOT NULL AND btrim(comentario_decision) <> '')),
    CHECK (fecha_decision IS NULL OR fecha_decision >= fecha_solicitud)
);
CREATE UNIQUE INDEX uq_aprobaciones_ticket_orden ON aprobaciones(id_ticket, orden) WHERE id_ticket IS NOT NULL;
CREATE UNIQUE INDEX uq_aprobaciones_cambio_orden ON aprobaciones(id_cambio, orden) WHERE id_cambio IS NOT NULL;
COMMENT ON TABLE aprobaciones IS 'La API exige resolver pasos en orden. Rechazo/cancelacion termina el proceso; una nueva solicitud de negocio requiere definir su nuevo ciclo.';

-- MODULO 14: SATISFACCION.
-- 24: Una respuesta por ticket. La API valida solicitante y ticket cerrado.
CREATE TABLE encuestas (
    id_encuesta uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    id_ticket uuid NOT NULL UNIQUE REFERENCES tickets(id_ticket),
    id_usuario uuid NOT NULL REFERENCES usuarios(id_usuario),
    calificacion smallint NOT NULL CHECK (calificacion BETWEEN 1 AND 5),
    comentario text,
    fecha_respuesta timestamptz NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- MODULO 16: AUDITORIA.
-- 25: Registra acciones relevantes del sistema, mas alla del historial de un ticket.
CREATE TABLE auditoria (
    id_auditoria uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    id_usuario uuid REFERENCES usuarios(id_usuario),
    accion varchar(100) NOT NULL,
    tabla_afectada varchar(80) NOT NULL,
    id_registro uuid,
    descripcion text NOT NULL,
    fecha timestamptz NOT NULL DEFAULT CURRENT_TIMESTAMP
);
COMMENT ON TABLE auditoria IS 'Solo insercion desde la API. No guardar claves, tokens ni datos sensibles. id_registro es una referencia informativa para distintas tablas.';

-- TABLAS ADICIONALES: recuperan funcionalidad sin volver al diseno de 52 tablas.

ALTER TABLE notificaciones ADD COLUMN tipo_evento varchar(80) NOT NULL;
COMMENT ON COLUMN notificaciones.tipo_evento IS 'Codigo estable que relaciona el aviso con las preferencias del usuario.';

-- 26 / MODULO 01: Permiso = accion que un rol puede realizar.
CREATE TABLE permisos (
    id_permiso uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    codigo varchar(100) NOT NULL UNIQUE,
    modulo varchar(50) NOT NULL,
    descripcion varchar(255) NOT NULL
);

-- 27: Define los permisos de cada rol. Tener permiso no elimina el filtro por equipo/propietario.
CREATE TABLE roles_permisos (
    id_rol uuid NOT NULL REFERENCES roles(id_rol),
    id_permiso uuid NOT NULL REFERENCES permisos(id_permiso),
    PRIMARY KEY (id_rol, id_permiso)
);

-- 28 / MODULO 03: Evita categorias escritas de diferentes formas en cada servicio.
CREATE TABLE categorias_servicios (
    id_categoria_servicio uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    nombre varchar(100) NOT NULL UNIQUE,
    descripcion varchar(255),
    activo boolean NOT NULL DEFAULT true
);
ALTER TABLE servicios ADD CONSTRAINT fk_servicios_categoria
    FOREIGN KEY (id_categoria_servicio) REFERENCES categorias_servicios(id_categoria_servicio);

-- 29 / MODULO 09: Clasificacion de articulos de ayuda.
CREATE TABLE categorias_conocimiento (
    id_categoria_conocimiento uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    nombre varchar(100) NOT NULL UNIQUE,
    descripcion varchar(255),
    activo boolean NOT NULL DEFAULT true
);
ALTER TABLE articulos_conocimiento ADD CONSTRAINT fk_articulos_categoria
    FOREIGN KEY (id_categoria_conocimiento) REFERENCES categorias_conocimiento(id_categoria_conocimiento);

-- 30 / MODULO 10: Tipos de bienes tecnologicos; no confundir con equipos de personas.
CREATE TABLE tipos_activos (
    id_tipo_activo uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    nombre varchar(100) NOT NULL UNIQUE,
    descripcion varchar(255),
    activo boolean NOT NULL DEFAULT true
);
ALTER TABLE activos ADD CONSTRAINT fk_activos_tipo
    FOREIGN KEY (id_tipo_activo) REFERENCES tipos_activos(id_tipo_activo);

-- 31 / MODULO 05: Historial estructurado de asignaciones, ademas del historial general.
CREATE TABLE asignaciones_tickets (
    id_asignacion_ticket uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    id_ticket uuid NOT NULL REFERENCES tickets(id_ticket),
    id_equipo uuid NOT NULL REFERENCES equipos_soporte(id_equipo),
    id_tecnico uuid REFERENCES usuarios(id_usuario),
    id_asignador uuid NOT NULL REFERENCES usuarios(id_usuario),
    fecha_asignacion timestamptz NOT NULL DEFAULT CURRENT_TIMESTAMP,
    fecha_fin timestamptz,
    motivo text NOT NULL CHECK (btrim(motivo) <> ''),
    FOREIGN KEY (id_equipo, id_tecnico) REFERENCES integrantes_equipo(id_equipo, id_usuario),
    CHECK (fecha_fin IS NULL OR fecha_fin >= fecha_asignacion)
);
CREATE UNIQUE INDEX uq_ticket_asignacion_vigente ON asignaciones_tickets(id_ticket)
    WHERE fecha_fin IS NULL;
COMMENT ON TABLE asignaciones_tickets IS 'Al reasignar: cerrar la anterior, crear la nueva y actualizar tickets en una sola transaccion. Mantener coherencia desde .NET.';

-- 32 / MODULO 09: Soluciones documentadas que ayudaron a atender un ticket.
CREATE TABLE tickets_articulos (
    id_ticket uuid NOT NULL REFERENCES tickets(id_ticket),
    id_articulo uuid NOT NULL REFERENCES articulos_conocimiento(id_articulo),
    id_usuario uuid NOT NULL REFERENCES usuarios(id_usuario),
    fecha_vinculacion timestamptz NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (id_ticket, id_articulo)
);

-- 33 / MODULO 01: Recuperacion de contrasena sin almacenar el token original.
CREATE TABLE recuperaciones_clave (
    id_recuperacion uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    id_usuario uuid NOT NULL REFERENCES usuarios(id_usuario),
    resumen_token varchar(128) NOT NULL UNIQUE CHECK (btrim(resumen_token) <> ''),
    fecha_solicitud timestamptz NOT NULL DEFAULT CURRENT_TIMESTAMP,
    fecha_vencimiento timestamptz NOT NULL,
    fecha_uso timestamptz,
    CHECK (fecha_vencimiento > fecha_solicitud),
    CHECK (fecha_uso IS NULL OR (fecha_uso >= fecha_solicitud AND fecha_uso <= fecha_vencimiento))
);
COMMENT ON COLUMN recuperaciones_clave.resumen_token IS 'Hash de un token aleatorio temporal enviado al usuario; nunca el token original.';

-- 34 / MODULO 08: Preferencias por usuario y tipo de aviso.
CREATE TABLE preferencias_notificaciones (
    id_usuario uuid NOT NULL REFERENCES usuarios(id_usuario),
    tipo_evento varchar(80) NOT NULL CHECK (btrim(tipo_evento) <> ''),
    recibir_en_sistema boolean NOT NULL DEFAULT true,
    recibir_por_correo boolean NOT NULL DEFAULT true,
    PRIMARY KEY (id_usuario, tipo_evento)
);
COMMENT ON TABLE preferencias_notificaciones IS 'La API define codigos de evento estables. Los correos de seguridad no dependen de preferencias opcionales.';

-- 35 / MODULO 08: Seguimiento del correo; un registro por aviso, varios intentos.
CREATE TABLE envios_notificaciones (
    id_envio uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    id_notificacion uuid NOT NULL UNIQUE REFERENCES notificaciones(id_notificacion),
    correo_destino varchar(254) NOT NULL CHECK (btrim(correo_destino) <> ''),
    estado varchar(20) NOT NULL DEFAULT 'pendiente'
        CHECK (estado IN ('pendiente','procesando','enviado','fallido','cancelado')),
    intentos integer NOT NULL DEFAULT 0 CHECK (intentos >= 0),
    maximo_intentos integer NOT NULL DEFAULT 3 CHECK (maximo_intentos > 0),
    fecha_creacion timestamptz NOT NULL DEFAULT CURRENT_TIMESTAMP,
    fecha_proximo_intento timestamptz NOT NULL DEFAULT CURRENT_TIMESTAMP,
    bloqueo_hasta timestamptz,
    fecha_envio timestamptz,
    ultimo_error text,
    identificador_proveedor varchar(255),
    CHECK (intentos <= maximo_intentos),
    CHECK ((estado = 'enviado') = (fecha_envio IS NOT NULL)),
    CHECK (fecha_envio IS NULL OR fecha_envio >= fecha_creacion)
);
COMMENT ON TABLE envios_notificaciones IS 'Crear junto al aviso en la misma transaccion. Worker reclama filas con bloqueo; correo puede duplicarse si el proveedor no ofrece idempotencia.';

-- 36 / MODULO 12: Actividades concretas para ejecutar un cambio planificado.
CREATE TABLE tareas_cambios (
    id_tarea_cambio uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    id_cambio uuid NOT NULL REFERENCES cambios(id_cambio),
    id_responsable uuid REFERENCES usuarios(id_usuario),
    orden smallint NOT NULL CHECK (orden > 0),
    titulo varchar(255) NOT NULL CHECK (btrim(titulo) <> ''),
    descripcion text,
    estado varchar(20) NOT NULL DEFAULT 'pendiente'
        CHECK (estado IN ('pendiente','en_ejecucion','completada','cancelada')),
    fecha_creacion timestamptz NOT NULL DEFAULT CURRENT_TIMESTAMP,
    fecha_finalizacion timestamptz,
    UNIQUE (id_cambio, orden),
    CHECK ((estado = 'completada') = (fecha_finalizacion IS NOT NULL)),
    CHECK (fecha_finalizacion IS NULL OR fecha_finalizacion >= fecha_creacion)
);

CREATE INDEX ix_roles_permisos_permiso ON roles_permisos(id_permiso);
CREATE INDEX ix_servicios_categoria ON servicios(id_categoria_servicio);
CREATE INDEX ix_articulos_categoria ON articulos_conocimiento(id_categoria_conocimiento);
CREATE INDEX ix_activos_tipo ON activos(id_tipo_activo);
CREATE INDEX ix_asignaciones_ticket_fecha ON asignaciones_tickets(id_ticket, fecha_asignacion);
CREATE INDEX ix_asignaciones_ticket_equipo ON asignaciones_tickets(id_equipo, id_tecnico);
CREATE INDEX ix_asignaciones_ticket_tecnico ON asignaciones_tickets(id_tecnico);
CREATE INDEX ix_tickets_articulos_articulo ON tickets_articulos(id_articulo);
CREATE INDEX ix_recuperaciones_usuario_fecha ON recuperaciones_clave(id_usuario, fecha_vencimiento);
CREATE INDEX ix_envios_pendientes ON envios_notificaciones(fecha_proximo_intento)
    WHERE estado IN ('pendiente','procesando');
CREATE INDEX ix_tareas_cambios_responsable ON tareas_cambios(id_responsable, estado);

-- MODULO 15: REPORTES. Se obtienen de las tablas anteriores, sin duplicar datos.
-- Ejemplos: tickets por estado, pendientes por tecnico, cumplimiento de fechas,
-- tiempos de resolucion y promedio de encuestas. La API filtra segun el rol/ambito.

-- Indices de consultas habituales. Las PK y restricciones UNIQUE ya crean indices.
CREATE INDEX ix_usuarios_area ON usuarios(id_area);
CREATE INDEX ix_usuarios_roles_rol ON usuarios_roles(id_rol);
CREATE INDEX ix_integrantes_usuario ON integrantes_equipo(id_usuario);
CREATE INDEX ix_servicios_equipo ON servicios(id_equipo_responsable);
CREATE INDEX ix_tickets_solicitante ON tickets(id_solicitante, fecha_creacion DESC);
CREATE INDEX ix_tickets_creador ON tickets(id_creador);
CREATE INDEX ix_tickets_area ON tickets(id_area_solicitante);
CREATE INDEX ix_tickets_servicio ON tickets(id_servicio);
CREATE INDEX ix_tickets_politica ON tickets(id_politica);
CREATE INDEX ix_tickets_equipo_estado ON tickets(id_equipo_asignado, estado);
CREATE INDEX ix_tickets_tecnico_estado ON tickets(id_tecnico_asignado, estado);
CREATE INDEX ix_tickets_estado_fecha ON tickets(estado, fecha_creacion);
CREATE INDEX ix_historial_ticket_fecha ON historial_tickets(id_ticket, fecha);
CREATE INDEX ix_comentarios_ticket_fecha ON comentarios(id_ticket, fecha);
CREATE INDEX ix_adjuntos_ticket ON adjuntos(id_ticket);
CREATE INDEX ix_adjuntos_comentario ON adjuntos(id_comentario, id_ticket);
CREATE INDEX ix_notificaciones_usuario_fecha ON notificaciones(id_usuario, fecha_creacion DESC);
CREATE INDEX ix_notificaciones_ticket ON notificaciones(id_ticket);
CREATE INDEX ix_articulos_estado ON articulos_conocimiento(estado);
CREATE INDEX ix_activos_area_sede ON activos(id_area, id_sede);
CREATE INDEX ix_activos_sede ON activos(id_sede);
CREATE INDEX ix_asignaciones_usuario ON asignaciones_activos(id_usuario_responsable);
CREATE INDEX ix_tickets_activos_activo ON tickets_activos(id_activo);
CREATE INDEX ix_problemas_tickets_ticket ON problemas_tickets(id_ticket);
CREATE INDEX ix_cambios_tickets_ticket ON cambios_tickets(id_ticket);
CREATE INDEX ix_aprobaciones_aprobador_estado ON aprobaciones(id_aprobador, estado);
CREATE INDEX ix_auditoria_usuario_fecha ON auditoria(id_usuario, fecha);
CREATE INDEX ix_auditoria_registro ON auditoria(tabla_afectada, id_registro);

-- Unicos datos iniciales: nombres de roles, sin usuarios ni privilegios del servidor.
INSERT INTO roles(nombre, descripcion) VALUES
('empleado', 'Registra solicitudes y consulta sus propios tickets.'),
('tecnico', 'Atiende tickets asignados y registra soluciones.'),
('coordinador', 'Asigna tickets y supervisa sus equipos de soporte.'),
('aprobador', 'Autoriza o rechaza solicitudes asignadas.'),
('administrador', 'Administra usuarios, roles y configuracion.');


-- Permisos iniciales de la aplicacion. No son privilegios del servidor PostgreSQL.
INSERT INTO permisos(codigo, modulo, descripcion) VALUES
('usuarios.administrar','usuarios','Gestionar cuentas, roles y permisos'),
('organizacion.administrar','organizacion','Gestionar sedes, areas y equipos'),
('servicios.consultar','servicios','Consultar servicios activos'),
('servicios.administrar','servicios','Administrar servicios y categorias'),
('tickets.crear','tickets','Crear tickets propios'),
('tickets.consultar_propios','tickets','Consultar tickets del solicitante'),
('tickets.consultar_asignados','tickets','Consultar casos asignados al tecnico'),
('tickets.consultar_equipo','tickets','Consultar casos de equipos autorizados'),
('tickets.consultar_todos','tickets','Consultar tickets globalmente'),
('tickets.atender','tickets','Iniciar atencion y registrar solucion autorizada'),
('tickets.confirmar_propios','tickets','Cerrar o pedir reapertura del ticket propio'),
('asignaciones.administrar','asignaciones','Asignar en equipos autorizados'),
('comentarios.responder','comunicacion','Responder en tickets accesibles'),
('comentarios.internos','comunicacion','Consultar y registrar notas internas autorizadas'),
('politicas.administrar','tiempos_atencion','Configurar plazos de atencion'),
('notificaciones.propias','notificaciones','Consultar avisos y preferencias propias'),
('conocimiento.consultar','conocimiento','Consultar articulos publicados visibles'),
('conocimiento.redactar','conocimiento','Crear borradores de articulos'),
('conocimiento.publicar','conocimiento','Revisar y publicar articulos'),
('activos.consultar','activos','Consultar inventario autorizado'),
('activos.administrar','activos','Administrar inventario y responsables'),
('problemas.gestionar','problemas','Investigar problemas autorizados'),
('cambios.gestionar','cambios','Planificar y ejecutar cambios autorizados'),
('aprobaciones.decidir','aprobaciones','Decidir pasos de aprobacion asignados'),
('encuestas.responder_propias','encuestas','Evaluar atencion del ticket propio'),
('reportes.consultar_equipo','reportes','Consultar indicadores de equipos autorizados'),
('reportes.consultar_todos','reportes','Consultar indicadores globales'),
('auditoria.consultar','auditoria','Consultar auditoria global');

INSERT INTO roles_permisos(id_rol, id_permiso)
SELECT r.id_rol, p.id_permiso FROM roles r CROSS JOIN permisos p
WHERE p.codigo IN ('servicios.consultar','tickets.crear','tickets.consultar_propios',
    'tickets.confirmar_propios','comentarios.responder','notificaciones.propias',
    'conocimiento.consultar','encuestas.responder_propias');

INSERT INTO roles_permisos(id_rol, id_permiso)
SELECT r.id_rol, p.id_permiso FROM roles r CROSS JOIN permisos p
WHERE (r.nombre = 'tecnico' AND p.codigo IN ('tickets.consultar_asignados','tickets.atender',
    'comentarios.internos','conocimiento.redactar','activos.consultar','problemas.gestionar','cambios.gestionar'))
 OR (r.nombre = 'coordinador' AND p.codigo IN ('tickets.consultar_equipo','asignaciones.administrar',
    'comentarios.internos','conocimiento.publicar','activos.consultar','reportes.consultar_equipo'))
 OR (r.nombre = 'aprobador' AND p.codigo = 'aprobaciones.decidir')
 OR (r.nombre = 'administrador' AND p.codigo IN ('usuarios.administrar','organizacion.administrar',
    'servicios.administrar','tickets.consultar_todos','politicas.administrar',
    'activos.administrar','reportes.consultar_todos','auditoria.consultar'));
-- Asignar varios roles si una persona realiza tareas de tecnico y coordinador.

INSERT INTO categorias_servicios(nombre, descripcion) VALUES
('Equipos','Soporte de computadoras y perifericos'),
('Programas','Instalacion y funcionamiento de aplicaciones'),
('Conectividad','Redes y acceso a servicios'),
('Cuentas y accesos','Identidades y autorizacion de recursos');
INSERT INTO categorias_conocimiento(nombre, descripcion) VALUES
('Guias de uso','Instrucciones para empleados'),
('Soluciones tecnicas','Diagnosticos y soluciones documentadas');
INSERT INTO tipos_activos(nombre) VALUES
('Computadora de escritorio'),('Laptop'),('Monitor'),('Impresora'),('Dispositivo de red'),('Otro');

-- Vistas de reportes: no aumentan el numero de tablas ni duplican informacion.
CREATE VIEW resumen_tickets AS
SELECT t.id_ticket, t.numero, t.asunto, t.tipo, t.estado, t.prioridad,
    t.id_solicitante, t.id_area_solicitante, t.id_equipo_asignado, t.id_tecnico_asignado,
    t.fecha_creacion, t.fecha_limite_respuesta, t.fecha_limite_resolucion,
    t.fecha_primera_respuesta, t.fecha_resolucion, t.fecha_cierre,
    CASE WHEN t.fecha_resolucion IS NOT NULL
        THEN extract(epoch FROM (t.fecha_resolucion - t.fecha_creacion)) / 60.0 END AS minutos_hasta_resolucion,
    CASE WHEN t.fecha_primera_respuesta IS NOT NULL
        THEN t.fecha_primera_respuesta <= t.fecha_limite_respuesta END AS respuesta_en_plazo,
    CASE WHEN t.fecha_resolucion IS NOT NULL
        THEN t.fecha_resolucion <= t.fecha_limite_resolucion END AS resolucion_en_plazo,
    e.calificacion
FROM tickets t LEFT JOIN encuestas e ON e.id_ticket = t.id_ticket;

CREATE VIEW carga_tecnicos AS
SELECT id_equipo_asignado, id_tecnico_asignado, prioridad, count(*) AS tickets_pendientes
FROM tickets WHERE estado NOT IN ('resuelto','cerrado','cancelado')
GROUP BY id_equipo_asignado, id_tecnico_asignado, prioridad;
COMMENT ON VIEW resumen_tickets IS 'La API aplica el ambito por solicitante/equipo; no exponer todos los datos por tener acceso a la vista.';

COMMIT;

/* ORDEN RECOMENDADO PARA CARGAR DATOS DESPUES
 1. Sedes y areas.
 2. Usuarios con clave_hash generada desde .NET y sus roles.
 3. Equipos de soporte e integrantes.
 4. Servicios con categoria y las cuatro politicas de atencion con plazos acordados.
 5. Tickets, comentarios y el resto de operaciones.

 FLUJO BASE: nuevo -> asignado -> en_atencion -> resuelto -> cerrado.
 en_espera requiere motivo; cancelado requiere motivo y fecha.
 Al reabrir, .NET registra el motivo en historial y limpia fecha_cierre.
 La politica de reapertura y ajuste de plazos se define antes de implementarla.

 CONSULTA OPCIONAL PARA REVISAR TABLAS, EJECUTAR MANUALMENTE:
 SELECT table_name FROM information_schema.tables
 WHERE table_schema = 'helpdesk' AND table_type = 'BASE TABLE' ORDER BY table_name;
*/
