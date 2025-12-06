using TeacherScheduleFrontend.Models;
using TeacherScheduleFrontend.Services;

namespace TeacherScheduleFrontend.Forms
{
    public partial class FormTaiKhoan : Form
    {
        private DataGridView dgvTaiKhoan = null!;
        private TextBox txtMaTK = null!, txtMatKhau = null!, txtXacNhan = null!, txtSearch = null!;
        private ComboBox cboChucVu = null!, cboGiangVien = null!, cboKhoa = null!, cboBoMon = null!;
        private List<TaiKhoan> _allTaiKhoans = new();
        private List<GiangVien> _giangViens = new();
        private List<Khoa> _khoas = new();
        private List<BoMon> _boMons = new();

        public FormTaiKhoan()
        {
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.Text = "Quản Lý Tài Khoản";
            this.BackColor = Color.White;

            // Header
            Panel pnlHeader = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.FromArgb(142, 68, 173) };
            pnlHeader.Controls.Add(new Label { Text = "👤 QUẢN LÝ TÀI KHOẢN", Font = new Font("Segoe UI", 18, FontStyle.Bold), ForeColor = Color.White, AutoSize = true, Location = new Point(20, 15) });
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

            // Main
            SplitContainer split = new SplitContainer { Dock = DockStyle.Fill, Orientation = Orientation.Vertical, SplitterDistance = 450 };

            // Form
            Panel pnlForm = new Panel { Dock = DockStyle.Fill, Padding = new Padding(15), BackColor = Color.FromArgb(250, 250, 250), AutoScroll = true };
            int y = 10;

            AddLabel(pnlForm, "Tên đăng nhập:", y);
            txtMaTK = new TextBox { Location = new Point(15, y + 22), Size = new Size(380, 28), Font = new Font("Segoe UI", 10) };
            pnlForm.Controls.Add(txtMaTK);
            y += 55;

            AddLabel(pnlForm, "Mật khẩu:", y);
            txtMatKhau = new TextBox { Location = new Point(15, y + 22), Size = new Size(380, 28), Font = new Font("Segoe UI", 10), PasswordChar = '●' };
            pnlForm.Controls.Add(txtMatKhau);
            y += 55;

            AddLabel(pnlForm, "Xác nhận MK:", y);
            txtXacNhan = new TextBox { Location = new Point(15, y + 22), Size = new Size(380, 28), Font = new Font("Segoe UI", 10), PasswordChar = '●' };
            pnlForm.Controls.Add(txtXacNhan);
            y += 55;

            AddLabel(pnlForm, "Chức vụ:", y);
            cboChucVu = new ComboBox { Location = new Point(15, y + 22), Size = new Size(380, 28), Font = new Font("Segoe UI", 10), DropDownStyle = ComboBoxStyle.DropDownList };
            cboChucVu.Items.AddRange(new object[] { "CQC - Cán bộ quản lý", "TK - Trưởng khoa", "TBM - Trưởng bộ môn", "GV - Giảng viên" });
            cboChucVu.SelectedIndex = 3;
            cboChucVu.SelectedIndexChanged += CboChucVu_SelectedIndexChanged;
            pnlForm.Controls.Add(cboChucVu);
            y += 55;

            AddLabel(pnlForm, "Giảng viên:", y);
            cboGiangVien = new ComboBox { Location = new Point(15, y + 22), Size = new Size(380, 28), Font = new Font("Segoe UI", 10), DropDownStyle = ComboBoxStyle.DropDownList };
            pnlForm.Controls.Add(cboGiangVien);
            y += 55;

            AddLabel(pnlForm, "Khoa:", y);
            cboKhoa = new ComboBox { Location = new Point(15, y + 22), Size = new Size(380, 28), Font = new Font("Segoe UI", 10), DropDownStyle = ComboBoxStyle.DropDownList };
            pnlForm.Controls.Add(cboKhoa);
            y += 55;

            AddLabel(pnlForm, "Bộ môn:", y);
            cboBoMon = new ComboBox { Location = new Point(15, y + 22), Size = new Size(380, 28), Font = new Font("Segoe UI", 10), DropDownStyle = ComboBoxStyle.DropDownList };
            pnlForm.Controls.Add(cboBoMon);
            y += 60;

            var btnSave = new Button { Text = "💾 Tạo TK", Location = new Point(15, y), Size = new Size(130, 38), BackColor = Color.FromArgb(40, 167, 69), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            btnSave.Click += BtnSave_Click;
            pnlForm.Controls.Add(btnSave);

            var btnClear = new Button { Text = "🔄 Mới", Location = new Point(155, y), Size = new Size(100, 38), BackColor = Color.FromArgb(108, 117, 125), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            btnClear.Click += (s, e) => ClearForm();
            pnlForm.Controls.Add(btnClear);

            split.Panel1.Controls.Add(pnlForm);

            // Grid
            dgvTaiKhoan = CreateGrid(Color.FromArgb(142, 68, 173));
            split.Panel2.Controls.Add(dgvTaiKhoan);

            this.Controls.Add(split);
            this.ResumeLayout(false);
        }

        private void AddButton(Panel p, string t, int x, Color bg, EventHandler h)
        {
            var btn = new Button { Text = t, Location = new Point(x, 12), Size = new Size(110, 35), BackColor = bg, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10) };
            btn.Click += h;
            p.Controls.Add(btn);
        }

        private void AddLabel(Panel p, string t, int y) => p.Controls.Add(new Label { Text = t, Location = new Point(15, y), AutoSize = true, Font = new Font("Segoe UI", 10) });

        private DataGridView CreateGrid(Color hc)
        {
            var dgv = new DataGridView { Dock = DockStyle.Fill, BackgroundColor = Color.White, BorderStyle = BorderStyle.None, SelectionMode = DataGridViewSelectionMode.FullRowSelect, MultiSelect = false, ReadOnly = true, AllowUserToAddRows = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, RowHeadersVisible = false };
            dgv.ColumnHeadersDefaultCellStyle.BackColor = hc;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgv.ColumnHeadersHeight = 40;
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgv.RowTemplate.Height = 35;
            return dgv;
        }

        private async void LoadData()
        {
            _giangViens = await ApiService.GetGiangViensAsync();
            _khoas = await ApiService.GetKhoasAsync();
            _boMons = await ApiService.GetBoMonsAsync();
            _allTaiKhoans = await ApiService.GetTaiKhoansAsync();

            cboGiangVien.DataSource = null;
            var gvList = new List<GiangVien> { new GiangVien { MaGV = "", TenGV = "-- Chọn --" } };
            gvList.AddRange(_giangViens);
            cboGiangVien.DataSource = gvList;
            cboGiangVien.DisplayMember = "TenGV";
            cboGiangVien.ValueMember = "MaGV";

            cboKhoa.DataSource = null;
            var khoaList = new List<Khoa> { new Khoa { MaKhoa = "", TenKhoa = "-- Chọn --" } };
            khoaList.AddRange(_khoas);
            cboKhoa.DataSource = khoaList;
            cboKhoa.DisplayMember = "TenKhoa";
            cboKhoa.ValueMember = "MaKhoa";

            cboBoMon.DataSource = null;
            var bmList = new List<BoMon> { new BoMon { MaBM = "", TenBM = "-- Chọn --" } };
            bmList.AddRange(_boMons);
            cboBoMon.DataSource = bmList;
            cboBoMon.DisplayMember = "TenBM";
            cboBoMon.ValueMember = "MaBM";

            DisplayData(_allTaiKhoans);
            UpdateFieldVisibility();
        }

        private void DisplayData(List<TaiKhoan> data)
        {
            dgvTaiKhoan.DataSource = null;
            dgvTaiKhoan.DataSource = data.Select(tk => new { tk.MaTK, ChucVu = GetChucVuName(tk.ChucVu), TenGV = tk.GiangVien?.TenGV ?? tk.MaGV, tk.MaKhoa, tk.MaBM }).ToList();
            if (dgvTaiKhoan.Columns.Count > 0) { dgvTaiKhoan.Columns["MaTK"].HeaderText = "Tên ĐN"; dgvTaiKhoan.Columns["ChucVu"].HeaderText = "Chức Vụ"; dgvTaiKhoan.Columns["TenGV"].HeaderText = "GV"; dgvTaiKhoan.Columns["MaKhoa"].HeaderText = "Khoa"; dgvTaiKhoan.Columns["MaBM"].HeaderText = "BM"; }
        }

        private string GetChucVuName(string cv) => cv switch { "CQC" => "Cán bộ QL", "TK" => "Trưởng khoa", "TBM" => "Trưởng BM", "GV" => "Giảng viên", _ => cv };

        private void CboChucVu_SelectedIndexChanged(object? s, EventArgs e) => UpdateFieldVisibility();

        private void UpdateFieldVisibility()
        {
            var sel = cboChucVu.SelectedItem?.ToString() ?? "";
            cboGiangVien.Enabled = sel.Contains("GV") || sel.Contains("TBM");
            cboKhoa.Enabled = sel.Contains("TK");
            cboBoMon.Enabled = sel.Contains("TBM");
        }

        private void TxtSearch_TextChanged(object? s, EventArgs e)
        {
            var search = txtSearch.Text.ToLower();
            DisplayData(_allTaiKhoans.Where(tk => tk.MaTK.ToLower().Contains(search)).ToList());
        }

        private void BtnAdd_Click(object? s, EventArgs e) { ClearForm(); txtMaTK.Focus(); }

        private async void BtnDelete_Click(object? s, EventArgs e)
        {
            if (dgvTaiKhoan.CurrentRow == null) return;
            var maTK = dgvTaiKhoan.CurrentRow.Cells["MaTK"].Value?.ToString();
            if (string.IsNullOrEmpty(maTK) || maTK.ToUpper() == "ADMIN") { MessageBox.Show("Không thể xóa!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (MessageBox.Show($"Xóa '{maTK}'?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                var (success, message) = await ApiService.DeleteTaiKhoanAsync(maTK);
                MessageBox.Show(message, success ? "OK" : "Lỗi", MessageBoxButtons.OK, success ? MessageBoxIcon.Information : MessageBoxIcon.Error);
                if (success) { LoadData(); ClearForm(); }
            }
        }

        private async void BtnSave_Click(object? s, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaTK.Text)) { MessageBox.Show("Nhập tên đăng nhập!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (txtMatKhau.Text.Length < 6) { MessageBox.Show("Mật khẩu >= 6 ký tự!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (txtMatKhau.Text != txtXacNhan.Text) { MessageBox.Show("MK không khớp!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            var chucVu = (cboChucVu.SelectedItem?.ToString() ?? "").Split('-')[0].Trim();
            var req = new RegisterRequest
            {
                MaTK = txtMaTK.Text.Trim().ToUpper(),
                MatKhau = txtMatKhau.Text,
                XacNhanMatKhau = txtXacNhan.Text,
                ChucVu = chucVu,
                MaGV = (chucVu == "GV" || chucVu == "TBM") ? cboGiangVien.SelectedValue?.ToString() : null,
                MaKhoa = chucVu == "TK" ? cboKhoa.SelectedValue?.ToString() : null,
                MaBM = chucVu == "TBM" ? cboBoMon.SelectedValue?.ToString() : null
            };

            var (success, message) = await ApiService.RegisterAsync(req);
            MessageBox.Show(message, success ? "OK" : "Lỗi", MessageBoxButtons.OK, success ? MessageBoxIcon.Information : MessageBoxIcon.Error);
            if (success) { LoadData(); ClearForm(); }
        }

        private void ClearForm()
        {
            txtMaTK.Text = txtMatKhau.Text = txtXacNhan.Text = "";
            cboChucVu.SelectedIndex = 3;
            if (cboGiangVien.Items.Count > 0) cboGiangVien.SelectedIndex = 0;
            if (cboKhoa.Items.Count > 0) cboKhoa.SelectedIndex = 0;
            if (cboBoMon.Items.Count > 0) cboBoMon.SelectedIndex = 0;
            dgvTaiKhoan.ClearSelection();
        }
    }
}
