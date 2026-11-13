using System;

namespace BilliardsManagement.DTO
{
    public class HoaDonDTO
    {
        public int MaHD { get; set; }
        public int MaBan { get; set; }
        public string TenBan { get; set; }
        public decimal GiaTheoGio { get; set; }
        public DateTime GioBatDau { get; set; }
        public DateTime? GioKetThuc { get; set; }
        public decimal SoGioTinhTien { get; set; }
        public decimal TienGioChoi { get; set; }
        public decimal TienDichVu { get; set; }
        public bool TrangThai { get; set; }
        public decimal TongCong => TienGioChoi + TienDichVu;
    }
}
