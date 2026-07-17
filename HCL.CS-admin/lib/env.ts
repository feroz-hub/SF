const LOCALHOST_ISSUER = "https://localhost:5001";
const LOCALHOST_NEXTAUTH_URL = "https://localhost:3000";

const defaultScopes =
  "openid profile email offline_access phone hcl-cs.apiresource hcl-cs.client hcl-cs.user hcl-cs.role hcl-cs.identityresource hcl-cs.adminuser hcl-cs.securitytoken";

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
const allowInsecureTls =
  process.env.HCL_CS_ALLOW_INSECURE_TLS === "true" && process.env.NODE_ENV !== "production";

export const env = {
  nextAuthUrl: readEnv("NEXTAUTH_URL") ?? LOCALHOST_NEXTAUTH_URL,
  nextAuthSecret: readEnv("NEXTAUTH_SECRET") ?? "",
  issuer,
  metadataAddress:
    process.env.HCL_CS_METADATA_ADDRESS ?? `${issuer.replace(/\/+$/, "")}/.well-known/openid-configuration`,
  clientId: readEnv("HCL_CS_CLIENT_ID") ?? "",
  clientSecret: readEnv("HCL_CS_CLIENT_SECRET") ?? "",
  scopes: process.env.HCL_CS_SCOPES ?? defaultScopes,
  tokenEndpoint: process.env.HCL_CS_TOKEN_ENDPOINT,
  revocationEndpoint: process.env.HCL_CS_REVOCATION_ENDPOINT,
  postLogoutRedirectUri:
    process.env.HCL_CS_POST_LOGOUT_REDIRECT_URI ?? `${readEnv("NEXTAUTH_URL") ?? LOCALHOST_NEXTAUTH_URL}/login`,
  enableFederatedLogout: process.env.HCL_CS_ENABLE_FEDERATED_LOGOUT === "true",
  apiBaseUrl: process.env.HCL_CS_API_BASE_URL ?? issuer,
  installerBaseUrl: process.env.HCL_CS_INSTALLER_BASE_URL ?? (process.env.HCL_CS_API_BASE_URL ?? issuer),
  // Demo Server health + external auth host. Default to issuer (Demo Server) and do NOT fall back to apiBaseUrl,
  // so health checks always reflect Demo Server rather than the management API.
  demoServerBaseUrl: process.env.HCL_CS_DEMO_SERVER_BASE_URL ?? issuer,
  allowInsecureTls,
  /** When true, show "Sign in with Google" on the login page (requires Demo Server Google config and admin client user_code grant). */
  googleLoginEnabled: process.env.NEXT_PUBLIC_GOOGLE_LOGIN_ENABLED === "true"
};

function assertProductionRuntimeEnv(): void {
  if (process.env.NODE_ENV !== "production") {
    return;
  }

  if (!readEnv("NEXTAUTH_URL")) {
    throw new Error("Missing NEXTAUTH_URL in production runtime configuration");
  }

  if (!readFirstEnv(["HCL_CS_ISSUER", "HCL_CS_AUTHORITY"])) {
    throw new Error("Missing HCL_CS_ISSUER (or HCL_CS_AUTHORITY) in production runtime configuration");
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
