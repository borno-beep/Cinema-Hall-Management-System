using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using CinemaHallSystem.BLL;
using CinemaHallSystem.Models;

namespace CinemaHallSystem.Forms
{
    public class ShowManagementForm : Form
    {
        private DataGridView dgvShows;
        private ShowManager showManager = new ShowManager();
        private MovieManager movieManager = new MovieManager();
        private HallManager hallManager = new HallManager();

        public ShowManagementForm()
        {
            InitializeComponent();
            LoadShows();
        }

        private void InitializeComponent()
        {
            this.Text = "Show Management";
            this.Font = new Font("Segoe UI", 10);
            this.Dock = DockStyle.Fill;

            var topPanel = new Panel { Dock = DockStyle.Top, Height = 60 };
            var lblTitle = new Label { Text = "Show Management", Font = new Font("Segoe UI", 14, FontStyle.Bold), Location = new Point(20, 15), AutoSize = true };
            var btnAdd = new Button { Text = "Add New Show", Location = new Point(250, 15), Width = 120, BackColor = Color.Green, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnAdd.Click += BtnAdd_Click;
            topPanel.Controls.Add(lblTitle);
            topPanel.Controls.Add(btnAdd);
            this.Controls.Add(topPanel);

            var bottomPanel = new Panel { Dock = DockStyle.Bottom, Height = 60 };
            var btnDelete = new Button { Text = "Delete Show", Location = new Point(20, 15), Width = 100, BackColor = Color.Red, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnDelete.Click += BtnDelete_Click;
            bottomPanel.Controls.Add(btnDelete);
            this.Controls.Add(bottomPanel);

            dgvShows = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            this.Controls.Add(dgvShows);
            dgvShows.BringToFront();
        }

        private void LoadShows()
        {
            try
            {
                var shows = showManager.GetAllShows();
                dgvShows.DataSource = shows.Select(s => new {
                    s.ShowID,
                    s.MovieID,
                    s.HallID,
                    Date = s.ShowDate.ToShortDateString(),
                    Time = s.ShowTime.ToString(),
                    Price = s.TicketPrice
                }).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error loading shows", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            using var dialog = new Form
            {
                Text = "Add Show",
                Size = new Size(400, 350),
                StartPosition = FormStartPosition.CenterParent,
                Font = new Font("Segoe UI", 10),
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false
            };

            var cmbMovie = new ComboBox { Location = new Point(120, 20), Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
            var cmbHall = new ComboBox { Location = new Point(120, 60), Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
            var dtpDate = new DateTimePicker { Location = new Point(120, 100), Width = 200, Format = DateTimePickerFormat.Short };
            var dtpTime = new DateTimePicker { Location = new Point(120, 140), Width = 200, Format = DateTimePickerFormat.Time, ShowUpDown = true };
            var numPrice = new NumericUpDown { Location = new Point(120, 180), Width = 200, Minimum = 50, Maximum = 5000, DecimalPlaces = 2 };

            try
            {
                var movies = movieManager.GetAllMovies();
                cmbMovie.DataSource = movies;
                cmbMovie.DisplayMember = "Title";
                cmbMovie.ValueMember = "MovieID";

                var halls = hallManager.GetAllHalls();
                cmbHall.DataSource = halls;
                cmbHall.DisplayMember = "HallName";
                cmbHall.ValueMember = "HallID";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading combos: " + ex.Message);
                return;
            }

            dialog.Controls.Add(new Label { Text = "Movie:", Location = new Point(20, 23), AutoSize = true }); dialog.Controls.Add(cmbMovie);
            dialog.Controls.Add(new Label { Text = "Hall:", Location = new Point(20, 63), AutoSize = true }); dialog.Controls.Add(cmbHall);
            dialog.Controls.Add(new Label { Text = "Date:", Location = new Point(20, 103), AutoSize = true }); dialog.Controls.Add(dtpDate);
            dialog.Controls.Add(new Label { Text = "Time:", Location = new Point(20, 143), AutoSize = true }); dialog.Controls.Add(dtpTime);
            dialog.Controls.Add(new Label { Text = "Price:", Location = new Point(20, 183), AutoSize = true }); dialog.Controls.Add(numPrice);

            var btnSave = new Button { Text = "Save", Location = new Point(120, 240), Width = 80, BackColor = Color.Green, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnSave.Click += (s, ev) =>
            {
                try
                {
                    if (cmbMovie.SelectedValue == null || cmbHall.SelectedValue == null) return;
                    
                    var newShow = new Show
                    {
                        MovieID = (int)cmbMovie.SelectedValue,
                        HallID = (int)cmbHall.SelectedValue,
                        ShowDate = dtpDate.Value.Date,
                        ShowTime = dtpTime.Value.TimeOfDay,
                        TicketPrice = numPrice.Value
                    };

                    showManager.AddShow(newShow);
                    dialog.DialogResult = DialogResult.OK;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error adding show", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
            dialog.Controls.Add(btnSave);

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                LoadShows();
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvShows.SelectedRows.Count > 0)
            {
                if (MessageBox.Show("Delete this show?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    try
                    {
                        int showId = (int)dgvShows.SelectedRows[0].Cells["ShowID"].Value;
                        showManager.DeleteShow(showId);
                        LoadShows();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error deleting", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
