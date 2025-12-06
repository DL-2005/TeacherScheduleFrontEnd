using TeacherScheduleFrontend.Models;
using TeacherScheduleFrontend.Services;

namespace TeacherScheduleFrontend.Forms
{
    public partial class FormKhoa : Form
    {
        private DataGridView dgvKhoa = null!;
        private TextBox txtMaKhoa = null!;
        private TextBox txtTenKhoa = null!;
        private TextBox txtEmail = null!;
        private TextBox txtDienThoai = null!;
        private TextBox txtSearch = null!;
        private List<Khoa> _allKhoas = new();

        public FormKhoa()
        {
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.Text = "Quản Lý Khoa";
            this.BackColor = Color.White;

            // ==================== HEADER ====================
            Panel pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(0, 102, 204),
                Padding = new Padding(20, 15, 20, 15)
            };

            Label lblTitle = new Label
            {
                Text = "🏫 QUẢN LÝ KHOA",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(20, 15)
            };
            pnlHeader.Controls.Add(lblTitle);
            this.Controls.Add(pnlHeader);

            // ==================== TOOLBAR ====================
            Panel pnlToolbar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(245, 245, 245),
                Padding = new Padding(10)
            };

            // Search box
            txtSearch = new TextBox
            {
                Location = new Point(15, 15),
                Size = new Size(250, 30),
                Font = new Font("Segoe UI", 11),
                PlaceholderText = "🔍 Tìm kiếm khoa..."
            };
            txtSearch.TextChanged += TxtSearch_TextChanged;
            pnlToolbar.Controls.Add(txtSearch);

            // Buttons
            Button btnAdd = CreateToolbarButton("➕ Thêm mới", 280);
            btnAdd.Click += BtnAdd_Click;
            pnlToolbar.Controls.Add(btnAdd);

            Button btnEdit = CreateToolbarButton("✏️ Sửa", 400);
            btnEdit.Click += BtnEdit_Click;
            pnlToolbar.Controls.Add(btnEdit);

            Button btnDelete = CreateToolbarButton("🗑️ Xóa", 500);
            btnDelete.BackColor = Color.FromArgb(220, 53, 69);
            btnDelete.Click += BtnDelete_Click;
            pnlToolbar.Controls.Add(btnDelete);

            Button btnRefresh = CreateToolbarButton("🔄 Làm mới", 620);
            btnRefresh.BackColor = Color.FromArgb(23, 162, 184);
            btnRefresh.Click += (s, e) => LoadData();
            pnlToolbar.Controls.Add(btnRefresh);

            this.Controls.Add(pnlToolbar);

            // ==================== MAIN CONTENT ====================
            SplitContainer splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                SplitterDistance = 450,
                Panel1MinSize = 300,
                Panel2MinSize = 300
            };

            // Left Panel - Form nhập liệu
            Panel pnlForm = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                BackColor = Color.FromArgb(250, 250, 250)
            };

            Label lblFormTitle = new Label
            {
                Text = "📝 THÔNG TIN KHOA",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 102, 204),
                Location = new Point(20, 10),
                AutoSize = true
            };
            pnlForm.Controls.Add(lblFormTitle);

            // Mã Khoa
            CreateFormField(pnlForm, "Mã Khoa:", 50, out txtMaKhoa);
            
            // Tên Khoa
            CreateFormField(pnlForm, "Tên Khoa:", 120, out txtTenKhoa);
            
            // Email
            CreateFormField(pnlForm, "Email:", 190, out txtEmail);
            
            // Điện thoại
            CreateFormField(pnlForm, "Điện thoại:", 260, out txtDienThoai);

            // Buttons
            Panel pnlFormButtons = new Panel
            {
                Location = new Point(20, 340),
                Size = new Size(400, 50)
            };

            Button btnSave = new Button
            {
                Text = "💾 Lưu",
                Location = new Point(0, 0),
                Size = new Size(120, 40),
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSave.Click += BtnSave_Click;
            pnlFormButtons.Controls.Add(btnSave);

            Button btnClear = new Button
            {
                Text = "🔄 Làm mới",
                Location = new Point(130, 0),
                Size = new Size(120, 40),
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnClear.Click += (s, e) => ClearForm();
            pnlFormButtons.Controls.Add(btnClear);

            pnlForm.Controls.Add(pnlFormButtons);
            splitContainer.Panel1.Controls.Add(pnlForm);

            // Right Panel - DataGridView
            Panel pnlGrid = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            dgvKhoa = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                EnableHeadersVisualStyles = false
            };

            // Style header
            dgvKhoa.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 102, 204);
            dgvKhoa.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvKhoa.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvKhoa.ColumnHeadersHeight = 40;

            // Style rows
            dgvKhoa.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvKhoa.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 122, 204);
            dgvKhoa.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
            dgvKhoa.RowTemplate.Height = 35;

            dgvKhoa.SelectionChanged += DgvKhoa_SelectionChanged;

            pnlGrid.Controls.Add(dgvKhoa);
            splitContainer.Panel2.Controls.Add(pnlGrid);

            this.Controls.Add(splitContainer);
            this.ResumeLayout(false);
        }

        private Button CreateToolbarButton(string text, int x)
        {
            return new Button
            {
                Text = text,
                Location = new Point(x, 12),
                Size = new Size(110, 35),
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10),
                Cursor = Cursors.Hand
            };
        }

        private void CreateFormField(Panel parent, string labelText, int y, out TextBox textBox)
        {
            Label lbl = new Label
            {
                Text = labelText,
                Location = new Point(20, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 10)
            };
            parent.Controls.Add(lbl);

            textBox = new TextBox
            {
                Location = new Point(20, y + 25),
                Size = new Size(380, 30),
                Font = new Font("Segoe UI", 11)
            };
            parent.Controls.Add(textBox);
        }

        private async void LoadData()
        {
            try
            {
                _allKhoas = await ApiService.GetKhoasAsync();
                DisplayData(_allKhoas);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DisplayData(List<Khoa> data)
        {
            dgvKhoa.DataSource = null;
            dgvKhoa.DataSource = data.Select(k => new
            {
                k.MaKhoa,
                k.TenKhoa,
                k.Email,
                k.DienThoai
            }).ToList();

            // Đặt tên cột
            if (dgvKhoa.Columns.Count > 0)
            {
                dgvKhoa.Columns["MaKhoa"].HeaderText = "Mã Khoa";
                dgvKhoa.Columns["TenKhoa"].HeaderText = "Tên Khoa";
                dgvKhoa.Columns["Email"].HeaderText = "Email";
                dgvKhoa.Columns["DienThoai"].HeaderText = "Điện Thoại";
            }
        }

        private void TxtSearch_TextChanged(object? sender, EventArgs e)
        {
            var searchText = txtSearch.Text.ToLower();
            var filtered = _allKhoas.Where(k =>
                k.MaKhoa.ToLower().Contains(searchText) ||
                k.TenKhoa.ToLower().Contains(searchText) ||
                (k.Email?.ToLower().Contains(searchText) ?? false)
            ).ToList();
            DisplayData(filtered);
        }

        private void DgvKhoa_SelectionChanged(object? sender, EventArgs e)
        {
            if (dgvKhoa.CurrentRow != null)
            {
                var maKhoa = dgvKhoa.CurrentRow.Cells["MaKhoa"].Value?.ToString();
                var khoa = _allKhoas.FirstOrDefault(k => k.MaKhoa == maKhoa);
                if (khoa != null)
                {
                    txtMaKhoa.Text = khoa.MaKhoa;
                    txtTenKhoa.Text = khoa.TenKhoa;
                    txtEmail.Text = khoa.Email ?? "";
                    txtDienThoai.Text = khoa.DienThoai ?? "";
                    txtMaKhoa.Enabled = false; // Không cho sửa mã khi edit
                }
            }
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            ClearForm();
            txtMaKhoa.Enabled = true;
            txtMaKhoa.Focus();
        }

        private void BtnEdit_Click(object? sender, EventArgs e)
        {
            if (dgvKhoa.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn khoa cần sửa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            txtTenKhoa.Focus();
        }

        private async void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (dgvKhoa.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn khoa cần xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var maKhoa = dgvKhoa.CurrentRow.Cells["MaKhoa"].Value?.ToString();
            if (string.IsNullOrEmpty(maKhoa)) return;

            if (MessageBox.Show($"Bạn có chắc muốn xóa khoa '{maKhoa}'?", "Xác nhận xóa",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                var (success, message) = await ApiService.DeleteKhoaAsync(maKhoa);
                if (success)
                {
                    MessageBox.Show(message, "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                    ClearForm();
                }
                else
                {
                    MessageBox.Show(message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void BtnSave_Click(object? sender, EventArgs e)
        {
            // Validate
            if (string.IsNullOrWhiteSpace(txtMaKhoa.Text))
            {
                MessageBox.Show("Vui lòng nhập mã khoa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMaKhoa.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtTenKhoa.Text))
            {
                MessageBox.Show("Vui lòng nhập tên khoa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenKhoa.Focus();
                return;
            }

            var khoa = new Khoa
            {
                MaKhoa = txtMaKhoa.Text.Trim().ToUpper(),
                TenKhoa = txtTenKhoa.Text.Trim(),
                Email = string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text.Trim(),
                DienThoai = string.IsNullOrWhiteSpace(txtDienThoai.Text) ? null : txtDienThoai.Text.Trim()
            };

            bool isNew = txtMaKhoa.Enabled;
            var (success, message) = isNew
                ? await ApiService.CreateKhoaAsync(khoa)
                : await ApiService.UpdateKhoaAsync(khoa.MaKhoa, khoa);

            if (success)
            {
                MessageBox.Show(message, "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
                ClearForm();
            }
            else
            {
                MessageBox.Show(message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearForm()
        {
            txtMaKhoa.Text = "";
            txtTenKhoa.Text = "";
            txtEmail.Text = "";
            txtDienThoai.Text = "";
            txtMaKhoa.Enabled = true;
            dgvKhoa.ClearSelection();
        }
    }
}
