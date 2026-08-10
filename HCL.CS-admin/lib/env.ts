/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

import { DEFAULT_HCL_CS_SCOPES, normalizeAuthScopes } from "@/lib/authentication/scopes";

const LOCALHOST_ISSUER = "https://localhost:5180";
const LOCALHOST_NEXTAUTH_URL = "https://localhost:3001";

export { normalizeAuthScopes };

function readEnv(name: string): string | undefined {
  const value = process.env[name]?.trim();
  return value ? value : undefined;
}

function readFirstEnv(names: string[]): string | undefined {
  for (const name of names) {
    const value = readEnv(name);
    if (value) {
      return value;
    }
  }

  return undefined;
}

const issuer = readFirstEnv(["HCL_CS_ISSUER", "HCL_CS_AUTHORITY"]) ?? LOCALHOST_ISSUER;
export const env = {
  nextAuthUrl: readEnv("NEXTAUTH_URL") ?? LOCALHOST_NEXTAUTH_URL,
  nextAuthSecret: readEnv("NEXTAUTH_SECRET") ?? "",
  issuer,
  metadataAddress:
    readEnv("HCL_CS_METADATA_ADDRESS") ??
    `${issuer.replace(/\/+$/, "")}/.well-known/openid-configuration`,
  clientId: readEnv("HCL_CS_CLIENT_ID") ?? "",
  clientSecret: readEnv("HCL_CS_CLIENT_SECRET") ?? "",
  scopes: normalizeAuthScopes(readEnv("HCL_CS_SCOPES") ?? DEFAULT_HCL_CS_SCOPES),
  tokenEndpoint: readEnv("HCL_CS_TOKEN_ENDPOINT"),
  revocationEndpoint: readEnv("HCL_CS_REVOCATION_ENDPOINT"),
  postLogoutRedirectUri:
    process.env.HCL_CS_POST_LOGOUT_REDIRECT_URI ??
    `${readEnv("NEXTAUTH_URL") ?? LOCALHOST_NEXTAUTH_URL}/login`,
  enableFederatedLogout: process.env.HCL_CS_ENABLE_FEDERATED_LOGOUT === "true",
  apiBaseUrl: process.env.HCL_CS_API_BASE_URL ?? issuer,
  installerBaseUrl:
    process.env.HCL_CS_INSTALLER_BASE_URL ?? process.env.HCL_CS_API_BASE_URL ?? issuer,
  // Demo Server health host. Default to issuer (Demo Server) and do NOT fall back to apiBaseUrl,
  // so health checks always reflect Demo Server rather than the management API.
  demoServerBaseUrl: process.env.HCL_CS_DEMO_SERVER_BASE_URL ?? issuer
};

function assertProductionRuntimeEnv(): void {
  if (process.env.NODE_ENV !== "production") {
    return;
  }

  if (!readEnv("NEXTAUTH_URL")) {
    throw new Error("Missing NEXTAUTH_URL in production runtime configuration");
  }

  if (!readFirstEnv(["HCL_CS_ISSUER", "HCL_CS_AUTHORITY"])) {
    throw new Error(
      "Missing HCL_CS_ISSUER (or HCL_CS_AUTHORITY) in production runtime configuration"
    );
  }
}

export function assertAuthEnv(): void {
  assertProductionRuntimeEnv();

  if (!env.nextAuthSecret) {
    throw new Error("Missing NEXTAUTH_SECRET");
  }

  if (!env.clientId) {
    throw new Error("Missing HCL_CS_CLIENT_ID");
  }

  if (!env.clientSecret) {
    throw new Error("Missing HCL_CS_CLIENT_SECRET");
  }
}
