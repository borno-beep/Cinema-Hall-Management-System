using System;
using System.Drawing;
using System.Windows.Forms;
using CinemaHallSystem.BLL;
using CinemaHallSystem.Models;

namespace CinemaHallSystem.Forms
{
    public class LoginForm : Form
    {
        private Label lblTitle;
        private Label lblUsername;
        private TextBox txtUsername;
        private Label lblPassword;
        private TextBox txtPassword;
        private Button btnLogin;
        private LinkLabel lnkRegister;

        public LoginForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Login - Cinema Hall Management System";
            this.Size = new Size(450, 350);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Font = new Font("Segoe UI", 10);
            this.FormClosing += LoginForm_FormClosing;

            lblTitle = new Label
            {
                Text = "Cinema Hall Management System",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(60, 30)
            };
            this.Controls.Add(lblTitle);

            lblUsername = new Label { Text = "Username:", Location = new Point(80, 100), AutoSize = true };
            this.Controls.Add(lblUsername);

            txtUsername = new TextBox { Location = new Point(180, 97), Width = 150 };
            this.Controls.Add(txtUsername);

            lblPassword = new Label { Text = "Password:", Location = new Point(80, 150), AutoSize = true };
            this.Controls.Add(lblPassword);

            txtPassword = new TextBox { Location = new Point(180, 147), Width = 150, PasswordChar = '*' };
            this.Controls.Add(txtPassword);

            btnLogin = new Button
            {
                Text = "Login",
                Location = new Point(150, 200),
                Width = 100,
                Height = 35,
                BackColor = Color.Blue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnLogin.Click += BtnLogin_Click;
            this.Controls.Add(btnLogin);

            lnkRegister = new LinkLabel
            {
                Text = "Register",
                Location = new Point(175, 250),
                AutoSize = true
            };
            lnkRegister.Click += LnkRegister_Click;
            this.Controls.Add(lnkRegister);
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                var user = AuthManager.Login(txtUsername.Text, txtPassword.Text);
                if (user != null)
                {
                    Form dashboard = user.GetDashboardForm();
                    dashboard.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Invalid username or password.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LnkRegister_Click(object sender, EventArgs e)
        {
            using var registerForm = new RegisterForm();
            registerForm.ShowDialog();
        }

        private void LoginForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
