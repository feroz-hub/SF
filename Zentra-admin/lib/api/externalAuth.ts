import { ApiRoutes } from "@/lib/api/routes";
import { zentraPostWithSession } from "@/lib/api/client";
import {
  type ExternalAuthProviderConfigModel,
  type ExternalAuthFieldDefinitionsResponse,
  type SaveExternalAuthProviderRequest,
  type DeleteExternalAuthProviderRequest,
  type TestExternalAuthProviderRequest,
  type FrameworkResult
} from "@/lib/types/zentra";

export async function getAllExternalAuthProviders(): Promise<ExternalAuthProviderConfigModel[]> {
  const result = await zentraPostWithSession<
    ExternalAuthProviderConfigModel[],
    Record<string, never>
  >(ApiRoutes.externalAuth.getAllProviders, {});
  return result ?? [];
}

export async function getExternalAuthProvider(
  id: string
): Promise<ExternalAuthProviderConfigModel | null> {
  return zentraPostWithSession<ExternalAuthProviderConfigModel | null, { Id: string }>(
    ApiRoutes.externalAuth.getProvider,
    { Id: id }
  );
}

export async function saveExternalAuthProvider(
  request: SaveExternalAuthProviderRequest
): Promise<FrameworkResult | null> {
  return zentraPostWithSession<FrameworkResult | null, SaveExternalAuthProviderRequest>(
    ApiRoutes.externalAuth.saveProvider,
    request
  );
}

export async function deleteExternalAuthProvider(
  request: DeleteExternalAuthProviderRequest
): Promise<FrameworkResult | null> {
  return zentraPostWithSession<FrameworkResult | null, DeleteExternalAuthProviderRequest>(
    ApiRoutes.externalAuth.deleteProvider,
    request
  );
}

export async function testExternalAuthProvider(
  request: TestExternalAuthProviderRequest
): Promise<FrameworkResult | null> {
  return zentraPostWithSession<FrameworkResult | null, TestExternalAuthProviderRequest>(
    ApiRoutes.externalAuth.testProvider,
    request
  );
}

export async function getExternalAuthFieldDefinitions(): Promise<ExternalAuthFieldDefinitionsResponse | null> {
  return zentraPostWithSession<ExternalAuthFieldDefinitionsResponse | null, Record<string, never>>(
    ApiRoutes.externalAuth.getFieldDefinitions,
    {}
  );
}
