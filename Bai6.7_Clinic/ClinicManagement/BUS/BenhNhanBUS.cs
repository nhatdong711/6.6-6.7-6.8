using System;
using System.Data;
using System.Text.RegularExpressions;
using ClinicManagement.DAL;
using ClinicManagement.DTO;

namespace ClinicManagement.BUS
{
    public class BenhNhanBUS
    {
        private readonly BenhNhanDAL dal = new BenhNhanDAL();

        public DataTable TraCuu(string sdt) => dal.TraCuu((sdt ?? "").Trim());
        public DataTable GetHoSoChoKeDon() => dal.GetHoSoChoKeDon();
        public DataTable GetLichSuKham(int maBN) => dal.GetLichSuKham(maBN);

        private static void Validate(BenhNhanDTO b)
        {
            if (string.IsNullOrWhiteSpace(b.HoTen)) throw new ArgumentException("Họ tên bệnh nhân không được để trống.");
            if (!Regex.IsMatch(b.SoDienThoai ?? "", @"^0\d{9,10}$")) throw new ArgumentException("Số điện thoại không hợp lệ (10-11 số, bắt đầu bằng 0).");
            if (b.NamSinh.HasValue && (b.NamSinh < 1900 || b.NamSinh > DateTime.Now.Year)) throw new ArgumentException("Năm sinh không hợp lệ.");
        }

        public int TiepNhan(BenhNhanDTO b, string trieuChung)
        {
            b.SoDienThoai = b.SoDienThoai?.Trim();
            Validate(b);
            if (string.IsNullOrWhiteSpace(trieuChung)) throw new ArgumentException("Vui lòng nhập triệu chứng.");
            int maCu = dal.FindByPhone(b.SoDienThoai);
            if (b.MaBN == 0 && maCu > 0) b.MaBN = maCu;
            return dal.TiepNhan(b, trieuChung.Trim());
        }

        public bool Update(BenhNhanDTO b) { Validate(b); return dal.Update(b); }
    }
}
