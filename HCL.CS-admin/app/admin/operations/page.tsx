/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

import Link from "next/link";

export default async function OperationsPage() {
  return (
    <section className="card">
      <header className="card-head">
        <div>
          <h2>Operations</h2>
          <p className="inline-message">
            Protocol-level tooling for HCL.CS: OAuth/OIDC endpoints, gateway APIs, installer, and demo server flows.
          </p>
        </div>
      </header>
      <div className="card-body" style={{ display: "grid", gap: "1rem" }}>
        <div>
          <h3 className="text-heading" style={{ marginBottom: "0.35rem" }}>
            Protocol &amp; OAuth flows
          </h3>
          <p className="inline-message" style={{ marginBottom: "0.5rem" }}>
            Explore and exercise the HCL.CS OAuth/OIDC endpoints (authorize, token, introspect, revocation, userinfo,
            discovery).
          </p>
          <Link className="btn btn-secondary" href="/admin/operations/api-explorer">
            Open API Explorer
          </Link>
        </div>

        <div>
          <h3 className="text-heading" style={{ marginBottom: "0.35rem" }}>
            Installer &amp; environment
          </h3>
          <p className="inline-message" style={{ marginBottom: "0.5rem" }}>
            Navigate the installer endpoints (setup, validate, migrate, seed) using presets inside the API Explorer.
          </p>
          <p className="text-caption">
            In the API Explorer, switch the base to <strong>installer</strong> to work against the installer host.
          </p>
        </div>

        <div>
          <h3 className="text-heading" style={{ marginBottom: "0.35rem" }}>
            Demo server
          </h3>
          <p className="inline-message" style={{ marginBottom: "0.5rem" }}>
            Call the Demo Server external auth endpoints for end-to-end identity testing.
          </p>
          <p className="text-caption">
            In the API Explorer, switch the base to <strong>demo</strong> to work against the Demo Server host.
          </p>
        </div>
      </div>
    </section>
  );
}
