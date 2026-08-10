<!--
Copyright (c) 2021 HCL CORPORATION.
All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
HCL is obtained. This is proprietary and confidential to HCL.
-->

# Deployment

## Local

1. Build the solution:
   - `dotnet build HCL.CS.sln`
2. Run identity runtime:
   - `dotnet run --project demos/HCL.CS.Demo.Server/HCL.CS.DemoServerApp.csproj`
3. Run installer:
   - `dotnet run --project installer/HCL.CS.Installer.Mvc/HclCsInstallerMVC.csproj`

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
