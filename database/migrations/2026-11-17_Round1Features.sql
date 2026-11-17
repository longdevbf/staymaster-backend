BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261117041317_Round1Features'
)
BEGIN
    ALTER TABLE [phong] ADD [gia_cuoi_tuan] decimal(18,2) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261117041317_Round1Features'
)
BEGIN
    ALTER TABLE [phong] ADD [ly_do_tu_choi] nvarchar(500) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261117041317_Round1Features'
)
BEGIN
    ALTER TABLE [danh_gia] ADD [bi_an] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261117041317_Round1Features'
)
BEGIN
    ALTER TABLE [danh_gia] ADD [ly_do_an] nvarchar(500) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261117041317_Round1Features'
)
BEGIN
    ALTER TABLE [danh_gia] ADD [ngay_phan_hoi] datetime NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261117041317_Round1Features'
)
BEGIN
    ALTER TABLE [danh_gia] ADD [phan_hoi_chu_nha] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261117041317_Round1Features'
)
BEGIN
    CREATE TABLE [cau_hinh_he_thong] (
        [khoa] nvarchar(100) NOT NULL,
        [gia_tri] nvarchar(1000) NOT NULL,
        [ngay_cap_nhat] datetime NOT NULL,
        [nguoi_cap_nhat_id] int NULL,
        CONSTRAINT [PK_cau_hinh_he_thong] PRIMARY KEY ([khoa])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261117041317_Round1Features'
)
BEGIN
    CREATE TABLE [ngay_chan_phong] (
        [id] int NOT NULL IDENTITY,
        [phong_id] int NOT NULL,
        [tu_ngay] date NOT NULL,
        [den_ngay] date NOT NULL,
        [ly_do] nvarchar(255) NULL,
        [ngay_tao] datetime NOT NULL,
        CONSTRAINT [PK_ngay_chan_phong] PRIMARY KEY ([id]),
        CONSTRAINT [FK_ngay_chan_phong_phong_phong_id] FOREIGN KEY ([phong_id]) REFERENCES [phong] ([id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261117041317_Round1Features'
)
BEGIN
    ALTER TABLE [thong_bao] ADD [duong_dan] nvarchar(255) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261117041317_Round1Features'
)
BEGIN
    CREATE INDEX [IX_ngay_chan_phong_phong_id_tu_ngay] ON [ngay_chan_phong] ([phong_id], [tu_ngay]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261117041317_Round1Features'
)
BEGIN
    CREATE INDEX [ix_thong_bao_nguoi_dung_da_doc] ON [thong_bao] ([nguoi_dung_id], [da_doc]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261117041317_Round1Features'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261117041317_Round1Features', N'9.0.0');
END;

COMMIT;
GO

