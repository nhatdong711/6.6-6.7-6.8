using System.Collections.Generic;

namespace ClinicManagement.DTO
{
    public class DonThuocDTO
    {
        public int MaDon { get; set; }
        public int MaHoSo { get; set; }
        public string ChanDoan { get; set; }
        public decimal TienKham { get; set; }
        public decimal TongTien { get; set; }
        public string TrangThai { get; set; }
        public List<ChiTietDonThuocDTO> ChiTiet { get; set; } = new List<ChiTietDonThuocDTO>();
    }

    public class ChiTietDonThuocDTO
    {
        public int MaDon { get; set; }
        public int MaThuoc { get; set; }
        public string TenThuoc { get; set; }
        public string DonViTinh { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public string LieuDung { get; set; }
        public decimal ThanhTien => SoLuong * DonGia;
    }
}
