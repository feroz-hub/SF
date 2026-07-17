import { ApiRoutes } from "@/lib/api/routes";
import { hclCsPostWithSession } from "@/lib/api/client";
import {
  type ChangePasswordRequest,
  type FrameworkResult,
  type GeneratePasswordResetTokenRequest,
  type GenerateUserTokenRequest,
  type GetUsersForClaimRequest,
  type LockUserWithEndDateRequest,
  type ResetPasswordRequest,
  type SecurityQuestionModel,
  type SetTwoFactorEnabledRequest,
  type TwoFactorType,
  type UUID,
  type UnLockUserBySecurityQuestionsRequest,
  type UnLockUserByTokenRequest,
  type UpdateUserTwoFactorTypeRequest,
  type VerifyEmailTokenRequest,
  type VerifySmsTokenRequest,
  type VerifyUserTokenRequest
} from "@/lib/types/hcl-cs";

export async function lockUserWithEndDate(payload: LockUserWithEndDateRequest): Promise<FrameworkResult> {
  return hclCsPostWithSession<FrameworkResult, LockUserWithEndDateRequest>(ApiRoutes.user.lockUserWithEndDatePath, payload);
}

export async function getUserRoleClaimsById(userId: UUID): Promise<unknown> {
  return hclCsPostWithSession<unknown, UUID>(ApiRoutes.user.getUserRoleClaimsById, userId);
}

export async function getUserRoleClaimsByName(userName: string): Promise<unknown> {
  return hclCsPostWithSession<unknown, string>(ApiRoutes.user.getUserRoleClaimsByName, userName);
}

export async function isUserExistsByClaimPrincipal(payload: unknown): Promise<boolean> {
  return hclCsPostWithSession<boolean, unknown>(ApiRoutes.user.isUserExistsByClaimPrincipal, payload);
}

export async function getClaims(userId: UUID): Promise<unknown> {
  return hclCsPostWithSession<unknown, UUID>(ApiRoutes.user.getClaims, userId);
}

export async function getUsersForClaim(payload: GetUsersForClaimRequest): Promise<unknown> {
  return hclCsPostWithSession<unknown, GetUsersForClaimRequest>(ApiRoutes.user.getUsersForClaim, payload);
}

export async function changePassword(payload: ChangePasswordRequest): Promise<FrameworkResult> {
  return hclCsPostWithSession<FrameworkResult, ChangePasswordRequest>(ApiRoutes.user.changePassword, payload);
}

export async function generatePasswordResetToken(payload: GeneratePasswordResetTokenRequest): Promise<unknown> {
  return hclCsPostWithSession<unknown, GeneratePasswordResetTokenRequest>(ApiRoutes.user.generatePasswordResetToken, payload);
}

export async function resetPassword(payload: ResetPasswordRequest): Promise<FrameworkResult> {
  return hclCsPostWithSession<FrameworkResult, ResetPasswordRequest>(ApiRoutes.user.resetPassword, payload);
}

export async function generateUserToken(payload: GenerateUserTokenRequest): Promise<unknown> {
  return hclCsPostWithSession<unknown, GenerateUserTokenRequest>(ApiRoutes.user.generateUserTokenAsync, payload);
}

export async function verifyUserToken(payload: VerifyUserTokenRequest): Promise<boolean> {
  return hclCsPostWithSession<boolean, VerifyUserTokenRequest>(ApiRoutes.user.verifyUserToken, payload);
}

export async function generateEmailConfirmationToken(userName: string): Promise<unknown> {
  return hclCsPostWithSession<unknown, string>(ApiRoutes.user.generateEmailConfirmationToken, userName);
}

export async function verifyEmailConfirmationToken(payload: VerifyEmailTokenRequest): Promise<boolean> {
  return hclCsPostWithSession<boolean, VerifyEmailTokenRequest>(ApiRoutes.user.verifyEmailConfirmationToken, payload);
}

export async function generatePhoneNumberConfirmationToken(userName: string): Promise<unknown> {
  return hclCsPostWithSession<unknown, string>(ApiRoutes.user.generatePhoneNumberConfirmationToken, userName);
}

export async function verifyPhoneNumberConfirmationToken(payload: VerifySmsTokenRequest): Promise<boolean> {
  return hclCsPostWithSession<boolean, VerifySmsTokenRequest>(ApiRoutes.user.verifyPhoneNumberConfirmationToken, payload);
}

export async function generateEmailTwoFactorToken(userName: string): Promise<unknown> {
  return hclCsPostWithSession<unknown, string>(ApiRoutes.user.generateEmailTwoFactorToken, userName);
}

export async function verifyEmailTwoFactorToken(payload: VerifyEmailTokenRequest): Promise<boolean> {
  return hclCsPostWithSession<boolean, VerifyEmailTokenRequest>(ApiRoutes.user.verifyEmailTwoFactorToken, payload);
}

export async function generateSmsTwoFactorToken(userName: string): Promise<unknown> {
  return hclCsPostWithSession<unknown, string>(ApiRoutes.user.generateSmsTwoFactorToken, userName);
}

export async function verifySmsTwoFactorToken(payload: VerifySmsTokenRequest): Promise<boolean> {
  return hclCsPostWithSession<boolean, VerifySmsTokenRequest>(ApiRoutes.user.verifySmsTwoFactorToken, payload);
}

export async function getAllTwoFactorTypes(): Promise<TwoFactorType[]> {
  return hclCsPostWithSession<TwoFactorType[], string>(ApiRoutes.user.getAllTwoFactorType, "");
}

export async function setTwoFactorEnabled(payload: SetTwoFactorEnabledRequest): Promise<FrameworkResult> {
  return hclCsPostWithSession<FrameworkResult, SetTwoFactorEnabledRequest>(ApiRoutes.user.setTwoFactorEnabled, payload);
}

export async function updateUserTwoFactorType(payload: UpdateUserTwoFactorTypeRequest): Promise<FrameworkResult> {
  return hclCsPostWithSession<FrameworkResult, UpdateUserTwoFactorTypeRequest>(
    ApiRoutes.user.updateUserTwoFactorType,
    payload
  );
}

export async function listSecurityQuestions(): Promise<SecurityQuestionModel[]> {
  return hclCsPostWithSession<SecurityQuestionModel[], string>(ApiRoutes.user.getAllSecurityQuestions, "");
}

export async function addSecurityQuestion(payload: SecurityQuestionModel): Promise<FrameworkResult> {
  return hclCsPostWithSession<FrameworkResult, SecurityQuestionModel>(ApiRoutes.user.addSecurityQuestion, payload);
}

export async function updateSecurityQuestion(payload: SecurityQuestionModel): Promise<FrameworkResult> {
  return hclCsPostWithSession<FrameworkResult, SecurityQuestionModel>(ApiRoutes.user.updateSecurityQuestion, payload);
}

export async function deleteSecurityQuestion(id: UUID): Promise<FrameworkResult> {
  return hclCsPostWithSession<FrameworkResult, UUID>(ApiRoutes.user.deleteSecurityQuestion, id);
}

export async function getUserSecurityQuestions(userId: UUID): Promise<unknown> {
  return hclCsPostWithSession<unknown, UUID>(ApiRoutes.user.getUserSecurityQuestions, userId);
}

export async function addUserSecurityQuestion(payload: unknown): Promise<FrameworkResult> {
  return hclCsPostWithSession<FrameworkResult, unknown>(ApiRoutes.user.addUserSecurityQuestion, payload);
}

export async function addUserSecurityQuestions(payload: unknown): Promise<FrameworkResult> {
  return hclCsPostWithSession<FrameworkResult, unknown>(ApiRoutes.user.addUserSecurityQuestionList, payload);
}

export async function updateUserSecurityQuestion(payload: unknown): Promise<FrameworkResult> {
  return hclCsPostWithSession<FrameworkResult, unknown>(ApiRoutes.user.updateUserSecurityQuestion, payload);
}

export async function deleteUserSecurityQuestion(payload: unknown): Promise<FrameworkResult> {
  return hclCsPostWithSession<FrameworkResult, unknown>(ApiRoutes.user.deleteUserSecurityQuestion, payload);
}

export async function deleteUserSecurityQuestions(payload: unknown): Promise<FrameworkResult> {
  return hclCsPostWithSession<FrameworkResult, unknown>(ApiRoutes.user.deleteUserSecurityQuestionList, payload);
}

export async function unlockUserByToken(payload: UnLockUserByTokenRequest): Promise<FrameworkResult> {
  return hclCsPostWithSession<FrameworkResult, UnLockUserByTokenRequest>(ApiRoutes.user.unLockUserByToken, payload);
}

export async function unlockUserBySecurityQuestions(payload: UnLockUserBySecurityQuestionsRequest): Promise<FrameworkResult> {
  return hclCsPostWithSession<FrameworkResult, UnLockUserBySecurityQuestionsRequest>(
    ApiRoutes.user.unLockUserByuserSecurityQuestions,
    payload
  );
}

