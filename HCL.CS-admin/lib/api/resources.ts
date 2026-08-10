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
  type ApiResourceClaimsModel,
  type ApiResourcesModel,
  type ApiScopeClaimsModel,
  type ApiScopesModel,
  type FrameworkResult,
  type UUID
} from "@/lib/types/hcl-cs";

export async function listApiResources(): Promise<ApiResourcesModel[]> {
  return hclCsPostWithSession<ApiResourcesModel[]>(ApiRoutes.resource.getAllApiResources, "");
}

export async function listApiResourcesByScopes(payload: unknown): Promise<ApiResourcesModel[]> {
  return hclCsPostWithSession<ApiResourcesModel[], unknown>(ApiRoutes.resource.getAllApiResourcesByScopesAsync, payload);
}

export async function getApiResource(resourceId: UUID): Promise<ApiResourcesModel> {
  return hclCsPostWithSession<ApiResourcesModel, UUID>(ApiRoutes.resource.getApiResourceById, resourceId);
}

export async function getApiResourceByName(name: string): Promise<ApiResourcesModel> {
  return hclCsPostWithSession<ApiResourcesModel, string>(ApiRoutes.resource.getApiResourceByName, name);
}

export async function createApiResource(model: ApiResourcesModel): Promise<FrameworkResult> {
  return hclCsPostWithSession<FrameworkResult, ApiResourcesModel>(ApiRoutes.resource.addApiResource, model);
}

export async function updateApiResource(model: ApiResourcesModel): Promise<FrameworkResult> {
  return hclCsPostWithSession<FrameworkResult, ApiResourcesModel>(ApiRoutes.resource.updateApiResource, model);
}

export async function deleteApiResource(resourceId: UUID): Promise<FrameworkResult> {
  return hclCsPostWithSession<FrameworkResult, UUID>(ApiRoutes.resource.deleteApiResourceById, resourceId);
}

export async function deleteApiResourceByName(name: string): Promise<FrameworkResult> {
  return hclCsPostWithSession<FrameworkResult, string>(ApiRoutes.resource.deleteApiResourceByName, name);
}

export async function createApiScope(model: ApiScopesModel): Promise<FrameworkResult> {
  return hclCsPostWithSession<FrameworkResult, ApiScopesModel>(ApiRoutes.resource.addApiScope, model);
}

export async function updateApiScope(model: ApiScopesModel): Promise<FrameworkResult> {
  return hclCsPostWithSession<FrameworkResult, ApiScopesModel>(ApiRoutes.resource.updateApiScope, model);
}

export async function deleteApiScope(scopeId: UUID): Promise<FrameworkResult> {
  return hclCsPostWithSession<FrameworkResult, UUID>(ApiRoutes.resource.deleteApiScopeById, scopeId);
}

export async function getApiScope(scopeId: UUID): Promise<ApiScopesModel> {
  return hclCsPostWithSession<ApiScopesModel, UUID>(ApiRoutes.resource.getApiScopeById, scopeId);
}

export async function getApiScopeByName(name: string): Promise<ApiScopesModel> {
  return hclCsPostWithSession<ApiScopesModel, string>(ApiRoutes.resource.getApiScopeByName, name);
}

export async function listApiScopes(): Promise<ApiScopesModel[]> {
  return hclCsPostWithSession<ApiScopesModel[]>(ApiRoutes.resource.getAllApiScopes, "");
}

export async function deleteApiScopeByName(name: string): Promise<FrameworkResult> {
  return hclCsPostWithSession<FrameworkResult, string>(ApiRoutes.resource.deleteApiScopeByName, name);
}

export async function addApiResourceClaim(model: ApiResourceClaimsModel): Promise<FrameworkResult> {
  return hclCsPostWithSession<FrameworkResult, ApiResourceClaimsModel>(
    ApiRoutes.resource.addApiResourceClaim,
    model
  );
}

export async function deleteApiResourceClaim(claimId: UUID): Promise<FrameworkResult> {
  return hclCsPostWithSession<FrameworkResult, UUID>(ApiRoutes.resource.deleteApiResourceClaimById, claimId);
}

export async function deleteApiResourceClaimByResourceId(resourceId: UUID): Promise<FrameworkResult> {
  return hclCsPostWithSession<FrameworkResult, UUID>(ApiRoutes.resource.deleteApiResourceClaimByResourceIdAsync, resourceId);
}

export async function deleteApiResourceClaimModel(model: ApiResourceClaimsModel): Promise<FrameworkResult> {
  return hclCsPostWithSession<FrameworkResult, ApiResourceClaimsModel>(ApiRoutes.resource.deleteApiResourceClaimModel, model);
}

export async function addApiScopeClaim(model: ApiScopeClaimsModel): Promise<FrameworkResult> {
  return hclCsPostWithSession<FrameworkResult, ApiScopeClaimsModel>(ApiRoutes.resource.addApiScopeClaim, model);
}

export async function deleteApiScopeClaim(claimId: UUID): Promise<FrameworkResult> {
  return hclCsPostWithSession<FrameworkResult, UUID>(ApiRoutes.resource.deleteApiScopeClaimById, claimId);
}

export async function deleteApiScopeClaimByScopeId(scopeId: UUID): Promise<FrameworkResult> {
  return hclCsPostWithSession<FrameworkResult, UUID>(ApiRoutes.resource.deleteApiScopeClaimByScopeId, scopeId);
}

export async function deleteApiScopeClaimModel(model: ApiScopeClaimsModel): Promise<FrameworkResult> {
  return hclCsPostWithSession<FrameworkResult, ApiScopeClaimsModel>(ApiRoutes.resource.deleteApiScopeClaimModel, model);
}

export async function getApiResourceClaimsById(resourceId: UUID): Promise<ApiResourceClaimsModel[]> {
  return hclCsPostWithSession<ApiResourceClaimsModel[], UUID>(ApiRoutes.resource.getApiResourceClaimsById, resourceId);
}

export async function getApiScopeClaims(scopeId: UUID): Promise<ApiScopeClaimsModel[]> {
  return hclCsPostWithSession<ApiScopeClaimsModel[], UUID>(ApiRoutes.resource.getApiScopeClaims, scopeId);
}

