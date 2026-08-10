/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

import { getServerSession, type NextAuthOptions } from "next-auth";
import { cache } from "react";

import { SESSION_ERROR_INVALIDATED, isHardSessionError } from "@/lib/auth-errors";
import { createHclCsProvider } from "@/lib/authentication/provider";
import { extractRolesFromToken, hasAdminRole } from "@/lib/authentication/roles";
import { attachServerOnlyTokens, type ServerSession } from "@/lib/authentication/session";
import {
  buildInitialOAuthToken,
  buildSessionInvalidatedToken,
  refreshAccessToken,
  shouldUseCurrentAccessToken
} from "@/lib/authentication/token-lifecycle";
import { assertAuthEnv, env } from "@/lib/env";
import { resolveTokenEndpoint } from "@/lib/oidc";
import { hclCsFetch } from "@/lib/server-fetch";

export { extractRolesFromToken, hasAdminRole };

assertAuthEnv();

export const authOptions: NextAuthOptions = {
  session: {
    strategy: "jwt"
  },
  providers: [
    createHclCsProvider({
      issuer: env.issuer,
      metadataAddress: env.metadataAddress,
      clientId: env.clientId,
      clientSecret: env.clientSecret,
      scopes: env.scopes
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
    async jwt({ token, account }) {
      if (account?.provider === "hcl-cs") {
        return buildInitialOAuthToken(token, account, env.scopes);
      }

      if (isHardSessionError(token.error)) {
        return buildSessionInvalidatedToken(token, token.error ?? SESSION_ERROR_INVALIDATED);
      }

      if (shouldUseCurrentAccessToken(token)) {
        return token;
      }

      return refreshAccessToken(token, {
        clientId: env.clientId,
        clientSecret: env.clientSecret,
        resolveTokenEndpoint,
        fetch: hclCsFetch
      });
    },
    async session({ session, token }) {
      if (!token.accessToken || isHardSessionError(token.error)) {
        return null as never;
      }

      session.accessTokenExpires = token.accessTokenExpires;
      session.scopes = token.scopes;
      session.roles = token.roles;
      session.isAdmin = Boolean(token.isAdmin);
      session.error = token.error;
      return attachServerOnlyTokens(session, token);
    }
  },
  pages: {
    signIn: "/login"
  }
};

// Reuse one resolved session per request/render tree so parallel loaders do not
// independently trigger the refresh path.
const getRequestScopedSession = cache(
  async () => (await getServerSession(authOptions)) as ServerSession | null
);

export async function auth(): Promise<ServerSession | null> {
  return getRequestScopedSession();
}
