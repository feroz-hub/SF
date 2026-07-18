/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

import { ClientsModule } from "@/components/modules/clients/ClientsModule";
import { getLoadErrorInfo } from "@/lib/api/client";
import { listClientScopes } from "@/lib/api/clientScopes";
import { listDetailedClients } from "@/lib/api/clients";

export default async function ClientsPage() {
  let clients: Awaited<ReturnType<typeof listDetailedClients>> = [];
  let availableScopes: string[] = [];
  let loadError: string | null = null;
  let loadErrorIsUnauthorized = false;

  try {
    const [clientsResult, scopes] = await Promise.all([listDetailedClients(), listClientScopes()]);
    clients = clientsResult;
    availableScopes = scopes;
  } catch (error) {
    const info = getLoadErrorInfo(error);
    loadError = info.message;
    loadErrorIsUnauthorized = info.isUnauthorized;
  }

  return (
    <ClientsModule
      clients={clients}
      availableScopes={availableScopes}
      loadError={loadError ?? undefined}
      loadErrorIsUnauthorized={loadErrorIsUnauthorized}
    />
  );
}
