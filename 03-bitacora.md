# Bitácora

## Herramientas de IA usadas

Claude Code (Claude Sonnet 5), usado en la totalidad del ejercicio: lectura crítica del PRD,
redacción del plan, modelo de datos, código de las tres capas (SQL, .NET, Angular), pruebas,
y esta misma bitácora. Se declara explícitamente porque así lo exige la regla del ejercicio —
no se declara "cero uso de IA" porque no sería cierto.

## Matriz de trazabilidad

| Criterio | Commit / archivos | Prueba | Estado |
|---|---|---|---|
| CA-1 | `8e96d21` (scripts `001`/`002`), `933c617` (`Patient`, `PatientService`), `2ff5a85` (EF Core, `PatientsController`), `aa61e01` (manejo de errores de restricción), `21645ee` (seed), `d1414ea` (`GET /api/managers`, CORS), `f0fca37` (formulario Angular) | `PatientServiceTests.cs`: `CA1_RegisterNewPatient_IsAddedSuccessfully`, `CA1_RegisterDuplicatePatient_ThrowsDuplicatePatientException` | Cubierto |
| CA-2 | — | — | Fuera de alcance (todavía — en construcción) |
| CA-3 | — | — | Fuera de alcance (todavía — en construcción) |
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
