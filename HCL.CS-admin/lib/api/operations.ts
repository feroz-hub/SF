/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

import { ApiRoutes } from "@/lib/api/routes";
import { hclCsGetWithSession, hclCsPostWithSession } from "@/lib/api/client";
import { env } from "@/lib/env";
import { hclCsFetch } from "@/lib/server-fetch";

async function fetchText(url: string, init?: RequestInit): Promise<string> {
  const response = await hclCsFetch(url, { ...init, cache: "no-store" });
  const body = await response.text();
  if (!response.ok) {
    throw new Error(body || `HTTP ${response.status}`);
  }
  return body;
}

export const Operations = {
  oidc: {
    jwks: async () => hclCsGetWithSession<unknown>(ApiRoutes.endpoint.jwks),
    discovery: async () => hclCsGetWithSession<unknown>(ApiRoutes.endpoint.discovery),
    token: async (payload: unknown) => hclCsPostWithSession<unknown, unknown>(ApiRoutes.endpoint.token, payload),
    introspect: async (payload: unknown) => hclCsPostWithSession<unknown, unknown>(ApiRoutes.endpoint.introspect, payload),
    userinfo: async (payload: unknown) => hclCsPostWithSession<unknown, unknown>(ApiRoutes.endpoint.userinfo, payload),
    revocation: async (payload: unknown) => hclCsPostWithSession<unknown, unknown>(ApiRoutes.endpoint.revocation, payload),
    authorizeUrl: () => `${env.apiBaseUrl.replace(/\/+$/, "")}${ApiRoutes.endpoint.authorize}`,
    endsessionUrl: () => `${env.apiBaseUrl.replace(/\/+$/, "")}${ApiRoutes.endpoint.endsession}`
  },
  health: {
    health: async () => hclCsGetWithSession<unknown>(ApiRoutes.health.health),
    live: async () => hclCsGetWithSession<unknown>(ApiRoutes.health.live),
    ready: async () => hclCsGetWithSession<unknown>(ApiRoutes.health.ready)
  },
  installer: {
    rootHtml: async () => fetchText(`${env.installerBaseUrl.replace(/\/+$/, "")}${ApiRoutes.installer.root}`),
    setupHtml: async () => fetchText(`${env.installerBaseUrl.replace(/\/+$/, "")}${ApiRoutes.installer.setup}`),
    providerGetHtml: async () =>
      fetchText(`${env.installerBaseUrl.replace(/\/+$/, "")}${ApiRoutes.installer.providerGet}`),
    providerPostHtml: async (payload: Record<string, string>) =>
      fetchText(`${env.installerBaseUrl.replace(/\/+$/, "")}${ApiRoutes.installer.providerPost}`, {
        method: "POST",
        headers: { "Content-Type": "application/x-www-form-urlencoded" },
        body: new URLSearchParams(payload).toString()
      }),
    connectionGetHtml: async () =>
      fetchText(`${env.installerBaseUrl.replace(/\/+$/, "")}${ApiRoutes.installer.connectionGet}`),
    connectionPostHtml: async (payload: Record<string, string>) =>
      fetchText(`${env.installerBaseUrl.replace(/\/+$/, "")}${ApiRoutes.installer.connectionPost}`, {
        method: "POST",
        headers: { "Content-Type": "application/x-www-form-urlencoded" },
        body: new URLSearchParams(payload).toString()
      }),
    validateGetHtml: async () => fetchText(`${env.installerBaseUrl.replace(/\/+$/, "")}${ApiRoutes.installer.validateGet}`),
    validatePostHtml: async (payload: Record<string, string>) =>
      fetchText(`${env.installerBaseUrl.replace(/\/+$/, "")}${ApiRoutes.installer.validatePost}`, {
        method: "POST",
        headers: { "Content-Type": "application/x-www-form-urlencoded" },
        body: new URLSearchParams(payload).toString()
      }),
    migrateGetHtml: async () => fetchText(`${env.installerBaseUrl.replace(/\/+$/, "")}${ApiRoutes.installer.migrateGet}`),
    migratePostHtml: async (payload: Record<string, string>) =>
      fetchText(`${env.installerBaseUrl.replace(/\/+$/, "")}${ApiRoutes.installer.migratePost}`, {
        method: "POST",
        headers: { "Content-Type": "application/x-www-form-urlencoded" },
        body: new URLSearchParams(payload).toString()
      }),
    seedGetHtml: async () => fetchText(`${env.installerBaseUrl.replace(/\/+$/, "")}${ApiRoutes.installer.seedGet}`),
    seedPostHtml: async (payload: Record<string, string>) =>
      fetchText(`${env.installerBaseUrl.replace(/\/+$/, "")}${ApiRoutes.installer.seedPost}`, {
        method: "POST",
        headers: { "Content-Type": "application/x-www-form-urlencoded" },
        body: new URLSearchParams(payload).toString()
      }),
    installedHtml: async () => fetchText(`${env.installerBaseUrl.replace(/\/+$/, "")}${ApiRoutes.installer.installed}`),
    completeHtml: async () => fetchText(`${env.installerBaseUrl.replace(/\/+$/, "")}${ApiRoutes.installer.complete}`),
    errorHtml: async () => fetchText(`${env.installerBaseUrl.replace(/\/+$/, "")}${ApiRoutes.installer.error}`)
  },
  demoExternalAuth: {
    googleStartUrl: () => `${env.demoServerBaseUrl.replace(/\/+$/, "")}${ApiRoutes.demoExternalAuth.googleStart}`,
    googleCallbackUrl: () => `${env.demoServerBaseUrl.replace(/\/+$/, "")}${ApiRoutes.demoExternalAuth.googleCallback}`,
    linkGoogle: async (payload: unknown) =>
      hclCsPostWithSession<unknown, unknown>(ApiRoutes.demoExternalAuth.linkGoogle, payload, env.demoServerBaseUrl),
    unlinkGoogle: async (payload: unknown) =>
      hclCsPostWithSession<unknown, unknown>(ApiRoutes.demoExternalAuth.unlinkGoogle, payload, env.demoServerBaseUrl)
  }
} as const;
