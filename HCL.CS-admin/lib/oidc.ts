/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

import { env } from "@/lib/env";
import { hclCsFetch } from "@/lib/server-fetch";

type DiscoveryDocument = {
  authorization_endpoint?: string;
  token_endpoint?: string;
  end_session_endpoint?: string;
  revocation_endpoint?: string;
};

let discoveryPromise: Promise<DiscoveryDocument | null> | null = null;

export function normalizeIssuer(issuer: string): string {
  return issuer.replace(/\/+$/, "");
}

export function getWellKnownUrl(): string {
  return env.metadataAddress;
}

export async function getDiscoveryDocument(): Promise<DiscoveryDocument | null> {
  if (!discoveryPromise) {
    discoveryPromise = hclCsFetch(getWellKnownUrl(), {
      method: "GET",
      cache: "force-cache"
    })
      .then(async (response) => {
        if (!response.ok) {
          return null;
        }

        return (await response.json()) as DiscoveryDocument;
      })
      .catch(() => null);
  }

  return discoveryPromise;
}

export async function resolveTokenEndpoint(): Promise<string> {
  if (env.tokenEndpoint) {
    return env.tokenEndpoint;
  }

  const discovery = await getDiscoveryDocument();
  if (discovery?.token_endpoint) {
    return discovery.token_endpoint;
  }

  return `${normalizeIssuer(env.issuer)}/security/token`;
}

export async function resolveEndSessionEndpoint(): Promise<string> {
  const discovery = await getDiscoveryDocument();
  if (discovery?.end_session_endpoint) {
    return discovery.end_session_endpoint;
  }

  return `${normalizeIssuer(env.issuer)}/security/endsession`;
}

export async function resolveRevocationEndpoint(): Promise<string> {
  if (env.revocationEndpoint) {
    return env.revocationEndpoint;
  }

  const discovery = await getDiscoveryDocument();
  if (discovery?.revocation_endpoint) {
    return discovery.revocation_endpoint;
  }

  return `${normalizeIssuer(env.issuer)}/security/revocation`;
}
