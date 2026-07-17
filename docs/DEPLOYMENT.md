# Deployment

## Local

1. Build the solution:
   - `dotnet build Zentra.sln`
2. Run identity runtime:
   - `dotnet run --project demos/Zentra.Demo.Server/Zentra.DemoServerApp.csproj`
3. Run installer:
   - `dotnet run --project installer/Zentra.Installer.Mvc/ZentraInstallerMVC.csproj`

## Containers

- Compose file: `docker/docker-compose.yml`
- Dockerfiles:
  - `docker/identity-api.Dockerfile`
  - `docker/admin-api.Dockerfile`
  - `docker/gateway.Dockerfile`

## Kubernetes

Apply manifests from `k8s/`:

- `configmap.yaml`
- `identity-deployment.yaml`
- `identity-service.yaml`
- `ingress.yaml`
