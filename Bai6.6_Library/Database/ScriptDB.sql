IF DB_ID('LibraryDB') IS NULL CREATE DATABASE LibraryDB;
GO
USE LibraryDB;
GO
IF OBJECT_ID('ChiTietMuon') IS NOT NULL DROP TABLE ChiTietMuon;
IF OBJECT_ID('PhieuMuon') IS NOT NULL DROP TABLE PhieuMuon;
IF OBJECT_ID('DauSach') IS NOT NULL DROP TABLE DauSach;
IF OBJECT_ID('DocGia') IS NOT NULL DROP TABLE DocGia;
GO
CREATE TABLE DocGia (
    MaDocGia       INT IDENTITY(1,1) PRIMARY KEY,
    HoTen          NVARCHAR(100) NOT NULL,
    Email          VARCHAR(100) NULL,
    NgayHetHanThe  DATETIME
);
CREATE UNIQUE INDEX UQ_DocGia_Email ON DocGia(Email) WHERE Email IS NOT NULL;
CREATE TABLE DauSach (
    MaSach      INT IDENTITY(1,1) PRIMARY KEY,
    TenSach     NVARCHAR(200) NOT NULL,
    TheLoai     NVARCHAR(50),
    SoLuongCon  INT NOT NULL DEFAULT 0 CHECK (SoLuongCon >= 0)
);
CREATE TABLE PhieuMuon (
    MaPhieu    INT IDENTITY(1,1) PRIMARY KEY,
    MaDocGia   INT NOT NULL FOREIGN KEY REFERENCES DocGia(MaDocGia),
    NgayMuon   DATETIME DEFAULT GETDATE(),
    HanTra     DATETIME,
    TrangThai  NVARCHAR(50) NOT NULL DEFAULT N'Đang mượn'
);
CREATE TABLE ChiTietMuon (
    MaPhieu  INT NOT NULL FOREIGN KEY REFERENCES PhieuMuon(MaPhieu),
    MaSach   INT NOT NULL FOREIGN KEY REFERENCES DauSach(MaSach),
    SoLuong  INT NOT NULL CHECK (SoLuong > 0),
    PRIMARY KEY (MaPhieu, MaSach)
);
GO
INSERT INTO DocGia(HoTen, Email, NgayHetHanThe) VALUES
(N'Nguyễn Văn An', 'an.nv@gmail.com', DATEADD(year,1,GETDATE())),
(N'Trần Thị Bình', 'binh.tt@gmail.com', DATEADD(month,6,GETDATE())),
(N'Lê Minh Cường', 'cuong.lm@gmail.com', DATEADD(day,-10,GETDATE())),
(N'Phạm Thu Dung', 'dung.pt@gmail.com', DATEADD(year,2,GETDATE()));
INSERT INTO DauSach(TenSach, TheLoai, SoLuongCon) VALUES
(N'Lập trình C# căn bản', N'Công nghệ thông tin', 10),
(N'Cấu trúc dữ liệu và giải thuật', N'Công nghệ thông tin', 6),
(N'An toàn mạng máy tính', N'Công nghệ thông tin', 4),
(N'Kinh tế vi mô', N'Kinh tế', 8),
(N'Nhà giả kim', N'Văn học', 5),
(N'Số đỏ', N'Văn học', 1);
INSERT INTO PhieuMuon(MaDocGia, NgayMuon, HanTra, TrangThai) VALUES
(1, DATEADD(day,-20,GETDATE()), DATEADD(day,-6,GETDATE()), N'Đã trả'),
(2, DATEADD(day,-18,GETDATE()), DATEADD(day,-4,GETDATE()), N'Đang mượn'),
(4, DATEADD(day,-3,GETDATE()), DATEADD(day,11,GETDATE()), N'Đang mượn'),
(1, DATEADD(day,-30,GETDATE()), DATEADD(day,-16,GETDATE()), N'Quá hạn');
INSERT INTO ChiTietMuon(MaPhieu, MaSach, SoLuong) VALUES
(1,1,1),(1,5,1),(2,2,1),(2,4,1),(3,1,1),(3,3,1),(4,5,1);
GO
