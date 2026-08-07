/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

import { isHardSessionError } from "../auth-errors.ts";
import { hasAdminRole } from "./roles.ts";

type AuthorizationToken = {
  accessToken?: unknown;
  roles?: unknown;
  error?: unknown;
};

export function isAuthorizedAdminToken(token: AuthorizationToken | null): boolean {
  if (!token || typeof token.accessToken !== "string" || token.accessToken.length === 0) {
    return false;
  }

  if (isHardSessionError(token.error)) {
    return false;
  }

  const roles = Array.isArray(token.roles) ? token.roles.map(String) : [];
  return hasAdminRole(roles);
}
