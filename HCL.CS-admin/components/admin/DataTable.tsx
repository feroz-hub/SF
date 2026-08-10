/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

"use client";

import { type ReactNode } from "react";

type Column = {
  key: string;
  label: string;
  sortable?: boolean;
  sortDirection?: "asc" | "desc" | null;
  onSort?: () => void;
};

type Props =
  | {
      columns: string[];
      children: ReactNode;
      empty?: ReactNode;
    }
  | {
      columns: Column[];
      children: ReactNode;
      empty?: ReactNode;
    };

function isColumnObjectArray(columns: Props["columns"]): columns is Column[] {
  return typeof (columns as Column[])[0] === "object";
}

export function DataTable({ columns, children, empty }: Props) {
  return (
    <div className="table-wrap">
      <table className="table">
        <thead>
          <tr>
            {isColumnObjectArray(columns)
              ? columns.map((col) => (
                  <th
                    key={col.key}
                    onClick={col.sortable && col.onSort ? col.onSort : undefined}
                    style={col.sortable ? { cursor: "pointer", userSelect: "none" } : undefined}
                  >
                    <span>
                      {col.label}
                      {col.sortable && col.sortDirection
                        ? col.sortDirection === "asc"
                          ? " ↑"
                          : " ↓"
                        : null}
                    </span>
                  </th>
                ))
              : columns.map((col) => <th key={col}>{col}</th>)}
          </tr>
        </thead>
        <tbody>{children || empty}</tbody>
      </table>
    </div>
  );
}

