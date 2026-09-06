using System;
using System.Drawing;
using System.Windows.Forms;
using CinemaHallSystem.BLL;
using CinemaHallSystem.Models;

namespace CinemaHallSystem.Forms
{
    public class MovieManagementForm : Form
    {
        private DataGridView dgvMovies;
        private MovieManager movieManager = new MovieManager();

        public MovieManagementForm()
        {
            InitializeComponent();
            LoadMovies();
        }

        private void InitializeComponent()
        {
            this.Text = "Movie Management";
            this.Font = new Font("Segoe UI", 10);
            this.Dock = DockStyle.Fill;

            var topPanel = new Panel { Dock = DockStyle.Top, Height = 60 };
            var lblTitle = new Label { Text = "Movie Management", Font = new Font("Segoe UI", 14, FontStyle.Bold), Location = new Point(20, 15), AutoSize = true };
            var btnAdd = new Button { Text = "Add New Movie", Location = new Point(250, 15), Width = 120, BackColor = Color.Green, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnAdd.Click += BtnAdd_Click;
            topPanel.Controls.Add(lblTitle);
            topPanel.Controls.Add(btnAdd);
            this.Controls.Add(topPanel);

            var bottomPanel = new Panel { Dock = DockStyle.Bottom, Height = 60 };
            var btnEdit = new Button { Text = "Edit Movie", Location = new Point(20, 15), Width = 100, BackColor = Color.Orange, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnEdit.Click += BtnEdit_Click;
            var btnDelete = new Button { Text = "Delete Movie", Location = new Point(140, 15), Width = 100, BackColor = Color.Red, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnDelete.Click += BtnDelete_Click;
            bottomPanel.Controls.Add(btnEdit);
            bottomPanel.Controls.Add(btnDelete);
            this.Controls.Add(bottomPanel);

            dgvMovies = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            this.Controls.Add(dgvMovies);
            dgvMovies.BringToFront();
        }

        private void LoadMovies()
        {
            try
            {
                var list = movieManager.GetAllMovies();
                dgvMovies.DataSource = null;
                dgvMovies.DataSource = list;
                if (dgvMovies.Columns != null && dgvMovies.Columns["MovieID"] != null)
                {
                    dgvMovies.Columns["MovieID"].Width = 60;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading movies: {ex.Message}", "Error loading movies", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            ShowEditDialog(null);
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgvMovies.SelectedRows.Count > 0)
            {
                var movie = (Movie)dgvMovies.SelectedRows[0].DataBoundItem;
                ShowEditDialog(movie);
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvMovies.SelectedRows.Count > 0)
            {
                if (MessageBox.Show("Are you sure you want to delete this movie?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    try
                    {
                        var movie = (Movie)dgvMovies.SelectedRows[0].DataBoundItem;
                        movieManager.DeleteMovie(movie.MovieID);
                        MessageBox.Show("Movie deleted.");
                        LoadMovies();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void ShowEditDialog(Movie movie)
        {
            using var dialog = new Form
            {
                Text = movie == null ? "Add Movie" : "Edit Movie",
                Size = new Size(400, 500),
                StartPosition = FormStartPosition.CenterParent,
                Font = new Font("Segoe UI", 10),
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false
            };

            var txtTitle = new TextBox { Location = new Point(120, 20), Width = 200 };
            var txtGenre = new TextBox { Location = new Point(120, 60), Width = 200 };
            var numDuration = new NumericUpDown { Location = new Point(120, 100), Width = 200, Maximum = 500 };
            var txtLanguage = new TextBox { Location = new Point(120, 140), Width = 200 };
            var txtRating = new TextBox { Location = new Point(120, 180), Width = 200 };
            var dtpRelease = new DateTimePicker { Location = new Point(120, 220), Width = 200, Format = DateTimePickerFormat.Short };
            var txtDesc = new TextBox { Location = new Point(120, 260), Width = 200, Height = 100, Multiline = true };

            if (movie != null)
            {
                txtTitle.Text = movie.Title;
                txtGenre.Text = movie.Genre;
                numDuration.Value = movie.DurationMinutes;
                txtLanguage.Text = movie.Language;
                txtRating.Text = movie.AgeRating;
                dtpRelease.Value = movie.ReleaseDate;
                txtDesc.Text = movie.Description;
            }

            dialog.Controls.Add(new Label { Text = "Title:", Location = new Point(20, 23), AutoSize = true }); dialog.Controls.Add(txtTitle);
            dialog.Controls.Add(new Label { Text = "Genre:", Location = new Point(20, 63), AutoSize = true }); dialog.Controls.Add(txtGenre);
            dialog.Controls.Add(new Label { Text = "Duration:", Location = new Point(20, 103), AutoSize = true }); dialog.Controls.Add(numDuration);
            dialog.Controls.Add(new Label { Text = "Language:", Location = new Point(20, 143), AutoSize = true }); dialog.Controls.Add(txtLanguage);
            dialog.Controls.Add(new Label { Text = "Rating:", Location = new Point(20, 183), AutoSize = true }); dialog.Controls.Add(txtRating);
            dialog.Controls.Add(new Label { Text = "Release Date:", Location = new Point(20, 223), AutoSize = true }); dialog.Controls.Add(dtpRelease);
            dialog.Controls.Add(new Label { Text = "Description:", Location = new Point(20, 263), AutoSize = true }); dialog.Controls.Add(txtDesc);

            var btnSave = new Button { Text = "Save", Location = new Point(120, 380), Width = 80, BackColor = Color.Green, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnSave.Click += (s, e) =>
            {
                try
                {
                    var m = movie ?? new Movie();
                    m.Title = txtTitle.Text;
                    m.Genre = txtGenre.Text;
                    m.DurationMinutes = (int)numDuration.Value;
                    m.Language = txtLanguage.Text;
                    m.AgeRating = txtRating.Text;
                    m.ReleaseDate = dtpRelease.Value;
                    m.Description = txtDesc.Text;

                    if (movie == null)
                    {
                        movieManager.AddMovie(m);
                    }
                    else
                    {
                        movieManager.UpdateMovie(m);
                    }
                    
                    dialog.DialogResult = DialogResult.OK;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error saving", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
            dialog.Controls.Add(btnSave);

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                LoadMovies();
            }
        }
    }
}
