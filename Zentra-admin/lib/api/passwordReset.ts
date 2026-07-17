import { ApiRoutes } from "@/lib/api/routes";
import { zentraPostAnonymous } from "@/lib/api/client";
import type { FrameworkResult, GeneratePasswordResetTokenRequest, ResetPasswordRequest } from "@/lib/types/zentra";

/** NotificationTypes: 1 = Email, 2 = SMS */
const NOTIFICATION_TYPE_EMAIL = 1;

export async function generatePasswordResetTokenAnonymous(username: string): Promise<FrameworkResult> {
  const payload: GeneratePasswordResetTokenRequest = {
    user_name: username.trim(),
    notification_type: NOTIFICATION_TYPE_EMAIL
  };
  return zentraPostAnonymous<FrameworkResult, GeneratePasswordResetTokenRequest>(
    ApiRoutes.user.generatePasswordResetToken,
    payload
  );
}

export async function resetPasswordAnonymous(payload: ResetPasswordRequest): Promise<FrameworkResult> {
  return zentraPostAnonymous<FrameworkResult, ResetPasswordRequest>(ApiRoutes.user.resetPassword, payload);
}
