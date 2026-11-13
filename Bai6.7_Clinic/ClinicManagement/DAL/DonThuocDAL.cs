using System;
using System.Data;
using Microsoft.Data.SqlClient;
using ClinicManagement.DTO;

namespace ClinicManagement.DAL
{
    public class DonThuocDAL
    {
        public DataTable GetAll() => DBConnect.ExecuteQuery(
            @"SELECT dt.MaDon, dt.MaHoSo, bn.HoTen, bn.SoDienThoai, hs.NgayKham, hs.ChanDoan, dt.TienKham, dt.TongTien, dt.TrangThai
              FROM DonThuoc dt JOIN HoSoKham hs ON dt.MaHoSo = hs.MaHoSo JOIN BenhNhan bn ON hs.MaBN = bn.MaBN
              ORDER BY dt.MaDon DESC");

        public DataTable GetChiTiet(int maDon) => DBConnect.ExecuteQuery(
            @"SELECT ct.MaThuoc, t.TenThuoc, t.DonViTinh, ct.SoLuong, t.DonGia, (ct.SoLuong * t.DonGia) AS ThanhTien, ct.LieuDung
              FROM ChiTietDonThuoc ct JOIN Thuoc t ON ct.MaThuoc = t.MaThuoc WHERE ct.MaDon = @ma",
            new SqlParameter("@ma", maDon));

        public int LuuDon(DonThuocDTO don)
        {
            using (var conn = DBConnect.GetConnection())
            {
                conn.Open();
                SqlTransaction trans = conn.BeginTransaction();
                try
                {
                    var cd = new SqlCommand("UPDATE HoSoKham SET ChanDoan = @cd WHERE MaHoSo = @hs", conn, trans);
                    cd.Parameters.AddWithValue("@cd", don.ChanDoan);
                    cd.Parameters.AddWithValue("@hs", don.MaHoSo);
                    cd.ExecuteNonQuery();

                    var cmdDon = new SqlCommand(
                        @"INSERT INTO DonThuoc(MaHoSo, TienKham, TongTien, TrangThai) VALUES(@hs, @tk, @tong, @tt);
                          SELECT CAST(SCOPE_IDENTITY() AS int);", conn, trans);
                    cmdDon.Parameters.AddWithValue("@hs", don.MaHoSo);
                    cmdDon.Parameters.AddWithValue("@tk", don.TienKham);
                    cmdDon.Parameters.AddWithValue("@tong", don.TongTien);
                    cmdDon.Parameters.AddWithValue("@tt", "Chờ lấy thuốc");
                    int maDon = Convert.ToInt32(cmdDon.ExecuteScalar());

                    foreach (var ct in don.ChiTiet)
                    {
                        var cmdCT = new SqlCommand(
                            "INSERT INTO ChiTietDonThuoc(MaDon, MaThuoc, SoLuong, LieuDung) VALUES(@md, @mt, @sl, @ld)", conn, trans);
                        cmdCT.Parameters.AddWithValue("@md", maDon);
                        cmdCT.Parameters.AddWithValue("@mt", ct.MaThuoc);
                        cmdCT.Parameters.AddWithValue("@sl", ct.SoLuong);
                        cmdCT.Parameters.AddWithValue("@ld", (object)ct.LieuDung ?? DBNull.Value);
                        cmdCT.ExecuteNonQuery();

                        var cmdKho = new SqlCommand(
                            "UPDATE Thuoc SET SoLuongTon = SoLuongTon - @sl WHERE MaThuoc = @mt AND SoLuongTon >= @sl", conn, trans);
                        cmdKho.Parameters.AddWithValue("@sl", ct.SoLuong);
                        cmdKho.Parameters.AddWithValue("@mt", ct.MaThuoc);
                        if (cmdKho.ExecuteNonQuery() == 0)
                            throw new InvalidOperationException($"Thuốc \"{ct.TenThuoc}\" không đủ số lượng tồn trong kho!");
                    }

                    trans.Commit();
                    return maDon;
                }
                catch
                {
                    trans.Rollback();
                    throw;
                }
            }
        }

        public bool UpdateTrangThai(int maDon, string trangThai) => DBConnect.ExecuteNonQuery(
            "UPDATE DonThuoc SET TrangThai=@tt WHERE MaDon=@ma",
            new SqlParameter("@tt", trangThai), new SqlParameter("@ma", maDon)) > 0;

        public DataTable DoanhThuTheoNgay() => DBConnect.ExecuteQuery(
            @"SELECT CONVERT(date, hs.NgayKham) AS Ngay, COUNT(*) AS SoDon, SUM(dt.TienKham) AS TienKham,
                     SUM(dt.TongTien - dt.TienKham) AS TienThuoc, SUM(dt.TongTien) AS DoanhThu
              FROM DonThuoc dt JOIN HoSoKham hs ON dt.MaHoSo = hs.MaHoSo
              WHERE dt.TrangThai = N'Đã thanh toán'
              GROUP BY CONVERT(date, hs.NgayKham) ORDER BY Ngay DESC");

        public DataTable ThuocSapHet(int nguong) => DBConnect.ExecuteQuery(
            "SELECT MaThuoc, TenThuoc, DonViTinh, SoLuongTon FROM Thuoc WHERE SoLuongTon <= @n ORDER BY SoLuongTon",
            new SqlParameter("@n", nguong));
    }
}
