using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CinemaHallSystem.BLL;
using CinemaHallSystem.Models;

namespace CinemaHallSystem.Forms
{
    public class FoodSelectionForm : Form
    {
        private FlowLayoutPanel foodPanel;
        private Panel bottomPanel;
        private Label lblTotal;
        private Button btnAdd;
        private Button btnSkip;

        private List<FoodItem> foodItems;
        private Dictionary<int, NumericUpDown> qtyControls = new Dictionary<int, NumericUpDown>();

        public List<FoodOrderItem> SelectedItems { get; private set; } = new List<FoodOrderItem>();
        public decimal FoodTotal { get; private set; }
        public bool HasOrder { get; private set; } = false;

        public FoodSelectionForm()
        {
            InitializeComponent();
            LoadFoodItems();
        }

        private void InitializeComponent()
        {
            this.Text = "Food Selection";
            this.Size = new Size(600, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 10);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            bottomPanel = new Panel { Dock = DockStyle.Bottom, Height = 60, BackColor = Color.FromArgb(240, 240, 240) };

            lblTotal = new Label { Text = "Total: ৳0.00", Font = new Font("Segoe UI", 12, FontStyle.Bold), AutoSize = true, Location = new Point(20, 20) };
            
            btnSkip = new Button { Text = "Skip", Location = new Point(360, 15), Size = new Size(80, 35), FlatStyle = FlatStyle.Flat };
            btnSkip.Click += (s, e) => { HasOrder = false; this.DialogResult = DialogResult.OK; this.Close(); };

            btnAdd = new Button { Text = "Add to Order", Location = new Point(450, 15), Size = new Size(120, 35), BackColor = Color.FromArgb(76, 175, 80), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnAdd.Click += BtnAdd_Click;

            bottomPanel.Controls.Add(lblTotal);
            bottomPanel.Controls.Add(btnSkip);
            bottomPanel.Controls.Add(btnAdd);
            this.Controls.Add(bottomPanel);

            foodPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoScroll = true, Padding = new Padding(10) };
            this.Controls.Add(foodPanel);
            foodPanel.BringToFront();
        }

        private void LoadFoodItems()
        {
            try
            {
                foodItems = new FoodManager().GetAllFoodItems();
                foreach (var item in foodItems)
                {
                    Panel pnlItem = new Panel
                    {
                        Size = new Size(270, 80),
                        BorderStyle = BorderStyle.FixedSingle,
                        Margin = new Padding(5)
                    };

                    Label lblName = new Label { Text = item.Name, Font = new Font("Segoe UI", 10, FontStyle.Bold), Location = new Point(10, 10), AutoSize = true };
                    Label lblPrice = new Label { Text = $"৳{item.Price:F2}", Location = new Point(10, 35), AutoSize = true };
                    Label lblCategory = new Label { Text = item.Category, Font = new Font("Segoe UI", 8), ForeColor = Color.Gray, Location = new Point(10, 55), AutoSize = true };

                    NumericUpDown nudQty = new NumericUpDown
                    {
                        Location = new Point(200, 25),
                        Size = new Size(50, 25),
                        Minimum = 0,
                        Maximum = 20,
                        Tag = item
                    };
                    nudQty.ValueChanged += NudQty_ValueChanged;
                    qtyControls[item.FoodItemID] = nudQty;

                    pnlItem.Controls.Add(lblName);
                    pnlItem.Controls.Add(lblPrice);
                    pnlItem.Controls.Add(lblCategory);
                    pnlItem.Controls.Add(nudQty);

                    foodPanel.Controls.Add(pnlItem);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading food items: {ex.Message}");
            }
        }

        private void NudQty_ValueChanged(object sender, EventArgs e)
        {
            UpdateTotal();
        }

        private void UpdateTotal()
        {
            decimal total = 0;
            foreach (var item in foodItems)
            {
                int qty = (int)qtyControls[item.FoodItemID].Value;
                total += item.Price * qty;
            }
            FoodTotal = total;
            lblTotal.Text = $"Total: ৳{FoodTotal:F2}";
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            SelectedItems.Clear();
            foreach (var item in foodItems)
            {
                int qty = (int)qtyControls[item.FoodItemID].Value;
                if (qty > 0)
                {
                    SelectedItems.Add(new FoodOrderItem
                    {
                        FoodItemID = item.FoodItemID,
                        Quantity = qty,
                        Subtotal = item.Price * qty
                    });
                }
            }

            if (SelectedItems.Count > 0)
            {
                HasOrder = true;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Please select at least one item or click Skip.");
            }
        }
    }
}
