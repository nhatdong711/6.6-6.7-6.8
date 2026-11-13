using System;
using System.Data;
using System.Text.RegularExpressions;
using LibraryManagement.DAL;
using LibraryManagement.DTO;

namespace LibraryManagement.BUS
{
    public class DocGiaBUS
    {
        private readonly DocGiaDAL dal = new DocGiaDAL();

        public DataTable GetAll() => dal.GetAll();
        public bool TheConHan(int maDocGia) => dal.TheConHan(maDocGia);

        private void Validate(DocGiaDTO d)
        {
            if (string.IsNullOrWhiteSpace(d.HoTen)) throw new ArgumentException("Họ tên không được để trống.");
            if (string.IsNullOrWhiteSpace(d.Email)) { d.Email = null; return; }
            if (!Regex.IsMatch(d.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$")) throw new ArgumentException("Email không hợp lệ.");
            if (dal.EmailExists(d.Email, d.MaDocGia)) throw new ArgumentException("Email đã tồn tại.");
        }

        public bool Insert(DocGiaDTO d) { Validate(d); return dal.Insert(d); }
        public bool Update(DocGiaDTO d) { Validate(d); return dal.Update(d); }
        public bool Delete(int ma) => dal.Delete(ma);
    }
}
