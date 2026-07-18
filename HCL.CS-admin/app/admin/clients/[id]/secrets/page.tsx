/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

import { notFound, redirect } from "next/navigation";

import { ClientSecretsModule } from "@/components/modules/clients/ClientSecretsModule";
import { HclCsApiError } from "@/lib/api/client";
import { getClient } from "@/lib/api/clients";

export default async function ClientSecretsPage({ params }: { params: { id: string } }) {
  const clientId = decodeURIComponent(params.id);

  try {
    const client = await getClient(clientId);

    return (
      <ClientSecretsModule
        clientId={client.ClientId}
        clientName={client.ClientName}
        secretExpiresAt={client.ClientSecretExpiresAt}
      />
    );
  } catch (error) {
    if (error instanceof HclCsApiError && error.statusCode === 401) {
      redirect("/login");
    }
    notFound();
  }
}
