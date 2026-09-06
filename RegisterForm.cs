using System;
using System.Drawing;
using System.Windows.Forms;
using CinemaHallSystem.BLL;
using CinemaHallSystem.Models;
using CinemaHallSystem.Utilities;

namespace CinemaHallSystem.Forms
{
    public class RegisterForm : Form
    {
        private TextBox txtUsername;
        private TextBox txtPassword;
        private TextBox txtConfirmPassword;
        private TextBox txtFullName;
        private TextBox txtPhone;
        private TextBox txtEmail;
        private Button btnRegister;
        private Button btnCancel;

        public RegisterForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Register - Cinema Hall Management System";
            this.Size = new Size(500, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Font = new Font("Segoe UI", 10);

            int startY = 40;
            int spacing = 45;

            var labels = new[] { "Username:", "Password:", "Confirm Password:", "Full Name:", "Phone:", "Email:" };
            var textboxes = new[] { 
                txtUsername = new TextBox(), 
                txtPassword = new TextBox { PasswordChar = '*' }, 
                txtConfirmPassword = new TextBox { PasswordChar = '*' }, 
                txtFullName = new TextBox(), 
                txtPhone = new TextBox(), 
                txtEmail = new TextBox() 
            };

            for (int i = 0; i < labels.Length; i++)
            {
                var lbl = new Label { Text = labels[i], Location = new Point(80, startY + (i * spacing)), AutoSize = true };
                textboxes[i].Location = new Point(220, startY - 3 + (i * spacing));
                textboxes[i].Width = 180;

                this.Controls.Add(lbl);
                this.Controls.Add(textboxes[i]);
            }

            btnRegister = new Button
            {
                Text = "Register",
                Location = new Point(140, 360),
                Width = 100,
                Height = 35,
                BackColor = Color.Green,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnRegister.Click += BtnRegister_Click;
            this.Controls.Add(btnRegister);

            btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(260, 360),
                Width = 100,
                Height = 35,
                BackColor = Color.Gray,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnCancel.Click += (s, e) => this.Close();
            this.Controls.Add(btnCancel);
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtPassword.Text != txtConfirmPassword.Text)
                {
                    MessageBox.Show("Passwords do not match.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string error;
                if (!Validator.ValidateRegistration(txtUsername.Text, txtPassword.Text, txtConfirmPassword.Text, txtFullName.Text, out error))
                {
                    MessageBox.Show(error, "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                AuthManager.Register(txtUsername.Text, txtPassword.Text, txtFullName.Text, txtPhone.Text, txtEmail.Text);
                MessageBox.Show("Registration successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Registration Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
