/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

import { IdentityResourcesModule } from "@/components/modules/identity-resources/IdentityResourcesModule";
import { getLoadErrorInfo } from "@/lib/api/client";
import { listIdentityResources } from "@/lib/api/identityResources";
import { type IdentityResourcesModel } from "@/lib/types/hcl-cs";

export default async function IdentityResourcesPage() {
  let resources: IdentityResourcesModel[] = [];
  let loadError: string | null = null;
  let loadErrorIsUnauthorized = false;

  try {
    resources = await listIdentityResources();
  } catch (error) {
    const info = getLoadErrorInfo(error);
    loadError = info.message;
    loadErrorIsUnauthorized = info.isUnauthorized;
  }

  return <IdentityResourcesModule resources={resources} loadError={loadError ?? undefined} loadErrorIsUnauthorized={loadErrorIsUnauthorized} />;
}

