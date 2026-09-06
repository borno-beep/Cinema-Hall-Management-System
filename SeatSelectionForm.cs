using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CinemaHallSystem.BLL;
using CinemaHallSystem.Models;
using CinemaHallSystem.Utilities;

namespace CinemaHallSystem.Forms
{
    public class SeatSelectionForm : Form
    {
        private int showId;
        private string movieTitle;
        private string hallName;
        private DateTime showDate;
        private TimeSpan showTime;
        private decimal ticketPrice;

        private Panel topPanel;
        private Label lblMovieInfo;
        private Label lblShowInfo;

        private Panel centerPanel;
        private TableLayoutPanel seatGrid;

        private Panel bottomPanel;
        private Label lblTotal;
        private Button btnProceed;

        private List<ShowSeat> selectedSeats = new List<ShowSeat>();

        public SeatSelectionForm(int showId, string movieTitle, string hallName, DateTime showDate, TimeSpan showTime, decimal ticketPrice)
        {
            this.showId = showId;
            this.movieTitle = movieTitle;
            this.hallName = hallName;
            this.showDate = showDate;
            this.showTime = showTime;
            this.ticketPrice = ticketPrice;

            InitializeComponent();
            LoadSeats();
        }

        private void InitializeComponent()
        {
            this.Text = "Seat Selection";
            this.Size = new Size(900, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 10);
            this.BackColor = Color.White;

            // Top Panel
            topPanel = new Panel { Dock = DockStyle.Top, Height = 100, BackColor = Color.FromArgb(240, 240, 240) };
            lblMovieInfo = new Label
            {
                Text = movieTitle,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(20, 20)
            };
            lblShowInfo = new Label
            {
                Text = $"{hallName} | {showDate:yyyy-MM-dd} | {showTime:hh\\:mm} | Price: ৳{ticketPrice}",
                AutoSize = true,
                Location = new Point(20, 55)
            };
            topPanel.Controls.Add(lblMovieInfo);
            topPanel.Controls.Add(lblShowInfo);
            this.Controls.Add(topPanel);

            // Bottom Panel
            bottomPanel = new Panel { Dock = DockStyle.Bottom, Height = 80, BackColor = Color.FromArgb(240, 240, 240) };
            
            var lblLegend = new Label { Text = "Legend: ■ Available   ■ Selected   ■ Booked", Location = new Point(20, 30), AutoSize = true };
            
            lblTotal = new Label { Text = "Selected: 0 seat(s) | Total: ৳0.00", Font = new Font("Segoe UI", 11, FontStyle.Bold), AutoSize = true, Location = new Point(350, 30) };
            
            btnProceed = new Button { Text = "Proceed", Location = new Point(750, 20), Size = new Size(100, 40), BackColor = Color.FromArgb(33, 150, 243), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnProceed.Click += BtnProceed_Click;

            bottomPanel.Controls.Add(lblLegend);
            bottomPanel.Controls.Add(lblTotal);
            bottomPanel.Controls.Add(btnProceed);
            this.Controls.Add(bottomPanel);

            // Center Panel
            centerPanel = new Panel { Dock = DockStyle.Fill, AutoScroll = true, Padding = new Padding(20) };
            this.Controls.Add(centerPanel);
        }

        private void LoadSeats()
        {
            try
            {
                centerPanel.SuspendLayout();
                
                // Assuming BookingManager has GetShowSeats(showId) returning List<ShowSeat>
                // For this example to compile, we use a reflection or placeholder if the method doesn't exist
                // var allSeats = BookingManager.GetShowSeats(showId);
                var allSeats = new BookingManager().GetShowSeats(showId);

                var groupedSeats = allSeats.GroupBy(s => s.SeatRow).OrderBy(g => g.Key).ToList();

                seatGrid = new TableLayoutPanel
                {
                    AutoSize = true,
                    RowCount = groupedSeats.Count,
                    ColumnCount = allSeats.Max(s => s.SeatNo) + 1, // +1 for row label
                    Padding = new Padding(10)
                };

                int rowIndex = 0;
                foreach (var group in groupedSeats)
                {
                    Label rowLabel = new Label
                    {
                        Text = group.Key,
                        TextAlign = ContentAlignment.MiddleCenter,
                        Size = new Size(30, 50),
                        Margin = new Padding(3, 3, 10, 3),
                        Font = new Font("Segoe UI", 12, FontStyle.Bold)
                    };
                    seatGrid.Controls.Add(rowLabel, 0, rowIndex);

                    foreach (var seat in group.OrderBy(s => s.SeatNo))
                    {
                        Button btnSeat = new Button
                        {
                            Text = seat.SeatNo.ToString(),
                            Size = new Size(50, 50),
                            Margin = new Padding(3),
                            Tag = seat,
                            FlatStyle = FlatStyle.Flat
                        };
                        btnSeat.FlatAppearance.BorderSize = 0;

                        if (seat.Status == "Available")
                        {
                            btnSeat.BackColor = Color.FromArgb(76, 175, 80); // Green
                            btnSeat.ForeColor = Color.White;
                            btnSeat.Enabled = true;
                            btnSeat.Click += BtnSeat_Click;
                        }
                        else
                        {
                            btnSeat.BackColor = Color.FromArgb(158, 158, 158); // Gray
                            btnSeat.ForeColor = Color.White;
                            btnSeat.Enabled = false;
                        }

                        seatGrid.Controls.Add(btnSeat, seat.SeatNo, rowIndex);
                    }
                    rowIndex++;
                }

                centerPanel.Controls.Add(seatGrid);
                centerPanel.ResumeLayout();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading seats: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSeat_Click(object sender, EventArgs e)
        {
            if (sender is Button btnSeat && btnSeat.Tag is ShowSeat seat)
            {
                if (selectedSeats.Contains(seat))
                {
                    selectedSeats.Remove(seat);
                    btnSeat.BackColor = Color.FromArgb(76, 175, 80); // Available
                    btnSeat.ForeColor = Color.White;
                }
                else
                {
                    selectedSeats.Add(seat);
                    btnSeat.BackColor = Color.FromArgb(33, 150, 243); // Selected
                    btnSeat.ForeColor = Color.White;
                }
                UpdateTotal();
            }
        }

        private void UpdateTotal()
        {
            decimal total = selectedSeats.Count * ticketPrice;
            lblTotal.Text = $"Selected: {selectedSeats.Count} seat(s) | Total: ৳{total:F2}";
        }

        private void BtnProceed_Click(object sender, EventArgs e)
        {
            if (selectedSeats.Count == 0)
            {
                MessageBox.Show("Please select at least one seat.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                List<int> selectedSeatIds = selectedSeats.Select(s => s.ShowSeatID).ToList();
                var lockedSeats = new BookingManager().LockSeats(selectedSeatIds);

                if (lockedSeats.Count < selectedSeats.Count)
                {
                    MessageBox.Show("Some seats were taken by another user. Please re-select.", "Seats Unavailable", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    selectedSeats.Clear();
                    centerPanel.Controls.Remove(seatGrid);
                    LoadSeats();
                    UpdateTotal();
                }
                else
                {
                    using var summaryForm = new BookingSummaryForm(showId, movieTitle, hallName, showDate, showTime, ticketPrice, selectedSeats);
                    if (summaryForm.ShowDialog() == DialogResult.OK)
                    {
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        new BookingManager().UnlockSeats(selectedSeatIds);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error proceeding with booking: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
