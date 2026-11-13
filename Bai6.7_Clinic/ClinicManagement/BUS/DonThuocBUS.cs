using System;
using System.Data;
using System.Linq;
using System.Text;
using ClinicManagement.DAL;
using ClinicManagement.DTO;

namespace ClinicManagement.BUS
{
    public class DonThuocBUS
    {
        private readonly DonThuocDAL dal = new DonThuocDAL();

        public DataTable GetAll() => dal.GetAll();
        public DataTable GetChiTiet(int maDon) => dal.GetChiTiet(maDon);

        public static decimal TinhTongTien(DonThuocDTO d) => d.TienKham + d.ChiTiet.Sum(c => c.SoLuong * c.DonGia);

        public int LuuDon(DonThuocDTO d)
        {
            if (d.MaHoSo <= 0) throw new ArgumentException("Vui lòng chọn hồ sơ khám.");
            if (string.IsNullOrWhiteSpace(d.ChanDoan)) throw new ArgumentException("Bác sĩ chưa nhập chẩn đoán.");
            if (d.TienKham < 0) throw new ArgumentException("Tiền khám không hợp lệ.");
            if (d.ChiTiet.Count == 0) throw new ArgumentException("Đơn thuốc phải có ít nhất 1 loại thuốc.");
            if (d.ChiTiet.Any(c => c.SoLuong <= 0)) throw new ArgumentException("Số lượng thuốc phải lớn hơn 0.");
            if (d.ChiTiet.Any(c => string.IsNullOrWhiteSpace(c.LieuDung))) throw new ArgumentException("Vui lòng nhập liều dùng cho từng thuốc.");
            d.ChanDoan = d.ChanDoan.Trim();
            d.TongTien = TinhTongTien(d);
            return dal.LuuDon(d);
        }

        public string ThanhToan(int maDon, string trangThaiHienTai)
        {
            if (trangThaiHienTai == "Đã thanh toán") throw new InvalidOperationException("Đơn thuốc đã được thanh toán.");
            dal.UpdateTrangThai(maDon, "Đã thanh toán");
            return InHoaDon(maDon);
        }

        public string InHoaDon(int maDon)
        {
            DataRow d = dal.GetAll().AsEnumerable().First(r => r.Field<int>("MaDon") == maDon);
            DataTable ct = dal.GetChiTiet(maDon);
            decimal tienThuoc = ct.AsEnumerable().Sum(r => r.Field<decimal>("ThanhTien"));
            var sb = new StringBuilder();
            sb.AppendLine("=========== HÓA ĐƠN KHÁM BỆNH ===========");
            sb.AppendLine($"Số đơn: {maDon}        Ngày in: {DateTime.Now:dd/MM/yyyy HH:mm}");
            sb.AppendLine($"Bệnh nhân: {d["HoTen"]}   SĐT: {d["SoDienThoai"]}");
            sb.AppendLine($"Chẩn đoán: {d["ChanDoan"]}");
            sb.AppendLine("-----------------------------------------");
            foreach (DataRow r in ct.Rows)
            {
                sb.AppendLine($"- {r["TenThuoc"]} x{r["SoLuong"]} {r["DonViTinh"]} @ {r["DonGia"]:N0} = {r["ThanhTien"]:N0}");
                sb.AppendLine($"    Liều dùng: {r["LieuDung"]}");
            }
            sb.AppendLine("-----------------------------------------");
            sb.AppendLine($"Tiền khám: {d["TienKham"]:N0}");
            sb.AppendLine($"Tiền thuốc: {tienThuoc:N0}");
            sb.AppendLine($"TỔNG CỘNG: {d["TongTien"]:N0} VNĐ");
            sb.AppendLine($"Trạng thái: {d["TrangThai"]}");
            return sb.ToString();
        }

        public DataTable DoanhThuTheoNgay() => dal.DoanhThuTheoNgay();
        public DataTable ThuocSapHet(int nguong = 10) => dal.ThuocSapHet(nguong);
    }
}
