/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

import { notFound, redirect } from "next/navigation";

import { UserDetailModule } from "@/components/modules/users/UserDetailModule";
import { HclCsApiError } from "@/lib/api/client";
import { listRoles } from "@/lib/api/roles";
import { getUser, getUserRoles } from "@/lib/api/users";

export default async function UserDetailPage({ params }: { params: Promise<{ id: string }> }) {
  const { id } = await params;
  const userId = decodeURIComponent(id);

  try {
    const [user, roles, userRoles] = await Promise.all([
      getUser(userId),
      listRoles(),
      getUserRoles(userId).catch(() => [])
    ]);

    return <UserDetailModule user={user} roles={roles} userRoles={userRoles} />;
  } catch (error) {
    if (error instanceof HclCsApiError) {
      if (error.statusCode === 401) {
        redirect("/login");
      }
      if (error.statusCode === 404) {
        notFound();
      }
    }

    // For any other error, surface it to the route error boundary
    // so you see the actual problem instead of a generic 404.
    throw error;
  }
}
