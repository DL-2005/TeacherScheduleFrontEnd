using TeacherScheduleFrontend.Services;

namespace TeacherScheduleFrontend.Forms
{
    public partial class FormMain : Form
    {
        private Panel pnlContent = null!;
        private Label lblWelcome = null!;
        private Label lblRole = null!;

        public FormMain()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            UpdateUserInfo();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form properties
            this.Text = "Hệ Thống Quản Lý Giảng Viên";
            this.Size = new Size(1400, 800);
            this.MinimumSize = new Size(1200, 700);
            this.BackColor = Color.FromArgb(240, 240, 240);

            // ==================== MENU STRIP ====================
            MenuStrip menuStrip = new MenuStrip
            {
                BackColor = Color.FromArgb(0, 102, 204),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10),
                Padding = new Padding(5)
            };

            // Menu Hệ thống
            ToolStripMenuItem mnuHeThong = new ToolStripMenuItem("🏠 Hệ Thống");
            mnuHeThong.DropDownItems.Add("📊 Dashboard", null, (s, e) => ShowDashboard());
            mnuHeThong.DropDownItems.Add("🔑 Đổi mật khẩu", null, (s, e) => ShowChangePassword());
            mnuHeThong.DropDownItems.Add(new ToolStripSeparator());
            mnuHeThong.DropDownItems.Add("🚪 Đăng xuất", null, (s, e) => Logout());
            menuStrip.Items.Add(mnuHeThong);

            // Menu Quản lý đơn vị (chỉ Admin/Trưởng khoa)
            if (IsAdminOrTruongKhoa())
            {
                ToolStripMenuItem mnuDonVi = new ToolStripMenuItem("🏫 Quản Lý Đơn Vị");
                mnuDonVi.DropDownItems.Add("📚 Quản lý Khoa", null, (s, e) => OpenForm(new FormKhoa()));
                mnuDonVi.DropDownItems.Add("📖 Quản lý Bộ môn", null, (s, e) => OpenForm(new FormBoMon()));
                menuStrip.Items.Add(mnuDonVi);
            }

            // Menu Quản lý nhân sự
            if (IsAdminOrTruongKhoa())
            {
                ToolStripMenuItem mnuNhanSu = new ToolStripMenuItem("👥 Quản Lý Nhân Sự");
                mnuNhanSu.DropDownItems.Add("👨‍🏫 Quản lý Giảng viên", null, (s, e) => OpenForm(new FormGiangVien()));
                mnuNhanSu.DropDownItems.Add("👤 Quản lý Tài khoản", null, (s, e) => OpenForm(new FormTaiKhoan()));
                menuStrip.Items.Add(mnuNhanSu);
            }

            // Menu Quản lý đào tạo
            ToolStripMenuItem mnuDaoTao = new ToolStripMenuItem("📚 Quản Lý Đào Tạo");
            mnuDaoTao.DropDownItems.Add("📕 Quản lý Môn học", null, (s, e) => OpenForm(new FormMonHoc()));
            mnuDaoTao.DropDownItems.Add("🎓 Quản lý Lớp", null, (s, e) => OpenForm(new FormLop()));
            mnuDaoTao.DropDownItems.Add(new ToolStripSeparator());
            mnuDaoTao.DropDownItems.Add("📋 Phân công giảng dạy", null, (s, e) => OpenForm(new FormPhanCong()));
            menuStrip.Items.Add(mnuDaoTao);

            // Menu Thống kê
            ToolStripMenuItem mnuThongKe = new ToolStripMenuItem("📊 Thống Kê");
            mnuThongKe.DropDownItems.Add("📈 Thống kê giờ giảng", null, (s, e) => ShowThongKeGioGiang());
            mnuThongKe.DropDownItems.Add("📉 Thống kê theo khoa", null, (s, e) => ShowThongKeKhoa());
            menuStrip.Items.Add(mnuThongKe);

            // Menu Trợ giúp
            ToolStripMenuItem mnuHelp = new ToolStripMenuItem("❓ Trợ Giúp");
            mnuHelp.DropDownItems.Add("ℹ️ Giới thiệu", null, (s, e) => ShowAbout());
            menuStrip.Items.Add(mnuHelp);

            this.MainMenuStrip = menuStrip;
            this.Controls.Add(menuStrip);

            // ==================== HEADER PANEL ====================
            Panel pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.FromArgb(0, 122, 204),
                Padding = new Padding(20, 10, 20, 10)
            };

            Label lblTitleHeader = new Label
            {
                Text = "🎓 HỆ THỐNG QUẢN LÝ GIẢNG VIÊN",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(20, 10)
            };
            pnlHeader.Controls.Add(lblTitleHeader);

            // Thông tin user
            lblWelcome = new Label
            {
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(20, 50)
            };
            pnlHeader.Controls.Add(lblWelcome);

            lblRole = new Label
            {
                Font = new Font("Segoe UI", 10, FontStyle.Italic),
                ForeColor = Color.LightYellow,
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Location = new Point(this.Width - 200, 50)
            };
            pnlHeader.Controls.Add(lblRole);

            this.Controls.Add(pnlHeader);

            // ==================== SIDEBAR ====================
            Panel pnlSidebar = new Panel
            {
                Dock = DockStyle.Left,
                Width = 220,
                BackColor = Color.FromArgb(45, 52, 54),
                Padding = new Padding(10)
            };

            // Các button sidebar
            int yPos = 20;
            string[] sidebarItems = new[]
            {
                "📊|Dashboard|ShowDashboard",
                "🏫|Quản lý Khoa|FormKhoa",
                "📖|Quản lý Bộ môn|FormBoMon",
                "👨‍🏫|Giảng viên|FormGiangVien",
                "📕|Môn học|FormMonHoc",
                "🎓|Lớp học|FormLop",
                "📋|Phân công|FormPhanCong",
                "👤|Tài khoản|FormTaiKhoan"
            };

            foreach (var item in sidebarItems)
            {
                var parts = item.Split('|');
                var btn = CreateSidebarButton(parts[0] + " " + parts[1], yPos);
                btn.Tag = parts[2];
                btn.Click += SidebarButton_Click;
                pnlSidebar.Controls.Add(btn);
                yPos += 50;
            }

            // Nút đăng xuất ở cuối sidebar
            Button btnLogout = CreateSidebarButton("🚪 Đăng xuất", pnlSidebar.Height - 60);
            btnLogout.Dock = DockStyle.Bottom;
            btnLogout.BackColor = Color.FromArgb(192, 57, 43);
            btnLogout.Click += (s, e) => Logout();
            pnlSidebar.Controls.Add(btnLogout);

            this.Controls.Add(pnlSidebar);

            // ==================== CONTENT PANEL ====================
            pnlContent = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(20)
            };
            this.Controls.Add(pnlContent);

            // Hiển thị Dashboard mặc định
            ShowDashboard();

            this.ResumeLayout(false);
        }

        private Button CreateSidebarButton(string text, int yPosition)
        {
            return new Button
            {
                Text = text,
                Location = new Point(5, yPosition),
                Size = new Size(200, 45),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(52, 73, 94),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0),
                Cursor = Cursors.Hand
            };
        }

        private void SidebarButton_Click(object? sender, EventArgs e)
        {
            if (sender is Button btn && btn.Tag is string formName)
            {
                switch (formName)
                {
                    case "ShowDashboard":
                        ShowDashboard();
                        break;
                    case "FormKhoa":
                        OpenForm(new FormKhoa());
                        break;
                    case "FormBoMon":
                        OpenForm(new FormBoMon());
                        break;
                    case "FormGiangVien":
                        OpenForm(new FormGiangVien());
                        break;
                    case "FormMonHoc":
                        OpenForm(new FormMonHoc());
                        break;
                    case "FormLop":
                        OpenForm(new FormLop());
                        break;
                    case "FormPhanCong":
                        OpenForm(new FormPhanCong());
                        break;
                    case "FormTaiKhoan":
                        OpenForm(new FormTaiKhoan());
                        break;
                }
            }
        }

        private void UpdateUserInfo()
        {
            var user = ApiService.CurrentUser;
            if (user != null)
            {
                lblWelcome.Text = $"👋 Xin chào, {user.TenGV ?? user.MaTK}!";
                lblRole.Text = $"Chức vụ: {GetRoleName(user.ChucVu)}";
            }
        }

        private string GetRoleName(string chucVu)
        {
            return chucVu switch
            {
                "CQC" => "Cán bộ quản lý",
                "TK" => "Trưởng khoa",
                "TBM" => "Trưởng bộ môn",
                "GV" => "Giảng viên",
                _ => chucVu
            };
        }

        private bool IsAdminOrTruongKhoa()
        {
            var user = ApiService.CurrentUser;
            return user != null && (user.ChucVu == "CQC" || user.ChucVu == "TK");
        }

        private void OpenForm(Form form)
        {
            pnlContent.Controls.Clear();
            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;
            pnlContent.Controls.Add(form);
            form.Show();
        }

        private void ShowDashboard()
        {
            pnlContent.Controls.Clear();

            // Title
            Label lblTitle = new Label
            {
                Text = "📊 DASHBOARD - TỔNG QUAN HỆ THỐNG",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 102, 204),
                AutoSize = true,
                Location = new Point(20, 20)
            };
            pnlContent.Controls.Add(lblTitle);

            // Tạo các card thống kê
            int cardY = 80;
            int cardX = 20;

            // Card 1 - Giảng viên
            CreateDashboardCard("👨‍🏫", "Giảng viên", "Đang tải...", cardX, cardY, Color.FromArgb(52, 152, 219));
            cardX += 270;

            // Card 2 - Khoa
            CreateDashboardCard("🏫", "Khoa", "Đang tải...", cardX, cardY, Color.FromArgb(46, 204, 113));
            cardX += 270;

            // Card 3 - Môn học
            CreateDashboardCard("📕", "Môn học", "Đang tải...", cardX, cardY, Color.FromArgb(155, 89, 182));
            cardX += 270;

            // Card 4 - Lớp học
            CreateDashboardCard("🎓", "Lớp học", "Đang tải...", cardX, cardY, Color.FromArgb(230, 126, 34));

            // Load dữ liệu
            LoadDashboardData();
        }

        private void CreateDashboardCard(string icon, string title, string value, int x, int y, Color bgColor)
        {
            Panel card = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(250, 120),
                BackColor = bgColor,
                Tag = title
            };

            Label lblIcon = new Label
            {
                Text = icon,
                Font = new Font("Segoe UI", 36),
                ForeColor = Color.White,
                Location = new Point(15, 20),
                AutoSize = true
            };
            card.Controls.Add(lblIcon);

            Label lblTitle = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.White,
                Location = new Point(90, 25),
                AutoSize = true
            };
            card.Controls.Add(lblTitle);

            Label lblValue = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(90, 55),
                AutoSize = true,
                Name = $"lblValue_{title}"
            };
            card.Controls.Add(lblValue);

            pnlContent.Controls.Add(card);
        }

        private async void LoadDashboardData()
        {
            try
            {
                var giangViens = await ApiService.GetGiangViensAsync();
                var khoas = await ApiService.GetKhoasAsync();
                var monHocs = await ApiService.GetMonHocsAsync();
                var lops = await ApiService.GetLopsAsync();

                UpdateCardValue("Giảng viên", giangViens.Count.ToString());
                UpdateCardValue("Khoa", khoas.Count.ToString());
                UpdateCardValue("Môn học", monHocs.Count.ToString());
                UpdateCardValue("Lớp học", lops.Count.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateCardValue(string cardTitle, string value)
        {
            foreach (Control ctrl in pnlContent.Controls)
            {
                if (ctrl is Panel panel && panel.Tag?.ToString() == cardTitle)
                {
                    var lblValue = panel.Controls.Find($"lblValue_{cardTitle}", false).FirstOrDefault();
                    if (lblValue != null)
                    {
                        lblValue.Text = value;
                    }
                }
            }
        }

        private void ShowChangePassword()
        {
            using var form = new FormChangePassword();
            form.ShowDialog();
        }

        private void ShowThongKeGioGiang()
        {
            OpenForm(new FormThongKe());
        }

        private void ShowThongKeKhoa()
        {
            OpenForm(new FormThongKe());
        }

        private void ShowAbout()
        {
            MessageBox.Show(
                "Hệ Thống Quản Lý Giảng Viên\n\n" +
                "Phiên bản: 1.0.0\n" +
                "Phát triển bởi: [Tên của bạn]\n" +
                "Email: [email@example.com]\n\n" +
                "© 2024 - All Rights Reserved",
                "Giới thiệu",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        private void Logout()
        {
            if (MessageBox.Show("Bạn có chắc muốn đăng xuất?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                ApiService.Logout();
                this.Hide();
                var loginForm = new FormLogin();
                loginForm.FormClosed += (s, args) => this.Close();
                loginForm.Show();
            }
        }
    }
}
