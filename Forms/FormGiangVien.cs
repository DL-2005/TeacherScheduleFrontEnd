using TeacherScheduleFrontend.Models;
using TeacherScheduleFrontend.Services;

namespace TeacherScheduleFrontend.Forms
{
    public partial class FormGiangVien : Form
    {
        private DataGridView dgvGiangVien = null!;
        private TextBox txtMaGV = null!, txtTenGV = null!, txtEmail = null!, txtSDT = null!, txtDiaChi = null!, txtSearch = null!;
        private DateTimePicker dtpNgaySinh = null!;
        private ComboBox cboKhoa = null!, cboBoMon = null!;
        private List<GiangVien> _allGiangViens = new();
        private List<Khoa> _khoas = new();
        private List<BoMon> _boMons = new();

        public FormGiangVien()
        {
            InitializeComponent();
            LoadData();
        }
        
        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.Text = "Quản Lý Giảng Viên";
            this.BackColor = Color.White;

            // Header
            Panel pnlHeader = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.FromArgb(52, 152, 219) };
            pnlHeader.Controls.Add(new Label
            {
                Text = "👨‍🏫 QUẢN LÝ GIẢNG VIÊN",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(20, 15)
            });
            this.Controls.Add(pnlHeader);

            // Toolbar
            Panel pnlToolbar = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.FromArgb(245, 245, 245) };

            txtSearch = new TextBox { Location = new Point(15, 15), Size = new Size(250, 30), Font = new Font("Segoe UI", 11), PlaceholderText = "🔍 Tìm kiếm giảng viên..." };
            txtSearch.TextChanged += TxtSearch_TextChanged;
            pnlToolbar.Controls.Add(txtSearch);

            AddToolbarButton(pnlToolbar, "➕ Thêm", 280, Color.FromArgb(0, 123, 255), BtnAdd_Click);
            AddToolbarButton(pnlToolbar, "🗑️ Xóa", 400, Color.FromArgb(220, 53, 69), BtnDelete_Click);
            AddToolbarButton(pnlToolbar, "🔄 Refresh", 520, Color.FromArgb(23, 162, 184), (s, e) => LoadData());

            this.Controls.Add(pnlToolbar);

            // Main Content
            SplitContainer split = new SplitContainer { Dock = DockStyle.Fill, Orientation = Orientation.Vertical, SplitterDistance = 450 };

            // Form Panel
            Panel pnlForm = new Panel { Dock = DockStyle.Fill, Padding = new Padding(15), BackColor = Color.FromArgb(250, 250, 250), AutoScroll = true };

            int y = 10;
            AddFormField(pnlForm, "Mã Giảng Viên:", ref y, out txtMaGV);
            AddFormField(pnlForm, "Họ và Tên:", ref y, out txtTenGV);

            // Ngày sinh
            pnlForm.Controls.Add(new Label { Text = "Ngày Sinh:", Location = new Point(15, y), AutoSize = true, Font = new Font("Segoe UI", 10) });
            dtpNgaySinh = new DateTimePicker { Location = new Point(15, y + 22), Size = new Size(200, 28), Font = new Font("Segoe UI", 10), Format = DateTimePickerFormat.Short };
            pnlForm.Controls.Add(dtpNgaySinh);
            y += 60;

            AddFormField(pnlForm, "Email:", ref y, out txtEmail);
            AddFormField(pnlForm, "Số điện thoại:", ref y, out txtSDT);
            AddFormField(pnlForm, "Địa chỉ:", ref y, out txtDiaChi);

            // Khoa
            pnlForm.Controls.Add(new Label { Text = "Khoa:", Location = new Point(15, y), AutoSize = true, Font = new Font("Segoe UI", 10) });
            cboKhoa = new ComboBox { Location = new Point(15, y + 22), Size = new Size(380, 28), Font = new Font("Segoe UI", 10), DropDownStyle = ComboBoxStyle.DropDownList };
            cboKhoa.SelectedIndexChanged += CboKhoa_SelectedIndexChanged;
            pnlForm.Controls.Add(cboKhoa);
            y += 60;

            // Bộ môn
            pnlForm.Controls.Add(new Label { Text = "Bộ Môn:", Location = new Point(15, y), AutoSize = true, Font = new Font("Segoe UI", 10) });
            cboBoMon = new ComboBox { Location = new Point(15, y + 22), Size = new Size(380, 28), Font = new Font("Segoe UI", 10), DropDownStyle = ComboBoxStyle.DropDownList };
            pnlForm.Controls.Add(cboBoMon);
            y += 70;

            // Buttons
            var btnSave = new Button { Text = "💾 Lưu", Location = new Point(15, y), Size = new Size(120, 38), BackColor = Color.FromArgb(40, 167, 69), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            btnSave.Click += BtnSave_Click;
            pnlForm.Controls.Add(btnSave);

            var btnClear = new Button { Text = "🔄 Làm mới", Location = new Point(145, y), Size = new Size(120, 38), BackColor = Color.FromArgb(108, 117, 125), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            btnClear.Click += (s, e) => ClearForm();
            pnlForm.Controls.Add(btnClear);

            split.Panel1.Controls.Add(pnlForm);

            // Grid
            dgvGiangVien = CreateDataGridView(Color.FromArgb(52, 152, 219));
            dgvGiangVien.SelectionChanged += DgvGiangVien_SelectionChanged;
            split.Panel2.Controls.Add(dgvGiangVien);

            this.Controls.Add(split);
            this.ResumeLayout(false);
        }

        private void AddToolbarButton(Panel panel, string text, int x, Color bgColor, EventHandler handler)
        {
            var btn = new Button { Text = text, Location = new Point(x, 12), Size = new Size(110, 35), BackColor = bgColor, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10) };
            btn.Click += handler;
            panel.Controls.Add(btn);
        }

        private void AddFormField(Panel panel, string label, ref int y, out TextBox textBox)
        {
            panel.Controls.Add(new Label { Text = label, Location = new Point(15, y), AutoSize = true, Font = new Font("Segoe UI", 10) });
            textBox = new TextBox { Location = new Point(15, y + 22), Size = new Size(380, 28), Font = new Font("Segoe UI", 10) };
            panel.Controls.Add(textBox);
            y += 60;
        }

        private DataGridView CreateDataGridView(Color headerColor)
        {
            var dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false
            };
            dgv.ColumnHeadersDefaultCellStyle.BackColor = headerColor;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgv.ColumnHeadersHeight = 40;
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 9);
            dgv.RowTemplate.Height = 32;
            return dgv;
        }

        private async void LoadData()
        {
            _khoas = await ApiService.GetKhoasAsync();
            _boMons = await ApiService.GetBoMonsAsync();
            _allGiangViens = await ApiService.GetGiangViensAsync();

            cboKhoa.DataSource = null;
            var khoaList = new List<Khoa> { new Khoa { MaKhoa = "", TenKhoa = "-- Chọn Khoa --" } };
            khoaList.AddRange(_khoas);
            cboKhoa.DataSource = khoaList;
            cboKhoa.DisplayMember = "TenKhoa";
            cboKhoa.ValueMember = "MaKhoa";

            DisplayData(_allGiangViens);
        }

        private void CboKhoa_SelectedIndexChanged(object? sender, EventArgs e)
        {
            var maKhoa = cboKhoa.SelectedValue?.ToString();
            var filtered = string.IsNullOrEmpty(maKhoa) ? new List<BoMon>() : _boMons.Where(b => b.MaKhoa == maKhoa).ToList();

            cboBoMon.DataSource = null;
            var bmList = new List<BoMon> { new BoMon { MaBM = "", TenBM = "-- Chọn Bộ Môn --" } };
            bmList.AddRange(filtered);
            cboBoMon.DataSource = bmList;
            cboBoMon.DisplayMember = "TenBM";
            cboBoMon.ValueMember = "MaBM";
        }

        private void DisplayData(List<GiangVien> data)
        {
            dgvGiangVien.DataSource = null;
            dgvGiangVien.DataSource = data.Select(g => new
            {
                g.MaGV,
                g.TenGV,
                NgaySinh = g.NgaySinh?.ToString("dd/MM/yyyy") ?? "",
                g.Email,
                g.SDT,
                TenKhoa = g.Khoa?.TenKhoa ?? g.MaKhoa,
                TenBM = g.BoMon?.TenBM ?? g.MaBM
            }).ToList();

            if (dgvGiangVien.Columns.Count > 0)
            {
                dgvGiangVien.Columns["MaGV"].HeaderText = "Mã GV";
                dgvGiangVien.Columns["TenGV"].HeaderText = "Họ Tên";
                dgvGiangVien.Columns["NgaySinh"].HeaderText = "Ngày Sinh";
                dgvGiangVien.Columns["Email"].HeaderText = "Email";
                dgvGiangVien.Columns["SDT"].HeaderText = "SĐT";
                dgvGiangVien.Columns["TenKhoa"].HeaderText = "Khoa";
                dgvGiangVien.Columns["TenBM"].HeaderText = "Bộ Môn";
            }
        }

        private void TxtSearch_TextChanged(object? sender, EventArgs e)
        {
            var search = txtSearch.Text.ToLower();
            var filtered = _allGiangViens.Where(g =>
                g.MaGV.ToLower().Contains(search) ||
                g.TenGV.ToLower().Contains(search) ||
                (g.Email?.ToLower().Contains(search) ?? false)
            ).ToList();
            DisplayData(filtered);
        }

        private void DgvGiangVien_SelectionChanged(object? sender, EventArgs e)
        {
            if (dgvGiangVien.CurrentRow != null)
            {
                var maGV = dgvGiangVien.CurrentRow.Cells["MaGV"].Value?.ToString();
                var gv = _allGiangViens.FirstOrDefault(g => g.MaGV == maGV);
                if (gv != null)
                {
                    txtMaGV.Text = gv.MaGV;
                    txtTenGV.Text = gv.TenGV;
                    dtpNgaySinh.Value = gv.NgaySinh ?? DateTime.Now;
                    txtEmail.Text = gv.Email ?? "";
                    txtSDT.Text = gv.SDT ?? "";
                    txtDiaChi.Text = gv.DiaChi ?? "";
                    cboKhoa.SelectedValue = gv.MaKhoa ?? "";
                    cboBoMon.SelectedValue = gv.MaBM ?? "";
                    txtMaGV.Enabled = false;
                }
            }
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            ClearForm();
            txtMaGV.Enabled = true;
            txtMaGV.Focus();
        }

        private async void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (dgvGiangVien.CurrentRow == null) return;

            var maGV = dgvGiangVien.CurrentRow.Cells["MaGV"].Value?.ToString();
            if (string.IsNullOrEmpty(maGV)) return;

            if (MessageBox.Show($"Xóa giảng viên '{maGV}'?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                var (success, message) = await ApiService.DeleteGiangVienAsync(maGV);
                MessageBox.Show(message, success ? "Thành công" : "Lỗi", MessageBoxButtons.OK, success ? MessageBoxIcon.Information : MessageBoxIcon.Error);
                if (success) { LoadData(); ClearForm(); }
            }
        }

        private async void BtnSave_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaGV.Text) || string.IsNullOrWhiteSpace(txtTenGV.Text))
            {
                MessageBox.Show("Vui lòng nhập Mã GV và Họ tên!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var gv = new GiangVien
            {
                MaGV = txtMaGV.Text.Trim().ToUpper(),
                TenGV = txtTenGV.Text.Trim(),
                NgaySinh = dtpNgaySinh.Value,
                Email = txtEmail.Text.Trim(),
                SDT = txtSDT.Text.Trim(),
                DiaChi = txtDiaChi.Text.Trim(),
                MaKhoa = string.IsNullOrEmpty(cboKhoa.SelectedValue?.ToString()) ? null : cboKhoa.SelectedValue?.ToString(),
                MaBM = string.IsNullOrEmpty(cboBoMon.SelectedValue?.ToString()) ? null : cboBoMon.SelectedValue?.ToString()
            };

            bool isNew = txtMaGV.Enabled;
            var (success, message) = isNew
                ? await ApiService.CreateGiangVienAsync(gv)
                : await ApiService.UpdateGiangVienAsync(gv.MaGV, gv);

            MessageBox.Show(message, success ? "Thành công" : "Lỗi", MessageBoxButtons.OK, success ? MessageBoxIcon.Information : MessageBoxIcon.Error);
            if (success) { LoadData(); ClearForm(); }
        }

        private void ClearForm()
        {
            txtMaGV.Text = txtTenGV.Text = txtEmail.Text = txtSDT.Text = txtDiaChi.Text = "";
            txtMaGV.Enabled = true;
            dtpNgaySinh.Value = DateTime.Now;
            if (cboKhoa.Items.Count > 0) cboKhoa.SelectedIndex = 0;
            if (cboBoMon.Items.Count > 0) cboBoMon.SelectedIndex = 0;
            dgvGiangVien.ClearSelection();
        }
    }
}
