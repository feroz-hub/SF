<!--
Copyright (c) 2021 HCL CORPORATION.
All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
HCL is obtained. This is proprietary and confidential to HCL.
-->

# HCL.CS Enterprise Architecture

HCL.CS is organized using layered boundaries:

- `src/Identity`: identity domain, application, infrastructure, persistence, and API composition.
- `src/Gateway`: gateway proxy components.
- `src/Admin`: administrative API/UI surface (scaffolded for next phase).
- `src/SharedKernel`: cross-cutting primitives and domain safety utilities.
- `src/Contracts`: external request/response/event contracts.

## Runtime topology

- Identity runtime is currently hosted by `demos/HCL.CS.Demo.Server`.
- Installer runtime is hosted by `installer/HCL.CS.Installer.Mvc`.
- Gateway is currently a library package and has a container placeholder for CI/CD completeness.

## Design priorities

- Keep domain and persistence decoupled.
- Keep transport contracts separated from domain entities.
- Keep deployment assets (`docker`, `k8s`) versioned with code.
