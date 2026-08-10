/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

import { ApiRoutes } from "@/lib/api/routes";
import { hclCsPostWithSession } from "@/lib/api/client";
import {
  type FrameworkResult,
  type RoleClaimModel,
  type RoleModel,
  type UserModel,
  type UUID
} from "@/lib/types/hcl-cs";

export async function listRoles(): Promise<RoleModel[]> {
  return hclCsPostWithSession<RoleModel[]>(ApiRoutes.role.getAllRoles, "");
}

export async function getRole(roleId: UUID): Promise<RoleModel> {
  return hclCsPostWithSession<RoleModel, UUID>(ApiRoutes.role.getRoleById, roleId);
}

export async function createRole(role: RoleModel): Promise<FrameworkResult> {
  return hclCsPostWithSession<FrameworkResult, RoleModel>(ApiRoutes.role.createRole, role);
}

export async function updateRole(role: RoleModel): Promise<FrameworkResult> {
  return hclCsPostWithSession<FrameworkResult, RoleModel>(ApiRoutes.role.updateRole, role);
}

export async function deleteRole(roleId: UUID): Promise<FrameworkResult> {
  return hclCsPostWithSession<FrameworkResult, UUID>(ApiRoutes.role.deleteRoleById, roleId);
}

export async function deleteRoleByName(roleName: string): Promise<FrameworkResult> {
  return hclCsPostWithSession<FrameworkResult, string>(ApiRoutes.role.deleteRoleByName, roleName);
}

export async function addRoleClaim(model: RoleClaimModel): Promise<FrameworkResult> {
  return hclCsPostWithSession<FrameworkResult, RoleClaimModel>(ApiRoutes.role.addRoleClaim, model);
}

export async function addRoleClaims(models: RoleClaimModel[]): Promise<FrameworkResult> {
  return hclCsPostWithSession<FrameworkResult, RoleClaimModel[]>(ApiRoutes.role.addRoleClaimList, models);
}

export async function getRoleByName(roleName: string): Promise<RoleModel> {
  return hclCsPostWithSession<RoleModel, string>(ApiRoutes.role.getRoleByName, roleName);
}

export async function getRoleClaim(roleId: UUID): Promise<RoleClaimModel[]> {
  return hclCsPostWithSession<RoleClaimModel[], UUID>(ApiRoutes.role.getRoleClaim, roleId);
}

export async function removeRoleClaim(claimId: number): Promise<FrameworkResult> {
  return hclCsPostWithSession<FrameworkResult, number>(ApiRoutes.role.removeRoleClaimsById, claimId);
}

export async function removeRoleClaimByModel(model: RoleClaimModel): Promise<FrameworkResult> {
  return hclCsPostWithSession<FrameworkResult, RoleClaimModel>(ApiRoutes.role.removeRoleClaim, model);
}

export async function removeRoleClaims(models: RoleClaimModel[]): Promise<FrameworkResult> {
  return hclCsPostWithSession<FrameworkResult, RoleClaimModel[]>(ApiRoutes.role.removeRoleClaimsList, models);
}

export async function listUsersInRole(roleName: string): Promise<UserModel[]> {
  return hclCsPostWithSession<UserModel[], string>(ApiRoutes.user.getUsersInRole, roleName);
}
