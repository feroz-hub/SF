# Architecture Map

## Layer Boundaries

```mermaid
flowchart LR
    API[API / Hosting\nsrc/Identity/Zentra.Identity.API] --> APP[Application\nsrc/Identity/Zentra.Identity.Application]
    API --> INFRA[Infrastructure + Persistence\nsrc/Identity/Zentra.Identity.Infrastructure\nsrc/Identity/Zentra.Identity.Persistence]
    APP --> ABS[Abstractions\nsrc/Identity/Zentra.Identity.DomainServices]
    ABS --> DOMAIN[Domain\nsrc/Identity/Zentra.Identity.Domain]
    INFRA --> ABS
    INFRA --> DOMAIN
```

## Initial Violations Found

1. API contained direct database connection access.
   - `src/Identity/Zentra.Identity.API/Extensions/ZentraExtension.cs`
2. Application services contained direct EF-Core query/update operations.
   - `src/Identity/Zentra.Identity.Application/Implementation/Api/Services/RoleService.cs`
   - `src/Identity/Zentra.Identity.Application/Implementation/Api/Services/UserAccountService.cs`
   - `src/Identity/Zentra.Identity.Application/Implementation/Endpoint/Services/AuthorizationService.cs`
   - `src/Identity/Zentra.Identity.Application/Implementation/Endpoint/Services/TokenGenerationService.cs`
3. Tenant context abstraction was missing from application-facing dependencies.

## Violations Resolved

1. Moved API DB connectivity checks behind abstraction.
   - Added abstraction: `src/Identity/Zentra.Identity.DomainServices/Infra/IDbConnectionValidator.cs`
   - Added implementation: `src/Identity/Zentra.Identity.Persistence/Validation/DbConnectionValidator.cs`
   - API now depends on abstraction/concrete validator instead of raw SQL connection types:
     - `src/Identity/Zentra.Identity.API/Extensions/ZentraExtension.cs`

2. Removed direct EF operations from application orchestration services.
   - Added token command abstraction:
     - `src/Identity/Zentra.Identity.DomainServices/Repository/Api/ISecurityTokenCommandRepository.cs`
   - Added token command implementation:
     - `src/Identity/Zentra.Identity.Persistence/Repository/Api/SecurityTokenCommandRepository.cs`
   - Moved role existence check into repository abstraction:
     - Interface: `src/Identity/Zentra.Identity.DomainServices/Repository/Api/IRoleRepository.cs`
     - Impl: `src/Identity/Zentra.Identity.Persistence/Repository/Api/RoleRepository.cs`
   - Moved "find user including deleted" into repository abstraction:
     - Interface: `src/Identity/Zentra.Identity.DomainServices/Repository/Api/IUserRepository.cs`
     - Impl: `src/Identity/Zentra.Identity.Persistence/Repository/Api/UserRepository.cs`
   - Application services now call abstractions:
     - `src/Identity/Zentra.Identity.Application/Implementation/Api/Services/RoleService.cs`
     - `src/Identity/Zentra.Identity.Application/Implementation/Api/Services/UserAccountService.cs`
     - `src/Identity/Zentra.Identity.Application/Implementation/Endpoint/Services/AuthorizationService.cs`
     - `src/Identity/Zentra.Identity.Application/Implementation/Endpoint/Services/TokenGenerationService.cs`

3. Introduced tenant context abstraction and injected it through application-facing service graph.
   - Added abstraction: `src/Identity/Zentra.Identity.DomainServices/Infra/ITenantContext.cs`
   - Added implementation: `src/Identity/Zentra.Identity.Infrastructure/Implementation/HttpTenantContext.cs`
   - Registered in DI (composition root/infrastructure):
     - `src/Identity/Zentra.Identity.Infrastructure/Extension/InfrastructureServiceExtension.cs`
   - Consumed by application service:
     - `src/Identity/Zentra.Identity.Application/Implementation/Endpoint/Services/TokenGenerationService.cs`

4. Added architecture regression tests for required boundaries.
   - `tests/Zentra.ArchitectureTests/LayerDependencyTests.cs`
   - `tests/Zentra.ArchitectureTests/README.md`

