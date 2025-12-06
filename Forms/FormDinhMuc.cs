using TeacherScheduleFrontend.Models;
using TeacherScheduleFrontend.Services;

namespace TeacherScheduleFrontend.Forms
{
    public partial class FormDinhMuc : Form
    {
        private DataGridView dgvDinhMuc = null!;
        private TextBox txtMaDM = null!, txtMoTa = null!, txtSearch = null!;
        private NumericUpDown nudGioChuan = null!, nudGioToiThieu = null!, nudGioToiDa = null!;
        private ComboBox cboChucVu = null!;
        private List<DinhMuc> _allDinhMucs = new();
        private int _currentId = 0;

        public FormDinhMuc()
        {
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.Text = "Quản Lý Định Mức";
            this.BackColor = Color.White;

            // Header
            Panel pnlHeader = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.FromArgb(22, 160, 133) };
            pnlHeader.Controls.Add(new Label
            {
                Text = "⚙️ QUẢN LÝ ĐỊNH MỨC GIỜ GIẢNG",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(20, 15)
            });
            this.Controls.Add(pnlHeader);

            // Toolbar
            Panel pnlToolbar = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.FromArgb(245, 245, 245) };

            txtSearch = new TextBox
            {
                Location = new Point(15, 15),
                Size = new Size(200, 30),
                Font = new Font("Segoe UI", 11),
                PlaceholderText = "🔍 Tìm kiếm..."
            };
            txtSearch.TextChanged += TxtSearch_TextChanged;
            pnlToolbar.Controls.Add(txtSearch);

            AddButton(pnlToolbar, "➕ Thêm", 230, Color.FromArgb(0, 123, 255), BtnAdd_Click);
            AddButton(pnlToolbar, "💾 Lưu", 350, Color.FromArgb(40, 167, 69), BtnSave_Click);
            AddButton(pnlToolbar, "🗑️ Xóa", 470, Color.FromArgb(220, 53, 69), BtnDelete_Click);
            AddButton(pnlToolbar, "🔄 Refresh", 590, Color.FromArgb(23, 162, 184), (s, e) => LoadData());

            this.Controls.Add(pnlToolbar);

            // Main
            SplitContainer split = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                SplitterDistance = 400
            };

            // Form Panel
            Panel pnlForm = new Panel { Dock = DockStyle.Fill, Padding = new Padding(15), BackColor = Color.FromArgb(250, 250, 250) };
            int y = 10;

            AddLabel(pnlForm, "Mã định mức:", y);
            txtMaDM = new TextBox { Location = new Point(15, y + 22), Size = new Size(350, 28), Font = new Font("Segoe UI", 10) };
            pnlForm.Controls.Add(txtMaDM);
            y += 55;

            AddLabel(pnlForm, "Chức vụ áp dụng:", y);
            cboChucVu = new ComboBox
            {
                Location = new Point(15, y + 22),
                Size = new Size(350, 28),
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboChucVu.Items.AddRange(new object[] { "GV - Giảng viên", "TBM - Trưởng bộ môn", "TK - Trưởng khoa", "CQC - Cán bộ quản lý" });
            cboChucVu.SelectedIndex = 0;
            pnlForm.Controls.Add(cboChucVu);
            y += 55;

            AddLabel(pnlForm, "Giờ chuẩn:", y);
            nudGioChuan = new NumericUpDown
            {
                Location = new Point(15, y + 22),
                Size = new Size(350, 28),
                Font = new Font("Segoe UI", 10),
                Minimum = 0,
                Maximum = 1000,
                Value = 280
            };
            pnlForm.Controls.Add(nudGioChuan);
            y += 55;

            AddLabel(pnlForm, "Giờ tối thiểu:", y);
            nudGioToiThieu = new NumericUpDown
            {
                Location = new Point(15, y + 22),
                Size = new Size(350, 28),
                Font = new Font("Segoe UI", 10),
                Minimum = 0,
                Maximum = 1000,
                Value = 200
            };
            pnlForm.Controls.Add(nudGioToiThieu);
            y += 55;

            AddLabel(pnlForm, "Giờ tối đa:", y);
            nudGioToiDa = new NumericUpDown
            {
                Location = new Point(15, y + 22),
                Size = new Size(350, 28),
                Font = new Font("Segoe UI", 10),
                Minimum = 0,
                Maximum = 2000,
                Value = 500
            };
            pnlForm.Controls.Add(nudGioToiDa);
            y += 55;

            AddLabel(pnlForm, "Mô tả:", y);
            txtMoTa = new TextBox
            {
                Location = new Point(15, y + 22),
                Size = new Size(350, 60),
                Font = new Font("Segoe UI", 10),
                Multiline = true
            };
            pnlForm.Controls.Add(txtMoTa);

            split.Panel1.Controls.Add(pnlForm);

            // Grid
            dgvDinhMuc = CreateGrid(Color.FromArgb(22, 160, 133));
            dgvDinhMuc.SelectionChanged += DgvDinhMuc_SelectionChanged;
            split.Panel2.Controls.Add(dgvDinhMuc);

            this.Controls.Add(split);
            this.ResumeLayout(false);
        }

        private void AddButton(Panel p, string text, int x, Color bg, EventHandler handler)
        {
            var btn = new Button
            {
                Text = text,
                Location = new Point(x, 12),
                Size = new Size(110, 35),
                BackColor = bg,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10)
            };
            btn.Click += handler;
            p.Controls.Add(btn);
        }

        private void AddLabel(Panel p, string text, int y)
        {
            p.Controls.Add(new Label
            {
                Text = text,
                Location = new Point(15, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 10)
            });
        }

        private DataGridView CreateGrid(Color headerColor)
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
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgv.RowTemplate.Height = 35;
            return dgv;
        }

        private async void LoadData()
        {
            _allDinhMucs = await ApiService.GetDinhMucsAsync();
            DisplayData(_allDinhMucs);
        }

        private void DisplayData(List<DinhMuc> data)
        {
            dgvDinhMuc.DataSource = null;
            dgvDinhMuc.DataSource = data.Select(dm => new
            {
                dm.Id,
                dm.MaDM,
                ChucVu = GetChucVuName(dm.ChucVu),
                dm.GioChuan,
                dm.GioToiThieu,
                dm.GioToiDa,
                dm.MoTa
            }).ToList();

            if (dgvDinhMuc.Columns.Count > 0)
            {
                dgvDinhMuc.Columns["Id"].Visible = false;
                dgvDinhMuc.Columns["MaDM"].HeaderText = "Mã ĐM";
                dgvDinhMuc.Columns["ChucVu"].HeaderText = "Chức Vụ";
                dgvDinhMuc.Columns["GioChuan"].HeaderText = "Giờ Chuẩn";
                dgvDinhMuc.Columns["GioToiThieu"].HeaderText = "Giờ Min";
                dgvDinhMuc.Columns["GioToiDa"].HeaderText = "Giờ Max";
                dgvDinhMuc.Columns["MoTa"].HeaderText = "Mô Tả";
            }
        }

        private string GetChucVuName(string cv) => cv switch
        {
            "GV" => "Giảng viên",
            "TBM" => "Trưởng BM",
            "TK" => "Trưởng khoa",
            "CQC" => "Cán bộ QL",
            _ => cv
        };

        private void DgvDinhMuc_SelectionChanged(object? sender, EventArgs e)
        {
            if (dgvDinhMuc.CurrentRow == null) return;

            var id = Convert.ToInt32(dgvDinhMuc.CurrentRow.Cells["Id"].Value);
            var dm = _allDinhMucs.FirstOrDefault(x => x.Id == id);
            if (dm != null)
            {
                _currentId = dm.Id;
                txtMaDM.Text = dm.MaDM;
                txtMaDM.Enabled = false;

                for (int i = 0; i < cboChucVu.Items.Count; i++)
                {
                    if (cboChucVu.Items[i].ToString()?.StartsWith(dm.ChucVu) == true)
                    {
                        cboChucVu.SelectedIndex = i;
                        break;
                    }
                }

                nudGioChuan.Value = dm.GioChuan;
                nudGioToiThieu.Value = dm.GioToiThieu;
                nudGioToiDa.Value = dm.GioToiDa;
                txtMoTa.Text = dm.MoTa;
            }
        }

        private void TxtSearch_TextChanged(object? sender, EventArgs e)
        {
            var search = txtSearch.Text.ToLower();
            var filtered = _allDinhMucs.Where(dm =>
                dm.MaDM.ToLower().Contains(search) ||
                dm.ChucVu.ToLower().Contains(search)
            ).ToList();
            DisplayData(filtered);
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            ClearForm();
            txtMaDM.Focus();
        }

        private async void BtnSave_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaDM.Text))
            {
                MessageBox.Show("Vui lòng nhập mã định mức!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var chucVuStr = cboChucVu.SelectedItem?.ToString() ?? "";
            var chucVu = chucVuStr.Split('-')[0].Trim();

            var dm = new DinhMuc
            {
                Id = _currentId,
                MaDM = txtMaDM.Text.Trim().ToUpper(),
                ChucVu = chucVu,
                GioChuan = (int)nudGioChuan.Value,
                GioToiThieu = (int)nudGioToiThieu.Value,
                GioToiDa = (int)nudGioToiDa.Value,
                MoTa = txtMoTa.Text.Trim()
            };

            bool success;
            string message;

            if (_currentId == 0)
            {
                (success, message) = await ApiService.CreateDinhMucAsync(dm);
                if (_currentId == 0)
                {
                    // ĐÃ SỬA TỪ: (success, message, _) = ...
                    (success, message) = await ApiService.CreateDinhMucAsync(dm);
                }
            }
            else
            {
                (success, message) = await ApiService.UpdateDinhMucAsync(_currentId, dm);
            }

            MessageBox.Show(message, success ? "Thành công" : "Lỗi", MessageBoxButtons.OK,
                success ? MessageBoxIcon.Information : MessageBoxIcon.Error);

            if (success)
            {
                LoadData();
                ClearForm();
            }
        }

        private async void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (_currentId == 0)
            {
                MessageBox.Show("Vui lòng chọn định mức cần xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show($"Xác nhận xóa định mức '{txtMaDM.Text}'?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                var (success, message) = await ApiService.DeleteDinhMucAsync(_currentId);
                MessageBox.Show(message, success ? "Thành công" : "Lỗi", MessageBoxButtons.OK,
                    success ? MessageBoxIcon.Information : MessageBoxIcon.Error);

                if (success)
                {
                    LoadData();
                    ClearForm();
                }
            }
        }

        private void ClearForm()
        {
            _currentId = 0;
            txtMaDM.Text = "";
            txtMaDM.Enabled = true;
            cboChucVu.SelectedIndex = 0;
            nudGioChuan.Value = 280;
            nudGioToiThieu.Value = 200;
            nudGioToiDa.Value = 500;
            txtMoTa.Text = "";
            dgvDinhMuc.ClearSelection();
        }
    }
}
