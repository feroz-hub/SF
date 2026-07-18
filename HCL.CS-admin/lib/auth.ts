/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

import { createHash } from "crypto";

import { getServerSession, type NextAuthOptions } from "next-auth";
import { type JWT } from "next-auth/jwt";
import CredentialsProvider from "next-auth/providers/credentials";
import { cache } from "react";

import { SESSION_ERROR_INVALIDATED, SESSION_ERROR_REFRESH_FAILED, isHardSessionError } from "@/lib/auth-errors";
import { assertAuthEnv, env } from "@/lib/env";
import { resolveTokenEndpoint } from "@/lib/oidc";
import { hclCsFetch } from "@/lib/server-fetch";

function encodeBasicAuth(clientId: string, clientSecret: string): string {
  const escapedClientId = encodeURIComponent(clientId);
  const escapedClientSecret = encodeURIComponent(clientSecret);
  return Buffer.from(`${escapedClientId}:${escapedClientSecret}`).toString("base64");
}

function decodeJwtPayload(token?: string): Record<string, unknown> {
  if (!token) {
    return {};
  }

  const sections = token.split(".");
  if (sections.length < 2) {
    return {};
  }

  try {
    const payload = sections[1].replace(/-/g, "+").replace(/_/g, "/");
    const json = Buffer.from(payload, "base64").toString("utf8");
    return JSON.parse(json) as Record<string, unknown>;
  } catch {
    return {};
  }
}

function coerceRoleValues(value: unknown): string[] {
  if (Array.isArray(value)) {
    return value.map((entry) => String(entry)).filter(Boolean);
  }

  if (typeof value === "string") {
    return value
      .split(/[\s,]+/)
      .map((entry) => entry.trim())
      .filter(Boolean);
  }

  return [];
}

export function extractRolesFromToken(accessToken?: string): string[] {
  const payload = decodeJwtPayload(accessToken);

  const roleCandidates = [
    payload.role,
    payload.roles,
    payload.userrole,
    payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"]
  ];

  const roles = roleCandidates.flatMap(coerceRoleValues);
  return [...new Set(roles.map((role) => role.trim()).filter(Boolean))];
}

export function hasAdminRole(roles: string[]): boolean {
  return roles.some((role) => role.toLowerCase().includes("admin"));
}

type TokenEndpointResponse = {
  access_token?: string;
  refresh_token?: string;
  id_token?: string;
  expires_in?: number;
  scope?: string;
  error?: string;
  error_description?: string;
};

type AuthenticatedUser = {
  id: string;
  name: string;
  email: string | null;
  accessToken: string;
  refreshToken?: string;
  idToken?: string;
  accessTokenExpires: number;
  scopes?: string;
  roles: string[];
  isAdmin: boolean;
};

type PasswordGrantAttempt = {
  includeScope: boolean;
  useClientSecretPost: boolean;
};

type PasswordGrantResult =
  | {
      ok: true;
      token: TokenEndpointResponse;
    }
  | {
      ok: false;
      errorCode?: string;
      errorDescription?: string;
    };

type SafeTokenMetadata = {
  sub?: string;
  exp?: number;
  expHuman?: string;
  aud?: unknown;
  iss?: unknown;
};

const ACCESS_TOKEN_REFRESH_BUFFER_MS = 60_000;
// Process-local single-flight keyed by refresh-token fingerprint.
const refreshFlights = new Map<string, Promise<JWT>>();

function createTokenFingerprint(value?: string): string | undefined {
  if (!value) {
    return undefined;
  }

  return createHash("sha256").update(value).digest("hex").slice(0, 12);
}

function readStringClaim(value: unknown): string | undefined {
  if (typeof value !== "string") {
    return undefined;
  }

  const trimmed = value.trim();
  return trimmed ? trimmed : undefined;
}

function resolveUserIdentity(userName: string, idToken?: string, accessToken?: string) {
  const idPayload = decodeJwtPayload(idToken);
  const accessPayload = decodeJwtPayload(accessToken);

  const id =
    readStringClaim(idPayload.sub) ??
    readStringClaim(accessPayload.sub) ??
    readStringClaim(accessPayload.user_id) ??
    userName;

  const name =
    readStringClaim(idPayload.name) ??
    readStringClaim(idPayload.preferred_username) ??
    readStringClaim(accessPayload.name) ??
    readStringClaim(accessPayload.preferred_username) ??
    readStringClaim(accessPayload.username) ??
    userName;

  const email = readStringClaim(idPayload.email) ?? readStringClaim(accessPayload.email) ?? null;

  return { id, name, email };
}

function readNumericClaim(value: unknown): number | undefined {
  if (typeof value === "number" && Number.isFinite(value)) {
    return value;
  }

  if (typeof value === "string") {
    const parsed = Number(value);
    if (Number.isFinite(parsed)) {
      return parsed;
    }
  }

  return undefined;
}

function extractSafeTokenMetadata(accessToken?: string, fallbackSub?: string): SafeTokenMetadata {
  const payload = decodeJwtPayload(accessToken);
  const exp = readNumericClaim(payload.exp);

  return {
    sub: readStringClaim(payload.sub) ?? fallbackSub,
    exp,
    expHuman: exp ? new Date(exp * 1000).toISOString() : undefined,
    aud: payload.aud,
    iss: payload.iss
  };
}

function describeJwtToken(token: JWT): SafeTokenMetadata {
  return extractSafeTokenMetadata(token.accessToken, readStringClaim(token.sub));
}

function logRefreshEvent(
  level: "info" | "warn",
  event: string,
  token: Pick<JWT, "accessToken" | "refreshToken" | "sub">,
  extra: Record<string, unknown> = {}
): void {
  const metadata = describeJwtToken(token as JWT);
  const logger = level === "warn" ? console.warn : console.info;
  logger(`[hcl-cs-auth] ${event}.`, {
    sub: metadata.sub ?? "unknown",
    exp_human: metadata.expHuman ?? "unknown",
    aud: metadata.aud,
    iss: metadata.iss,
    refresh_key: createTokenFingerprint(token.refreshToken),
    ...extra
  });
}

function buildSessionInvalidatedToken(token: JWT, errorCode: string): JWT {
  return {
    ...token,
    name: undefined,
    email: undefined,
    picture: undefined,
    sub: undefined,
    accessToken: undefined,
    refreshToken: undefined,
    idToken: undefined,
    accessTokenExpires: undefined,
    scopes: undefined,
    roles: [],
    isAdmin: false,
    error: errorCode
  };
}

function buildRefreshedToken(token: JWT, refreshed: TokenEndpointResponse): JWT {
  if (!refreshed.access_token || !refreshed.refresh_token) {
    return buildSessionInvalidatedToken(token, SESSION_ERROR_REFRESH_FAILED);
  }

  const expiresInSeconds = Number(refreshed.expires_in ?? 300);
  const roles = extractRolesFromToken(refreshed.access_token);

  return {
    ...token,
    accessToken: refreshed.access_token,
    refreshToken: refreshed.refresh_token,
    idToken: refreshed.id_token ?? token.idToken,
    accessTokenExpires: Date.now() + expiresInSeconds * 1000,
    scopes: refreshed.scope ?? token.scopes,
    roles,
    isAdmin: hasAdminRole(roles),
    error: undefined
  };
}

function shouldUseCurrentAccessToken(token: JWT): boolean {
  if (!token.accessToken || !token.accessTokenExpires) {
    return false;
  }

  return Date.now() < token.accessTokenExpires - ACCESS_TOKEN_REFRESH_BUFFER_MS;
}

function shouldInvalidateSession(errorCode?: string): boolean {
  return isHardSessionError(errorCode);
}

function createAuthenticatedUser(userName: string, tokenResponse: TokenEndpointResponse): AuthenticatedUser {
  const accessToken = tokenResponse.access_token ?? "";
  const expiresInSeconds = Number(tokenResponse.expires_in ?? 300);
  const roles = extractRolesFromToken(accessToken);
  const identity = resolveUserIdentity(userName, tokenResponse.id_token, accessToken);

  return {
    id: identity.id,
    name: identity.name,
    email: identity.email,
    accessToken,
    refreshToken: tokenResponse.refresh_token,
    idToken: tokenResponse.id_token,
    accessTokenExpires: Date.now() + expiresInSeconds * 1000,
    scopes: tokenResponse.scope ?? env.scopes,
    roles,
    isAdmin: hasAdminRole(roles)
  };
}

function buildLoginErrorMessage(errorCode?: string, errorDescription?: string): string {
  const description = typeof errorDescription === "string" ? errorDescription.trim() : "";
  if (description) {
    if (description.includes("Two Factor authentication should be disabled for resource owner password flow")) {
      return "Two-factor is enabled for this user. Password login in Admin UI is not supported for 2FA users.";
    }

    return description;
  }

  switch (errorCode) {
    case "invalid_client":
      return "Client authentication failed. Verify admin client id and secret.";
    case "unauthorized_client":
      return "This client is not allowed to use password grant.";
    case "invalid_scope":
      return "Requested scope is not allowed for this client.";
    case "invalid_grant":
      return "Invalid username or password.";
    default:
      return "Login failed.";
  }
}

async function executePasswordGrantAttempt(
  tokenEndpoint: string,
  userName: string,
  password: string,
  attempt: PasswordGrantAttempt
): Promise<PasswordGrantResult> {
  const payload = new URLSearchParams({
    grant_type: "password",
    username: userName,
    password
  });

  if (attempt.includeScope && env.scopes.trim()) {
    payload.set("scope", env.scopes);
  }

  const headers: Record<string, string> = {
    "Content-Type": "application/x-www-form-urlencoded"
  };

  if (attempt.useClientSecretPost) {
    payload.set("client_id", env.clientId);
    payload.set("client_secret", env.clientSecret);
  } else {
    headers.Authorization = `Basic ${encodeBasicAuth(env.clientId, env.clientSecret)}`;
  }

  const response = await hclCsFetch(tokenEndpoint, {
    method: "POST",
    headers,
    body: payload.toString(),
    cache: "no-store"
  });

  let tokenResponse: TokenEndpointResponse = {};
  try {
    tokenResponse = (await response.json()) as TokenEndpointResponse;
  } catch {
    tokenResponse = {};
  }

  if (response.ok && tokenResponse.access_token) {
    return { ok: true, token: tokenResponse };
  }

  return {
    ok: false,
    errorCode: tokenResponse.error,
    errorDescription: tokenResponse.error_description
  };
}

async function requestPasswordGrantTokens(userName: string, password: string): Promise<TokenEndpointResponse> {
  const tokenEndpoint = await resolveTokenEndpoint();

  // Try with scopes first (both client auth methods), then without scopes ONLY if the error
  // was scope-related. Never silently fall back to scopeless tokens for credential errors.
  const scopedAttempts: PasswordGrantAttempt[] = [
    { includeScope: true, useClientSecretPost: true },
    { includeScope: true, useClientSecretPost: false }
  ];

  const scopelessAttempts: PasswordGrantAttempt[] = [
    { includeScope: false, useClientSecretPost: true },
    { includeScope: false, useClientSecretPost: false }
  ];

  let lastErrorCode: string | undefined;
  let lastErrorDescription: string | undefined;
  let shouldTryScopeless = false;

  // Phase 1: Try with scopes (preferred — token will include required audience/scopes)
  for (const attempt of scopedAttempts) {
    const result = await executePasswordGrantAttempt(tokenEndpoint, userName, password, attempt);
    if (result.ok) {
      console.info(`[hcl-cs-auth] Password grant succeeded (scopes=included, clientSecretPost=${attempt.useClientSecretPost}).`);
      return result.token;
    }

    lastErrorCode = result.errorCode;
    lastErrorDescription = result.errorDescription;
    console.warn(`[hcl-cs-auth] Password grant attempt failed (scopes=included, clientSecretPost=${attempt.useClientSecretPost}): ${result.errorCode} — ${result.errorDescription}`);

    // invalid_grant is final for this credential pair; additional attempts won't help.
    if (result.errorCode === "invalid_grant") {
      throw new Error(buildLoginErrorMessage(lastErrorCode, lastErrorDescription));
    }

    // Only fall back to scopeless if the error was scope-related or client auth-related.
    if (result.errorCode === "invalid_scope" || result.errorCode === "invalid_client" || result.errorCode === "unauthorized_client") {
      shouldTryScopeless = true;
    }
  }

  // Phase 2: Try without scopes only if scoped attempts failed for scope/client reasons.
  // WARNING: Scopeless tokens may lack required audience/scopes for admin API access.
  if (shouldTryScopeless) {
    console.warn("[hcl-cs-auth] Scoped attempts failed. Trying without scopes — token may lack required admin API access.");
    for (const attempt of scopelessAttempts) {
      const result = await executePasswordGrantAttempt(tokenEndpoint, userName, password, attempt);
      if (result.ok) {
        console.warn(`[hcl-cs-auth] Password grant succeeded WITHOUT scopes (clientSecretPost=${attempt.useClientSecretPost}). Token may lack hcl-cs.* scopes needed for admin APIs.`);
        return result.token;
      }

      lastErrorCode = result.errorCode;
      lastErrorDescription = result.errorDescription;

      if (result.errorCode === "invalid_grant") {
        break;
      }
    }
  }

  throw new Error(buildLoginErrorMessage(lastErrorCode, lastErrorDescription));
}

async function requestUserCodeGrantTokens(userCode: string): Promise<TokenEndpointResponse> {
  const tokenEndpoint = await resolveTokenEndpoint();
  const payload = new URLSearchParams({
    grant_type: "user_code",
    user_code: userCode
  });
  if (env.scopes.trim()) {
    payload.set("scope", env.scopes);
  }
  payload.set("client_id", env.clientId);
  payload.set("client_secret", env.clientSecret);

  const response = await hclCsFetch(tokenEndpoint, {
    method: "POST",
    headers: { "Content-Type": "application/x-www-form-urlencoded" },
    body: payload.toString(),
    cache: "no-store"
  });

  const tokenResponse = (await response.json()) as TokenEndpointResponse;
  if (!response.ok || !tokenResponse.access_token) {
    const msg =
      typeof tokenResponse.error_description === "string"
        ? tokenResponse.error_description
        : tokenResponse.error ?? "Invalid or expired sign-in code.";
    throw new Error(msg);
  }
  return tokenResponse;
}

async function refreshAccessToken(token: JWT): Promise<JWT> {
  if (!token.refreshToken) {
    logRefreshEvent("warn", "refresh failed", token, {
      reason: "missing_refresh_token"
    });
    return buildSessionInvalidatedToken(token, SESSION_ERROR_INVALIDATED);
  }

  const refreshKey = createTokenFingerprint(token.refreshToken);
  if (!refreshKey) {
    logRefreshEvent("warn", "refresh failed", token, {
      reason: "refresh_key_unavailable"
    });
    return buildSessionInvalidatedToken(token, SESSION_ERROR_INVALIDATED);
  }

  const existingRefresh = refreshFlights.get(refreshKey);
  if (existingRefresh) {
    logRefreshEvent("info", "refresh joined existing in-flight promise", token);
    return existingRefresh;
  }

  logRefreshEvent("info", "refresh started", token);

  const refreshPromise = (async () => {
    try {
      const tokenEndpoint = await resolveTokenEndpoint();
      const payload = new URLSearchParams({
        grant_type: "refresh_token",
        refresh_token: token.refreshToken!
      });

      const response = await hclCsFetch(tokenEndpoint, {
        method: "POST",
        headers: {
          "Content-Type": "application/x-www-form-urlencoded",
          Authorization: `Basic ${encodeBasicAuth(env.clientId, env.clientSecret)}`
        },
        body: payload.toString(),
        cache: "no-store"
      });

      let refreshed: TokenEndpointResponse = {};
      try {
        refreshed = (await response.json()) as TokenEndpointResponse;
      } catch {
        refreshed = {};
      }

      if (!response.ok || !refreshed.access_token || !refreshed.refresh_token) {
        const reason =
          typeof refreshed.error_description === "string" && refreshed.error_description.trim()
            ? refreshed.error_description.trim()
            : refreshed.error ?? "RefreshAccessTokenError";
        logRefreshEvent("warn", "refresh failed", token, {
          reason,
          httpStatus: response.status
        });
        return buildSessionInvalidatedToken(token, SESSION_ERROR_REFRESH_FAILED);
      }

      const updatedToken = buildRefreshedToken(token, refreshed);
      logRefreshEvent("info", "refresh succeeded", updatedToken, {
        previous_refresh_key: refreshKey
      });
      return updatedToken;
    } catch {
      logRefreshEvent("warn", "refresh failed", token, {
        reason: "network_or_runtime_error"
      });
      return buildSessionInvalidatedToken(token, SESSION_ERROR_REFRESH_FAILED);
    }
  })().finally(() => {
    refreshFlights.delete(refreshKey);
  });

  refreshFlights.set(refreshKey, refreshPromise);
  return refreshPromise;
}

assertAuthEnv();

export const authOptions: NextAuthOptions = {
  session: {
    strategy: "jwt"
  },
  providers: [
    CredentialsProvider({
      name: "HCL.CS Credentials",
      credentials: {
        username: { label: "Username", type: "text" },
        password: { label: "Password", type: "password" },
        code: { label: "User code", type: "text" }
      },
      async authorize(credentials) {
        const userCode = typeof credentials?.code === "string" ? credentials.code.trim() : "";
        if (userCode) {
          try {
            const tokenResponse = await requestUserCodeGrantTokens(userCode);
            const preferred = decodeJwtPayload(tokenResponse.access_token)?.preferred_username;
            const userName =
              (tokenResponse as { username?: string }).username ??
              (typeof preferred === "string" ? preferred : "user");
            return createAuthenticatedUser(userName, tokenResponse);
          } catch (err) {
            return null;
          }
        }

        const userName = typeof credentials?.username === "string" ? credentials.username.trim() : "";
        const password = typeof credentials?.password === "string" ? credentials.password : "";

        if (!userName || !password) {
          return null;
        }

        const tokenResponse = await requestPasswordGrantTokens(userName, password);
        return createAuthenticatedUser(userName, tokenResponse);
      }
    })
  ],
  callbacks: {
    async redirect({ url, baseUrl }) {
      const configuredBaseUrl = env.nextAuthUrl.replace(/\/+$/, "");
      const effectiveBaseUrl = configuredBaseUrl || baseUrl.replace(/\/+$/, "");

      if (url.startsWith("/") && !url.startsWith("//")) {
        return `${effectiveBaseUrl}${url}`;
      }

      try {
        const parsed = new URL(url);
        if (parsed.origin === effectiveBaseUrl) {
          return parsed.toString();
        }
      } catch {
        // Ignore parse errors and fall back to the default admin landing page.
      }

      return `${effectiveBaseUrl}/admin/clients`;
    },
    async jwt({ token, account, user }) {
      if (account?.provider === "credentials" && user) {
        const authenticatedUser = user as AuthenticatedUser;
        return {
          ...token,
          name: authenticatedUser.name,
          email: authenticatedUser.email,
          accessToken: authenticatedUser.accessToken,
          refreshToken: authenticatedUser.refreshToken,
          idToken: authenticatedUser.idToken,
          accessTokenExpires: authenticatedUser.accessTokenExpires,
          scopes: authenticatedUser.scopes,
          roles: authenticatedUser.roles,
          isAdmin: authenticatedUser.isAdmin,
          error: undefined
        };
      }

      if (shouldInvalidateSession(token.error)) {
        return buildSessionInvalidatedToken(token, token.error ?? SESSION_ERROR_INVALIDATED);
      }

      if (shouldUseCurrentAccessToken(token)) {
        return token;
      }

      return refreshAccessToken(token);
    },
    async session({ session, token }) {
      if (!token.accessToken || shouldInvalidateSession(token.error)) {
        logRefreshEvent("warn", "stale token blocked after refresh failure", token, {
          reason: token.error ?? SESSION_ERROR_INVALIDATED
        });
        return null as never;
      }

      session.accessToken = token.accessToken;
      session.refreshToken = token.refreshToken;
      session.idToken = token.idToken;
      session.accessTokenExpires = token.accessTokenExpires;
      session.scopes = token.scopes;
      session.roles = token.roles;
      session.isAdmin = Boolean(token.isAdmin);
      session.error = token.error;
      return session;
    }
  },
  pages: {
    signIn: "/login"
  }
};

// Reuse one resolved session per request/render tree so parallel loaders do not
// independently trigger the refresh path.
const getRequestScopedSession = cache(async () => getServerSession(authOptions));

export async function auth() {
  return getRequestScopedSession();
}
