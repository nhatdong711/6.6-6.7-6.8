using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BilliardsManagement.BUS;
using BilliardsManagement.DTO;
using static BilliardsManagement.GUI.UiHelper;

namespace BilliardsManagement.GUI
{
    public class FrmMain : Form
    {
        private readonly BanBidaBUS banBus = new BanBidaBUS();
        private readonly MenuDichVuBUS menuBus = new MenuDichVuBUS();
        private readonly HoaDonBUS hdBus = new HoaDonBUS();
        private readonly TabControl tabs = new TabControl { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10F) };
        private readonly Timer timer = new Timer { Interval = 1000 };

        private static readonly Color Green = Color.FromArgb(22, 163, 74);
        private static readonly Color Orange = Color.FromArgb(234, 88, 12);
        private static readonly Color Felt = Color.FromArgb(6, 78, 59);

        private FlowLayoutPanel flpBan;
        private Label lblBan, lblThoiGian, lblTien, lblThongKe;
        private Button btnMo, btnThanhToan, btnGoiMon, btnXoaMon, btnLuuOrder;
        private DataGridView gridMenu, gridOrder;
        private NumericUpDown numSL;
        private BanBidaDTO banChon;
        private HoaDonDTO hdHienTai;
        private DataTable menuTable, orderTable;

        private DataGridView gridDV;
        private DataTable dvTable;

        private DataGridView gridNgay, gridLoaiBan, gridLichSu;

        public FrmMain()
        {
            Text = "Hệ thống Quản lý Bàn Bida & Dịch vụ Giải trí (3-Tier)";
            Size = new Size(1320, 790);
            StartPosition = FormStartPosition.CenterScreen;
            Font = new Font("Segoe UI", 9.5F);
            BackColor = Color.White;
            var header = new Label
            {
                Text = "  🎱 BILLIARDS CLUB MANAGEMENT", Dock = DockStyle.Top, Height = 48, BackColor = Felt,
                ForeColor = Color.White, Font = new Font("Segoe UI", 15F, FontStyle.Bold), TextAlign = ContentAlignment.MiddleLeft
            };
            tabs.TabPages.Add(BuildSoDo());
            tabs.TabPages.Add(BuildMenu());
            tabs.TabPages.Add(BuildBaoCao());
            tabs.SelectedIndexChanged += (s, e) => Reload();
            Controls.Add(tabs);
            Controls.Add(header);
            Load += (s, e) => Reload();
            timer.Tick += (s, e) => CapNhatTamTinh();
            timer.Start();
        }

        private void Reload()
        {
            try
            {
                switch (tabs.SelectedIndex)
                {
                    case 0: LoadSoDo(); if (banChon != null) ChonBan(banBus.GetAll().First(b => b.MaBan == banChon.MaBan)); break;
                    case 1: LoadMenu(); break;
                    case 2: LoadBaoCao(); break;
                }
            }
            catch (Exception ex) { Error(ex); }
        }

        private TabPage BuildSoDo()
        {
            var tp = new TabPage("Sơ đồ bàn bida") { BackColor = Color.White };
            flpBan = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoScroll = true, Padding = new Padding(15), BackColor = Color.FromArgb(240, 253, 244) };
            lblThongKe = new Label { Dock = DockStyle.Top, Height = 34, Font = new Font("Segoe UI", 10.5F, FontStyle.Bold), TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(15, 0, 0, 0) };

            var right = new Panel { Dock = DockStyle.Right, Width = 540, BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            lblBan = new Label { Left = 10, Top = 10, Width = 510, Height = 30, Font = new Font("Segoe UI", 14F, FontStyle.Bold), ForeColor = Felt, Text = "Chọn một bàn bên trái" };
            lblThoiGian = new Label { Left = 10, Top = 42, Width = 510, Height = 22 };
            lblTien = new Label { Left = 10, Top = 64, Width = 510, Height = 24, Font = new Font("Segoe UI", 11F, FontStyle.Bold), ForeColor = Color.FromArgb(185, 28, 28) };
            btnMo = Btn("▶ MỞ BÀN", 10, 92, Green, 140);
            btnThanhToan = Btn("💵 THANH TOÁN", 160, 92, Orange, 160);

            var lblMenu = new Label { Left = 10, Top = 135, Width = 300, Text = "Menu dịch vụ:", Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
            gridMenu = Grid(); gridMenu.SetBounds(10, 158, 330, 200);
            var lblSL = new Label { Left = 350, Top = 161, Width = 80, Text = "Số lượng:" };
            numSL = new NumericUpDown { Left = 430, Top = 158, Width = 90, Minimum = 1, Maximum = 100, Value = 1 };
            btnGoiMon = Btn("+ Gọi thêm", 350, 195, Felt, 170);
            btnXoaMon = Btn("− Xóa dòng", 350, 235, Color.Gray, 170);
            btnLuuOrder = Btn("💾 Lưu order", 350, 275, Green, 170);

            var lblOrder = new Label { Left = 10, Top = 366, Width = 510, Text = "Dịch vụ bàn đã gọi (sửa trực tiếp cột Số lượng rồi bấm Lưu order):", Font = new Font("Segoe UI", 9.5F, FontStyle.Bold) };
            gridOrder = Grid(); gridOrder.SetBounds(10, 390, 510, 250);
            gridOrder.ReadOnly = false;
            gridOrder.DataError += (s, e) => { Info("Số lượng không hợp lệ."); e.Cancel = true; };
            gridOrder.CellEndEdit += (s, e) => CapNhatTamTinh();
            right.Controls.AddRange(new Control[] { lblBan, lblThoiGian, lblTien, btnMo, btnThanhToan, lblMenu, gridMenu, lblSL, numSL, btnGoiMon, btnXoaMon, btnLuuOrder, lblOrder, gridOrder });

            btnMo.Click += (s, e) => Try(() =>
            {
                if (banChon == null) throw new Exception("Chọn bàn.");
                int maHD = banBus.MoBan(banChon.MaBan);
                Info($"Đã mở {banChon.TenBan}. Tạo hóa đơn #{maHD}, giờ bắt đầu {DateTime.Now:HH:mm}.");
                int ma = banChon.MaBan;
                LoadSoDo(); ChonBan(banBus.GetAll().First(b => b.MaBan == ma));
            });
            btnGoiMon.Click += (s, e) => Try(() =>
            {
                if (hdHienTai == null) throw new Exception("Bàn chưa mở.");
                if (!(gridMenu.CurrentRow?.DataBoundItem is DataRowView dv)) throw new Exception("Chọn món trong menu.");
                hdBus.GoiMon(orderTable, hdHienTai.MaHD, dv.Row, (int)numSL.Value);
                FillTenDV();
                CapNhatTamTinh();
            });
            btnXoaMon.Click += (s, e) =>
            {
                if (gridOrder.CurrentRow?.DataBoundItem is DataRowView r) { r.Row.Delete(); CapNhatTamTinh(); }
            };
            btnLuuOrder.Click += (s, e) => Try(() =>
            {
                gridOrder.EndEdit();
                int n = hdBus.LuuOrder(orderTable);
                Info($"Đã đồng bộ {n} thay đổi order về CSDL (adapter.Update).");
                LoadOrder();
            });
            btnThanhToan.Click += (s, e) =>
            {
                if (hdHienTai == null) { Info("Bàn chưa mở."); return; }
                try
                {
                    gridOrder.EndEdit();
                    if (orderTable.GetChanges() != null) hdBus.LuuOrder(orderTable);
                    var preview = hdBus.TinhTien(hdHienTai, DateTime.Now);
                    if (!Confirm(hdBus.InHoaDon(preview) + "\nXác nhận thanh toán?")) { LoadOrder(); return; }
                    string bill = hdBus.ThanhToan(hdHienTai);
                    MessageBox.Show(bill + "\n✔ Đã thanh toán (trans.Commit()).", "Hóa đơn", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    int ma = banChon.MaBan;
                    LoadSoDo(); ChonBan(banBus.GetAll().First(b => b.MaBan == ma));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("THANH TOÁN THẤT BẠI - trans.Rollback()\n\n" + ex.Message + "\n\nHóa đơn, kho và trạng thái bàn được giữ nguyên.",
                        "Lỗi giao dịch", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    LoadOrder();
                }
            };

            tp.Controls.Add(flpBan);
            tp.Controls.Add(lblThongKe);
            tp.Controls.Add(right);
            return tp;
        }

        private void LoadSoDo()
        {
            var list = banBus.GetAll();
            flpBan.SuspendLayout();
            flpBan.Controls.Clear();
            foreach (var b in list)
            {
                bool trong = b.TrangThai == "Trống";
                var btn = new Button
                {
                    Width = 165, Height = 120, Margin = new Padding(8), FlatStyle = FlatStyle.Flat,
                    BackColor = trong ? Green : Orange, ForeColor = Color.White, Cursor = Cursors.Hand,
                    Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                    Text = $"🎱 {b.TenBan}\n{b.LoaiBan}\n{b.GiaTheoGio:N0}đ/giờ\n[{b.TrangThai}]",
                    Tag = b
                };
                btn.FlatAppearance.BorderSize = banChon != null && banChon.MaBan == b.MaBan ? 4 : 0;
                btn.FlatAppearance.BorderColor = Color.Gold;
                btn.Click += (s, e) => Try(() => ChonBan((BanBidaDTO)((Button)s).Tag));
                flpBan.Controls.Add(btn);
            }
            flpBan.ResumeLayout();
            int dangChoi = list.Count(b => b.TrangThai != "Trống");
            lblThongKe.Text = $"Tổng {list.Count} bàn   |   🟩 Trống: {list.Count - dangChoi}   |   🟧 Đang chơi: {dangChoi}";

            menuTable = menuBus.LoadMenu();
            gridMenu.DataSource = menuTable;
            SetHeaders(gridMenu, ("MaDV", "Mã"), ("TenDV", "Dịch vụ"), ("DonGia", "Giá"), ("TonKho", "Tồn"));
            if (gridMenu.Columns.Contains("MaDV")) gridMenu.Columns["MaDV"].Visible = false;
            if (gridMenu.Columns.Contains("DonGia")) gridMenu.Columns["DonGia"].DefaultCellStyle.Format = "N0";
            if (gridMenu.Columns.Contains("TonKho")) gridMenu.Columns["TonKho"].DefaultCellStyle.NullValue = "∞";
            SetButtons();
        }

        private void ChonBan(BanBidaDTO b)
        {
            banChon = b;
            hdHienTai = b.TrangThai == "Trống" ? null : hdBus.GetHoaDonDangMo(b.MaBan);
            lblBan.Text = $"{b.TenBan} ({b.LoaiBan}) - {b.TrangThai}";
            foreach (Button btn in flpBan.Controls)
                btn.FlatAppearance.BorderSize = ((BanBidaDTO)btn.Tag).MaBan == b.MaBan ? 4 : 0;
            SetButtons();
            LoadOrder();
        }

        private void SetButtons()
        {
            bool mo = hdHienTai != null;
            btnMo.Enabled = banChon != null && !mo;
            btnThanhToan.Enabled = btnGoiMon.Enabled = btnXoaMon.Enabled = btnLuuOrder.Enabled = mo;
        }

        private void LoadOrder()
        {
            if (hdHienTai == null)
            {
                orderTable = null; gridOrder.DataSource = null;
                lblThoiGian.Text = banChon == null ? "" : "Bàn đang trống — bấm MỞ BÀN để nhận khách."; lblTien.Text = "";
                return;
            }
            orderTable = hdBus.LoadOrder(hdHienTai.MaHD);
            orderTable.Columns.Add("TenDV", typeof(string));
            orderTable.Columns.Add("ThanhTien", typeof(decimal), "SoLuong * DonGia");
            FillTenDV();
            orderTable.AcceptChanges();
            gridOrder.DataSource = orderTable;
            SetHeaders(gridOrder, ("TenDV", "Dịch vụ"), ("SoLuong", "Số lượng"), ("DonGia", "Đơn giá"), ("ThanhTien", "Thành tiền"));
            foreach (var c in new[] { "MaHD", "MaDV" }) if (gridOrder.Columns.Contains(c)) gridOrder.Columns[c].Visible = false;
            foreach (DataGridViewColumn c in gridOrder.Columns) c.ReadOnly = c.Name != "SoLuong";
            gridOrder.Columns["TenDV"].DisplayIndex = 0;
            foreach (var c in new[] { "DonGia", "ThanhTien" }) gridOrder.Columns[c].DefaultCellStyle.Format = "N0";
            gridOrder.Columns["SoLuong"].DefaultCellStyle.BackColor = Color.FromArgb(254, 249, 195);
            CapNhatTamTinh();
        }

        private void FillTenDV()
        {
            if (orderTable == null || menuTable == null) return;
            foreach (DataRow r in orderTable.Rows)
            {
                if (r.RowState == DataRowState.Deleted || r["TenDV"] != DBNull.Value) continue;
                DataRow m = menuTable.AsEnumerable().FirstOrDefault(x => x.Field<int>("MaDV") == r.Field<int>("MaDV"));
                bool unchanged = r.RowState == DataRowState.Unchanged;
                r["TenDV"] = m?["TenDV"].ToString() ?? "";
                if (unchanged) r.AcceptChanges();
            }
        }

        private void CapNhatTamTinh()
        {
            if (hdHienTai == null || tabs.SelectedIndex != 0) return;
            TimeSpan t = DateTime.Now - hdHienTai.GioBatDau;
            decimal dv = orderTable == null ? 0 : orderTable.AsEnumerable().Where(r => r.RowState != DataRowState.Deleted)
                .Sum(r => r["SoLuong"] == DBNull.Value ? 0 : r.Field<int>("SoLuong") * r.Field<decimal>("DonGia"));
            double phutTinh = Math.Ceiling(Math.Max(t.TotalMinutes, 1) / HoaDonBUS.BlockPhut) * HoaDonBUS.BlockPhut;
            decimal gio = Math.Round((decimal)phutTinh / 60m * hdHienTai.GiaTheoGio, 0);
            lblThoiGian.Text = $"HĐ #{hdHienTai.MaHD} | Bắt đầu: {hdHienTai.GioBatDau:HH:mm}  |  Đã chơi: {(int)t.TotalHours:00}:{t.Minutes:00}:{t.Seconds:00}";
            lblTien.Text = $"Tạm tính: giờ chơi {gio:N0} + dịch vụ {dv:N0} = {gio + dv:N0} VNĐ";
        }

        private TabPage BuildMenu()
        {
            var tp = new TabPage("Quản lý menu dịch vụ") { BackColor = Color.White };
            var pnl = new Panel { Dock = DockStyle.Top, Height = 54 };
            var btnLuu = Btn("💾 Lưu thay đổi (adapter.Update)", 15, 10, Green, 280);
            var btnHuy = Btn("↺ Hủy thay đổi", 305, 10, Color.Gray, 150);
            pnl.Controls.Add(btnLuu); pnl.Controls.Add(btnHuy);
            pnl.Controls.Add(new Label { Left = 470, Top = 8, Width = 760, Height = 40, ForeColor = Color.DimGray, Text = "Sửa trực tiếp trên lưới, thêm dòng mới ở cuối, chọn dòng + Delete để xóa.\nĐể trống cột Tồn kho cho dịch vụ không trừ kho (thuê gậy, thuê găng)." });
            gridDV = Grid(); gridDV.Dock = DockStyle.Fill;
            gridDV.ReadOnly = false; gridDV.AllowUserToAddRows = true; gridDV.AllowUserToDeleteRows = true;
            gridDV.DataError += (s, e) => { Info("Giá trị không hợp lệ: " + e.Exception.Message); e.Cancel = true; };
            btnLuu.Click += (s, e) => Try(() => { gridDV.EndEdit(); int n = menuBus.Save(dvTable); Info($"Đã đồng bộ {n} dòng thay đổi về CSDL."); LoadMenu(); });
            btnHuy.Click += (s, e) => dvTable.RejectChanges();
            tp.Controls.Add(gridDV);
            tp.Controls.Add(pnl);
            return tp;
        }

        private void LoadMenu()
        {
            dvTable = menuBus.LoadMenu();
            gridDV.DataSource = dvTable;
            gridDV.Columns["MaDV"].ReadOnly = true;
            SetHeaders(gridDV, ("MaDV", "Mã DV"), ("TenDV", "Tên dịch vụ"), ("DonGia", "Đơn giá"), ("TonKho", "Tồn kho (trống = không trừ kho)"));
            gridDV.Columns["DonGia"].DefaultCellStyle.Format = "N0";
        }

        private TabPage BuildBaoCao()
        {
            var tp = new TabPage("Báo cáo doanh thu") { BackColor = Color.White };
            var outer = new SplitContainer { Dock = DockStyle.Fill, Orientation = Orientation.Horizontal, SplitterDistance = 250 };
            var top = new SplitContainer { Dock = DockStyle.Fill, Orientation = Orientation.Vertical, SplitterDistance = 640 };
            gridNgay = Grid(); gridNgay.Dock = DockStyle.Fill;
            gridLoaiBan = Grid(); gridLoaiBan.Dock = DockStyle.Fill;
            gridLichSu = Grid(); gridLichSu.Dock = DockStyle.Fill;
            top.Panel1.Controls.Add(gridNgay);
            top.Panel1.Controls.Add(new Label { Text = "💰 DOANH THU THEO NGÀY", Dock = DockStyle.Top, Height = 28, Font = new Font("Segoe UI", 10F, FontStyle.Bold), ForeColor = Felt });
            top.Panel2.Controls.Add(gridLoaiBan);
            top.Panel2.Controls.Add(new Label { Text = "🎱 DOANH THU THEO LOẠI BÀN", Dock = DockStyle.Top, Height = 28, Font = new Font("Segoe UI", 10F, FontStyle.Bold), ForeColor = Felt });
            outer.Panel1.Controls.Add(top);
            outer.Panel2.Controls.Add(gridLichSu);
            outer.Panel2.Controls.Add(new Label { Text = "🧾 LỊCH SỬ HÓA ĐƠN", Dock = DockStyle.Top, Height = 28, Font = new Font("Segoe UI", 10F, FontStyle.Bold), ForeColor = Felt });
            tp.Controls.Add(outer);
            return tp;
        }

        private void LoadBaoCao()
        {
            gridNgay.DataSource = hdBus.DoanhThuTheoNgay();
            SetHeaders(gridNgay, ("Ngay", "Ngày"), ("SoHoaDon", "Số HĐ"), ("TienGioChoi", "Tiền giờ"), ("TienDichVu", "Tiền DV"), ("TongDoanhThu", "Tổng"));
            gridLoaiBan.DataSource = hdBus.DoanhThuTheoLoaiBan();
            SetHeaders(gridLoaiBan, ("LoaiBan", "Loại bàn"), ("SoLuot", "Số lượt"), ("TongPhut", "Tổng phút chơi"), ("DoanhThu", "Doanh thu"));
            gridLichSu.DataSource = hdBus.LichSuHoaDon();
            SetHeaders(gridLichSu, ("MaHD", "Mã HĐ"), ("TenBan", "Bàn"), ("GioBatDau", "Bắt đầu"), ("GioKetThuc", "Kết thúc"), ("TienGioChoi", "Tiền giờ"), ("TienDichVu", "Tiền DV"), ("TongCong", "Tổng cộng"), ("TrangThai", "Trạng thái"));
            foreach (var g in new[] { gridNgay, gridLoaiBan, gridLichSu })
                foreach (DataGridViewColumn c in g.Columns)
                    if (c.ValueType == typeof(decimal)) c.DefaultCellStyle.Format = "N0";
        }

        private static void SetHeaders(DataGridView g, params (string col, string header)[] map)
        {
            foreach (var m in map) if (g.Columns.Contains(m.col)) g.Columns[m.col].HeaderText = m.header;
        }

        private static void Try(Action a)
        {
            try { a(); } catch (Exception ex) { Error(ex); }
        }
    }
}
