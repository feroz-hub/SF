/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

export default function ForbiddenPage() {
  return (
    <main className="forbidden-shell">
      <section className="forbidden-card">
        <p className="forbidden-code">403</p>
        <h1 className="text-display">Access Restricted</h1>
        <p className="text-body">
          Your session doesn&apos;t have the admin role required for this area. If you believe this is a mistake,
          contact your HCL.CS administrator.
        </p>
      </section>
    </main>
  );
}
