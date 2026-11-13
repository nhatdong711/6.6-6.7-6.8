using System;
using System.Data;
using Microsoft.Data.SqlClient;
using ClinicManagement.DTO;

namespace ClinicManagement.DAL
{
    public class ThuocDAL
    {
        public DataTable GetAll() => DBConnect.ExecuteQuery("SELECT MaThuoc, TenThuoc, DonViTinh, DonGia, SoLuongTon FROM Thuoc ORDER BY MaThuoc");

        public bool Insert(ThuocDTO t) => DBConnect.ExecuteNonQuery(
            "INSERT INTO Thuoc(TenThuoc, DonViTinh, DonGia, SoLuongTon) VALUES(@ten, @dvt, @gia, @sl)",
            new SqlParameter("@ten", t.TenThuoc), new SqlParameter("@dvt", (object)t.DonViTinh ?? DBNull.Value),
            new SqlParameter("@gia", t.DonGia), new SqlParameter("@sl", t.SoLuongTon)) > 0;

        public bool Update(ThuocDTO t) => DBConnect.ExecuteNonQuery(
            "UPDATE Thuoc SET TenThuoc=@ten, DonViTinh=@dvt, DonGia=@gia, SoLuongTon=@sl WHERE MaThuoc=@ma",
            new SqlParameter("@ten", t.TenThuoc), new SqlParameter("@dvt", (object)t.DonViTinh ?? DBNull.Value),
            new SqlParameter("@gia", t.DonGia), new SqlParameter("@sl", t.SoLuongTon), new SqlParameter("@ma", t.MaThuoc)) > 0;

        public bool NhapKho(int maThuoc, int soLuong) => DBConnect.ExecuteNonQuery(
            "UPDATE Thuoc SET SoLuongTon = SoLuongTon + @sl WHERE MaThuoc=@ma",
            new SqlParameter("@sl", soLuong), new SqlParameter("@ma", maThuoc)) > 0;

        public bool Delete(int ma)
        {
            int n = Convert.ToInt32(DBConnect.ExecuteScalar("SELECT COUNT(*) FROM ChiTietDonThuoc WHERE MaThuoc=@ma", new SqlParameter("@ma", ma)));
            if (n > 0) throw new InvalidOperationException("Thuốc đã được kê trong đơn, không thể xóa.");
            return DBConnect.ExecuteNonQuery("DELETE FROM Thuoc WHERE MaThuoc=@ma", new SqlParameter("@ma", ma)) > 0;
        }
    }
}
