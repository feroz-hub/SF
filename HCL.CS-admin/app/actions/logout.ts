/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

"use server";

import { signOut as hclCsSignOut } from "@/lib/api/authentication";
import { revokeToken } from "@/lib/api/revocation";
import { auth } from "@/lib/auth";

/**
 * Performs server-side logout with HCL.CS: calls SignOut API and revokes the
 * current access and refresh tokens so they cannot be reused. Errors are
 * swallowed so the client can always clear the session and redirect.
 */
export async function logoutAction(): Promise<void> {
  const session = await auth();

  if (!session?.accessToken) {
    return;
  }

  try {
    await hclCsSignOut();
  } catch {
    // Continue; client will still clear session.
  }

  try {
    await revokeToken(session.accessToken, "access_token");
  } catch {
    // Continue.
  }

  if (session.refreshToken) {
    try {
      await revokeToken(session.refreshToken, "refresh_token");
    } catch {
      // Continue.
    }
  }
}
