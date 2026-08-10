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
  type ExternalAuthProviderConfigModel,
  type ExternalAuthFieldDefinitionsResponse,
  type SaveExternalAuthProviderRequest,
  type DeleteExternalAuthProviderRequest,
  type TestExternalAuthProviderRequest,
  type FrameworkResult
} from "@/lib/types/hcl-cs";

export async function getAllExternalAuthProviders(): Promise<ExternalAuthProviderConfigModel[]> {
  const result = await hclCsPostWithSession<
    ExternalAuthProviderConfigModel[],
    Record<string, never>
  >(ApiRoutes.externalAuth.getAllProviders, {});
  return result ?? [];
}

export async function getExternalAuthProvider(
  id: string
): Promise<ExternalAuthProviderConfigModel | null> {
  return hclCsPostWithSession<ExternalAuthProviderConfigModel | null, { Id: string }>(
    ApiRoutes.externalAuth.getProvider,
    { Id: id }
  );
}

export async function saveExternalAuthProvider(
  request: SaveExternalAuthProviderRequest
): Promise<FrameworkResult | null> {
  return hclCsPostWithSession<FrameworkResult | null, SaveExternalAuthProviderRequest>(
    ApiRoutes.externalAuth.saveProvider,
    request
  );
}

export async function deleteExternalAuthProvider(
  request: DeleteExternalAuthProviderRequest
): Promise<FrameworkResult | null> {
  return hclCsPostWithSession<FrameworkResult | null, DeleteExternalAuthProviderRequest>(
    ApiRoutes.externalAuth.deleteProvider,
    request
  );
}

export async function testExternalAuthProvider(
  request: TestExternalAuthProviderRequest
): Promise<FrameworkResult | null> {
  return hclCsPostWithSession<FrameworkResult | null, TestExternalAuthProviderRequest>(
    ApiRoutes.externalAuth.testProvider,
    request
  );
}

export async function getExternalAuthFieldDefinitions(): Promise<ExternalAuthFieldDefinitionsResponse | null> {
  return hclCsPostWithSession<ExternalAuthFieldDefinitionsResponse | null, Record<string, never>>(
    ApiRoutes.externalAuth.getFieldDefinitions,
    {}
  );
}
