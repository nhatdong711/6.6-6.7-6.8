using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace LibraryManagement.DAL
{
    public class BaoCaoDAL
    {
        public DataTable GetPhieuQuaHan()
        {
            var dt = new DataTable();
            dt.Columns.Add("MaPhieu", typeof(int));
            dt.Columns.Add("HoTen", typeof(string));
            dt.Columns.Add("Email", typeof(string));
            dt.Columns.Add("NgayMuon", typeof(DateTime));
            dt.Columns.Add("HanTra", typeof(DateTime));
            dt.Columns.Add("SoNgayQuaHan", typeof(int));
            dt.Columns.Add("TrangThai", typeof(string));

            const string sql =
                @"SELECT pm.MaPhieu, dg.HoTen, ISNULL(dg.Email, ''), pm.NgayMuon, pm.HanTra,
                         DATEDIFF(day, pm.HanTra, GETDATE()), pm.TrangThai
                  FROM PhieuMuon pm JOIN DocGia dg ON pm.MaDocGia = dg.MaDocGia
                  WHERE pm.HanTra < GETDATE() AND pm.TrangThai <> @daTra
                  ORDER BY pm.HanTra";
            using (var conn = DBConnect.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@daTra", "Đã trả");
                conn.Open();
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        dt.Rows.Add(rd.GetInt32(0), rd.GetString(1), rd.GetString(2), rd.GetDateTime(3), rd.GetDateTime(4), rd.GetInt32(5), rd.GetString(6));
                }
            }
            return dt;
        }

        public DataTable ThongKeTheoTheLoai() => DBConnect.ExecuteQuery(
            @"SELECT ISNULL(ds.TheLoai, N'Khác') AS TheLoai,
                     COUNT(DISTINCT ct.MaPhieu) AS SoPhieu,
                     SUM(ct.SoLuong) AS TongLuotMuon
              FROM ChiTietMuon ct JOIN DauSach ds ON ct.MaSach = ds.MaSach
              GROUP BY ISNULL(ds.TheLoai, N'Khác')
              ORDER BY TongLuotMuon DESC");

        public int TongLuotMuon()
        {
            object r = DBConnect.ExecuteScalar("SELECT ISNULL(SUM(SoLuong), 0) FROM ChiTietMuon");
            return Convert.ToInt32(r);
        }
    }
}
