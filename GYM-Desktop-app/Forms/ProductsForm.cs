using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using GYM_Desktop_app.Database;
using GYM_Desktop_app.Models;

namespace GYM_Desktop_app.Forms
{
    // Products page: manage items and stock, sell them, and see the sales history.
    public class ProductsForm : Form
    {
        private const string AllCategories = "All Categories";

        private bool  _dragging;
        private Point _dragStart;
        private int   _selectedProductID;
        private List<Product> _products = new List<Product>();

        private readonly DataGridView dgvProducts = new DataGridView();
        private readonly DataGridView dgvSales    = new DataGridView();
        private readonly Guna2TextBox txtSearch   = NewTextBox("Search products...", 10, 10, 300);
        private readonly ComboBox     cmbFilter   = NewCombo(320, 13, 190);
        private readonly ComboBox     cmbCategory = NewCombo(20, 34, 170);
        private readonly Guna2TextBox txtName     = NewTextBox("", 200, 30, 260);
        private readonly Guna2TextBox txtSize     = NewTextBox("", 470, 30, 110);
        private readonly NumericUpDown numPrice   = NewNumber(590, 34, 130, 1000000);
        private readonly NumericUpDown numStock   = NewNumber(730, 34, 110, 100000);
        private readonly NumericUpDown numQty     = NewNumber(590, 112, 80, 100000);
        private readonly ComboBox     cmbMethod   = NewCombo(680, 112, 160);
        private readonly Label        lblSalesTotal = new Label();

        public ProductsForm()
        {
            BuildUi();
            LoadProducts();
            LoadSales();
        }

        // ── Layout ───────────────────────────────────────────────────────────

        private void BuildUi()
        {
            Text            = "Products";
            FormBorderStyle = FormBorderStyle.None;
            StartPosition   = FormStartPosition.CenterScreen;
            ClientSize      = new Size(1000, 700);
            BackColor       = Color.FromArgb(245, 247, 250);

            var top = new Guna2Panel { FillColor = Color.FromArgb(26, 26, 26), Location = new Point(0, 0), Size = new Size(1000, 55) };
            top.MouseDown += (s, e) => { _dragging = true; _dragStart = e.Location; };
            top.MouseMove += (s, e) => { if (_dragging) Location = new Point(Location.X + e.X - _dragStart.X, Location.Y + e.Y - _dragStart.Y); };
            top.MouseUp   += (s, e) => _dragging = false;
            top.Controls.Add(new Label { Text = "Products", Font = new Font("Segoe UI", 14F, FontStyle.Bold), ForeColor = Color.White, BackColor = Color.Transparent, Location = new Point(20, 12), Size = new Size(400, 30) });
            var close = NewButton("✕", 960, 13, 28, Color.FromArgb(220, 53, 69));
            close.Size = new Size(28, 28);
            close.BorderRadius = 14;
            close.Click += (s, e) => Close();
            top.Controls.Add(close);

            var tabs = new TabControl { Location = new Point(10, 62), Size = new Size(980, 628), Font = new Font("Segoe UI", 10F) };
            var tabProducts = new TabPage("Products") { BackColor = BackColor };
            var tabSales    = new TabPage("Sales History") { BackColor = BackColor };
            tabs.TabPages.Add(tabProducts);
            tabs.TabPages.Add(tabSales);

            // Products tab
            cmbFilter.Items.Add(AllCategories);
            cmbFilter.Items.AddRange(Product.Categories);
            cmbFilter.SelectedIndex = 0;
            cmbFilter.SelectedIndexChanged += (s, e) => ShowProducts();
            txtSearch.TextChanged += (s, e) => ShowProducts();
            tabProducts.Controls.Add(txtSearch);
            tabProducts.Controls.Add(cmbFilter);

            dgvProducts.Location = new Point(10, 58);
            dgvProducts.Size     = new Size(950, 345);
            dgvProducts.CellClick      += dgvProducts_CellClick;
            dgvProducts.CellFormatting += dgvProducts_CellFormatting;
            StyleGrid(dgvProducts);
            tabProducts.Controls.Add(dgvProducts);

            var panel = new Guna2Panel { BorderColor = Color.FromArgb(230, 230, 230), BorderRadius = 12, BorderThickness = 1, FillColor = Color.White, Location = new Point(10, 412), Size = new Size(950, 175) };
            cmbCategory.Items.AddRange(Product.Categories);
            cmbCategory.SelectedIndex = 0;
            cmbMethod.Items.AddRange(new object[] { "Cash", "Vodafone Cash", "InstaPay" });
            cmbMethod.SelectedIndex = 0;
            numPrice.DecimalPlaces = 0;
            numQty.Minimum = 1;
            numQty.Value   = 1;

            panel.Controls.Add(NewFieldLabel("CATEGORY", 20, 12, 170));
            panel.Controls.Add(cmbCategory);
            panel.Controls.Add(NewFieldLabel("PRODUCT NAME", 200, 12, 260));
            panel.Controls.Add(txtName);
            panel.Controls.Add(NewFieldLabel("SIZE (OPTIONAL)", 470, 12, 110));
            panel.Controls.Add(txtSize);
            panel.Controls.Add(NewFieldLabel("PRICE (EGP)", 590, 12, 130));
            panel.Controls.Add(numPrice);
            panel.Controls.Add(NewFieldLabel("STOCK", 730, 12, 110));
            panel.Controls.Add(numStock);
            panel.Controls.Add(NewFieldLabel("SELL QUANTITY", 590, 90, 90));
            panel.Controls.Add(numQty);
            panel.Controls.Add(NewFieldLabel("PAID BY", 680, 90, 160));
            panel.Controls.Add(cmbMethod);

            var add    = NewButton("Add", 20, 105, 100, Color.FromArgb(242, 101, 34));
            var update = NewButton("Update", 130, 105, 100, Color.FromArgb(52, 152, 219));
            var delete = NewButton("Delete", 240, 105, 100, Color.FromArgb(220, 53, 69));
            var clear  = NewButton("Clear", 350, 105, 100, Color.Gray);
            var sell   = NewButton("Sell", 850, 105, 85, Color.FromArgb(40, 167, 69));
            add.Click    += btnAdd_Click;
            update.Click += btnUpdate_Click;
            delete.Click += btnDelete_Click;
            clear.Click  += (s, e) => ClearFields();
            sell.Click   += btnSell_Click;
            panel.Controls.AddRange(new Control[] { add, update, delete, clear, sell });
            tabProducts.Controls.Add(panel);

            // Sales tab
            dgvSales.Location = new Point(10, 10);
            dgvSales.Size     = new Size(950, 510);
            StyleGrid(dgvSales);
            tabSales.Controls.Add(dgvSales);
            lblSalesTotal.Font      = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblSalesTotal.ForeColor = Color.FromArgb(40, 167, 69);
            lblSalesTotal.Location  = new Point(10, 538);
            lblSalesTotal.Size      = new Size(600, 30);
            tabSales.Controls.Add(lblSalesTotal);
            var undo = NewButton("Undo Selected Sale", 760, 530, 200, Color.FromArgb(220, 53, 69));
            undo.Click += btnUndoSale_Click;
            tabSales.Controls.Add(undo);

            Controls.Add(top);
            Controls.Add(tabs);
        }

        private static Guna2TextBox NewTextBox(string placeholder, int x, int y, int width) => new Guna2TextBox
        {
            BorderRadius    = 8,
            DefaultText     = "",
            Font            = new Font("Segoe UI", 10F),
            PlaceholderText = placeholder,
            Location        = new Point(x, y),
            Size            = new Size(width, 38)
        };

        private static ComboBox NewCombo(int x, int y, int width) => new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Font          = new Font("Segoe UI", 10F),
            Location      = new Point(x, y),
            Size          = new Size(width, 30)
        };

        private static NumericUpDown NewNumber(int x, int y, int width, int max) => new NumericUpDown
        {
            Font     = new Font("Segoe UI", 10F),
            Location = new Point(x, y),
            Size     = new Size(width, 30),
            Maximum  = max
        };

        private static Label NewFieldLabel(string text, int x, int y, int width) => new Label
        {
            Text      = text,
            Font      = new Font("Segoe UI", 8F, FontStyle.Bold),
            ForeColor = Color.FromArgb(100, 100, 100),
            Location  = new Point(x, y),
            Size      = new Size(width, 17)
        };

        private static Guna2Button NewButton(string text, int x, int y, int width, Color fill) => new Guna2Button
        {
            Text         = text,
            BorderRadius = 8,
            FillColor    = fill,
            Font         = new Font("Segoe UI", 10F, FontStyle.Bold),
            ForeColor    = Color.White,
            Location     = new Point(x, y),
            Size         = new Size(width, 40)
        };

        internal static void StyleGrid(DataGridView dgv)
        {
            dgv.BackgroundColor                           = Color.White;
            dgv.BorderStyle                               = BorderStyle.None;
            dgv.GridColor                                 = Color.FromArgb(230, 230, 230);
            dgv.RowHeadersVisible                         = false;
            dgv.AllowUserToAddRows                        = false;
            dgv.AutoSizeColumnsMode                       = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.SelectionMode                             = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect                               = false;
            dgv.ReadOnly                                  = true;
            dgv.EnableHeadersVisualStyles                 = false;
            dgv.RowTemplate.Height                        = 36;
            dgv.DefaultCellStyle.Font                     = new Font("Segoe UI", 9.5f);
            dgv.DefaultCellStyle.SelectionBackColor       = Color.FromArgb(242, 101, 34);
            dgv.DefaultCellStyle.SelectionForeColor       = Color.White;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 247, 250);
            dgv.ColumnHeadersHeightSizeMode               = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgv.ColumnHeadersHeight                       = 40;
            dgv.ColumnHeadersDefaultCellStyle.BackColor   = Color.FromArgb(242, 101, 34);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor   = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font        = new Font("Segoe UI", 9.5f, FontStyle.Bold);
        }

        // ── Data ─────────────────────────────────────────────────────────────

        private void LoadProducts()
        {
            try
            {
                _products = DatabaseHelper.GetAllProducts();
                ShowProducts();
            }
            catch (Exception ex) { MessageBox.Show("Error loading products: " + ex.Message); }
        }

        private void ShowProducts()
        {
            string search   = txtSearch.Text.Trim();
            string category = cmbFilter.SelectedItem as string;

            var shown = _products.Where(p =>
                (category == AllCategories || p.Category == category) &&
                (search.Length == 0
                    || (p.Name ?? "").IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0
                    || (p.Size ?? "").IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0
                    || (p.Category ?? "").IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0)).ToList();

            dgvProducts.DataSource = null;
            dgvProducts.DataSource = shown;
            dgvProducts.Columns["ProductID"].Visible   = false;
            dgvProducts.Columns["Price"].HeaderText    = "Price (EGP)";
            dgvProducts.Columns["Price"].DefaultCellStyle.Format = "N0";
            dgvProducts.ClearSelection();
        }

        private void LoadSales()
        {
            try
            {
                DataTable sales = DatabaseHelper.GetProductSales();
                dgvSales.DataSource = sales;
                dgvSales.Columns["SaleID"].Visible = false;
                dgvSales.Columns["UnitPrice"].HeaderText = "Unit Price (EGP)";
                dgvSales.Columns["Total"].HeaderText     = "Total (EGP)";
                dgvSales.Columns["UnitPrice"].DefaultCellStyle.Format = "N0";
                dgvSales.Columns["Total"].DefaultCellStyle.Format     = "N0";

                decimal total = 0;
                foreach (DataRow row in sales.Rows) total += Convert.ToDecimal(row["Total"]);
                lblSalesTotal.Text = $"Sales: {sales.Rows.Count}     Total: {total:N0} EGP";
            }
            catch (Exception ex) { MessageBox.Show("Error loading sales: " + ex.Message); }
        }

        private void dgvProducts_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || dgvProducts.Columns[e.ColumnIndex].Name != "Stock") return;
            if (e.Value is int stock && stock <= 2)
            {
                e.CellStyle.ForeColor = stock == 0 ? Color.FromArgb(220, 53, 69) : Color.FromArgb(214, 137, 16);
                e.CellStyle.Font      = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            }
        }

        private void dgvProducts_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var p = dgvProducts.Rows[e.RowIndex].DataBoundItem as Product;
            if (p == null) return;
            _selectedProductID = p.ProductID;
            cmbCategory.SelectedItem = p.Category;
            txtName.Text   = p.Name;
            txtSize.Text   = p.Size;
            numPrice.Value = Math.Min(p.Price, numPrice.Maximum);
            numStock.Value = Math.Min(p.Stock, numStock.Maximum);
            numQty.Value   = 1;
        }

        private Product ReadFields()
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Please enter a product name.");
                return null;
            }
            return new Product
            {
                ProductID = _selectedProductID,
                Category  = cmbCategory.SelectedItem as string,
                Name      = txtName.Text.Trim(),
                Size      = txtSize.Text.Trim(),
                Price     = numPrice.Value,
                Stock     = (int)numStock.Value
            };
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var p = ReadFields();
            if (p == null) return;
            try
            {
                DatabaseHelper.AddProduct(p);
                LoadProducts();
                ClearFields();
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (_selectedProductID == 0) { MessageBox.Show("Select a product first."); return; }
            var p = ReadFields();
            if (p == null) return;
            try
            {
                DatabaseHelper.UpdateProduct(p);
                LoadProducts();
                ClearFields();
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (_selectedProductID == 0) { MessageBox.Show("Select a product first."); return; }
            if (MessageBox.Show("Delete this product?\n\nIts past sales stay in the Sales History.", "Confirm",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            try
            {
                DatabaseHelper.DeleteProduct(_selectedProductID);
                LoadProducts();
                ClearFields();
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void btnSell_Click(object sender, EventArgs e)
        {
            var p = _products.FirstOrDefault(x => x.ProductID == _selectedProductID);
            if (p == null) { MessageBox.Show("Select a product from the table first."); return; }

            int qty = (int)numQty.Value;
            string method = cmbMethod.SelectedItem.ToString();
            if (MessageBox.Show(
                    $"Sell {qty} x {p.Name} {p.Size}\nTotal: {p.Price * qty:N0} EGP, paid by {method}?",
                    "Confirm Sale", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            try
            {
                if (!DatabaseHelper.SellProduct(p.ProductID, qty, method, out string error))
                {
                    MessageBox.Show(error, "Cannot Sell", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                LoadProducts();
                LoadSales();
                ClearFields();
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void btnUndoSale_Click(object sender, EventArgs e)
        {
            if (dgvSales.SelectedRows.Count == 0) { MessageBox.Show("Select a sale first."); return; }
            var row = dgvSales.SelectedRows[0];
            if (MessageBox.Show(
                    $"Undo this sale of {row.Cells["Quantity"].Value} x {row.Cells["Product"].Value}?\n\n"
                    + "The money is removed from revenue and the items go back into stock.",
                    "Undo Sale", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
            try
            {
                DatabaseHelper.UndoProductSale(Convert.ToInt32(row.Cells["SaleID"].Value));
                LoadProducts();
                LoadSales();
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void ClearFields()
        {
            _selectedProductID = 0;
            txtName.Clear();
            txtSize.Clear();
            numPrice.Value = 0;
            numStock.Value = 0;
            numQty.Value   = 1;
            dgvProducts.ClearSelection();
        }
    }
}
