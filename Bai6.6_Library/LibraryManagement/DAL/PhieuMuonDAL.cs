using System;
using System.Data;
using Microsoft.Data.SqlClient;
using LibraryManagement.DTO;

namespace LibraryManagement.DAL
{
    public class PhieuMuonDAL
    {
        public int CapNhatQuaHan() => DBConnect.ExecuteNonQuery(
            "UPDATE PhieuMuon SET TrangThai = @qh WHERE TrangThai = @dm AND HanTra < GETDATE()",
            new SqlParameter("@qh", "Quá hạn"), new SqlParameter("@dm", "Đang mượn"));

        public DataTable Filter(int maDocGia, string trangThai)
        {
            const string sql =
                @"SELECT pm.MaPhieu, pm.MaDocGia, dg.HoTen, pm.NgayMuon, pm.HanTra, pm.TrangThai,
                         ISNULL(STUFF((SELECT ', ' + ds.TenSach + ' (x' + CAST(ct.SoLuong AS varchar(10)) + ')'
                                       FROM ChiTietMuon ct JOIN DauSach ds ON ct.MaSach = ds.MaSach
                                       WHERE ct.MaPhieu = pm.MaPhieu FOR XML PATH('')), 1, 2, ''), '') AS DanhSachSach
                  FROM PhieuMuon pm JOIN DocGia dg ON pm.MaDocGia = dg.MaDocGia
                  WHERE (@dg = 0 OR pm.MaDocGia = @dg) AND (@tt IS NULL OR pm.TrangThai = @tt)
                  ORDER BY pm.MaPhieu DESC";
            return DBConnect.ExecuteQuery(sql,
                new SqlParameter("@dg", maDocGia),
                new SqlParameter("@tt", SqlDbType.NVarChar, 50) { Value = (object)trangThai ?? DBNull.Value });
        }

        public int LapPhieu(PhieuMuonDTO p)
        {
            using (var conn = DBConnect.GetConnection())
            {
                conn.Open();
                SqlTransaction trans = conn.BeginTransaction();
                try
                {
                    var cmd = new SqlCommand(
                        @"INSERT INTO PhieuMuon(MaDocGia, NgayMuon, HanTra, TrangThai) VALUES(@dg, GETDATE(), @han, @tt);
                          SELECT CAST(SCOPE_IDENTITY() AS int);", conn, trans);
                    cmd.Parameters.AddWithValue("@dg", p.MaDocGia);
                    cmd.Parameters.AddWithValue("@han", p.HanTra);
                    cmd.Parameters.AddWithValue("@tt", "Đang mượn");
                    int maPhieu = Convert.ToInt32(cmd.ExecuteScalar());

                    foreach (var ct in p.ChiTiet)
                    {
                        var ins = new SqlCommand("INSERT INTO ChiTietMuon(MaPhieu, MaSach, SoLuong) VALUES(@mp, @ms, @sl)", conn, trans);
                        ins.Parameters.AddWithValue("@mp", maPhieu);
                        ins.Parameters.AddWithValue("@ms", ct.MaSach);
                        ins.Parameters.AddWithValue("@sl", ct.SoLuong);
                        ins.ExecuteNonQuery();

                        var upd = new SqlCommand("UPDATE DauSach SET SoLuongCon = SoLuongCon - @sl WHERE MaSach = @ms AND SoLuongCon >= @sl", conn, trans);
                        upd.Parameters.AddWithValue("@sl", ct.SoLuong);
                        upd.Parameters.AddWithValue("@ms", ct.MaSach);
                        if (upd.ExecuteNonQuery() == 0)
                            throw new InvalidOperationException($"Sách \"{ct.TenSach}\" không còn đủ số lượng để cho mượn.");
                    }
                    trans.Commit();
                    return maPhieu;
                }
                catch
                {
                    trans.Rollback();
                    throw;
                }
            }
        }

        public void TraSach(int maPhieu)
        {
            using (var conn = DBConnect.GetConnection())
            {
                conn.Open();
                SqlTransaction trans = conn.BeginTransaction();
                try
                {
                    var upd = new SqlCommand(
                        "UPDATE PhieuMuon SET TrangThai = @daTra WHERE MaPhieu = @ma AND TrangThai <> @daTra", conn, trans);
                    upd.Parameters.AddWithValue("@daTra", "Đã trả");
                    upd.Parameters.AddWithValue("@ma", maPhieu);
                    if (upd.ExecuteNonQuery() == 0) throw new InvalidOperationException("Phiếu không tồn tại hoặc đã được trả.");

                    var cong = new SqlCommand(
                        @"UPDATE ds SET ds.SoLuongCon = ds.SoLuongCon + ct.SoLuong
                          FROM DauSach ds JOIN ChiTietMuon ct ON ds.MaSach = ct.MaSach
                          WHERE ct.MaPhieu = @ma", conn, trans);
                    cong.Parameters.AddWithValue("@ma", maPhieu);
                    cong.ExecuteNonQuery();

                    trans.Commit();
                }
                catch
                {
                    trans.Rollback();
                    throw;
                }
            }
        }
    }
}
