<!--
Copyright (c) 2021 HCL CORPORATION.
All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
HCL is obtained. This is proprietary and confidential to HCL.
-->

# HCL.CS.ArchitectureTests

Architecture regression tests that enforce layer boundaries:

- Domain and DomainServices cannot reference Application or Infrastructure assemblies.
- Application cannot reference Persistence assemblies.
- Persistence cannot reference Application assemblies.
- API source files cannot reference `HCL.CS.Infrastructure.*` outside composition root files.
- Domain source files cannot reference `HCL.CS.Infrastructure.*` namespaces.
