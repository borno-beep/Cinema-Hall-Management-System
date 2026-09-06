using System;
using System.Drawing;
using System.Windows.Forms;
using CinemaHallSystem.BLL;

namespace CinemaHallSystem.Forms
{
    public class ReportsForm : Form
    {
        private ComboBox cmbReportType;
        private DateTimePicker dtpFrom;
        private DateTimePicker dtpTo;
        private NumericUpDown nudYear;
        private NumericUpDown nudMonth;
        private Button btnGenerate;
        private Button btnExport;
        private DataGridView dgvReports;

        public ReportsForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Reports";
            this.Font = new Font("Segoe UI", 10);
            this.Dock = DockStyle.Fill;
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopLevel = false;

            Panel topPanel = new Panel { Dock = DockStyle.Top, Height = 80, BackColor = Color.FromArgb(245, 245, 245) };
            
            cmbReportType = new ComboBox
            {
                Location = new Point(20, 25),
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbReportType.Items.AddRange(new string[] { 
                "Daily Sales", "Monthly Revenue", "Movie Popularity", 
                "Hall Occupancy", "Food Sales", "Cancelled Bookings" 
            });
            cmbReportType.SelectedIndex = 0;
            cmbReportType.SelectedIndexChanged += CmbReportType_SelectedIndexChanged;

            dtpFrom = new DateTimePicker { Format = DateTimePickerFormat.Short, Location = new Point(300, 25), Width = 120 };
            dtpTo = new DateTimePicker { Format = DateTimePickerFormat.Short, Location = new Point(450, 25), Width = 120 };
            
            nudYear = new NumericUpDown { Location = new Point(300, 25), Width = 80, Minimum = 2000, Maximum = 2100, Value = DateTime.Now.Year, Visible = false };
            nudMonth = new NumericUpDown { Location = new Point(400, 25), Width = 60, Minimum = 1, Maximum = 12, Value = DateTime.Now.Month, Visible = false };

            btnGenerate = new Button { Text = "Generate", Location = new Point(600, 22), Size = new Size(100, 30), BackColor = Color.FromArgb(33, 150, 243), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnGenerate.Click += BtnGenerate_Click;

            topPanel.Controls.Add(new Label { Text = "Report:", Location = new Point(20, 5), AutoSize = true });
            topPanel.Controls.Add(cmbReportType);
            topPanel.Controls.Add(new Label { Text = "From:", Location = new Point(250, 28), AutoSize = true, Name = "lblFrom" });
            topPanel.Controls.Add(dtpFrom);
            topPanel.Controls.Add(new Label { Text = "To:", Location = new Point(430, 28), AutoSize = true, Name = "lblTo" });
            topPanel.Controls.Add(dtpTo);
            topPanel.Controls.Add(nudYear);
            topPanel.Controls.Add(nudMonth);
            topPanel.Controls.Add(btnGenerate);

            this.Controls.Add(topPanel);

            Panel bottomPanel = new Panel { Dock = DockStyle.Bottom, Height = 60, BackColor = Color.FromArgb(245, 245, 245) };
            btnExport = new Button { Text = "Export to CSV", Location = new Point(20, 15), Size = new Size(120, 35), FlatStyle = FlatStyle.Flat };
            btnExport.Click += BtnExport_Click;
            bottomPanel.Controls.Add(btnExport);
            this.Controls.Add(bottomPanel);

            dgvReports = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                BackgroundColor = Color.White,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            this.Controls.Add(dgvReports);
            dgvReports.BringToFront();
        }

        private void CmbReportType_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool isMonthly = cmbReportType.SelectedItem?.ToString() == "Monthly Revenue";
            
            dtpFrom.Visible = !isMonthly;
            dtpTo.Visible = !isMonthly;
            this.Controls.Find("lblFrom", true)[0].Visible = !isMonthly;
            this.Controls.Find("lblTo", true)[0].Visible = !isMonthly;

            nudYear.Visible = isMonthly;
            nudMonth.Visible = isMonthly;
        }

        private void BtnGenerate_Click(object sender, EventArgs e)
        {
            try
            {
                string reportType = cmbReportType.SelectedItem?.ToString() ?? "";
                var reportManager = new ReportManager();
                System.Data.DataTable result = null;

                switch (reportType)
                {
                    case "Daily Sales":
                        result = reportManager.GetDailySales(dtpFrom.Value);
                        break;
                    case "Monthly Revenue":
                        result = reportManager.GetMonthlyRevenue((int)nudYear.Value, (int)nudMonth.Value);
                        break;
                    case "Movie Popularity":
                        result = reportManager.GetMoviePopularity(dtpFrom.Value.Date, dtpTo.Value.Date);
                        break;
                    case "Hall Occupancy":
                        result = reportManager.GetHallOccupancy(dtpFrom.Value.Date, dtpTo.Value.Date);
                        break;
                    case "Food Sales":
                        result = reportManager.GetFoodSales(dtpFrom.Value.Date, dtpTo.Value.Date);
                        break;
                    case "Cancelled Bookings":
                        result = reportManager.GetCancelledBookings(dtpFrom.Value.Date, dtpTo.Value.Date);
                        break;
                }

                dgvReports.DataSource = result;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating report: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            if (dgvReports.Rows.Count == 0)
            {
                MessageBox.Show("No data to export. Generate a report first.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                using var sfd = new SaveFileDialog
                {
                    Filter = "CSV file (*.csv)|*.csv",
                    FileName = $"{cmbReportType.SelectedItem?.ToString().Replace(" ", "_")}_{DateTime.Now:yyyyMMdd}.csv"
                };

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    using var sw = new System.IO.StreamWriter(sfd.FileName);
                    var headers = new System.Collections.Generic.List<string>();
                    foreach (DataGridViewColumn col in dgvReports.Columns)
                        headers.Add($"\"{col.HeaderText}\"");
                    sw.WriteLine(string.Join(",", headers));

                    foreach (DataGridViewRow row in dgvReports.Rows)
                    {
                        var cells = new System.Collections.Generic.List<string>();
                        foreach (DataGridViewCell cell in row.Cells)
                            cells.Add($"\"{cell.Value?.ToString()?.Replace("\"", "\"\"")}\"");
                        sw.WriteLine(string.Join(",", cells));
                    }
                    MessageBox.Show("Report exported successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting report: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
