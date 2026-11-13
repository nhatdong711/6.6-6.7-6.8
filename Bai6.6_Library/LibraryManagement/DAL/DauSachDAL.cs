using System;
using System.Data;
using Microsoft.Data.SqlClient;
using LibraryManagement.DTO;

namespace LibraryManagement.DAL
{
    public class DauSachDAL
    {
        public DataTable GetAll() =>
            DBConnect.ExecuteQuery("SELECT MaSach, TenSach, TheLoai, SoLuongCon FROM DauSach ORDER BY MaSach");

        public bool Insert(DauSachDTO s) => DBConnect.ExecuteNonQuery(
            "INSERT INTO DauSach(TenSach, TheLoai, SoLuongCon) VALUES(@ten, @tl, @sl)",
            new SqlParameter("@ten", s.TenSach),
            new SqlParameter("@tl", (object)s.TheLoai ?? DBNull.Value),
            new SqlParameter("@sl", s.SoLuongCon)) > 0;

        public bool Update(DauSachDTO s) => DBConnect.ExecuteNonQuery(
            "UPDATE DauSach SET TenSach=@ten, TheLoai=@tl, SoLuongCon=@sl WHERE MaSach=@ma",
            new SqlParameter("@ten", s.TenSach),
            new SqlParameter("@tl", (object)s.TheLoai ?? DBNull.Value),
            new SqlParameter("@sl", s.SoLuongCon),
            new SqlParameter("@ma", s.MaSach)) > 0;

        public bool Delete(int ma)
        {
            int n = Convert.ToInt32(DBConnect.ExecuteScalar("SELECT COUNT(*) FROM ChiTietMuon WHERE MaSach=@ma", new SqlParameter("@ma", ma)));
            if (n > 0) throw new InvalidOperationException("Đầu sách đã có lịch sử mượn, không thể xóa.");
            return DBConnect.ExecuteNonQuery("DELETE FROM DauSach WHERE MaSach=@ma", new SqlParameter("@ma", ma)) > 0;
        }
    }
}
