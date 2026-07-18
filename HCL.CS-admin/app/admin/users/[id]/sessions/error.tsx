/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

"use client";

import { Button } from "@/components/ui/button";

export default function UserSessionsError({ error, reset }: { error: Error; reset: () => void }) {
  return (
    <section className="card">
      <div className="card-body" style={{ display: "grid", gap: "0.7rem" }}>
        <h2>User sessions module failed</h2>
        <p className="inline-message error">{error.message}</p>
        <div className="toolbar" style={{ gap: "0.5rem" }}>
          <Button type="button" variant="secondary" onClick={reset}>
            Retry
          </Button>
        </div>
      </div>
    </section>
  );
}
