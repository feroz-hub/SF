/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

import { createHash } from "node:crypto";

import type { Account } from "next-auth";
import type { JWT } from "next-auth/jwt";

import { SESSION_ERROR_INVALIDATED, SESSION_ERROR_REFRESH_FAILED } from "../auth-errors.ts";
import { decodeJwtPayload, extractRolesFromToken, hasAdminRole } from "./roles.ts";

export const ACCESS_TOKEN_REFRESH_BUFFER_MS = 60_000;

type TokenEndpointResponse = {
  access_token?: string;
  refresh_token?: string;
  id_token?: string;
  expires_in?: number;
  scope?: string;
  error?: string;
  error_description?: string;
};

type SafeTokenMetadata = {
  sub?: string;
  exp?: number;
  expHuman?: string;
  aud?: unknown;
  iss?: unknown;
};

export type RefreshAccessTokenOptions = {
  clientId: string;
  clientSecret: string;
  resolveTokenEndpoint: () => Promise<string>;
  fetch: typeof fetch;
};

const refreshFlights = new Map<string, Promise<JWT>>();

function encodeBasicAuth(clientId: string, clientSecret: string): string {
  const escapedClientId = encodeURIComponent(clientId);
  const escapedClientSecret = encodeURIComponent(clientSecret);
  return Buffer.from(`${escapedClientId}:${escapedClientSecret}`).toString("base64");
}

function readStringClaim(value: unknown): string | undefined {
  if (typeof value !== "string") {
    return undefined;
  }

  const trimmed = value.trim();
  return trimmed ? trimmed : undefined;
}

function readNumericClaim(value: unknown): number | undefined {
  if (typeof value === "number" && Number.isFinite(value)) {
    return value;
  }

  if (typeof value === "string") {
    const parsed = Number(value);
    return Number.isFinite(parsed) ? parsed : undefined;
  }

  return undefined;
}

function createTokenFingerprint(value?: string): string | undefined {
  return value ? createHash("sha256").update(value).digest("hex").slice(0, 12) : undefined;
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

function logRefreshEvent(
  level: "info" | "warn",
  event: string,
  token: Pick<JWT, "accessToken" | "refreshToken" | "sub">,
  extra: Record<string, unknown> = {}
): void {
  const metadata = extractSafeTokenMetadata(token.accessToken, readStringClaim(token.sub));
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

export function buildSessionInvalidatedToken(token: JWT, errorCode: string): JWT {
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

function resolveInitialExpiry(account: Account, accessToken: string): number {
  const expiresAt = readNumericClaim(account.expires_at);
  if (expiresAt) {
    return expiresAt * 1000;
  }

  const expiresIn = readNumericClaim(account.expires_in);
  if (expiresIn) {
    return Date.now() + expiresIn * 1000;
  }

  const accessTokenExp = readNumericClaim(decodeJwtPayload(accessToken).exp);
  return accessTokenExp ? accessTokenExp * 1000 : Date.now() + 300_000;
}

export function buildInitialOAuthToken(token: JWT, account: Account, defaultScopes?: string): JWT {
  if (account.provider !== "hcl-cs" || !account.access_token) {
    return token;
  }

  const roles = extractRolesFromToken(account.access_token);
  return {
    ...token,
    accessToken: account.access_token,
    refreshToken: account.refresh_token,
    idToken: account.id_token,
    accessTokenExpires: resolveInitialExpiry(account, account.access_token),
    scopes: typeof account.scope === "string" ? account.scope : defaultScopes,
    roles,
    isAdmin: hasAdminRole(roles),
    error: undefined
  };
}

export function shouldUseCurrentAccessToken(token: JWT): boolean {
  if (!token.accessToken || !token.accessTokenExpires) {
    return false;
  }

  return Date.now() < token.accessTokenExpires - ACCESS_TOKEN_REFRESH_BUFFER_MS;
}

function buildRefreshedToken(token: JWT, refreshed: TokenEndpointResponse): JWT {
  if (!refreshed.access_token) {
    return buildSessionInvalidatedToken(token, SESSION_ERROR_REFRESH_FAILED);
  }

  const parsedExpiresIn = Number(refreshed.expires_in ?? 300);
  const expiresInSeconds =
    Number.isFinite(parsedExpiresIn) && parsedExpiresIn > 0 ? parsedExpiresIn : 300;
  const roles = extractRolesFromToken(refreshed.access_token);

  return {
    ...token,
    accessToken: refreshed.access_token,
    refreshToken: refreshed.refresh_token ?? token.refreshToken,
    idToken: refreshed.id_token ?? token.idToken,
    accessTokenExpires: Date.now() + expiresInSeconds * 1000,
    scopes: refreshed.scope ?? token.scopes,
    roles,
    isAdmin: hasAdminRole(roles),
    error: undefined
  };
}

export async function refreshAccessToken(
  token: JWT,
  options: RefreshAccessTokenOptions
): Promise<JWT> {
  if (!token.refreshToken) {
    logRefreshEvent("warn", "refresh failed", token, { reason: "missing_refresh_token" });
    return buildSessionInvalidatedToken(token, SESSION_ERROR_INVALIDATED);
  }

  const refreshKey = createTokenFingerprint(token.refreshToken);
  if (!refreshKey) {
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
      const tokenEndpoint = await options.resolveTokenEndpoint();
      const payload = new URLSearchParams({
        grant_type: "refresh_token",
        refresh_token: token.refreshToken!
      });
      const response = await options.fetch(tokenEndpoint, {
        method: "POST",
        headers: {
          "Content-Type": "application/x-www-form-urlencoded",
          Authorization: `Basic ${encodeBasicAuth(options.clientId, options.clientSecret)}`
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

      if (!response.ok || !refreshed.access_token) {
        const reason =
          typeof refreshed.error_description === "string" && refreshed.error_description.trim()
            ? refreshed.error_description.trim()
            : (refreshed.error ?? "RefreshAccessTokenError");
        logRefreshEvent("warn", "refresh failed", token, { reason, httpStatus: response.status });
        return buildSessionInvalidatedToken(token, SESSION_ERROR_REFRESH_FAILED);
      }

      const updatedToken = buildRefreshedToken(token, refreshed);
      logRefreshEvent("info", "refresh succeeded", updatedToken, {
        previous_refresh_key: refreshKey
      });
      return updatedToken;
    } catch {
      logRefreshEvent("warn", "refresh failed", token, { reason: "network_or_runtime_error" });
      return buildSessionInvalidatedToken(token, SESSION_ERROR_REFRESH_FAILED);
    }
  })().finally(() => {
    refreshFlights.delete(refreshKey);
  });

  refreshFlights.set(refreshKey, refreshPromise);
  return refreshPromise;
}
