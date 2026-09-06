using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using CinemaHallSystem.BLL;
using CinemaHallSystem.Models;

namespace CinemaHallSystem.Forms
{
    public class CustomerHomeForm : Form
    {
        private AppUser currentUser;
        private FlowLayoutPanel flowPanel;
        private ComboBox cmbGenre;
        private MovieManager movieManager = new MovieManager();
        private ShowManager showManager = new ShowManager();

        public CustomerHomeForm(AppUser user = null)
        {
            currentUser = user;
            InitializeComponent();
            LoadMovies();
        }

        private void InitializeComponent()
        {
            this.Text = "Now Showing - Cinema Hall Management System";
            this.Size = new Size(1100, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 10);
            this.FormClosing += (s, e) =>
            {
                if (this.TopLevel) Application.Exit();
            };

            var topPanel = new Panel { Dock = DockStyle.Top, Height = 80, BackColor = Color.LightGray };
            this.Controls.Add(topPanel);

            var lblTitle = new Label
            {
                Text = "Now Showing",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(20, 20)
            };
            topPanel.Controls.Add(lblTitle);

            cmbGenre = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(300, 30),
                Width = 200
            };
            cmbGenre.Items.AddRange(new[] { "All Genres", "Action", "Sci-Fi", "Thriller", "Comedy", "Drama", "Horror" });
            cmbGenre.SelectedIndex = 0;
            cmbGenre.SelectedIndexChanged += (s, e) => LoadMovies();
            topPanel.Controls.Add(cmbGenre);

            var btnLogout = new Button
            {
                Text = "LogOut",
                Location = new Point(this.Width - 120, 25),
                Width = 80,
                Height = 30,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnLogout.Click += (s, e) =>
            {
                if (this.TopLevel)
                {
                    AuthManager.Logout();
                    this.Hide();
                    new LoginForm().Show();
                }
            };
            topPanel.Controls.Add(btnLogout);

            flowPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(20)
            };
            this.Controls.Add(flowPanel);
            flowPanel.BringToFront();
        }

        private void LoadMovies()
        {
            if (flowPanel == null) return;
            flowPanel.Controls.Clear();
            string selectedGenre = cmbGenre?.SelectedItem?.ToString() ?? "All Genres";

            try
            {
                var movies = movieManager.GetAllMovies();
                if (movies == null) return;

                if (!string.IsNullOrEmpty(selectedGenre) && selectedGenre != "All Genres")
                {
                    movies = movies.Where(m => m != null && m.Genre != null && m.Genre.IndexOf(selectedGenre, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
                }

                foreach (var movie in movies)
                {
                    if (movie == null) continue;

                    var card = new Panel
                    {
                        Size = new Size(280, 340),
                        BackColor = Color.White,
                        BorderStyle = BorderStyle.FixedSingle,
                        Margin = new Padding(10)
                    };

                    var lblMTitle = new Label { Text = movie.Title ?? "Untitled", Font = new Font("Segoe UI", 13, FontStyle.Bold), AutoSize = false, Size = new Size(260, 55), Location = new Point(10, 10), TextAlign = ContentAlignment.TopLeft };
                    var lblMGenre = new Label { Text = $"Genre: {movie.Genre ?? "N/A"}", AutoSize = true, Location = new Point(10, 70) };
                    var lblMLang = new Label { Text = $"Language: {movie.Language ?? "N/A"}", AutoSize = true, Location = new Point(10, 100) };
                    var lblMDur = new Label { Text = $"Duration: {movie.DurationMinutes} mins", AutoSize = true, Location = new Point(10, 130) };
                    var lblMAge = new Label { Text = $"Rating: {movie.AgeRating ?? "N/A"}", AutoSize = true, Location = new Point(10, 160) };

                    var btnViewShows = new Button { Text = "View Shows", Size = new Size(240, 40), Location = new Point(20, 280), BackColor = Color.DarkBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 11, FontStyle.Bold) };
                    btnViewShows.Click += (s, e) => ShowAvailableShows(movie);

                    card.Controls.AddRange(new Control[] { lblMTitle, lblMGenre, lblMLang, lblMDur, lblMAge, btnViewShows });
                    flowPanel.Controls.Add(card);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading movies: {ex.Message}", "Error loading movies", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowAvailableShows(Movie movie)
        {
            try
            {
                var shows = showManager.GetAllShows().Where(sh => sh.MovieID == movie.MovieID).ToList();

                if (shows.Count == 0)
                {
                    MessageBox.Show("No shows available for this movie.", "No Shows", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                using var showDialog = new Form
                {
                    Text = $"Shows for {movie.Title}",
                    Size = new Size(700, 400),
                    StartPosition = FormStartPosition.CenterParent,
                    Font = new Font("Segoe UI", 10)
                };

                var dgvShows = new DataGridView
                {
                    Dock = DockStyle.Fill,
                    ReadOnly = true,
                    SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                    AllowUserToAddRows = false,
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                    DataSource = shows.Select(s => new {
                        s.ShowID,
                        Date = s.ShowDate.ToShortDateString(),
                        Time = s.ShowTime.ToString(@"hh\:mm"),
                        Hall = s.HallName,
                        s.HallID,
                        Price = $"৳{s.TicketPrice:F2}"
                    }).ToList()
                };

                // Hide ShowID and HallID columns
                dgvShows.DataBindingComplete += (ds, de) =>
                {
                    if (dgvShows.Columns.Contains("ShowID")) dgvShows.Columns["ShowID"].Visible = false;
                    if (dgvShows.Columns.Contains("HallID")) dgvShows.Columns["HallID"].Visible = false;
                };

                var bottomPanel = new Panel { Dock = DockStyle.Bottom, Height = 60 };
                var btnSelect = new Button
                {
                    Text = "🎬 Select Show & Pick Seats",
                    Size = new Size(250, 40),
                    Location = new Point(220, 10),
                    BackColor = Color.FromArgb(33, 150, 243),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 11, FontStyle.Bold)
                };
                btnSelect.Click += (s, e) =>
                {
                    if (dgvShows.SelectedRows.Count > 0)
                    {
                        var row = dgvShows.SelectedRows[0];
                        int showId = (int)row.Cells["ShowID"].Value;

                        // Find the matching Show object
                        var selectedShow = shows.First(sh => sh.ShowID == showId);

                        showDialog.Close();

                        // Open BookingWizardForm — handles everything in ONE window
                        using var wizardForm = new BookingWizardForm(
                            selectedShow.ShowID,
                            movie.Title,
                            selectedShow.HallName ?? $"Hall {selectedShow.HallID}",
                            selectedShow.ShowDate,
                            selectedShow.ShowTime,
                            selectedShow.TicketPrice
                        );
                        wizardForm.ShowDialog();

                        // Refresh movie list after booking
                        LoadMovies();
                    }
                    else
                    {
                        MessageBox.Show("Please select a show first.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                };

                // Double-click also selects
                dgvShows.CellDoubleClick += (s, e) =>
                {
                    if (e.RowIndex >= 0)
                    {
                        btnSelect.PerformClick();
                    }
                };

                bottomPanel.Controls.Add(btnSelect);
                showDialog.Controls.Add(dgvShows);
                showDialog.Controls.Add(bottomPanel);
                showDialog.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error loading shows", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
