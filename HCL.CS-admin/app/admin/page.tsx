/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

import Link from "next/link";

import { listClientNames } from "@/lib/api/clients";
import { listRoles } from "@/lib/api/roles";
import { listUsers } from "@/lib/api/users";
import { searchAudit } from "@/lib/api/audit";
import { getLoadErrorInfo } from "@/lib/api/client";
import { SignInAgainButton } from "@/components/admin/SignInAgainButton";
import { type AuditTrailModel } from "@/lib/types/hcl-cs";

export default async function AdminDashboardPage() {
  let usersCount = 0;
  let rolesCount = 0;
  let clientsCount = 0;
  let recentAudit: AuditTrailModel[] = [];
  let loadErrorMessage: string | null = null;
  let isUnauthorized = false;

  try {
    const [users, roles, clientNames, audit] = await Promise.all([
      listUsers(),
      listRoles(),
      listClientNames(),
      searchAudit({
        page: {
          TotalItems: 0,
          ItemsPerPage: 5,
          CurrentPage: 1,
          TotalPages: 0,
          TotalDisplayPages: 5
        }
      })
    ]);
    usersCount = users.length;
    rolesCount = roles.length;
    clientsCount = Object.keys(clientNames).length;
    recentAudit = audit?.AuditList ?? [];
  } catch (error) {
    const info = getLoadErrorInfo(error);
    loadErrorMessage = info.message;
    isUnauthorized = info.isUnauthorized;
  }

  const cards = [
    {
      title: "Users",
      count: loadErrorMessage ? "—" : usersCount,
      href: "/admin/users",
      cta: "Manage users"
    },
    {
      title: "Roles & Claims",
      count: loadErrorMessage ? "—" : rolesCount,
      href: "/admin/roles",
      cta: "Manage roles"
    },
    {
      title: "Clients",
      count: loadErrorMessage ? "—" : clientsCount,
      href: "/admin/clients",
      cta: "Manage clients"
    }
  ];

  return (
    <>
      <section className="card">
        <header className="card-head">
          <div>
            <h2 className="text-heading">Control center</h2>
            <p className="inline-message">Overview and quick actions for your HCL.CS identity platform.</p>
          </div>
        </header>
        <div className="card-body">
          {loadErrorMessage ? (
            <section className="card" style={{ marginBottom: "1rem" }}>
              <div className="card-body" style={{ display: "grid", gap: "0.7rem" }}>
                <p className="inline-message error">{loadErrorMessage}</p>
                <div className="toolbar" style={{ gap: "0.5rem" }}>
                  {isUnauthorized ? <SignInAgainButton /> : null}
                  <Link href="/admin" className="btn btn-secondary">
                    Retry
                  </Link>
                </div>
              </div>
            </section>
          ) : null}

          <div className="dashboard-grid">
            {cards.map((item) => (
              <Link key={item.href} href={item.href} className="dashboard-card">
                <div className="text-caption" style={{ marginBottom: "var(--space-1)" }}>
                  {item.title}
                </div>
                <div
                  className="text-display"
                  style={{ fontSize: "1.75rem", marginBottom: "var(--space-2)" }}
                >
                  {item.count}
                </div>
                <span className="text-caption" style={{ color: "var(--accent)" }}>
                  {item.cta} →
                </span>
              </Link>
            ))}
          </div>

          <div style={{ marginTop: "var(--space-6)" }}>
            <h3 className="text-heading" style={{ marginBottom: "var(--space-2)" }}>
              Quick actions
            </h3>
            <div className="toolbar dashboard-actions">
              <Link href="/admin/users" className="btn btn-secondary">
                Create user
              </Link>
              <Link href="/admin/roles" className="btn btn-secondary">
                Create role
              </Link>
              <Link href="/admin/clients" className="btn btn-secondary">
                Register client
              </Link>
              <Link href="/admin/audit" className="btn btn-ghost">
                Audit log
              </Link>
              <Link href="/admin/operations/api-explorer" className="btn btn-ghost">
                API Explorer
              </Link>
            </div>
          </div>

          <div style={{ marginTop: "var(--space-6)" }}>
            <h3 className="text-heading" style={{ marginBottom: "var(--space-2)" }}>
              Recent activity
            </h3>
            {recentAudit.length === 0 ? (
              <p className="inline-message">No recent audit entries.</p>
            ) : (
              <ul className="text-body" style={{ listStyle: "none", padding: 0, margin: 0 }}>
                {recentAudit.map((entry) => (
                  <li
                    key={entry.Id}
                    style={{
                      paddingBlock: "var(--space-2)",
                      borderBottom: "1px solid var(--border-subtle)"
                    }}
                  >
                    <div className="text-caption" style={{ marginBottom: "2px" }}>
                      {entry.ActionName} on {entry.TableName}
                    </div>
                    <div className="text-caption">
                      {entry.CreatedBy ?? "system"} ·{" "}
                      <span className="text-mono">{entry.CreatedOn}</span>
                    </div>
                  </li>
                ))}
              </ul>
            )}
          </div>
        </div>
      </section>
    </>
  );
}
