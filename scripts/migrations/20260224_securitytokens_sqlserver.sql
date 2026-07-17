IF COL_LENGTH('Zentra_SecurityTokens', 'ConsumedAt') IS NULL
BEGIN
    ALTER TABLE [Zentra_SecurityTokens] ADD [ConsumedAt] datetime2 NULL;
END;
GO

IF COL_LENGTH('Zentra_SecurityTokens', 'TokenReuseDetected') IS NULL
BEGIN
    ALTER TABLE [Zentra_SecurityTokens] ADD [TokenReuseDetected] bit NOT NULL CONSTRAINT [DF_Zentra_SecurityTokens_TokenReuseDetected] DEFAULT (0);
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_SECTOK_TOKTYPE_KEY' AND object_id = OBJECT_ID('Zentra_SecurityTokens'))
BEGIN
    CREATE INDEX [IX_SECTOK_TOKTYPE_KEY] ON [Zentra_SecurityTokens] ([TokenType], [Key]);
END;
GO
