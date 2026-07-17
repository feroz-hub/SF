"use client";

import { type ReactNode } from "react";

type Props = {
  children: ReactNode;
};

export function FilterBar({ children }: Props) {
  return <div style={{ display: "flex", gap: "0.6rem", alignItems: "center", flexWrap: "wrap" }}>{children}</div>;
}

