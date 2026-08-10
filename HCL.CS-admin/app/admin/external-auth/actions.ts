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
  saveExternalAuthProvider,
  deleteExternalAuthProvider,
  testExternalAuthProvider
} from "@/lib/api/externalAuth";
import { type ActionResult, type FrameworkResult } from "@/lib/types/hcl-cs";

const saveProviderSchema = z.object({
  id: z.string().nullable().default(null),
  providerName: z.string().min(1, "Provider name is required"),
  providerType: z.number().int().min(1).default(1),
  isEnabled: z.boolean().default(false),
  settings: z.record(z.string(), z.string()),
  autoProvisionEnabled: z.boolean().default(false),
  allowedDomains: z.string().nullable().default(null)
});

export async function saveExternalAuthProviderAction(
  input: z.input<typeof saveProviderSchema>
): Promise<ActionResult<FrameworkResult>> {
  const parsed = saveProviderSchema.safeParse(input);
  if (!parsed.success) {
    return {
      ok: false,
      message: "Invalid provider configuration.",
      errors: parsed.error.flatten().fieldErrors
    };
  }

  try {
    const result = await saveExternalAuthProvider({
      Id: parsed.data.id ?? undefined,
      ProviderName: parsed.data.providerName,
      ProviderType: parsed.data.providerType,
      IsEnabled: parsed.data.isEnabled,
      Settings: parsed.data.settings,
      AutoProvisionEnabled: parsed.data.autoProvisionEnabled,
      AllowedDomains: parsed.data.allowedDomains
    });

    if (result && (result.Status === 0 || result.Status === "Succeeded")) {
      return { ok: true, message: "Provider configuration saved.", data: result };
    }

    const errorMsg = result?.Errors?.[0]?.Description ?? "Failed to save provider configuration.";
    return { ok: false, message: errorMsg };
  } catch (error) {
    return {
      ok: false,
      message: error instanceof Error ? error.message : "Save provider config failed."
    };
  }
}

const deleteProviderSchema = z.object({
  id: z.string().min(1, "Provider ID is required")
});

export async function deleteExternalAuthProviderAction(
  input: z.input<typeof deleteProviderSchema>
): Promise<ActionResult> {
  const parsed = deleteProviderSchema.safeParse(input);
  if (!parsed.success) {
    return { ok: false, message: "Invalid provider ID." };
  }

  try {
    const result = await deleteExternalAuthProvider({ Id: parsed.data.id });

    if (result && (result.Status === 0 || result.Status === "Succeeded")) {
      return { ok: true, message: "Provider configuration deleted." };
    }

    const errorMsg = result?.Errors?.[0]?.Description ?? "Failed to delete provider configuration.";
    return { ok: false, message: errorMsg };
  } catch (error) {
    return {
      ok: false,
      message: error instanceof Error ? error.message : "Delete provider config failed."
    };
  }
}

const testProviderSchema = z.object({
  id: z.string().min(1, "Provider ID is required")
});

export async function testExternalAuthProviderAction(
  input: z.input<typeof testProviderSchema>
): Promise<ActionResult> {
  const parsed = testProviderSchema.safeParse(input);
  if (!parsed.success) {
    return { ok: false, message: "Invalid provider ID." };
  }

  try {
    const result = await testExternalAuthProvider({ Id: parsed.data.id });

    if (result && (result.Status === 0 || result.Status === "Succeeded")) {
      return { ok: true, message: "Provider connection test successful." };
    }

    const errorMsg = result?.Errors?.[0]?.Description ?? "Provider connection test failed.";
    return { ok: false, message: errorMsg };
  } catch (error) {
    return {
      ok: false,
      message: error instanceof Error ? error.message : "Test provider failed."
    };
  }
}
