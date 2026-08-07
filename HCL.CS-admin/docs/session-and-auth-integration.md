<!--
Copyright (c) 2021 HCL CORPORATION.
All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
HCL is obtained. This is proprietary and confidential to HCL.
-->

# HCL.CS Admin authentication and session integration

## Login

The Admin application is an OAuth 2.0 confidential web client using OpenID
Connect Authorization Code flow. The login page calls `signIn("hcl-cs")` and
redirects the browser to the `authorization_endpoint` published by
`HCL_CS_METADATA_ADDRESS`. HCL.CS owns credential and external-provider
collection. NextAuth generates and validates PKCE (`S256`) and state and handles
the callback at `/api/auth/callback/hcl-cs`.

The provider requests `response_type=code` plus the configured scopes. The
default scope set includes `openid` and `offline_access`. NextAuth exchanges the
code at the discovered token endpoint using `client_secret_basic`, which HCL.CS
advertises and supports for its confidential Admin client.

The previous Admin-specific CredentialsProvider, Resource Owner Password grant,
and `user_code`/direct Google bridge were removed. Google or another upstream
identity provider may still be offered by the HCL.CS hosted login experience;
the Admin app no longer exchanges a Demo Server `UserCode`. Forgot/reset-password
pages remain separate account-recovery features and do not establish an Admin
session.

## Session and authorization

NextAuth uses the encrypted JWT cookie strategy. The JWT callback copies
`access_token`, `refresh_token`, `id_token`, expiry, scopes, roles, and derived
`isAdmin` from the initial HCL.CS OAuth account. Roles are decoded from the HCL.CS
access token.

Bearer, refresh, and ID tokens are non-enumerable on the server session object.
Server actions can use them for HCL.CS API calls and logout, but they are omitted
from `/api/auth/session` and the client-side SessionProvider payload. Browser
components receive only user metadata, expiry, scopes, roles, `isAdmin`, and any
session error.

Both `proxy.ts` and `app/admin/layout.tsx` enforce the administrator role. A valid
login without an admin role remains blocked from `/admin/*`.

## Refresh and invalidation

The JWT callback reuses an access token until it is within the 60-second refresh
buffer. It then sends `grant_type=refresh_token` to the discovered/configured
token endpoint using server-side Basic client authentication. Refresh requests
for the same refresh-token fingerprint are single-flight. A rotated refresh token
replaces the previous token; if HCL.CS does not rotate it, the existing refresh
token remains in use.

Any missing refresh token, failed refresh response, or runtime failure clears all
bearer tokens and roles and marks the JWT with a hard session error. The session
callback then returns no usable session, so stale credentials cannot continue.

## Logout

Logout first obtains an optional federated logout URL while the ID token is still
available, attempts the HCL.CS sign-out API and token revocation, and always calls
NextAuth `signOut()` to clear the local cookie. If
`HCL_CS_ENABLE_FEDERATED_LOGOUT=true`, the browser is sent to the discovered
`end_session_endpoint` with `client_id`, `id_token_hint` when available, and
`HCL_CS_POST_LOGOUT_REDIRECT_URI`. Otherwise it navigates locally to `/login`.
