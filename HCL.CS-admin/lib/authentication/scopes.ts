/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

export const ADMIN_IDENTITY_SCOPES = ["openid", "profile", "email", "phone", "offline_access"] as const;

export const ADMIN_PERMISSION_SCOPES = [
  "hcl-cs.apiresource.read",
  "hcl-cs.apiresource.write",
  "hcl-cs.apiresource.manage",
  "hcl-cs.apiresource.delete",
  "hcl-cs.identityresource.read",
  "hcl-cs.identityresource.write",
  "hcl-cs.identityresource.manage",
  "hcl-cs.identityresource.delete",
  "hcl-cs.client.read",
  "hcl-cs.client.write",
  "hcl-cs.client.manage",
  "hcl-cs.client.delete",
  "hcl-cs.user.read",
  "hcl-cs.user.write",
  "hcl-cs.user.manage",
  "hcl-cs.user.delete",
  "hcl-cs.role.read",
  "hcl-cs.role.write",
  "hcl-cs.role.manage",
  "hcl-cs.role.delete",
  "hcl-cs.adminuser.read",
  "hcl-cs.adminuser.write",
  "hcl-cs.adminuser.manage",
  "hcl-cs.adminuser.delete",
  "hcl-cs.securitytoken.read",
  "hcl-cs.securitytoken.manage"
] as const;

export const DEFAULT_HCL_CS_SCOPES = [...ADMIN_IDENTITY_SCOPES, ...ADMIN_PERMISSION_SCOPES].join(" ");

const legacyScopeReplacements: Readonly<Record<string, readonly string[]>> = {
  "hcl-cs.apiresource": ADMIN_PERMISSION_SCOPES.filter((scope) => scope.startsWith("hcl-cs.apiresource.")),
  "hcl-cs.identityresource": ADMIN_PERMISSION_SCOPES.filter((scope) =>
    scope.startsWith("hcl-cs.identityresource.")
  ),
  "hcl-cs.client": ADMIN_PERMISSION_SCOPES.filter((scope) => scope.startsWith("hcl-cs.client.")),
  "hcl-cs.user": ADMIN_PERMISSION_SCOPES.filter((scope) => scope.startsWith("hcl-cs.user.")),
  "hcl-cs.role": ADMIN_PERMISSION_SCOPES.filter((scope) => scope.startsWith("hcl-cs.role.")),
  "hcl-cs.adminuser": ADMIN_PERMISSION_SCOPES.filter((scope) => scope.startsWith("hcl-cs.adminuser.")),
  "hcl-cs.securitytoken": ADMIN_PERMISSION_SCOPES.filter((scope) =>
    scope.startsWith("hcl-cs.securitytoken.")
  )
};

/**
 * Keeps required OIDC/refresh scopes present and migrates legacy Admin umbrella
 * configuration to the exact granular permissions advertised by HCL.CS.
 */
export function normalizeAuthScopes(value: string): string {
  const scopes: string[] = [];
  const add = (scope: string) => {
    if (scope && !scopes.includes(scope)) {
      scopes.push(scope);
    }
  };

  for (const scope of value.split(/\s+/).filter(Boolean)) {
    const replacement = legacyScopeReplacements[scope];
    if (replacement) {
      replacement.forEach(add);
    } else {
      add(scope);
    }
  }

  if (!scopes.includes("openid")) {
    scopes.unshift("openid");
  }
  add("offline_access");
  return scopes.join(" ");
}
