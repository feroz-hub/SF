/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

import type { OAuthConfig } from "next-auth/providers/oauth";

export type HclCsProfile = {
  sub?: unknown;
  name?: unknown;
  preferred_username?: unknown;
  email?: unknown;
};

export type HclCsProviderConfig = {
  issuer: string;
  metadataAddress: string;
  clientId: string;
  clientSecret: string;
  scopes: string;
};

function claimString(value: unknown): string | undefined {
  return typeof value === "string" && value.trim() ? value.trim() : undefined;
}

export function createHclCsProvider(config: HclCsProviderConfig): OAuthConfig<HclCsProfile> {
  return {
    id: "hcl-cs",
    name: "HCL.CS",
    type: "oauth",
    issuer: config.issuer.replace(/\/+$/, ""),
    wellKnown: config.metadataAddress,
    clientId: config.clientId,
    clientSecret: config.clientSecret,
    client: {
      token_endpoint_auth_method: "client_secret_basic"
    },
    idToken: true,
    checks: ["pkce", "state", "nonce"],
    authorization: {
      params: {
        response_type: "code",
        scope: config.scopes
      }
    },
    profile(profile) {
      const id = claimString(profile.sub);
      if (!id) {
        throw new TypeError("HCL.CS ID token is missing the required sub claim");
      }

      return {
        id,
        name: claimString(profile.name) ?? claimString(profile.preferred_username) ?? id,
        email: claimString(profile.email) ?? null
      };
    }
  };
}
