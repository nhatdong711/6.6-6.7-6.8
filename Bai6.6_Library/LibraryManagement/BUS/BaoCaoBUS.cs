using System.Data;
using LibraryManagement.DAL;

namespace LibraryManagement.BUS
{
    public class BaoCaoBUS
    {
        private readonly BaoCaoDAL dal = new BaoCaoDAL();
        public DataTable GetPhieuQuaHan() => dal.GetPhieuQuaHan();
        public DataTable ThongKeTheoTheLoai() => dal.ThongKeTheoTheLoai();
        public int TongLuotMuon() => dal.TongLuotMuon();
    }
}
