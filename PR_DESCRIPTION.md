PR: Settings management + secure SMTP + E2E

Summary
- Adds persistent Lab settings management (identity + SMTP) for SUPER_ADMIN.
- Stores SMTP password protected at rest using `IDataProtection` (base64 of protected bytes).
- Centralizes email sending to prefer DB-backed `LabSettings` (single source of truth) and falls back to `appsettings` when DB row is not configured.
- Adds an SMTP test endpoint and service used by admin UI to verify credentials.
- Wires frontend RTK Query slice + minimal Settings UI.
- Adds unit tests and an E2E test that updates DB SMTP settings and sends a test email (requires Docker).

Key files changed
- `LIMTIC.Application.Services.Settings.SettingsService.cs` — new settings application service; protects SMTP password before persisting. (needed)
- `LIMTIC.Infrastructure.Repositories.SettingsRepository.cs` — repository for `LabSettings`. (needed)
- `LIMTIC.Infrastructure.Services.SmtpService.cs` — SMTP tester that unprotects password. (needed)
- `LIMTIC.Infrastructure.Emails.EmailService.cs` — prefers DB-backed SMTP for app emails and unprotects password. (needed)
- `LIMTIC.Infrastructure.IOC.RegisterInfrastructureModule.cs` — registers `ISettingsRepository`, `ISmtpService`, and `AddDataProtection()`. (needed)
- `LIMTIC.WebAPI.Controllers.SettingsController.cs` — API endpoints for GET/PUT and smtp/test (protected to SUPER_ADMIN). (needed)
- `LIMTIC.Application.DTOs.Settings.*` — DTOs for update and response (response DTO deliberately omits password). (needed)
- `LIMTIC.UnitTests.Tests.SettingsServiceTests.cs` — unit tests for data protection and DTO shape. (needed)
- `LIMTIC.E2Es.Tests.SettingsE2ETests.cs` — E2E test for SMTP flow (requires Docker; added). (added — cannot run here)

Notes / Run instructions
- Unit tests:
  - Run: `dotnet test LIMTIC.Service/LIMTIC.UnitTests/LIMTIC.UnitTests.csproj`
  - These tests pass in CI and locally in this workspace.

- E2E tests (MailHog + Postgres via Testcontainers):
  - Require Docker. From repository root run:

```powershell
dotnet test LIMTIC.Service/LIMTIC.E2Es/LIMTIC.E2Es.csproj
```

  - In this environment Docker is not available, so E2E run failed. Locally (or CI runner with Docker) the E2E should pass.

- Database migration:
  - A migration must be created/applied to add `LabSettings` table if not present.
  - Commands (local developer):

```powershell
dotnet tool install --global dotnet-ef
dotnet ef migrations add AddLabSettings --project "LIMTIC.Service/LIMTIC.Infrastructure/LIMTIC.Infrastructure.csproj" --startup-project "LIMTIC.Service/LIMTIC.WebAPI/LIMTIC.WebAPI.csproj" -o Data/Migrations
dotnet ef database update --project "LIMTIC.Service/LIMTIC.Infrastructure/LIMTIC.Infrastructure.csproj" --startup-project "LIMTIC.Service/LIMTIC.WebAPI/LIMTIC.WebAPI.csproj"
```

Security notes
- SMTP password is stored protected using `IDataProtector` and serialized as base64 of the protected bytes. The code falls back to legacy plaintext if protection fails (migration path).

Suggested PR description (copy into GitHub/GitLab PR):
"Add Settings management for SUPER_ADMIN with DB-backed SMTP settings, secure storage of SMTP password using ASP.NET Core Data Protection, centralize EmailService to prefer DB settings, add SMTP test endpoint/service, wire minimal frontend, and add unit + E2E tests. Requires applying EF migration `AddLabSettings` and running E2E with Docker-enabled CI."
