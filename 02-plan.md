# Plan y cierre de alcance

## 1. Alcance cerrado

Esta entrega cubre **CA-1, CA-2 y CA-3** (tope de tres permitido por el documento de la prueba).

| CA | Qué se implementa | Hallazgo que resuelve |
|---|---|---|
| CA-1 | El gestor registra un paciente nuevo, que queda disponible para agendar contactos. | #2 (contradicción teléfono/autorregistro), #3 (unicidad de documento por país) |
| CA-2 | El gestor registra un contacto sobre un paciente existente, con fecha, canal y resultado. | #6 (catálogo de resultados de contacto) |
| CA-3 | El gestor corrige un contacto con error, quedando trazable qué cambió, quién lo cambió y por qué. | #5 (corrección sin sobrescritura, auditable) |

Juntos forman una historia completa y demostrable de punta a punta: un paciente entra al programa, se le registra un contacto, y si ese contacto tiene un error, se corrige dejando rastro — sin depender de los hallazgos más frágiles (calendario de seguimiento inexistente, estado activo/ilocalizable, denominador móvil del reporte de adherencia).

## 2. Fuera de alcance

| Qué se deja fuera | Justificación |
|---|---|
| Autorregistro (alcance funcional #2) | El hallazgo #2 mostró que es incompatible con el teléfono obligatorio de CA-1 sin una regla de reconciliación que el PRD no da. Se construye únicamente el registro por gestor. |
| CA-4 (vista de contactos del mes filtrada por gestor y ciudad) | Entre los cuatro candidatos que no dependían de los hallazgos más frágiles (CA-1, CA-2, CA-3, CA-4), se priorizó cubrir la corrección auditable (CA-3) sobre el filtro por gestor/ciudad, porque CA-3 resuelve directamente el hallazgo #5 (el tema central de la introducción: "un registro no se puede sobrescribir sin dejar rastro"), mientras CA-4 es una consulta con filtros sin ningún hallazgo bloqueante detrás — menos señal para el mismo costo. Sí se construye una consulta de apoyo (historial de contactos de un paciente) para no perder por completo el requisito de la Parte III de una consulta que combine tablas — esa consulta no reclama cubrir CA-4 ni sus filtros específicos. |
| CA-5 (marcado de ilocalizable) | Depende de decidir si el estado es persistido o calculado (hallazgo #7), decisión que no es necesaria para CA-1/2/3 y que agregaría alcance no cerrado por encima del tope de tres CA. |
| CA-6 (reporte de adherencia) | El más caro de defender: depende de tres supuestos propios encadenados (calendario de seguimiento inexistente — hallazgo #4, definición de "activo" — hallazgo #7, y el riesgo de diseño del indicador con denominador móvil — hallazgo #9). Construir sobre tres supuestos propios no es defendible en el tiempo disponible. |
| Reportes al laboratorio, exportación, notificaciones | No hay CA que los exija en el alcance cerrado. |
| Consentimiento informado / base legal para datos de salud (hallazgo #8) | CA-1 registra documento de identidad y fecha de inicio de tratamiento — datos personales y de salud — sin implementar ningún flujo de consentimiento. Se asume, igual que en el hallazgo #8, que esto queda fuera de alcance del ejercicio; no se construye pantalla ni campo de consentimiento. |
| Autenticación/login de gestores | El PRD no define roles ni permisos (hallazgo #1 de `01-hallazgos.md`). Construir un sistema de identidad real no está pedido por ningún CA, no mueve ningún bloque de la rúbrica, y compite contra el tiempo protegido para las tres funcionalidades elegidas — la "Parte V. Cambio en caliente" es el mecanismo que el propio documento prevé para razonar este tipo de vacío sin construirlo preventivamente. En su lugar, `Manager` es un catálogo simple (sin credenciales) seleccionado manualmente en cada formulario: identidad **declarada, no verificada**, documentada así en todos los puntos donde aparece. |

## 3. Modelo de datos

Se agregan campos ausentes en el PRD (país, tipo de documento) según los supuestos ya declarados en `01-hallazgos.md`. Nombres de tabla/columna en inglés, siguiendo la convención de código del proyecto; la prosa de este documento queda en español.

### Manager

Se construye antes que `Patient`, porque `Patient` la referencia por FK.

| Columna | Tipo | Restricción |
|---|---|---|
| Id | INT IDENTITY | PK |
| Name | NVARCHAR(150) | NOT NULL |

Catálogo simple sin credenciales, cargado por seed. No hay autenticación en el alcance (hallazgo #1 de `01-hallazgos.md`): el valor de `Name` se selecciona manualmente en cada formulario, es identidad **declarada, no verificada** — nunca funciona como control de acceso. Esta misma advertencia aplica a `Patient.RegisteredByManagerId`, `Contact.ManagerId` y `ContactCorrection.CorrectedByManagerId` por igual.

### Patient

| Columna | Tipo | Restricción |
|---|---|---|
| Id | INT IDENTITY | PK |
| RegisteredByManagerId | INT | NOT NULL, FK → Manager.Id |
| Country | NVARCHAR(20) | NOT NULL, CHECK IN ('Colombia','Peru','Ecuador') |
| DocumentType | NVARCHAR(20) | NOT NULL, CHECK IN ('CC','DNI','CI','Passport') |
| DocumentNumber | NVARCHAR(20) | NOT NULL |
| Name | NVARCHAR(150) | NOT NULL |
| Phone | NVARCHAR(20) | NOT NULL |
| Email | NVARCHAR(150) | NULL |
| City | NVARCHAR(100) | NOT NULL |
| TreatmentStartDate | DATE | NOT NULL |
| CreatedAt | DATETIME2 | NOT NULL, DEFAULT SYSUTCDATETIME() |

Restricción única: `(Country, DocumentType, DocumentNumber)` — resuelve el hallazgo #3 de unicidad multi-país. `RegisteredByManagerId` no lo exige ningún CA ni hallazgo por sí solo; se agrega por consistencia con `Contact`/`ContactCorrection` y para que el formulario de registro no quede sin pedir ninguna identificación — sigue siendo declarada, no verificada.

### Contact

| Columna | Tipo | Restricción |
|---|---|---|
| Id | INT IDENTITY | PK |
| PatientId | INT | NOT NULL, FK → Patient.Id |
| ManagerId | INT | NOT NULL, FK → Manager.Id |
| ContactDate | DATE | NOT NULL |
| Channel | NVARCHAR(20) | NOT NULL, CHECK IN ('Call','WhatsApp','Email') |
| Result | NVARCHAR(30) | NOT NULL, CHECK IN ('Answered','NotAnswered','DeclinedFollowUp') |
| Notes | NVARCHAR(500) | NULL |
| CreatedAt | DATETIME2 | NOT NULL, DEFAULT SYSUTCDATETIME() |
| UpdatedAt | DATETIME2 | NULL |

El catálogo cerrado de `Result` resuelve el hallazgo #6. `ManagerId` es, igual que en `Patient`, identidad declarada por selección, no verificada.

### ContactCorrection

| Columna | Tipo | Restricción |
|---|---|---|
| Id | INT IDENTITY | PK |
| ContactId | INT | NOT NULL, FK → Contact.Id |
| PreviousContactDate | DATE | NOT NULL |
| PreviousChannel | NVARCHAR(20) | NOT NULL |
| PreviousResult | NVARCHAR(30) | NOT NULL |
| PreviousNotes | NVARCHAR(500) | NULL |
| CorrectedByManagerId | INT | NOT NULL, FK → Manager.Id |
| Reason | NVARCHAR(300) | NOT NULL |
| CorrectionDate | DATETIME2 | NOT NULL, DEFAULT SYSUTCDATETIME() |

### Nota sobre la corrección de un registro ya guardado

`Contact` siempre refleja el valor vigente. Antes de aplicar un `UPDATE`, el servicio inserta una fila en `ContactCorrection` con una fotografía completa del valor anterior (fecha, canal, resultado, notas) más quién corrige, cuándo y por qué (`Reason` es obligatorio). Nada se sobrescribe sin dejar ese rastro — resuelve directamente el hallazgo #5. "Quién corrige" (`CorrectedByManagerId`) es, igual que en el resto del modelo, identidad declarada por selección, no verificada: el rastro deja evidencia de qué cambió, cuándo y con qué motivo, pero no prueba criptográficamente la identidad de quien lo hizo. No se modela versionado genérico de todas las entidades: solo `Contact` lo necesita, porque es el único registro que el PRD pide poder corregir.

### Relaciones

`Manager 1—N Patient`, `Patient 1—N Contact`, `Manager 1—N Contact`, `Manager 1—N ContactCorrection`, `Contact 1—N ContactCorrection`.

## 4. Contrato de la interfaz

### API

| Endpoint | Entrada | Salida | Errores |
|---|---|---|---|
| `POST /api/patients` (CA-1) | `{ registeredByManagerId, name, country, documentType, documentNumber, phone, email?, city, treatmentStartDate }` | `201` + paciente creado con `id` | `400` campo obligatorio faltante o inválido, o si `registeredByManagerId` no corresponde a un gestor existente; `409` si `(country, documentType, documentNumber)` ya existe |
| `GET /api/patients/{id}` (soporte) | — | `200` + datos del paciente y sus contactos | `404` si no existe |
| `POST /api/patients/{patientId}/contacts` (CA-2) | `{ managerId, contactDate, channel, result, notes? }` | `201` + contacto creado | `404` paciente inexistente; `400` canal/resultado fuera del catálogo o fecha inválida |
| `PUT /api/contacts/{id}` (CA-3) | `{ correctedByManagerId, reason, contactDate?, channel?, result?, notes? }` | `200` + contacto actualizado | `404` contacto inexistente; `400` si `reason` está vacío o no se envía ningún campo a corregir |
| `GET /api/patients/{patientId}/contacts` (consulta con criterio, apoyo) | — | `200` + lista de contactos del paciente, cada uno con indicador de si tiene correcciones | `404` paciente inexistente |
| `GET /api/managers` (soporte) | — | `200` + lista de gestores (`id`, `name`) | — |
| `GET /api/patients` (soporte) | — | `200` + lista de pacientes (`id`, `name`, `documentNumber`) | — |

Se agrega `GET /api/managers` como endpoint de apoyo, no declarado en la versión original de este plan. El formulario de registro de CA-1 necesita un selector de gestores — la decisión de "identidad declarada, no verificada" (hallazgo #1) exige un selector, no un campo numérico de texto libre — y sin este endpoint no hay de dónde obtener esa lista. No es un CA nuevo ni cuenta contra el tope de 2-3 CA: es infraestructura de soporte, con el mismo carácter que `GET /api/patients/{id}` y la consulta de historial de contactos, que ya estaban en el contrato por la misma razón.

Por la misma razón se agrega `GET /api/patients`: el formulario de CA-2 necesita un selector de "¿para cuál paciente?", y no existía ninguna fuente para esa lista. `documentNumber` se incluye junto a `name` para poder distinguir dos pacientes que compartan nombre.

DTOs propios en la frontera del API (nunca se expone la entidad de EF Core directamente), por la regla estricta de separación de capas.

**Caso de error de punta a punta que se muestra explícitamente** (requisito mínimo de la Parte III): registrar un paciente con el mismo `(country, documentType, documentNumber)` de uno ya existente. La API responde `409` con un mensaje describiendo el conflicto; el formulario Angular lo muestra sin perder los datos ya digitados por el gestor, en vez de solo mostrar "error" genérico.

### Pantallas (Angular)

Estilos con Tailwind CSS (decisión de estilo, no exigida por el documento — la introducción dice explícitamente "no evaluamos... la belleza de la interfaz"; se elige por velocidad propia al escribir los formularios, no por requisito).

1. **Registro de paciente** — formulario que consume `POST /api/patients` (CA-1).
2. **Detalle de paciente** — datos del paciente + historial de contactos (`GET /api/patients/{id}` y `GET .../contacts`), con acción "Registrar contacto" (CA-2) y "Corregir" por fila de contacto (CA-3), mostrando si un contacto tiene correcciones.
3. **Listado de pacientes** — navegación mínima de soporte hacia el detalle; no corresponde a ningún CA, solo permite llegar a la pantalla 2.

## 5. Secuencia de trabajo

| # | Tarea | Tiempo estimado |
|---|---|---|
| 1 | Repositorio, `.gitignore`, estructura de carpetas, commit de `01-hallazgos.md` y `02-plan.md` | 15 min |
| 2 | Scripts SQL versionados: `Manager`, `Patient`, `Contact`, `ContactCorrection` (en ese orden, por las FK) | 40 min |
| 3 | API: DTOs + endpoint de paciente (CA-1) + servicio + prueba `CA1_*` | 45 min |
| 4 | API: endpoint de contacto (CA-2) + servicio + prueba `CA2_*` | 40 min |
| 5 | API: corrección de contacto (CA-3) + consulta de historial + servicio + prueba `CA3_*` | 50 min |
| 6 | Script de datos de prueba (seed) | 20 min |
| 7 | Angular: proyecto + Tailwind CSS + formulario de registro de paciente + servicio inyectado | 40 min |
| 8 | Angular: detalle de paciente, registrar y corregir contacto | 50 min |
| 9 | `03-bitacora.md`: matriz de trazabilidad + registro de decisiones | 25 min |
| 10 | README, prueba en limpio, ajustes finales | 30 min |

Total: 15+40+45+40+50+20+40+50+25+30 = 355 min (**5h55min**), dentro del rango de 4-6 horas que indica el documento.

## 6. Riesgos

| Riesgo | Mitigación |
|---|---|
| Se agota el tiempo antes de llegar a las pruebas de interfaz Angular | Son opcionales según el documento; se prioriza siempre la prueba de servicio (xUnit) obligatoria por CA antes de cualquier prueba de UI |
| El script SQL o el README no funcionan en una máquina limpia | Se prueba el README siguiéndolo al pie de la letra en un entorno nuevo antes de entregar, no se asume que "funciona en mi máquina" |
| La lógica de corrección con histórico (`ContactCorrection`) toma más tiempo del estimado | Se implementa primero el camino feliz (crear paciente → contacto → corregir) y se dejan validaciones adicionales como incremento posterior si sobra tiempo |
| SQL Server no está disponible fácilmente en el entorno de desarrollo | Se documenta en el README una alternativa con contenedor Docker de SQL Server, o LocalDB para desarrollo local |

## 7. Extensión móvil

El gestor en campo necesita registrar y corregir contactos sin señal. En el dispositivo se guarda una copia local de los pacientes asignados al gestor (solo lectura, para poder identificar a quién se contacta) y una cola de operaciones pendientes (crear contacto, corregir contacto) con un identificador temporal local, generadas mientras está desconectado. La sincronización ocurre al recuperar conexión, de forma automática en segundo plano y también con un botón manual, reintentando la cola en orden hasta confirmar cada operación contra el servidor.

Si el mismo contacto fue corregido en el servidor mientras el teléfono estaba desconectado, no se aplica la corrección local como una sobrescritura ciega: el mismo diseño de auditoría del modelo de datos (nunca sobrescribir sin dejar rastro) se extiende al conflicto. El servidor evalúa la corrección offline contra la versión vigente; si coincide, se aplica y queda registrada como una corrección más. Si hay conflicto real (el servidor tiene un valor distinto al que el gestor vio antes de desconectarse), la operación se marca como pendiente de revisión y se le muestra al gestor para que decida manualmente, en vez de que el sistema elija un lado automáticamente. Para la creación de pacientes, el mismo riesgo de duplicado se resuelve al momento de sincronizar: si la restricción única `(Country, DocumentType, DocumentNumber)` ya existe en el servidor, la creación offline se rechaza como duplicado y queda pendiente de que el gestor la revise, en vez de crear un segundo paciente.
