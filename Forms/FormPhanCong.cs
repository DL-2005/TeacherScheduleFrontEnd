using TeacherScheduleFrontend.Models;
using TeacherScheduleFrontend.Services;

namespace TeacherScheduleFrontend.Forms
{
    public partial class FormPhanCong : Form
    {
        private DataGridView dgvPhanCong = null!;
        private ComboBox cboGiangVien = null!, cboMonHoc = null!, cboLop = null!, cboThu = null!, cboThoiGian = null!;
        private NumericUpDown nudTietBatDau = null!, nudSoTiet = null!;
        private TextBox txtPhongHoc = null!, txtGhiChu = null!, txtSearch = null!;
        private List<PhanCong> _allPhanCongs = new();
        private List<GiangVien> _giangViens = new();
        private List<MonHoc> _monHocs = new();
        private List<Lop> _lops = new();
        private int _currentId = 0;

        public FormPhanCong()
        {
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.Text = "Phân Công Giảng Dạy";
            this.BackColor = Color.White;

            // Header
            Panel pnlHeader = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.FromArgb(241, 196, 15) };
            pnlHeader.Controls.Add(new Label { Text = "📋 PHÂN CÔNG GIẢNG DẠY", Font = new Font("Segoe UI", 18, FontStyle.Bold), ForeColor = Color.FromArgb(44, 62, 80), AutoSize = true, Location = new Point(20, 15) });
            this.Controls.Add(pnlHeader);

            // Toolbar
            Panel pnlToolbar = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.FromArgb(245, 245, 245) };

            txtSearch = new TextBox { Location = new Point(15, 15), Size = new Size(250, 30), Font = new Font("Segoe UI", 11), PlaceholderText = "🔍 Tìm kiếm..." };
            txtSearch.TextChanged += TxtSearch_TextChanged;
            pnlToolbar.Controls.Add(txtSearch);

            AddButton(pnlToolbar, "➕ Thêm", 280, Color.FromArgb(0, 123, 255), BtnAdd_Click);
            AddButton(pnlToolbar, "🗑️ Xóa", 400, Color.FromArgb(220, 53, 69), BtnDelete_Click);
            AddButton(pnlToolbar, "🔄 Refresh", 520, Color.FromArgb(23, 162, 184), (s, e) => LoadData());

            this.Controls.Add(pnlToolbar);

            // Main Content
            SplitContainer split = new SplitContainer { Dock = DockStyle.Fill, Orientation = Orientation.Vertical, SplitterDistance = 480 };

            // Form
            Panel pnlForm = new Panel { Dock = DockStyle.Fill, Padding = new Padding(15), BackColor = Color.FromArgb(250, 250, 250), AutoScroll = true };
            int y = 10;

            // Giảng viên
            AddLabel(pnlForm, "Giảng Viên:", y);
            cboGiangVien = AddComboBox(pnlForm, y + 22);
            y += 55;

            // Môn học
            AddLabel(pnlForm, "Môn Học:", y);
            cboMonHoc = AddComboBox(pnlForm, y + 22);
            y += 55;

            // Lớp
            AddLabel(pnlForm, "Lớp:", y);
            cboLop = AddComboBox(pnlForm, y + 22);
            y += 55;

            // Thứ
            AddLabel(pnlForm, "Thứ:", y);
            cboThu = AddComboBox(pnlForm, y + 22);
            cboThu.Items.AddRange(new object[] { "Thứ 2", "Thứ 3", "Thứ 4", "Thứ 5", "Thứ 6", "Thứ 7", "Chủ nhật" });
            cboThu.SelectedIndex = 0;
            y += 55;

            // Tiết bắt đầu & Số tiết
            AddLabel(pnlForm, "Tiết Bắt Đầu:", y);
            nudTietBatDau = new NumericUpDown { Location = new Point(15, y + 22), Size = new Size(80, 28), Font = new Font("Segoe UI", 10), Minimum = 1, Maximum = 15, Value = 1 };
            pnlForm.Controls.Add(nudTietBatDau);

            pnlForm.Controls.Add(new Label { Text = "Số Tiết:", Location = new Point(120, y), AutoSize = true, Font = new Font("Segoe UI", 10) });
            nudSoTiet = new NumericUpDown { Location = new Point(120, y + 22), Size = new Size(80, 28), Font = new Font("Segoe UI", 10), Minimum = 1, Maximum = 10, Value = 3 };
            pnlForm.Controls.Add(nudSoTiet);
            y += 55;

            // Phòng học
            AddLabel(pnlForm, "Phòng Học:", y);
            txtPhongHoc = new TextBox { Location = new Point(15, y + 22), Size = new Size(200, 28), Font = new Font("Segoe UI", 10) };
            pnlForm.Controls.Add(txtPhongHoc);
            y += 55;

            // Thời gian học
            AddLabel(pnlForm, "Thời Gian Học:", y);
            cboThoiGian = AddComboBox(pnlForm, y + 22);
            var currentYear = DateTime.Now.Year;
            cboThoiGian.Items.AddRange(new object[] { $"HK1-{currentYear}", $"HK2-{currentYear}", $"HK1-{currentYear + 1}", $"HK2-{currentYear + 1}" });
            cboThoiGian.SelectedIndex = 0;
            y += 55;

            // Ghi chú
            AddLabel(pnlForm, "Ghi Chú:", y);
            txtGhiChu = new TextBox { Location = new Point(15, y + 22), Size = new Size(400, 50), Font = new Font("Segoe UI", 10), Multiline = true };
            pnlForm.Controls.Add(txtGhiChu);
            y += 85;

            // Buttons
            var btnSave = new Button { Text = "💾 Lưu", Location = new Point(15, y), Size = new Size(110, 38), BackColor = Color.FromArgb(40, 167, 69), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            btnSave.Click += BtnSave_Click;
            pnlForm.Controls.Add(btnSave);

            var btnClear = new Button { Text = "🔄 Mới", Location = new Point(135, y), Size = new Size(110, 38), BackColor = Color.FromArgb(108, 117, 125), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            btnClear.Click += (s, e) => ClearForm();
            pnlForm.Controls.Add(btnClear);

            split.Panel1.Controls.Add(pnlForm);

            // Grid
            dgvPhanCong = CreateGrid(Color.FromArgb(241, 196, 15), Color.FromArgb(44, 62, 80));
            dgvPhanCong.SelectionChanged += DgvPhanCong_SelectionChanged;
            split.Panel2.Controls.Add(dgvPhanCong);

            this.Controls.Add(split);
            this.ResumeLayout(false);
        }

        private void AddButton(Panel panel, string text, int x, Color bg, EventHandler handler)
        {
            var btn = new Button { Text = text, Location = new Point(x, 12), Size = new Size(110, 35), BackColor = bg, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10) };
            btn.Click += handler;
            panel.Controls.Add(btn);
        }

        private void AddLabel(Panel panel, string text, int y)
        {
            panel.Controls.Add(new Label { Text = text, Location = new Point(15, y), AutoSize = true, Font = new Font("Segoe UI", 10) });
        }

        private ComboBox AddComboBox(Panel panel, int y)
        {
            var cbo = new ComboBox { Location = new Point(15, y), Size = new Size(400, 28), Font = new Font("Segoe UI", 10), DropDownStyle = ComboBoxStyle.DropDownList };
            panel.Controls.Add(cbo);
            return cbo;
        }

        private DataGridView CreateGrid(Color headerBg, Color headerFg)
        {
            var dgv = new DataGridView
            {
                Dock = DockStyle.Fill, BackgroundColor = Color.White, BorderStyle = BorderStyle.None,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect, MultiSelect = false, ReadOnly = true,
                AllowUserToAddRows = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, RowHeadersVisible = false
            };
            dgv.ColumnHeadersDefaultCellStyle.BackColor = headerBg;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = headerFg;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgv.ColumnHeadersHeight = 38;
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 9);
            dgv.RowTemplate.Height = 30;
            return dgv;
        }

        private async void LoadData()
        {
            _giangViens = await ApiService.GetGiangViensAsync();
            _monHocs = await ApiService.GetMonHocsAsync();
            _lops = await ApiService.GetLopsAsync();
            _allPhanCongs = await ApiService.GetPhanCongsAsync();

            cboGiangVien.DataSource = null;
            cboGiangVien.DataSource = _giangViens;
            cboGiangVien.DisplayMember = "TenGV";
            cboGiangVien.ValueMember = "MaGV";

            cboMonHoc.DataSource = null;
            cboMonHoc.DataSource = _monHocs;
            cboMonHoc.DisplayMember = "TenMH";
            cboMonHoc.ValueMember = "MaMH";

            cboLop.DataSource = null;
            cboLop.DataSource = _lops;
            cboLop.DisplayMember = "MaLop";
            cboLop.ValueMember = "MaLop";

            DisplayData(_allPhanCongs);
        }

        private void DisplayData(List<PhanCong> data)
        {
            dgvPhanCong.DataSource = null;
            dgvPhanCong.DataSource = data.Select(pc => new
            {
                pc.Id,
                TenGV = pc.GiangVien?.TenGV ?? pc.MaGV,
                TenMH = pc.MonHoc?.TenMH ?? pc.MaMH,
                pc.MaLop,
                Thu = GetThuText(pc.Thu),
                Tiet = $"{pc.TietBatDau}-{pc.TietBatDau + pc.SoTiet - 1}",
                pc.PhongHoc,
                pc.ThoiGianHoc
            }).ToList();

            if (dgvPhanCong.Columns.Count > 0)
            {
                dgvPhanCong.Columns["Id"].Visible = false;
                dgvPhanCong.Columns["TenGV"].HeaderText = "Giảng Viên";
                dgvPhanCong.Columns["TenMH"].HeaderText = "Môn Học";
                dgvPhanCong.Columns["MaLop"].HeaderText = "Lớp";
                dgvPhanCong.Columns["Thu"].HeaderText = "Thứ";
                dgvPhanCong.Columns["Tiet"].HeaderText = "Tiết";
                dgvPhanCong.Columns["PhongHoc"].HeaderText = "Phòng";
                dgvPhanCong.Columns["ThoiGianHoc"].HeaderText = "Học Kỳ";
            }
        }

        private string GetThuText(int thu) => thu switch { 2 => "Thứ 2", 3 => "Thứ 3", 4 => "Thứ 4", 5 => "Thứ 5", 6 => "Thứ 6", 7 => "Thứ 7", 8 => "CN", _ => thu.ToString() };

        private void TxtSearch_TextChanged(object? sender, EventArgs e)
        {
            var s = txtSearch.Text.ToLower();
            var filtered = _allPhanCongs.Where(pc =>
                (pc.GiangVien?.TenGV?.ToLower().Contains(s) ?? false) ||
                (pc.MonHoc?.TenMH?.ToLower().Contains(s) ?? false) ||
                pc.MaLop.ToLower().Contains(s)
            ).ToList();
            DisplayData(filtered);
        }

        private void DgvPhanCong_SelectionChanged(object? sender, EventArgs e)
        {
            if (dgvPhanCong.CurrentRow != null)
            {
                var id = Convert.ToInt32(dgvPhanCong.CurrentRow.Cells["Id"].Value);
                var pc = _allPhanCongs.FirstOrDefault(p => p.Id == id);
                if (pc != null)
                {
                    _currentId = pc.Id;
                    cboGiangVien.SelectedValue = pc.MaGV;
                    cboMonHoc.SelectedValue = pc.MaMH;
                    cboLop.SelectedValue = pc.MaLop;
                    cboThu.SelectedIndex = pc.Thu - 2;
                    nudTietBatDau.Value = pc.TietBatDau;
                    nudSoTiet.Value = pc.SoTiet;
                    txtPhongHoc.Text = pc.PhongHoc ?? "";
                    txtGhiChu.Text = pc.GhiChu ?? "";
                    for (int i = 0; i < cboThoiGian.Items.Count; i++)
                        if (cboThoiGian.Items[i]?.ToString() == pc.ThoiGianHoc) { cboThoiGian.SelectedIndex = i; break; }
                }
            }
        }

        private void BtnAdd_Click(object? sender, EventArgs e) { ClearForm(); }

        private async void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (dgvPhanCong.CurrentRow == null) return;
            var id = Convert.ToInt32(dgvPhanCong.CurrentRow.Cells["Id"].Value);

            if (MessageBox.Show("Xóa phân công này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                var (success, message) = await ApiService.DeletePhanCongAsync(id);
                MessageBox.Show(message, success ? "Thành công" : "Lỗi", MessageBoxButtons.OK, success ? MessageBoxIcon.Information : MessageBoxIcon.Error);
                if (success) { LoadData(); ClearForm(); }
            }
        }

        private async void BtnSave_Click(object? sender, EventArgs e)
        {
            if (cboGiangVien.SelectedValue == null || cboMonHoc.SelectedValue == null || cboLop.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn đầy đủ thông tin!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var pc = new PhanCong
            {
                Id = _currentId,
                MaGV = cboGiangVien.SelectedValue.ToString()!,
                MaMH = cboMonHoc.SelectedValue.ToString()!,
                MaLop = cboLop.SelectedValue.ToString()!,
                Thu = cboThu.SelectedIndex + 2,
                TietBatDau = (int)nudTietBatDau.Value,
                SoTiet = (int)nudSoTiet.Value,
                PhongHoc = txtPhongHoc.Text.Trim(),
                ThoiGianHoc = cboThoiGian.SelectedItem?.ToString(),
                GhiChu = txtGhiChu.Text.Trim()
            };

            bool isNew = _currentId == 0;
            var (success, message) = isNew ? await ApiService.CreatePhanCongAsync(pc) : await ApiService.UpdatePhanCongAsync(pc.Id, pc);
            MessageBox.Show(message, success ? "Thành công" : "Lỗi", MessageBoxButtons.OK, success ? MessageBoxIcon.Information : MessageBoxIcon.Error);
            if (success) { LoadData(); ClearForm(); }
        }

        private void ClearForm()
        {
            _currentId = 0;
            if (cboGiangVien.Items.Count > 0) cboGiangVien.SelectedIndex = 0;
            if (cboMonHoc.Items.Count > 0) cboMonHoc.SelectedIndex = 0;
            if (cboLop.Items.Count > 0) cboLop.SelectedIndex = 0;
            cboThu.SelectedIndex = 0;
            nudTietBatDau.Value = 1;
            nudSoTiet.Value = 3;
            txtPhongHoc.Text = txtGhiChu.Text = "";
            cboThoiGian.SelectedIndex = 0;
            dgvPhanCong.ClearSelection();
        }
    }
}
