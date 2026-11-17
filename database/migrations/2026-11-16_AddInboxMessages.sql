BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261116134046_AddInboxMessages'
)
BEGIN
    ALTER TABLE [hop_thu_cau_hoi] ADD [ngay_tin_nhan_cuoi] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261116134046_AddInboxMessages'
)
BEGIN
    CREATE TABLE [hop_thu_tin_nhan] (
        [id] int NOT NULL IDENTITY,
        [cau_hoi_id] int NOT NULL,
        [nguoi_gui_id] int NULL,
        [ben_gui] nvarchar(20) NOT NULL,
        [noi_dung] nvarchar(4000) NOT NULL,
        [ngay_tao] datetime2 NOT NULL,
        CONSTRAINT [PK_hop_thu_tin_nhan] PRIMARY KEY ([id]),
        CONSTRAINT [FK_hop_thu_tin_nhan_hop_thu_cau_hoi_cau_hoi_id] FOREIGN KEY ([cau_hoi_id]) REFERENCES [hop_thu_cau_hoi] ([id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261116134046_AddInboxMessages'
)
BEGIN
    CREATE INDEX [IX_hop_thu_tin_nhan_cau_hoi_id_ngay_tao] ON [hop_thu_tin_nhan] ([cau_hoi_id], [ngay_tao]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261116134046_AddInboxMessages'
)
BEGIN

    INSERT INTO hop_thu_tin_nhan (cau_hoi_id, nguoi_gui_id, ben_gui, noi_dung, ngay_tao)
    SELECT id, khach_hang_id, N'customer', cau_hoi, ngay_tao FROM hop_thu_cau_hoi;

    INSERT INTO hop_thu_tin_nhan (cau_hoi_id, nguoi_gui_id, ben_gui, noi_dung, ngay_tao)
    SELECT id, nguoi_tra_loi_id, CASE WHEN doi_tac_id IS NULL THEN N'support' ELSE N'host' END, tra_loi, COALESCE(ngay_tra_loi, ngay_tao)
    FROM hop_thu_cau_hoi WHERE tra_loi IS NOT NULL;

    UPDATE hop_thu_cau_hoi SET ngay_tin_nhan_cuoi = COALESCE(ngay_tra_loi, ngay_tao);

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261116134046_AddInboxMessages'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261116134046_AddInboxMessages', N'9.0.0');
END;

COMMIT;
GO

