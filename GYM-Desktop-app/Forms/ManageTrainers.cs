using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using GYM_Desktop_app.Models;
using GYM_Desktop_app.Database;

namespace GYM_Desktop_app.Forms
{
    public partial class ManageTrainers : Form
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(int l, int t, int r, int b, int w, int h);

        private bool _dragging;
        private Point _dragStart;
        private int selectedTrainerID = 0;

        public ManageTrainers()
        {
            InitializeComponent();
            LoadTrainers();
        }

        private void ManageTrainers_Load(object sender, EventArgs e)
        {
            Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 16, 16));
            ApplyGridStyle(dgvTrainers);
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

        private void LoadTrainers()
        {
            try
            {
                dgvTrainers.DataSource = null;
                dgvTrainers.DataSource = DatabaseHelper.GetAllTrainers();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Trainer name is required.");
                return;
            }

            try
            {
                var trainer = new Trainer
                {
                    Name      = txtName.Text.Trim(),
                    Specialty = txtSpecialty.Text.Trim(),
                    Phone     = txtPhone.Text.Trim()
                };
                DatabaseHelper.AddTrainer(trainer);
                MessageBox.Show("Trainer added!");
                LoadTrainers();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedTrainerID == 0)
            {
                MessageBox.Show("Select a trainer first.");
                return;
            }

            var result = MessageBox.Show("Delete this trainer?", "Confirm",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                try
                {
                    DatabaseHelper.DeleteTrainer(selectedTrainerID);
                    MessageBox.Show("Trainer deleted.");
                    LoadTrainers();
                    ClearFields();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void dgvTrainers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var row            = dgvTrainers.Rows[e.RowIndex];
                selectedTrainerID  = Convert.ToInt32(row.Cells["TrainerID"].Value);
                txtName.Text       = row.Cells["Name"].Value.ToString();
                txtSpecialty.Text  = row.Cells["Specialty"].Value?.ToString() ?? "";
                txtPhone.Text      = row.Cells["Phone"].Value?.ToString() ?? "";
            }
        }

        private void ClearFields()
        {
            selectedTrainerID = 0;
            txtName.Clear();
            txtSpecialty.Clear();
            txtPhone.Clear();
        }
    }
}
