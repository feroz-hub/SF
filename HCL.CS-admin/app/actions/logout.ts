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
