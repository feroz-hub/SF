# Zentra

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
dotnet build Zentra.sln
```

## Docker Quick Start

Run the standalone Zentra identity server with PostgreSQL and Redis:

```bash
docker compose -f docker/docker-compose.yml up --build -d
```

Default local endpoints:

- Zentra: `https://localhost:5180`
- PostgreSQL: `localhost:55433`
- Redis: `localhost:56380`

Default PostgreSQL credentials:

- Database: `zentra`
- User: `zentra`
- Password: `zentra`

Startup behavior in Docker:

- PostgreSQL starts first
- Redis starts first
- Zentra waits for PostgreSQL to become ready
- Zentra applies the PostgreSQL bootstrap SQL from `scripts/seed/PostgreSql/ZentraPostgreSqlV1.sql`
- Zentra applies every available PostgreSQL migration from `scripts/migrations/*_postgresql.sql`
- Zentra generates a self-signed HTTPS certificate in Docker on first boot if none exists
- Zentra then starts the identity server

Useful commands:

```bash
docker compose -f docker/docker-compose.yml logs -f zentra
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
  `Host=postgres;Port=5432;Database=zentra;Username=zentra;Password=zentra;`
- From host tools such as Rider Database:
  `Host=localhost;Port=55433;Database=zentra;Username=zentra;Password=zentra;`

## Run demo identity host

```bash
dotnet run --project demos/Zentra.Demo.Server/Zentra.DemoServerApp.csproj
```
