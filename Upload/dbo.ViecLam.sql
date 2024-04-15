CREATE TABLE [dbo].[ViecLam] (
    [MaCongViec]       INT            IDENTITY (1, 1) NOT NULL,
    [TenVieclam]       NVARCHAR (255) NULL,
    [DiaDiemLamViec]   NVARCHAR (255) NULL,
    [LoaiViecLam]      NVARCHAR (255) NULL,
    [Luong]            MONEY          NULL,
    [MoTaCongViec]     NVARCHAR (255) NULL,
    [YeuCauCongViec]   NVARCHAR (255) NULL,
    [QuyenLoiCongViec] NVARCHAR (255) NULL,
    [NgayDang]         DATE           NULL,
    [NgayHetHan]       DATE           NULL,
    [Hinh]             NVARCHAR (500) NULL,
    [TrangThai]        BIT            CONSTRAINT [DF_ViecLam_TrangThai] DEFAULT ((0)) NULL,
    [MaNTD]            INT            NULL,
    [MaKH]             INT            NULL,
    [Mal]              INT            NULL,
    PRIMARY KEY CLUSTERED ([MaCongViec] ASC),
    FOREIGN KEY ([MaNTD]) REFERENCES [dbo].[NhaTuyenDung] ([MaNTD]),
    CONSTRAINT [FK_ViecLam_KhachHangs] FOREIGN KEY ([MaKH]) REFERENCES [dbo].[KhachHang] ([MaKH]),
    CONSTRAINT [FK_ViecLam_LoaiVieclams] FOREIGN KEY ([Mal]) REFERENCES [dbo].[LoaiVieclam] ([Mal])
);

