IF DB_ID('ClinicDB') IS NULL CREATE DATABASE ClinicDB;
GO
USE ClinicDB;
GO
IF OBJECT_ID('ChiTietDonThuoc') IS NOT NULL DROP TABLE ChiTietDonThuoc;
IF OBJECT_ID('DonThuoc') IS NOT NULL DROP TABLE DonThuoc;
IF OBJECT_ID('Thuoc') IS NOT NULL DROP TABLE Thuoc;
IF OBJECT_ID('HoSoKham') IS NOT NULL DROP TABLE HoSoKham;
IF OBJECT_ID('BenhNhan') IS NOT NULL DROP TABLE BenhNhan;
GO
CREATE TABLE BenhNhan (
    MaBN         INT IDENTITY(1,1) PRIMARY KEY,
    HoTen        NVARCHAR(100) NOT NULL,
    SoDienThoai  VARCHAR(15) UNIQUE,
    NamSinh      INT
);
CREATE TABLE HoSoKham (
    MaHoSo      INT IDENTITY(1,1) PRIMARY KEY,
    MaBN        INT NOT NULL FOREIGN KEY REFERENCES BenhNhan(MaBN),
    NgayKham    DATETIME DEFAULT GETDATE(),
    TrieuChung  NVARCHAR(MAX),
    ChanDoan    NVARCHAR(250)
);
CREATE TABLE Thuoc (
    MaThuoc     INT IDENTITY(1,1) PRIMARY KEY,
    TenThuoc    NVARCHAR(150) NOT NULL,
    DonViTinh   NVARCHAR(20),
    DonGia      DECIMAL(18,2) NOT NULL DEFAULT 0,
    SoLuongTon  INT NOT NULL DEFAULT 0 CHECK (SoLuongTon >= 0)
);
CREATE TABLE DonThuoc (
    MaDon      INT IDENTITY(1,1) PRIMARY KEY,
    MaHoSo     INT NOT NULL FOREIGN KEY REFERENCES HoSoKham(MaHoSo),
    TienKham   DECIMAL(18,2) DEFAULT 0,
    TongTien   DECIMAL(18,2) DEFAULT 0,
    TrangThai  NVARCHAR(30) DEFAULT N'Chờ lấy thuốc'
);
CREATE TABLE ChiTietDonThuoc (
    MaDon     INT NOT NULL FOREIGN KEY REFERENCES DonThuoc(MaDon),
    MaThuoc   INT NOT NULL FOREIGN KEY REFERENCES Thuoc(MaThuoc),
    SoLuong   INT NOT NULL CHECK (SoLuong > 0),
    LieuDung  NVARCHAR(100),
    PRIMARY KEY (MaDon, MaThuoc)
);
GO
INSERT INTO BenhNhan(HoTen, SoDienThoai, NamSinh) VALUES
(N'Nguyễn Văn Hải', '0901234567', 1985),
(N'Trần Thị Mai', '0912345678', 1992),
(N'Lê Hoàng Nam', '0987654321', 2001);
INSERT INTO HoSoKham(MaBN, NgayKham, TrieuChung, ChanDoan) VALUES
(1, DATEADD(day,-5,GETDATE()), N'Sốt, đau họng, ho khan', N'Viêm họng cấp'),
(2, DATEADD(day,-1,GETDATE()), N'Đau đầu, chóng mặt', N'Thiếu máu não nhẹ'),
(3, GETDATE(), N'Đau bụng vùng thượng vị sau ăn', NULL);
INSERT INTO Thuoc(TenThuoc, DonViTinh, DonGia, SoLuongTon) VALUES
(N'Paracetamol 500mg', N'Viên', 1000, 500),
(N'Amoxicillin 500mg', N'Viên', 2500, 300),
(N'Vitamin C 500mg', N'Vỉ', 15000, 80),
(N'Siro ho Prospan', N'Chai', 85000, 20),
(N'Omeprazol 20mg', N'Viên', 3000, 150),
(N'Ginkgo Biloba 120mg', N'Vỉ', 45000, 3);
INSERT INTO DonThuoc(MaHoSo, TienKham, TongTien, TrangThai) VALUES
(1, 100000, 150000, N'Đã thanh toán'),
(2, 100000, 240000, N'Chờ lấy thuốc');
INSERT INTO ChiTietDonThuoc(MaDon, MaThuoc, SoLuong, LieuDung) VALUES
(1, 1, 10, N'Sáng 1 viên, tối 1 viên sau ăn'),
(1, 2, 10, N'Sáng 1 viên, chiều 1 viên sau ăn'),
(1, 3, 1, N'Ngày 1 viên'),
(2, 6, 3, N'Sáng 1 viên sau ăn'),
(2, 1, 5, N'Uống khi đau đầu');
GO
