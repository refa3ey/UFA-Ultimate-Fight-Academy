using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using GYM_Desktop_app.Models;
using GYM_Desktop_app.Database;

namespace GYM_Desktop_app.Forms
{
    public partial class ManagePlans : Form
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(int l, int t, int r, int b, int w, int h);

        private bool _dragging;
        private Point _dragStart;
        private int selectedPlanID = 0;

        private ComboBox cmbCoach;
        private Label lblCoach;

        public ManagePlans()
        {
            InitializeComponent();
            SetupCoachUI();
            LoadPlans();
        }

        // Add a coach selector + relabel the reused fields for the MMA session model
        private void SetupCoachUI()
        {
            lblTitle.Text     = "Manage Coach Plans";
            lblDuration.Text  = "SESSIONS";
            lblPrice.Text     = "PRICE (EGP)";
            numDuration.Maximum = 100;
            numDuration.Value   = 8;
            txtPlanName.Size    = new Size(150, 38);

            lblCoach = new Label
            {
                Text      = "COACH",
                Font      = new Font("Segoe UI", 8F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 100, 100),
                Location  = new Point(185, 15),
                Size      = new Size(160, 17)
            };
            cmbCoach = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font          = new Font("Segoe UI", 10F),
                Location      = new Point(185, 37),
                Size          = new Size(165, 30)
            };
            panelInputs.Controls.Add(lblCoach);
            panelInputs.Controls.Add(cmbCoach);

            LoadCoaches();
        }

        private void LoadCoaches()
        {
            try
            {
                cmbCoach.DataSource    = DatabaseHelper.GetAllTrainers();
                cmbCoach.DisplayMember = "Name";
                cmbCoach.ValueMember   = "TrainerID";
            }
            catch (Exception ex) { MessageBox.Show("Error loading coaches: " + ex.Message); }
        }

        private void ManagePlans_Load(object sender, EventArgs e)
        {
            Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 16, 16));
            ApplyGridStyle(dgvPlans);
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
            dgv.DefaultCellStyle.SelectionBackColor       = Helpers.Theme.Primary;
            dgv.DefaultCellStyle.SelectionForeColor       = Color.White;
            dgv.DefaultCellStyle.Padding                  = new Padding(5, 0, 5, 0);
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 247, 250);
            dgv.ColumnHeadersHeight                       = 42;
            dgv.ColumnHeadersDefaultCellStyle.BackColor   = Helpers.Theme.Primary;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor   = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font        = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.Padding     = new Padding(5, 0, 5, 0);
        }

        private void LoadPlans()
        {
            try
            {
                dgvPlans.DataSource = null;
                dgvPlans.DataSource = DatabaseHelper.GetAllPlans();
                if (dgvPlans.Columns.Contains("PlanID"))  dgvPlans.Columns["PlanID"].Visible  = false;
                if (dgvPlans.Columns.Contains("CoachID")) dgvPlans.Columns["CoachID"].Visible = false;
                if (dgvPlans.Columns.Contains("CoachName")) dgvPlans.Columns["CoachName"].HeaderText = "Coach";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private bool ValidateInput(out int coachID)
        {
            coachID = 0;
            if (string.IsNullOrWhiteSpace(txtPlanName.Text))
            {
                MessageBox.Show("Please enter a plan name.");
                return false;
            }
            if (cmbCoach.SelectedValue == null)
            {
                MessageBox.Show("Please add a coach first (Coaches screen), then select one.");
                return false;
            }
            coachID = Convert.ToInt32(cmbCoach.SelectedValue);
            return true;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateInput(out int coachID)) return;
            try
            {
                DatabaseHelper.AddPlan(new MembershipPlan
                {
                    CoachID  = coachID,
                    PlanName = txtPlanName.Text.Trim(),
                    Sessions = (int)numDuration.Value,
                    Price    = numPrice.Value
                });
                MessageBox.Show("Plan added!");
                LoadPlans();
                ClearFields();
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (selectedPlanID == 0)
            {
                MessageBox.Show("Please select a plan from the table first.", "No Plan Selected",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!ValidateInput(out int coachID)) return;
            try
            {
                DatabaseHelper.UpdatePlan(new MembershipPlan
                {
                    PlanID   = selectedPlanID,
                    CoachID  = coachID,
                    PlanName = txtPlanName.Text.Trim(),
                    Sessions = (int)numDuration.Value,
                    Price    = numPrice.Value
                });
                MessageBox.Show("Plan updated successfully!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadPlans();
                ClearFields();
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedPlanID == 0)
            {
                MessageBox.Show("Select a plan first.");
                return;
            }
            if (MessageBox.Show("Delete this plan?", "Confirm",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    DatabaseHelper.DeletePlan(selectedPlanID);
                    MessageBox.Show("Plan deleted.");
                    LoadPlans();
                    ClearFields();
                }
                catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
            }
        }

        private void dgvPlans_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var row          = dgvPlans.Rows[e.RowIndex];
                selectedPlanID   = Convert.ToInt32(row.Cells["PlanID"].Value);
                txtPlanName.Text = row.Cells["PlanName"].Value?.ToString() ?? "";
                numDuration.Value= Math.Max(1, Convert.ToInt32(row.Cells["Sessions"].Value));
                numPrice.Value   = Convert.ToDecimal(row.Cells["Price"].Value);
                if (row.Cells["CoachID"].Value != null && row.Cells["CoachID"].Value != DBNull.Value)
                    cmbCoach.SelectedValue = Convert.ToInt32(row.Cells["CoachID"].Value);
            }
        }

        private void ClearFields()
        {
            selectedPlanID   = 0;
            txtPlanName.Clear();
            numDuration.Value = 8;
            numPrice.Value    = 0;
        }
    }
}
