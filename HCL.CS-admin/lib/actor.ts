/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

import { MAX_LENGTH_255 } from "@/lib/constants";

/** Session-like shape used for actor resolution (avoids circular dependency on auth). */
export type SessionLike = { user?: { name?: string | null; email?: string | null } } | null;

/**
 * Returns the current actor name for audit fields (CreatedBy/ModifiedBy).
 * Truncated to server limit (255) to avoid validation errors.
 */
export function getActor(session: SessionLike, maxLength: number = MAX_LENGTH_255): string {
  const raw = session?.user?.name ?? session?.user?.email ?? "hcl-cs-admin";
  const s = typeof raw === "string" ? raw : "hcl-cs-admin";
  return s.length > maxLength ? s.slice(0, maxLength) : s;
}
