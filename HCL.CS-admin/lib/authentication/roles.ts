/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

export function decodeJwtPayload(token?: string): Record<string, unknown> {
  if (!token) {
    return {};
  }

  const sections = token.split(".");
  if (sections.length < 2) {
    return {};
  }

  try {
    const payload = sections[1].replace(/-/g, "+").replace(/_/g, "/");
    const json = Buffer.from(payload, "base64").toString("utf8");
    return JSON.parse(json) as Record<string, unknown>;
  } catch {
    return {};
  }
}

function coerceRoleValues(value: unknown): string[] {
  if (Array.isArray(value)) {
    return value.map((entry) => String(entry)).filter(Boolean);
  }

  if (typeof value === "string") {
    return value
      .split(/[\s,]+/)
      .map((entry) => entry.trim())
      .filter(Boolean);
  }

  return [];
}

export function extractRolesFromToken(accessToken?: string): string[] {
  const payload = decodeJwtPayload(accessToken);
  const roleCandidates = [
    payload.role,
    payload.roles,
    payload.userrole,
    payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"]
  ];

  const roles = roleCandidates.flatMap(coerceRoleValues);
  return [...new Set(roles.map((role) => role.trim()).filter(Boolean))];
}

export function hasAdminRole(roles: string[]): boolean {
  return roles.some((role) => role.toLowerCase().includes("admin"));
}
