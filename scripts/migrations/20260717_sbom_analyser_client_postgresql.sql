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
    'hcl-cs-bootstrap', NULL, 'sbom-analyser-api', 'SBOM Analyser API',
    'OAuth resource and audience for the SBOM Analyser API', TRUE
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
    'hcl-cs-bootstrap', NULL, r."Id", 'sbom-analyser-api', 'Access SBOM Analyser',
    'Access the SBOM Analyser API as the signed-in user', FALSE, FALSE
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
    ('53424f4d-0000-4000-8000-000000000003'::uuid, 'role'),
    ('53424f4d-0000-4000-8000-000000000004'::uuid, 'tenant_id')
) AS claims(claim_id, claim_type)
WHERE r."Name" = 'sbom-analyser-api'
ON CONFLICT ("ApiResourceId", "Type") DO UPDATE SET "IsDeleted" = FALSE;

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
    'hcl-cs-bootstrap', NULL, 'sbom-analyser-web', 'SBOM Analyser Web',
    'https://localhost:3000', EXTRACT(EPOCH FROM CURRENT_TIMESTAMP)::bigint, 0,
    NULL, NULL, NULL, NULL, 86400, 3600, 3600, 300, 300, 0,
    TRUE, FALSE, FALSE, TRUE, TRUE,
    'openid profile email offline_access sbom-analyser-api', FALSE, 2, 'RS256',
    'authorization_code refresh_token', 'code', FALSE, NULL, FALSE, NULL,
    'sbom-analyser-api'
WHERE NOT EXISTS (
    SELECT 1 FROM "HclCs_Clients" WHERE "ClientId" = 'sbom-analyser-web'
);

UPDATE "HclCs_Clients" SET
    "ClientName" = 'SBOM Analyser Web',
    "ClientUri" = 'https://localhost:3000',
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
