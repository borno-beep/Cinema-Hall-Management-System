using System;
using System.Drawing;
using System.Windows.Forms;

namespace CinemaHallSystem.Forms
{
    public class PaymentForm : Form
    {
        private decimal totalAmount;
        public string SelectedPaymentMethod { get; private set; } = "Cash";

        private RadioButton rbCash;
        private RadioButton rbCard;
        private RadioButton rbMobileBanking;

        public PaymentForm(decimal totalAmount)
        {
            this.totalAmount = totalAmount;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Payment";
            this.Size = new Size(400, 350);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 10);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            Label lblTitle = new Label { Text = "Payment", Font = new Font("Segoe UI", 16, FontStyle.Bold), AutoSize = true, Location = new Point(150, 20) };
            this.Controls.Add(lblTitle);

            Label lblAmount = new Label { Text = $"Amount to Pay: ৳{totalAmount:F2}", Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = Color.FromArgb(211, 47, 47), AutoSize = true, Location = new Point(80, 70) };
            this.Controls.Add(lblAmount);

            GroupBox gbMethod = new GroupBox { Text = "Select Payment Method", Location = new Point(50, 120), Size = new Size(280, 120) };
            
            rbCash = new RadioButton { Text = "Cash", Location = new Point(20, 30), Checked = true, AutoSize = true };
            rbCard = new RadioButton { Text = "Credit/Debit Card", Location = new Point(20, 60), AutoSize = true };
            rbMobileBanking = new RadioButton { Text = "Mobile Banking (bKash/Nagad)", Location = new Point(20, 90), AutoSize = true };

            gbMethod.Controls.Add(rbCash);
            gbMethod.Controls.Add(rbCard);
            gbMethod.Controls.Add(rbMobileBanking);
            this.Controls.Add(gbMethod);

            Button btnConfirm = new Button { Text = "Confirm Payment", Location = new Point(190, 260), Size = new Size(140, 40), BackColor = Color.FromArgb(76, 175, 80), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnConfirm.Click += BtnConfirm_Click;
            
            Button btnCancel = new Button { Text = "Cancel", Location = new Point(80, 260), Size = new Size(90, 40), FlatStyle = FlatStyle.Flat };
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            this.Controls.Add(btnConfirm);
            this.Controls.Add(btnCancel);
        }

        private void BtnConfirm_Click(object sender, EventArgs e)
        {
            if (rbCash.Checked) SelectedPaymentMethod = "Cash";
            else if (rbCard.Checked) SelectedPaymentMethod = "Card";
            else if (rbMobileBanking.Checked) SelectedPaymentMethod = "Mobile Banking";

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
