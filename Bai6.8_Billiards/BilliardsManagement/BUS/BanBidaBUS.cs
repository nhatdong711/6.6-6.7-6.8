using System.Collections.Generic;
using BilliardsManagement.DAL;
using BilliardsManagement.DTO;

namespace BilliardsManagement.BUS
{
    public class BanBidaBUS
    {
        private readonly BanBidaDAL dal = new BanBidaDAL();
        public List<BanBidaDTO> GetAll() => dal.GetAll();
        public int MoBan(int maBan) => dal.MoBan(maBan);
    }
}
