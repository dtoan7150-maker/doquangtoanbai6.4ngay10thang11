CREATE DATABASE GarageDB;
GO
USE GarageDB;
GO

CREATE TABLE KhachHang (
    MaKH INT PRIMARY KEY IDENTITY(1,1),
    TenKH NVARCHAR(100) NOT NULL,
    DienThoai VARCHAR(15) UNIQUE,
    DiaChi NVARCHAR(200)
);

CREATE TABLE Xe (
    BienSo VARCHAR(20) PRIMARY KEY,
    MaKH INT FOREIGN KEY REFERENCES KhachHang(MaKH),
    HieuXe NVARCHAR(50),
    NamSanXuat INT
);

CREATE TABLE PhuTung (
    MaPhuTung INT PRIMARY KEY IDENTITY(1,1),
    TenPhuTung NVARCHAR(150) NOT NULL,
    DonGia DECIMAL(18,2),
    SoLuongTon INT CHECK (SoLuongTon >= 0)
);

CREATE TABLE PhieuSuaChua (
    MaPhieu INT PRIMARY KEY IDENTITY(1,1),
    BienSo VARCHAR(20) FOREIGN KEY REFERENCES Xe(BienSo),
    NgayNhan DATETIME DEFAULT GETDATE(),
    NoiDungSua NVARCHAR(MAX),
    TongTien DECIMAL(18,2) DEFAULT 0,
    TrangThai NVARCHAR(30) DEFAULT N'Đang sửa'
);

CREATE TABLE ChiTietSuaChua (
    MaPhieu INT FOREIGN KEY REFERENCES PhieuSuaChua(MaPhieu),
    MaPhuTung INT FOREIGN KEY REFERENCES PhuTung(MaPhuTung),
    SoLuong INT CHECK (SoLuong > 0),
    TienCong DECIMAL(18,2) DEFAULT 0,
    PRIMARY KEY (MaPhieu, MaPhuTung)
);

INSERT INTO PhuTung (TenPhuTung, DonGia, SoLuongTon) VALUES 
(N'Lọc gió động cơ', 150000, 10),
(N'Bugi Curoa', 300000, 5),
(N'Dầu nhớt Castrol 4L', 450000, 20);
