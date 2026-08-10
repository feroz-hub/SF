<!--
Copyright (c) 2021 HCL CORPORATION.
All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
HCL is obtained. This is proprietary and confidential to HCL.
-->

# HCL.CS Admin (Next.js)

This project is a Next.js admin client for HCL.CS, with login behavior aligned to MVC:

- OIDC Authorization Code + PKCE
- Access/refresh/id token storage in secure session JWT
- Automatic access-token refresh on expiry
- Federated logout URL generation (`/security/endsession` fallback)

Important: HCL.CS validates redirect URLs as HTTPS. Run this app over `https://localhost:3001`.

## 1) Configure env

Copy `.env.example` to `.env.local` and fill values.

Required keys:

- `NEXTAUTH_URL`
- `NEXTAUTH_SECRET`
- `HCL_CS_ISSUER`
- `HCL_CS_CLIENT_ID`
- `HCL_CS_CLIENT_SECRET`
- `HCL_CS_SCOPES`
- `HCL_CS_METADATA_ADDRESS`
- `HCL_CS_TOKEN_ENDPOINT`
- `HCL_CS_REVOCATION_ENDPOINT`
- `HCL_CS_POST_LOGOUT_REDIRECT_URI`

## 2) Installer URIs for this client

Use these in HCL.CS installer/client config:

- Redirect URI: `https://localhost:3001/api/auth/callback/hcl-cs`
- Post Logout Redirect URI: `https://localhost:3001/login`

If you run on a different host/port, update both values accordingly.

## Federated Logout Toggle

Set `HCL_CS_ENABLE_FEDERATED_LOGOUT=true` to clear IdP session via HCL.CS `/security/endsession`.
If false, admin app performs local logout only and redirects to `/login`.

## 3) Run

```bash
cd HCL.CS-admin
npm install
npm run setup:https:windows
npm run dev:https
```

Open `https://localhost:3001`.

## 4) Local certificate trust

Trust the HCL.CS development CA in the operating-system trust store, or provide
that CA to Node through `NODE_EXTRA_CA_CERTS`. TLS validation is not disabled by
the Admin authentication implementation.
