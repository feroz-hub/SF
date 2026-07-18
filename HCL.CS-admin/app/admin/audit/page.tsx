/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

import { AuditModule } from "@/components/modules/audit/AuditModule";
import { getLoadErrorInfo } from "@/lib/api/client";
import { searchAudit } from "@/lib/api/audit";
import { type AuditResponseModel } from "@/lib/types/hcl-cs";

const emptyAuditData: AuditResponseModel = {
  AuditList: [],
  PageInfo: {
    TotalItems: 0,
    ItemsPerPage: 20,
    CurrentPage: 1,
    TotalPages: 0,
    TotalDisplayPages: 0
  }
};

export default async function AuditPage() {
  let data: AuditResponseModel = emptyAuditData;
  let loadError: string | null = null;
  let loadErrorIsUnauthorized = false;

  try {
    const result = await searchAudit({
      actionType: 0,
      createdBy: "",
      searchValue: "",
      page: {
        TotalItems: 0,
        ItemsPerPage: 20,
        CurrentPage: 1,
        TotalPages: 0,
        TotalDisplayPages: 10
      }
    });
    data = result ?? emptyAuditData;
  } catch (error) {
    const info = getLoadErrorInfo(error);
    loadError = info.message;
    loadErrorIsUnauthorized = info.isUnauthorized;
  }

  return <AuditModule initialData={data} loadError={loadError ?? undefined} loadErrorIsUnauthorized={loadErrorIsUnauthorized} />;
}
