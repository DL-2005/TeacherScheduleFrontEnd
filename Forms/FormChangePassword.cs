using TeacherScheduleFrontend.Models;
using TeacherScheduleFrontend.Services;

namespace TeacherScheduleFrontend.Forms
{
    public partial class FormChangePassword : Form
    {
        public FormChangePassword()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
        }

        private TextBox txtOldPassword = null!;
        private TextBox txtNewPassword = null!;
        private TextBox txtConfirmPassword = null!;

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.Text = "Đổi Mật Khẩu";
            this.Size = new Size(400, 320);
            this.BackColor = Color.White;

            // Title
            Label lblTitle = new Label
            {
                Text = "🔑 ĐỔI MẬT KHẨU",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 102, 204),
                AutoSize = true,
                Location = new Point(100, 20)
            };
            this.Controls.Add(lblTitle);

            int y = 70;

            // Old password
            this.Controls.Add(new Label { Text = "Mật khẩu cũ:", Location = new Point(30, y), AutoSize = true, Font = new Font("Segoe UI", 10) });
            txtOldPassword = new TextBox { Location = new Point(30, y + 25), Size = new Size(320, 30), Font = new Font("Segoe UI", 11), PasswordChar = '●' };
            this.Controls.Add(txtOldPassword);
            y += 60;

            // New password
            this.Controls.Add(new Label { Text = "Mật khẩu mới:", Location = new Point(30, y), AutoSize = true, Font = new Font("Segoe UI", 10) });
            txtNewPassword = new TextBox { Location = new Point(30, y + 25), Size = new Size(320, 30), Font = new Font("Segoe UI", 11), PasswordChar = '●' };
            this.Controls.Add(txtNewPassword);
            y += 60;

            // Confirm password
            this.Controls.Add(new Label { Text = "Xác nhận mật khẩu:", Location = new Point(30, y), AutoSize = true, Font = new Font("Segoe UI", 10) });
            txtConfirmPassword = new TextBox { Location = new Point(30, y + 25), Size = new Size(320, 30), Font = new Font("Segoe UI", 11), PasswordChar = '●' };
            this.Controls.Add(txtConfirmPassword);
            y += 70;

            // Buttons
            Button btnSave = new Button
            {
                Text = "💾 Lưu",
                Location = new Point(30, y),
                Size = new Size(150, 40),
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);

            Button btnCancel = new Button
            {
                Text = "❌ Hủy",
                Location = new Point(200, y),
                Size = new Size(150, 40),
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };
            btnCancel.Click += (s, e) => this.Close();
            this.Controls.Add(btnCancel);

            this.ResumeLayout(false);
        }

        private async void BtnSave_Click(object? sender, EventArgs e)
        {
            // Validate
            if (string.IsNullOrWhiteSpace(txtOldPassword.Text))
            {
                MessageBox.Show("Vui lòng nhập mật khẩu cũ!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtOldPassword.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtNewPassword.Text) || txtNewPassword.Text.Length < 6)
            {
                MessageBox.Show("Mật khẩu mới phải có ít nhất 6 ký tự!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNewPassword.Focus();
                return;
            }

            if (txtNewPassword.Text != txtConfirmPassword.Text)
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtConfirmPassword.Focus();
                return;
            }

            var request = new ChangePasswordRequest
            {
                MatKhauCu = txtOldPassword.Text,
                MatKhauMoi = txtNewPassword.Text,
                XacNhanMatKhauMoi = txtConfirmPassword.Text
            };

            var (success, message) = await ApiService.ChangePasswordAsync(request);

            if (success)
            {
                MessageBox.Show(message, "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show(message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
