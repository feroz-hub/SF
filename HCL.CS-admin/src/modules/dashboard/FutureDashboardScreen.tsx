/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

"use client";

import { motion } from "framer-motion";
import { AIInsightCard } from "@/src/components/ui/AIInsightCard";

export function FutureDashboardScreen() {
  return (
    <div className="future-dashboard-grid">
      <section className="future-dashboard-primary">
        <div className="future-insight-grid">
          <AIInsightCard
            title="Identity Stability"
            metric="99.97%"
            trend="up"
            trendLabel="+0.12% vs 24h"
            footer="Federated logins are stable across all tenants."
            accent="emerald"
          />
          <AIInsightCard
            title="Anomaly Surface"
            metric="Low"
            trend="flat"
            trendLabel="2 weak signals"
            footer="Minor outliers detected in a single client region."
            accent="violet"
          />
          <AIInsightCard
            title="Policy Drift"
            metric="0.3%"
            trend="down"
            trendLabel="-0.1% vs baseline"
            footer="Role and claim changes remain within safe bounds."
            accent="cyan"
          />
        </div>
        <motion.div
          initial={{ opacity: 0, y: 12 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ delay: 0.15, duration: 0.4 }}
          className="future-panel future-mesh-panel"
        >
          <div className="future-panel-header">
            <div>
              <h2 className="future-panel-eyebrow">Live Mesh</h2>
              <p className="future-panel-title">Requests & authorizations over time</p>
            </div>
            <span className="future-stream-badge">Streaming from demo server</span>
          </div>
          <div className="future-chart-placeholder">
            <p>
              This panel is wired for Framer Motion & charts. Plug in real-time metrics (requests,
              latency, authorization decisions) when you are ready, without changing the surrounding
              layout.
            </p>
          </div>
        </motion.div>
      </section>
      <motion.section
        initial={{ opacity: 0, x: 12 }}
        animate={{ opacity: 1, x: 0 }}
        transition={{ delay: 0.1, duration: 0.4 }}
        className="future-panel future-impact-panel"
      >
        <header className="future-panel-header">
          <div>
            <h2 className="future-panel-eyebrow">Impact Preview</h2>
            <p className="future-panel-title">Last 5 high-sensitivity changes</p>
          </div>
          <span className="future-readonly-badge">Read‑only demo</span>
        </header>
        <div className="future-impact-copy">
          <p>
            This stream will show the most recent role and policy changes with their blast radius —
            users impacted, clients affected, and downstream systems.
          </p>
          <p>
            Connect this panel to your existing audit API and reuse the identity relationships we
            already surfaced in the admin experience.
          </p>
        </div>
      </motion.section>
    </div>
  );
}
