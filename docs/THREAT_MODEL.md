<!--
Copyright (c) 2021 HCL CORPORATION.
All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
HCL is obtained. This is proprietary and confidential to HCL.
-->

# Threat Model

## Primary assets

- User credentials and tokens.
- Client secrets and signing keys.
- Audit records.

## Key threat categories

- Credential theft and replay.
- Token forgery and key compromise.
- Privilege escalation via weak authorization checks.
- Sensitive data leakage in logs or error responses.

## Controls baseline

- Strong hashing and key storage controls.
- HTTPS-only deployment for all public endpoints.
- Least-privilege service permissions and DB credentials.
- Centralized logging with tamper-evident retention.
