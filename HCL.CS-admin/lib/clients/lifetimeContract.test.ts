/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

import assert from "node:assert/strict";
import test from "node:test";

import {
  LIFETIME_DEFAULTS,
  LIFETIME_LIMITS,
  isLifetimeInRange,
  lifetimeRangeMessage
} from "./lifetimeContract.ts";

/**
 * Canonical contract, duplicated here on purpose as an independent expectation.
 * It MUST equal the backend defaults in
 *   src/Identity/HCL.CS.Identity.Domain/Configurations/Endpoint/TokenConfig.cs (TokenExpiration)
 *   src/Identity/HCL.CS.Identity.Domain/Models/Endpoint/ClientsModel.cs (defaults)
 * and the Installer seed in
 *   installer/HCL.CS.Installer.Mvc/Infrastructure/Seeding/HclCsMasterDataSeed.cs (CreateClientMaster).
 * If these drift, this test fails and the fix must be applied on every layer, not just here.
 */
const EXPECTED_LIMITS = {
  accessToken: { min: 60, max: 900 },
  refreshToken: { min: 300, max: 86400 },
  identityToken: { min: 60, max: 3600 },
  logoutToken: { min: 1800, max: 86400 },
  authorizationCode: { min: 60, max: 600 }
};

const EXPECTED_DEFAULTS = {
  accessToken: 900,
  refreshToken: 86400,
  identityToken: 3600,
  logoutToken: 1800,
  authorizationCode: 600
};

// The exact values a fresh Installer-created Admin client is seeded with
// (installer/.../HclCsMasterDataSeed.cs -> CreateClientMaster).
const INSTALLER_SEED = {
  accessToken: 900,
  refreshToken: 86400,
  identityToken: 3600,
  logoutToken: 1800,
  authorizationCode: 600
};

// The old, buggy Installer seed that triggered "Validation failed for client update."
const LEGACY_BROKEN_SEED = {
  accessToken: 3600,
  authorizationCode: 1800
};

test("contract limits match the canonical backend/Installer contract (drift guard)", () => {
  assert.deepEqual(LIFETIME_LIMITS, EXPECTED_LIMITS);
});

test("contract defaults match the canonical backend/Installer defaults (drift guard)", () => {
  assert.deepEqual(LIFETIME_DEFAULTS, EXPECTED_DEFAULTS);
});

test("every default lifetime is within its own contract range", () => {
  for (const field of Object.keys(LIFETIME_DEFAULTS) as (keyof typeof LIFETIME_DEFAULTS)[]) {
    assert.equal(isLifetimeInRange(field, LIFETIME_DEFAULTS[field]), true, `${field} default out of range`);
  }
});

test("a fresh Installer-created Admin client passes the Admin update contract unchanged", () => {
  // Loading the seeded client and submitting it (e.g. after adding hcl-cs.client.manage)
  // must not fail on any lifetime field.
  assert.equal(isLifetimeInRange("accessToken", INSTALLER_SEED.accessToken), true);
  assert.equal(isLifetimeInRange("refreshToken", INSTALLER_SEED.refreshToken), true);
  assert.equal(isLifetimeInRange("identityToken", INSTALLER_SEED.identityToken), true);
  assert.equal(isLifetimeInRange("logoutToken", INSTALLER_SEED.logoutToken), true);
  assert.equal(isLifetimeInRange("authorizationCode", INSTALLER_SEED.authorizationCode), true);
});

test("the legacy broken Installer seed would be rejected (documents the fixed bug)", () => {
  assert.equal(isLifetimeInRange("accessToken", LEGACY_BROKEN_SEED.accessToken), false);
  assert.equal(isLifetimeInRange("authorizationCode", LEGACY_BROKEN_SEED.authorizationCode), false);
});

test("out-of-range and non-finite lifetimes are still rejected", () => {
  assert.equal(isLifetimeInRange("accessToken", LIFETIME_LIMITS.accessToken.min - 1), false);
  assert.equal(isLifetimeInRange("accessToken", LIFETIME_LIMITS.accessToken.max + 1), false);
  assert.equal(isLifetimeInRange("refreshToken", Number.NaN), false);
  assert.equal(isLifetimeInRange("authorizationCode", LIFETIME_LIMITS.authorizationCode.max + 1), false);
});

test("boundary values are accepted (inclusive range)", () => {
  assert.equal(isLifetimeInRange("accessToken", LIFETIME_LIMITS.accessToken.min), true);
  assert.equal(isLifetimeInRange("accessToken", LIFETIME_LIMITS.accessToken.max), true);
  assert.equal(isLifetimeInRange("logoutToken", LIFETIME_LIMITS.logoutToken.min), true);
});

test("lifetimeRangeMessage produces the friendly first-error message the update path now surfaces", () => {
  assert.equal(lifetimeRangeMessage("accessToken"), "Access token lifetime must be between 60 and 900 seconds.");
  assert.equal(lifetimeRangeMessage("refreshToken"), "Refresh token lifetime must be between 300 and 86400 seconds.");
  assert.equal(lifetimeRangeMessage("identityToken"), "Identity token lifetime must be between 60 and 3600 seconds.");
  assert.equal(lifetimeRangeMessage("logoutToken"), "Logout token lifetime must be between 1800 and 86400 seconds.");
  assert.equal(
    lifetimeRangeMessage("authorizationCode"),
    "Authorization code lifetime must be between 60 and 600 seconds."
  );
});
