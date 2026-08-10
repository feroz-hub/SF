<!--
Copyright (c) 2021 HCL CORPORATION.
All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
HCL is obtained. This is proprietary and confidential to HCL.
-->

# Domain Model

Core bounded contexts currently represented in code:

- Identity users, roles, claims, tokens, security questions.
- OAuth/OpenID resources, clients, and authorization flows.
- Audit and notification support models.

Domain source lives in `src/Identity/HCL.CS.Identity.Domain`.
Domain service abstractions live in `src/Identity/HCL.CS.Identity.DomainServices`.
