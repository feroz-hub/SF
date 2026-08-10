/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

import { notFound, redirect } from "next/navigation";

import { UserSessionsModule } from "@/components/modules/users/UserSessionsModule";
import { HclCsApiError } from "@/lib/api/client";
import { getUser, listUserActiveTokens } from "@/lib/api/users";

export default async function UserSessionsPage({ params }: { params: Promise<{ id: string }> }) {
  const { id } = await params;
  const userId = decodeURIComponent(id);

  try {
    const [user, sessions] = await Promise.all([getUser(userId), listUserActiveTokens([userId])]);

    return <UserSessionsModule userId={user.Id} userName={user.UserName} sessions={sessions} />;
  } catch (error) {
    if (error instanceof HclCsApiError) {
      if (error.statusCode === 401) {
        redirect("/login");
      }
      if (error.statusCode === 404) {
        notFound();
      }
    }

    throw error;
  }
}
