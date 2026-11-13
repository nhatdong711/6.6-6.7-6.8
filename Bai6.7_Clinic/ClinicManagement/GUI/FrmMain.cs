using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ClinicManagement.BUS;
using ClinicManagement.DTO;
using static ClinicManagement.GUI.UiHelper;

namespace ClinicManagement.GUI
{
    public class FrmMain : Form
    {
        private readonly BenhNhanBUS bnBus = new BenhNhanBUS();
        private readonly ThuocBUS thuocBus = new ThuocBUS();
        private readonly DonThuocBUS donBus = new DonThuocBUS();
        private readonly TabControl tabs = new TabControl { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10F) };
        private static readonly Color Teal = Color.FromArgb(13, 148, 136);

        private Label lblTongQuan;

        private TextBox txtTimSDT, txtHoTen, txtSDT, txtNamSinh, txtTrieuChung;
        private DataGridView gridBN, gridLichSu;
        private int selBN;

        private DataGridView gridThuoc;
        private TextBox txtTenThuoc;
        private ComboBox cboDVT;
        private NumericUpDown numGia, numTon, numNhap;
        private int selThuoc;

        private ComboBox cboHoSo, cboThuocKe;
        private TextBox txtChanDoan, txtLieuDung;
        private Label lblTrieuChung, lblTong;
        private NumericUpDown numTienKham, numSLKe;
        private DataGridView gridDon;
        private readonly List<ChiTietDonThuocDTO> donTam = new List<ChiTietDonThuocDTO>();

        private DataGridView gridDS, gridCT;

        private DataGridView gridDoanhThu, gridSapHet;

        public FrmMain()
        {
            Text = "Hệ thống Quản lý Phòng khám & Kê đơn thuốc (3-Tier)";
            Size = new Size(1300, 780);
            StartPosition = FormStartPosition.CenterScreen;
            Font = new Font("Segoe UI", 9.5F);
            BackColor = Color.White;
            var header = new Label
            {
                Text = "  🏥 CLINIC & PHARMACY MANAGEMENT", Dock = DockStyle.Top, Height = 48, BackColor = Teal,
                ForeColor = Color.White, Font = new Font("Segoe UI", 15F, FontStyle.Bold), TextAlign = ContentAlignment.MiddleLeft
            };
            tabs.TabPages.Add(BuildTongQuan());
            tabs.TabPages.Add(BuildTiepNhan());
            tabs.TabPages.Add(BuildThuoc());
            tabs.TabPages.Add(BuildKeDon());
            tabs.TabPages.Add(BuildDonThuoc());
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
                    case 1: gridBN.DataSource = bnBus.TraCuu(txtTimSDT.Text); HeadersBN(); break;
                    case 2: LoadThuoc(); break;
                    case 3: LoadKeDon(); break;
                    case 4: LoadDonThuoc(); break;
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
            DataTable don = donBus.GetAll();
            Func<string, int> dem = st => don.AsEnumerable().Count(r => r.Field<string>("TrangThai") == st);
            decimal dt = don.AsEnumerable().Where(r => r.Field<string>("TrangThai") == "Đã thanh toán").Sum(r => r.Field<decimal>("TongTien"));
            lblTongQuan.Text =
                "TỔNG QUAN PHÒNG KHÁM\n\n" +
                $"🧑  Số bệnh nhân: {bnBus.TraCuu("").Rows.Count}\n" +
                $"📝  Hồ sơ đang chờ kê đơn: {bnBus.GetHoSoChoKeDon().Rows.Count}\n" +
                $"💊  Số loại thuốc: {thuocBus.GetAll().Rows.Count}   (sắp hết: {donBus.ThuocSapHet().Rows.Count})\n" +
                $"📋  Đơn thuốc: {don.Rows.Count}  —  Chờ lấy thuốc: {dem("Chờ lấy thuốc")}, Đã thanh toán: {dem("Đã thanh toán")}\n" +
                $"💰  Doanh thu: {dt:N0} VNĐ\n\n" +
                "Quy trình: Tiếp nhận bệnh nhân → Bác sĩ chẩn đoán & kê đơn (SqlTransaction trừ kho) → Thanh toán & in hóa đơn.";
        }

        private TabPage BuildTiepNhan()
        {
            var tp = new TabPage("Tiếp nhận bệnh nhân") { BackColor = Color.White };
            var pnl = new Panel { Dock = DockStyle.Top, Height = 200 };
            txtTimSDT = new TextBox { Left = 130, Top = 12, Width = 200 };
            var btnTim = Btn("🔍 Tra cứu SĐT", 340, 8, Teal, 140);
            txtHoTen = new TextBox { Left = 130, Top = 55, Width = 250 };
            txtSDT = new TextBox { Left = 130, Top = 90, Width = 250 };
            txtNamSinh = new TextBox { Left = 130, Top = 125, Width = 120 };
            txtTrieuChung = new TextBox { Left = 520, Top = 55, Width = 360, Height = 95, Multiline = true };
            var btnTiepNhan = Btn("➕ Tiếp nhận & Tạo hồ sơ khám", 900, 55, Color.FromArgb(22, 163, 74), 260);
            var btnSua = Btn("Sửa thông tin BN", 900, 100, Color.FromArgb(234, 88, 12), 260);
            var btnMoi = Btn("Bệnh nhân mới", 900, 145, Color.Gray, 260);
            pnl.Controls.AddRange(new Control[]
            {
                Lbl("Tra SĐT:", 20, 12), txtTimSDT, btnTim,
                Lbl("Họ tên:", 20, 55), txtHoTen, Lbl("Số điện thoại:", 20, 90), txtSDT, Lbl("Năm sinh:", 20, 125), txtNamSinh,
                Lbl("Triệu chứng:", 410, 55), txtTrieuChung, btnTiepNhan, btnSua, btnMoi
            });
            var split = new SplitContainer { Dock = DockStyle.Fill, Orientation = Orientation.Horizontal, SplitterDistance = 260 };
            gridBN = Grid(); gridBN.Dock = DockStyle.Fill;
            gridLichSu = Grid(); gridLichSu.Dock = DockStyle.Fill;
            split.Panel1.Controls.Add(gridBN);
            split.Panel2.Controls.Add(gridLichSu);
            split.Panel2.Controls.Add(new Label { Text = "Lịch sử khám của bệnh nhân đang chọn", Dock = DockStyle.Top, Height = 26, Font = new Font("Segoe UI", 10F, FontStyle.Bold), ForeColor = Teal });
            gridBN.SelectionChanged += (s, e) => Try(() =>
            {
                if (gridBN.CurrentRow?.DataBoundItem is DataRowView r)
                {
                    selBN = (int)r["MaBN"];
                    txtHoTen.Text = r["HoTen"].ToString(); txtSDT.Text = r["SoDienThoai"].ToString(); txtNamSinh.Text = r["NamSinh"].ToString();
                    gridLichSu.DataSource = bnBus.GetLichSuKham(selBN);
                    SetHeaders(gridLichSu, ("MaHoSo", "Mã hồ sơ"), ("NgayKham", "Ngày khám"), ("TrieuChung", "Triệu chứng"), ("ChanDoan", "Chẩn đoán"));
                }
            });
            btnTim.Click += (s, e) => Try(() =>
            {
                DataTable dt = bnBus.TraCuu(txtTimSDT.Text);
                gridBN.DataSource = dt; HeadersBN();
                if (dt.Rows.Count == 0)
                {
                    selBN = 0; gridLichSu.DataSource = null;
                    txtHoTen.Clear(); txtNamSinh.Clear(); txtSDT.Text = txtTimSDT.Text.Trim();
                    Info("Không tìm thấy bệnh nhân. Vui lòng nhập thông tin bệnh nhân mới.");
                    txtHoTen.Focus();
                }
            });
            Func<BenhNhanDTO> read = () => new BenhNhanDTO
            {
                MaBN = selBN, HoTen = txtHoTen.Text.Trim(), SoDienThoai = txtSDT.Text.Trim(),
                NamSinh = int.TryParse(txtNamSinh.Text, out int ns) ? ns : (int?)null
            };
            btnTiepNhan.Click += (s, e) => Try(() =>
            {
                int maHoSo = bnBus.TiepNhan(read(), txtTrieuChung.Text);
                txtTrieuChung.Clear();
                txtTimSDT.Text = txtSDT.Text;
                Reload();
                Info($"Đã tạo hồ sơ khám #{maHoSo}. Chuyển sang tab \"Kê đơn thuốc\" để bác sĩ chẩn đoán.");
            });
            btnSua.Click += (s, e) => Try(() => { if (selBN == 0) throw new Exception("Chọn bệnh nhân cần sửa."); bnBus.Update(read()); Reload(); Info("Đã cập nhật thông tin bệnh nhân."); });
            btnMoi.Click += (s, e) => { selBN = 0; txtHoTen.Clear(); txtSDT.Clear(); txtNamSinh.Clear(); txtTrieuChung.Clear(); gridLichSu.DataSource = null; txtHoTen.Focus(); };
            tp.Controls.Add(split);
            tp.Controls.Add(pnl);
            return tp;
        }

        private void HeadersBN() => SetHeaders(gridBN, ("MaBN", "Mã BN"), ("HoTen", "Họ tên"), ("SoDienThoai", "Số điện thoại"), ("NamSinh", "Năm sinh"));

        private TabPage BuildThuoc()
        {
            var tp = new TabPage("Danh mục thuốc") { BackColor = Color.White };
            var pnl = new Panel { Dock = DockStyle.Top, Height = 165 };
            txtTenThuoc = new TextBox { Left = 130, Top = 15, Width = 300 };
            cboDVT = new ComboBox { Left = 130, Top = 50, Width = 150 };
            cboDVT.Items.AddRange(ThuocBUS.DonViList); cboDVT.SelectedIndex = 0;
            numGia = new NumericUpDown { Left = 130, Top = 85, Width = 180, Maximum = 100000000, ThousandsSeparator = true, Increment = 500 };
            numTon = new NumericUpDown { Left = 130, Top = 120, Width = 120, Maximum = 1000000 };
            var btnThem = Btn("Thêm thuốc", 640, 15, Color.FromArgb(22, 163, 74), 120);
            var btnSua = Btn("Cập nhật giá", 770, 15, Color.FromArgb(234, 88, 12), 120);
            var btnXoa = Btn("Xóa", 900, 15, Color.FromArgb(220, 38, 38));
            numNhap = new NumericUpDown { Left = 770, Top = 72, Width = 120, Minimum = 1, Maximum = 100000, Value = 100 };
            var btnNhap = Btn("📦 Nhập thêm kho", 900, 68, Teal, 170);
            pnl.Controls.AddRange(new Control[]
            {
                Lbl("Tên thuốc:", 20, 15), txtTenThuoc, Lbl("Đơn vị tính:", 20, 50), cboDVT, Lbl("Đơn giá:", 20, 85), numGia, Lbl("Số lượng tồn:", 20, 120), numTon,
                btnThem, btnSua, btnXoa, Lbl("SL nhập thêm:", 640, 72, 125), numNhap, btnNhap
            });
            gridThuoc = Grid(); gridThuoc.Dock = DockStyle.Fill;
            gridThuoc.SelectionChanged += (s, e) =>
            {
                if (gridThuoc.CurrentRow?.DataBoundItem is DataRowView r)
                {
                    selThuoc = (int)r["MaThuoc"]; txtTenThuoc.Text = r["TenThuoc"].ToString(); cboDVT.Text = r["DonViTinh"].ToString();
                    numGia.Value = (decimal)r["DonGia"]; numTon.Value = (int)r["SoLuongTon"];
                }
            };
            gridThuoc.CellFormatting += (s, e) =>
            {
                if (gridThuoc.Columns[e.ColumnIndex].Name == "SoLuongTon" && e.Value is int ton && ton <= 10)
                { e.CellStyle.ForeColor = Color.Red; e.CellStyle.Font = new Font(gridThuoc.Font, FontStyle.Bold); }
            };
            Func<ThuocDTO> read = () => new ThuocDTO { MaThuoc = selThuoc, TenThuoc = txtTenThuoc.Text.Trim(), DonViTinh = cboDVT.Text.Trim(), DonGia = numGia.Value, SoLuongTon = (int)numTon.Value };
            btnThem.Click += (s, e) => Try(() => { thuocBus.Insert(read()); LoadThuoc(); Info("Đã thêm thuốc mới."); });
            btnSua.Click += (s, e) => Try(() => { if (selThuoc == 0) throw new Exception("Chọn thuốc cần sửa."); thuocBus.Update(read()); LoadThuoc(); Info("Đã cập nhật thông tin thuốc."); });
            btnXoa.Click += (s, e) => Try(() => { if (selThuoc == 0) throw new Exception("Chọn thuốc cần xóa."); if (Confirm("Xóa thuốc này?")) { thuocBus.Delete(selThuoc); selThuoc = 0; LoadThuoc(); } });
            btnNhap.Click += (s, e) => Try(() => { thuocBus.NhapKho(selThuoc, (int)numNhap.Value); LoadThuoc(); Info($"Đã nhập thêm {numNhap.Value} vào kho."); });
            tp.Controls.Add(gridThuoc);
            tp.Controls.Add(pnl);
            return tp;
        }

        private void LoadThuoc()
        {
            gridThuoc.DataSource = thuocBus.GetAll();
            SetHeaders(gridThuoc, ("MaThuoc", "Mã thuốc"), ("TenThuoc", "Tên thuốc"), ("DonViTinh", "Đơn vị"), ("DonGia", "Đơn giá"), ("SoLuongTon", "Tồn kho"));
            if (gridThuoc.Columns.Contains("DonGia")) gridThuoc.Columns["DonGia"].DefaultCellStyle.Format = "N0";
        }

        private TabPage BuildKeDon()
        {
            var tp = new TabPage("Kê đơn thuốc") { BackColor = Color.White };
            var pnl = new Panel { Dock = DockStyle.Top, Height = 215 };
            cboHoSo = new ComboBox { Left = 130, Top = 15, Width = 340, DropDownStyle = ComboBoxStyle.DropDownList };
            lblTrieuChung = new Label { Left = 130, Top = 45, Width = 340, Height = 40, ForeColor = Color.DimGray };
            txtChanDoan = new TextBox { Left = 130, Top = 90, Width = 340 };
            numTienKham = new NumericUpDown { Left = 130, Top = 125, Width = 160, Maximum = 10000000, ThousandsSeparator = true, Increment = 10000, Value = 100000 };
            cboThuocKe = new ComboBox { Left = 600, Top = 15, Width = 330, DropDownStyle = ComboBoxStyle.DropDownList };
            numSLKe = new NumericUpDown { Left = 600, Top = 50, Width = 100, Minimum = 1, Maximum = 1000, Value = 10 };
            txtLieuDung = new TextBox { Left = 600, Top = 85, Width = 330, Text = "Sáng 1 viên, Chiều 1 viên sau ăn" };
            var btnAdd = Btn("+ Thêm thuốc", 950, 12, Teal, 150);
            var btnRemove = Btn("− Bỏ thuốc", 950, 50, Color.Gray, 150);
            var btnLuu = Btn("💾 LƯU & XUẤT ĐƠN", 950, 120, Color.FromArgb(22, 163, 74), 220);
            lblTong = new Label { Left = 130, Top = 165, Width = 700, Font = new Font("Segoe UI", 13F, FontStyle.Bold), ForeColor = Color.FromArgb(185, 28, 28) };
            pnl.Controls.AddRange(new Control[]
            {
                Lbl("Hồ sơ khám:", 20, 15), cboHoSo, Lbl("Triệu chứng:", 20, 45), lblTrieuChung, Lbl("Chẩn đoán:", 20, 90), txtChanDoan, Lbl("Tiền khám:", 20, 125), numTienKham,
                Lbl("Thuốc:", 500, 15, 95), cboThuocKe, Lbl("Số lượng:", 500, 50, 95), numSLKe, Lbl("Liều dùng:", 500, 85, 95), txtLieuDung,
                btnAdd, btnRemove, btnLuu, lblTong
            });
            gridDon = Grid(); gridDon.Dock = DockStyle.Fill;
            cboHoSo.SelectedIndexChanged += (s, e) =>
            {
                if (cboHoSo.SelectedItem is DataRowView r)
                {
                    lblTrieuChung.Text = r["TrieuChung"].ToString();
                    txtChanDoan.Text = r["ChanDoan"].ToString();
                }
                else { lblTrieuChung.Text = ""; txtChanDoan.Clear(); }
            };
            numTienKham.ValueChanged += (s, e) => RefreshDon();
            btnAdd.Click += (s, e) => Try(() =>
            {
                if (!(cboThuocKe.SelectedItem is DataRowView t)) throw new Exception("Chọn thuốc.");
                if (string.IsNullOrWhiteSpace(txtLieuDung.Text)) throw new Exception("Nhập liều dùng.");
                int ma = (int)t["MaThuoc"];
                var exist = donTam.FirstOrDefault(c => c.MaThuoc == ma);
                if (exist != null) { exist.SoLuong += (int)numSLKe.Value; exist.LieuDung = txtLieuDung.Text.Trim(); }
                else donTam.Add(new ChiTietDonThuocDTO
                {
                    MaThuoc = ma, TenThuoc = t["TenThuoc"].ToString(), DonViTinh = t["DonViTinh"].ToString(),
                    DonGia = (decimal)t["DonGia"], SoLuong = (int)numSLKe.Value, LieuDung = txtLieuDung.Text.Trim()
                });
                RefreshDon();
            });
            btnRemove.Click += (s, e) =>
            {
                if (gridDon.CurrentRow != null && gridDon.CurrentRow.Index < donTam.Count) { donTam.RemoveAt(gridDon.CurrentRow.Index); RefreshDon(); }
            };
            btnLuu.Click += (s, e) =>
            {
                try
                {
                    var don = new DonThuocDTO
                    {
                        MaHoSo = cboHoSo.SelectedValue is int v ? v : 0, ChanDoan = txtChanDoan.Text,
                        TienKham = numTienKham.Value, ChiTiet = donTam.ToList()
                    };
                    int ma = donBus.LuuDon(don);
                    MessageBox.Show(donBus.InHoaDon(ma), $"Đã lưu & xuất đơn thuốc #{ma}", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    donTam.Clear(); LoadKeDon();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("GIAO DỊCH ĐÃ BỊ HỦY (trans.Rollback())\n\n" + ex.Message + "\n\nĐơn thuốc chưa được lưu, tồn kho giữ nguyên.",
                        "Cảnh báo kê đơn", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    LoadKeDon();
                }
            };
            tp.Controls.Add(gridDon);
            tp.Controls.Add(pnl);
            return tp;
        }

        private void LoadKeDon()
        {
            DataTable hs = bnBus.GetHoSoChoKeDon();
            hs.Columns.Add("Hien", typeof(string), "'HS#' + Convert(MaHoSo, 'System.String') + ' - ' + HoTen + ' (' + SoDienThoai + ')'");
            cboHoSo.DisplayMember = "Hien"; cboHoSo.ValueMember = "MaHoSo"; cboHoSo.DataSource = hs;
            DataTable th = thuocBus.GetAll();
            th.Columns.Add("Hien", typeof(string), "TenThuoc + ' | ' + Convert(DonGia, 'System.String') + 'đ/' + DonViTinh + ' | tồn: ' + Convert(SoLuongTon, 'System.String')");
            cboThuocKe.DisplayMember = "Hien"; cboThuocKe.DataSource = th;
            RefreshDon();
        }

        private void RefreshDon()
        {
            gridDon.DataSource = null;
            gridDon.DataSource = donTam.Select(c => new { c.MaThuoc, c.TenThuoc, c.DonViTinh, c.SoLuong, DonGia = c.DonGia.ToString("N0"), ThanhTien = c.ThanhTien.ToString("N0"), c.LieuDung }).ToList();
            SetHeaders(gridDon, ("MaThuoc", "Mã"), ("TenThuoc", "Thuốc"), ("DonViTinh", "ĐVT"), ("SoLuong", "SL"), ("DonGia", "Đơn giá"), ("ThanhTien", "Thành tiền"), ("LieuDung", "Liều dùng"));
            decimal tong = DonThuocBUS.TinhTongTien(new DonThuocDTO { TienKham = numTienKham.Value, ChiTiet = donTam });
            lblTong.Text = $"Tổng chi phí = Tiền khám {numTienKham.Value:N0} + Tiền thuốc {tong - numTienKham.Value:N0} = {tong:N0} VNĐ";
        }

        private TabPage BuildDonThuoc()
        {
            var tp = new TabPage("Đơn thuốc & Thanh toán") { BackColor = Color.White };
            var pnl = new Panel { Dock = DockStyle.Top, Height = 50 };
            var btnTT = Btn("💵 Thanh toán & In hóa đơn", 15, 9, Color.FromArgb(22, 163, 74), 250);
            var btnHD = Btn("🧾 Xem hóa đơn", 275, 9, Color.Gray, 150);
            pnl.Controls.AddRange(new Control[] { btnTT, btnHD });
            var split = new SplitContainer { Dock = DockStyle.Fill, Orientation = Orientation.Horizontal, SplitterDistance = 320 };
            gridDS = Grid(); gridDS.Dock = DockStyle.Fill;
            gridCT = Grid(); gridCT.Dock = DockStyle.Fill;
            split.Panel1.Controls.Add(gridDS);
            split.Panel2.Controls.Add(gridCT);
            split.Panel2.Controls.Add(new Label { Text = "Chi tiết thuốc trong đơn đang chọn", Dock = DockStyle.Top, Height = 26, Font = new Font("Segoe UI", 10F, FontStyle.Bold), ForeColor = Teal });
            gridDS.SelectionChanged += (s, e) => Try(() =>
            {
                if (gridDS.CurrentRow?.DataBoundItem is DataRowView r)
                {
                    gridCT.DataSource = donBus.GetChiTiet((int)r["MaDon"]);
                    SetHeaders(gridCT, ("MaThuoc", "Mã"), ("TenThuoc", "Thuốc"), ("DonViTinh", "ĐVT"), ("SoLuong", "SL"), ("DonGia", "Đơn giá"), ("ThanhTien", "Thành tiền"), ("LieuDung", "Liều dùng"));
                    foreach (var c in new[] { "DonGia", "ThanhTien" }) if (gridCT.Columns.Contains(c)) gridCT.Columns[c].DefaultCellStyle.Format = "N0";
                }
            });
            gridDS.CellFormatting += (s, e) =>
            {
                if (gridDS.Columns[e.ColumnIndex].Name == "TrangThai" && e.Value != null)
                    e.CellStyle.ForeColor = e.Value.ToString() == "Đã thanh toán" ? Color.Green : Color.OrangeRed;
            };
            Func<DataRowView> sel = () => gridDS.CurrentRow?.DataBoundItem as DataRowView ?? throw new Exception("Chọn đơn thuốc.");
            btnTT.Click += (s, e) => Try(() =>
            {
                var r = sel();
                string hd = donBus.ThanhToan((int)r["MaDon"], r["TrangThai"].ToString());
                LoadDonThuoc();
                MessageBox.Show(hd, "Hóa đơn thanh toán", MessageBoxButtons.OK, MessageBoxIcon.Information);
            });
            btnHD.Click += (s, e) => Try(() => MessageBox.Show(donBus.InHoaDon((int)sel()["MaDon"]), "Hóa đơn"));
            tp.Controls.Add(split);
            tp.Controls.Add(pnl);
            return tp;
        }

        private void LoadDonThuoc()
        {
            gridDS.DataSource = donBus.GetAll();
            SetHeaders(gridDS, ("MaDon", "Mã đơn"), ("MaHoSo", "Hồ sơ"), ("HoTen", "Bệnh nhân"), ("SoDienThoai", "SĐT"), ("NgayKham", "Ngày khám"),
                ("ChanDoan", "Chẩn đoán"), ("TienKham", "Tiền khám"), ("TongTien", "Tổng tiền"), ("TrangThai", "Trạng thái"));
            foreach (var c in new[] { "TienKham", "TongTien" }) if (gridDS.Columns.Contains(c)) gridDS.Columns[c].DefaultCellStyle.Format = "N0";
        }

        private TabPage BuildBaoCao()
        {
            var tp = new TabPage("Báo cáo - Thống kê") { BackColor = Color.White };
            var split = new SplitContainer { Dock = DockStyle.Fill, Orientation = Orientation.Horizontal, SplitterDistance = 280 };
            gridDoanhThu = Grid(); gridDoanhThu.Dock = DockStyle.Fill;
            gridSapHet = Grid(); gridSapHet.Dock = DockStyle.Fill; gridSapHet.DefaultCellStyle.ForeColor = Color.Red;
            split.Panel1.Controls.Add(gridDoanhThu);
            split.Panel1.Controls.Add(new Label { Text = "💰 DOANH THU THEO NGÀY (đơn đã thanh toán)", Dock = DockStyle.Top, Height = 28, Font = new Font("Segoe UI", 10F, FontStyle.Bold), ForeColor = Teal });
            split.Panel2.Controls.Add(gridSapHet);
            split.Panel2.Controls.Add(new Label { Text = "⚠ THUỐC SẮP HẾT (tồn ≤ 10)", Dock = DockStyle.Top, Height = 28, Font = new Font("Segoe UI", 10F, FontStyle.Bold), ForeColor = Color.Red });
            tp.Controls.Add(split);
            return tp;
        }

        private void LoadBaoCao()
        {
            gridDoanhThu.DataSource = donBus.DoanhThuTheoNgay();
            SetHeaders(gridDoanhThu, ("Ngay", "Ngày"), ("SoDon", "Số đơn"), ("TienKham", "Tiền khám"), ("TienThuoc", "Tiền thuốc"), ("DoanhThu", "Doanh thu"));
            foreach (DataGridViewColumn c in gridDoanhThu.Columns) if (c.ValueType == typeof(decimal)) c.DefaultCellStyle.Format = "N0";
            gridSapHet.DataSource = donBus.ThuocSapHet();
            SetHeaders(gridSapHet, ("MaThuoc", "Mã"), ("TenThuoc", "Thuốc"), ("DonViTinh", "ĐVT"), ("SoLuongTon", "Tồn kho"));
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
