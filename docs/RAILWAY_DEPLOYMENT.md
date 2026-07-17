# Railway Deployment

This guide deploys Zentra to a single Railway project with these services:

- `postgres`: Railway PostgreSQL
- `redis`: Railway Redis
- `zentra-server`: the identity server
- `zentra-installer`: the first-time setup UI
- `zentra-admin`: the Next.js admin app

Important:

- Railway terminates TLS at the edge. The containers should listen on internal HTTP, but the public URLs are still `https://`.
- `zentra-server` already runs the PostgreSQL bootstrap SQL and every `scripts/migrations/*_postgresql.sql` file during startup.
- Use the installer only for the first baseline setup and seed. After that, redeploying `zentra-server` is enough for later SQL migrations.
- Token signing certificates are not the same as HTTPS certificates. You must keep the signing certificates stable across restarts.
- Railway reference variables use the syntax `${{service-name.VARIABLE_NAME}}`. The examples below assume your service names are exactly `postgres`, `redis`, `zentra-server`, `zentra-installer`, and `zentra-admin`.
- Zentra now uses `AutoMapper 16.1.1`. If you have an AutoMapper license key, set `ZENTRA_AUTOMAPPER_LICENSE_KEY` on the server and installer. If you leave it unset, deployment still works but AutoMapper will log a license warning at startup.

## 1. Before You Start

Create these secrets locally:

```bash
export ZENTRA_SIGNING_CERT_PASSWORD="$(openssl rand -base64 48 | tr -d '\n')"
export NEXTAUTH_SECRET="$(openssl rand -base64 32 | tr -d '\n')"
```

Generate persistent token-signing certificates for Zentra:

```bash
openssl req -x509 -nodes -newkey rsa:4096 \
  -keyout zentra-rsa.key \
  -out zentra-rsa.crt \
  -days 825 \
  -subj "/CN=Zentra Token RSA"

openssl pkcs12 -export \
  -out zentra-rsa.pfx \
  -inkey zentra-rsa.key \
  -in zentra-rsa.crt \
  -password "pass:${ZENTRA_SIGNING_CERT_PASSWORD}"

openssl ecparam -genkey -name prime256v1 -noout -out zentra-ecdsa.key

openssl req -new -x509 \
  -key zentra-ecdsa.key \
  -out zentra-ecdsa.crt \
  -days 825 \
  -subj "/CN=Zentra Token ECDSA"

openssl pkcs12 -export \
  -out zentra-ecdsa.pfx \
  -inkey zentra-ecdsa.key \
  -in zentra-ecdsa.crt \
  -password "pass:${ZENTRA_SIGNING_CERT_PASSWORD}"
```

Convert both `.pfx` files to one-line Base64 strings for Railway secrets:

```bash
base64 < zentra-rsa.pfx | tr -d '\n'
base64 < zentra-ecdsa.pfx | tr -d '\n'
```

## 2. Create The Railway Services

Create one Railway project, then add:

- PostgreSQL service
- Redis service
- `zentra-server` service from this repo
- `zentra-installer` service from this repo
- `zentra-admin` service from this repo

Recommended service settings:

| Service | Root Directory | Dockerfile | Public Domain | Health Check |
| --- | --- | --- | --- | --- |
| `zentra-server` | `.` | `docker/identity-api.Dockerfile` | Yes | `/health/ready` |
| `zentra-installer` | `.` | `docker/installer.Dockerfile` | Yes | `/health` |
| `zentra-admin` | `Zentra-admin` | `Dockerfile` | Yes | `/api/health` |

Recommended volumes:

- Add a volume to `zentra-server` mounted at `/data`
- Add a volume to `zentra-installer` mounted at `/data`

The server volume keeps ASP.NET Core data-protection keys stable. The installer volume keeps its installation lock file stable after the first successful setup.

For `zentra-server` and `zentra-installer`, set `RAILWAY_DOCKERFILE_PATH` to the Dockerfile path shown above. Railway uses that configuration variable to select a non-root Dockerfile.

## 3. Configure `zentra-server`

Set these variables on the `zentra-server` service.

Required:

| Variable | Value |
| --- | --- |
| `ASPNETCORE_ENVIRONMENT` | `Production` |
| `RAILWAY_DOCKERFILE_PATH` | `docker/identity-api.Dockerfile` |
| `SystemSettings__DBConfig__Database` | `PostgreSQL` |
| `ZENTRA_DB_CONNECTION_STRING` | `Host=${{postgres.PGHOST}};Port=${{postgres.PGPORT}};Database=${{postgres.PGDATABASE}};Username=${{postgres.PGUSER}};Password=${{postgres.PGPASSWORD}};SSL Mode=Require;` |
| `ZENTRA_REDIS_CONNECTION_STRING` | `${{redis.REDIS_URL}}` |
| `ZENTRA_REDIS_INSTANCE_NAME` | `zentra:` |
| `TokenSettings__TokenConfig__IssuerUri` | `https://${{RAILWAY_PUBLIC_DOMAIN}}` |
| `Security__Cors__AllowedOrigins__0` | `https://${{zentra-admin.RAILWAY_PUBLIC_DOMAIN}}` |
| `Security__Cors__AllowedOrigins__1` | `https://${{RAILWAY_PUBLIC_DOMAIN}}` |
| `ZENTRA_TRUST_PROXY_HEADERS` | `true` |
| `ZENTRA_DATA_PROTECTION_KEYS_PATH` | `/data/keys` |
| `ZENTRA_SIGNING_CERT_PASSWORD` | the secret you generated above |
| `ZENTRA_RSA_SIGNING_CERT_BASE64` | Base64 of `zentra-rsa.pfx` |
| `ZENTRA_ECDSA_SIGNING_CERT_BASE64` | Base64 of `zentra-ecdsa.pfx` |

Recommended:

| Variable | Value |
| --- | --- |
| `ZENTRA_RSA_SIGNING_KID` | `zentra-rsa-current` |
| `ZENTRA_ECDSA_SIGNING_KID` | `zentra-ecdsa-current` |

Optional, if you have an AutoMapper license key:

| Variable | Value |
| --- | --- |
| `ZENTRA_AUTOMAPPER_LICENSE_KEY` | your AutoMapper license key |

Optional, only if you enable Google login in Zentra:

| Variable | Value |
| --- | --- |
| `Authentication__Google__Enabled` | `true` |
| `Authentication__Google__ClientId` | your Google OAuth client id |
| `Authentication__Google__ClientSecret` | your Google OAuth client secret |
| `Authentication__Google__AllowedRedirectHosts__0` | admin hostname only, for example `zentra-admin.up.railway.app` |

Notes:

- `TokenSettings__TokenConfig__IssuerUri` must exactly match the public URL users and clients use for Zentra.
- Do not use `${{postgres.DATABASE_URL}}` for `ZENTRA_DB_CONNECTION_STRING`. Zentra startup bootstrap expects the Npgsql keyword format shown above.
- Use a custom domain for production if possible. If you switch domains later, update the issuer and any seeded client URLs together.
- If SMTP or SMS is required, also set `ZENTRA_SMTP_*` and `ZENTRA_SMS_*` variables from `demos/Zentra.Demo.Server/Configurations/SystemSettings.json`.

## 4. Configure `zentra-installer`

Set these variables on the `zentra-installer` service.

| Variable | Value |
| --- | --- |
| `ASPNETCORE_ENVIRONMENT` | `Production` |
| `RAILWAY_DOCKERFILE_PATH` | `docker/installer.Dockerfile` |
| `ZENTRA_TRUST_PROXY_HEADERS` | `true` |
| `InstallerLock__MarkerFilePath` | `/data/installer.lock.json` |
| `ZENTRA_INSTALLER_DATA_PROTECTION_KEYS_PATH` | `/data/keys` |

Optional:

| Variable | Value |
| --- | --- |
| `ZENTRA_AUTOMAPPER_LICENSE_KEY` | your AutoMapper license key |

Notes:

- The installer does not need direct DB variables ahead of time. You provide the connection string in the installer UI.
- Keep the installer public only while you need first-time setup. After installation, remove its public domain or restrict access.

## 5. Configure `zentra-admin`

Set these variables on the `zentra-admin` service.

Required:

| Variable | Value |
| --- | --- |
| `NEXTAUTH_URL` | `https://${{RAILWAY_PUBLIC_DOMAIN}}` |
| `NEXTAUTH_SECRET` | the secret you generated above |
| `ZENTRA_ISSUER` | `https://${{zentra-server.RAILWAY_PUBLIC_DOMAIN}}` |
| `ZENTRA_API_BASE_URL` | `https://${{zentra-server.RAILWAY_PUBLIC_DOMAIN}}` |
| `ZENTRA_DEMO_SERVER_BASE_URL` | `https://${{zentra-server.RAILWAY_PUBLIC_DOMAIN}}` |
| `ZENTRA_INSTALLER_BASE_URL` | `https://${{zentra-installer.RAILWAY_PUBLIC_DOMAIN}}` |
| `ZENTRA_POST_LOGOUT_REDIRECT_URI` | `https://${{RAILWAY_PUBLIC_DOMAIN}}/login` |
| `ZENTRA_CLIENT_ID` | client id created in the installer seed step |
| `ZENTRA_CLIENT_SECRET` | client secret created in the installer seed step |

Recommended:

| Variable | Value |
| --- | --- |
| `ZENTRA_SCOPES` | `openid profile email offline_access phone zentra.apiresource zentra.client zentra.user zentra.role zentra.identityresource zentra.adminuser zentra.securitytoken` |
| `ZENTRA_ENABLE_FEDERATED_LOGOUT` | `false` for the first deployment |
| `ZENTRA_ALLOW_INSECURE_TLS` | `false` |
| `NEXT_PUBLIC_GOOGLE_LOGIN_ENABLED` | `false` unless you explicitly configure Google flow support |

Optional:

| Variable | Value |
| --- | --- |
| `ZENTRA_METADATA_ADDRESS` | `https://${{zentra-server.RAILWAY_PUBLIC_DOMAIN}}/.well-known/openid-configuration` |
| `ZENTRA_TOKEN_ENDPOINT` | `https://${{zentra-server.RAILWAY_PUBLIC_DOMAIN}}/security/token` |
| `ZENTRA_REVOCATION_ENDPOINT` | `https://${{zentra-server.RAILWAY_PUBLIC_DOMAIN}}/security/revocation` |

Notes:

- `zentra-admin` no longer needs Railway build arguments. Set these values as runtime service variables only.

## 6. First-Time Deployment Order

1. Deploy `postgres` and `redis`.
2. Deploy `zentra-server` with its production variables.
3. Deploy `zentra-installer`.
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
11. Set those two values on `zentra-admin`.
12. Deploy `zentra-admin`.

If the installer complains about PostgreSQL TLS validation, append this once:

```text
Trust Server Certificate=true;
```

## 7. Recommended Installer Seed Values For `zentra-admin`

Use these values in the installer seed screen.

Client:

- Client name: `Zentra Admin`
- Client URI: `https://<your-zentra-admin-domain>`
- Grant types: enable `Authorization Code`, `Refresh Token`, and `Resource Owner Password`
- Response types: keep `code`
- Use default scopes: `true`
- Redirect URI: `https://<your-zentra-admin-domain>/api/auth/callback/zentra`
- Post logout redirect URI: `https://<your-zentra-admin-domain>/login`

Admin user:

- Create your first Zentra administrator account here

Notes:

- The current `Zentra-admin` app signs users in with password grant and refresh token. That is why `Resource Owner Password` and `Refresh Token` must be enabled.
- If you later enable Google login inside `Zentra-admin`, you will also need a client that supports the `user_code` grant. The installer seed UI does not create that grant today, so keep `NEXT_PUBLIC_GOOGLE_LOGIN_ENABLED=false` until you add it separately.
- Seeded client secrets currently expire after 100 days. Plan to rotate or replace that client secret before it expires.
- If you switch from Railway public domains to custom domains, update the service variables above to the custom URLs before you re-seed or ask users to sign in.

## 8. Verification

After deployment, verify these URLs:

- `https://<your-zentra-server-domain>/.well-known/openid-configuration`
- `https://<your-zentra-server-domain>/health/ready`
- `https://<your-zentra-installer-domain>/health`
- `https://<your-zentra-admin-domain>/api/health`

Expected outcome:

- discovery returns the public issuer URL
- installer validates PostgreSQL and completes seed once
- admin app loads and can sign in with the seeded client and admin user

## 9. After The First Install

After the first successful installer run:

- keep `zentra-server` deployed
- keep `postgres` and `redis` deployed
- keep `zentra-admin` deployed
- remove the installer public domain or restrict access

For later code or schema changes:

- redeploy `zentra-server`
- its startup entrypoint will apply the PostgreSQL bootstrap script and all available PostgreSQL migration scripts again
- do not rerun seed unless you intentionally want a brand-new empty database
