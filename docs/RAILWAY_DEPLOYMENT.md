# Railway Deployment

This guide deploys HCL.CS to a single Railway project with these services:

- `postgres`: Railway PostgreSQL
- `redis`: Railway Redis
- `hcl-cs-server`: the identity server
- `hcl-cs-installer`: the first-time setup UI
- `hcl-cs-admin`: the Next.js admin app

Important:

- Railway terminates TLS at the edge. The containers should listen on internal HTTP, but the public URLs are still `https://`.
- `hcl-cs-server` already runs the PostgreSQL bootstrap SQL and every `scripts/migrations/*_postgresql.sql` file during startup.
- Use the installer only for the first baseline setup and seed. After that, redeploying `hcl-cs-server` is enough for later SQL migrations.
- Token signing certificates are not the same as HTTPS certificates. You must keep the signing certificates stable across restarts.
- Railway reference variables use the syntax `${{service-name.VARIABLE_NAME}}`. The examples below assume your service names are exactly `postgres`, `redis`, `hcl-cs-server`, `hcl-cs-installer`, and `hcl-cs-admin`.
- HCL.CS now uses `AutoMapper 16.1.1`. If you have an AutoMapper license key, set `HCL_CS_AUTOMAPPER_LICENSE_KEY` on the server and installer. If you leave it unset, deployment still works but AutoMapper will log a license warning at startup.

## 1. Before You Start

Create these secrets locally:

```bash
export HCL_CS_SIGNING_CERT_PASSWORD="$(openssl rand -base64 48 | tr -d '\n')"
export NEXTAUTH_SECRET="$(openssl rand -base64 32 | tr -d '\n')"
```

Generate persistent token-signing certificates for HCL.CS:

```bash
openssl req -x509 -nodes -newkey rsa:4096 \
  -keyout hcl-cs-rsa.key \
  -out hcl-cs-rsa.crt \
  -days 825 \
  -subj "/CN=HCL.CS Token RSA"

openssl pkcs12 -export \
  -out hcl-cs-rsa.pfx \
  -inkey hcl-cs-rsa.key \
  -in hcl-cs-rsa.crt \
  -password "pass:${HCL_CS_SIGNING_CERT_PASSWORD}"

openssl ecparam -genkey -name prime256v1 -noout -out hcl-cs-ecdsa.key

openssl req -new -x509 \
  -key hcl-cs-ecdsa.key \
  -out hcl-cs-ecdsa.crt \
  -days 825 \
  -subj "/CN=HCL.CS Token ECDSA"

openssl pkcs12 -export \
  -out hcl-cs-ecdsa.pfx \
  -inkey hcl-cs-ecdsa.key \
  -in hcl-cs-ecdsa.crt \
  -password "pass:${HCL_CS_SIGNING_CERT_PASSWORD}"
```

Convert both `.pfx` files to one-line Base64 strings for Railway secrets:

```bash
base64 < hcl-cs-rsa.pfx | tr -d '\n'
base64 < hcl-cs-ecdsa.pfx | tr -d '\n'
```

## 2. Create The Railway Services

Create one Railway project, then add:

- PostgreSQL service
- Redis service
- `hcl-cs-server` service from this repo
- `hcl-cs-installer` service from this repo
- `hcl-cs-admin` service from this repo

Recommended service settings:

| Service | Root Directory | Dockerfile | Public Domain | Health Check |
| --- | --- | --- | --- | --- |
| `hcl-cs-server` | `.` | `docker/identity-api.Dockerfile` | Yes | `/health/ready` |
| `hcl-cs-installer` | `.` | `docker/installer.Dockerfile` | Yes | `/health` |
| `hcl-cs-admin` | `HCL.CS-admin` | `Dockerfile` | Yes | `/api/health` |

Recommended volumes:

- Add a volume to `hcl-cs-server` mounted at `/data`
- Add a volume to `hcl-cs-installer` mounted at `/data`

The server volume keeps ASP.NET Core data-protection keys stable. The installer volume keeps its installation lock file stable after the first successful setup.

For `hcl-cs-server` and `hcl-cs-installer`, set `RAILWAY_DOCKERFILE_PATH` to the Dockerfile path shown above. Railway uses that configuration variable to select a non-root Dockerfile.

## 3. Configure `hcl-cs-server`

Set these variables on the `hcl-cs-server` service.

Required:

| Variable | Value |
| --- | --- |
| `ASPNETCORE_ENVIRONMENT` | `Production` |
| `RAILWAY_DOCKERFILE_PATH` | `docker/identity-api.Dockerfile` |
| `SystemSettings__DBConfig__Database` | `PostgreSQL` |
| `HCL_CS_DB_CONNECTION_STRING` | `Host=${{postgres.PGHOST}};Port=${{postgres.PGPORT}};Database=${{postgres.PGDATABASE}};Username=${{postgres.PGUSER}};Password=${{postgres.PGPASSWORD}};SSL Mode=Require;` |
| `HCL_CS_REDIS_CONNECTION_STRING` | `${{redis.REDIS_URL}}` |
| `HCL_CS_REDIS_INSTANCE_NAME` | `hcl-cs:` |
| `TokenSettings__TokenConfig__IssuerUri` | `https://${{RAILWAY_PUBLIC_DOMAIN}}` |
| `Security__Cors__AllowedOrigins__0` | `https://${{hcl-cs-admin.RAILWAY_PUBLIC_DOMAIN}}` |
| `Security__Cors__AllowedOrigins__1` | `https://${{RAILWAY_PUBLIC_DOMAIN}}` |
| `HCL_CS_TRUST_PROXY_HEADERS` | `true` |
| `HCL_CS_DATA_PROTECTION_KEYS_PATH` | `/data/keys` |
| `HCL_CS_SIGNING_CERT_PASSWORD` | the secret you generated above |
| `HCL_CS_RSA_SIGNING_CERT_BASE64` | Base64 of `hcl-cs-rsa.pfx` |
| `HCL_CS_ECDSA_SIGNING_CERT_BASE64` | Base64 of `hcl-cs-ecdsa.pfx` |

Recommended:

| Variable | Value |
| --- | --- |
| `HCL_CS_RSA_SIGNING_KID` | `hcl-cs-rsa-current` |
| `HCL_CS_ECDSA_SIGNING_KID` | `hcl-cs-ecdsa-current` |

Optional, if you have an AutoMapper license key:

| Variable | Value |
| --- | --- |
| `HCL_CS_AUTOMAPPER_LICENSE_KEY` | your AutoMapper license key |

Optional, only if you enable Google login in HCL.CS:

| Variable | Value |
| --- | --- |
| `Authentication__Google__Enabled` | `true` |
| `Authentication__Google__ClientId` | your Google OAuth client id |
| `Authentication__Google__ClientSecret` | your Google OAuth client secret |
| `Authentication__Google__AllowedRedirectHosts__0` | admin hostname only, for example `hcl-cs-admin.up.railway.app` |

Notes:

- `TokenSettings__TokenConfig__IssuerUri` must exactly match the public URL users and clients use for HCL.CS.
- Do not use `${{postgres.DATABASE_URL}}` for `HCL_CS_DB_CONNECTION_STRING`. HCL.CS startup bootstrap expects the Npgsql keyword format shown above.
- Use a custom domain for production if possible. If you switch domains later, update the issuer and any seeded client URLs together.
- If SMTP or SMS is required, also set `HCL_CS_SMTP_*` and `HCL_CS_SMS_*` variables from `demos/HCL.CS.Demo.Server/Configurations/SystemSettings.json`.

## 4. Configure `hcl-cs-installer`

Set these variables on the `hcl-cs-installer` service.

| Variable | Value |
| --- | --- |
| `ASPNETCORE_ENVIRONMENT` | `Production` |
| `RAILWAY_DOCKERFILE_PATH` | `docker/installer.Dockerfile` |
| `HCL_CS_TRUST_PROXY_HEADERS` | `true` |
| `InstallerLock__MarkerFilePath` | `/data/installer.lock.json` |
| `HCL_CS_INSTALLER_DATA_PROTECTION_KEYS_PATH` | `/data/keys` |

Optional:

| Variable | Value |
| --- | --- |
| `HCL_CS_AUTOMAPPER_LICENSE_KEY` | your AutoMapper license key |

Notes:

- The installer does not need direct DB variables ahead of time. You provide the connection string in the installer UI.
- Keep the installer public only while you need first-time setup. After installation, remove its public domain or restrict access.

## 5. Configure `hcl-cs-admin`

Set these variables on the `hcl-cs-admin` service.

Required:

| Variable | Value |
| --- | --- |
| `NEXTAUTH_URL` | `https://${{RAILWAY_PUBLIC_DOMAIN}}` |
| `NEXTAUTH_SECRET` | the secret you generated above |
| `HCL_CS_ISSUER` | `https://${{hcl-cs-server.RAILWAY_PUBLIC_DOMAIN}}` |
| `HCL_CS_API_BASE_URL` | `https://${{hcl-cs-server.RAILWAY_PUBLIC_DOMAIN}}` |
| `HCL_CS_DEMO_SERVER_BASE_URL` | `https://${{hcl-cs-server.RAILWAY_PUBLIC_DOMAIN}}` |
| `HCL_CS_INSTALLER_BASE_URL` | `https://${{hcl-cs-installer.RAILWAY_PUBLIC_DOMAIN}}` |
| `HCL_CS_POST_LOGOUT_REDIRECT_URI` | `https://${{RAILWAY_PUBLIC_DOMAIN}}/login` |
| `HCL_CS_CLIENT_ID` | client id created in the installer seed step |
| `HCL_CS_CLIENT_SECRET` | client secret created in the installer seed step |

Recommended:

| Variable | Value |
| --- | --- |
| `HCL_CS_SCOPES` | `openid profile email offline_access phone hcl-cs.apiresource hcl-cs.client hcl-cs.user hcl-cs.role hcl-cs.identityresource hcl-cs.adminuser hcl-cs.securitytoken` |
| `HCL_CS_ENABLE_FEDERATED_LOGOUT` | `false` for the first deployment |
| `HCL_CS_ALLOW_INSECURE_TLS` | `false` |
| `NEXT_PUBLIC_GOOGLE_LOGIN_ENABLED` | `false` unless you explicitly configure Google flow support |

Optional:

| Variable | Value |
| --- | --- |
| `HCL_CS_METADATA_ADDRESS` | `https://${{hcl-cs-server.RAILWAY_PUBLIC_DOMAIN}}/.well-known/openid-configuration` |
| `HCL_CS_TOKEN_ENDPOINT` | `https://${{hcl-cs-server.RAILWAY_PUBLIC_DOMAIN}}/security/token` |
| `HCL_CS_REVOCATION_ENDPOINT` | `https://${{hcl-cs-server.RAILWAY_PUBLIC_DOMAIN}}/security/revocation` |

Notes:

- `hcl-cs-admin` no longer needs Railway build arguments. Set these values as runtime service variables only.

## 6. First-Time Deployment Order

1. Deploy `postgres` and `redis`.
2. Deploy `hcl-cs-server` with its production variables.
3. Deploy `hcl-cs-installer`.
4. Open the installer public URL.
5. In the installer, choose `PostgreSql`.
6. Use the Railway PostgreSQL internal values in the connection string:

```text
Host=<PGHOST>;Port=<PGPORT>;Database=<PGDATABASE>;Username=<PGUSER>;Password=<PGPASSWORD>;SSL Mode=Require;
```

7. Validate the connection.
8. Run migrations.
9. Seed the baseline data.
10. Copy the generated `Client Id` and `Client Secret`.
11. Set those two values on `hcl-cs-admin`.
12. Deploy `hcl-cs-admin`.

If the installer complains about PostgreSQL TLS validation, append this once:

```text
Trust Server Certificate=true;
```

## 7. Recommended Installer Seed Values For `hcl-cs-admin`

Use these values in the installer seed screen.

Client:

- Client name: `HCL.CS Admin`
- Client URI: `https://<your-hcl-cs-admin-domain>`
- Grant types: enable `Authorization Code`, `Refresh Token`, and `Resource Owner Password`
- Response types: keep `code`
- Use default scopes: `true`
- Redirect URI: `https://<your-hcl-cs-admin-domain>/api/auth/callback/hcl-cs`
- Post logout redirect URI: `https://<your-hcl-cs-admin-domain>/login`

Admin user:

- Create your first HCL.CS administrator account here

Notes:

- The current `HCL.CS-admin` app signs users in with password grant and refresh token. That is why `Resource Owner Password` and `Refresh Token` must be enabled.
- If you later enable Google login inside `HCL.CS-admin`, you will also need a client that supports the `user_code` grant. The installer seed UI does not create that grant today, so keep `NEXT_PUBLIC_GOOGLE_LOGIN_ENABLED=false` until you add it separately.
- Seeded client secrets currently expire after 100 days. Plan to rotate or replace that client secret before it expires.
- If you switch from Railway public domains to custom domains, update the service variables above to the custom URLs before you re-seed or ask users to sign in.

## 8. Verification

After deployment, verify these URLs:

- `https://<your-hcl-cs-server-domain>/.well-known/openid-configuration`
- `https://<your-hcl-cs-server-domain>/health/ready`
- `https://<your-hcl-cs-installer-domain>/health`
- `https://<your-hcl-cs-admin-domain>/api/health`

Expected outcome:

- discovery returns the public issuer URL
- installer validates PostgreSQL and completes seed once
- admin app loads and can sign in with the seeded client and admin user

## 9. After The First Install

After the first successful installer run:

- keep `hcl-cs-server` deployed
- keep `postgres` and `redis` deployed
- keep `hcl-cs-admin` deployed
- remove the installer public domain or restrict access

For later code or schema changes:

- redeploy `hcl-cs-server`
- its startup entrypoint will apply the PostgreSQL bootstrap script and all available PostgreSQL migration scripts again
- do not rerun seed unless you intentionally want a brand-new empty database
