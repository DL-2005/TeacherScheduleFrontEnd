using TeacherScheduleFrontend.Models;
using TeacherScheduleFrontend.Services;

namespace TeacherScheduleFrontend.Forms
{
    public partial class FormLop : Form
    {
        private DataGridView dgvLop = null!;
        private TextBox txtMaLop = null!, txtNganh = null!, txtNamHoc = null!, txtSearch = null!;
        private NumericUpDown nudSiSo = null!;
        private ComboBox cboKhoa = null!;
        private List<Lop> _allLops = new();
        private List<Khoa> _khoas = new();

        public FormLop()
        {
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.Text = "Quản Lý Lớp Học";
            this.BackColor = Color.White;

            // Header
            Panel pnlHeader = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.FromArgb(230, 126, 34) };
            pnlHeader.Controls.Add(new Label { Text = "🎓 QUẢN LÝ LỚP HỌC", Font = new Font("Segoe UI", 18, FontStyle.Bold), ForeColor = Color.White, AutoSize = true, Location = new Point(20, 15) });
            this.Controls.Add(pnlHeader);

            // Toolbar
            Panel pnlToolbar = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.FromArgb(245, 245, 245) };

            txtSearch = new TextBox { Location = new Point(15, 15), Size = new Size(250, 30), Font = new Font("Segoe UI", 11), PlaceholderText = "🔍 Tìm kiếm lớp..." };
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

            AddField(pnlForm, "Mã Lớp:", ref y, out txtMaLop);
            AddField(pnlForm, "Ngành:", ref y, out txtNganh);
            AddField(pnlForm, "Năm Học:", ref y, out txtNamHoc);

            pnlForm.Controls.Add(new Label { Text = "Sĩ Số:", Location = new Point(20, y), AutoSize = true, Font = new Font("Segoe UI", 10) });
            nudSiSo = new NumericUpDown { Location = new Point(20, y + 25), Size = new Size(100, 30), Font = new Font("Segoe UI", 11), Minimum = 1, Maximum = 200, Value = 40 };
            pnlForm.Controls.Add(nudSiSo);
            y += 70;

            pnlForm.Controls.Add(new Label { Text = "Khoa:", Location = new Point(20, y), AutoSize = true, Font = new Font("Segoe UI", 10) });
            cboKhoa = new ComboBox { Location = new Point(20, y + 25), Size = new Size(350, 30), Font = new Font("Segoe UI", 11), DropDownStyle = ComboBoxStyle.DropDownList };
            pnlForm.Controls.Add(cboKhoa);
            y += 80;

            var btnSave = new Button { Text = "💾 Lưu", Location = new Point(20, y), Size = new Size(120, 40), BackColor = Color.FromArgb(40, 167, 69), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 11, FontStyle.Bold) };
            btnSave.Click += BtnSave_Click;
            pnlForm.Controls.Add(btnSave);

            var btnClear = new Button { Text = "🔄 Làm mới", Location = new Point(150, y), Size = new Size(120, 40), BackColor = Color.FromArgb(108, 117, 125), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 11, FontStyle.Bold) };
            btnClear.Click += (s, e) => ClearForm();
            pnlForm.Controls.Add(btnClear);

            split.Panel1.Controls.Add(pnlForm);

            // Grid
            dgvLop = CreateGrid(Color.FromArgb(230, 126, 34));
            dgvLop.SelectionChanged += DgvLop_SelectionChanged;
            split.Panel2.Controls.Add(dgvLop);

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
            _khoas = await ApiService.GetKhoasAsync();
            _allLops = await ApiService.GetLopsAsync();

            cboKhoa.DataSource = null;
            var list = new List<Khoa> { new Khoa { MaKhoa = "", TenKhoa = "-- Chọn Khoa --" } };
            list.AddRange(_khoas);
            cboKhoa.DataSource = list;
            cboKhoa.DisplayMember = "TenKhoa";
            cboKhoa.ValueMember = "MaKhoa";

            DisplayData(_allLops);
        }

        private void DisplayData(List<Lop> data)
        {
            dgvLop.DataSource = null;
            dgvLop.DataSource = data.Select(l => new { l.MaLop, l.Nganh, l.SiSo, l.NamHoc, TenKhoa = l.Khoa?.TenKhoa ?? l.MaKhoa }).ToList();
            if (dgvLop.Columns.Count > 0)
            {
                dgvLop.Columns["MaLop"].HeaderText = "Mã Lớp";
                dgvLop.Columns["Nganh"].HeaderText = "Ngành";
                dgvLop.Columns["SiSo"].HeaderText = "Sĩ Số";
                dgvLop.Columns["NamHoc"].HeaderText = "Năm Học";
                dgvLop.Columns["TenKhoa"].HeaderText = "Khoa";
            }
        }

        private void TxtSearch_TextChanged(object? sender, EventArgs e)
        {
            var s = txtSearch.Text.ToLower();
            DisplayData(_allLops.Where(l => l.MaLop.ToLower().Contains(s) || (l.Nganh?.ToLower().Contains(s) ?? false)).ToList());
        }

        private void DgvLop_SelectionChanged(object? sender, EventArgs e)
        {
            if (dgvLop.CurrentRow != null)
            {
                var maLop = dgvLop.CurrentRow.Cells["MaLop"].Value?.ToString();
                var lop = _allLops.FirstOrDefault(l => l.MaLop == maLop);
                if (lop != null)
                {
                    txtMaLop.Text = lop.MaLop;
                    txtNganh.Text = lop.Nganh ?? "";
                    txtNamHoc.Text = lop.NamHoc ?? "";
                    nudSiSo.Value = lop.SiSo;
                    cboKhoa.SelectedValue = lop.MaKhoa ?? "";
                    txtMaLop.Enabled = false;
                }
            }
        }

        private void BtnAdd_Click(object? sender, EventArgs e) { ClearForm(); txtMaLop.Enabled = true; txtMaLop.Focus(); }

        private async void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (dgvLop.CurrentRow == null) return;
            var maLop = dgvLop.CurrentRow.Cells["MaLop"].Value?.ToString();
            if (string.IsNullOrEmpty(maLop)) return;

            if (MessageBox.Show($"Xóa lớp '{maLop}'?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                var (success, message) = await ApiService.DeleteLopAsync(maLop);
                MessageBox.Show(message, success ? "Thành công" : "Lỗi", MessageBoxButtons.OK, success ? MessageBoxIcon.Information : MessageBoxIcon.Error);
                if (success) { LoadData(); ClearForm(); }
            }
        }

        private async void BtnSave_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaLop.Text))
            {
                MessageBox.Show("Vui lòng nhập Mã Lớp!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var lop = new Lop
            {
                MaLop = txtMaLop.Text.Trim().ToUpper(),
                Nganh = txtNganh.Text.Trim(),
                NamHoc = txtNamHoc.Text.Trim(),
                SiSo = (int)nudSiSo.Value,
                MaKhoa = string.IsNullOrEmpty(cboKhoa.SelectedValue?.ToString()) ? null : cboKhoa.SelectedValue?.ToString()
            };

            bool isNew = txtMaLop.Enabled;
            var (success, message) = isNew ? await ApiService.CreateLopAsync(lop) : await ApiService.UpdateLopAsync(lop.MaLop, lop);
            MessageBox.Show(message, success ? "Thành công" : "Lỗi", MessageBoxButtons.OK, success ? MessageBoxIcon.Information : MessageBoxIcon.Error);
            if (success) { LoadData(); ClearForm(); }
        }

        private void ClearForm()
        {
            txtMaLop.Text = txtNganh.Text = txtNamHoc.Text = "";
            txtMaLop.Enabled = true;
            nudSiSo.Value = 40;
            if (cboKhoa.Items.Count > 0) cboKhoa.SelectedIndex = 0;
            dgvLop.ClearSelection();
        }
    }
}
