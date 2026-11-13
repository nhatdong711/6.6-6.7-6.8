IF DB_ID('BilliardsDB') IS NULL CREATE DATABASE BilliardsDB;
GO
USE BilliardsDB;
GO
IF OBJECT_ID('ChiTietHoaDon') IS NOT NULL DROP TABLE ChiTietHoaDon;
IF OBJECT_ID('HoaDon') IS NOT NULL DROP TABLE HoaDon;
IF OBJECT_ID('MenuDichVu') IS NOT NULL DROP TABLE MenuDichVu;
IF OBJECT_ID('BanBida') IS NOT NULL DROP TABLE BanBida;
GO
CREATE TABLE BanBida (
    MaBan       INT IDENTITY(1,1) PRIMARY KEY,
    TenBan      NVARCHAR(50) NOT NULL,
    LoaiBan     NVARCHAR(30),
    GiaTheoGio  DECIMAL(18,2) NOT NULL,
    TrangThai   NVARCHAR(20) NOT NULL DEFAULT N'Trống'
);
CREATE TABLE MenuDichVu (
    MaDV    INT IDENTITY(1,1) PRIMARY KEY,
    TenDV   NVARCHAR(100) NOT NULL,
    DonGia  DECIMAL(18,2) NOT NULL,
    TonKho  INT NULL
);
CREATE TABLE HoaDon (
    MaHD         INT IDENTITY(1,1) PRIMARY KEY,
    MaBan        INT NOT NULL FOREIGN KEY REFERENCES BanBida(MaBan),
    GioBatDau    DATETIME NOT NULL,
    GioKetThuc   DATETIME NULL,
    TienGioChoi  DECIMAL(18,2) DEFAULT 0,
    TienDichVu   DECIMAL(18,2) DEFAULT 0,
    TrangThai    BIT NOT NULL DEFAULT 0
);
CREATE TABLE ChiTietHoaDon (
    MaHD     INT NOT NULL FOREIGN KEY REFERENCES HoaDon(MaHD),
    MaDV     INT NOT NULL FOREIGN KEY REFERENCES MenuDichVu(MaDV),
    SoLuong  INT NOT NULL CHECK (SoLuong > 0),
    DonGia   DECIMAL(18,2) NOT NULL,
    PRIMARY KEY (MaHD, MaDV)
);
GO
INSERT INTO BanBida(TenBan, LoaiBan, GiaTheoGio, TrangThai) VALUES
(N'Bàn Lỗ 01', N'Pool (Lỗ)', 60000, N'Trống'),
(N'Bàn Lỗ 02', N'Pool (Lỗ)', 60000, N'Trống'),
(N'Bàn Lỗ 03', N'Pool (Lỗ)', 60000, N'Trống'),
(N'Bàn Carom 01', N'Carom (Phăng)', 50000, N'Trống'),
(N'Bàn Carom 02', N'Carom (Phăng)', 50000, N'Trống'),
(N'Bàn Carom 03', N'Carom (Phăng)', 50000, N'Trống'),
(N'Bàn Snooker 01', N'Snooker', 120000, N'Trống'),
(N'Bàn Snooker 02', N'Snooker', 120000, N'Trống');
INSERT INTO MenuDichVu(TenDV, DonGia, TonKho) VALUES
(N'Nước tăng lực Sting', 15000, 100),
(N'Trà đá', 5000, 200),
(N'Nước suối Aquafina', 10000, 120),
(N'Bia Tiger', 25000, 80),
(N'Mì xào bò', 45000, 30),
(N'Thuốc lá Vinataba', 30000, 2),
(N'Thuê gậy Carbon', 30000, NULL),
(N'Thuê găng tay', 10000, NULL);
INSERT INTO HoaDon(MaBan, GioBatDau, GioKetThuc, TienGioChoi, TienDichVu, TrangThai) VALUES
(1, DATEADD(day,-1,DATEADD(minute,-120,GETDATE())), DATEADD(day,-1,GETDATE()), 120000, 75000, 1),
(7, DATEADD(day,-2,DATEADD(minute,-90,GETDATE())), DATEADD(day,-2,GETDATE()), 180000, 60000, 1),
(2, DATEADD(minute,-75,GETDATE()), NULL, 0, 0, 0),
(4, DATEADD(minute,-35,GETDATE()), NULL, 0, 0, 0);
INSERT INTO ChiTietHoaDon(MaHD, MaDV, SoLuong, DonGia) VALUES
(1,1,3,15000),(1,7,1,30000),(2,4,2,25000),(2,2,2,5000),(3,1,2,15000),(3,7,1,30000),(4,2,2,5000);
UPDATE BanBida SET TrangThai = N'Đang chơi' WHERE MaBan IN (2,4);
GO
