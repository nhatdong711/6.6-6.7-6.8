using System;

namespace ClinicManagement.DTO
{
    public class HoSoKhamDTO
    {
        public int MaHoSo { get; set; }
        public int MaBN { get; set; }
        public DateTime NgayKham { get; set; }
        public string TrieuChung { get; set; }
        public string ChanDoan { get; set; }
    }
}
