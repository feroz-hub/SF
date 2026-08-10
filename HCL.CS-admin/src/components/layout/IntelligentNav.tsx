/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

"use client";

import { motion } from "framer-motion";
import { Home, Shield, Activity, Users, Cpu } from "lucide-react";
import { useUiStore } from "@/src/store/uiStore";

type NavItem = {
  id: string;
  label: string;
  icon: React.ComponentType<{ className?: string }>;
  route: string;
  section: "operations" | "intelligence" | "security" | "system";
};

const NAV_ITEMS: NavItem[] = [
  {
    id: "dashboard",
    label: "Dashboard",
    icon: Home,
    route: "/future-dashboard",
    section: "operations"
  },
  { id: "threats", label: "Threat Matrix", icon: Shield, route: "/threats", section: "security" },
  { id: "users", label: "Identities", icon: Users, route: "/users", section: "operations" },
  {
    id: "signals",
    label: "AI Signals",
    icon: Activity,
    route: "/signals",
    section: "intelligence"
  },
  { id: "system", label: "System Mesh", icon: Cpu, route: "/system", section: "system" }
];

export function IntelligentNav() {
  const { navCollapsed, setNavCollapsed, activeRoute, setActiveRoute } = useUiStore();

  return (
    <motion.aside
      initial={false}
      animate={{ width: navCollapsed ? 72 : 260 }}
      className="future-nav"
    >
      <div className="future-nav-brand">
        <div className="future-nav-mark">CS</div>
        {!navCollapsed && (
          <div className="future-nav-wordmark">
            <strong>HCL.CS</strong>
            <small>Intelligence</small>
          </div>
        )}
      </div>

      <div className="future-nav-sections">
        {(["operations", "intelligence", "security", "system"] as const).map((section) => (
          <div key={section} className="future-nav-section">
            {!navCollapsed && <div className="future-nav-section-label">{section}</div>}
            {NAV_ITEMS.filter((item) => item.section === section).map((item) => {
              const Icon = item.icon;
              const active = activeRoute === item.route;
              return (
                <button
                  key={item.id}
                  type="button"
                  onClick={() => setActiveRoute(item.route)}
                  className={`future-nav-link${active ? " future-nav-link-active" : ""}`}
                >
                  <Icon className="future-nav-link-icon" />
                  {!navCollapsed && <span>{item.label}</span>}
                </button>
              );
            })}
          </div>
        ))}
      </div>

      <div className="future-nav-footer">
        <button
          type="button"
          onClick={() => setNavCollapsed(!navCollapsed)}
          className="future-nav-collapse"
        >
          {navCollapsed ? "Expand" : "Collapse"}
        </button>
      </div>
    </motion.aside>
  );
}
