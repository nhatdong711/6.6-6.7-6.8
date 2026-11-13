namespace BilliardsManagement.DTO
{
    public class MenuDichVuDTO
    {
        public int MaDV { get; set; }
        public string TenDV { get; set; }
        public decimal DonGia { get; set; }
        public int? TonKho { get; set; }
        public bool QuanLyKho => TonKho.HasValue;
    }
}
