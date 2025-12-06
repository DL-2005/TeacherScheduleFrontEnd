using TeacherScheduleFrontend.Models;
using TeacherScheduleFrontend.Services;

namespace TeacherScheduleFrontend.Forms
{
    public partial class FormMonHoc : Form
    {
        private DataGridView dgvMonHoc = null!;
        private TextBox txtMaMH = null!, txtTenMH = null!, txtSearch = null!;
        private NumericUpDown nudSoTinChi = null!;
        private ComboBox cboHeDaoTao = null!;
        private List<MonHoc> _allMonHocs = new();

        public FormMonHoc()
        {
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.Text = "Quản Lý Môn Học";
            this.BackColor = Color.White;

            // Header
            Panel pnlHeader = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.FromArgb(155, 89, 182) };
            pnlHeader.Controls.Add(new Label { Text = "📕 QUẢN LÝ MÔN HỌC", Font = new Font("Segoe UI", 18, FontStyle.Bold), ForeColor = Color.White, AutoSize = true, Location = new Point(20, 15) });
            this.Controls.Add(pnlHeader);

            // Toolbar
            Panel pnlToolbar = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.FromArgb(245, 245, 245) };

            txtSearch = new TextBox { Location = new Point(15, 15), Size = new Size(250, 30), Font = new Font("Segoe UI", 11), PlaceholderText = "🔍 Tìm kiếm môn học..." };
            txtSearch.TextChanged += TxtSearch_TextChanged;
            pnlToolbar.Controls.Add(txtSearch);

            AddButton(pnlToolbar, "➕ Thêm", 280, Color.FromArgb(0, 123, 255), BtnAdd_Click);
            AddButton(pnlToolbar, "🗑️ Xóa", 400, Color.FromArgb(220, 53, 69), BtnDelete_Click);
            AddButton(pnlToolbar, "🔄 Refresh", 520, Color.FromArgb(23, 162, 184), (s, e) => LoadData());

            this.Controls.Add(pnlToolbar);

            // Main Content
            SplitContainer split = new SplitContainer { Dock = DockStyle.Fill, Orientation = Orientation.Vertical, SplitterDistance = 400 };

            // Form
            Panel pnlForm = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20), BackColor = Color.FromArgb(250, 250, 250) };
            int y = 20;

            AddField(pnlForm, "Mã Môn Học:", ref y, out txtMaMH);
            AddField(pnlForm, "Tên Môn Học:", ref y, out txtTenMH);

            pnlForm.Controls.Add(new Label { Text = "Số Tín Chỉ:", Location = new Point(20, y), AutoSize = true, Font = new Font("Segoe UI", 10) });
            nudSoTinChi = new NumericUpDown { Location = new Point(20, y + 25), Size = new Size(100, 30), Font = new Font("Segoe UI", 11), Minimum = 1, Maximum = 10, Value = 3 };
            pnlForm.Controls.Add(nudSoTinChi);
            y += 70;

            pnlForm.Controls.Add(new Label { Text = "Hệ Đào Tạo:", Location = new Point(20, y), AutoSize = true, Font = new Font("Segoe UI", 10) });
            cboHeDaoTao = new ComboBox { Location = new Point(20, y + 25), Size = new Size(350, 30), Font = new Font("Segoe UI", 11), DropDownStyle = ComboBoxStyle.DropDownList };
            cboHeDaoTao.Items.AddRange(new[] { "Đại học chính quy", "Cao đẳng", "Vừa làm vừa học", "Liên thông", "Khác" });
            cboHeDaoTao.SelectedIndex = 0;
            pnlForm.Controls.Add(cboHeDaoTao);
            y += 80;

            var btnSave = new Button { Text = "💾 Lưu", Location = new Point(20, y), Size = new Size(120, 40), BackColor = Color.FromArgb(40, 167, 69), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 11, FontStyle.Bold) };
            btnSave.Click += BtnSave_Click;
            pnlForm.Controls.Add(btnSave);

            var btnClear = new Button { Text = "🔄 Làm mới", Location = new Point(150, y), Size = new Size(120, 40), BackColor = Color.FromArgb(108, 117, 125), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 11, FontStyle.Bold) };
            btnClear.Click += (s, e) => ClearForm();
            pnlForm.Controls.Add(btnClear);

            split.Panel1.Controls.Add(pnlForm);

            // Grid
            dgvMonHoc = CreateGrid(Color.FromArgb(155, 89, 182));
            dgvMonHoc.SelectionChanged += DgvMonHoc_SelectionChanged;
            split.Panel2.Controls.Add(dgvMonHoc);

            this.Controls.Add(split);
            this.ResumeLayout(false);
        }

        private void AddButton(Panel panel, string text, int x, Color bg, EventHandler handler)
        {
            var btn = new Button { Text = text, Location = new Point(x, 12), Size = new Size(110, 35), BackColor = bg, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10) };
            btn.Click += handler;
            panel.Controls.Add(btn);
        }

        private void AddField(Panel panel, string label, ref int y, out TextBox txt)
        {
            panel.Controls.Add(new Label { Text = label, Location = new Point(20, y), AutoSize = true, Font = new Font("Segoe UI", 10) });
            txt = new TextBox { Location = new Point(20, y + 25), Size = new Size(350, 30), Font = new Font("Segoe UI", 11) };
            panel.Controls.Add(txt);
            y += 70;
        }

        private DataGridView CreateGrid(Color headerColor)
        {
            var dgv = new DataGridView
            {
                Dock = DockStyle.Fill, BackgroundColor = Color.White, BorderStyle = BorderStyle.None,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect, MultiSelect = false, ReadOnly = true,
                AllowUserToAddRows = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, RowHeadersVisible = false
            };
            dgv.ColumnHeadersDefaultCellStyle.BackColor = headerColor;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgv.ColumnHeadersHeight = 40;
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgv.RowTemplate.Height = 35;
            return dgv;
        }

        private async void LoadData()
        {
            _allMonHocs = await ApiService.GetMonHocsAsync();
            DisplayData(_allMonHocs);
        }

        private void DisplayData(List<MonHoc> data)
        {
            dgvMonHoc.DataSource = null;
            dgvMonHoc.DataSource = data.Select(m => new { m.MaMH, m.TenMH, m.SoTinChi, m.HeDaoTao }).ToList();
            if (dgvMonHoc.Columns.Count > 0)
            {
                dgvMonHoc.Columns["MaMH"].HeaderText = "Mã MH";
                dgvMonHoc.Columns["TenMH"].HeaderText = "Tên Môn Học";
                dgvMonHoc.Columns["SoTinChi"].HeaderText = "Số TC";
                dgvMonHoc.Columns["HeDaoTao"].HeaderText = "Hệ Đào Tạo";
            }
        }

        private void TxtSearch_TextChanged(object? sender, EventArgs e)
        {
            var s = txtSearch.Text.ToLower();
            DisplayData(_allMonHocs.Where(m => m.MaMH.ToLower().Contains(s) || m.TenMH.ToLower().Contains(s)).ToList());
        }

        private void DgvMonHoc_SelectionChanged(object? sender, EventArgs e)
        {
            if (dgvMonHoc.CurrentRow != null)
            {
                var maMH = dgvMonHoc.CurrentRow.Cells["MaMH"].Value?.ToString();
                var mh = _allMonHocs.FirstOrDefault(m => m.MaMH == maMH);
                if (mh != null)
                {
                    txtMaMH.Text = mh.MaMH;
                    txtTenMH.Text = mh.TenMH;
                    nudSoTinChi.Value = mh.SoTinChi;
                    cboHeDaoTao.SelectedItem = mh.HeDaoTao ?? "Đại học chính quy";
                    txtMaMH.Enabled = false;
                }
            }
        }

        private void BtnAdd_Click(object? sender, EventArgs e) { ClearForm(); txtMaMH.Enabled = true; txtMaMH.Focus(); }

        private async void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (dgvMonHoc.CurrentRow == null) return;
            var maMH = dgvMonHoc.CurrentRow.Cells["MaMH"].Value?.ToString();
            if (string.IsNullOrEmpty(maMH)) return;

            if (MessageBox.Show($"Xóa môn học '{maMH}'?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                var (success, message) = await ApiService.DeleteMonHocAsync(maMH);
                MessageBox.Show(message, success ? "Thành công" : "Lỗi", MessageBoxButtons.OK, success ? MessageBoxIcon.Information : MessageBoxIcon.Error);
                if (success) { LoadData(); ClearForm(); }
            }
        }

        private async void BtnSave_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaMH.Text) || string.IsNullOrWhiteSpace(txtTenMH.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var mh = new MonHoc
            {
                MaMH = txtMaMH.Text.Trim().ToUpper(),
                TenMH = txtTenMH.Text.Trim(),
                SoTinChi = (int)nudSoTinChi.Value,
                HeDaoTao = cboHeDaoTao.SelectedItem?.ToString()
            };

            bool isNew = txtMaMH.Enabled;
            var (success, message) = isNew ? await ApiService.CreateMonHocAsync(mh) : await ApiService.UpdateMonHocAsync(mh.MaMH, mh);
            MessageBox.Show(message, success ? "Thành công" : "Lỗi", MessageBoxButtons.OK, success ? MessageBoxIcon.Information : MessageBoxIcon.Error);
            if (success) { LoadData(); ClearForm(); }
        }

        private void ClearForm()
        {
            txtMaMH.Text = txtTenMH.Text = "";
            txtMaMH.Enabled = true;
            nudSoTinChi.Value = 3;
            cboHeDaoTao.SelectedIndex = 0;
            dgvMonHoc.ClearSelection();
        }
    }
}
