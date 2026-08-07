<!--
Copyright (c) 2021 HCL CORPORATION.
All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
HCL is obtained. This is proprietary and confidential to HCL.
-->

# HCL.CS Admin login and session architecture

HCL.CS Admin uses NextAuth 4 with the JWT session strategy and a custom HCL.CS
OIDC provider (`id: hcl-cs`, `type: oauth`). It discovers HCL.CS endpoints from
`HCL_CS_METADATA_ADDRESS`, requests Authorization Code with the configured scopes,
and enables NextAuth checks for PKCE and state.

The browser flow is:

1. `/login` displays **Sign in with HCL.CS**.
2. `signIn("hcl-cs")` redirects to the discovered HCL.CS authorization endpoint.
3. HCL.CS hosts credential and upstream-provider authentication.
4. HCL.CS returns a code to `/api/auth/callback/hcl-cs`.
5. NextAuth validates state and the PKCE verifier, exchanges the code with
   server-side confidential client authentication, validates the ID token, and
   creates the encrypted session JWT.

The JWT retains access, refresh, and ID tokens, access-token expiry, scopes,
roles, and `isAdmin`. Sensitive tokens are attached non-enumerably to sessions
returned to server code, so they are usable by server actions but absent from the
browser-visible `/api/auth/session` response.

Access tokens refresh within a 60-second expiry buffer through
`grant_type=refresh_token`. Refresh is single-flight per refresh-token fingerprint,
supports refresh-token rotation, and hard-invalidates the session on failure.

`proxy.ts` and the Admin layout independently enforce the administrator role.
Logout always clears the NextAuth cookie; it also attempts HCL.CS sign-out and
revocation and can use the discovered end-session endpoint with `id_token_hint`.

The former Admin CredentialsProvider, Resource Owner Password login, direct
Demo Server Google button, and `user_code` exchange were removed. They were one
combined alternate login path and required grant types not present on the intended
Admin client. Google authentication is not removed from HCL.CS itself: when
configured, it belongs on the HCL.CS hosted authorization/login experience.
Forgot-password and reset-password pages were retained as separate account-recovery
features; they do not authenticate an Admin session.
