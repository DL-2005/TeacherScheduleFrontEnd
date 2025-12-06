using TeacherScheduleFrontend.Models;
using TeacherScheduleFrontend.Services;

namespace TeacherScheduleFrontend.Forms
{
    public partial class FormBoMon : Form
    {
        private DataGridView dgvBoMon = null!;
        private TextBox txtMaBM = null!;
        private TextBox txtTenBM = null!;
        private ComboBox cboKhoa = null!;
        private TextBox txtMoTa = null!;
        private TextBox txtSearch = null!;
        private List<BoMon> _allBoMons = new();
        private List<Khoa> _khoas = new();

        public FormBoMon()
        {
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.Text = "Quản Lý Bộ Môn";
            this.BackColor = Color.White;

            // Header
            Panel pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(46, 204, 113)
            };

            Label lblTitle = new Label
            {
                Text = "📖 QUẢN LÝ BỘ MÔN",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(20, 15)
            };
            pnlHeader.Controls.Add(lblTitle);
            this.Controls.Add(pnlHeader);

            // Toolbar
            Panel pnlToolbar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(245, 245, 245)
            };

            txtSearch = new TextBox
            {
                Location = new Point(15, 15),
                Size = new Size(250, 30),
                Font = new Font("Segoe UI", 11),
                PlaceholderText = "🔍 Tìm kiếm..."
            };
            txtSearch.TextChanged += TxtSearch_TextChanged;
            pnlToolbar.Controls.Add(txtSearch);

            var btnAdd = CreateButton("➕ Thêm", 280, Color.FromArgb(0, 123, 255));
            btnAdd.Click += BtnAdd_Click;
            pnlToolbar.Controls.Add(btnAdd);

            var btnDelete = CreateButton("🗑️ Xóa", 400, Color.FromArgb(220, 53, 69));
            btnDelete.Click += BtnDelete_Click;
            pnlToolbar.Controls.Add(btnDelete);

            var btnRefresh = CreateButton("🔄 Refresh", 520, Color.FromArgb(23, 162, 184));
            btnRefresh.Click += (s, e) => LoadData();
            pnlToolbar.Controls.Add(btnRefresh);

            this.Controls.Add(pnlToolbar);

            // Main Content
            SplitContainer split = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                SplitterDistance = 400
            };

            // Form Panel
            Panel pnlForm = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20), BackColor = Color.FromArgb(250, 250, 250) };

            int y = 20;
            pnlForm.Controls.Add(new Label { Text = "Mã Bộ Môn:", Location = new Point(20, y), AutoSize = true, Font = new Font("Segoe UI", 10) });
            txtMaBM = new TextBox { Location = new Point(20, y + 25), Size = new Size(350, 30), Font = new Font("Segoe UI", 11) };
            pnlForm.Controls.Add(txtMaBM);

            y += 70;
            pnlForm.Controls.Add(new Label { Text = "Tên Bộ Môn:", Location = new Point(20, y), AutoSize = true, Font = new Font("Segoe UI", 10) });
            txtTenBM = new TextBox { Location = new Point(20, y + 25), Size = new Size(350, 30), Font = new Font("Segoe UI", 11) };
            pnlForm.Controls.Add(txtTenBM);

            y += 70;
            pnlForm.Controls.Add(new Label { Text = "Thuộc Khoa:", Location = new Point(20, y), AutoSize = true, Font = new Font("Segoe UI", 10) });
            cboKhoa = new ComboBox { Location = new Point(20, y + 25), Size = new Size(350, 30), Font = new Font("Segoe UI", 11), DropDownStyle = ComboBoxStyle.DropDownList };
            pnlForm.Controls.Add(cboKhoa);

            y += 70;
            pnlForm.Controls.Add(new Label { Text = "Mô tả:", Location = new Point(20, y), AutoSize = true, Font = new Font("Segoe UI", 10) });
            txtMoTa = new TextBox { Location = new Point(20, y + 25), Size = new Size(350, 60), Font = new Font("Segoe UI", 11), Multiline = true };
            pnlForm.Controls.Add(txtMoTa);

            y += 100;
            var btnSave = new Button
            {
                Text = "💾 Lưu",
                Location = new Point(20, y),
                Size = new Size(120, 40),
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };
            btnSave.Click += BtnSave_Click;
            pnlForm.Controls.Add(btnSave);

            var btnClear = new Button
            {
                Text = "🔄 Làm mới",
                Location = new Point(150, y),
                Size = new Size(120, 40),
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };
            btnClear.Click += (s, e) => ClearForm();
            pnlForm.Controls.Add(btnClear);

            split.Panel1.Controls.Add(pnlForm);

            // Grid Panel
            dgvBoMon = new DataGridView
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
            dgvBoMon.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(46, 204, 113);
            dgvBoMon.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvBoMon.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvBoMon.ColumnHeadersHeight = 40;
            dgvBoMon.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvBoMon.RowTemplate.Height = 35;
            dgvBoMon.SelectionChanged += DgvBoMon_SelectionChanged;
            split.Panel2.Controls.Add(dgvBoMon);

            this.Controls.Add(split);
            this.ResumeLayout(false);
        }

        private Button CreateButton(string text, int x, Color bgColor)
        {
            return new Button
            {
                Text = text,
                Location = new Point(x, 12),
                Size = new Size(110, 35),
                BackColor = bgColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10)
            };
        }

        private async void LoadData()
        {
            _khoas = await ApiService.GetKhoasAsync();
            _allBoMons = await ApiService.GetBoMonsAsync();

            cboKhoa.DataSource = null;
            cboKhoa.DataSource = _khoas;
            cboKhoa.DisplayMember = "TenKhoa";
            cboKhoa.ValueMember = "MaKhoa";

            DisplayData(_allBoMons);
        }

        private void DisplayData(List<BoMon> data)
        {
            dgvBoMon.DataSource = null;
            dgvBoMon.DataSource = data.Select(b => new
            {
                b.MaBM,
                b.TenBM,
                TenKhoa = b.Khoa?.TenKhoa ?? b.MaKhoa,
                b.MoTa
            }).ToList();

            if (dgvBoMon.Columns.Count > 0)
            {
                dgvBoMon.Columns["MaBM"].HeaderText = "Mã BM";
                dgvBoMon.Columns["TenBM"].HeaderText = "Tên Bộ Môn";
                dgvBoMon.Columns["TenKhoa"].HeaderText = "Khoa";
                dgvBoMon.Columns["MoTa"].HeaderText = "Mô Tả";
            }
        }

        private void TxtSearch_TextChanged(object? sender, EventArgs e)
        {
            var search = txtSearch.Text.ToLower();
            var filtered = _allBoMons.Where(b =>
                b.MaBM.ToLower().Contains(search) ||
                b.TenBM.ToLower().Contains(search)
            ).ToList();
            DisplayData(filtered);
        }

        private void DgvBoMon_SelectionChanged(object? sender, EventArgs e)
        {
            if (dgvBoMon.CurrentRow != null)
            {
                var maBM = dgvBoMon.CurrentRow.Cells["MaBM"].Value?.ToString();
                var bm = _allBoMons.FirstOrDefault(b => b.MaBM == maBM);
                if (bm != null)
                {
                    txtMaBM.Text = bm.MaBM;
                    txtTenBM.Text = bm.TenBM;
                    cboKhoa.SelectedValue = bm.MaKhoa ?? "";
                    txtMoTa.Text = bm.MoTa ?? "";
                    txtMaBM.Enabled = false;
                }
            }
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            ClearForm();
            txtMaBM.Enabled = true;
            txtMaBM.Focus();
        }

        private async void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (dgvBoMon.CurrentRow == null) return;

            var maBM = dgvBoMon.CurrentRow.Cells["MaBM"].Value?.ToString();
            if (string.IsNullOrEmpty(maBM)) return;

            if (MessageBox.Show($"Xóa bộ môn '{maBM}'?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                var (success, message) = await ApiService.DeleteBoMonAsync(maBM);
                MessageBox.Show(message, success ? "Thành công" : "Lỗi", MessageBoxButtons.OK, success ? MessageBoxIcon.Information : MessageBoxIcon.Error);
                if (success) { LoadData(); ClearForm(); }
            }
        }

        private async void BtnSave_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaBM.Text) || string.IsNullOrWhiteSpace(txtTenBM.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var boMon = new BoMon
            {
                MaBM = txtMaBM.Text.Trim().ToUpper(),
                TenBM = txtTenBM.Text.Trim(),
                MaKhoa = cboKhoa.SelectedValue?.ToString(),
                MoTa = txtMoTa.Text.Trim()
            };

            bool isNew = txtMaBM.Enabled;
            var (success, message) = isNew
                ? await ApiService.CreateBoMonAsync(boMon)
                : await ApiService.UpdateBoMonAsync(boMon.MaBM, boMon);

            MessageBox.Show(message, success ? "Thành công" : "Lỗi", MessageBoxButtons.OK, success ? MessageBoxIcon.Information : MessageBoxIcon.Error);
            if (success) { LoadData(); ClearForm(); }
        }

        private void ClearForm()
        {
            txtMaBM.Text = "";
            txtTenBM.Text = "";
            txtMoTa.Text = "";
            txtMaBM.Enabled = true;
            if (cboKhoa.Items.Count > 0) cboKhoa.SelectedIndex = 0;
            dgvBoMon.ClearSelection();
        }
    }
}
