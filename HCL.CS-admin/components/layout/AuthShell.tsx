import { BadgeCheck, LockKeyhole, ShieldCheck } from "lucide-react";
import type { ReactNode } from "react";

type AuthShellProps = {
  children: ReactNode;
};

export function AuthShell({ children }: AuthShellProps) {
  return (
    <main className="login-shell">
      <section className="login-brand-panel" aria-label="HCL.CS product information">
        <div className="auth-brand-lockup">
          <span className="auth-brand-mark" aria-hidden="true">
            CS
          </span>
          <span className="auth-brand-copy">
            <strong>HCL.CS</strong>
            <small>An HCLTech Product</small>
          </span>
        </div>

        <div className="auth-brand-message">
          <p className="auth-brand-eyebrow">Identity &amp; access management</p>
          <h2>Secure every identity. Govern every connection.</h2>
          <p>
            Enterprise-grade administration for users, applications, access policies, and security
            operations from one trusted control center.
          </p>
          <ul className="auth-trust-list">
            <li>
              <ShieldCheck size={19} aria-hidden="true" /> Policy-led security
            </li>
            <li>
              <LockKeyhole size={19} aria-hidden="true" /> Standards-based access
            </li>
            <li>
              <BadgeCheck size={19} aria-hidden="true" /> Operational confidence
            </li>
          </ul>
        </div>

        <div className="auth-brand-footer">
          <span>HCLTech</span>
          <span className="auth-brand-footer-line" aria-hidden="true" />
          <span>Supercharging Progress</span>
        </div>
      </section>

      <section className="login-card">{children}</section>
    </main>
  );
}
