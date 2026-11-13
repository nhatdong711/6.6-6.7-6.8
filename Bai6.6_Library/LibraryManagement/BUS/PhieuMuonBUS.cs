using System;
using System.Data;
using System.Linq;
using LibraryManagement.DAL;
using LibraryManagement.DTO;

namespace LibraryManagement.BUS
{
    public class PhieuMuonBUS
    {
        private readonly PhieuMuonDAL dal = new PhieuMuonDAL();
        private readonly DocGiaDAL docGiaDal = new DocGiaDAL();
        public static readonly string[] TrangThaiList = { "Đang mượn", "Đã trả", "Quá hạn" };

        public DataTable Filter(int maDocGia, string trangThai)
        {
            dal.CapNhatQuaHan();
            return dal.Filter(maDocGia, trangThai);
        }

        public int LapPhieu(PhieuMuonDTO p)
        {
            if (p.MaDocGia <= 0) throw new ArgumentException("Vui lòng chọn độc giả.");
            if (!docGiaDal.TheConHan(p.MaDocGia)) throw new InvalidOperationException("Thẻ độc giả đã hết hạn, không thể lập phiếu mượn mới.");
            if (p.ChiTiet.Count == 0) throw new ArgumentException("Phiếu mượn phải có ít nhất 1 cuốn sách.");
            if (p.ChiTiet.Any(c => c.SoLuong <= 0)) throw new ArgumentException("Số lượng mượn phải lớn hơn 0.");
            if (p.HanTra.Date <= DateTime.Today) throw new ArgumentException("Hạn trả phải sau ngày hôm nay.");
            return dal.LapPhieu(p);
        }

        public void TraSach(int maPhieu, string trangThaiHienTai)
        {
            if (trangThaiHienTai == "Đã trả") throw new InvalidOperationException("Phiếu này đã được trả.");
            dal.TraSach(maPhieu);
        }
    }
}
