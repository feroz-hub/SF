/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

import { NotificationsModule } from "@/components/modules/notifications/NotificationsModule";
import { getLoadErrorInfo } from "@/lib/api/client";
import {
  searchNotificationLogs,
  getAllProviderConfigs,
  getProviderFieldDefinitions,
  getNotificationTemplates
} from "@/lib/api/notifications";
import {
  type NotificationLogResponseModel,
  type ProviderConfigModel,
  type ProviderFieldDefinitionsResponse,
  type NotificationTemplateResponseModel
} from "@/lib/types/hcl-cs";

const emptyLogData: NotificationLogResponseModel = {
  Notifications: [],
  PageInfo: {
    TotalItems: 0,
    ItemsPerPage: 20,
    CurrentPage: 1,
    TotalPages: 0,
    TotalDisplayPages: 0
  }
};

export default async function NotificationsPage() {
  let logData: NotificationLogResponseModel = emptyLogData;
  let providerConfigs: ProviderConfigModel[] = [];
  let fieldDefinitions: ProviderFieldDefinitionsResponse | null = null;
  let templates: NotificationTemplateResponseModel | null = null;
  let loadError: string | null = null;
  let loadErrorIsUnauthorized = false;

  try {
    const [logsResult, configsResult, fieldsResult, templatesResult] = await Promise.allSettled([
      searchNotificationLogs({
        page: {
          TotalItems: 0,
          ItemsPerPage: 20,
          CurrentPage: 1,
          TotalPages: 0,
          TotalDisplayPages: 10
        }
      }),
      getAllProviderConfigs(),
      getProviderFieldDefinitions(),
      getNotificationTemplates()
    ]);

    logData = logsResult.status === "fulfilled" ? (logsResult.value ?? emptyLogData) : emptyLogData;
    providerConfigs = configsResult.status === "fulfilled" ? (configsResult.value ?? []) : [];
    fieldDefinitions = fieldsResult.status === "fulfilled" ? fieldsResult.value : null;
    templates = templatesResult.status === "fulfilled" ? templatesResult.value : null;

    const failed = [logsResult, configsResult, fieldsResult, templatesResult]
      .filter((r): r is PromiseRejectedResult => r.status === "rejected");
    if (failed.length === 4) {
      const info = getLoadErrorInfo(failed[0].reason);
      loadError = info.message;
      loadErrorIsUnauthorized = info.isUnauthorized;
    }
  } catch (error) {
    const info = getLoadErrorInfo(error);
    loadError = info.message;
    loadErrorIsUnauthorized = info.isUnauthorized;
  }

  return (
    <NotificationsModule
      initialLogData={logData}
      initialProviderConfigs={providerConfigs}
      fieldDefinitions={fieldDefinitions}
      templates={templates}
      loadError={loadError ?? undefined}
      loadErrorIsUnauthorized={loadErrorIsUnauthorized}
    />
  );
}
