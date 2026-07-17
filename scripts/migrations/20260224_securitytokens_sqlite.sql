-- Apply once per SQLite database.
ALTER TABLE "Zentra_SecurityTokens" ADD COLUMN "ConsumedAt" TEXT NULL;
ALTER TABLE "Zentra_SecurityTokens" ADD COLUMN "TokenReuseDetected" INTEGER NOT NULL DEFAULT 0;
CREATE INDEX IF NOT EXISTS "IX_SECTOK_TOKTYPE_KEY" ON "Zentra_SecurityTokens" ("TokenType", "Key");
