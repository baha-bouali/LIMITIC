# Profile CRUD Implementation — Progress Summary

## Overview

Adding CRUD operations for user profiles (Researcher, PhDStudent, Masterian) to the LIMTIC clean architecture .NET 8.0 project.

---

## Architecture

```
LIMTIC.Domain       → Entities, Enums, Repository Interfaces
LIMTIC.Application  → Services, Commands, DTOs, Validators, Mappers
LIMTIC.Infrastructure → EF Core, Repositories, AppDbContext
LIMTIC.WebAPI       → Controllers, Request/Response Models
```

**Key patterns:** Direct service pattern (no MediatR), `Result<T>` response wrapper, FluentValidation, manual mapping, `BaseResponse` for HTTP responses.

---

## User Onboarding Flow

```
SuperAdmin creates user  →  Role = Visitor (always)
         ↓
Admin updates role       →  Researcher / PhDStudent / Masterian
         ↓                   (profile entity created in same request)
Admin/User manages profile →  Update fields, Delete (resets to Visitor)
```

---

## Authorization

| Operation | Allowed Roles |
|-----------|--------------|
| Create user | SuperAdmin |
| Update role + create profile | SuperAdmin, Admin |
| Get profile | Any authenticated user |
| Update profile (all fields) | SuperAdmin, Admin |
| Update own profile (subset) | Any authenticated user |
| Delete profile | SuperAdmin, Admin |

---

## Implementation Status

### ✅ Phase 1 — Entity Modifications (COMPLETE)

| File | Change |
|------|--------|
| `ResearcherEntity.cs` | Added `ICollection<ResearchAxisEntity> ResearchAxes` |
| `PhDStudentEntity.cs` | Added `ICollection<ResearchAxisEntity> ResearchAxes`; made `ThesisSubject`, `SupervisorId`, `Supervisor` nullable |
| `MasterianEntity.cs` | Made `SupervisorId`, `Supervisor` nullable |
| `ResearchAxisEntity.cs` | Added `ICollection<ResearcherEntity> Researchers` and `ICollection<PhDStudentEntity> PhDStudents` |
| `ResearcherConfiguration.cs` | Added many-to-many with ResearchAxes |
| `PhDStudentConfiguration.cs` | Added many-to-many with ResearchAxes; made SupervisorId optional |
| `MasterianConfiguration.cs` | Made SupervisorId optional |

> ⚠️ **Pending:** Run EF Core migration: `dotnet ef migrations add AddResearchAxesToProfiles_MakeFieldsOptional`

---

### ✅ Phase 2 — User Creation Always Visitor (COMPLETE)

| File | Change |
|------|--------|
| `CreateUserCommand.cs` | Removed `Role` property |
| `CreateUserRequest.cs` | Removed `Role` property |
| `UsersManagementService.cs` | Hardcoded `role: UserRole.Visitor` in `CreateUserAsync` |

> ⚠️ **Pending:** Remove `Role =` mapping from `UsersManagementController.cs` (`AddUser` action)

---

### 🔲 Phase 3 — Update Role + Create Profile (TODO)

New `PUT api/users/{userId}/role` endpoint that:
- Changes user's role enum
- Creates the corresponding profile entity in the same transaction

**Files to create:**

| Layer | File | Purpose |
|-------|------|---------|
| Domain | `IResearcherRepository.cs` | Interface with `GetByUserIdAsync`, `AddAsync`, `UpdateAsync`, `DeleteAsync`, `ExistsAsync` |
| Domain | `IPhDStudentRepository.cs` | Same shape (no `ExistsAsync`) |
| Domain | `IMasterianRepository.cs` | Same shape |
| Domain | `IResearchAxisRepository.cs` | `GetByIdsAsync(List<Guid>)` for resolving axis IDs |
| Infrastructure | `ResearcherRepository.cs` | EF Core impl with `.Include(r => r.ResearchAxes)` |
| Infrastructure | `PhDStudentRepository.cs` | EF Core impl with includes |
| Infrastructure | `MasterianRepository.cs` | EF Core impl with includes |
| Infrastructure | `ResearchAxisRepository.cs` | EF Core impl |
| Application | `UpdateUserRoleCommand.cs` | Flat command: Role + nullable role-specific fields |
| Application | `UpdateUserRoleCommandValidator.cs` | Validates UserId, Role |
| WebAPI | `UpdateUserRoleRequest.cs` | HTTP request model |

**Files to modify:**

| File | Change |
|------|--------|
| `IUsersManagementService.cs` | Add `UpdateUserRoleAsync(UpdateUserRoleCommand)` |
| `UsersManagementService.cs` | Implement role-dispatch logic with profile creation |
| `UsersManagementController.cs` | Add `PUT updateRole/` endpoint; remove `Role =` from `AddUser` |
| `RegisterInfrastructureModule.cs` | Register 4 new repositories |
| `RegisterApplicationModule.cs` | (No new services in this phase) |

**Role-specific required fields:**

| Role | Required Fields |
|------|----------------|
| Researcher | `Rank`, `Specialty`, `Office`, `PhoneNumber`, `ResearchAxisIds?` |
| PhDStudent | `EnrollmentYear` |
| Masterian | `Cohort`, `DissertationSubject` |
| Admin / Visitor | None |

---

### 🔲 Phase 4 — Profile Get / Update / Delete (TODO)

Three controllers (`api/profiles/researchers`, `api/profiles/phd-students`, `api/profiles/masterians`), each with `GET /{userId}`, `PUT /{userId}`, `DELETE /{userId}`.

**Files to create:**

| Layer | File |
|-------|------|
| Application DTOs | `ResearcherProfileDto.cs`, `PhDStudentProfileDto.cs`, `MasterianProfileDto.cs` |
| Application Mapper | `IProfileMapper.cs`, `ProfileMapper.cs` |
| Application Commands | `UpdateResearcherProfileCommand.cs` + Response, `UpdatePhDStudentProfileCommand.cs` + Response, `UpdateMasterianProfileCommand.cs` + Response |
| Application Validators | 3 update validators |
| Application Interfaces | `IResearcherProfileService.cs`, `IPhDStudentProfileService.cs`, `IMasterianProfileService.cs` |
| Application Services | `ResearcherProfileService.cs`, `PhDStudentProfileService.cs`, `MasterianProfileService.cs` |
| WebAPI Models | 3 request models, 3 response models |
| WebAPI Controllers | `ResearcherProfileController.cs`, `PhDStudentProfileController.cs`, `MasterianProfileController.cs` |

**Files to modify:** `RegisterApplicationModule.cs`, `MappersModule.cs`

> Delete profile also resets user's `Role` back to `Visitor`.

---

### 🔲 Phase 5 — EF Core Migration (TODO)

```bash
dotnet ef migrations add AddResearchAxesToProfiles_MakeFieldsOptional --project LIMTIC.Infrastructure --startup-project LIMTIC.WebAPI
dotnet ef database update --project LIMTIC.Infrastructure --startup-project LIMTIC.WebAPI
```

---

## Verification Checklist

- [ ] `dotnet build` — 0 errors
- [ ] Migration runs without errors
- [ ] `dotnet test` — existing tests pass
- [ ] Create user → role is `Visitor`
- [ ] Update role to Researcher (with required fields) → profile created
- [ ] Get researcher profile → combined user + profile + research axes
- [ ] Update researcher profile → fields saved
- [ ] Delete researcher profile → user still exists, role = Visitor
- [ ] Same for PhDStudent and Masterian
- [ ] Missing required fields → validation error
- [ ] Invalid SupervisorId → rejected
- [ ] Non-admin cannot update role
- [ ] User can update own profile fields
