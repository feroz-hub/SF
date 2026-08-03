/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

/**
 * Single source of truth for OAuth/OIDC client token-lifetime bounds and defaults used by the
 * Admin client editor (both the server action validation and the React form).
 *
 * These MUST stay in lockstep with the backend canonical contract:
 *   - src/Identity/HCL.CS.Identity.Domain/Configurations/Endpoint/TokenConfig.cs (class TokenExpiration defaults)
 *   - src/Identity/HCL.CS.Identity.Domain/Models/Endpoint/ClientsModel.cs (property defaults)
 *   - installer/HCL.CS.Installer.Mvc/Infrastructure/Seeding/HclCsMasterDataSeed.cs (CreateClientMaster seed)
 *
 * A freshly Installer-created Admin client is seeded with LIFETIME_DEFAULTS, every value of which is
 * within LIFETIME_LIMITS, so the client can be opened and updated in the Admin UI without editing
 * unrelated lifetime fields. See lib/clients/lifetimeContract.test.ts for the drift guard.
 */

export type LifetimeField =
  | "accessToken"
  | "refreshToken"
  | "identityToken"
  | "logoutToken"
  | "authorizationCode";

export type LifetimeRange = { min: number; max: number };

/** Inclusive [min, max] second bounds accepted for each token lifetime. */
export const LIFETIME_LIMITS: Record<LifetimeField, LifetimeRange> = {
  accessToken: { min: 60, max: 900 },
  refreshToken: { min: 300, max: 86400 },
  identityToken: { min: 60, max: 3600 },
  logoutToken: { min: 1800, max: 86400 },
  authorizationCode: { min: 60, max: 600 }
};

/**
 * Canonical defaults for a freshly-created client (Installer bootstrap client and the Admin
 * "new client" form). Every value is within LIFETIME_LIMITS.
 */
export const LIFETIME_DEFAULTS: Record<LifetimeField, number> = {
  accessToken: 900,
  refreshToken: 86400,
  identityToken: 3600,
  logoutToken: 1800,
  authorizationCode: 600
};

/** Human-readable field labels used in validation messages. */
export const LIFETIME_LABELS: Record<LifetimeField, string> = {
  accessToken: "Access token",
  refreshToken: "Refresh token",
  identityToken: "Identity token",
  logoutToken: "Logout token",
  authorizationCode: "Authorization code"
};

/** Builds the friendly range message, e.g. "Access token lifetime must be between 60 and 900 seconds." */
export function lifetimeRangeMessage(field: LifetimeField): string {
  const { min, max } = LIFETIME_LIMITS[field];
  return `${LIFETIME_LABELS[field]} lifetime must be between ${min} and ${max} seconds.`;
}

/** True when value is a finite number within the inclusive contract bounds for the field. */
export function isLifetimeInRange(field: LifetimeField, value: number): boolean {
  const { min, max } = LIFETIME_LIMITS[field];
  return Number.isFinite(value) && value >= min && value <= max;
}
