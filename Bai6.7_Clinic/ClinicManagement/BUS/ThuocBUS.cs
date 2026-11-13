using System;
using System.Data;
using ClinicManagement.DAL;
using ClinicManagement.DTO;

namespace ClinicManagement.BUS
{
    public class ThuocBUS
    {
        private readonly ThuocDAL dal = new ThuocDAL();
        public static readonly string[] DonViList = { "Viên", "Vỉ", "Chai", "Hộp", "Ống", "Gói" };

        public DataTable GetAll() => dal.GetAll();

        private static void Validate(ThuocDTO t)
        {
            if (string.IsNullOrWhiteSpace(t.TenThuoc)) throw new ArgumentException("Tên thuốc không được để trống.");
            if (t.DonGia < 0) throw new ArgumentException("Đơn giá không được âm.");
            if (t.SoLuongTon < 0) throw new ArgumentException("Số lượng tồn không được âm.");
        }

        public bool Insert(ThuocDTO t) { Validate(t); return dal.Insert(t); }
        public bool Update(ThuocDTO t) { Validate(t); return dal.Update(t); }
        public bool Delete(int ma) => dal.Delete(ma);

        public bool NhapKho(int maThuoc, int soLuong)
        {
            if (maThuoc <= 0) throw new ArgumentException("Chọn thuốc cần nhập kho.");
            if (soLuong <= 0) throw new ArgumentException("Số lượng nhập phải lớn hơn 0.");
            return dal.NhapKho(maThuoc, soLuong);
        }
    }
}
