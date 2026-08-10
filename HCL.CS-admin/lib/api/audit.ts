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
  type AuditResponseModel,
  type AuditSearchRequestModel,
  type PagingModel
} from "@/lib/types/hcl-cs";

export type AuditQueryInput = {
  actionType?: number;
  createdBy?: string;
  fromDate?: string;
  toDate?: string;
  page?: PagingModel;
  searchValue?: string;
};

export function buildAuditSearchRequest(input: AuditQueryInput): AuditSearchRequestModel {
  const page = input.page ?? {
    TotalItems: 0,
    ItemsPerPage: 20,
    CurrentPage: 1,
    TotalPages: 0,
    TotalDisplayPages: 10
  };

  const fromDate = input.fromDate?.trim();
  const toDate = input.toDate?.trim();

  return {
    ActionType: (input.actionType ?? 0) as 0 | 1 | 2 | 3,
    CreatedBy: input.createdBy ?? "",
    FromDate: fromDate ? fromDate : null,
    ToDate: toDate ? toDate : null,
    Page: page,
    CreatedOn: null,
    SearchValue: input.searchValue ?? ""
  };
}

export async function searchAudit(input: AuditQueryInput): Promise<AuditResponseModel | null> {
  const payload = buildAuditSearchRequest(input);
  const result = await hclCsPostWithSession<AuditResponseModel | null, AuditSearchRequestModel>(
    ApiRoutes.audit.getAuditDetails,
    payload
  );
  return result ?? null;
}

export async function addAuditTrail(payload: unknown): Promise<unknown> {
  return hclCsPostWithSession<unknown, unknown>(ApiRoutes.audit.addAuditTrail, payload);
}

export async function addAuditTrailModel(payload: unknown): Promise<unknown> {
  return hclCsPostWithSession<unknown, unknown>(ApiRoutes.audit.addAuditTrailModel, payload);
}
