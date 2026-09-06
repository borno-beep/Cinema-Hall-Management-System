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
    /// <summary>
    /// All-in-one booking wizard: Seat Selection → Food → Summary & Payment → Ticket.
    /// Everything stays in ONE window using panel swapping.
    /// </summary>
    public class BookingWizardForm : Form
    {
        // Show info
        private int showId;
        private string movieTitle;
        private string hallName;
        private DateTime showDate;
        private TimeSpan showTime;
        private decimal ticketPrice;

        // Managers
        private BookingManager bookingManager = new BookingManager();
        private FoodManager foodManager = new FoodManager();

        // State
        private List<ShowSeat> selectedSeats = new List<ShowSeat>();
        private List<FoodOrderItem> foodItems = new List<FoodOrderItem>();
        private decimal foodTotal = 0;
        private int currentStep = 1;
        private int bookingId = 0;
        private string paymentMethod = "Cash";

        // Layout
        private Panel headerPanel;
        private Panel contentPanel;
        private Panel footerPanel;
        private Label lblStepTitle;
        private Label[] stepLabels;
        private Button btnBack;
        private Button btnNext;

        public BookingWizardForm(int showId, string movieTitle, string hallName,
            DateTime showDate, TimeSpan showTime, decimal ticketPrice)
        {
            this.showId = showId;
            this.movieTitle = movieTitle;
            this.hallName = hallName;
            this.showDate = showDate;
            this.showTime = showTime;
            this.ticketPrice = ticketPrice;
            InitializeComponent();
            ShowStep(1);
        }

        private void InitializeComponent()
        {
            this.Text = $"Book: {movieTitle}";
            this.Size = new Size(950, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 10);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // === HEADER — step indicator ===
            headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.FromArgb(33, 37, 41)
            };
            this.Controls.Add(headerPanel);

            lblStepTitle = new Label
            {
                Text = "Step 1: Select Seats",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(20, 10)
            };
            headerPanel.Controls.Add(lblStepTitle);

            // Step indicators
            string[] stepNames = { "1. Seats", "2. Food", "3. Pay", "4. Ticket" };
            stepLabels = new Label[4];
            for (int i = 0; i < 4; i++)
            {
                stepLabels[i] = new Label
                {
                    Text = stepNames[i],
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    ForeColor = i == 0 ? Color.FromArgb(76, 175, 80) : Color.Gray,
                    AutoSize = true,
                    Location = new Point(20 + i * 220, 50)
                };
                headerPanel.Controls.Add(stepLabels[i]);
            }

            // === FOOTER — navigation buttons ===
            footerPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                BackColor = Color.FromArgb(245, 245, 245)
            };
            this.Controls.Add(footerPanel);

            btnBack = new Button
            {
                Text = "← Back",
                Size = new Size(120, 40),
                Location = new Point(20, 10),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11),
                Visible = false
            };
            btnBack.Click += (s, e) => GoBack();
            footerPanel.Controls.Add(btnBack);

            btnNext = new Button
            {
                Text = "Proceed →",
                Size = new Size(160, 40),
                Location = new Point(750, 10),
                BackColor = Color.FromArgb(33, 150, 243),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };
            btnNext.Click += (s, e) => GoNext();
            footerPanel.Controls.Add(btnNext);

            contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };
            this.Controls.Add(contentPanel);
            contentPanel.BringToFront();
        }

        // =========================================================
        // STEP NAVIGATION
        // =========================================================
        private void ShowStep(int step)
        {
            currentStep = step;
            contentPanel.Controls.Clear();

            // Update header
            string[] titles = { "", "Step 1: Select Seats", "Step 2: Add Food (Optional)", "Step 3: Review & Pay", "Booking Confirmed!" };
            lblStepTitle.Text = titles[step];

            for (int i = 0; i < 4; i++)
            {
                if (i < step - 1) stepLabels[i].ForeColor = Color.FromArgb(76, 175, 80); // completed = green
                else if (i == step - 1) stepLabels[i].ForeColor = Color.White; // current = white
                else stepLabels[i].ForeColor = Color.Gray; // upcoming = gray
            }

            switch (step)
            {
                case 1: BuildSeatSelection(); break;
                case 2: BuildFoodSelection(); break;
                case 3: BuildSummaryAndPayment(); break;
                case 4: BuildTicket(); break;
            }

            // Button visibility
            btnBack.Visible = step == 2 || step == 3;
            btnNext.Visible = step != 4;

            if (step == 1) btnNext.Text = $"Proceed → ({selectedSeats.Count} seats)";
            else if (step == 2) btnNext.Text = "Proceed → Summary";
            else if (step == 3) { btnNext.Text = "💳 Confirm & Pay"; btnNext.BackColor = Color.FromArgb(76, 175, 80); }
        }

        private void GoNext()
        {
            if (currentStep == 1)
            {
                if (selectedSeats.Count == 0)
                {
                    MessageBox.Show("Please select at least one seat.", "No Seats", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                // Lock seats
                var seatIds = selectedSeats.Select(s => s.ShowSeatID).ToList();
                var locked = bookingManager.LockSeats(seatIds);
                if (locked.Count < seatIds.Count)
                {
                    MessageBox.Show("Some seats were just taken by another user. Please select again.", "Seats Unavailable", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    selectedSeats.Clear();
                    ShowStep(1);
                    return;
                }
                ShowStep(2);
            }
            else if (currentStep == 2)
            {
                ShowStep(3);
            }
            else if (currentStep == 3)
            {
                CompleteBooking();
            }
        }

        private void GoBack()
        {
            if (currentStep == 2)
            {
                ShowStep(1);
            }
            else if (currentStep == 3)
            {
                ShowStep(2);
            }
        }

        // =========================================================
        // STEP 1: SEAT SELECTION
        // =========================================================
        private void BuildSeatSelection()
        {
            // Info bar
            var infoPanel = new Panel { Dock = DockStyle.Top, Height = 50, BackColor = Color.FromArgb(240, 240, 240) };
            var lblInfo = new Label
            {
                Text = $"🎬 {movieTitle}   |   🏛 {hallName}   |   📅 {showDate:dd MMM yyyy}   |   ⏰ {showTime:hh\\:mm}   |   💰 ৳{ticketPrice:F0}/seat",
                Font = new Font("Segoe UI", 11),
                AutoSize = true,
                Location = new Point(15, 15)
            };
            infoPanel.Controls.Add(lblInfo);
            contentPanel.Controls.Add(infoPanel);

            // Legend
            var legendPanel = new Panel { Dock = DockStyle.Top, Height = 40 };
            AddLegendItem(legendPanel, "Available", Color.FromArgb(76, 175, 80), 20);
            AddLegendItem(legendPanel, "Selected", Color.FromArgb(33, 150, 243), 170);
            AddLegendItem(legendPanel, "Booked", Color.FromArgb(158, 158, 158), 310);

            var lblTotal = new Label
            {
                Text = $"Selected: {selectedSeats.Count} | Total: ৳{selectedSeats.Count * ticketPrice:F0}",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(550, 10),
                Name = "lblSeatTotal"
            };
            legendPanel.Controls.Add(lblTotal);
            contentPanel.Controls.Add(legendPanel);

            // Seat grid
            var gridPanel = new Panel { Dock = DockStyle.Fill, AutoScroll = true, Padding = new Padding(20) };
            contentPanel.Controls.Add(gridPanel);
            gridPanel.BringToFront();

            try
            {
                var showSeats = bookingManager.GetShowSeats(showId);
                var grouped = showSeats.GroupBy(s => s.SeatRow).OrderBy(g => g.Key);

                gridPanel.SuspendLayout();
                int yPos = 10;
                foreach (var row in grouped)
                {
                    // Row label
                    var rowLabel = new Label
                    {
                        Text = row.Key,
                        Font = new Font("Segoe UI", 12, FontStyle.Bold),
                        Size = new Size(30, 45),
                        Location = new Point(5, yPos + 8),
                        TextAlign = ContentAlignment.MiddleCenter
                    };
                    gridPanel.Controls.Add(rowLabel);

                    int xPos = 45;
                    foreach (var seat in row.OrderBy(s => s.SeatNo))
                    {
                        var btn = new Button
                        {
                            Text = seat.SeatNo.ToString(),
                            Size = new Size(45, 45),
                            Location = new Point(xPos, yPos),
                            FlatStyle = FlatStyle.Flat,
                            Font = new Font("Segoe UI", 9, FontStyle.Bold),
                            Tag = seat,
                            Cursor = Cursors.Hand
                        };
                        btn.FlatAppearance.BorderSize = 1;

                        bool isSelected = selectedSeats.Any(s => s.ShowSeatID == seat.ShowSeatID);

                        if (seat.Status != "Available" && !isSelected)
                        {
                            btn.BackColor = Color.FromArgb(158, 158, 158);
                            btn.ForeColor = Color.White;
                            btn.Enabled = false;
                        }
                        else if (isSelected)
                        {
                            btn.BackColor = Color.FromArgb(33, 150, 243);
                            btn.ForeColor = Color.White;
                            btn.Click += SeatBtn_Click;
                        }
                        else
                        {
                            btn.BackColor = Color.FromArgb(76, 175, 80);
                            btn.ForeColor = Color.White;
                            btn.Click += SeatBtn_Click;
                        }

                        gridPanel.Controls.Add(btn);
                        xPos += 50;
                    }
                    yPos += 55;
                }
                gridPanel.ResumeLayout();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error loading seats", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddLegendItem(Panel panel, string text, Color color, int xPos)
        {
            var box = new Panel { BackColor = color, Size = new Size(20, 20), Location = new Point(xPos, 10) };
            var lbl = new Label { Text = text, AutoSize = true, Location = new Point(xPos + 25, 10) };
            panel.Controls.Add(box);
            panel.Controls.Add(lbl);
        }

        private void SeatBtn_Click(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            var seat = (ShowSeat)btn.Tag;

            if (selectedSeats.Any(s => s.ShowSeatID == seat.ShowSeatID))
            {
                selectedSeats.RemoveAll(s => s.ShowSeatID == seat.ShowSeatID);
                btn.BackColor = Color.FromArgb(76, 175, 80); // back to green
            }
            else
            {
                selectedSeats.Add(seat);
                btn.BackColor = Color.FromArgb(33, 150, 243); // blue = selected
            }

            // Update total label
            var lblTotal = contentPanel.Controls.Find("lblSeatTotal", true).FirstOrDefault() as Label;
            if (lblTotal != null)
                lblTotal.Text = $"Selected: {selectedSeats.Count} | Total: ৳{selectedSeats.Count * ticketPrice:F0}";

            btnNext.Text = $"Proceed → ({selectedSeats.Count} seats)";
        }

        // =========================================================
        // STEP 2: FOOD SELECTION
        // =========================================================
        private void BuildFoodSelection()
        {
            var lblHint = new Label
            {
                Text = "🍿 Add snacks & drinks to your order (optional — click Proceed to skip)",
                Font = new Font("Segoe UI", 11),
                AutoSize = false,
                Height = 35,
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(15, 0, 0, 0),
                BackColor = Color.FromArgb(245, 245, 250)
            };
            contentPanel.Controls.Add(lblHint);

            // Food total label
            var totalPanel = new Panel { Dock = DockStyle.Bottom, Height = 55, BackColor = Color.FromArgb(245, 245, 245) };
            var lblFoodTotal = new Label
            {
                Text = $"Food Total: ৳{foodTotal:F0}",
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(20, 15),
                Name = "lblFoodTotal"
            };
            totalPanel.Controls.Add(lblFoodTotal);
            contentPanel.Controls.Add(totalPanel);

            var foodFlowPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(15),
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true
            };
            contentPanel.Controls.Add(foodFlowPanel);
            foodFlowPanel.BringToFront();

            try
            {
                var allFood = foodManager.GetAllFoodItems();
                foreach (var item in allFood)
                {
                    var card = new Panel
                    {
                        Size = new Size(270, 140),
                        BackColor = Color.FromArgb(250, 250, 250),
                        BorderStyle = BorderStyle.FixedSingle,
                        Margin = new Padding(8)
                    };

                    var lblName = new Label { Text = item.Name, Font = new Font("Segoe UI", 12, FontStyle.Bold), AutoSize = false, Size = new Size(250, 30), Location = new Point(10, 10) };
                    var lblPrice = new Label { Text = $"৳{item.Price:F0}", Font = new Font("Segoe UI", 11, FontStyle.Bold), ForeColor = Color.FromArgb(33, 150, 243), AutoSize = true, Location = new Point(10, 42) };
                    var lblCat = new Label { Text = item.Category, Font = new Font("Segoe UI", 9), ForeColor = Color.Gray, AutoSize = true, Location = new Point(120, 45) };

                    var lblQty = new Label { Text = "Qty:", Font = new Font("Segoe UI", 10), AutoSize = true, Location = new Point(10, 82) };
                    var nud = new NumericUpDown
                    {
                        Minimum = 0,
                        Maximum = 10,
                        Size = new Size(70, 30),
                        Location = new Point(55, 78),
                        Font = new Font("Segoe UI", 11),
                        Tag = item
                    };

                    // Pre-fill if already selected
                    var existing = foodItems.FirstOrDefault(f => f.FoodItemID == item.FoodItemID);
                    if (existing != null) nud.Value = existing.Quantity;

                    nud.ValueChanged += (s, ev) =>
                    {
                        var fi = (FoodItem)nud.Tag;
                        foodItems.RemoveAll(f => f.FoodItemID == fi.FoodItemID);
                        if (nud.Value > 0)
                        {
                            foodItems.Add(new FoodOrderItem
                            {
                                FoodItemID = fi.FoodItemID,
                                FoodItemName = fi.Name,
                                Quantity = (int)nud.Value,
                                Subtotal = fi.Price * (int)nud.Value
                            });
                        }
                        foodTotal = foodItems.Sum(f => f.Subtotal);
                        var lbl = contentPanel.Controls.Find("lblFoodTotal", true).FirstOrDefault() as Label;
                        if (lbl != null) lbl.Text = $"Food Total: ৳{foodTotal:F0}";
                    };


                    card.Controls.AddRange(new Control[] { lblName, lblPrice, lblCat, lblQty, nud });
                    foodFlowPanel.Controls.Add(card);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error loading food items", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =========================================================
        // STEP 3: SUMMARY & PAYMENT
        // =========================================================
        private void BuildSummaryAndPayment()
        {
            var scrollPanel = new Panel { Dock = DockStyle.Fill, AutoScroll = true, Padding = new Padding(30) };
            contentPanel.Controls.Add(scrollPanel);

            int y = 10;

            // Movie info
            AddSummaryRow(scrollPanel, "🎬 Movie", movieTitle, ref y, true);
            AddSummaryRow(scrollPanel, "🏛 Hall", hallName, ref y, false);
            AddSummaryRow(scrollPanel, "📅 Date", showDate.ToString("dd MMM yyyy"), ref y, false);
            AddSummaryRow(scrollPanel, "⏰ Time", showTime.ToString(@"hh\:mm"), ref y, false);

            y += 15;

            // Seats
            string seatList = string.Join(", ", selectedSeats.Select(s => $"{s.SeatRow}{s.SeatNo}"));
            AddSummaryRow(scrollPanel, "💺 Seats", $"{seatList} ({selectedSeats.Count} seats)", ref y, true);
            decimal seatTotal = selectedSeats.Count * ticketPrice;
            AddSummaryRow(scrollPanel, "Ticket Subtotal", $"৳{seatTotal:F0}", ref y, false);

            y += 10;

            // Food
            if (foodItems.Count > 0)
            {
                AddSummaryRow(scrollPanel, "🍿 Food Order", "", ref y, true);
                foreach (var fi in foodItems)
                {
                    AddSummaryRow(scrollPanel, $"   {fi.FoodItemName} x{fi.Quantity}", $"৳{fi.Subtotal:F0}", ref y, false);
                }
                AddSummaryRow(scrollPanel, "Food Subtotal", $"৳{foodTotal:F0}", ref y, false);
            }

            y += 15;

            // Grand total
            decimal grandTotal = seatTotal + foodTotal;
            var lblGrand = new Label
            {
                Text = $"Grand Total: ৳{grandTotal:F0}",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(211, 47, 47),
                AutoSize = true,
                Location = new Point(30, y)
            };
            scrollPanel.Controls.Add(lblGrand);
            y += 50;

            // Payment method
            var gbPay = new GroupBox
            {
                Text = "Select Payment Method",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Location = new Point(30, y),
                Size = new Size(400, 130)
            };

            var rbCash = new RadioButton { Text = "💵 Cash", Location = new Point(20, 30), Checked = true, AutoSize = true, Font = new Font("Segoe UI", 11) };
            var rbCard = new RadioButton { Text = "💳 Credit/Debit Card", Location = new Point(20, 60), AutoSize = true, Font = new Font("Segoe UI", 11) };
            var rbMobile = new RadioButton { Text = "📱 Mobile Banking (bKash/Nagad)", Location = new Point(20, 90), AutoSize = true, Font = new Font("Segoe UI", 11) };

            rbCash.CheckedChanged += (s, e) => { if (rbCash.Checked) paymentMethod = "Cash"; };
            rbCard.CheckedChanged += (s, e) => { if (rbCard.Checked) paymentMethod = "Card"; };
            rbMobile.CheckedChanged += (s, e) => { if (rbMobile.Checked) paymentMethod = "MobileBanking"; };

            gbPay.Controls.AddRange(new Control[] { rbCash, rbCard, rbMobile });
            scrollPanel.Controls.Add(gbPay);
        }

        private void AddSummaryRow(Panel parent, string label, string value, ref int y, bool isHeader)
        {
            var lblLabel = new Label
            {
                Text = label,
                Font = new Font("Segoe UI", isHeader ? 13 : 11, isHeader ? FontStyle.Bold : FontStyle.Regular),
                AutoSize = true,
                Location = new Point(30, y)
            };
            parent.Controls.Add(lblLabel);

            if (!string.IsNullOrEmpty(value))
            {
                var lblValue = new Label
                {
                    Text = value,
                    Font = new Font("Segoe UI", isHeader ? 13 : 11, isHeader ? FontStyle.Bold : FontStyle.Regular),
                    ForeColor = isHeader ? Color.FromArgb(33, 37, 41) : Color.FromArgb(80, 80, 80),
                    AutoSize = true,
                    Location = new Point(350, y)
                };
                parent.Controls.Add(lblValue);
            }
            y += isHeader ? 35 : 28;
        }

        // =========================================================
        // COMPLETE BOOKING
        // =========================================================
        private void CompleteBooking()
        {
            try
            {
                int userId = AuthManager.CurrentUser?.UserID ?? 1;
                decimal grandTotal = (selectedSeats.Count * ticketPrice) + foodTotal;

                FoodOrder foodOrder = null;
                if (foodItems.Count > 0)
                {
                    foodOrder = new FoodOrder { TotalAmount = foodTotal };
                }

                bookingId = bookingManager.CompleteBooking(
                    userId, showId,
                    selectedSeats.Select(s => s.ShowSeatID).ToList(),
                    paymentMethod, grandTotal,
                    foodOrder, foodItems.Count > 0 ? foodItems : null
                );

                ShowStep(4);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Booking Failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =========================================================
        // STEP 4: TICKET / RECEIPT
        // =========================================================
        private void BuildTicket()
        {
            headerPanel.BackColor = Color.FromArgb(76, 175, 80);

            var scrollPanel = new Panel { Dock = DockStyle.Fill, AutoScroll = true, BackColor = Color.White };
            contentPanel.Controls.Add(scrollPanel);

            // Ticket card
            var ticketCard = new Panel
            {
                Size = new Size(500, 500),
                Location = new Point(200, 20),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            scrollPanel.Controls.Add(ticketCard);

            int y = 20;

            AddTicketLine(ticketCard, "🎬 CINEMA HALL", new Font("Segoe UI", 18, FontStyle.Bold), Color.FromArgb(33, 37, 41), ref y, true);
            y += 5;
            AddTicketLine(ticketCard, "════════════════════════════", new Font("Segoe UI", 10), Color.Gray, ref y, true);

            AddTicketLine(ticketCard, $"Booking ID: #{bookingId}", new Font("Segoe UI", 13, FontStyle.Bold), Color.FromArgb(33, 150, 243), ref y, true);
            y += 10;

            AddTicketLine(ticketCard, $"Movie:   {movieTitle}", new Font("Segoe UI", 12), Color.Black, ref y, false);
            AddTicketLine(ticketCard, $"Hall:      {hallName}", new Font("Segoe UI", 12), Color.Black, ref y, false);
            AddTicketLine(ticketCard, $"Date:     {showDate:dd MMM yyyy}", new Font("Segoe UI", 12), Color.Black, ref y, false);
            AddTicketLine(ticketCard, $"Time:    {showTime:hh\\:mm}", new Font("Segoe UI", 12), Color.Black, ref y, false);
            y += 5;

            string seatList = string.Join(", ", selectedSeats.Select(s => $"{s.SeatRow}{s.SeatNo}"));
            AddTicketLine(ticketCard, $"Seats:   {seatList}", new Font("Segoe UI", 12, FontStyle.Bold), Color.Black, ref y, false);
            y += 5;

            AddTicketLine(ticketCard, "════════════════════════════", new Font("Segoe UI", 10), Color.Gray, ref y, true);

            decimal grandTotal = (selectedSeats.Count * ticketPrice) + foodTotal;
            AddTicketLine(ticketCard, $"Amount Paid: ৳{grandTotal:F0}", new Font("Segoe UI", 14, FontStyle.Bold), Color.FromArgb(211, 47, 47), ref y, true);
            AddTicketLine(ticketCard, $"Payment: {paymentMethod}", new Font("Segoe UI", 11), Color.Gray, ref y, true);

            y += 10;
            AddTicketLine(ticketCard, "════════════════════════════", new Font("Segoe UI", 10), Color.Gray, ref y, true);
            AddTicketLine(ticketCard, "Thank you for your purchase! 🎉", new Font("Segoe UI", 12, FontStyle.Italic), Color.FromArgb(76, 175, 80), ref y, true);

            ticketCard.Height = y + 20;

            // Close button
            var btnClose = new Button
            {
                Text = "✓ Done",
                Size = new Size(150, 45),
                Location = new Point(380, 540),
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 13, FontStyle.Bold)
            };
            btnClose.Click += (s, e) =>
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            };
            scrollPanel.Controls.Add(btnClose);

            // Hide back button, hide next button
            btnBack.Visible = false;
            btnNext.Visible = false;
        }

        private void AddTicketLine(Panel parent, string text, Font font, Color color, ref int y, bool center)
        {
            var lbl = new Label
            {
                Text = text,
                Font = font,
                ForeColor = color,
                AutoSize = true,
                Location = new Point(center ? 0 : 30, y)
            };
            if (center)
            {
                lbl.AutoSize = false;
                lbl.Size = new Size(parent.Width - 10, 35);
                lbl.TextAlign = ContentAlignment.MiddleCenter;
            }
            parent.Controls.Add(lbl);
            y += font.Height + 10;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // If closing before step 4 and seats are locked, unlock them
            if (currentStep > 1 && currentStep < 4 && selectedSeats.Count > 0)
            {
                bookingManager.UnlockSeats(selectedSeats.Select(s => s.ShowSeatID).ToList());
            }
            base.OnFormClosing(e);
        }
    }
}
