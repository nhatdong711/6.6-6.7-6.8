using System;
using System.Data;
using LibraryManagement.DAL;
using LibraryManagement.DTO;

namespace LibraryManagement.BUS
{
    public class DauSachBUS
    {
        private readonly DauSachDAL dal = new DauSachDAL();
        public static readonly string[] TheLoaiList = { "Công nghệ thông tin", "Kinh tế", "Văn học", "Ngoại ngữ", "Khoa học" };

        public DataTable GetAll() => dal.GetAll();

        private static void Validate(DauSachDTO s)
        {
            if (string.IsNullOrWhiteSpace(s.TenSach)) throw new ArgumentException("Tên sách không được để trống.");
            if (s.TenSach.Length > 200) throw new ArgumentException("Tên sách tối đa 200 ký tự.");
            if (s.SoLuongCon < 0) throw new ArgumentException("Số lượng còn không được âm.");
        }

        public bool Insert(DauSachDTO s) { Validate(s); return dal.Insert(s); }
        public bool Update(DauSachDTO s) { Validate(s); return dal.Update(s); }
        public bool Delete(int ma) => dal.Delete(ma);
    }
}
