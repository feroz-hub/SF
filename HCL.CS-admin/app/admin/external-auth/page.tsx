/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

import { ExternalAuthModule } from "@/components/modules/external-auth/ExternalAuthModule";
import { getLoadErrorInfo } from "@/lib/api/client";
import {
  getAllExternalAuthProviders,
  getExternalAuthFieldDefinitions
} from "@/lib/api/externalAuth";
import {
  type ExternalAuthProviderConfigModel,
  type ExternalAuthFieldDefinitionsResponse
} from "@/lib/types/hcl-cs";

export default async function ExternalAuthPage() {
  let providers: ExternalAuthProviderConfigModel[] = [];
  let fieldDefinitions: ExternalAuthFieldDefinitionsResponse | null = null;
  let loadError: string | null = null;
  let loadErrorIsUnauthorized = false;

  try {
    const [providersResult, fieldsResult] = await Promise.all([
      getAllExternalAuthProviders(),
      getExternalAuthFieldDefinitions()
    ]);

    providers = providersResult ?? [];
    fieldDefinitions = fieldsResult;
  } catch (error) {
    const info = getLoadErrorInfo(error);
    loadError = info.message;
    loadErrorIsUnauthorized = info.isUnauthorized;
  }

  return (
    <ExternalAuthModule
      initialProviders={providers}
      fieldDefinitions={fieldDefinitions}
      loadError={loadError ?? undefined}
      loadErrorIsUnauthorized={loadErrorIsUnauthorized}
    />
  );
}
