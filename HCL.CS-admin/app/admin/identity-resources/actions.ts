/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

"use server";

import { z } from "zod";

import {
  addIdentityResourceClaim,
  createIdentityResource,
  deleteIdentityResourceById,
  deleteIdentityResourceClaimById,
  getIdentityResourceById,
  listIdentityResourceClaims,
  updateIdentityResource
} from "@/lib/api/identityResources";
import { getActor } from "@/lib/actor";
import { auth } from "@/lib/auth";
import { MAX_LENGTH_255 } from "@/lib/constants";
import { type ActionResult, type IdentityClaimsModel, type IdentityResourcesModel } from "@/lib/types/hcl-cs";

const ZERO_GUID = "00000000-0000-0000-0000-000000000000";

const resourceSchema = z.object({
  id: z.string().default(ZERO_GUID),
  name: z.string().min(2).max(MAX_LENGTH_255),
  displayName: z.string().min(1).max(MAX_LENGTH_255),
  description: z.string().default(""),
  enabled: z.boolean().default(true),
  required: z.boolean().default(false),
  emphasize: z.boolean().default(false)
});

const claimSchema = z.object({
  id: z.string().default(ZERO_GUID),
  identityResourceId: z.string().min(1),
  type: z.string().min(1).max(MAX_LENGTH_255),
  aliasType: z.string().default("")
});

const deleteSchema = z.object({
  id: z.string().min(1)
});

export async function createIdentityResourceAction(input: z.infer<typeof resourceSchema>): Promise<ActionResult> {
  const parsed = resourceSchema.safeParse(input);
  if (!parsed.success) {
    return { ok: false, message: "Validation failed for identity resource create.", errors: parsed.error.flatten().fieldErrors };
  }

  const session = await auth();
  const actor = getActor(session);
  const now = new Date().toISOString();

  const model: IdentityResourcesModel = {
    Id: ZERO_GUID,
    Name: parsed.data.name,
    DisplayName: parsed.data.displayName,
    Description: parsed.data.description,
    Enabled: parsed.data.enabled,
    Required: parsed.data.required,
    Emphasize: parsed.data.emphasize,
    IdentityClaims: [],
    CreatedBy: actor,
    CreatedOn: now,
    IsDeleted: false
  };

  try {
    await createIdentityResource(model);
    return { ok: true, message: "Identity resource created." };
  } catch (error) {
    return { ok: false, message: error instanceof Error ? error.message : "Create failed." };
  }
}

export async function updateIdentityResourceAction(input: z.infer<typeof resourceSchema>): Promise<ActionResult> {
  const parsed = resourceSchema.safeParse(input);
  if (!parsed.success) {
    return { ok: false, message: "Validation failed for identity resource update.", errors: parsed.error.flatten().fieldErrors };
  }

  try {
    const existing = await getIdentityResourceById(parsed.data.id);
    const session = await auth();
    const actor = getActor(session);

    const model: IdentityResourcesModel = {
      ...existing,
      Name: parsed.data.name,
      DisplayName: parsed.data.displayName,
      Description: parsed.data.description,
      Enabled: parsed.data.enabled,
      Required: parsed.data.required,
      Emphasize: parsed.data.emphasize,
      ModifiedBy: actor,
      ModifiedOn: new Date().toISOString()
    };

    await updateIdentityResource(model);
    return { ok: true, message: "Identity resource updated." };
  } catch (error) {
    return { ok: false, message: error instanceof Error ? error.message : "Update failed." };
  }
}

export async function deleteIdentityResourceAction(input: { id: string }): Promise<ActionResult> {
  const parsed = deleteSchema.safeParse(input);
  if (!parsed.success) {
    return { ok: false, message: "Invalid identity resource identifier." };
  }

  try {
    await deleteIdentityResourceById(parsed.data.id);
    return { ok: true, message: "Identity resource deleted." };
  } catch (error) {
    return { ok: false, message: error instanceof Error ? error.message : "Delete failed." };
  }
}

export async function listIdentityClaimsAction(resourceId: string): Promise<ActionResult<IdentityClaimsModel[]>> {
  const parsed = deleteSchema.safeParse({ id: resourceId });
  if (!parsed.success) {
    return { ok: false, message: "Invalid identity resource identifier." };
  }

  try {
    const claims = await listIdentityResourceClaims(resourceId);
    return { ok: true, message: "Claims loaded.", data: claims };
  } catch (error) {
    return { ok: false, message: error instanceof Error ? error.message : "Failed to load claims." };
  }
}

export async function addIdentityClaimAction(input: z.infer<typeof claimSchema>): Promise<ActionResult> {
  const parsed = claimSchema.safeParse(input);
  if (!parsed.success) {
    return { ok: false, message: "Validation failed for claim create.", errors: parsed.error.flatten().fieldErrors };
  }

  const session = await auth();
  const actor = getActor(session);
  const now = new Date().toISOString();

  const model: IdentityClaimsModel = {
    Id: ZERO_GUID,
    IdentityResourceId: parsed.data.identityResourceId,
    Type: parsed.data.type,
    AliasType: parsed.data.aliasType,
    CreatedBy: actor,
    CreatedOn: now,
    IsDeleted: false
  };

  try {
    await addIdentityResourceClaim(model);
    return { ok: true, message: "Identity claim created." };
  } catch (error) {
    return { ok: false, message: error instanceof Error ? error.message : "Create failed." };
  }
}

export async function deleteIdentityClaimAction(input: { id: string }): Promise<ActionResult> {
  const parsed = deleteSchema.safeParse(input);
  if (!parsed.success) {
    return { ok: false, message: "Invalid claim identifier." };
  }

  try {
    await deleteIdentityResourceClaimById(parsed.data.id);
    return { ok: true, message: "Identity claim deleted." };
  } catch (error) {
    return { ok: false, message: error instanceof Error ? error.message : "Delete failed." };
  }
}

