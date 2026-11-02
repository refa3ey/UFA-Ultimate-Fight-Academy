using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using GYM_Desktop_app.Database;
using GYM_Desktop_app.Helpers;

namespace GYM_Desktop_app.Forms
{
    public partial class ReportsForm : Form
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(int l, int t, int r, int b, int w, int h);

        private bool      _dragging;
        private Point     _dragStart;
        private DataTable _reportData;
        private enum ReportView { Payments, Coaches, Sales }
        private ReportView _view = ReportView.Payments;
        private bool       _lastMonthOnly;
        private Guna.UI2.WinForms.Guna2Button btnPayments, btnSales, btnTotal, btnLastMonth;
        private Label lblRevenue;
        private readonly Font _totalRowFont = new Font("Segoe UI", 9.5f, FontStyle.Bold);

        public ReportsForm()
        {
            InitializeComponent();
            SetupFilters();
            LoadReport();
        }

        private static readonly Color Selected   = Color.FromArgb(242, 101, 34);
        private static readonly Color Unselected = Color.FromArgb(90, 90, 90);

        // Row 1: what to show, and for which period. Row 2: coach search.
        private void SetupFilters()
        {
            btnPayments = NewFilterButton("Payments", 10, 120, (s, e) => SetView(ReportView.Payments));
            btnAllCoaches.Location = new Point(135, 65);
            btnAllCoaches.Size     = new Size(120, 40);
            btnSales = NewFilterButton("Product Sales", 260, 140, (s, e) => SetView(ReportView.Sales));

            btnTotal = NewFilterButton("Total", 540, 120, (s, e) => SetPeriod(false));
            btnLastMonth = NewFilterButton(
                "Last Month (" + DateTime.Today.AddMonths(-1).ToString("MMM yyyy") + ")",
                665, 245, (s, e) => SetPeriod(true));

            txtSearch.Location = new Point(10, 112);
            dgvReport.Location = new Point(10, 160);
            panelSummary.Location = new Point(10, 525);
            ClientSize = new Size(920, 665);

            lblRevenue = new Label
            {
                Font      = new Font("Segoe UI", 10.5F),
                ForeColor = Color.FromArgb(33, 33, 33),
                Location  = new Point(20, 60),
                Size      = new Size(570, 60)
            };
            panelSummary.Controls.Add(lblRevenue);

            var btnMonthly = new Guna.UI2.WinForms.Guna2Button
            {
                Text         = "Monthly PDF",
                BorderRadius = 8,
                FillColor    = Color.FromArgb(26, 26, 26),
                Font         = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor    = Color.White,
                Location     = new Point(610, 15),
                Size         = new Size(140, 40)
            };
            btnMonthly.Click += btnMonthlyPdf_Click;
            panelSummary.Controls.Add(btnMonthly);
        }

        private void btnMonthlyPdf_Click(object sender, EventArgs e)
        {
            DateTime? month = AskForMonth();
            if (month == null) return;
            string path = ExportDialog.PromptForPDFPath($"UFA_Monthly_Report_{month:yyyy_MM}.pdf");
            if (path == null) return;
            try
            {
                ExportHelper.ExportMonthlyReportToPDF(path, month.Value, DatabaseHelper.GetMonthlyReport(month.Value));
                ExportHelper.OpenFile(path);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not create the report: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Starts on last month, and can't go past this month.
        private DateTime? AskForMonth()
        {
            using (var dialog = new Form
            {
                Text            = "Monthly Report",
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition   = FormStartPosition.CenterParent,
                MaximizeBox     = false,
                MinimizeBox     = false,
                ClientSize      = new Size(300, 130),
                BackColor       = Color.White
            })
            {
                var picker = new DateTimePicker
                {
                    Format       = DateTimePickerFormat.Custom,
                    CustomFormat = "MMMM yyyy",
                    ShowUpDown   = true,
                    Font         = new Font("Segoe UI", 11F),
                    Location     = new Point(20, 38),
                    Size         = new Size(260, 30),
                    MaxDate      = DateTime.Today,
                    Value        = DateTime.Today.AddMonths(-1)
                };
                var ok = new Button { Text = "Create", DialogResult = DialogResult.OK, Location = new Point(110, 88), Size = new Size(80, 30) };
                var cancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Location = new Point(200, 88), Size = new Size(80, 30) };
                dialog.Controls.Add(new Label { Text = "Which month?", Font = new Font("Segoe UI", 10F, FontStyle.Bold), Location = new Point(20, 12), Size = new Size(260, 22) });
                dialog.Controls.AddRange(new Control[] { picker, ok, cancel });
                dialog.AcceptButton = ok;
                dialog.CancelButton = cancel;
                return dialog.ShowDialog(this) == DialogResult.OK ? picker.Value : (DateTime?)null;
            }
        }

        private Guna.UI2.WinForms.Guna2Button NewFilterButton(string text, int x, int width, EventHandler onClick)
        {
            var btn = new Guna.UI2.WinForms.Guna2Button
            {
                Text         = text,
                BorderRadius = 8,
                Font         = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor    = Color.White,
                Location     = new Point(x, 65),
                Size         = new Size(width, 40)
            };
            btn.Click += onClick;
            Controls.Add(btn);
            return btn;
        }

        private void SetView(ReportView view)
        {
            _view = view;
            txtSearch.Text = "";
            LoadReport();
        }

        private void SetPeriod(bool lastMonthOnly)
        {
            _lastMonthOnly = lastMonthOnly;
            LoadReport();
        }

        // The calendar month before this one.
        private DateTime? PeriodStart => _lastMonthOnly ? new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-1) : (DateTime?)null;
        private DateTime? PeriodEnd   => _lastMonthOnly ? new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1) : (DateTime?)null;
        private string    PeriodName  => _lastMonthOnly ? PeriodStart.Value.ToString("MMMM yyyy") : "All time";

        private void ReportsForm_Load(object sender, EventArgs e)
        {
            Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 16, 16));
            ApplyGridStyle(dgvReport);
        }

        private void DragPanel_MouseDown(object sender, MouseEventArgs e)
        {
            _dragging = true;
            _dragStart = e.Location;
        }

        private void DragPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (_dragging)
                Location = new Point(Location.X + e.X - _dragStart.X, Location.Y + e.Y - _dragStart.Y);
        }

        private void DragPanel_MouseUp(object sender, MouseEventArgs e) => _dragging = false;

        private void btnClose_Click(object sender, EventArgs e) => this.Close();

        private void ApplyGridStyle(DataGridView dgv)
        {
            dgv.BackgroundColor                           = Color.White;
            dgv.BorderStyle                               = BorderStyle.None;
            dgv.GridColor                                 = Color.FromArgb(230, 230, 230);
            dgv.RowHeadersVisible                         = false;
            dgv.AutoSizeColumnsMode                       = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.AllowUserToAddRows                        = false;
            dgv.SelectionMode                             = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect                               = false;
            dgv.ReadOnly                                  = true;
            dgv.EnableHeadersVisualStyles                 = false;
            dgv.RowTemplate.Height                        = 38;
            dgv.DefaultCellStyle.Font                     = new Font("Segoe UI", 9.5f);
            dgv.DefaultCellStyle.SelectionBackColor       = Color.FromArgb(242, 101, 34);
            dgv.DefaultCellStyle.SelectionForeColor       = Color.White;
            dgv.DefaultCellStyle.Padding                  = new Padding(5, 0, 5, 0);
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 247, 250);
            dgv.ColumnHeadersHeight                       = 42;
            dgv.ColumnHeadersDefaultCellStyle.BackColor   = Color.FromArgb(242, 101, 34);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor   = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font        = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.Padding     = new Padding(5, 0, 5, 0);
        }

        private void LoadReport()
        {
            btnPayments.FillColor   = _view == ReportView.Payments ? Selected : Unselected;
            btnAllCoaches.FillColor = _view == ReportView.Coaches  ? Selected : Unselected;
            btnSales.FillColor      = _view == ReportView.Sales    ? Selected : Unselected;
            btnTotal.FillColor      = _lastMonthOnly ? Unselected : Selected;
            btnLastMonth.FillColor  = _lastMonthOnly ? Selected : Unselected;
            txtSearch.PlaceholderText =
                  _view == ReportView.Coaches ? "Search by coach name..."
                : _view == ReportView.Sales   ? "Search by product, category or method..."
                :                               "Search by member, coach, plan or method...";

            if (_view == ReportView.Coaches) LoadCoachReport(txtSearch.Text.Trim());
            else if (_view == ReportView.Sales) LoadSalesList();
            else LoadPaymentsList();
            ShowRevenue();
        }

        // Keeps only the rows the search box matches, so the list, the totals and the
        // Excel export all agree.
        private void ApplySearch(DataTable table, params string[] columns)
        {
            string search = txtSearch.Text.Trim();
            if (search.Length == 0) return;
            for (int i = table.Rows.Count - 1; i >= 0; i--)
            {
                bool match = false;
                foreach (string column in columns)
                    if (table.Rows[i][column].ToString().IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0)
                        match = true;
                if (!match) table.Rows.RemoveAt(i);
            }
        }

        // Always the whole picture for the period, whichever list is on screen.
        private void ShowRevenue()
        {
            try
            {
                var (memberships, products) = DatabaseHelper.GetRevenueTotals(PeriodStart, PeriodEnd);
                lblRevenue.Text = $"{PeriodName}\nMemberships {memberships:N0} + Products {products:N0} = {memberships + products:N0} EGP";
            }
            catch (Exception ex) { MessageBox.Show("Error loading totals: " + ex.Message); }
        }

        private void LoadSalesList()
        {
            try
            {
                _reportData          = DatabaseHelper.GetProductSales(PeriodStart, PeriodEnd);
                ApplySearch(_reportData, "Product", "Category", "Method");
                dgvReport.DataSource = null;
                dgvReport.DataSource = _reportData;
                dgvReport.Columns["SaleID"].Visible = false; dgvReport.Columns["Date"].FillWeight = 170;
                dgvReport.Columns["UnitPrice"].HeaderText = "Unit Price (EGP)";
                dgvReport.Columns["Total"].HeaderText     = "Total (EGP)";
                dgvReport.Columns["UnitPrice"].DefaultCellStyle.Format = "N0";
                dgvReport.Columns["Total"].DefaultCellStyle.Format     = "N0";

                decimal total = 0;
                foreach (DataRow row in _reportData.Rows) total += Convert.ToDecimal(row["Total"]);
                lblTotalPayments.Text = $"Total Sales: {_reportData.Rows.Count}";
                lblTotalAmount.Text   = $"Total Amount: {total:N0} EGP";
            }
            catch (Exception ex) { MessageBox.Show("Error loading report: " + ex.Message); }
        }

        private void LoadCoachReport(string search)
        {
            try
            {
                var raw = DatabaseHelper.GetCoachPaymentsReport(search, PeriodStart, PeriodEnd);

                // The totals go in their own column, so the Amount sum in the Excel
                // export doesn't count them twice.
                var view = new DataTable();
                view.Columns.Add("Coach", typeof(string));
                view.Columns.Add("Member", typeof(string));
                view.Columns.Add("Payments", typeof(int));
                view.Columns.Add("Amount", typeof(decimal));
                view.Columns.Add("Coach Total", typeof(decimal));

                var     allMembers = new HashSet<string>();
                int     coaches    = 0;
                decimal grandTotal = 0;
                int     i          = 0;
                while (i < raw.Rows.Count)
                {
                    string  coachKey   = raw.Rows[i]["CoachKey"].ToString();
                    string  coach      = raw.Rows[i]["Coach"].ToString();
                    int     members    = 0;
                    int     payments   = 0;
                    decimal coachTotal = 0;

                    for (; i < raw.Rows.Count && raw.Rows[i]["CoachKey"].ToString() == coachKey; i++)
                    {
                        DataRow row = raw.Rows[i];
                        if (row["MemberKey"] == DBNull.Value) continue;   // coach with no members
                        int     count  = Convert.ToInt32(row["Payments"]);
                        decimal amount = Convert.ToDecimal(row["Amount"]);
                        view.Rows.Add(coach, row["Member"], count, amount, DBNull.Value);
                        allMembers.Add(row["MemberKey"].ToString());
                        members++;
                        payments   += count;
                        coachTotal += amount;
                    }

                    view.Rows.Add(coach, $"TOTAL: {members} member(s)", payments, DBNull.Value, coachTotal);
                    coaches++;
                    grandTotal += coachTotal;
                }

                _reportData          = view;
                dgvReport.DataSource = null;
                dgvReport.DataSource = view;
                dgvReport.Columns["Amount"].HeaderText              = "Amount (EGP)";
                dgvReport.Columns["Amount"].DefaultCellStyle.Format = "N0";
                dgvReport.Columns["Coach Total"].HeaderText              = "Coach Total (EGP)";
                dgvReport.Columns["Coach Total"].DefaultCellStyle.Format = "N0";

                lblTotalPayments.Text = $"Coaches: {coaches}   Members: {allMembers.Count}";
                lblTotalAmount.Text   = $"Total Amount: {grandTotal:N0} EGP";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading report: " + ex.Message);
            }
        }

        private void dgvReport_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || !dgvReport.Columns.Contains("Coach Total")) return;
            if (dgvReport.Rows[e.RowIndex].Cells["Coach Total"].Value is decimal)
            {
                e.CellStyle.Font      = _totalRowFont;
                e.CellStyle.BackColor = Color.FromArgb(255, 236, 224);
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadReport();
        }

        private void btnAllCoaches_Click(object sender, EventArgs e) => SetView(ReportView.Coaches);

        private void LoadPaymentsList()
        {
            try
            {
                _reportData          = DatabaseHelper.GetPaymentsReport(PeriodStart, PeriodEnd);
                ApplySearch(_reportData, "MemberName", "Plan", "Method");
                dgvReport.DataSource = null;
                dgvReport.DataSource = _reportData;

                // Make the Plan column clear and well-placed
                if (dgvReport.Columns.Contains("PaymentID"))  dgvReport.Columns["PaymentID"].Visible = false;
                if (dgvReport.Columns.Contains("MemberName")) dgvReport.Columns["MemberName"].HeaderText = "Member";
                if (dgvReport.Columns.Contains("Plan"))
                {
                    dgvReport.Columns["Plan"].HeaderText   = "Plan / Coach";
                    dgvReport.Columns["Plan"].DisplayIndex = 1;
                }
                if (dgvReport.Columns.Contains("Amount")) dgvReport.Columns["Amount"].HeaderText = "Amount (EGP)";

                if (dgvReport.Columns.Contains("Amount")) dgvReport.Columns["Amount"].DefaultCellStyle.Format = "N0";

                // Free sessions are listed, but they aren't payments.
                int     totalCount  = 0;
                decimal totalAmount = 0;
                foreach (DataRow row in _reportData.Rows)
                {
                    decimal amount = Convert.ToDecimal(row["Amount"]);
                    totalAmount += amount;
                    if (amount > 0) totalCount++;
                }

                lblTotalPayments.Text = $"Total Payments: {totalCount}";
                lblTotalAmount.Text   = $"Total Amount: {totalAmount:N0} EGP";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading report: " + ex.Message);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e) => LoadReport();

        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            if (_reportData == null || _reportData.Rows.Count == 0)
            {
                MessageBox.Show("No data to export.", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string report = _view == ReportView.Sales ? "Product Sales" : _view == ReportView.Coaches ? "Coach Report" : "Payment Report";
            string path = ExportDialog.PromptForExcelPath($"{report.Replace(" ", "_")}_{PeriodName.Replace(" ", "_")}.xlsx");
            if (path == null) return;
            try
            {
                ExportHelper.ExportPaymentsToExcel(path, _reportData, $"{report} - {PeriodName}");
                if (MessageBox.Show("Excel file saved.\n\nOpen it now?", "Export Complete",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    ExportHelper.OpenFile(path);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Export failed: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
