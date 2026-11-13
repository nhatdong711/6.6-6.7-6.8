using System;
using System.Windows.Forms;
using BilliardsManagement.DAL;
using BilliardsManagement.GUI;

namespace BilliardsManagement
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                DBConnect.EnsureDatabase();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi kết nối CSDL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Application.Run(new FrmMain());
        }
    }
}
