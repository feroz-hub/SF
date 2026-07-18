/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

import {
  createContext,
  type ReactNode,
  useCallback,
  useContext,
  useEffect,
  useMemo,
  useState
} from "react";

type Density = "compact" | "default" | "comfortable";

type DensityContextValue = {
  density: Density;
  setDensity: (value: Density) => void;
};

const DensityContext = createContext<DensityContextValue | null>(null);

const STORAGE_KEY = "hcl-cs-density";

function applyDensity(value: Density) {
  if (typeof document === "undefined") return;
  document.documentElement.setAttribute("data-density", value);
}

export function DensityProvider({ children }: { children: ReactNode }) {
  const [density, setDensityState] = useState<Density>("default");

  useEffect(() => {
    if (typeof window === "undefined") return;
    const stored = window.localStorage.getItem(STORAGE_KEY) as Density | null;
    if (stored === "compact" || stored === "default" || stored === "comfortable") {
      setDensityState(stored);
      applyDensity(stored);
    } else {
      applyDensity("default");
    }
  }, []);

  const setDensity = useCallback((value: Density) => {
    setDensityState(value);
    applyDensity(value);
    try {
      window.localStorage.setItem(STORAGE_KEY, value);
    } catch {
      // ignore
    }
  }, []);

  const value = useMemo(() => ({ density, setDensity }), [density, setDensity]);

  return <DensityContext.Provider value={value}>{children}</DensityContext.Provider>;
}

export function useDensity(): DensityContextValue {
  const ctx = useContext(DensityContext);
  if (!ctx) {
    throw new Error("useDensity must be used within DensityProvider");
  }

  return ctx;
}

