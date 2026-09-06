using System;
using System.Drawing;
using System.Windows.Forms;
using CinemaHallSystem.BLL;
using CinemaHallSystem.Models;

namespace CinemaHallSystem.Forms
{
    public class AdminDashboardForm : Form
    {
        private Panel sidePanel;
        private Panel mainPanel;
        private AppUser currentUser;

        public AdminDashboardForm(AppUser user = null)
        {
            currentUser = user;
            InitializeComponent();
            LoadDashboard();
        }

        private void InitializeComponent()
        {
            this.Text = "Admin Dashboard - Cinema Hall Management System";
            this.Size = new Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Normal;
            this.Font = new Font("Segoe UI", 10);
            this.FormClosing += (s, e) => Application.Exit();

            sidePanel = new Panel
            {
                Width = 200,
                Dock = DockStyle.Left,
                BackColor = Color.FromArgb(33, 37, 41)
            };
            this.Controls.Add(sidePanel);

            var lblTitle = new Label
            {
                Text = "ADMIN PANEL",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 60,
                TextAlign = ContentAlignment.MiddleCenter
            };
            sidePanel.Controls.Add(lblTitle);

            string[] btnNames = { "🚪 Logout", "📈 Reports", "📋 Bookings", "🍿 Food Items", "📅 Shows", "🏛 Halls", "🎬 Movies", "📊 Dashboard" };
            foreach (var name in btnNames)
            {
                var btn = new Button
                {
                    Text = name,
                    Dock = DockStyle.Top,
                    Height = 50,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Padding = new Padding(10, 0, 0, 0)
                };
                btn.FlatAppearance.BorderSize = 0;
                btn.Click += SidebarButton_Click;
                sidePanel.Controls.Add(btn);
            }

            mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };
            this.Controls.Add(mainPanel);
            mainPanel.BringToFront();
        }

        private void LoadDashboard()
        {
            var pnl = new Panel { Dock = DockStyle.Fill, AutoScroll = true, BackColor = Color.FromArgb(245, 247, 250), Padding = new Padding(25) };

            var lblHeader = new Label
            {
                Text = "📊 Dashboard Overview",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 37, 41),
                AutoSize = true,
                Location = new Point(25, 20)
            };
            pnl.Controls.Add(lblHeader);

            var lblSub = new Label
            {
                Text = "Live operational performance and revenue metrics for today. Click 'Reports' for custom date ranges.",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(108, 117, 125),
                AutoSize = true,
                Location = new Point(25, 55)
            };
            pnl.Controls.Add(lblSub);

            try
            {
                var dash = new DashboardManager();
                decimal revenue = dash.GetTodayRevenue();
                int bookings = dash.GetTodayBookingCount();
                int movies = dash.GetActiveMovieCount();
                double occupancy = dash.GetTodayOccupancyRate();
                int customers = dash.GetTotalCustomers();

                var flow = new FlowLayoutPanel
                {
                    Location = new Point(20, 90),
                    Size = new Size(950, 400),
                    FlowDirection = FlowDirection.LeftToRight,
                    AutoScroll = false,
                    WrapContents = true
                };

                AddStatCard(flow, "💰 Today's Revenue", $"৳{revenue:N2}", Color.FromArgb(40, 167, 69));
                AddStatCard(flow, "🎟 Today's Bookings", $"{bookings} confirmed", Color.FromArgb(0, 123, 255));
                AddStatCard(flow, "🎬 Active Movies", $"{movies} scheduled", Color.FromArgb(111, 66, 193));
                AddStatCard(flow, "💺 Today's Occupancy", $"{occupancy:F1}%", Color.FromArgb(253, 126, 20));
                AddStatCard(flow, "👥 Total Customers", $"{customers} registered", Color.FromArgb(23, 162, 184));

                pnl.Controls.Add(flow);
            }
            catch (Exception ex)
            {
                pnl.Controls.Add(new Label { Text = $"Error loading stats: {ex.Message}", Location = new Point(25, 90), AutoSize = true, ForeColor = Color.Red });
            }

            LoadFormInPanel(pnl);
        }

        private void AddStatCard(FlowLayoutPanel parent, string title, string value, Color accentColor)
        {
            var card = new Panel
            {
                Size = new Size(280, 110),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(10)
            };

            var bar = new Panel { Dock = DockStyle.Left, Width = 6, BackColor = accentColor };
            card.Controls.Add(bar);

            var lblTitle = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                ForeColor = Color.FromArgb(108, 117, 125),
                AutoSize = true,
                Location = new Point(18, 16)
            };
            card.Controls.Add(lblTitle);

            var lblValue = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 37, 41),
                AutoSize = true,
                Location = new Point(18, 48)
            };
            card.Controls.Add(lblValue);

            parent.Controls.Add(card);
        }

        private void SidebarButton_Click(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            string text = btn.Text;

            if (text.Contains("Dashboard")) LoadDashboard();
            else if (text.Contains("Movies")) LoadFormInPanel(new MovieManagementForm());
            else if (text.Contains("Halls")) LoadFormInPanel(new HallManagementForm());
            else if (text.Contains("Shows")) LoadFormInPanel(new ShowManagementForm());
            else if (text.Contains("Food Items")) LoadFormInPanel(new FoodSelectionForm());
            else if (text.Contains("Bookings")) LoadFormInPanel(new BookingHistoryForm());
            else if (text.Contains("Reports")) LoadFormInPanel(new ReportsForm());
            else if (text.Contains("Logout"))
            {
                AuthManager.Logout();
                this.Hide();
                new LoginForm().Show();
            }
        }

        private void LoadFormInPanel(Form form)
        {
            mainPanel.Controls.Clear();
            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;
            mainPanel.Controls.Add(form);
            form.Show();
        }

        private void LoadFormInPanel(Panel pnl)
        {
            mainPanel.Controls.Clear();
            mainPanel.Controls.Add(pnl);
        }
    }
}
