using System.Data;
using Microsoft.Data.SqlClient;

namespace BilliardsManagement.DAL
{
    public class MenuDichVuDAL
    {
        private SqlDataAdapter adapter;

        public DataTable LoadMenu()
        {
            adapter = new SqlDataAdapter("SELECT MaDV, TenDV, DonGia, TonKho FROM MenuDichVu ORDER BY MaDV", DBConnect.GetConnection());
            var builder = new SqlCommandBuilder(adapter);
            adapter.InsertCommand = builder.GetInsertCommand();
            adapter.UpdateCommand = builder.GetUpdateCommand();
            adapter.DeleteCommand = builder.GetDeleteCommand();
            var dt = new DataTable("MenuDichVu");
            adapter.Fill(dt);
            dt.Columns["MaDV"].AutoIncrement = true;
            dt.Columns["MaDV"].AutoIncrementSeed = -1;
            dt.Columns["MaDV"].AutoIncrementStep = -1;
            return dt;
        }

        public int SaveMenu(DataTable dt) => adapter.Update(dt);
    }
}
