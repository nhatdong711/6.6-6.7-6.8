using System;
using System.Data;
using System.Linq;
using System.Text;
using BilliardsManagement.DAL;
using BilliardsManagement.DTO;

namespace BilliardsManagement.BUS
{
    public class HoaDonBUS
    {
        private readonly HoaDonDAL dal = new HoaDonDAL();
        public const int BlockPhut = 15;

        public HoaDonDTO GetHoaDonDangMo(int maBan) => dal.GetHoaDonDangMo(maBan);
        public DataTable LoadOrder(int maHD) => dal.LoadOrder(maHD);
        public DataTable GetChiTietHienThi(int maHD) => dal.GetChiTietHienThi(maHD);

        public void GoiMon(DataTable order, int maHD, DataRow dv, int soLuong)
        {
            if (soLuong <= 0) throw new ArgumentException("Số lượng phải lớn hơn 0.");
            int maDV = Convert.ToInt32(dv["MaDV"]);
            DataRow exist = order.Rows.Find(new object[] { maHD, maDV });
            int daGoi = exist != null && exist.RowState != DataRowState.Deleted ? Convert.ToInt32(exist["SoLuong"]) : 0;
            if (dv["TonKho"] != DBNull.Value && daGoi + soLuong > Convert.ToInt32(dv["TonKho"]))
                throw new InvalidOperationException($"\"{dv["TenDV"]}\" chỉ còn {dv["TonKho"]} trong kho.");
            if (exist != null && exist.RowState != DataRowState.Deleted) exist["SoLuong"] = daGoi + soLuong;
            else order.Rows.Add(maHD, maDV, soLuong, Convert.ToDecimal(dv["DonGia"]));
        }

        public int LuuOrder(DataTable order)
        {
            foreach (DataRow r in order.Rows)
            {
                if (r.RowState == DataRowState.Deleted) continue;
                if (r["SoLuong"] == DBNull.Value || Convert.ToInt32(r["SoLuong"]) <= 0)
                    throw new ArgumentException("Số lượng mỗi món phải lớn hơn 0 (muốn bỏ món hãy xóa dòng).");
            }
            return dal.SaveOrder(order);
        }

        public HoaDonDTO TinhTien(HoaDonDTO hd, DateTime gioKetThuc)
        {
            hd.GioKetThuc = gioKetThuc;
            double phut = Math.Max((gioKetThuc - hd.GioBatDau).TotalMinutes, 1);
            double phutTinh = Math.Ceiling(phut / BlockPhut) * BlockPhut;
            hd.SoGioTinhTien = Math.Round((decimal)phutTinh / 60m, 2);
            hd.TienGioChoi = Math.Round((decimal)phutTinh / 60m * hd.GiaTheoGio, 0);
            hd.TienDichVu = dal.TinhTienDichVu(hd.MaHD);
            return hd;
        }

        public string ThanhToan(HoaDonDTO hd)
        {
            TinhTien(hd, DateTime.Now);
            dal.ThanhToan(hd);
            return InHoaDon(hd);
        }

        public string InHoaDon(HoaDonDTO hd)
        {
            TimeSpan t = hd.GioKetThuc.Value - hd.GioBatDau;
            var sb = new StringBuilder();
            sb.AppendLine("========= HÓA ĐƠN BIDA =========");
            sb.AppendLine($"Hóa đơn #{hd.MaHD} - {hd.TenBan}");
            sb.AppendLine($"Bắt đầu: {hd.GioBatDau:HH:mm dd/MM}   Kết thúc: {hd.GioKetThuc:HH:mm dd/MM}");
            sb.AppendLine($"Thời gian chơi: {(int)t.TotalHours} giờ {t.Minutes} phút (tính {hd.SoGioTinhTien:0.##} giờ, block {BlockPhut} phút)");
            sb.AppendLine($"Tiền giờ ({hd.GiaTheoGio:N0}/giờ): {hd.TienGioChoi:N0}");
            sb.AppendLine("--------------------------------");
            foreach (DataRow r in dal.GetChiTietHienThi(hd.MaHD).Rows)
                sb.AppendLine($"  {r["TenDV"]} x{r["SoLuong"]} = {r["ThanhTien"]:N0}");
            sb.AppendLine($"Tiền dịch vụ: {hd.TienDichVu:N0}");
            sb.AppendLine("--------------------------------");
            sb.AppendLine($"TỔNG THANH TOÁN: {hd.TongCong:N0} VNĐ");
            return sb.ToString();
        }

        public DataTable DoanhThuTheoNgay() => dal.DoanhThuTheoNgay();
        public DataTable DoanhThuTheoLoaiBan() => dal.DoanhThuTheoLoaiBan();
        public DataTable LichSuHoaDon() => dal.LichSuHoaDon();
    }
}
