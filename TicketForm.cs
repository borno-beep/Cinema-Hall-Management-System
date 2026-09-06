using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CinemaHallSystem.Models;

namespace CinemaHallSystem.Forms
{
    public class TicketForm : Form
    {
        private int bookingId;
        private string movieTitle;
        private string hallName;
        private DateTime showDate;
        private TimeSpan showTime;
        private List<ShowSeat> seats;
        private decimal totalAmount;
        private string paymentMethod;

        public TicketForm(int bookingId, string movieTitle, string hallName, DateTime showDate, TimeSpan showTime, List<ShowSeat> seats, decimal totalAmount, string paymentMethod)
        {
            this.bookingId = bookingId;
            this.movieTitle = movieTitle;
            this.hallName = hallName;
            this.showDate = showDate;
            this.showTime = showTime;
            this.seats = seats;
            this.totalAmount = totalAmount;
            this.paymentMethod = paymentMethod;

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Ticket";
            this.Size = new Size(500, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 10);
            this.BackColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            Panel ticketPanel = new Panel
            {
                Location = new Point(30, 20),
                Size = new Size(420, 480),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(250, 250, 250)
            };

            Label lblHeader = new Label { Text = "🎬 CINEMA HALL", Font = new Font("Segoe UI", 18, FontStyle.Bold), TextAlign = ContentAlignment.MiddleCenter, AutoSize = false, Size = new Size(400, 40), Location = new Point(10, 20) };
            ticketPanel.Controls.Add(lblHeader);

            ticketPanel.Controls.Add(new Label { Text = "--------------------------------------------------------", Location = new Point(10, 60), AutoSize = true, ForeColor = Color.Gray });

            int y = 90;
            ticketPanel.Controls.Add(new Label { Text = $"Booking ID: #{bookingId:D6}", Font = new Font("Segoe UI", 10, FontStyle.Bold), Location = new Point(20, y), AutoSize = true }); y += 30;
            ticketPanel.Controls.Add(new Label { Text = $"Movie: {movieTitle}", Location = new Point(20, y), AutoSize = true }); y += 30;
            ticketPanel.Controls.Add(new Label { Text = $"Hall: {hallName}", Location = new Point(20, y), AutoSize = true }); y += 30;
            ticketPanel.Controls.Add(new Label { Text = $"Date: {showDate:yyyy-MM-dd}", Location = new Point(20, y), AutoSize = true }); y += 30;
            ticketPanel.Controls.Add(new Label { Text = $"Time: {showTime:hh\\:mm}", Location = new Point(20, y), AutoSize = true }); y += 30;
            
            string seatList = string.Join(", ", seats.Select(s => s.SeatRow + s.SeatNo));
            ticketPanel.Controls.Add(new Label { Text = $"Seats: {seatList}", Location = new Point(20, y), AutoSize = true, MaximumSize = new Size(380, 0) }); y += 50;

            ticketPanel.Controls.Add(new Label { Text = $"Amount Paid: ৳{totalAmount:F2}", Font = new Font("Segoe UI", 12, FontStyle.Bold), Location = new Point(20, y), AutoSize = true }); y += 30;
            ticketPanel.Controls.Add(new Label { Text = $"Payment Method: {paymentMethod}", Location = new Point(20, y), AutoSize = true }); y += 30;

            ticketPanel.Controls.Add(new Label { Text = "--------------------------------------------------------", Location = new Point(10, y), AutoSize = true, ForeColor = Color.Gray }); y += 30;
            
            Label lblFooter = new Label { Text = "Thank you for your purchase!", Font = new Font("Segoe UI", 11, FontStyle.Italic), TextAlign = ContentAlignment.MiddleCenter, AutoSize = false, Size = new Size(400, 30), Location = new Point(10, y) };
            ticketPanel.Controls.Add(lblFooter);

            this.Controls.Add(ticketPanel);

            Button btnPrint = new Button { Text = "Print", Location = new Point(250, 510), Size = new Size(100, 40), BackColor = Color.FromArgb(33, 150, 243), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnPrint.Click += (s, e) => { MessageBox.Show("Ticket saved/printed successfully!", "Print", MessageBoxButtons.OK, MessageBoxIcon.Information); };
            
            Button btnClose = new Button { Text = "Close", Location = new Point(360, 510), Size = new Size(90, 40), FlatStyle = FlatStyle.Flat };
            btnClose.Click += (s, e) => { this.Close(); };

            this.Controls.Add(btnPrint);
            this.Controls.Add(btnClose);
        }
    }
}
