using System;
using System.Data;
using Microsoft.Data.SqlClient;
using BilliardsManagement.DTO;

namespace BilliardsManagement.DAL
{
    public class HoaDonDAL
    {
        private SqlDataAdapter orderAdapter;

        public HoaDonDTO GetHoaDonDangMo(int maBan)
        {
            const string sql = @"SELECT TOP 1 hd.MaHD, hd.MaBan, b.TenBan, b.GiaTheoGio, hd.GioBatDau
                                 FROM HoaDon hd JOIN BanBida b ON hd.MaBan = b.MaBan
                                 WHERE hd.MaBan = @b AND hd.TrangThai = 0 ORDER BY hd.MaHD DESC";
            using (var conn = DBConnect.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@b", maBan);
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    if (!rd.Read()) return null;
                    return new HoaDonDTO
                    {
                        MaHD = rd.GetInt32(0), MaBan = rd.GetInt32(1), TenBan = rd.GetString(2),
                        GiaTheoGio = rd.GetDecimal(3), GioBatDau = rd.GetDateTime(4)
                    };
                }
            }
        }

        public DataTable LoadOrder(int maHD)
        {
            var select = new SqlCommand("SELECT MaHD, MaDV, SoLuong, DonGia FROM ChiTietHoaDon WHERE MaHD = @hd", DBConnect.GetConnection());
            select.Parameters.AddWithValue("@hd", maHD);
            orderAdapter = new SqlDataAdapter(select);
            var builder = new SqlCommandBuilder(orderAdapter);
            orderAdapter.InsertCommand = builder.GetInsertCommand();
            orderAdapter.UpdateCommand = builder.GetUpdateCommand();
            orderAdapter.DeleteCommand = builder.GetDeleteCommand();
            var dt = new DataTable("ChiTietHoaDon");
            orderAdapter.Fill(dt);
            dt.PrimaryKey = new[] { dt.Columns["MaHD"], dt.Columns["MaDV"] };
            return dt;
        }

        public int SaveOrder(DataTable dt) => orderAdapter.Update(dt);

        public decimal TinhTienDichVu(int maHD)
        {
            object r = DBConnect.ExecuteScalar("SELECT ISNULL(SUM(SoLuong * DonGia), 0) FROM ChiTietHoaDon WHERE MaHD = @hd", new SqlParameter("@hd", maHD));
            return Convert.ToDecimal(r);
        }

        public DataTable GetChiTietHienThi(int maHD) => DBConnect.ExecuteQuery(
            @"SELECT dv.TenDV, ct.SoLuong, ct.DonGia, (ct.SoLuong * ct.DonGia) AS ThanhTien
              FROM ChiTietHoaDon ct JOIN MenuDichVu dv ON ct.MaDV = dv.MaDV WHERE ct.MaHD = @hd",
            new SqlParameter("@hd", maHD));

        public void ThanhToan(HoaDonDTO hd)
        {
            using (var conn = DBConnect.GetConnection())
            {
                conn.Open();
                SqlTransaction trans = conn.BeginTransaction();
                try
                {
                    var cmd1 = new SqlCommand(
                        @"UPDATE HoaDon SET GioKetThuc = @kt, TienGioChoi = @tg, TienDichVu = @tdv, TrangThai = 1
                          WHERE MaHD = @hd AND TrangThai = 0", conn, trans);
                    cmd1.Parameters.AddWithValue("@kt", hd.GioKetThuc.Value);
                    cmd1.Parameters.AddWithValue("@tg", hd.TienGioChoi);
                    cmd1.Parameters.AddWithValue("@tdv", hd.TienDichVu);
                    cmd1.Parameters.AddWithValue("@hd", hd.MaHD);
                    if (cmd1.ExecuteNonQuery() == 0) throw new InvalidOperationException("Hóa đơn không tồn tại hoặc đã thanh toán.");

                    var check = new SqlCommand(
                        @"SELECT TOP 1 dv.TenDV FROM ChiTietHoaDon ct JOIN MenuDichVu dv ON ct.MaDV = dv.MaDV
                          WHERE ct.MaHD = @hd AND dv.TonKho IS NOT NULL AND dv.TonKho < ct.SoLuong", conn, trans);
                    check.Parameters.AddWithValue("@hd", hd.MaHD);
                    object thieu = check.ExecuteScalar();
                    if (thieu != null) throw new InvalidOperationException($"Dịch vụ \"{thieu}\" không đủ tồn kho!");

                    var cmd2 = new SqlCommand(
                        @"UPDATE dv SET dv.TonKho = dv.TonKho - ct.SoLuong
                          FROM MenuDichVu dv JOIN ChiTietHoaDon ct ON dv.MaDV = ct.MaDV
                          WHERE ct.MaHD = @hd AND dv.TonKho IS NOT NULL", conn, trans);
                    cmd2.Parameters.AddWithValue("@hd", hd.MaHD);
                    cmd2.ExecuteNonQuery();

                    var cmd3 = new SqlCommand("UPDATE BanBida SET TrangThai = @tr WHERE MaBan = @b", conn, trans);
                    cmd3.Parameters.AddWithValue("@tr", "Trống");
                    cmd3.Parameters.AddWithValue("@b", hd.MaBan);
                    cmd3.ExecuteNonQuery();

                    trans.Commit();
                }
                catch
                {
                    trans.Rollback();
                    throw;
                }
            }
        }

        public DataTable DoanhThuTheoNgay() => DBConnect.ExecuteQuery(
            @"SELECT CONVERT(date, GioKetThuc) AS Ngay, COUNT(*) AS SoHoaDon, SUM(TienGioChoi) AS TienGioChoi,
                     SUM(TienDichVu) AS TienDichVu, SUM(TienGioChoi + TienDichVu) AS TongDoanhThu
              FROM HoaDon WHERE TrangThai = 1 GROUP BY CONVERT(date, GioKetThuc) ORDER BY Ngay DESC");

        public DataTable DoanhThuTheoLoaiBan() => DBConnect.ExecuteQuery(
            @"SELECT b.LoaiBan, COUNT(hd.MaHD) AS SoLuot, SUM(DATEDIFF(minute, hd.GioBatDau, hd.GioKetThuc)) AS TongPhut,
                     SUM(hd.TienGioChoi + hd.TienDichVu) AS DoanhThu
              FROM HoaDon hd JOIN BanBida b ON hd.MaBan = b.MaBan
              WHERE hd.TrangThai = 1 GROUP BY b.LoaiBan ORDER BY DoanhThu DESC");

        public DataTable LichSuHoaDon() => DBConnect.ExecuteQuery(
            @"SELECT hd.MaHD, b.TenBan, hd.GioBatDau, hd.GioKetThuc, hd.TienGioChoi, hd.TienDichVu,
                     (hd.TienGioChoi + hd.TienDichVu) AS TongCong,
                     CASE hd.TrangThai WHEN 1 THEN N'Đã thanh toán' ELSE N'Chưa thanh toán' END AS TrangThai
              FROM HoaDon hd JOIN BanBida b ON hd.MaBan = b.MaBan ORDER BY hd.MaHD DESC");
    }
}
