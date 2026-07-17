"use client";

import { type ReactNode } from "react";

type Props = {
  title: string;
  subtitle?: string;
  actions?: ReactNode;
};

export function PageHeader({ title, subtitle, actions }: Props) {
  return (
    <header className="card-head">
      <div>
        <h2>{title}</h2>
        {subtitle ? <p className="inline-message">{subtitle}</p> : null}
      </div>
      {actions ? <div style={{ display: "flex", gap: "0.6rem", alignItems: "center" }}>{actions}</div> : null}
    </header>
  );
}

