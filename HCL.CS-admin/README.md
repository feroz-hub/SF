# HCL.CS Admin (Next.js)

This project is a Next.js admin client for HCL.CS, with login behavior aligned to MVC:

- OIDC Authorization Code + PKCE
- Access/refresh/id token storage in secure session JWT
- Automatic access-token refresh on expiry
- Federated logout URL generation (`/security/endsession` fallback)

Important: HCL.CS validates redirect URLs as HTTPS. Run this app over `https://localhost:3000`.

## 1) Configure env

Copy `.env.example` to `.env.local` and fill values.

Required keys:
- `NEXTAUTH_URL`
- `NEXTAUTH_SECRET`
- `HCL_CS_AUTHORITY`
- `HCL_CS_CLIENT_ID`
- `HCL_CS_CLIENT_SECRET`

## 2) Installer URIs for this client

Use these in HCL.CS installer/client config:

- Redirect URI: `https://localhost:3000/api/auth/callback/hcl-cs`
- Post Logout Redirect URI: `https://localhost:3000/login`

If you run on a different host/port, update both values accordingly.

## Federated Logout Toggle

Set `HCL_CS_ENABLE_FEDERATED_LOGOUT=true` to clear IdP session via HCL.CS `/security/endsession`.
If false, admin app performs local logout only and redirects to `/login`.

## 3) Run

```bash
cd HCL.CS-admin
npm install
npm run dev:https
```

Open `https://localhost:3000`.

## 4) If you see self-signed certificate error

Preferred (safer) option:

```bash
NODE_OPTIONS=--use-system-ca npm run dev:https
```

If that still fails in local dev, set in `.env.local`:

```bash
HCL_CS_ALLOW_INSECURE_TLS=true
```

This disables TLS certificate validation for this app process in non-production only.
