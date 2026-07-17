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
