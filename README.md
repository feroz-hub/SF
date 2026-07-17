# HCL.CS

Enterprise-oriented repository structure for identity, gateway, admin, installer, demos, and tests.

## Layout

- `.github/workflows`: CI, docker build, security scan.
- `docker`: container build assets.
- `k8s`: Kubernetes deployment manifests.
- `scripts`: automation and migration scripts.
- `docs`: architecture, domain, threat, deployment, and API collection docs.
- `src`: production source code.
- `installer`: installer runtime.
- `demos`: runnable demo hosts.
- `tests`: integration/unit/architecture testing layout.

## Build

```bash
dotnet build HCL.CS.sln
```

## Docker Quick Start

Run the standalone HCL.CS identity server with PostgreSQL and Redis:

```bash
docker compose -f docker/docker-compose.yml up --build -d
```

Default local endpoints:

- HCL.CS: `https://localhost:5180`
- PostgreSQL: `localhost:55433`
- Redis: `localhost:56380`

Default PostgreSQL credentials:

- Database: `hcl-cs`
- User: `hcl-cs`
- Password: `hcl-cs`

Startup behavior in Docker:

- PostgreSQL starts first
- Redis starts first
- HCL.CS waits for PostgreSQL to become ready
- HCL.CS applies the PostgreSQL bootstrap SQL from `scripts/seed/PostgreSql/HclCsPostgreSqlV1.sql`
- HCL.CS applies every available PostgreSQL migration from `scripts/migrations/*_postgresql.sql`
- HCL.CS generates a self-signed HTTPS certificate in Docker on first boot if none exists
- HCL.CS then starts the identity server

Useful commands:

```bash
docker compose -f docker/docker-compose.yml logs -f hcl-cs
docker compose -f docker/docker-compose.yml ps
docker compose -f docker/docker-compose.yml down
docker compose -f docker/docker-compose.yml down -v
```

Quick HTTPS check:

```bash
curl -k https://localhost:5180/.well-known/openid-configuration
```

Railway deployment:

- [`docs/RAILWAY_DEPLOYMENT.md`](docs/RAILWAY_DEPLOYMENT.md)

The default compose path starts only the identity server and its dependencies. Optional `installer` and `gateway`
services are available behind the `extras` profile:

```bash
docker compose -f docker/docker-compose.yml --profile extras up --build -d
```

Installer endpoint with the `extras` profile:

- Installer UI: `http://localhost:7039`
- Installer health: `http://localhost:7039/health`

Installer PostgreSQL connection strings:

- From the Dockerized installer to the bundled PostgreSQL service:
  `Host=postgres;Port=5432;Database=hcl-cs;Username=hcl-cs;Password=hcl-cs;`
- From host tools such as Rider Database:
  `Host=localhost;Port=55433;Database=hcl-cs;Username=hcl-cs;Password=hcl-cs;`

## Run demo identity host

```bash
dotnet run --project demos/HCL.CS.Demo.Server/HCL.CS.DemoServerApp.csproj
```

## Native Windows setup (no Docker)

The Windows PowerShell scripts under `scripts/windows` configure local PostgreSQL, the supported Installer, HTTPS, the runnable Demo Server, and Admin UI. The complete HCL.CS + SBOM procedure is documented in `../sbom/docs/WINDOWS_NATIVE_SETUP.md` when the repositories are checked out side by side.
