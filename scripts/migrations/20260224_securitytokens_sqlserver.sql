IF COL_LENGTH('HclCs_SecurityTokens', 'ConsumedAt') IS NULL
BEGIN
    ALTER TABLE [HclCs_SecurityTokens] ADD [ConsumedAt] datetime2 NULL;
END;
GO

IF COL_LENGTH('HclCs_SecurityTokens', 'TokenReuseDetected') IS NULL
BEGIN
    ALTER TABLE [HclCs_SecurityTokens] ADD [TokenReuseDetected] bit NOT NULL CONSTRAINT [DF_HclCs_SecurityTokens_TokenReuseDetected] DEFAULT (0);
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_SECTOK_TOKTYPE_KEY' AND object_id = OBJECT_ID('HclCs_SecurityTokens'))
BEGIN
    CREATE INDEX [IX_SECTOK_TOKTYPE_KEY] ON [HclCs_SecurityTokens] ([TokenType], [Key]);
END;
GO
