/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

import { ApiRoutes } from "@/lib/api/routes";
import { requireAccessToken, hclCsPost, hclCsPostWithSession } from "@/lib/api/client";
import { type ClientsModel, type FrameworkResult } from "@/lib/types/hcl-cs";

type ClientNameLookup = Record<string, string>;

export async function listClientNames(): Promise<ClientNameLookup> {
  return hclCsPostWithSession<ClientNameLookup>(ApiRoutes.client.getAllClient, "");
}

export async function getClient(clientId: string): Promise<ClientsModel> {
  return hclCsPostWithSession<ClientsModel, string>(ApiRoutes.client.getClient, clientId);
}

export async function createClient(model: ClientsModel): Promise<ClientsModel> {
  return hclCsPostWithSession<ClientsModel, ClientsModel>(ApiRoutes.client.registerClient, model);
}

export async function updateClient(model: ClientsModel): Promise<ClientsModel> {
  return hclCsPostWithSession<ClientsModel, ClientsModel>(ApiRoutes.client.updateClient, model);
}

export async function deleteClient(clientId: string): Promise<FrameworkResult> {
  return hclCsPostWithSession<FrameworkResult, string>(ApiRoutes.client.deleteClient, clientId);
}

export async function rotateClientSecret(clientId: string): Promise<ClientsModel> {
  return hclCsPostWithSession<ClientsModel, string>(ApiRoutes.client.generateClientSecret, clientId);
}

/**
 * Fetches all clients with full details using a single access token for the whole batch.
 * This avoids 401s caused by many concurrent auth() calls when using hclCsPostWithSession per request.
 */
export async function listDetailedClients(): Promise<ClientsModel[]> {
  const accessToken = await requireAccessToken();

  const pairs = await hclCsPost<ClientNameLookup>(ApiRoutes.client.getAllClient, "", accessToken);
  const clientIds = Object.keys(pairs);

  return Promise.all(
    clientIds.map((clientId) =>
      hclCsPost<ClientsModel, string>(ApiRoutes.client.getClient, clientId, accessToken)
    )
  );
}
