using System;
using System.Drawing;
using System.Windows.Forms;
using CinemaHallSystem.BLL;
using CinemaHallSystem.Models;

namespace CinemaHallSystem.Forms
{
    public class HallManagementForm : Form
    {
        private DataGridView dgvHalls;
        private HallManager hallManager = new HallManager();

        public HallManagementForm()
        {
            InitializeComponent();
            LoadHalls();
        }

        private void InitializeComponent()
        {
            this.Text = "Hall Management";
            this.Font = new Font("Segoe UI", 10);
            this.Dock = DockStyle.Fill;

            var topPanel = new Panel { Dock = DockStyle.Top, Height = 60 };
            var lblTitle = new Label { Text = "Hall Management", Font = new Font("Segoe UI", 14, FontStyle.Bold), Location = new Point(20, 15), AutoSize = true };
            var btnAdd = new Button { Text = "Add New Hall", Location = new Point(250, 15), Width = 120, BackColor = Color.Green, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnAdd.Click += BtnAdd_Click;
            topPanel.Controls.Add(lblTitle);
            topPanel.Controls.Add(btnAdd);
            this.Controls.Add(topPanel);

            var bottomPanel = new Panel { Dock = DockStyle.Bottom, Height = 60 };
            var btnDelete = new Button { Text = "Delete Hall", Location = new Point(20, 15), Width = 100, BackColor = Color.Red, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnDelete.Click += BtnDelete_Click;
            bottomPanel.Controls.Add(btnDelete);
            this.Controls.Add(bottomPanel);

            dgvHalls = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            this.Controls.Add(dgvHalls);
            dgvHalls.BringToFront();
        }

        private void LoadHalls()
        {
            try
            {
                dgvHalls.DataSource = hallManager.GetAllHalls();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error loading halls", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            using var dialog = new Form
            {
                Text = "Add Hall",
                Size = new Size(350, 250),
                StartPosition = FormStartPosition.CenterParent,
                Font = new Font("Segoe UI", 10),
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false
            };

            var txtHallName = new TextBox { Location = new Point(120, 20), Width = 150 };
            var numRows = new NumericUpDown { Location = new Point(120, 60), Width = 150, Minimum = 2, Maximum = 20 };
            var numSeats = new NumericUpDown { Location = new Point(120, 100), Width = 150, Minimum = 5, Maximum = 20 };

            dialog.Controls.Add(new Label { Text = "Hall Name:", Location = new Point(20, 23), AutoSize = true }); dialog.Controls.Add(txtHallName);
            dialog.Controls.Add(new Label { Text = "Rows:", Location = new Point(20, 63), AutoSize = true }); dialog.Controls.Add(numRows);
            dialog.Controls.Add(new Label { Text = "Seats/Row:", Location = new Point(20, 103), AutoSize = true }); dialog.Controls.Add(numSeats);

            var btnSave = new Button { Text = "Save", Location = new Point(120, 150), Width = 80, BackColor = Color.Green, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnSave.Click += (s, ev) =>
            {
                try
                {
                    hallManager.AddHall(txtHallName.Text, (int)numRows.Value, (int)numSeats.Value);
                    dialog.DialogResult = DialogResult.OK;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error adding hall", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
            dialog.Controls.Add(btnSave);

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                LoadHalls();
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvHalls.SelectedRows.Count > 0)
            {
                if (MessageBox.Show("Delete this hall?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    try
                    {
                        var hall = (Hall)dgvHalls.SelectedRows[0].DataBoundItem;
                        hallManager.DeleteHall(hall.HallID);
                        LoadHalls();
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
