/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

"use client";

import { type ReactNode } from "react";

import { Button } from "@/components/ui/button";
import { Dialog } from "@/components/ui/dialog";

type Props = {
  open: boolean;
  title: string;
  description?: string;
  submitLabel?: string;
  cancelLabel?: string;
  pending?: boolean;
  onClose: () => void;
  onSubmit: () => void;
  children: ReactNode;
};

export function FormDialog({
  open,
  title,
  description,
  submitLabel = "Save",
  cancelLabel = "Cancel",
  pending = false,
  onClose,
  onSubmit,
  children
}: Props) {
  return (
    <Dialog open={open} title={title} description={description} onClose={onClose}>
      <div className="form-grid">
        {children}
        <div className="form-row" style={{ display: "flex", justifyContent: "flex-end", gap: "0.6rem" }}>
          <Button type="button" variant="secondary" onClick={onClose} disabled={pending}>
            {cancelLabel}
          </Button>
          <Button type="button" onClick={onSubmit} disabled={pending}>
            {pending ? "Saving..." : submitLabel}
          </Button>
        </div>
      </div>
    </Dialog>
  );
}

