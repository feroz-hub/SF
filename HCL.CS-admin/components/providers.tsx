/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

"use client";

import { type Session } from "next-auth";
import { SessionProvider } from "next-auth/react";
import { type ReactNode } from "react";

import { ToasterProvider } from "@/components/ui/toaster";
import { DensityProvider } from "@/context/DensityContext";
import { CommandPaletteProvider } from "@/context/CommandPaletteContext";

type ProvidersProps = {
  children: ReactNode;
  session: Session | null;
};

export function Providers({ children, session }: ProvidersProps) {
  return (
    <SessionProvider session={session}>
      <DensityProvider>
        <CommandPaletteProvider>
          <ToasterProvider>{children}</ToasterProvider>
        </CommandPaletteProvider>
      </DensityProvider>
    </SessionProvider>
  );
}
