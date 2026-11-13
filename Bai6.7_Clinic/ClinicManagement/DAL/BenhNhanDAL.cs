using System;
using System.Data;
using Microsoft.Data.SqlClient;
using ClinicManagement.DTO;

namespace ClinicManagement.DAL
{
    public class BenhNhanDAL
    {
        public DataTable TraCuu(string soDienThoai) => DBConnect.ExecuteQuery(
            "SELECT MaBN, HoTen, SoDienThoai, NamSinh FROM BenhNhan WHERE SoDienThoai LIKE @sdt ORDER BY MaBN",
            new SqlParameter("@sdt", SqlDbType.VarChar, 17) { Value = "%" + soDienThoai + "%" });

        public int FindByPhone(string soDienThoai)
        {
            object r = DBConnect.ExecuteScalar("SELECT MaBN FROM BenhNhan WHERE SoDienThoai = @sdt", new SqlParameter("@sdt", soDienThoai));
            return r == null ? 0 : Convert.ToInt32(r);
        }

        public bool Update(BenhNhanDTO b) => DBConnect.ExecuteNonQuery(
            "UPDATE BenhNhan SET HoTen=@ten, SoDienThoai=@sdt, NamSinh=@ns WHERE MaBN=@ma",
            new SqlParameter("@ten", b.HoTen), new SqlParameter("@sdt", b.SoDienThoai),
            new SqlParameter("@ns", (object)b.NamSinh ?? DBNull.Value), new SqlParameter("@ma", b.MaBN)) > 0;

        public int TiepNhan(BenhNhanDTO bn, string trieuChung)
        {
            using (var conn = DBConnect.GetConnection())
            {
                conn.Open();
                SqlTransaction trans = conn.BeginTransaction();
                try
                {
                    int maBN = bn.MaBN;
                    if (maBN == 0)
                    {
                        var ins = new SqlCommand(
                            "INSERT INTO BenhNhan(HoTen, SoDienThoai, NamSinh) VALUES(@ten, @sdt, @ns); SELECT CAST(SCOPE_IDENTITY() AS int);", conn, trans);
                        ins.Parameters.AddWithValue("@ten", bn.HoTen);
                        ins.Parameters.AddWithValue("@sdt", bn.SoDienThoai);
                        ins.Parameters.AddWithValue("@ns", (object)bn.NamSinh ?? DBNull.Value);
                        maBN = Convert.ToInt32(ins.ExecuteScalar());
                    }
                    var hs = new SqlCommand(
                        "INSERT INTO HoSoKham(MaBN, NgayKham, TrieuChung) VALUES(@bn, GETDATE(), @tc); SELECT CAST(SCOPE_IDENTITY() AS int);", conn, trans);
                    hs.Parameters.AddWithValue("@bn", maBN);
                    hs.Parameters.AddWithValue("@tc", (object)trieuChung ?? DBNull.Value);
                    int maHoSo = Convert.ToInt32(hs.ExecuteScalar());
                    trans.Commit();
                    return maHoSo;
                }
                catch
                {
                    trans.Rollback();
                    throw;
                }
            }
        }

        public DataTable GetHoSoChoKeDon() => DBConnect.ExecuteQuery(
            @"SELECT hs.MaHoSo, bn.HoTen, bn.SoDienThoai, hs.NgayKham, hs.TrieuChung, hs.ChanDoan
              FROM HoSoKham hs JOIN BenhNhan bn ON hs.MaBN = bn.MaBN
              WHERE NOT EXISTS (SELECT 1 FROM DonThuoc dt WHERE dt.MaHoSo = hs.MaHoSo)
              ORDER BY hs.MaHoSo DESC");

        public DataTable GetLichSuKham(int maBN) => DBConnect.ExecuteQuery(
            @"SELECT hs.MaHoSo, hs.NgayKham, hs.TrieuChung, hs.ChanDoan
              FROM HoSoKham hs WHERE hs.MaBN = @bn ORDER BY hs.NgayKham DESC",
            new SqlParameter("@bn", maBN));
    }
}
