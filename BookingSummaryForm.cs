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
    public class BookingSummaryForm : Form
    {
        private int showId;
        private string movieTitle;
        private string hallName;
        private DateTime showDate;
        private TimeSpan showTime;
        private decimal ticketPrice;
        private List<ShowSeat> selectedSeats;

        private Label lblGrandTotal;
        private List<FoodOrderItem> foodItems = new List<FoodOrderItem>();
        private decimal seatTotal;
        private decimal foodTotal = 0;

        private Panel foodListPanel;

        public BookingSummaryForm(int showId, string movieTitle, string hallName, DateTime showDate, TimeSpan showTime, decimal ticketPrice, List<ShowSeat> selectedSeats)
        {
            this.showId = showId;
            this.movieTitle = movieTitle;
            this.hallName = hallName;
            this.showDate = showDate;
            this.showTime = showTime;
            this.ticketPrice = ticketPrice;
            this.selectedSeats = selectedSeats;

            seatTotal = selectedSeats.Count * ticketPrice;

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Booking Summary";
            this.Size = new Size(650, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 10);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            Label lblTitle = new Label { Text = "Booking Summary", Font = new Font("Segoe UI", 16, FontStyle.Bold), AutoSize = true, Location = new Point(20, 20) };
            this.Controls.Add(lblTitle);

            // Movie Info
            Panel pnlMovie = new Panel { Location = new Point(20, 60), Size = new Size(600, 100), BorderStyle = BorderStyle.FixedSingle };
            pnlMovie.Controls.Add(new Label { Text = "Movie: " + movieTitle, Font = new Font("Segoe UI", 12, FontStyle.Bold), Location = new Point(10, 10), AutoSize = true });
            pnlMovie.Controls.Add(new Label { Text = $"Hall: {hallName} | Date: {showDate:yyyy-MM-dd} | Time: {showTime:hh\\:mm}", Location = new Point(10, 40), AutoSize = true });
            this.Controls.Add(pnlMovie);

            // Seats Info
            Panel pnlSeats = new Panel { Location = new Point(20, 170), Size = new Size(600, 80), BorderStyle = BorderStyle.FixedSingle };
            string seatList = string.Join(", ", selectedSeats.Select(s => s.SeatRow + s.SeatNo));
            pnlSeats.Controls.Add(new Label { Text = "Selected Seats:", Font = new Font("Segoe UI", 10, FontStyle.Bold), Location = new Point(10, 10), AutoSize = true });
            pnlSeats.Controls.Add(new Label { Text = seatList, Location = new Point(10, 35), AutoSize = true, MaximumSize = new Size(580, 0) });
            pnlSeats.Controls.Add(new Label { Text = $"Tickets Subtotal: ৳{seatTotal:F2}", Font = new Font("Segoe UI", 10, FontStyle.Bold), Location = new Point(400, 35), AutoSize = true });
            this.Controls.Add(pnlSeats);

            // Food Info
            Panel pnlFood = new Panel { Location = new Point(20, 260), Size = new Size(600, 150), BorderStyle = BorderStyle.FixedSingle };
            pnlFood.Controls.Add(new Label { Text = "Food & Beverages", Font = new Font("Segoe UI", 10, FontStyle.Bold), Location = new Point(10, 10), AutoSize = true });
            
            Button btnAddFood = new Button { Text = "Add Food", Location = new Point(500, 10), Size = new Size(90, 30), BackColor = Color.FromArgb(255, 152, 0), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnAddFood.Click += BtnAddFood_Click;
            pnlFood.Controls.Add(btnAddFood);

            foodListPanel = new Panel { Location = new Point(10, 40), Size = new Size(580, 100), AutoScroll = true };
            pnlFood.Controls.Add(foodListPanel);
            this.Controls.Add(pnlFood);

            // Grand Total
            lblGrandTotal = new Label { Text = $"Grand Total: ৳{seatTotal:F2}", Font = new Font("Segoe UI", 14, FontStyle.Bold), AutoSize = true, Location = new Point(20, 430) };
            this.Controls.Add(lblGrandTotal);

            // Buttons
            Button btnProceed = new Button { Text = "Proceed to Payment", Location = new Point(450, 500), Size = new Size(170, 40), BackColor = Color.FromArgb(33, 150, 243), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnProceed.Click += BtnProceed_Click;
            
            Button btnCancel = new Button { Text = "Cancel", Location = new Point(340, 500), Size = new Size(100, 40), FlatStyle = FlatStyle.Flat };
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            this.Controls.Add(btnProceed);
            this.Controls.Add(btnCancel);
        }

        private void BtnAddFood_Click(object sender, EventArgs e)
        {
            using var foodForm = new FoodSelectionForm();
            if (foodForm.ShowDialog() == DialogResult.OK && foodForm.HasOrder)
            {
                foodItems = foodForm.SelectedItems;
                foodTotal = foodForm.FoodTotal;
                UpdateFoodPanel();
                UpdateGrandTotal();
            }
        }

        private void UpdateFoodPanel()
        {
            foodListPanel.Controls.Clear();
            int y = 0;
            foreach (var item in foodItems)
            {
                Label lblItem = new Label
                {
                    Text = $"Item ID: {item.FoodItemID} | Qty: {item.Quantity} | Subtotal: ৳{item.Subtotal:F2}",
                    Location = new Point(0, y),
                    AutoSize = true
                };
                foodListPanel.Controls.Add(lblItem);
                y += 25;
            }
            if (foodItems.Count > 0)
            {
                Label lblFoodTotal = new Label { Text = $"Food Subtotal: ৳{foodTotal:F2}", Font = new Font("Segoe UI", 10, FontStyle.Bold), Location = new Point(400, y), AutoSize = true };
                foodListPanel.Controls.Add(lblFoodTotal);
            }
        }

        private void UpdateGrandTotal()
        {
            lblGrandTotal.Text = $"Grand Total: ৳{(seatTotal + foodTotal):F2}";
        }

        private void BtnProceed_Click(object sender, EventArgs e)
        {
            decimal grandTotal = seatTotal + foodTotal;
            using var paymentForm = new PaymentForm(grandTotal);
            if (paymentForm.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string paymentMethod = paymentForm.SelectedPaymentMethod;
                    int userId = AuthManager.CurrentUser?.UserID ?? 1; // Fallback to 1 if null for testing

                    FoodOrder foodOrder = null;
                    if (foodItems != null && foodItems.Count > 0)
                    {
                        foodOrder = new FoodOrder
                        {
                            TotalAmount = foodItems.Sum(f => f.Subtotal)
                        };
                    }

                    int bookingId = new BookingManager().CompleteBooking(
                        userId, showId,
                        selectedSeats.Select(s => s.ShowSeatID).ToList(),
                        paymentMethod, grandTotal,
                        foodOrder, foodItems);

                    using var ticketForm = new TicketForm(bookingId, movieTitle, hallName, showDate, showTime, selectedSeats, grandTotal, paymentMethod);
                    ticketForm.ShowDialog();

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Booking Failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
