/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

"use client";

import { motion } from "framer-motion";

type AIInsightCardProps = {
  title: string;
  metric: string;
  trend?: "up" | "down" | "flat";
  trendLabel?: string;
  footer?: string;
  accent?: "cyan" | "violet" | "emerald";
};

export function AIInsightCard({
  title,
  metric,
  trend = "flat",
  trendLabel,
  footer,
  accent = "cyan"
}: AIInsightCardProps) {
  return (
    <motion.article
      initial={{ y: 8, opacity: 0 }}
      animate={{ y: 0, opacity: 1 }}
      transition={{ duration: 0.4, ease: "easeOut" }}
      className={`ai-insight-card ai-insight-${accent}`}
    >
      <div className="ai-insight-content">
        <div className="ai-insight-heading">
          <h3>{title}</h3>
          <span className="ai-insight-label">AI Signal</span>
        </div>
        <div className="ai-insight-metric-row">
          <div className="ai-insight-metric">{metric}</div>
          {trendLabel && (
            <div className={`ai-insight-trend ai-insight-trend-${trend}`}>{trendLabel}</div>
          )}
        </div>
        {footer && <p className="ai-insight-footer">{footer}</p>}
      </div>
    </motion.article>
  );
}
