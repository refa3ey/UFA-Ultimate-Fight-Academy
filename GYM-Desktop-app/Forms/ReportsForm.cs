using System;
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

        public ReportsForm()
        {
            InitializeComponent();
            LoadReport();
        }

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
            try
            {
                _reportData          = DatabaseHelper.GetPaymentsReport();
                dgvReport.DataSource = _reportData;

                int     totalCount  = _reportData.Rows.Count;
                decimal totalAmount = 0;
                foreach (DataRow row in _reportData.Rows)
                    totalAmount += Convert.ToDecimal(row["Amount"]);

                lblTotalPayments.Text = $"Total Payments: {totalCount}";
                lblTotalAmount.Text   = $"Total Amount: ${totalAmount:N2}";
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
            string path = ExportDialog.PromptForExcelPath($"Payments_{DateTime.Now:yyyyMMdd}.xlsx");
            if (path == null) return;
            try
            {
                ExportHelper.ExportPaymentsToExcel(path, _reportData);
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
