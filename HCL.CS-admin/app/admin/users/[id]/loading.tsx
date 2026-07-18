/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

import { Skeleton } from "@/components/ui/skeleton";

export default function UserDetailLoading() {
  return (
    <section className="card">
      <header className="card-head">
        <Skeleton style={{ height: 24, width: 260 }} />
      </header>
      <div className="card-body" style={{ display: "grid", gap: "0.6rem" }}>
        {Array.from({ length: 10 }).map((_, index) => (
          <Skeleton key={index} style={{ height: 42 }} />
        ))}
      </div>
    </section>
  );
}
