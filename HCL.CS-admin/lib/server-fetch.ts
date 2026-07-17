import "server-only";

import { Agent, type Dispatcher } from "undici";

import { env } from "@/lib/env";

type HclCsFetchInit = RequestInit & {
  dispatcher?: Dispatcher;
};

const insecureDispatcher: Dispatcher | undefined = env.allowInsecureTls
  ? new Agent({
      connect: {
        rejectUnauthorized: false
      }
    })
  : undefined;

function normalizeOrigin(value: string | undefined): string | null {
  if (!value) {
    return null;
  }

  try {
    return new URL(value).origin;
  } catch {
    return null;
  }
}

const insecureOrigins = new Set(
  [
    env.issuer,
    env.metadataAddress,
    env.apiBaseUrl,
    env.installerBaseUrl,
    env.demoServerBaseUrl
  ]
    .map(normalizeOrigin)
    .filter((value): value is string => Boolean(value))
);

function resolveRequestUrl(input: RequestInfo | URL): URL | null {
  if (typeof input === "string") {
    try {
      return new URL(input);
    } catch {
      return null;
    }
  }

  if (input instanceof URL) {
    return input;
  }

  try {
    return new URL(input.url);
  } catch {
    return null;
  }
}

function shouldUseInsecureDispatcher(input: RequestInfo | URL): boolean {
  if (!env.allowInsecureTls || !insecureDispatcher) {
    return false;
  }

  const url = resolveRequestUrl(input);
  if (!url || url.protocol !== "https:") {
    return false;
  }

  return insecureOrigins.has(url.origin);
}

export function withHclCsTls(init: RequestInit = {}, input?: RequestInfo | URL): HclCsFetchInit {
  if (!input || !shouldUseInsecureDispatcher(input)) {
    return init as HclCsFetchInit;
  }

  return {
    ...init,
    dispatcher: insecureDispatcher
  };
}

export function hclCsFetch(input: RequestInfo | URL, init: RequestInit = {}) {
  return fetch(input, withHclCsTls(init, input));
}
