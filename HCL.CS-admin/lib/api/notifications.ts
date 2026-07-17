import { ApiRoutes } from "@/lib/api/routes";
import { hclCsPostWithSession } from "@/lib/api/client";
import {
  type NotificationLogResponseModel,
  type NotificationSearchRequestModel,
  type NotificationTemplateResponseModel,
  type ProviderConfigModel,
  type ProviderFieldDefinitionsResponse,
  type SaveProviderConfigRequest,
  type SetActiveProviderRequest,
  type DeleteProviderConfigRequest,
  type SendTestNotificationRequest,
  type FrameworkResult,
  type PagingModel
} from "@/lib/types/hcl-cs";

export type NotificationLogQueryInput = {
  type?: number | null;
  status?: number | null;
  fromDate?: string;
  toDate?: string;
  searchValue?: string;
  page?: PagingModel;
};

export function buildNotificationSearchRequest(
  input: NotificationLogQueryInput
): NotificationSearchRequestModel {
  const page = input.page ?? {
    TotalItems: 0,
    ItemsPerPage: 20,
    CurrentPage: 1,
    TotalPages: 0,
    TotalDisplayPages: 10
  };

  return {
    Type: input.type ?? null,
    Status: input.status ?? null,
    FromDate: input.fromDate?.trim() || null,
    ToDate: input.toDate?.trim() || null,
    SearchValue: input.searchValue ?? "",
    Page: page
  };
}

export async function searchNotificationLogs(
  input: NotificationLogQueryInput
): Promise<NotificationLogResponseModel | null> {
  const payload = buildNotificationSearchRequest(input);
  const result = await hclCsPostWithSession<
    NotificationLogResponseModel | null,
    NotificationSearchRequestModel
  >(ApiRoutes.notification.getNotificationLogs, payload);
  return result ?? null;
}

export async function getNotificationTemplates(): Promise<NotificationTemplateResponseModel | null> {
  return hclCsPostWithSession<NotificationTemplateResponseModel | null, Record<string, never>>(
    ApiRoutes.notification.getNotificationTemplates,
    {}
  );
}

export async function getAllProviderConfigs(): Promise<ProviderConfigModel[]> {
  const result = await hclCsPostWithSession<ProviderConfigModel[], Record<string, never>>(
    ApiRoutes.notification.getAllProviderConfigs,
    {}
  );
  return result ?? [];
}

export async function saveProviderConfig(
  request: SaveProviderConfigRequest
): Promise<FrameworkResult | null> {
  return hclCsPostWithSession<FrameworkResult | null, SaveProviderConfigRequest>(
    ApiRoutes.notification.saveProviderConfig,
    request
  );
}

export async function setActiveProvider(
  request: SetActiveProviderRequest
): Promise<FrameworkResult | null> {
  return hclCsPostWithSession<FrameworkResult | null, SetActiveProviderRequest>(
    ApiRoutes.notification.setActiveProvider,
    request
  );
}

export async function deleteProviderConfig(
  request: DeleteProviderConfigRequest
): Promise<FrameworkResult | null> {
  return hclCsPostWithSession<FrameworkResult | null, DeleteProviderConfigRequest>(
    ApiRoutes.notification.deleteProviderConfig,
    request
  );
}

export async function getProviderFieldDefinitions(): Promise<ProviderFieldDefinitionsResponse | null> {
  return hclCsPostWithSession<ProviderFieldDefinitionsResponse | null, Record<string, never>>(
    ApiRoutes.notification.getProviderFieldDefinitions,
    {}
  );
}

export async function sendTestNotification(
  request: SendTestNotificationRequest
): Promise<FrameworkResult | null> {
  return hclCsPostWithSession<FrameworkResult | null, SendTestNotificationRequest>(
    ApiRoutes.notification.sendTestNotification,
    request
  );
}
