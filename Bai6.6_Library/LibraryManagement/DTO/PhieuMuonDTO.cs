using System;
using System.Collections.Generic;

namespace LibraryManagement.DTO
{
    public class PhieuMuonDTO
    {
        public int MaPhieu { get; set; }
        public int MaDocGia { get; set; }
        public DateTime NgayMuon { get; set; }
        public DateTime HanTra { get; set; }
        public string TrangThai { get; set; }
        public List<ChiTietMuonDTO> ChiTiet { get; set; } = new List<ChiTietMuonDTO>();
    }

    public class ChiTietMuonDTO
    {
        public int MaPhieu { get; set; }
        public int MaSach { get; set; }
        public string TenSach { get; set; }
        public int SoLuong { get; set; }
    }
}
