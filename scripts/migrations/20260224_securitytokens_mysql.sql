ALTER TABLE `HclCs_SecurityTokens`
    ADD COLUMN IF NOT EXISTS `ConsumedAt` datetime(6) NULL;

ALTER TABLE `HclCs_SecurityTokens`
    ADD COLUMN IF NOT EXISTS `TokenReuseDetected` tinyint(1) NOT NULL DEFAULT FALSE;

CREATE INDEX `IX_SECTOK_TOKTYPE_KEY`
    ON `HclCs_SecurityTokens` (`TokenType`(64), `Key`(255));
