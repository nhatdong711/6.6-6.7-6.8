using System;
using System.Data;
using Microsoft.Data.SqlClient;
using LibraryManagement.DTO;

namespace LibraryManagement.DAL
{
    public class DocGiaDAL
    {
        public DataTable GetAll() =>
            DBConnect.ExecuteQuery("SELECT MaDocGia, HoTen, Email, NgayHetHanThe FROM DocGia ORDER BY MaDocGia");

        public bool EmailExists(string email, int exceptId) =>
            Convert.ToInt32(DBConnect.ExecuteScalar("SELECT COUNT(*) FROM DocGia WHERE Email=@e AND MaDocGia<>@id",
                new SqlParameter("@e", email), new SqlParameter("@id", exceptId))) > 0;

        public bool TheConHan(int maDocGia) =>
            Convert.ToInt32(DBConnect.ExecuteScalar("SELECT COUNT(*) FROM DocGia WHERE MaDocGia=@ma AND NgayHetHanThe >= GETDATE()",
                new SqlParameter("@ma", maDocGia))) > 0;

        public bool Insert(DocGiaDTO d) => DBConnect.ExecuteNonQuery(
            "INSERT INTO DocGia(HoTen, Email, NgayHetHanThe) VALUES(@ten, @email, @han)",
            new SqlParameter("@ten", d.HoTen),
            new SqlParameter("@email", (object)d.Email ?? DBNull.Value),
            new SqlParameter("@han", (object)d.NgayHetHanThe ?? DBNull.Value)) > 0;

        public bool Update(DocGiaDTO d) => DBConnect.ExecuteNonQuery(
            "UPDATE DocGia SET HoTen=@ten, Email=@email, NgayHetHanThe=@han WHERE MaDocGia=@ma",
            new SqlParameter("@ten", d.HoTen),
            new SqlParameter("@email", (object)d.Email ?? DBNull.Value),
            new SqlParameter("@han", (object)d.NgayHetHanThe ?? DBNull.Value),
            new SqlParameter("@ma", d.MaDocGia)) > 0;

        public bool Delete(int ma)
        {
            int n = Convert.ToInt32(DBConnect.ExecuteScalar("SELECT COUNT(*) FROM PhieuMuon WHERE MaDocGia=@ma", new SqlParameter("@ma", ma)));
            if (n > 0) throw new InvalidOperationException("Độc giả đã có phiếu mượn, không thể xóa.");
            return DBConnect.ExecuteNonQuery("DELETE FROM DocGia WHERE MaDocGia=@ma", new SqlParameter("@ma", ma)) > 0;
        }
    }
}
