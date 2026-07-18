/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

"use server";

import { z } from "zod";

import { searchAudit } from "@/lib/api/audit";
import { type ActionResult, type AuditResponseModel } from "@/lib/types/hcl-cs";

const searchSchema = z.object({
  actionType: z.number().int().min(0).max(3).default(0),
  actor: z.string().default(""),
  searchValue: z.string().default(""),
  fromDate: z.string().default(""),
  toDate: z.string().default(""),
  page: z.number().int().min(1).default(1),
  itemsPerPage: z.number().int().min(1).max(200).default(20)
});

export async function searchAuditAction(
  input: z.input<typeof searchSchema>
): Promise<ActionResult<AuditResponseModel>> {
  const parsed = searchSchema.safeParse(input);
  if (!parsed.success) {
    return {
      ok: false,
      message: "Invalid audit search input.",
      errors: parsed.error.flatten().fieldErrors
    };
  }

  try {
    const result = await searchAudit({
      actionType: parsed.data.actionType,
      createdBy: parsed.data.actor,
      searchValue: parsed.data.searchValue,
      fromDate: parsed.data.fromDate || undefined,
      toDate: parsed.data.toDate || undefined,
      page: {
        TotalItems: 0,
        ItemsPerPage: parsed.data.itemsPerPage,
        CurrentPage: parsed.data.page,
        TotalPages: 0,
        TotalDisplayPages: 10
      }
    });

    const data: AuditResponseModel = result ?? {
      AuditList: [],
      PageInfo: {
        TotalItems: 0,
        ItemsPerPage: parsed.data.itemsPerPage,
        CurrentPage: parsed.data.page,
        TotalPages: 0,
        TotalDisplayPages: 0
      }
    };

    return {
      ok: true,
      message: `${data.AuditList.length} audit event(s) loaded.`,
      data
    };
  } catch (error) {
    return {
      ok: false,
      message: error instanceof Error ? error.message : "Audit search failed."
    };
  }
}
