# SBOM Analyzer identity contract v1

HCL.CS authenticates users and issues identity tokens. SBOM Analyzer remains the
authority for local provisioning, verification, tenant membership, roles, and
permissions.

## OIDC registration

- Client: `sbom-analyser-web`
- Public client: no client secret
- Grants: `authorization_code refresh_token`
- PKCE: required, `S256` only
- Redirect URI: `https://localhost:3000/auth/callback`
- Post-logout URI: `https://localhost:3000`
- Scopes: `openid profile email offline_access sbom-analyser-api`
- API audience: `sbom-analyser-api`
- Signing algorithm: `RS256`
- Authorization code: 300 seconds
- Access token: 3600 seconds
- Refresh token: 86400 seconds

The access token identifies the employee through `sub`, `email`, `name`,
`preferred_username`, `employee_id` when required by LDAP policy, and optional
`department`. `sub` remains the HCL.CS `Users.Id`; the LDAP immutable ID is stored
separately and never replaces it. `role` and `tenant_id`, when present for legacy
HCL.CS consumers, are not SBOM authorization.

LDAP account status is validated internally and is not emitted as a token claim.

## Signing certificates

Production and other non-development environments must configure both persistent
signing certificates:

```text
HCL_CS_SIGNING_CERT_PASSWORD
HCL_CS_RSA_SIGNING_CERT_PATH or HCL_CS_RSA_SIGNING_CERT_BASE64
HCL_CS_ECDSA_SIGNING_CERT_PATH or HCL_CS_ECDSA_SIGNING_CERT_BASE64
HCL_CS_RSA_SIGNING_KID
HCL_CS_ECDSA_SIGNING_KID
```

The host fails startup if either configured certificate is missing, expired, or
does not contain the required private key. Development may opt into temporary
self-signed keys with:

```text
ASPNETCORE_ENVIRONMENT=Development
HCL_CS_ALLOW_EPHEMERAL_SIGNING_KEYS=true
```

Temporary keys invalidate outstanding tokens on restart and must not be used in
production. The current keystore supports one active key per signing algorithm;
therefore overlapping-key rotation is not yet implemented. The safe extension
design is to split the store into one active private signing key per algorithm
and a collection of verification-only public certificates indexed by `kid`.
JWKS would publish both collections, token generation would use only the active
entry, and a previous verification key could be removed only after the maximum
outstanding token lifetime. Until that extension is implemented, operators must
not rotate a certificate in place while tokens signed by it remain valid.

## LDAP refresh revalidation

Refresh requests for LDAP-backed users locate the directory account using
`DirectoryImmutableId` and the service bind. A password is not required. Refresh
fails closed for disabled, locked, expired, missing, ambiguous, malformed, or
unavailable directory state. Local and Google-backed users do not use LDAP
revalidation.

Security audit persistence is best effort: an audit-store failure raises a
high-severity operational log but does not turn an otherwise valid authentication
or token request into an outage.
