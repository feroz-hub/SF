/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

import { RolesModule } from "@/components/modules/roles/RolesModule";
import { getLoadErrorInfo } from "@/lib/api/client";
import { getRole, listRoles, listUsersInRole } from "@/lib/api/roles";
import { type RoleModel, type UserModel } from "@/lib/types/hcl-cs";

export type RoleRow = {
  role: RoleModel;
  claimsCount: number;
  usersCount: number;
  usersInRole: UserModel[];
};

export default async function RolesPage() {
  let rows: RoleRow[] = [];
  let loadError: string | null = null;
  let loadErrorIsUnauthorized = false;

  try {
    const rolesRaw = await listRoles();
    const roles = Array.isArray(rolesRaw) ? rolesRaw : [];
    rows = await Promise.all(
      roles.map(async (role) => {
        const [fullRole, usersInRoleRaw] = await Promise.all([
          getRole(role.Id).catch(() => role),
          listUsersInRole(role.Name).catch(() => [])
        ]);
        const usersInRole: UserModel[] = Array.isArray(usersInRoleRaw) ? usersInRoleRaw : [];
        return {
          role: fullRole,
          claimsCount: fullRole?.RoleClaims?.length ?? 0,
          usersCount: usersInRole.length,
          usersInRole
        };
      })
    );
  } catch (error) {
    const info = getLoadErrorInfo(error);
    loadError = info.message;
    loadErrorIsUnauthorized = info.isUnauthorized;
  }

  return <RolesModule rows={rows} loadError={loadError ?? undefined} loadErrorIsUnauthorized={loadErrorIsUnauthorized} />;
}
