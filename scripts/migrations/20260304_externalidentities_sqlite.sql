-- Apply once per SQLite database.
CREATE TABLE IF NOT EXISTS "Zentra_ExternalIdentities" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Zentra_ExternalIdentities" PRIMARY KEY,
    "IsDeleted" INTEGER NOT NULL DEFAULT 0,
    "CreatedOn" TEXT NOT NULL,
    "ModifiedOn" TEXT NULL,
    "CreatedBy" TEXT NOT NULL,
    "ModifiedBy" TEXT NULL,
    "RowVersion" BLOB NULL,
    "UserId" TEXT NOT NULL,
    "TenantId" TEXT NULL,
    "Provider" TEXT NOT NULL,
    "Issuer" TEXT NOT NULL,
    "Subject" TEXT NOT NULL,
    "Email" TEXT NOT NULL,
    "EmailVerified" INTEGER NOT NULL,
    "LinkedAt" TEXT NOT NULL,
    "LastSignInAt" TEXT NULL,
    CONSTRAINT "FK_Zentra_ExternalIdentities_Zentra_Users_UserId"
        FOREIGN KEY ("UserId") REFERENCES "Zentra_Users" ("Id") ON DELETE RESTRICT
);

CREATE UNIQUE INDEX IF NOT EXISTS "IX_EXTID_PROVIDER_ISSUER_SUBJECT"
    ON "Zentra_ExternalIdentities" ("Provider", "Issuer", "Subject");

CREATE INDEX IF NOT EXISTS "IX_EXTID_USERID"
    ON "Zentra_ExternalIdentities" ("UserId");

CREATE INDEX IF NOT EXISTS "IX_EXTID_TENANT_EMAIL"
    ON "Zentra_ExternalIdentities" ("TenantId", "Email");
