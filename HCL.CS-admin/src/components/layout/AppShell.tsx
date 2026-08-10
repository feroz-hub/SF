/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

"use client";

import { AmbientBackground } from "@/src/components/layout/AmbientBackground";
import { IntelligentNav } from "@/src/components/layout/IntelligentNav";
import { CommandBar } from "@/src/components/layout/CommandBar";

type AppShellProps = {
  children: React.ReactNode;
  title?: string;
  subtitle?: string;
  className?: string;
};

export function AppShell({ children, title, subtitle, className }: AppShellProps) {
  return (
    <div className="future-shell">
      <AmbientBackground />
      <div className="future-shell-frame">
        <IntelligentNav />
        <main className="future-main">
          <div className="future-main-inner">
            <CommandBar title={title} subtitle={subtitle} />
            <section className={`future-content${className ? ` ${className}` : ""}`}>
              {children}
            </section>
          </div>
        </main>
      </div>
    </div>
  );
}
