# FollowUp -- Patient Follow-Up Tracking

A small system for tracking patient follow-up contacts (calls, WhatsApp, email)
for a health program, built as a technical assessment. Scope for this delivery
covers three acceptance criteria:

- **CA-1**: register a patient.
- **CA-2**: register a follow-up contact for a patient.
- **CA-3**: correct a contact that was recorded with an error, keeping a full
  audit trail (what changed, who changed it, when, and why), plus a query
  that combines tables to show a patient's contact history with corrections.

See [`01-hallazgos.md`](01-hallazgos.md) (findings on the PRD), [`02-plan.md`](02-plan.md)
(data model and scope decisions) and [`03-bitacora.md`](03-bitacora.md) (traceability
matrix, decision log, and which AI assistant was used and where -- Claude Code,
used throughout).

## Prerequisites

- [Docker](https://www.docker.com/) (for SQL Server)
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 18+](https://nodejs.org/) and npm (Angular 17 requires it)
- A bash-compatible shell (Git Bash on Windows, or a native shell on Linux/Mac)
  for the commands below

## 1. Start SQL Server

```bash
cp .env.example .env
# edit .env and set a real password, then load it into this shell:
export $(cat .env | xargs)

docker compose up -d
```

## 2. Configure the API

```bash
cp api/FollowUp.Api/appsettings.Example.json api/FollowUp.Api/appsettings.Development.json
```

Edit that new file and put the **same password** you set in `.env` in the
`FollowUpDb` connection string.

## 3. Create the database and load the schema

The numbered scripts in `scripts/` create the schema and seed data; none of
them create the database itself (that's a one-time step, not part of the
versioned, repeatable schema), so it has to run first:

```bash
docker exec followup-db /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -C \
  -Q "CREATE DATABASE FollowUpDb;"

for f in scripts/*.sql; do
  echo "Running $f..."
  docker exec -i followup-db /opt/mssql-tools18/bin/sqlcmd \
    -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -C -d FollowUpDb < "$f"
done
```

This creates `Manager`, `Patient`, `Contact` and `ContactCorrection`, and seeds
3 managers, 3 patients and 6 contacts (2 per patient) -- all fictional data,
see `005_seed_data.sql` and `006_seed_contacts.sql`. None of them has been
corrected yet, so the "Corregido" column will show "No" until you correct one
yourself (see CA-3 below).

> **Windows/Git Bash note**: if any `docker exec` command above fails with a
> path-related error, prefix it with `MSYS_NO_PATHCONV=1` -- Git Bash rewrites
> paths that look absolute (like `/opt/...`) before they reach Docker.

## 4. Run the API

```bash
cd api
dotnet run --project FollowUp.Api --urls "http://localhost:5129"
```

Leave this running. The Angular app is hardcoded to call `http://localhost:5129/api`
(see `web/src/app/patients/patients.service.ts`), and the API's CORS policy only
allows `http://localhost:4200` (see `Program.cs`) -- both ports matter.

## 5. Run the frontend

In a separate terminal:

```bash
cd web
npm install
npm start
```

Open `http://localhost:4200`.

## Trying each CA in the browser

**CA-1 -- register a patient**: on the home page (`/`), fill the form (manager,
name, country, document type/number, phone, city, treatment start date) and
submit. The 3 seeded managers should already populate the "Gestor" dropdown.

**CA-2 -- register a contact**: click "Registrar un contacto" (`/contactos/nuevo`),
pick one of the seeded patients, a manager, a date, channel and result, and
submit.

**CA-3 -- view history and correct a contact**: click "Ver historial de contactos"
(`/pacientes/historial`), pick a patient, and you'll see their contacts. Click
"Corregir" on any row to open `/contactos/:id/corregir` -- the form preloads
the contact's current values; change only what you want to correct, pick who's
correcting it and why, and save. Back on the history screen, that contact now
shows "Sí (n)" under "Corregido"; click it to expand the full correction
history, showing only the fields that actually changed in each correction.

**Error case, end to end**: on the correction form, try submitting without
changing any field, or without picking a manager -- both are rejected in the
browser before any request is sent, with a specific message under the field
that's missing. The same validation is enforced by the API independently
(see the `curl` examples below): the browser check is a UX shortcut, not the
real guard.

## Trying the API directly with curl

With the API running (step 4):

```bash
# List seeded managers and patients
curl http://localhost:5129/api/managers
curl http://localhost:5129/api/patients

# Register a contact (replace 6 with a real patient id from the list above)
curl -X POST http://localhost:5129/api/patients/6/contacts \
  -H "Content-Type: application/json" \
  -d '{"managerId":3,"contactDate":"2026-03-01","channel":"Call","result":"NotAnswered","notes":"First attempt"}'

# Correct that contact (replace 6 with the id the previous call returned)
curl -X PUT http://localhost:5129/api/contacts/6 \
  -H "Content-Type: application/json" \
  -d '{"correctedByManagerId":4,"reason":"Result was recorded wrong, they actually answered","result":"Answered"}'

# See the patient's full contact history with corrections
curl http://localhost:5129/api/patients/6/contacts

# Error case: correction with no reason and no field to correct -> 400
curl -i -X PUT http://localhost:5129/api/contacts/6 \
  -H "Content-Type: application/json" \
  -d '{"correctedByManagerId":4,"reason":""}'
```

Valid values: `channel` is `Call`, `WhatsApp` or `Email`; `result` is
`Answered`, `NotAnswered` or `DeclinedFollowUp`.

## Running the tests

```bash
cd api
dotnet test
```

Tests are named after the acceptance criterion they verify (`CA1_*`, `CA2_*`,
`CA3_*`) in `FollowUp.Application.Tests`.

## Project layout

```
api/            .NET 8 Web API (Controller -> Service -> Repository), EF Core
web/            Angular 17 frontend
scripts/        Numbered, idempotent SQL schema/seed scripts
01-hallazgos.md PRD findings
02-plan.md      Data model and scope decisions
03-bitacora.md  Traceability matrix and decision log (includes AI usage)
```
