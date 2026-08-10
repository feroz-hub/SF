<!--
Copyright (c) 2021 HCL CORPORATION.
All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
HCL is obtained. This is proprietary and confidential to HCL.
-->

# HCL.CS Key Rotation SOP

## Scope
- Signing keys used by `/security/token` and published by `/.well-known/openid-configuration/jwks`.
- Confidential client secrets stored outside source control.

## Preconditions
- Vault/KMS is the source of truth for `HCL_CS_RSA_SIGNING_CERT_*`, `HCL_CS_ECDSA_SIGNING_CERT_*`, and `HCL_CS_SIGNING_CERT_PASSWORD`.
- Monitoring is active for token validation failures and JWKS fetch errors.

## Rotation Window
1. `T0`: Generate new signing certificates in KMS/HSM.
2. `T0 + 0h`: Deploy new cert material to all identity nodes with new `kid`.
3. `T0 + 0h`: Keep previous verification keys published in JWKS during overlap window.
4. `T0 + 24h`: Start signing with new key (`kid` switch).
5. `T0 + 48h`: Revoke old signing key for issuance, keep old public key in JWKS.
6. `T0 + 7d`: Remove old key from JWKS after max token lifetime and refresh overlap expires.

## Operational Steps
1. Create and escrow new key pair in vault.
2. Publish vault version metadata and planned `kid`.
3. Deploy identity service with new env vars:
   - `HCL_CS_RSA_SIGNING_CERT_BASE64`
   - `HCL_CS_ECDSA_SIGNING_CERT_BASE64`
   - `HCL_CS_SIGNING_CERT_PASSWORD`
   - `HCL_CS_RSA_SIGNING_KID`
   - `HCL_CS_ECDSA_SIGNING_KID`
4. Validate:
   - Discovery endpoint reachable.
   - JWKS includes expected `kid`.
   - Token header includes matching `kid`.
5. Monitor 24h:
   - JWT validation errors per client/API.
   - Introspection failure spikes.
6. Retire old key only after overlap completion and rollback window closes.

## Emergency Rollback
1. Restore prior vault version.
2. Redeploy identity nodes with prior `kid` values.
3. Keep both key sets in JWKS until all services stabilize.
4. Trigger incident review and root-cause analysis.

## Compliance Controls
- No certificates or passwords committed to repository.
- Rotation evidence retained (change request, deployment IDs, validator logs).
- Maximum overlap must exceed access token + refresh token validity envelope.
