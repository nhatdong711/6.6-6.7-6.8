using System;
using System.Data;
using BilliardsManagement.DAL;

namespace BilliardsManagement.BUS
{
    public class MenuDichVuBUS
    {
        private readonly MenuDichVuDAL dal = new MenuDichVuDAL();
        public DataTable LoadMenu() => dal.LoadMenu();

        public int Save(DataTable dt)
        {
            foreach (DataRow r in dt.Rows)
            {
                if (r.RowState == DataRowState.Deleted) continue;
                if (string.IsNullOrWhiteSpace(r["TenDV"]?.ToString())) throw new ArgumentException("Tên dịch vụ không được trống.");
                if (r["DonGia"] == DBNull.Value || Convert.ToDecimal(r["DonGia"]) < 0) throw new ArgumentException("Đơn giá không hợp lệ.");
                if (r["TonKho"] != DBNull.Value && Convert.ToInt32(r["TonKho"]) < 0) throw new ArgumentException("Tồn kho không được âm.");
            }
            return dal.SaveMenu(dt);
        }
    }
}
