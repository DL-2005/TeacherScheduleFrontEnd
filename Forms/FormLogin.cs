using TeacherScheduleFrontend.Services;

namespace TeacherScheduleFrontend.Forms
{
    public partial class FormLogin : Form
    {
        public FormLogin()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form properties
            this.Text = "Đăng Nhập - Hệ Thống Quản Lý Giảng Viên";
            this.Size = new Size(600, 400);
            this.BackColor = Color.White;

            // Panel chính
            Panel mainPanel = new Panel
            {
                Size = new Size(550, 280),
                Location = new Point(15, 20),
                BackColor = Color.WhiteSmoke
            };

            // Tiêu đề
            Label lblTitle = new Label
            {
                Text = "🎓 HỆ THỐNG QUẢN LÝ GIẢNG VIÊN",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 102, 204),
                AutoSize = true,
                Location = new Point(50, 20)
            };
            mainPanel.Controls.Add(lblTitle);

            // Label Tên đăng nhập
            Label lblUsername = new Label
            {
                Text = "Tên đăng nhập:",
                Location = new Point(50, 70),
                AutoSize = true,
                Font = new Font("Segoe UI", 10)
            };
            mainPanel.Controls.Add(lblUsername);

            // TextBox Tên đăng nhập
            txtUsername = new TextBox
            {
                Location = new Point(50, 95),
                Size = new Size(450, 30),
                Font = new Font("Segoe UI", 11),
                PlaceholderText = "Nhập tên đăng nhập..."
            };
            mainPanel.Controls.Add(txtUsername);

            // Label Mật khẩu
            Label lblPassword = new Label
            {
                Text = "Mật khẩu:",
                Location = new Point(50, 135),
                AutoSize = true,
                Font = new Font("Segoe UI", 10)
            };
            mainPanel.Controls.Add(lblPassword);

            // TextBox Mật khẩu
            txtPassword = new TextBox
            {
                Location = new Point(50, 160),
                Size = new Size(450, 30),
                Font = new Font("Segoe UI", 11),
                PasswordChar = '●',
                PlaceholderText = "Nhập mật khẩu..."
            };
            txtPassword.KeyPress += TxtPassword_KeyPress;
            mainPanel.Controls.Add(txtPassword);

            // Checkbox hiện mật khẩu
            chkShowPassword = new CheckBox
            {
                Text = "Hiện mật khẩu",
                Location = new Point(50, 195),
                AutoSize = true,
                Font = new Font("Segoe UI", 9)
            };
            chkShowPassword.CheckedChanged += ChkShowPassword_CheckedChanged;
            mainPanel.Controls.Add(chkShowPassword);

            // Button Đăng nhập
            btnLogin = new Button
            {
                Text = "🔐 Đăng Nhập",
                Location = new Point(50, 230),
                Size = new Size(165, 40),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Click += BtnLogin_Click;
            mainPanel.Controls.Add(btnLogin);

            // Button Thoát
            btnExit = new Button
            {
                Text = "❌ Thoát",
                Location = new Point(335, 230),
                Size = new Size(165, 40),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnExit.FlatAppearance.BorderSize = 0;
            btnExit.Click += BtnExit_Click;
            mainPanel.Controls.Add(btnExit);

            this.Controls.Add(mainPanel);

            // Label trạng thái
            lblStatus = new Label
            {
                Text = "",
                Location = new Point(25, 310),
                Size = new Size(400, 25),
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Red,
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(lblStatus);

            this.ResumeLayout(false);
        }

        private TextBox txtUsername = null!;
        private TextBox txtPassword = null!;
        private CheckBox chkShowPassword = null!;
        private Button btnLogin = null!;
        private Button btnExit = null!;
        private Label lblStatus = null!;

        private void ChkShowPassword_CheckedChanged(object? sender, EventArgs e)
        {
            txtPassword.PasswordChar = chkShowPassword.Checked ? '\0' : '●';
        }

        private void TxtPassword_KeyPress(object? sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                BtnLogin_Click(sender, e);
            }
        }

        private async void BtnLogin_Click(object? sender, EventArgs e)
        {
            // Validate
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                ShowError("Vui lòng nhập tên đăng nhập!");
                txtUsername.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                ShowError("Vui lòng nhập mật khẩu!");
                txtPassword.Focus();
                return;
            }

            // Disable controls
            SetControlsEnabled(false);
            lblStatus.ForeColor = Color.Blue;
            lblStatus.Text = "⏳ Đang đăng nhập...";



            try
            {
                var (success, message, data) = await ApiService.LoginAsync(
                    txtUsername.Text.Trim(),
                    txtPassword.Text
                );

                if (success && data != null)
                {
                    lblStatus.ForeColor = Color.Green;
                    lblStatus.Text = "✅ " + message;

                    // Mở form chính
                    this.Hide();
                    var mainForm = new FormMain();
                    mainForm.FormClosed += (s, args) => this.Close();
                    mainForm.Show();
                }
                else
                {
                    ShowError(message);
                    txtPassword.Clear();
                    txtPassword.Focus();
                }
            }
            catch (Exception ex)
            {
                ShowError($"Lỗi: {ex.Message}");
            }
            finally
            {
                SetControlsEnabled(true);
            }
        }

        private void BtnExit_Click(object? sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc muốn thoát?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void ShowError(string message)
        {
            lblStatus.ForeColor = Color.Red;
            lblStatus.Text = "❌ " + message;
        }

        private void SetControlsEnabled(bool enabled)
        {
            txtUsername.Enabled = enabled;
            txtPassword.Enabled = enabled;
            btnLogin.Enabled = enabled;
            btnExit.Enabled = enabled;
            chkShowPassword.Enabled = enabled;
        }
    }
}
