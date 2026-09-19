# Bitácora

## Herramientas de IA usadas

Claude Code (Claude Sonnet 5), usado en la totalidad del ejercicio: lectura crítica del PRD,
redacción del plan, modelo de datos, código de las tres capas (SQL, .NET, Angular), pruebas,
y esta misma bitácora. Se declara explícitamente porque así lo exige la regla del ejercicio —
no se declara "cero uso de IA" porque no sería cierto.

## Matriz de trazabilidad

| Criterio | Commit / archivos | Prueba | Estado |
|---|---|---|---|
| CA-1 | `8e96d21` (scripts `001`/`002`), `933c617` (`Patient`, `PatientService`), `2ff5a85` (EF Core, `PatientsController`), `aa61e01` (manejo de errores de restricción), `21645ee` (seed), `d1414ea` (`GET /api/managers`, CORS), `f0fca37` (formulario Angular), `a5d1c1f` (corrección de nulabilidad) | `PatientServiceTests.cs`: `CA1_RegisterNewPatient_IsAddedSuccessfully`, `CA1_RegisterDuplicatePatient_ThrowsDuplicatePatientException` | Cubierto |
| CA-2 | `98593fc` (`Contact`, `ContactService`), `3589869` (EF Core), `9b19592` (`ContactsController`, `GET /api/patients`), `168fcc8` (formulario Angular) | `ContactServiceTests.cs`: `CA2_RegisterContact_IsAddedSuccessfully`, `CA2_RegisterContactForNonexistentPatient_ThrowsPatientNotFoundException` | Cubierto |
| CA-3 | `40c7c25` (`ContactCorrection`, `ContactHistory`, repositorio/servicio), `3be835f` (tests), `a050740` (`ContactsController`, manejo de errores), `b13c579` (pantallas Angular), `1a1019c` (seed de contactos), `c1a9e0c` (índices de la consulta con criterio), `ed867ce` (404 de paciente inexistente en el historial) | `ContactServiceTests.cs`: `CA3_CorrectContact_UpdatesContactAndRecordsCorrection`, `CA3_CorrectContactWithNoFieldsProvided_ThrowsArgumentException`, `CA3_CorrectNonexistentContact_ThrowsContactNotFoundException`, `CA3_GetHistoryForNonexistentPatient_ThrowsPatientNotFoundException` | Cubierto |
| CA-4, CA-5, CA-6 | — | — | Fuera de alcance (justificado en `02-plan.md`, sección 2) |

## Registro de decisiones y uso de IA

- Se fijaron reglas de trabajo (`CLAUDE.md`) antes de tocar el PRD, distinguiendo lo estricto
  de lo negociable. Propuesta de Claude, aprobada por Jorge.
- Los 9 hallazgos de `01-hallazgos.md` se priorizaron por bloqueo real sobre lo cosmético.
  Criterio propuesto por Claude, confirmado por Jorge.
- El hallazgo del denominador móvil en el reporte de adherencia salió de una intuición de
  Jorge ("si la base de pacientes crece, el porcentaje baja aunque el desempeño mejore");
  Claude la validó con un ejemplo numérico antes de aceptarla como hallazgo.
- El hallazgo de falta de autenticación/roles de gestores surgió de una pregunta directa de
  Jorge ("¿cómo garantiza el sistema que quien registra es un gestor?"), no de un hallazgo que
  Claude hubiera detectado por sí solo.
- **Corrección a la IA (1/2):** Claude afirmó que "Manager entra desde CA-2" como si el
  criterio lo exigiera. Jorge pidió verificarlo contra el texto literal — ni CA-1 ni CA-2 lo
  exigen, solo CA-3 (vía el hallazgo #4/#5). Se corrigió la afirmación y se agregó una regla
  permanente a `CLAUDE.md` (doble verificación antes de proponer) para no repetir el error.
- Alcance cerrado en CA-1/CA-2/CA-3 (no CA-4/5/6): decisión conjunta, justificada por que son
  los que dependen de menos supuestos propios encadenados.
- Se consideró construir autenticación real (Basic Auth) para que `ManagerId` fuera una
  garantía real, no declarada. Se decidió no construirla: ningún CA la exige, y la Parte V
  ("cambio en caliente") es el mecanismo que el propio examen prevé para razonar sobre esto sin
  construirlo. Decisión de Jorge tras el análisis de costo/beneficio presentado por Claude.
- Se agregó `RegisteredByManagerId` a `Patient` (y el mismo campo a `Contact`/`ContactCorrection`)
  como identidad declarada y no verificada, documentado explícitamente como tal en cada tabla —
  para no dejar ninguna pantalla sin pedir al menos ese mínimo.
- Se lanzaron 3 agentes en paralelo a auditar `01-hallazgos.md`/`02-plan.md` contra las reglas
  del PDF, y más tarde otros 3 a auditar el código de CA-1 (seguridad, cumplimiento,
  correctitud técnica). Pedido explícito de Jorge en ambos casos.
- **Corrección a la IA (2/2):** la auditoría de correctitud encontró que solo
  `DuplicatePatientException` se manejaba de forma centralizada — un `registeredByManagerId`
  inexistente, o una condición de carrera en la verificación de duplicados, producían un `500`
  genérico en vez de un `400`/`409` claro. Se corrigió con `DatabaseConstraintExceptionHandler`
  (commit `aa61e01`), verificado en vivo contra la API real antes de aceptarlo como resuelto.
- Se renombró el proyecto de `Tbtb` a `FollowUp` porque el primero era el nombre de la empresa
  evaluadora, sin relación con el dominio. Se descartó reescribir el historial de git para
  ocultar el cambio (contradice la propia regla del examen de "decir por qué" en vez de
  esconder que algo cambió); se hizo con un commit de renombrado explícito. Decisión de Jorge.
- Se agregó Tailwind CSS al frontend por decisión de Jorge, tras confirmar que no afecta
  ninguna regla del examen ("no se evalúa la belleza de la interfaz") y que le ahorra tiempo.
- Se agregó `GET /api/managers` (no declarado originalmente en `02-plan.md`) al descubrir, ya
  construyendo el formulario de Angular, que no existía ninguna fuente para el selector de
  gestores. Documentado en `02-plan.md` con su justificación antes de construirse (commit
  `a73bac4`), no después.
- El formulario de registro de CA-1 se verificó con un navegador real (Playwright, headless)
  en vez de solo confiar en que compilara — se instaló Chromium y se confirmó que el `<select>`
  de gestores carga los 3 gestores reales desde la API, sin errores de consola. Decisión de
  Claude de no declarar la UI funcionando sin evidencia visual real.
- Se anticipó el endpoint `GET /api/patients` desde el plan de CA-2, en vez de descubrirlo a
  mitad de camino como pasó con `GET /api/managers` en CA-1 — aplicando lo ya aprendido.
- Se decidió que "paciente inexistente" (404, viene de la URL) y "gestor inexistente" (400,
  viene del cuerpo) necesitan manejo distinto, aunque los dos son violaciones de FK — se
  construyó `PatientNotFoundException` en vez de reutilizar el manejador genérico existente.
- Se lanzaron de nuevo 3 agentes en paralelo a auditar el código de CA-2 (seguridad,
  cumplimiento, correctitud técnica). Pedido explícito de Jorge.
- **Corrección a la IA (3/3 o más):** los agentes encontraron que un campo enum/fecha omitido
  pasaba silenciosamente en vez de fallar, porque el valor "vacío" de un enum en C# es
  indistinguible de su primer valor real (`Country` vacío se ve igual que `Colombia`). Se
  corrigió cambiando los campos a tipos anulables (commit `a5d1c1f`), verificado en vivo:
  omitir el campo ahora falla con `400`; enviar el valor explícito sigue funcionando igual.
- La "consulta con criterio" exigida por el stack se resolvió en `ContactRepository.
  GetHistoryByPatientIdAsync`: una sola expresión LINQ con un `Select` anidado (subconsultas
  correlacionadas) que combina `Contact` + `ContactCorrection` + `Manager`, resolviendo el
  nombre real del gestor que corrigió en vez de exponer solo su id. Verificado en vivo contra
  SQL Server real: se creó y corrigió un contacto de prueba con dos gestores distintos
  (registrado por uno, corregido por otro) y la consulta devolvió el nombre correcto de quien
  corrigió, no de quien registró. Índices que necesitaría con volumen real (agregados en
  `007_add_contact_history_indexes.sql`): `Contact.PatientId`, porque es el filtro principal de
  la consulta y una FK no crea índice por sí sola en SQL Server -- sin él, cada consulta del
  historial de un paciente escanea toda la tabla `Contact`, cada vez más lenta cuantos más
  contactos acumule el programa en total (~400 pacientes al arrancar, según el PRD), no solo el
  paciente consultado; y `ContactCorrection.ContactId`, más crítico aún porque esa búsqueda se
  repite una vez por cada contacto del paciente (subconsulta correlacionada), multiplicando el
  costo de un table scan por el número de contactos en vez de una sola vez.
- Se lanzaron de nuevo 3 agentes en paralelo a auditar el código de CA-3 (seguridad,
  correctitud, cumplimiento de `CLAUDE.md`). Pedido explícito de Jorge, mismo patrón que en
  CA-1/CA-2.
- **Corrección a la IA (4):** la auditoría de correctitud encontró que la regla "al menos un
  campo a corregir" solo vivía en `CorrectContactRequest.Validate()` (frontera del API); si
  `ContactService.CorrectAsync` se llamara directo sin pasar por ese DTO, se guardaría una
  `ContactCorrection` sin cambio real, pisando `UpdatedAt` sin motivo. Se agregó la misma
  validación como guarda dentro del servicio (`ArgumentException`), y se corrigió el test
  existente que sin querer dependía de ese hueco (usaba los 4 campos `null` solo para simular
  "contacto no encontrado"). Se agregó también `.FirstOrDefault()` en vez de `.First()` sobre
  `Managers` en la consulta con criterio, por robustez, aunque la FK ya impide el caso huérfano.
- Se verificó, releyendo el texto literal de `01-hallazgos.md` antes de proponer nada nuevo
  (regla de doble verificación de `CLAUDE.md`), que la falta de autenticación aplicada a
  `CorrectedByManagerId` **ya** estaba cubierta por el hallazgo #1 y por la decisión ya
  registrada arriba sobre `Contact`/`ContactCorrection` como identidad declarada y no
  verificada — no hizo falta agregar nada nuevo a los hallazgos ni a esta bitácora por ese
  punto.
- El frontend de CA-3 (`patient-detail`, `contact-correction`) se verificó con Playwright
  (headless), igual que CA-1/CA-2: se probó el flujo completo (ver historial → expandir
  correcciones → corregir → volver) y, a diferencia de CA-1/CA-2, también el caso de error de
  punta a punta: enviar el formulario de corrección sin ningún campo dispara la validación de
  grupo en el navegador ("Debes corregir al menos un campo") antes de llegar al API.
- Se releyó el PDF de la prueba completo (`Prueba_Tecnica_Desarrollador_TBTB_2.pdf`), palabra
  por palabra contra el estado real del repositorio, en vez de confiar en el resumen acumulado
  de la sesión. Pedido explícito de Jorge. Encontró un requisito textual de la Parte III sin
  cumplir: "justifica en la bitácora... qué índice necesitaría con volumen real" -- la consulta
  con criterio ya estaba resuelta, pero esta bitácora nunca mencionó ningún índice. Se cerró
  agregando el razonamiento arriba y el script `007_add_contact_history_indexes.sql`
  (`Contact.PatientId`, `ContactCorrection.ContactId`), verificado en una base de datos aislada
  antes de aceptarlo como resuelto.
- La misma relectura encontró dos desviaciones entre `02-plan.md` y el código real, ninguna
  documentada hasta ahora. Decisión de Jorge sobre cómo cerrar cada una, con criterio distinto
  para cada caso:
  - **(1) El `404` de paciente inexistente**: el plan lo prometía en
    `GET /api/patients/{patientId}/contacts`; el código devolvía lista vacía (mismo patrón que
    `PatientsController.GetAll`, nunca escrito como desviación). Se ajustó el código para cumplir
    el plan: `IContactService.GetHistoryByPatientIdAsync` ahora valida la existencia del paciente
    (reutilizando `IPatientRepository.ExistsByIdAsync`, el mismo método de `RegisterAsync`) antes
    de delegar al repositorio, y lanza `PatientNotFoundException` si no existe -- misma excepción
    y manejador que ya existían desde CA-2. De paso, `ContactsController` dejó de inyectar
    `IContactRepository` directamente: las 3 acciones del controlador ahora pasan por
    `IContactService`, cerrando la última excepción a "la lógica de negocio no vive en el
    controlador". Cubierto por el test `CA3_GetHistoryForNonexistentPatient_
    ThrowsPatientNotFoundException` y verificado en vivo (`404` real para un id inexistente,
    `200` para uno real).
  - **(2) 3 pantallas planeadas vs. 4 construidas**: el plan describía "Registro", "Detalle de
    paciente" (historial + registrar contacto + corregir, todo junto) y "Listado". Se construyó,
    en cambio, "Registro", "Registrar contacto" (pantalla propia), "Historial de contactos" (con
    selector de paciente inline, fusionando lo que iba a ser listado+detalle) y "Corregir
    contacto" (pantalla propia con ruta paramétrica). Se decidió **no** rehacer el frontend para
    calzar con el boceto original: la separación por pantalla actual (una responsabilidad cada
    una) es más limpia que fusionar registrar+ver+corregir en una sola, y ese boceto se escribió
    antes de conocer el detalle real de la funcionalidad de corrección (precarga de valores,
    avisos de "Modificado", diff de qué cambió) -- retroceder a la versión fusionada sacrificaría
    diseño ya probado solo para coincidir textualmente con un plan anterior. El propio documento
    de la prueba lo permite explícitamente ("el plan se cierra antes, y cambiarlo después exige
    decir por qué"); esta entrada es esa justificación.
