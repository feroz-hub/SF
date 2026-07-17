ALTER TABLE "HclCs_SecurityTokens"
    ADD COLUMN IF NOT EXISTS "ConsumedAt" timestamp with time zone NULL;

ALTER TABLE "HclCs_SecurityTokens"
    ADD COLUMN IF NOT EXISTS "TokenReuseDetected" boolean NOT NULL DEFAULT FALSE;

CREATE INDEX IF NOT EXISTS "IX_SECTOK_TOKTYPE_KEY"
    ON "HclCs_SecurityTokens" ("TokenType", "Key");
