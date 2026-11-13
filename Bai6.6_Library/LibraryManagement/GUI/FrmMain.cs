using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using LibraryManagement.BUS;
using LibraryManagement.DTO;
using static LibraryManagement.GUI.UiHelper;

namespace LibraryManagement.GUI
{
    public class FrmMain : Form
    {
        private readonly DauSachBUS sachBus = new DauSachBUS();
        private readonly DocGiaBUS dgBus = new DocGiaBUS();
        private readonly PhieuMuonBUS pmBus = new PhieuMuonBUS();
        private readonly BaoCaoBUS bcBus = new BaoCaoBUS();
        private readonly TabControl tabs = new TabControl { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10F) };

        private Label lblTongQuan;

        private DataGridView gridSach;
        private TextBox txtTenSach;
        private ComboBox cboTheLoai;
        private NumericUpDown numSLCon;
        private int selSach;

        private DataGridView gridDG;
        private TextBox txtHoTen, txtEmail;
        private DateTimePicker dtpHetHan;
        private int selDG;

        private ComboBox cboDGMuon, cboSachMuon;
        private NumericUpDown numSLMuon;
        private DateTimePicker dtpHanTra;
        private Label lblHanThe;
        private DataGridView gridGio;
        private readonly List<ChiTietMuonDTO> gio = new List<ChiTietMuonDTO>();

        private ComboBox cboLocDG, cboLocTT;
        private DataGridView gridPhieu;

        private DataGridView gridQuaHan, gridTheLoai;
        private Label lblTongLuot;

        public FrmMain()
        {
            Text = "Hệ thống Quản lý Mượn Trả Sách Thư viện (3-Tier)";
            Size = new Size(1280, 760);
            StartPosition = FormStartPosition.CenterScreen;
            Font = new Font("Segoe UI", 9.5F);
            BackColor = Color.White;
            var header = new Label
            {
                Text = "  📚 LIBRARY MANAGEMENT SYSTEM", Dock = DockStyle.Top, Height = 48, BackColor = Primary,
                ForeColor = Color.White, Font = new Font("Segoe UI", 15F, FontStyle.Bold), TextAlign = ContentAlignment.MiddleLeft
            };
            tabs.TabPages.Add(BuildTongQuan());
            tabs.TabPages.Add(BuildSach());
            tabs.TabPages.Add(BuildDocGia());
            tabs.TabPages.Add(BuildLapPhieu());
            tabs.TabPages.Add(BuildPhieu());
            tabs.TabPages.Add(BuildBaoCao());
            tabs.SelectedIndexChanged += (s, e) => Reload();
            Controls.Add(tabs);
            Controls.Add(header);
            Load += (s, e) => Reload();
        }

        private void Reload()
        {
            try
            {
                switch (tabs.SelectedIndex)
                {
                    case 0: LoadTongQuan(); break;
                    case 1: LoadSach(); break;
                    case 2: LoadDocGia(); break;
                    case 3: LoadLapPhieu(); break;
                    case 4: LoadComboLoc(); LoadPhieu(); break;
                    case 5: LoadBaoCao(); break;
                }
            }
            catch (Exception ex) { Error(ex); }
        }

        private TabPage BuildTongQuan()
        {
            var tp = new TabPage("Tổng quan") { BackColor = Color.White };
            lblTongQuan = new Label { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 13F), Padding = new Padding(30) };
            tp.Controls.Add(lblTongQuan);
            return tp;
        }

        private void LoadTongQuan()
        {
            DataTable sach = sachBus.GetAll();
            DataTable dg = dgBus.GetAll();
            DataTable pm = pmBus.Filter(0, null);
            Func<string, int> dem = st => pm.AsEnumerable().Count(r => r.Field<string>("TrangThai") == st);
            int tongCon = sach.AsEnumerable().Sum(r => r.Field<int>("SoLuongCon"));
            int theHetHan = dg.AsEnumerable().Count(r => r["NgayHetHanThe"] != DBNull.Value && r.Field<DateTime>("NgayHetHanThe") < DateTime.Now);
            lblTongQuan.Text =
                "TỔNG QUAN THƯ VIỆN\n\n" +
                $"📖  Số đầu sách: {sach.Rows.Count}   (tổng số cuốn còn trong kho: {tongCon})\n" +
                $"👤  Số độc giả: {dg.Rows.Count}   (thẻ hết hạn: {theHetHan})\n" +
                $"📋  Phiếu mượn: {pm.Rows.Count}  —  Đang mượn: {dem("Đang mượn")}, Đã trả: {dem("Đã trả")}, Quá hạn: {dem("Quá hạn")}\n" +
                $"📊  Tổng lượt mượn sách: {bcBus.TongLuotMuon()}\n\n" +
                "Quy trình: Quản lý đầu sách / độc giả → Lập phiếu mượn (kiểm tra hạn thẻ) → Trả sách → Theo dõi quá hạn & thống kê.";
        }

        private TabPage BuildSach()
        {
            var tp = new TabPage("Đầu sách") { BackColor = Color.White };
            var pnl = new Panel { Dock = DockStyle.Top, Height = 130 };
            txtTenSach = new TextBox { Left = 130, Top = 15, Width = 380 };
            cboTheLoai = new ComboBox { Left = 130, Top = 50, Width = 250 };
            cboTheLoai.Items.AddRange(DauSachBUS.TheLoaiList);
            cboTheLoai.SelectedIndex = 0;
            numSLCon = new NumericUpDown { Left = 130, Top = 85, Width = 120, Maximum = 100000 };
            var btnThem = Btn("Thêm", 640, 15, Color.FromArgb(22, 163, 74));
            var btnSua = Btn("Sửa", 750, 15, Color.FromArgb(234, 88, 12));
            var btnXoa = Btn("Xóa", 860, 15, Color.FromArgb(220, 38, 38));
            var btnMoi = Btn("Làm mới", 640, 55, Color.Gray);
            pnl.Controls.AddRange(new Control[] { Lbl("Tên sách:", 20, 15), txtTenSach, Lbl("Thể loại:", 20, 50), cboTheLoai, Lbl("Số lượng còn:", 20, 85), numSLCon, btnThem, btnSua, btnXoa, btnMoi });
            gridSach = Grid(); gridSach.Dock = DockStyle.Fill;
            gridSach.SelectionChanged += (s, e) =>
            {
                if (gridSach.CurrentRow?.DataBoundItem is DataRowView r)
                {
                    selSach = (int)r["MaSach"];
                    txtTenSach.Text = r["TenSach"].ToString();
                    cboTheLoai.Text = r["TheLoai"].ToString();
                    numSLCon.Value = (int)r["SoLuongCon"];
                }
            };
            gridSach.CellFormatting += (s, e) =>
            {
                if (gridSach.Columns[e.ColumnIndex].Name == "SoLuongCon" && e.Value is int sl && sl == 0)
                    e.CellStyle.ForeColor = Color.Red;
            };
            Func<DauSachDTO> read = () => new DauSachDTO { MaSach = selSach, TenSach = txtTenSach.Text.Trim(), TheLoai = cboTheLoai.Text.Trim(), SoLuongCon = (int)numSLCon.Value };
            btnThem.Click += (s, e) => Try(() => { sachBus.Insert(read()); LoadSach(); Info("Đã thêm đầu sách."); });
            btnSua.Click += (s, e) => Try(() => { if (selSach == 0) throw new Exception("Chọn đầu sách cần sửa."); sachBus.Update(read()); LoadSach(); Info("Đã cập nhật đầu sách."); });
            btnXoa.Click += (s, e) => Try(() => { if (selSach == 0) throw new Exception("Chọn đầu sách cần xóa."); if (Confirm("Xóa đầu sách này?")) { sachBus.Delete(selSach); selSach = 0; LoadSach(); } });
            btnMoi.Click += (s, e) => { selSach = 0; txtTenSach.Clear(); numSLCon.Value = 0; LoadSach(); };
            tp.Controls.Add(gridSach);
            tp.Controls.Add(pnl);
            return tp;
        }

        private void LoadSach()
        {
            gridSach.DataSource = sachBus.GetAll();
            SetHeaders(gridSach, ("MaSach", "Mã sách"), ("TenSach", "Tên sách"), ("TheLoai", "Thể loại"), ("SoLuongCon", "Số lượng còn"));
        }

        private TabPage BuildDocGia()
        {
            var tp = new TabPage("Độc giả") { BackColor = Color.White };
            var pnl = new Panel { Dock = DockStyle.Top, Height = 130 };
            txtHoTen = new TextBox { Left = 140, Top = 15, Width = 300 };
            txtEmail = new TextBox { Left = 140, Top = 50, Width = 300 };
            dtpHetHan = new DateTimePicker { Left = 140, Top = 85, Width = 160, Format = DateTimePickerFormat.Short, Value = DateTime.Today.AddYears(1) };
            var btnThem = Btn("Thêm", 640, 15, Color.FromArgb(22, 163, 74));
            var btnSua = Btn("Sửa", 750, 15, Color.FromArgb(234, 88, 12));
            var btnXoa = Btn("Xóa", 860, 15, Color.FromArgb(220, 38, 38));
            var btnGiaHan = Btn("Gia hạn thẻ 1 năm", 640, 55, Color.FromArgb(124, 58, 237), 210);
            pnl.Controls.AddRange(new Control[] { Lbl("Họ tên:", 20, 15), txtHoTen, Lbl("Email:", 20, 50), txtEmail, Lbl("Hết hạn thẻ:", 20, 85, 120), dtpHetHan, btnThem, btnSua, btnXoa, btnGiaHan });
            gridDG = Grid(); gridDG.Dock = DockStyle.Fill;
            gridDG.SelectionChanged += (s, e) =>
            {
                if (gridDG.CurrentRow?.DataBoundItem is DataRowView r)
                {
                    selDG = (int)r["MaDocGia"];
                    txtHoTen.Text = r["HoTen"].ToString();
                    txtEmail.Text = r["Email"].ToString();
                    if (r["NgayHetHanThe"] != DBNull.Value) dtpHetHan.Value = (DateTime)r["NgayHetHanThe"];
                }
            };
            gridDG.CellFormatting += (s, e) =>
            {
                if (gridDG.Columns[e.ColumnIndex].Name == "NgayHetHanThe" && e.Value is DateTime d && d < DateTime.Now)
                { e.CellStyle.ForeColor = Color.Red; e.CellStyle.Font = new Font(gridDG.Font, FontStyle.Bold); }
            };
            Func<DocGiaDTO> read = () => new DocGiaDTO { MaDocGia = selDG, HoTen = txtHoTen.Text.Trim(), Email = txtEmail.Text.Trim(), NgayHetHanThe = dtpHetHan.Value.Date.AddDays(1).AddSeconds(-1) };
            btnThem.Click += (s, e) => Try(() => { var d = read(); d.MaDocGia = 0; dgBus.Insert(d); LoadDocGia(); Info("Đã thêm độc giả."); });
            btnSua.Click += (s, e) => Try(() => { if (selDG == 0) throw new Exception("Chọn độc giả cần sửa."); dgBus.Update(read()); LoadDocGia(); Info("Đã cập nhật độc giả."); });
            btnXoa.Click += (s, e) => Try(() => { if (selDG == 0) throw new Exception("Chọn độc giả cần xóa."); if (Confirm("Xóa độc giả này?")) { dgBus.Delete(selDG); selDG = 0; LoadDocGia(); } });
            btnGiaHan.Click += (s, e) => Try(() =>
            {
                if (selDG == 0) throw new Exception("Chọn độc giả cần gia hạn.");
                dtpHetHan.Value = DateTime.Today.AddYears(1);
                dgBus.Update(read()); LoadDocGia(); Info("Đã gia hạn thẻ đến " + dtpHetHan.Value.ToString("dd/MM/yyyy"));
            });
            tp.Controls.Add(gridDG);
            tp.Controls.Add(pnl);
            return tp;
        }

        private void LoadDocGia()
        {
            gridDG.DataSource = dgBus.GetAll();
            SetHeaders(gridDG, ("MaDocGia", "Mã độc giả"), ("HoTen", "Họ tên"), ("Email", "Email"), ("NgayHetHanThe", "Ngày hết hạn thẻ"));
            if (gridDG.Columns.Contains("NgayHetHanThe")) gridDG.Columns["NgayHetHanThe"].DefaultCellStyle.Format = "dd/MM/yyyy";
        }

        private TabPage BuildLapPhieu()
        {
            var tp = new TabPage("Lập phiếu mượn") { BackColor = Color.White };
            var pnl = new Panel { Dock = DockStyle.Top, Height = 160 };
            cboDGMuon = new ComboBox { Left = 130, Top = 15, Width = 300, DropDownStyle = ComboBoxStyle.DropDownList };
            lblHanThe = new Label { Left = 130, Top = 45, Width = 320, Height = 22, Font = new Font("Segoe UI", 9.5F, FontStyle.Bold) };
            dtpHanTra = new DateTimePicker { Left = 130, Top = 75, Width = 160, Format = DateTimePickerFormat.Short, Value = DateTime.Today.AddDays(14) };
            cboSachMuon = new ComboBox { Left = 560, Top = 15, Width = 330, DropDownStyle = ComboBoxStyle.DropDownList };
            numSLMuon = new NumericUpDown { Left = 560, Top = 50, Width = 100, Minimum = 1, Maximum = 100, Value = 1 };
            var btnAdd = Btn("+ Thêm sách", 910, 12, Primary, 150);
            var btnRemove = Btn("− Bỏ sách", 910, 50, Color.Gray, 150);
            var btnLuu = Btn("💾 LẬP PHIẾU MƯỢN", 910, 105, Color.FromArgb(22, 163, 74), 200);
            pnl.Controls.AddRange(new Control[]
            {
                Lbl("Độc giả:", 20, 15), cboDGMuon, lblHanThe, Lbl("Hạn trả:", 20, 75), dtpHanTra,
                Lbl("Sách:", 460, 15, 90), cboSachMuon, Lbl("Số lượng:", 460, 50, 90), numSLMuon, btnAdd, btnRemove, btnLuu
            });
            gridGio = Grid(); gridGio.Dock = DockStyle.Fill;
            cboDGMuon.SelectedIndexChanged += (s, e) => CapNhatHanThe();
            btnAdd.Click += (s, e) => Try(() =>
            {
                if (!(cboSachMuon.SelectedItem is DataRowView sach)) throw new Exception("Chọn sách.");
                int ma = (int)sach["MaSach"];
                var exist = gio.FirstOrDefault(c => c.MaSach == ma);
                if (exist != null) exist.SoLuong += (int)numSLMuon.Value;
                else gio.Add(new ChiTietMuonDTO { MaSach = ma, TenSach = sach["TenSach"].ToString(), SoLuong = (int)numSLMuon.Value });
                RefreshGio();
            });
            btnRemove.Click += (s, e) =>
            {
                if (gridGio.CurrentRow != null && gridGio.CurrentRow.Index < gio.Count) { gio.RemoveAt(gridGio.CurrentRow.Index); RefreshGio(); }
            };
            btnLuu.Click += (s, e) =>
            {
                try
                {
                    var p = new PhieuMuonDTO { MaDocGia = cboDGMuon.SelectedValue is int v ? v : 0, HanTra = dtpHanTra.Value.Date.AddDays(1).AddSeconds(-1), ChiTiet = gio.ToList() };
                    int ma = pmBus.LapPhieu(p);
                    Info($"Lập phiếu mượn #{ma} thành công!\nHạn trả: {p.HanTra:dd/MM/yyyy}");
                    gio.Clear(); LoadLapPhieu();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Không thể lập phiếu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    LoadLapPhieu();
                }
            };
            tp.Controls.Add(gridGio);
            tp.Controls.Add(pnl);
            return tp;
        }

        private void LoadLapPhieu()
        {
            DataTable dg = dgBus.GetAll();
            cboDGMuon.DisplayMember = "HoTen"; cboDGMuon.ValueMember = "MaDocGia"; cboDGMuon.DataSource = dg;
            DataTable sach = sachBus.GetAll();
            sach.Columns.Add("Hien", typeof(string), "TenSach + ' | ' + TheLoai + ' | còn: ' + Convert(SoLuongCon, 'System.String')");
            cboSachMuon.DisplayMember = "Hien"; cboSachMuon.DataSource = sach;
            CapNhatHanThe();
            RefreshGio();
        }

        private void CapNhatHanThe()
        {
            if (!(cboDGMuon.SelectedItem is DataRowView r)) { lblHanThe.Text = ""; return; }
            if (r["NgayHetHanThe"] == DBNull.Value) { lblHanThe.Text = "Chưa có ngày hết hạn thẻ"; lblHanThe.ForeColor = Color.Red; return; }
            DateTime han = (DateTime)r["NgayHetHanThe"];
            bool conHan = han >= DateTime.Now;
            lblHanThe.Text = conHan ? $"✔ Thẻ còn hạn đến {han:dd/MM/yyyy}" : $"✖ Thẻ đã hết hạn từ {han:dd/MM/yyyy}";
            lblHanThe.ForeColor = conHan ? Color.FromArgb(22, 163, 74) : Color.Red;
        }

        private void RefreshGio()
        {
            gridGio.DataSource = null;
            gridGio.DataSource = gio.Select(c => new { c.MaSach, c.TenSach, c.SoLuong }).ToList();
            SetHeaders(gridGio, ("MaSach", "Mã sách"), ("TenSach", "Tên sách"), ("SoLuong", "Số lượng mượn"));
        }

        private TabPage BuildPhieu()
        {
            var tp = new TabPage("Phiếu mượn & Trả sách") { BackColor = Color.White };
            var pnl = new Panel { Dock = DockStyle.Top, Height = 52, BackColor = Color.FromArgb(241, 245, 249) };
            cboLocDG = new ComboBox { Left = 85, Top = 13, Width = 240, DropDownStyle = ComboBoxStyle.DropDownList };
            cboLocTT = new ComboBox { Left = 425, Top = 13, Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
            cboLocTT.Items.Add("(Tất cả)");
            cboLocTT.Items.AddRange(PhieuMuonBUS.TrangThaiList);
            cboLocTT.SelectedIndex = 0;
            var btnTra = Btn("📥 Trả sách", 600, 9, Color.FromArgb(22, 163, 74), 150);
            pnl.Controls.AddRange(new Control[] { Lbl("Độc giả:", 15, 13, 70), cboLocDG, Lbl("Trạng thái:", 340, 13, 85), cboLocTT, btnTra });
            gridPhieu = Grid(); gridPhieu.Dock = DockStyle.Fill;
            gridPhieu.CellFormatting += (s, e) =>
            {
                if (gridPhieu.Columns[e.ColumnIndex].Name != "TrangThai" || e.Value == null) return;
                string v = e.Value.ToString();
                e.CellStyle.ForeColor = v == "Đã trả" ? Color.FromArgb(22, 163, 74) : v == "Quá hạn" ? Color.Red : Primary;
                e.CellStyle.Font = new Font(gridPhieu.Font, FontStyle.Bold);
            };
            cboLocDG.SelectedIndexChanged += (s, e) => Try(LoadPhieu);
            cboLocTT.SelectedIndexChanged += (s, e) => Try(LoadPhieu);
            btnTra.Click += (s, e) => Try(() =>
            {
                if (!(gridPhieu.CurrentRow?.DataBoundItem is DataRowView r)) throw new Exception("Chọn phiếu mượn cần trả.");
                int ma = (int)r["MaPhieu"];
                pmBus.TraSach(ma, r["TrangThai"].ToString());
                LoadPhieu();
                Info($"Phiếu #{ma} đã chuyển sang \"Đã trả\".\nSố lượng sách đã được cộng lại vào kho.");
            });
            tp.Controls.Add(gridPhieu);
            tp.Controls.Add(pnl);
            return tp;
        }

        private void LoadComboLoc()
        {
            DataTable dg = dgBus.GetAll();
            DataRow all = dg.NewRow(); all["MaDocGia"] = 0; all["HoTen"] = "(Tất cả độc giả)"; dg.Rows.InsertAt(all, 0);
            int keep = cboLocDG.SelectedValue is int v ? v : 0;
            cboLocDG.DisplayMember = "HoTen"; cboLocDG.ValueMember = "MaDocGia"; cboLocDG.DataSource = dg;
            cboLocDG.SelectedValue = keep;
        }

        private void LoadPhieu()
        {
            int maDG = cboLocDG.SelectedValue is int v ? v : 0;
            string tt = cboLocTT.SelectedIndex <= 0 ? null : cboLocTT.SelectedItem.ToString();
            gridPhieu.DataSource = pmBus.Filter(maDG, tt);
            SetHeaders(gridPhieu, ("MaPhieu", "Mã phiếu"), ("HoTen", "Độc giả"), ("NgayMuon", "Ngày mượn"), ("HanTra", "Hạn trả"), ("TrangThai", "Trạng thái"), ("DanhSachSach", "Sách mượn"));
            if (gridPhieu.Columns.Contains("MaDocGia")) gridPhieu.Columns["MaDocGia"].Visible = false;
            foreach (var c in new[] { "NgayMuon", "HanTra" }) if (gridPhieu.Columns.Contains(c)) gridPhieu.Columns[c].DefaultCellStyle.Format = "dd/MM/yyyy";
        }

        private TabPage BuildBaoCao()
        {
            var tp = new TabPage("Quá hạn & Thống kê") { BackColor = Color.White };
            var split = new SplitContainer { Dock = DockStyle.Fill, Orientation = Orientation.Horizontal, SplitterDistance = 300 };
            gridQuaHan = Grid(); gridQuaHan.Dock = DockStyle.Fill;
            gridQuaHan.DefaultCellStyle.ForeColor = Color.FromArgb(185, 28, 28);
            gridTheLoai = Grid(); gridTheLoai.Dock = DockStyle.Fill;
            lblTongLuot = new Label { Dock = DockStyle.Bottom, Height = 30, Font = new Font("Segoe UI", 11F, FontStyle.Bold), ForeColor = Primary, TextAlign = ContentAlignment.MiddleLeft };
            split.Panel1.Controls.Add(gridQuaHan);
            split.Panel1.Controls.Add(new Label { Text = "⚠ PHIẾU MƯỢN QUÁ HẠN (HanTra < hôm nay và chưa trả)", Dock = DockStyle.Top, Height = 30, Font = new Font("Segoe UI", 10F, FontStyle.Bold), ForeColor = Color.FromArgb(185, 28, 28) });
            split.Panel2.Controls.Add(gridTheLoai);
            split.Panel2.Controls.Add(lblTongLuot);
            split.Panel2.Controls.Add(new Label { Text = "📊 THỐNG KÊ LƯỢT MƯỢN THEO THỂ LOẠI", Dock = DockStyle.Top, Height = 30, Font = new Font("Segoe UI", 10F, FontStyle.Bold), ForeColor = Primary });
            tp.Controls.Add(split);
            return tp;
        }

        private void LoadBaoCao()
        {
            pmBus.Filter(0, null);
            gridQuaHan.DataSource = bcBus.GetPhieuQuaHan();
            SetHeaders(gridQuaHan, ("MaPhieu", "Mã phiếu"), ("HoTen", "Độc giả"), ("Email", "Email"), ("NgayMuon", "Ngày mượn"), ("HanTra", "Hạn trả"), ("SoNgayQuaHan", "Số ngày quá hạn"), ("TrangThai", "Trạng thái"));
            foreach (var c in new[] { "NgayMuon", "HanTra" }) if (gridQuaHan.Columns.Contains(c)) gridQuaHan.Columns[c].DefaultCellStyle.Format = "dd/MM/yyyy";
            gridTheLoai.DataSource = bcBus.ThongKeTheoTheLoai();
            SetHeaders(gridTheLoai, ("TheLoai", "Thể loại"), ("SoPhieu", "Số phiếu"), ("TongLuotMuon", "Tổng lượt mượn"));
            lblTongLuot.Text = $"  Tổng số lượt mượn toàn thư viện: {bcBus.TongLuotMuon()}";
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
