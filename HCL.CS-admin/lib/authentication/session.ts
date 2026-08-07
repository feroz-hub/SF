/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

import type { Session } from "next-auth";
import type { JWT } from "next-auth/jwt";

export type ServerSession = Session & {
  accessToken?: string;
  refreshToken?: string;
  idToken?: string;
};

/**
 * Makes tokens available to server-side API helpers without serializing them
 * in the browser-visible NextAuth session response.
 */
export function attachServerOnlyTokens(session: Session, token: JWT): ServerSession {
  Object.defineProperties(session, {
    accessToken: { value: token.accessToken, enumerable: false },
    refreshToken: { value: token.refreshToken, enumerable: false },
    idToken: { value: token.idToken, enumerable: false }
  });
  return session as ServerSession;
}
