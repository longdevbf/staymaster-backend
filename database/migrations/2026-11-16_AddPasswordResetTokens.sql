BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261116124336_AddPasswordResetTokens'
)
BEGIN
    CREATE TABLE [ma_dat_lai_mat_khau] (
        [id] int NOT NULL IDENTITY,
        [nguoi_dung_id] int NOT NULL,
        [ma_bam] nvarchar(64) NOT NULL,
        [het_han] datetime2 NOT NULL,
        [da_dung_luc] datetime2 NULL,
        [ngay_tao] datetime2 NOT NULL,
        CONSTRAINT [PK_ma_dat_lai_mat_khau] PRIMARY KEY ([id]),
        CONSTRAINT [FK_ma_dat_lai_mat_khau_nguoi_dung_nguoi_dung_id] FOREIGN KEY ([nguoi_dung_id]) REFERENCES [nguoi_dung] ([id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261116124336_AddPasswordResetTokens'
)
BEGIN
    CREATE UNIQUE INDEX [IX_ma_dat_lai_mat_khau_ma_bam] ON [ma_dat_lai_mat_khau] ([ma_bam]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261116124336_AddPasswordResetTokens'
)
BEGIN
    CREATE INDEX [IX_ma_dat_lai_mat_khau_nguoi_dung_id] ON [ma_dat_lai_mat_khau] ([nguoi_dung_id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261116124336_AddPasswordResetTokens'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261116124336_AddPasswordResetTokens', N'9.0.0');
END;

COMMIT;
GO

