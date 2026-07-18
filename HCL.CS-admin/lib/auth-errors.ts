/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

export const SESSION_ERROR_INVALIDATED = "SessionInvalidated";
export const SESSION_ERROR_REFRESH_FAILED = "SessionRefreshFailed";

const hardSessionErrors = new Set([
  SESSION_ERROR_INVALIDATED,
  SESSION_ERROR_REFRESH_FAILED,
  "RefreshAccessTokenError",
  "Refresh token validation failed.",
  "MissingRefreshToken"
]);

export function isHardSessionError(error: unknown): boolean {
  return typeof error === "string" && hardSessionErrors.has(error);
}
