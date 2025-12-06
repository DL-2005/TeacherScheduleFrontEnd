using TeacherScheduleFrontend.Models;
using TeacherScheduleFrontend.Services;

namespace TeacherScheduleFrontend.Forms
{
    public partial class FormThongKe : Form
    {
        private TabControl tabControl = null!;
        private ComboBox cboHocKy = null!, cboKhoa = null!;
        private DataGridView dgvGioGiang = null!, dgvTheoKhoa = null!;
        private List<PhanCong> _phanCongs = new();
        private List<GiangVien> _giangViens = new();
        private List<Khoa> _khoas = new();
        private List<MonHoc> _monHocs = new();

        public FormThongKe()
        {
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.Text = "Thống Kê";
            this.BackColor = Color.White;

            // Header
            Panel pnlHeader = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.FromArgb(52, 73, 94) };
            pnlHeader.Controls.Add(new Label
            {
                Text = "📊 THỐNG KÊ GIẢNG DẠY",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(20, 15)
            });
            this.Controls.Add(pnlHeader);

            // Tab Control
            tabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11)
            };

            // Tab 1: Thống kê giờ giảng
            TabPage tabGioGiang = new TabPage("📈 Giờ Giảng Theo GV");
            tabGioGiang.BackColor = Color.White;
            CreateTabGioGiang(tabGioGiang);
            tabControl.TabPages.Add(tabGioGiang);

            // Tab 2: Thống kê theo khoa
            TabPage tabTheoKhoa = new TabPage("🏛️ Thống Kê Theo Khoa");
            tabTheoKhoa.BackColor = Color.White;
            CreateTabTheoKhoa(tabTheoKhoa);
            tabControl.TabPages.Add(tabTheoKhoa);

            // Tab 3: Tổng quan
            TabPage tabTongQuan = new TabPage("📋 Tổng Quan");
            tabTongQuan.BackColor = Color.White;
            CreateTabTongQuan(tabTongQuan);
            tabControl.TabPages.Add(tabTongQuan);

            this.Controls.Add(tabControl);
            this.ResumeLayout(false);
        }

        private void CreateTabGioGiang(TabPage tab)
        {
            // Filter panel
            Panel pnlFilter = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.FromArgb(245, 245, 245) };

            pnlFilter.Controls.Add(new Label { Text = "Học kỳ:", Location = new Point(15, 18), AutoSize = true, Font = new Font("Segoe UI", 10) });
            cboHocKy = new ComboBox
            {
                Location = new Point(75, 15),
                Size = new Size(150, 30),
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            // Add học kỳ options
            int year = DateTime.Now.Year;
            cboHocKy.Items.Add("Tất cả");
            for (int i = 0; i < 3; i++)
            {
                cboHocKy.Items.Add($"HK1-{year - i}");
                cboHocKy.Items.Add($"HK2-{year - i}");
            }
            cboHocKy.SelectedIndex = 0;
            cboHocKy.SelectedIndexChanged += (s, e) => RefreshGioGiang();
            pnlFilter.Controls.Add(cboHocKy);

            Button btnRefresh = new Button
            {
                Text = "🔄 Làm mới",
                Location = new Point(250, 12),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10)
            };
            btnRefresh.Click += (s, e) => RefreshGioGiang();
            pnlFilter.Controls.Add(btnRefresh);

            Button btnExport = new Button
            {
                Text = "📥 Xuất Excel",
                Location = new Point(360, 12),
                Size = new Size(110, 35),
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10)
            };
            btnExport.Click += BtnExportGioGiang_Click;
            pnlFilter.Controls.Add(btnExport);

            tab.Controls.Add(pnlFilter);

            // Grid
            dgvGioGiang = CreateGrid(Color.FromArgb(41, 128, 185));
            tab.Controls.Add(dgvGioGiang);
        }

        private void CreateTabTheoKhoa(TabPage tab)
        {
            // Filter panel
            Panel pnlFilter = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.FromArgb(245, 245, 245) };

            pnlFilter.Controls.Add(new Label { Text = "Khoa:", Location = new Point(15, 18), AutoSize = true, Font = new Font("Segoe UI", 10) });
            cboKhoa = new ComboBox
            {
                Location = new Point(60, 15),
                Size = new Size(250, 30),
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboKhoa.SelectedIndexChanged += (s, e) => RefreshTheoKhoa();
            pnlFilter.Controls.Add(cboKhoa);

            Button btnRefresh = new Button
            {
                Text = "🔄 Làm mới",
                Location = new Point(330, 12),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10)
            };
            btnRefresh.Click += (s, e) => RefreshTheoKhoa();
            pnlFilter.Controls.Add(btnRefresh);

            tab.Controls.Add(pnlFilter);

            // Grid
            dgvTheoKhoa = CreateGrid(Color.FromArgb(39, 174, 96));
            tab.Controls.Add(dgvTheoKhoa);
        }

        private void CreateTabTongQuan(TabPage tab)
        {
            FlowLayoutPanel flow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(20),
                BackColor = Color.FromArgb(236, 240, 241)
            };

            tab.Controls.Add(flow);
            tab.Tag = flow; // Store reference for later update
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
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
            return dgv;
        }

        private async void LoadData()
        {
            _phanCongs = await ApiService.GetPhanCongsAsync();
            _giangViens = await ApiService.GetGiangViensAsync();
            _khoas = await ApiService.GetKhoasAsync();
            _monHocs = await ApiService.GetMonHocsAsync();

            // Load khoa combo
            cboKhoa.DataSource = null;
            var khoaList = new List<Khoa> { new Khoa { MaKhoa = "", TenKhoa = "Tất cả" } };
            khoaList.AddRange(_khoas);
            cboKhoa.DataSource = khoaList;
            cboKhoa.DisplayMember = "TenKhoa";
            cboKhoa.ValueMember = "MaKhoa";

            RefreshGioGiang();
            RefreshTheoKhoa();
            RefreshTongQuan();
        }

        private void RefreshGioGiang()
        {
            var hocKy = cboHocKy.SelectedItem?.ToString() ?? "Tất cả";
            var filtered = hocKy == "Tất cả" ? _phanCongs : _phanCongs.Where(p => p.ThoiGianHoc == hocKy).ToList();

            // Group by giảng viên
            var stats = filtered
                .GroupBy(p => p.MaGV)
                .Select(g =>
                {
                    var gv = _giangViens.FirstOrDefault(x => x.MaGV == g.Key);
                    var tongTiet = g.Sum(p => p.SoTiet);
                    var soLop = g.Select(p => p.MaLop).Distinct().Count();
                    var soMon = g.Select(p => p.MaMH).Distinct().Count();
                    return new
                    {
                        MaGV = g.Key,
                        TenGV = gv?.TenGV ?? g.Key,
                        TenKhoa = gv?.Khoa?.TenKhoa ?? "",
                        SoLop = soLop,
                        SoMon = soMon,
                        TongTiet = tongTiet,
                        TongGio = Math.Round(tongTiet * 0.75, 1) // 1 tiết = 45 phút = 0.75 giờ
                    };
                })
                .OrderByDescending(x => x.TongTiet)
                .ToList();

            dgvGioGiang.DataSource = null;
            dgvGioGiang.DataSource = stats;

            if (dgvGioGiang.Columns.Count > 0)
            {
                dgvGioGiang.Columns["MaGV"].HeaderText = "Mã GV";
                dgvGioGiang.Columns["TenGV"].HeaderText = "Tên Giảng Viên";
                dgvGioGiang.Columns["TenKhoa"].HeaderText = "Khoa";
                dgvGioGiang.Columns["SoLop"].HeaderText = "Số Lớp";
                dgvGioGiang.Columns["SoMon"].HeaderText = "Số Môn";
                dgvGioGiang.Columns["TongTiet"].HeaderText = "Tổng Tiết";
                dgvGioGiang.Columns["TongGio"].HeaderText = "Tổng Giờ";
            }
        }

        private void RefreshTheoKhoa()
        {
            var maKhoa = cboKhoa.SelectedValue?.ToString() ?? "";

            // Lọc GV theo khoa
            var gvFiltered = string.IsNullOrEmpty(maKhoa) ? _giangViens : _giangViens.Where(g => g.MaKhoa == maKhoa).ToList();
            var gvIds = gvFiltered.Select(g => g.MaGV).ToHashSet();

            // Thống kê theo khoa
            var stats = _khoas
                .Where(k => string.IsNullOrEmpty(maKhoa) || k.MaKhoa == maKhoa)
                .Select(k =>
                {
                    var gvsOfKhoa = _giangViens.Where(g => g.MaKhoa == k.MaKhoa).ToList();
                    var gvIdsKhoa = gvsOfKhoa.Select(g => g.MaGV).ToHashSet();
                    var pcsOfKhoa = _phanCongs.Where(p => gvIdsKhoa.Contains(p.MaGV)).ToList();

                    return new
                    {
                        k.MaKhoa,
                        k.TenKhoa,
                        SoGV = gvsOfKhoa.Count,
                        SoPhanCong = pcsOfKhoa.Count,
                        TongTiet = pcsOfKhoa.Sum(p => p.SoTiet),
                        SoLop = pcsOfKhoa.Select(p => p.MaLop).Distinct().Count(),
                        SoMon = pcsOfKhoa.Select(p => p.MaMH).Distinct().Count()
                    };
                })
                .OrderByDescending(x => x.TongTiet)
                .ToList();

            dgvTheoKhoa.DataSource = null;
            dgvTheoKhoa.DataSource = stats;

            if (dgvTheoKhoa.Columns.Count > 0)
            {
                dgvTheoKhoa.Columns["MaKhoa"].HeaderText = "Mã Khoa";
                dgvTheoKhoa.Columns["TenKhoa"].HeaderText = "Tên Khoa";
                dgvTheoKhoa.Columns["SoGV"].HeaderText = "Số GV";
                dgvTheoKhoa.Columns["SoPhanCong"].HeaderText = "Số PC";
                dgvTheoKhoa.Columns["TongTiet"].HeaderText = "Tổng Tiết";
                dgvTheoKhoa.Columns["SoLop"].HeaderText = "Số Lớp";
                dgvTheoKhoa.Columns["SoMon"].HeaderText = "Số Môn";
            }
        }

        private void RefreshTongQuan()
        {
            var tabTongQuan = tabControl.TabPages[2];
            if (tabTongQuan.Tag is FlowLayoutPanel flow)
            {
                flow.Controls.Clear();

                // Cards thống kê
                AddStatCard(flow, "👨‍🏫", "Tổng Giảng Viên", _giangViens.Count.ToString(), Color.FromArgb(52, 152, 219));
                AddStatCard(flow, "🏛️", "Tổng Số Khoa", _khoas.Count.ToString(), Color.FromArgb(155, 89, 182));
                AddStatCard(flow, "📚", "Tổng Môn Học", _monHocs.Count.ToString(), Color.FromArgb(46, 204, 113));
                AddStatCard(flow, "📋", "Tổng Phân Công", _phanCongs.Count.ToString(), Color.FromArgb(241, 196, 15));
                AddStatCard(flow, "⏱️", "Tổng Số Tiết", _phanCongs.Sum(p => p.SoTiet).ToString(), Color.FromArgb(231, 76, 60));
                AddStatCard(flow, "🎓", "Số Lớp Được PC", _phanCongs.Select(p => p.MaLop).Distinct().Count().ToString(), Color.FromArgb(26, 188, 156));
            }
        }

        private void AddStatCard(FlowLayoutPanel flow, string icon, string title, string value, Color color)
        {
            Panel card = new Panel
            {
                Size = new Size(200, 120),
                BackColor = color,
                Margin = new Padding(10)
            };

            card.Controls.Add(new Label
            {
                Text = icon,
                Font = new Font("Segoe UI", 28),
                ForeColor = Color.White,
                Location = new Point(15, 10),
                AutoSize = true
            });

            card.Controls.Add(new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(80, 15),
                AutoSize = true
            });

            card.Controls.Add(new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.White,
                Location = new Point(15, 85),
                AutoSize = true
            });

            flow.Controls.Add(card);
        }

        private void BtnExportGioGiang_Click(object? sender, EventArgs e)
        {
            try
            {
                using SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "CSV files (*.csv)|*.csv";
                sfd.FileName = $"ThongKeGioGiang_{DateTime.Now:yyyyMMdd}.csv";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    using var writer = new StreamWriter(sfd.FileName, false, System.Text.Encoding.UTF8);

                    // Header
                    var headers = new List<string>();
                    foreach (DataGridViewColumn col in dgvGioGiang.Columns)
                    {
                        headers.Add(col.HeaderText);
                    }
                    writer.WriteLine(string.Join(",", headers));

                    // Data
                    foreach (DataGridViewRow row in dgvGioGiang.Rows)
                    {
                        var values = new List<string>();
                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            values.Add($"\"{cell.Value}\"");
                        }
                        writer.WriteLine(string.Join(",", values));
                    }

                    MessageBox.Show($"Đã xuất file: {sfd.FileName}", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi xuất file: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
