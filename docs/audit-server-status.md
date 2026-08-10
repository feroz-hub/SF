<!--
Copyright (c) 2021 HCL CORPORATION.
All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
HCL is obtained. This is proprietary and confidential to HCL.
-->

# Server audit status

## Summary

- **Read (GetAuditDetails):** Working. The Identity server exposes `GetAuditDetailsAsync`; the Gateway forwards it; the admin calls it and displays results. Pagination and filters (actor, date range, action type, search) are applied. Total count used for paging uses the **filtered** count.
- **Write (AddAuditTrail):** 
  - **Role CRUD:** Audit records are written automatically when roles are created, updated, or deleted (via `RoleService` + `IAuditTrailService`). The table will populate when you add/edit/delete roles in the admin or via API.
  - **Other entities:** User, Client, ApiResource, IdentityResource, etc. do not yet write audit. Only the AddAuditTrail API or future wiring (like RoleService) can add entries for those.

## Server components

| Component | Role |
|-----------|------|
| `IAuditTrailService` / `AuditTrailService` | Add single/batch audit; get audit list with filters and paging |
| `IAuditRepository` / `AuditRepository` | Persistence: insert, filtered count, filtered list with paging |
| `AuditUtil` (internal) | Helpers to build audit models for Create/Update/Delete; **not used by any service** |
| Gateway `AuditTrailServiceRoute` | Exposes `GetAuditDetailsAsync`, `AddAuditTrail`, `AddAuditTrailModel` |
| `ApplicationDbContext.AuditTrail` | DbSet for the audit table |

## Behaviour

- **GetAuditDetails:** Request body is `AuditSearchRequestModel` (CreatedBy, FromDate, ToDate, ActionType, SearchValue, Page). Response is `AuditResponseModel` (AuditList, PageInfo). Empty result returns `AuditList = []` and valid PageInfo (no null).
- **Pagination:** `TotalItems` is the count of records matching the current filters. `TotalPages` is derived from `TotalItems` and `ItemsPerPage`.
- **Writing audit:** 
  - **Roles:** `RoleService` injects `IAuditTrailService` and calls `AddAuditTrailAsync` after successful create/update/delete (fire-and-forget; audit failure is logged but does not fail the operation).
  - **Others:** User, Client, ApiResource, IdentityResource services do not write audit yet. Use the AddAuditTrail API or wire `IAuditTrailService`/`AuditUtil` in those services to populate audit for them.

## Database

- The installer/migrations create the audit table (e.g. `AuditTrail` or equivalent per provider). The Identity `ApplicationDbContext` and the installer DbContexts both include the audit entity.
