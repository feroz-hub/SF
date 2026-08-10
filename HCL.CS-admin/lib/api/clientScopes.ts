/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

import { listIdentityResources } from "@/lib/api/identityResources";
import { listApiResources } from "@/lib/api/resources";

const OFFLINE_ACCESS_SCOPE = "offline_access";

export async function listClientScopes(): Promise<string[]> {
  const [apiResources, identityResources] = await Promise.all([listApiResources(), listIdentityResources()]);

  return Array.from(
    new Set([
      ...identityResources.map((resource) => resource.Name.trim()).filter(Boolean),
      OFFLINE_ACCESS_SCOPE,
      ...apiResources.flatMap((resource) =>
        [resource.Name, ...resource.ApiScopes.map((scope) => scope.Name)].map((value) => value.trim()).filter(Boolean)
      )
    ])
  ).sort((left, right) => left.localeCompare(right));
}
