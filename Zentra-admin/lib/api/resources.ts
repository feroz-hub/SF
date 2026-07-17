import { ApiRoutes } from "@/lib/api/routes";
import { zentraPostWithSession } from "@/lib/api/client";
import {
  type ApiResourceClaimsModel,
  type ApiResourcesModel,
  type ApiScopeClaimsModel,
  type ApiScopesModel,
  type FrameworkResult,
  type UUID
} from "@/lib/types/zentra";

export async function listApiResources(): Promise<ApiResourcesModel[]> {
  return zentraPostWithSession<ApiResourcesModel[]>(ApiRoutes.resource.getAllApiResources, "");
}

export async function listApiResourcesByScopes(payload: unknown): Promise<ApiResourcesModel[]> {
  return zentraPostWithSession<ApiResourcesModel[], unknown>(ApiRoutes.resource.getAllApiResourcesByScopesAsync, payload);
}

export async function getApiResource(resourceId: UUID): Promise<ApiResourcesModel> {
  return zentraPostWithSession<ApiResourcesModel, UUID>(ApiRoutes.resource.getApiResourceById, resourceId);
}

export async function getApiResourceByName(name: string): Promise<ApiResourcesModel> {
  return zentraPostWithSession<ApiResourcesModel, string>(ApiRoutes.resource.getApiResourceByName, name);
}

export async function createApiResource(model: ApiResourcesModel): Promise<FrameworkResult> {
  return zentraPostWithSession<FrameworkResult, ApiResourcesModel>(ApiRoutes.resource.addApiResource, model);
}

export async function updateApiResource(model: ApiResourcesModel): Promise<FrameworkResult> {
  return zentraPostWithSession<FrameworkResult, ApiResourcesModel>(ApiRoutes.resource.updateApiResource, model);
}

export async function deleteApiResource(resourceId: UUID): Promise<FrameworkResult> {
  return zentraPostWithSession<FrameworkResult, UUID>(ApiRoutes.resource.deleteApiResourceById, resourceId);
}

export async function deleteApiResourceByName(name: string): Promise<FrameworkResult> {
  return zentraPostWithSession<FrameworkResult, string>(ApiRoutes.resource.deleteApiResourceByName, name);
}

export async function createApiScope(model: ApiScopesModel): Promise<FrameworkResult> {
  return zentraPostWithSession<FrameworkResult, ApiScopesModel>(ApiRoutes.resource.addApiScope, model);
}

export async function updateApiScope(model: ApiScopesModel): Promise<FrameworkResult> {
  return zentraPostWithSession<FrameworkResult, ApiScopesModel>(ApiRoutes.resource.updateApiScope, model);
}

export async function deleteApiScope(scopeId: UUID): Promise<FrameworkResult> {
  return zentraPostWithSession<FrameworkResult, UUID>(ApiRoutes.resource.deleteApiScopeById, scopeId);
}

export async function getApiScope(scopeId: UUID): Promise<ApiScopesModel> {
  return zentraPostWithSession<ApiScopesModel, UUID>(ApiRoutes.resource.getApiScopeById, scopeId);
}

export async function getApiScopeByName(name: string): Promise<ApiScopesModel> {
  return zentraPostWithSession<ApiScopesModel, string>(ApiRoutes.resource.getApiScopeByName, name);
}

export async function listApiScopes(): Promise<ApiScopesModel[]> {
  return zentraPostWithSession<ApiScopesModel[]>(ApiRoutes.resource.getAllApiScopes, "");
}

export async function deleteApiScopeByName(name: string): Promise<FrameworkResult> {
  return zentraPostWithSession<FrameworkResult, string>(ApiRoutes.resource.deleteApiScopeByName, name);
}

export async function addApiResourceClaim(model: ApiResourceClaimsModel): Promise<FrameworkResult> {
  return zentraPostWithSession<FrameworkResult, ApiResourceClaimsModel>(
    ApiRoutes.resource.addApiResourceClaim,
    model
  );
}

export async function deleteApiResourceClaim(claimId: UUID): Promise<FrameworkResult> {
  return zentraPostWithSession<FrameworkResult, UUID>(ApiRoutes.resource.deleteApiResourceClaimById, claimId);
}

export async function deleteApiResourceClaimByResourceId(resourceId: UUID): Promise<FrameworkResult> {
  return zentraPostWithSession<FrameworkResult, UUID>(ApiRoutes.resource.deleteApiResourceClaimByResourceIdAsync, resourceId);
}

export async function deleteApiResourceClaimModel(model: ApiResourceClaimsModel): Promise<FrameworkResult> {
  return zentraPostWithSession<FrameworkResult, ApiResourceClaimsModel>(ApiRoutes.resource.deleteApiResourceClaimModel, model);
}

export async function addApiScopeClaim(model: ApiScopeClaimsModel): Promise<FrameworkResult> {
  return zentraPostWithSession<FrameworkResult, ApiScopeClaimsModel>(ApiRoutes.resource.addApiScopeClaim, model);
}

export async function deleteApiScopeClaim(claimId: UUID): Promise<FrameworkResult> {
  return zentraPostWithSession<FrameworkResult, UUID>(ApiRoutes.resource.deleteApiScopeClaimById, claimId);
}

export async function deleteApiScopeClaimByScopeId(scopeId: UUID): Promise<FrameworkResult> {
  return zentraPostWithSession<FrameworkResult, UUID>(ApiRoutes.resource.deleteApiScopeClaimByScopeId, scopeId);
}

export async function deleteApiScopeClaimModel(model: ApiScopeClaimsModel): Promise<FrameworkResult> {
  return zentraPostWithSession<FrameworkResult, ApiScopeClaimsModel>(ApiRoutes.resource.deleteApiScopeClaimModel, model);
}

export async function getApiResourceClaimsById(resourceId: UUID): Promise<ApiResourceClaimsModel[]> {
  return zentraPostWithSession<ApiResourceClaimsModel[], UUID>(ApiRoutes.resource.getApiResourceClaimsById, resourceId);
}

export async function getApiScopeClaims(scopeId: UUID): Promise<ApiScopeClaimsModel[]> {
  return zentraPostWithSession<ApiScopeClaimsModel[], UUID>(ApiRoutes.resource.getApiScopeClaims, scopeId);
}

