/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

-- Idempotent HCL.CS OAuth/OIDC registration for the SBOM Analyser public client.
-- No secrets or private key material are stored by this migration.

ALTER TABLE "HclCs_Clients"
    ADD COLUMN IF NOT EXISTS "PreferredAudience" character varying(300) NULL;

INSERT INTO "HclCs_ApiResources" (
    "Id", "IsDeleted", "CreatedOn", "ModifiedOn", "CreatedBy", "ModifiedBy",
    "Name", "DisplayName", "Description", "Enabled"
) VALUES (
    '53424f4d-0000-4000-8000-000000000001', FALSE, CURRENT_TIMESTAMP, NULL,
    'hcl-cs-bootstrap', NULL, 'sbom-analyser-api', 'SBOM Analyzer API',
    'OAuth resource and audience for the SBOM Analyzer API', TRUE
)
ON CONFLICT ("Name") DO UPDATE SET
    "DisplayName" = EXCLUDED."DisplayName",
    "Description" = EXCLUDED."Description",
    "Enabled" = TRUE,
    "IsDeleted" = FALSE;

INSERT INTO "HclCs_ApiScopes" (
    "Id", "IsDeleted", "CreatedOn", "ModifiedOn", "CreatedBy", "ModifiedBy",
    "ApiResourceId", "Name", "DisplayName", "Description", "Required", "Emphasize"
)
SELECT
    '53424f4d-0000-4000-8000-000000000002', FALSE, CURRENT_TIMESTAMP, NULL,
    'hcl-cs-bootstrap', NULL, r."Id", 'sbom-analyser-api', 'Access SBOM Analyzer',
    'Access the SBOM Analyzer API as the signed-in user', FALSE, FALSE
FROM "HclCs_ApiResources" r
WHERE r."Name" = 'sbom-analyser-api'
ON CONFLICT ("ApiResourceId", "Name") DO UPDATE SET
    "DisplayName" = EXCLUDED."DisplayName",
    "Description" = EXCLUDED."Description",
    "IsDeleted" = FALSE;

INSERT INTO "HclCs_ApiResourceClaims" (
    "Id", "IsDeleted", "CreatedOn", "ModifiedOn", "CreatedBy", "ModifiedBy",
    "ApiResourceId", "Type"
)
SELECT claim_id, FALSE, CURRENT_TIMESTAMP, NULL, 'hcl-cs-bootstrap', NULL, r."Id", claim_type
FROM "HclCs_ApiResources" r
CROSS JOIN (VALUES
    ('53424f4d-0000-4000-8000-000000000020'::uuid, 'sub'),
    ('53424f4d-0000-4000-8000-000000000021'::uuid, 'email'),
    ('53424f4d-0000-4000-8000-000000000022'::uuid, 'name'),
    ('53424f4d-0000-4000-8000-000000000023'::uuid, 'preferred_username'),
    ('53424f4d-0000-4000-8000-000000000024'::uuid, 'employee_id'),
    ('53424f4d-0000-4000-8000-000000000025'::uuid, 'department')
) AS claims(claim_id, claim_type)
WHERE r."Name" = 'sbom-analyser-api'
ON CONFLICT ("ApiResourceId", "Type") DO UPDATE SET "IsDeleted" = FALSE;

UPDATE "HclCs_ApiResourceClaims" claim
SET "IsDeleted" = TRUE,
    "ModifiedOn" = CURRENT_TIMESTAMP,
    "ModifiedBy" = 'hcl-cs-bootstrap'
FROM "HclCs_ApiResources" resource
WHERE claim."ApiResourceId" = resource."Id"
  AND resource."Name" = 'sbom-analyser-api'
  AND claim."Type" IN ('role', 'tenant_id');

INSERT INTO "HclCs_IdentityClaims" (
    "Id", "IsDeleted", "CreatedOn", "ModifiedOn", "CreatedBy", "ModifiedBy",
    "IdentityResourceId", "Type", "AliasType"
)
SELECT claim_id, FALSE, CURRENT_TIMESTAMP, NULL, 'hcl-cs-bootstrap', NULL,
       resource."Id", claim_type, alias_type
FROM "HclCs_IdentityResources" resource
CROSS JOIN (VALUES
    ('53424f4d-0000-4000-8000-000000000030'::uuid, 'name', 'displayname'),
    ('53424f4d-0000-4000-8000-000000000031'::uuid, 'preferred_username', 'userprincipalname'),
    ('53424f4d-0000-4000-8000-000000000032'::uuid, 'employee_id', 'employeeid'),
    ('53424f4d-0000-4000-8000-000000000033'::uuid, 'department', 'department')
) AS claims(claim_id, claim_type, alias_type)
WHERE resource."Name" = 'profile'
ON CONFLICT ("IdentityResourceId", "Type") DO UPDATE SET
    "AliasType" = EXCLUDED."AliasType",
    "IsDeleted" = FALSE;

INSERT INTO "HclCs_Clients" (
    "Id", "IsDeleted", "CreatedOn", "ModifiedOn", "CreatedBy", "ModifiedBy",
    "ClientId", "ClientName", "ClientUri", "ClientIdIssuedAt", "ClientSecretExpiresAt",
    "ClientSecret", "LogoUri", "TermsOfServiceUri", "PolicyUri",
    "RefreshTokenExpiration", "AccessTokenExpiration", "IdentityTokenExpiration",
    "LogoutTokenExpiration", "AuthorizationCodeExpiration", "AccessTokenType",
    "RequirePkce", "IsPkceTextPlain", "RequireClientSecret", "IsFirstPartyApp",
    "AllowOfflineAccess", "AllowedScopes", "AllowAccessTokensViaBrowser",
    "ApplicationType", "AllowedSigningAlgorithm", "SupportedGrantTypes",
    "SupportedResponseTypes", "FrontChannelLogoutSessionRequired", "FrontChannelLogoutUri",
    "BackChannelLogoutSessionRequired", "BackChannelLogoutUri", "PreferredAudience"
) SELECT
    '53424f4d-0000-4000-8000-000000000010', FALSE, CURRENT_TIMESTAMP, NULL,
    'hcl-cs-bootstrap', NULL, 'sbom-analyser-web', 'SBOM Analyzer Web',
    'https://localhost:3000', EXTRACT(EPOCH FROM CURRENT_TIMESTAMP)::bigint, 0,
    NULL, NULL, NULL, NULL, 86400, 3600, 3600, 300, 300, 1,
    TRUE, FALSE, FALSE, TRUE, TRUE,
    'openid profile email offline_access sbom-analyser-api', FALSE, 2, 'RS256',
    'authorization_code refresh_token', 'code', FALSE, NULL, FALSE, NULL,
    'sbom-analyser-api'
WHERE NOT EXISTS (
    SELECT 1 FROM "HclCs_Clients" WHERE "ClientId" = 'sbom-analyser-web'
);

UPDATE "HclCs_Clients" SET
    "ClientName" = 'SBOM Analyzer Web',
    "ClientUri" = 'https://localhost:3000',
    "RefreshTokenExpiration" = 86400,
    "AccessTokenExpiration" = 3600,
    "IdentityTokenExpiration" = 3600,
    "LogoutTokenExpiration" = 300,
    "AuthorizationCodeExpiration" = 300,
    "AccessTokenType" = 1,
    "RequirePkce" = TRUE,
    "IsPkceTextPlain" = FALSE,
    "RequireClientSecret" = FALSE,
    "AllowOfflineAccess" = TRUE,
    "AllowedScopes" = 'openid profile email offline_access sbom-analyser-api',
    "AllowAccessTokensViaBrowser" = FALSE,
    "ApplicationType" = 2,
    "AllowedSigningAlgorithm" = 'RS256',
    "SupportedGrantTypes" = 'authorization_code refresh_token',
    "SupportedResponseTypes" = 'code',
    "PreferredAudience" = 'sbom-analyser-api',
    "IsDeleted" = FALSE
WHERE "ClientId" = 'sbom-analyser-web';

INSERT INTO "HclCs_ClientRedirectUris" (
    "Id", "IsDeleted", "CreatedOn", "ModifiedOn", "CreatedBy", "ModifiedBy",
    "ClientId", "RedirectUri"
)
SELECT '53424f4d-0000-4000-8000-000000000011', FALSE, CURRENT_TIMESTAMP, NULL,
       'hcl-cs-bootstrap', NULL, c."Id", 'https://localhost:3000/auth/callback'
FROM "HclCs_Clients" c WHERE c."ClientId" = 'sbom-analyser-web'
ON CONFLICT ("ClientId", "RedirectUri") DO UPDATE SET "IsDeleted" = FALSE;

INSERT INTO "HclCs_ClientPostLogoutRedirectUris" (
    "Id", "IsDeleted", "CreatedOn", "ModifiedOn", "CreatedBy", "ModifiedBy",
    "ClientId", "PostLogoutRedirectUri"
)
SELECT '53424f4d-0000-4000-8000-000000000012', FALSE, CURRENT_TIMESTAMP, NULL,
       'hcl-cs-bootstrap', NULL, c."Id", 'https://localhost:3000'
FROM "HclCs_Clients" c WHERE c."ClientId" = 'sbom-analyser-web'
ON CONFLICT ("ClientId", "PostLogoutRedirectUri") DO UPDATE SET "IsDeleted" = FALSE;
