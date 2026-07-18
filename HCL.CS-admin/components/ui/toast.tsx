/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

import { cn } from "@/lib/utils";

export type ToastKind = "success" | "error" | "info";

export type ToastItem = {
  id: string;
  title: string;
  kind: ToastKind;
};

type ToastProps = {
  item: ToastItem;
  onDismiss: (id: string) => void;
};

export function Toast({ item, onDismiss }: ToastProps) {
  return (
    <div className={cn("toast", `toast-${item.kind}`)}>
      <span>{item.title}</span>
      <button type="button" onClick={() => onDismiss(item.id)} aria-label="Dismiss notification">
        ×
      </button>
    </div>
  );
}
