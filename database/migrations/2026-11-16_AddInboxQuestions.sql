BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261116125527_AddInboxQuestions'
)
BEGIN
    CREATE TABLE [hop_thu_cau_hoi] (
        [id] int NOT NULL IDENTITY,
        [khach_hang_id] int NOT NULL,
        [doi_tac_id] int NULL,
        [phong_id] int NULL,
        [cau_hoi] nvarchar(2000) NOT NULL,
        [tra_loi] nvarchar(4000) NULL,
        [nguoi_tra_loi_id] int NULL,
        [nguon] nvarchar(20) NOT NULL,
        [trang_thai] nvarchar(20) NOT NULL,
        [ngay_tao] datetime2 NOT NULL,
        [ngay_tra_loi] datetime2 NULL,
        CONSTRAINT [PK_hop_thu_cau_hoi] PRIMARY KEY ([id]),
        CONSTRAINT [FK_hop_thu_cau_hoi_doi_tac_doi_tac_id] FOREIGN KEY ([doi_tac_id]) REFERENCES [doi_tac] ([id]),
        CONSTRAINT [FK_hop_thu_cau_hoi_nguoi_dung_khach_hang_id] FOREIGN KEY ([khach_hang_id]) REFERENCES [nguoi_dung] ([id]) ON DELETE CASCADE,
        CONSTRAINT [FK_hop_thu_cau_hoi_phong_phong_id] FOREIGN KEY ([phong_id]) REFERENCES [phong] ([id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261116125527_AddInboxQuestions'
)
BEGIN
    CREATE INDEX [IX_hop_thu_cau_hoi_doi_tac_id_trang_thai] ON [hop_thu_cau_hoi] ([doi_tac_id], [trang_thai]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261116125527_AddInboxQuestions'
)
BEGIN
    CREATE INDEX [IX_hop_thu_cau_hoi_khach_hang_id] ON [hop_thu_cau_hoi] ([khach_hang_id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261116125527_AddInboxQuestions'
)
BEGIN
    CREATE INDEX [IX_hop_thu_cau_hoi_phong_id] ON [hop_thu_cau_hoi] ([phong_id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261116125527_AddInboxQuestions'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261116125527_AddInboxQuestions', N'9.0.0');
END;

COMMIT;
GO

