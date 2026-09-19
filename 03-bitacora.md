# Bitácora

## Herramientas de IA usadas

Claude Code (Claude Sonnet 5), usado en la totalidad del ejercicio: lectura crítica del PRD,
redacción del plan, modelo de datos, código de las tres capas (SQL, .NET, Angular), pruebas,
y esta misma bitácora. Se declara explícitamente porque así lo exige la regla del ejercicio —
no se declara "cero uso de IA" porque no sería cierto.

## Matriz de trazabilidad

| Criterio | Commit / archivos | Prueba | Estado |
|---|---|---|---|
| CA-1 | `8e96d21` (scripts `001`/`002`), `933c617` (`Patient`, `PatientService`), `2ff5a85` (EF Core, `PatientsController`), `aa61e01` (manejo de errores de restricción), `21645ee` (seed), `d1414ea` (`GET /api/managers`, CORS), `f0fca37` (formulario Angular), `a5d1c1f` (corrección de nulabilidad), `10e0dee` (formato de teléfono, correo vacío) | `PatientServiceTests.cs`: `CA1_RegisterNewPatient_IsAddedSuccessfully`, `CA1_RegisterDuplicatePatient_ThrowsDuplicatePatientException` | Cubierto |
| CA-2 | `98593fc` (`Contact`, `ContactService`), `3589869` (EF Core), `9b19592` (`ContactsController`, `GET /api/patients`), `168fcc8` (formulario Angular), `0c191e6` (mensajes de campo) | `ContactServiceTests.cs`: `CA2_RegisterContact_IsAddedSuccessfully`, `CA2_RegisterContactForNonexistentPatient_ThrowsPatientNotFoundException` | Cubierto |
| CA-3 | `40c7c25` (`ContactCorrection`, `ContactHistory`, repositorio/servicio), `3be835f` (tests), `a050740` (`ContactsController`, manejo de errores), `b13c579` (pantallas Angular), `1a1019c` (seed de contactos), `c1a9e0c` (índices de la consulta con criterio), `ed867ce` (404 de paciente inexistente en el historial), `0c191e6` (mensajes de campo) | `ContactServiceTests.cs`: `CA3_CorrectContact_UpdatesContactAndRecordsCorrection`, `CA3_CorrectContactWithNoFieldsProvided_ThrowsArgumentException`, `CA3_CorrectNonexistentContact_ThrowsContactNotFoundException`, `CA3_GetHistoryForNonexistentPatient_ThrowsPatientNotFoundException` | Cubierto |
| CA-4, CA-5, CA-6 | — | — | Fuera de alcance (justificado en `02-plan.md`, sección 2) |

Manejo de errores compartido por los 3 CA: `0609445` (`InvalidModelStateResponseFactory.cs`, `Program.cs`) -- normaliza las claves de `errors` a camelCase y humaniza los mensajes de enum/fecha inválidos.

## Registro de decisiones y uso de IA

- Se fijaron reglas de trabajo (`CLAUDE.md`) antes de tocar el PRD, distinguiendo lo estricto de lo negociable. Propuesta de Claude, aprobada por Jorge.
- Los 9 hallazgos iniciales de `01-hallazgos.md` (hoy 10, tras el hallazgo #10 agregado más tarde) se priorizaron por bloqueo real sobre lo cosmético. Criterio propuesto por Claude, confirmado por Jorge.
- El hallazgo del denominador móvil en el reporte de adherencia salió de una intuición de Jorge, validada por Claude con un ejemplo numérico antes de aceptarla.
- El hallazgo de falta de autenticación/roles de gestores surgió de una pregunta directa de Jorge, no de un hallazgo que Claude hubiera detectado por sí solo.
- **Corrección a la IA (1/6):** Claude afirmó que "Manager entra desde CA-2" como si el criterio lo exigiera; Jorge verificó contra el texto literal — solo CA-3 lo exige (hallazgo #5) — y se agregó la regla de doble verificación a `CLAUDE.md` para no repetirlo.
- Alcance cerrado en CA-1/CA-2/CA-3 (no CA-4/5/6): decisión conjunta, son los que dependen de menos supuestos propios encadenados.
- Se consideró construir autenticación real (Basic Auth) para `ManagerId`; se descartó porque ningún CA la exige y la Parte V ("cambio en caliente") ya prevé razonar sobre esto sin construirlo — decisión de Jorge tras el análisis de Claude.
- Se agregó `RegisteredByManagerId` a `Patient`/`Contact`/`ContactCorrection` como identidad declarada y no verificada, documentado así en cada tabla.
- Se lanzaron 3 agentes a auditar `01-hallazgos.md`/`02-plan.md`, y luego otros 3 a auditar el código de CA-1 — pedido explícito de Jorge en ambos casos.
- **Corrección a la IA (2/6):** la auditoría encontró que solo `DuplicatePatientException` se manejaba centralizadamente (un `registeredByManagerId` inexistente daba `500` genérico); se corrigió con `DatabaseConstraintExceptionHandler` (commit `aa61e01`), verificado en vivo.
- Se renombró el proyecto de `Tbtb` a `FollowUp` (el primero era el nombre de la evaluadora); se descartó reescribir el historial para ocultarlo, se hizo con un commit explícito — decisión de Jorge.
- Se agregó Tailwind CSS al frontend por decisión de Jorge, confirmando antes que no afecta ninguna regla del examen.
- Se agregó `GET /api/managers` (no declarado en `02-plan.md`) al faltar una fuente para el selector de gestores del formulario Angular; documentado en el plan antes de construirse (commit `a73bac4`).
- El formulario de CA-1 se verificó con un navegador real (Playwright headless), no solo compilación — decisión de Claude de no declarar la UI funcionando sin evidencia visual.
- Se anticipó `GET /api/patients` desde el plan de CA-2, en vez de descubrirlo a mitad de camino como pasó con `GET /api/managers` en CA-1.
- Se decidió que "paciente inexistente" (404, de la URL) y "gestor inexistente" (400, del cuerpo) necesitan manejo distinto pese a ser ambos violaciones de FK; se construyó `PatientNotFoundException` en vez de reutilizar el manejador genérico.
- Se lanzaron de nuevo 3 agentes a auditar el código de CA-2 — pedido explícito de Jorge.
- **Corrección a la IA (3/6):** un campo enum/fecha omitido pasaba silenciosamente al primer valor del enum en vez de fallar; se corrigió haciendo los campos anulables (commit `a5d1c1f`), verificado en vivo.
- La consulta con criterio (`ContactRepository.GetHistoryByPatientIdAsync`) combina `Contact`+`ContactCorrection`+`Manager` en un solo `Select` anidado, resolviendo el nombre del gestor que corrigió; verificada en vivo con dos gestores distintos (uno registra, otro corrige); necesita `Contact.PatientId` (filtro principal) y `ContactCorrection.ContactId` (se repite una vez por contacto) como índices, porque una FK no crea índice por sí sola en SQL Server (agregados en `007_add_contact_history_indexes.sql`).
- Se lanzaron de nuevo 3 agentes a auditar el código de CA-3 — pedido explícito de Jorge, mismo patrón que CA-1/CA-2.
- **Corrección a la IA (4/6):** la regla "al menos un campo a corregir" solo vivía en `CorrectContactRequest.Validate()` (frontera del API), no en el servicio; se agregó la misma guarda dentro de `ContactService.CorrectAsync` (`ArgumentException`) y se corrigió el test que dependía de ese hueco, más `.FirstOrDefault()` en vez de `.First()` sobre `Managers` por robustez.
- Se verificó, releyendo `01-hallazgos.md` literal antes de proponer nada, que la falta de autenticación en `CorrectedByManagerId` ya estaba cubierta por el hallazgo #1 — no hizo falta nada nuevo.
- El frontend de CA-3 se verificó con Playwright igual que CA-1/CA-2, incluido el caso de error de punta a punta: enviar la corrección sin campos dispara la validación de grupo en el navegador antes de llegar al API.
- Se releyó el PDF completo de la prueba, palabra por palabra, contra el repositorio real — pedido explícito de Jorge — y encontró que la bitácora nunca justificaba qué índice necesitaría la consulta con criterio; se cerró con el razonamiento de arriba y `007_add_contact_history_indexes.sql`, verificado en una base aislada.
- La misma relectura encontró que `02-plan.md` prometía `404` para paciente inexistente en `GET /api/patients/{patientId}/contacts` y el código devolvía lista vacía; se ajustó el código (no el plan) porque el costo era bajo: `IContactService.GetHistoryByPatientIdAsync` ahora valida existencia y lanza `PatientNotFoundException`, cubierto por `CA3_GetHistoryForNonexistentPatient_ThrowsPatientNotFoundException` y verificado en vivo.
- La misma relectura encontró que `02-plan.md` preveía 3 pantallas Angular (registro, detalle-con-todo, listado) y se construyeron 4 (registro, registrar-contacto aparte, historial-con-selector, corregir); se decidió documentar la desviación en vez de rehacer el frontend, porque la separación actual es más limpia que la fusión planeada y esa fusión se escribió antes de conocer el detalle real de la corrección (precarga, avisos, diff) — el propio examen permite cambiar el plan si se explica por qué.
- **Corrección a la IA (5/6):** al verificar en vivo el caso de error de punta a punta de CA-1 (paciente duplicado), dio `400` en vez de `409` porque `PatientRegistrationComponent.submit()` enviaba `email: ""` cuando el campo opcional quedaba vacío y `[EmailAddress]` rechaza una cadena vacía presente — afectaba a **cualquier** registro sin correo, no solo esta prueba; se corrigió omitiendo `email` del request cuando está vacío, reverificado en vivo con Playwright.
- Jorge probó la aplicación levantada por él mismo y registró un paciente con `Phone = "ytytyu"` (ningún dígito), confirmando el hallazgo #10 de `01-hallazgos.md`; se aplicó el criterio acordado (solo dígitos, 7-15, "+" opcional) en `CreatePatientRequest.Phone` y en el `FormControl` de Angular, verificado en vivo con `curl` directo.
- **Corrección a la IA (6/6) — la más significativa de la sesión:** al mejorar el mensaje de un `Country` inválido (Jorge alteró el `value` de un `<option>` en devtools) se descubrió que ningún mensaje de validación de campo se había mostrado jamás en ningún formulario, porque `[ApiController]` devuelve las claves de `errors` en PascalCase (`"Name"`) mientras Angular siempre las busca en camelCase (`fieldErrors['name']`); se corrigió centralizadamente con `InvalidModelStateResponseFactory` (normaliza claves a camelCase, humaniza mensajes de enum/fecha inválidos, descarta el `"request"` genérico) y se completó agregando `fieldErrors[...]` a los 9 campos que nunca lo habían tenido en los 3 formularios, verificado campo por campo interceptando peticiones reales del navegador.
