using System;
using System.Drawing;
using System.Windows.Forms;
using CinemaHallSystem.BLL;
using CinemaHallSystem.Utilities;

namespace CinemaHallSystem.Forms
{
    public class BookingHistoryForm : Form
    {
        private DataGridView dgvBookings;
        private DateTimePicker dtpFrom;
        private DateTimePicker dtpTo;

        public BookingHistoryForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Booking History";
            this.Font = new Font("Segoe UI", 10);
            this.Dock = DockStyle.Fill;
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopLevel = false;

            // Top Panel
            Panel topPanel = new Panel { Dock = DockStyle.Top, Height = 70, BackColor = Color.FromArgb(245, 245, 245) };
            
            Label lblTitle = new Label { Text = "Booking History", Font = new Font("Segoe UI", 16, FontStyle.Bold), AutoSize = true, Location = new Point(20, 20) };
            topPanel.Controls.Add(lblTitle);

            dtpFrom = new DateTimePicker { Format = DateTimePickerFormat.Short, Location = new Point(300, 25), Width = 120 };
            dtpTo = new DateTimePicker { Format = DateTimePickerFormat.Short, Location = new Point(450, 25), Width = 120 };
            
            Button btnSearch = new Button { Text = "Search", Location = new Point(600, 22), Size = new Size(80, 30), BackColor = Color.FromArgb(33, 150, 243), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnSearch.Click += BtnSearch_Click;

            topPanel.Controls.Add(new Label { Text = "From:", Location = new Point(250, 28), AutoSize = true });
            topPanel.Controls.Add(dtpFrom);
            topPanel.Controls.Add(new Label { Text = "To:", Location = new Point(430, 28), AutoSize = true });
            topPanel.Controls.Add(dtpTo);
            topPanel.Controls.Add(btnSearch);
            this.Controls.Add(topPanel);

            // Bottom Panel
            Panel bottomPanel = new Panel { Dock = DockStyle.Bottom, Height = 60, BackColor = Color.FromArgb(245, 245, 245) };
            
            Button btnViewDetails = new Button { Text = "View Details", Location = new Point(20, 15), Size = new Size(120, 35), FlatStyle = FlatStyle.Flat };
            btnViewDetails.Click += BtnViewDetails_Click;

            Button btnCancelBooking = new Button { Text = "Cancel Booking", Location = new Point(150, 15), Size = new Size(130, 35), BackColor = Color.FromArgb(211, 47, 47), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnCancelBooking.Click += BtnCancelBooking_Click;

            bottomPanel.Controls.Add(btnViewDetails);
            bottomPanel.Controls.Add(btnCancelBooking);
            this.Controls.Add(bottomPanel);

            // DataGridView
            dgvBookings = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White
            };
            this.Controls.Add(dgvBookings);
            dgvBookings.BringToFront();

            this.Load += (s, e) => LoadBookings();
        }

        private void LoadBookings()
        {
            try
            {
                var manager = new BookingManager();
                if (AuthManager.CurrentUser?.Role == "Customer")
                {
                    dgvBookings.DataSource = manager.GetUserBookings(AuthManager.CurrentUser.UserID);
                }
                else
                {
                    dgvBookings.DataSource = manager.GetAllBookings();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading bookings: {ex.Message}");
            }
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            // Ideally filter the data in BLL or DataTable
            MessageBox.Show("Search functionality to be implemented.");
        }

        private void BtnViewDetails_Click(object sender, EventArgs e)
        {
            if (dgvBookings.SelectedRows.Count > 0)
            {
                int bookingId = Convert.ToInt32(dgvBookings.SelectedRows[0].Cells["BookingID"].Value);
                MessageBox.Show($"Showing details for Booking #{bookingId}");
            }
        }

        private void BtnCancelBooking_Click(object sender, EventArgs e)
        {
            if (dgvBookings.SelectedRows.Count > 0)
            {
                string status = dgvBookings.SelectedRows[0].Cells["Status"].Value?.ToString() ?? "";
                if (status == "Cancelled")
                {
                    MessageBox.Show("This booking is already cancelled.");
                    return;
                }

                int bookingId = Convert.ToInt32(dgvBookings.SelectedRows[0].Cells["BookingID"].Value);
                if (MessageBox.Show($"Are you sure you want to cancel booking #{bookingId}?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        new BookingManager().CancelBooking(bookingId);
                        MessageBox.Show("Booking cancelled successfully.");
                        LoadBookings();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error cancelling booking: {ex.Message}");
                    }
                }
            }
        }
    }
}
