import { ApiRoutes } from "@/lib/api/routes";
import { hclCsPostWithSession } from "@/lib/api/client";
import { type FrameworkResult, type IdentityClaimsModel, type IdentityResourcesModel, type UUID } from "@/lib/types/hcl-cs";

export async function listIdentityResources(): Promise<IdentityResourcesModel[]> {
  return hclCsPostWithSession<IdentityResourcesModel[]>(ApiRoutes.identityResource.getAllIdentityResources, "");
}

export async function getIdentityResourceById(id: UUID): Promise<IdentityResourcesModel> {
  return hclCsPostWithSession<IdentityResourcesModel, UUID>(ApiRoutes.identityResource.getIdentityResourceById, id);
}

export async function getIdentityResourceByName(name: string): Promise<IdentityResourcesModel> {
  return hclCsPostWithSession<IdentityResourcesModel, string>(ApiRoutes.identityResource.getIdentityResourceByName, name);
}

export async function createIdentityResource(model: IdentityResourcesModel): Promise<FrameworkResult> {
  return hclCsPostWithSession<FrameworkResult, IdentityResourcesModel>(ApiRoutes.identityResource.addIdentityResource, model);
}

export async function updateIdentityResource(model: IdentityResourcesModel): Promise<FrameworkResult> {
  return hclCsPostWithSession<FrameworkResult, IdentityResourcesModel>(ApiRoutes.identityResource.updateIdentityResource, model);
}

export async function deleteIdentityResourceById(id: UUID): Promise<FrameworkResult> {
  return hclCsPostWithSession<FrameworkResult, UUID>(ApiRoutes.identityResource.deleteIdentityResourceById, id);
}

export async function deleteIdentityResourceByName(name: string): Promise<FrameworkResult> {
  return hclCsPostWithSession<FrameworkResult, string>(ApiRoutes.identityResource.deleteIdentityResourceByName, name);
}

export async function listIdentityResourceClaims(resourceId: UUID): Promise<IdentityClaimsModel[]> {
  return hclCsPostWithSession<IdentityClaimsModel[], UUID>(ApiRoutes.identityResource.getIdentityResourceClaims, resourceId);
}

export async function addIdentityResourceClaim(model: IdentityClaimsModel): Promise<FrameworkResult> {
  return hclCsPostWithSession<FrameworkResult, IdentityClaimsModel>(ApiRoutes.identityResource.addIdentityResourceClaim, model);
}

export async function deleteIdentityResourceClaimById(id: UUID): Promise<FrameworkResult> {
  return hclCsPostWithSession<FrameworkResult, UUID>(ApiRoutes.identityResource.deleteIdentityResourceClaimById, id);
}

export async function deleteIdentityResourceClaimByResourceId(resourceId: UUID): Promise<FrameworkResult> {
  return hclCsPostWithSession<FrameworkResult, UUID>(
    ApiRoutes.identityResource.deleteIdentityResourceClaimByResourceIdAsync,
    resourceId
  );
}

export async function deleteIdentityResourceClaimModel(model: IdentityClaimsModel): Promise<FrameworkResult> {
  return hclCsPostWithSession<FrameworkResult, IdentityClaimsModel>(ApiRoutes.identityResource.deleteIdentityResourceClaimModel, model);
}

