/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

import { UsersModule } from "@/components/modules/users/UsersModule";
import { getLoadErrorInfo } from "@/lib/api/client";
import { listUsers } from "@/lib/api/users";

export default async function UsersPage() {
  let rows: { id: string; userName: string; email: string; mfaType: string; enabled: boolean; lockedOut: boolean; createdAt: string | null }[] = [];
  let loadError: string | null = null;
  let loadErrorIsUnauthorized = false;

  try {
    const users = await listUsers();
    rows = users.map((user) => ({
      id: user.Id,
      userName: user.UserName,
      email: user.Email,
      mfaType: "—",
      enabled: !user.LockoutEnabled,
      lockedOut: user.LockoutEnabled,
      createdAt: user.CreatedOn ?? null
    }));
  } catch (error) {
    const info = getLoadErrorInfo(error);
    loadError = info.message;
    loadErrorIsUnauthorized = info.isUnauthorized;
  }

  return <UsersModule rows={rows} loadError={loadError ?? undefined} loadErrorIsUnauthorized={loadErrorIsUnauthorized} />;
}
