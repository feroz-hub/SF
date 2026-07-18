/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

"use client";

import { motion } from "framer-motion";
import { Search, Sparkles, Command } from "lucide-react";

type CommandBarProps = {
  title?: string;
  subtitle?: string;
};

export function CommandBar({ title = "Command Center", subtitle }: CommandBarProps) {
  return (
    <motion.header
      initial={{ y: -12, opacity: 0 }}
      animate={{ y: 0, opacity: 1 }}
      transition={{ duration: 0.4, ease: "easeOut" }}
      className="future-command-bar"
    >
      <div className="future-command-title-group">
        <div className="future-live-badge">
          <Sparkles size={13} />
          Live AI Telemetry
        </div>
        <div className="future-command-copy">
          <div className="future-command-product">HCL.CS</div>
          <div className="future-command-title">{title}</div>
          {subtitle && <p className="future-command-subtitle">{subtitle}</p>}
        </div>
      </div>

      <div className="future-command-actions">
        <button type="button" className="future-command-search">
          <Search size={14} />
          <span>Ask anything</span>
          <span className="future-command-key">
            <Command size={12} />
            <span>K</span>
          </span>
        </button>
      </div>
    </motion.header>
  );
}
