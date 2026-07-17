import { randomUUID } from "crypto";

import { isHardSessionError } from "@/lib/auth-errors";
import { auth } from "@/lib/auth";
import { env } from "@/lib/env";
import { hclCsFetch } from "@/lib/server-fetch";
import { type FrameworkResult, type ResultStatus } from "@/lib/types/hcl-cs";

export class HclCsApiError extends Error {
  statusCode: number;
  correlationId: string;
  details: unknown;

  constructor(message: string, statusCode: number, correlationId: string, details: unknown) {
    super(message);
    this.name = "HclCsApiError";
    this.statusCode = statusCode;
    this.correlationId = correlationId;
    this.details = details;
  }
}

export function apiErrorMessage(error: unknown): string {
  if (error instanceof HclCsApiError) {
    if (error.statusCode === 401) {
      return "Session expired. Please sign in again.";
    }
    if (error.statusCode === 403) {
      return `Access denied (HTTP 403). ${error.message}`;
    }
    return `${error.message} (HTTP ${error.statusCode}, correlation=${error.correlationId})`;
  }
  if (error instanceof Error) {
    return error.message;
  }
  return "Failed to load data.";
}

/** Use when building page load error state; provides message and whether to offer sign-in. */
export function getLoadErrorInfo(error: unknown): { message: string; isUnauthorized: boolean } {
  const isUnauthorized = isUnauthorizedError(error);
  return {
    message: apiErrorMessage(error),
    isUnauthorized
  };
}

export function isUnauthorizedError(error: unknown): error is HclCsApiError {
  return error instanceof HclCsApiError && error.statusCode === 401;
}

/** Extract safe-to-log metadata from a JWT without exposing the full token. */
function safeTokenSummary(token: string): Record<string, unknown> {
  try {
    const parts = token.split(".");
    if (parts.length < 2) return { error: "not_a_jwt" };
    const payload = JSON.parse(Buffer.from(parts[1].replace(/-/g, "+").replace(/_/g, "/"), "base64").toString("utf8"));
    return {
      sub: payload.sub,
      aud: payload.aud,
      iss: payload.iss,
      scope: payload.scope,
      client_id: payload.client_id,
      exp: payload.exp,
      exp_human: payload.exp ? new Date(payload.exp * 1000).toISOString() : undefined,
      role: payload.role ?? payload.roles ?? payload.userrole,
      nbf: payload.nbf
    };
  } catch {
    return { error: "decode_failed" };
  }
}

/** Log API call failures with safe diagnostics for debugging. */
function logApiFailure(
  method: string,
  path: string,
  correlationId: string,
  status: number,
  responseExcerpt: unknown,
  tokenSummary?: Record<string, unknown>,
  wwwAuthenticate?: string | null
): void {
  console.error(
    `[hcl-cs-api] FAILED ${method} ${path}`,
    JSON.stringify({
      correlationId,
      httpStatus: status,
      response: typeof responseExcerpt === "string" ? responseExcerpt.slice(0, 500) : responseExcerpt,
      ...(tokenSummary ? { token: tokenSummary } : {}),
      ...(wwwAuthenticate ? { wwwAuthenticate } : {})
    })
  );
}

function isFailedStatus(value: ResultStatus | undefined): boolean {
  if (value === undefined) {
    return false;
  }

  if (typeof value === "number") {
    return value !== 0;
  }

  return value.toLowerCase() !== "succeeded";
}

function normalizeMessage(payload: unknown, fallback: string): string {
  if (!payload || typeof payload !== "object") {
    return fallback;
  }

  const frameworkResult = payload as Partial<FrameworkResult>;
  if (frameworkResult.Errors && frameworkResult.Errors.length > 0) {
    return frameworkResult.Errors.map((error) => error.Description).join(", ");
  }

  if ("message" in payload && typeof payload.message === "string") {
    return payload.message;
  }

  if ("error_description" in payload && typeof payload.error_description === "string") {
    return payload.error_description;
  }

  if ("error" in payload && typeof payload.error === "string") {
    return payload.error;
  }

  return fallback;
}

function toApiUrl(path: string, baseUrl: string = env.apiBaseUrl): string {
  const base = baseUrl.replace(/\/+$/, "");
  return `${base}${path}`;
}

export async function requireAccessToken(): Promise<string> {
  const session = await auth();
  if (!session) {
    console.warn("[hcl-cs-api] requireAccessToken: blocked request because no valid session is available.");
    throw new HclCsApiError("Session expired. Please sign in again.", 401, "none", null);
  }

  if (isHardSessionError(session.error)) {
    const tokenSummary = session.accessToken ? safeTokenSummary(session.accessToken) : { error: "missing_access_token" };
    console.warn("[hcl-cs-api] requireAccessToken: blocked stale token after refresh failure.", {
      sub: tokenSummary.sub ?? "unknown",
      exp_human: tokenSummary.exp_human ?? "unknown",
      sessionError: session.error
    });
    throw new HclCsApiError("Session expired. Please sign in again.", 401, "none", null);
  }

  if (!session?.accessToken) {
    console.error("[hcl-cs-api] requireAccessToken: no accessToken in session.", {
      hasSession: !!session,
      hasError: session?.error ?? "none",
      roles: session?.roles ?? [],
      isAdmin: session?.isAdmin ?? false,
      expiresAt: session?.accessTokenExpires
        ? new Date(session.accessTokenExpires).toISOString()
        : "unknown"
    });
    throw new HclCsApiError("Authenticated session is missing an access token.", 401, "none", null);
  }

  if (process.env.NODE_ENV !== "production") {
    const summary = safeTokenSummary(session.accessToken);
    console.info("[hcl-cs-api] requireAccessToken: token acquired.", {
      sub: summary.sub,
      aud: summary.aud,
      scope: summary.scope,
      exp_human: summary.exp_human,
      sessionError: session.error ?? "none"
    });
  }

  return session.accessToken;
}

export async function hclCsPost<TResponse, TRequest = string>(
  path: string,
  payload: TRequest,
  accessToken: string,
  baseUrl?: string
): Promise<TResponse> {
  const correlationId = randomUUID();
  const body = JSON.stringify(payload);

  if (process.env.NODE_ENV !== "production") {
    console.info(`[hcl-cs-api] POST ${path} correlation=${correlationId}`);
  }

  const response = await hclCsFetch(toApiUrl(path, baseUrl), {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${accessToken}`,
      "X-Correlation-ID": correlationId
    },
    body,
    cache: "no-store"
  });

  let parsedPayload: unknown = null;
  try {
    parsedPayload = await response.json();
  } catch {
    parsedPayload = null;
  }

  if (!response.ok) {
    const wwwAuth = response.headers.get("www-authenticate");
    logApiFailure("POST", path, correlationId, response.status, parsedPayload, safeTokenSummary(accessToken), wwwAuth);
    throw new HclCsApiError(
      normalizeMessage(parsedPayload, wwwAuth ? `HTTP ${response.status}: ${wwwAuth}` : `HTTP ${response.status}`),
      response.status,
      correlationId,
      parsedPayload
    );
  }

  if (parsedPayload && typeof parsedPayload === "object") {
    const frameworkResult = parsedPayload as Partial<FrameworkResult>;
    if (isFailedStatus(frameworkResult.Status)) {
      throw new HclCsApiError(
        normalizeMessage(parsedPayload, "HCL.CS API request failed."),
        response.status,
        correlationId,
        parsedPayload
      );
    }
  }

  return parsedPayload as TResponse;
}

export async function hclCsPostWithSession<TResponse, TRequest = string>(
  path: string,
  payload: TRequest,
  baseUrl?: string
): Promise<TResponse> {
  const accessToken = await requireAccessToken();
  return hclCsPost<TResponse, TRequest>(path, payload, accessToken, baseUrl);
}

/** POST to HCL.CS API without Authorization. Use for anonymous endpoints (e.g. Forgot Password, Reset Password). */
export async function hclCsPostAnonymous<TResponse, TRequest = string>(
  path: string,
  payload: TRequest,
  baseUrl?: string
): Promise<TResponse> {
  const correlationId = randomUUID();
  const body = JSON.stringify(payload);

  if (process.env.NODE_ENV !== "production") {
    console.info(`[hcl-cs-api] POST (anonymous) ${path} correlation=${correlationId}`);
  }

  const response = await hclCsFetch(toApiUrl(path, baseUrl), {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      "X-Correlation-ID": correlationId
    },
    body,
    cache: "no-store"
  });

  let parsedPayload: unknown = null;
  try {
    parsedPayload = await response.json();
  } catch {
    parsedPayload = null;
  }

  if (!response.ok) {
    throw new HclCsApiError(
      normalizeMessage(parsedPayload, `HTTP ${response.status}`),
      response.status,
      correlationId,
      parsedPayload
    );
  }

  if (parsedPayload && typeof parsedPayload === "object") {
    const frameworkResult = parsedPayload as Partial<FrameworkResult>;
    if (isFailedStatus(frameworkResult.Status)) {
      throw new HclCsApiError(
        normalizeMessage(parsedPayload, "Request failed."),
        response.status,
        correlationId,
        parsedPayload
      );
    }
  }

  return parsedPayload as TResponse;
}

export async function hclCsGet<TResponse>(path: string, accessToken: string, baseUrl?: string): Promise<TResponse> {
  const correlationId = randomUUID();

  if (process.env.NODE_ENV !== "production") {
    console.info(`[hcl-cs-api] GET ${path} correlation=${correlationId}`);
  }

  const response = await hclCsFetch(toApiUrl(path, baseUrl), {
    method: "GET",
    headers: {
      Authorization: `Bearer ${accessToken}`,
      "X-Correlation-ID": correlationId
    },
    cache: "no-store"
  });

  let parsedPayload: unknown = null;
  try {
    parsedPayload = await response.json();
  } catch {
    parsedPayload = null;
  }

  if (!response.ok) {
    const wwwAuth = response.headers.get("www-authenticate");
    logApiFailure("GET", path, correlationId, response.status, parsedPayload, safeTokenSummary(accessToken), wwwAuth);
    throw new HclCsApiError(
      normalizeMessage(parsedPayload, wwwAuth ? `HTTP ${response.status}: ${wwwAuth}` : `HTTP ${response.status}`),
      response.status,
      correlationId,
      parsedPayload
    );
  }

  if (parsedPayload && typeof parsedPayload === "object") {
    const frameworkResult = parsedPayload as Partial<FrameworkResult>;
    if (isFailedStatus(frameworkResult.Status)) {
      throw new HclCsApiError(
        normalizeMessage(parsedPayload, "HCL.CS API request failed."),
        response.status,
        correlationId,
        parsedPayload
      );
    }
  }

  return parsedPayload as TResponse;
}

export async function hclCsGetWithSession<TResponse>(path: string, baseUrl?: string): Promise<TResponse> {
  const accessToken = await requireAccessToken();
  return hclCsGet<TResponse>(path, accessToken, baseUrl);
}

export function getClientBasicAuthHeaderValue(): string {
  const encoded = Buffer.from(`${encodeURIComponent(env.clientId)}:${encodeURIComponent(env.clientSecret)}`).toString(
    "base64"
  );
  return `Basic ${encoded}`;
}
