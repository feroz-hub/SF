/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

import { ApiRoutes } from "@/lib/api/routes";
import { hclCsPostAnonymous } from "@/lib/api/client";
import type { FrameworkResult, GeneratePasswordResetTokenRequest, ResetPasswordRequest } from "@/lib/types/hcl-cs";

/** NotificationTypes: 1 = Email, 2 = SMS */
const NOTIFICATION_TYPE_EMAIL = 1;

export async function generatePasswordResetTokenAnonymous(username: string): Promise<FrameworkResult> {
  const payload: GeneratePasswordResetTokenRequest = {
    user_name: username.trim(),
    notification_type: NOTIFICATION_TYPE_EMAIL
  };
  return hclCsPostAnonymous<FrameworkResult, GeneratePasswordResetTokenRequest>(
    ApiRoutes.user.generatePasswordResetToken,
    payload
  );
}

export async function resetPasswordAnonymous(payload: ResetPasswordRequest): Promise<FrameworkResult> {
  return hclCsPostAnonymous<FrameworkResult, ResetPasswordRequest>(ApiRoutes.user.resetPassword, payload);
}
