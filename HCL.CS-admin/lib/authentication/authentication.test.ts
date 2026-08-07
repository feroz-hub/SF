/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

import assert from "node:assert/strict";
import { readFile } from "node:fs/promises";
import test from "node:test";

import type { Account, Session } from "next-auth";
import type { JWT } from "next-auth/jwt";

import { SESSION_ERROR_REFRESH_FAILED } from "../auth-errors.ts";
import { isAuthorizedAdminToken } from "./authorization.ts";
import { createHclCsProvider } from "./provider.ts";
import { extractRolesFromToken, hasAdminRole } from "./roles.ts";
import {
  ADMIN_IDENTITY_SCOPES,
  ADMIN_PERMISSION_SCOPES,
  DEFAULT_HCL_CS_SCOPES,
  normalizeAuthScopes
} from "./scopes.ts";
import { attachServerOnlyTokens } from "./session.ts";
import { buildInitialOAuthToken, refreshAccessToken } from "./token-lifecycle.ts";

const configuredScopes = DEFAULT_HCL_CS_SCOPES;

function jwt(payload: Record<string, unknown>): string {
  const encode = (value: unknown) => Buffer.from(JSON.stringify(value)).toString("base64url");
  return `${encode({ alg: "none" })}.${encode(payload)}.`;
}

function account(overrides: Partial<Account> = {}): Account {
  return {
    provider: "hcl-cs",
    type: "oauth",
    providerAccountId: "admin-1",
    access_token: jwt({ sub: "admin-1", role: ["Users", "PlatformAdmin"] }),
    refresh_token: "refresh-one",
    id_token: jwt({ sub: "admin-1", name: "Admin" }),
    expires_at: Math.floor(Date.now() / 1000) + 900,
    scope: configuredScopes,
    ...overrides
  };
}

test("HCL.CS provider uses discovery, Authorization Code, PKCE, state, and confidential client auth", () => {
  const provider = createHclCsProvider({
    issuer: "https://localhost:5180/",
    metadataAddress: "https://localhost:5180/.well-known/openid-configuration",
    clientId: "admin-client",
    clientSecret: "server-only-secret",
    scopes: configuredScopes
  });

  assert.equal(provider.id, "hcl-cs");
  assert.equal(provider.name, "HCL.CS");
  assert.equal(provider.type, "oauth");
  assert.equal(provider.issuer, "https://localhost:5180");
  assert.equal(provider.wellKnown, "https://localhost:5180/.well-known/openid-configuration");
  assert.deepEqual(provider.checks, ["pkce", "state"]);
  assert.equal(
    provider.authorization && typeof provider.authorization === "object"
      ? provider.authorization.params?.response_type
      : undefined,
    "code"
  );
  assert.equal(provider.client?.token_endpoint_auth_method, "client_secret_basic");
  assert.equal(provider.idToken, true);
  assert.equal(
    `https://localhost:3001/api/auth/callback/${provider.id}`,
    "https://localhost:3001/api/auth/callback/hcl-cs"
  );
});

test("configured authorization scopes include identity, offline access, and every required HCL.CS scope", () => {
  const provider = createHclCsProvider({
    issuer: "https://localhost:5180",
    metadataAddress: "https://localhost:5180/.well-known/openid-configuration",
    clientId: "id",
    clientSecret: "secret",
    scopes: configuredScopes
  });
  const scope =
    provider.authorization && typeof provider.authorization === "object"
      ? String(provider.authorization.params?.scope)
      : "";
  const values = new Set(scope.split(/\s+/));

  for (const required of configuredScopes.split(" ")) {
    assert.equal(values.has(required), true, `missing scope ${required}`);
  }
  assert.equal(values.has("openid"), true);
  assert.equal(values.has("offline_access"), true);
});

test("scope normalization always includes openid and offline_access", () => {
  const values = new Set(normalizeAuthScopes("profile hcl-cs.apiresource").split(/\s+/));
  assert.equal(values.has("openid"), true);
  assert.equal(values.has("offline_access"), true);
  assert.equal(values.has("profile"), true);
  assert.equal(values.has("hcl-cs.apiresource.read"), true);
  assert.equal(values.has("hcl-cs.apiresource.manage"), true);
  assert.equal(values.has("hcl-cs.apiresource"), false);
});

test("legacy Admin umbrella scopes normalize to the exact granular permission contract", () => {
  const legacyScopes =
    "openid profile email phone offline_access hcl-cs.apiresource hcl-cs.identityresource hcl-cs.client hcl-cs.user hcl-cs.role hcl-cs.adminuser hcl-cs.securitytoken";
  const normalized = normalizeAuthScopes(legacyScopes).split(/\s+/);
  const expected = [...ADMIN_IDENTITY_SCOPES, ...ADMIN_PERMISSION_SCOPES];

  assert.deepEqual(new Set(normalized), new Set(expected));
  assert.equal(normalized.some((scope) => /^hcl-cs\.[^.]+$/.test(scope)), false);
});

test("Admin, Installer, and example environment share one scope contract", async () => {
  const [contractSource, envExample] = await Promise.all([
    readFile(
      new URL(
        "../../../src/Identity/HCL.CS.Identity.Domain/Constants/Endpoint/AdminClientScopeContract.cs",
        import.meta.url
      ),
      "utf8"
    ),
    readFile(new URL("../../.env.example", import.meta.url), "utf8")
  ]);
  const contractExpression = contractSource.match(/SpaceSeparated\s*=([\s\S]*?);/)?.[1] ?? "";
  const installerScopes = [...contractExpression.matchAll(/"([^"]*)"/g)]
    .map((match) => match[1])
    .join("")
    .trim();
  const exampleScopes = envExample.match(/^HCL_CS_SCOPES=(.+)$/m)?.[1]?.trim();

  assert.equal(installerScopes, DEFAULT_HCL_CS_SCOPES);
  assert.equal(exampleScopes, DEFAULT_HCL_CS_SCOPES);
});

test("initial OAuth callback tokens are copied from account into the JWT and roles determine admin access", () => {
  const initial = buildInitialOAuthToken({ name: "Admin" }, account(), configuredScopes);

  assert.match(String(initial.accessToken), /^[^.]+\.[^.]+\.$/);
  assert.equal(initial.refreshToken, "refresh-one");
  assert.match(String(initial.idToken), /^[^.]+\.[^.]+\.$/);
  assert.equal(initial.scopes, configuredScopes);
  assert.deepEqual(initial.roles, ["Users", "PlatformAdmin"]);
  assert.equal(initial.isAdmin, true);
  assert.equal(typeof initial.accessTokenExpires, "number");
});

test("role extraction supports HCL.CS role claim shapes and admin detection is case-insensitive", () => {
  const accessToken = jwt({
    role: "Reader,SecurityAdmin",
    roles: ["Operator"],
    userrole: "AuditAdmin",
    "http://schemas.microsoft.com/ws/2008/06/identity/claims/role": "TenantAdmin"
  });
  const roles = extractRolesFromToken(accessToken);

  assert.deepEqual(roles, ["Reader", "SecurityAdmin", "Operator", "AuditAdmin", "TenantAdmin"]);
  assert.equal(hasAdminRole(roles), true);
  assert.equal(hasAdminRole(["Reader", "Operator"]), false);
});

test("refresh-token flow uses refresh grant, Basic client auth, and refresh-token rotation", async () => {
  const refreshedAccessToken = jwt({ sub: "admin-1", role: "Admin", exp: 4_000_000_000 });
  let request: { url: string; init?: RequestInit } | undefined;
  const result = await refreshAccessToken(
    {
      accessToken: jwt({ sub: "admin-1", role: "Admin", exp: 1 }),
      refreshToken: "refresh-old",
      roles: ["Admin"],
      isAdmin: true
    },
    {
      clientId: "client-id",
      clientSecret: "client-secret",
      resolveTokenEndpoint: async () => "https://localhost:5180/security/token",
      fetch: async (url, init) => {
        request = { url: String(url), init };
        return Response.json({
          access_token: refreshedAccessToken,
          refresh_token: "refresh-rotated",
          expires_in: 900,
          scope: configuredScopes
        });
      }
    }
  );

  assert.equal(request?.url, "https://localhost:5180/security/token");
  assert.equal(new URLSearchParams(String(request?.init?.body)).get("grant_type"), "refresh_token");
  assert.equal(
    new URLSearchParams(String(request?.init?.body)).get("refresh_token"),
    "refresh-old"
  );
  assert.match(String((request?.init?.headers as Record<string, string>).Authorization), /^Basic /);
  assert.equal(result.accessToken, refreshedAccessToken);
  assert.equal(result.refreshToken, "refresh-rotated");
  assert.equal(result.isAdmin, true);
});

test("refresh preserves the existing refresh token when the server does not rotate it", async () => {
  const result = await refreshAccessToken(
    { accessToken: jwt({ exp: 1 }), refreshToken: "still-valid" },
    {
      clientId: "client",
      clientSecret: "secret",
      resolveTokenEndpoint: async () => "https://localhost:5180/security/token",
      fetch: async () => Response.json({ access_token: jwt({ role: "Admin" }), expires_in: 900 })
    }
  );

  assert.equal(result.refreshToken, "still-valid");
});

test("concurrent refresh attempts for one refresh token share a single request", async () => {
  let fetchCount = 0;
  let release!: () => void;
  const gate = new Promise<void>((resolve) => {
    release = resolve;
  });
  const options = {
    clientId: "client",
    clientSecret: "secret",
    resolveTokenEndpoint: async () => "https://localhost:5180/security/token",
    fetch: async () => {
      fetchCount += 1;
      await gate;
      return Response.json({ access_token: jwt({ role: "Admin" }), expires_in: 900 });
    }
  };
  const stale: JWT = { accessToken: jwt({ exp: 1 }), refreshToken: "single-flight" };
  const first = refreshAccessToken(stale, options);
  const second = refreshAccessToken(stale, options);

  await Promise.resolve();
  assert.equal(fetchCount, 1);
  release();
  await Promise.all([first, second]);
});

test("failed refresh hard-invalidates the session and removes all bearer tokens", async () => {
  const result = await refreshAccessToken(
    {
      accessToken: jwt({ sub: "admin-1", role: "Admin", exp: 1 }),
      refreshToken: "expired-refresh",
      idToken: "id-token",
      roles: ["Admin"],
      isAdmin: true
    },
    {
      clientId: "client",
      clientSecret: "secret",
      resolveTokenEndpoint: async () => "https://localhost:5180/security/token",
      fetch: async () =>
        Response.json(
          { error: "invalid_grant", error_description: "Refresh token validation failed." },
          { status: 400 }
        )
    }
  );

  assert.equal(result.error, SESSION_ERROR_REFRESH_FAILED);
  assert.equal(result.accessToken, undefined);
  assert.equal(result.refreshToken, undefined);
  assert.equal(result.idToken, undefined);
  assert.deepEqual(result.roles, []);
  assert.equal(result.isAdmin, false);
});

test("authorization rejects missing, failed, and authenticated non-admin sessions", () => {
  assert.equal(isAuthorizedAdminToken(null), false);
  assert.equal(isAuthorizedAdminToken({ accessToken: "token", roles: ["Reader"] }), false);
  assert.equal(
    isAuthorizedAdminToken({
      accessToken: "token",
      roles: ["Admin"],
      error: SESSION_ERROR_REFRESH_FAILED
    }),
    false
  );
  assert.equal(isAuthorizedAdminToken({ accessToken: "token", roles: ["TenantAdmin"] }), true);
});

test("server token fields remain available to server callers but are not serialized to clients", () => {
  const session = attachServerOnlyTokens(
    {
      user: { name: "Admin" },
      expires: new Date(Date.now() + 60_000).toISOString(),
      roles: ["Admin"]
    } as Session,
    { accessToken: "access-secret", refreshToken: "refresh-secret", idToken: "identity-secret" }
  );
  const serialized = JSON.stringify(session);

  assert.equal(session.accessToken, "access-secret");
  assert.doesNotMatch(serialized, /access-secret|refresh-secret|identity-secret/);
  assert.match(serialized, /Admin/);
});

test("normal Admin login contains no CredentialsProvider, password grant, or user_code fallback", async () => {
  const [authSource, loginSource] = await Promise.all([
    readFile(new URL("../auth.ts", import.meta.url), "utf8"),
    readFile(new URL("../../app/(auth)/login/page.tsx", import.meta.url), "utf8")
  ]);
  const normalLoginSource = `${authSource}\n${loginSource}`;

  assert.doesNotMatch(normalLoginSource, /CredentialsProvider/);
  assert.doesNotMatch(normalLoginSource, /grant_type\s*:\s*["']password["']/);
  assert.doesNotMatch(normalLoginSource, /user_code|UserCode/);
  assert.doesNotMatch(normalLoginSource, /signIn\(["']credentials["']/);
  assert.match(loginSource, /signIn\(["']hcl-cs["']/);
  assert.match(loginSource, /Sign in with HCL\.CS/);
});
