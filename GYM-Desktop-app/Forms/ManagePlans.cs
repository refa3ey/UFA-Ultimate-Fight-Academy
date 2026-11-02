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
        private NumericUpDown numLength;
        private ComboBox cmbLengthUnit;
        private Label lblDurationPicker;

        public ManagePlans()
        {
            InitializeComponent();
            SetupCoachUI();
            LoadPlans();
        }

        // Add coach + duration selectors, and relabel the reused fields for the
        // MMA session model. Row 1 of panelInputs runs:
        //   PLAN NAME | COACH | SESSIONS | DURATION | PRICE
        private void SetupCoachUI()
        {
            lblTitle.Text     = "Manage Coach Plans";
            lblPrice.Text     = "PRICE (EGP)";

            // numDuration is the old months spinner, reused as the session count
            lblDuration.Text    = "SESSIONS";
            numDuration.Maximum = 100;
            numDuration.Value   = 8;

            lblPlanName.Location = new Point(20, 15);
            lblPlanName.Size     = new Size(140, 17);
            txtPlanName.Location = new Point(20, 35);
            txtPlanName.Size     = new Size(140, 38);

            lblDuration.Location = new Point(330, 15);
            lblDuration.Size     = new Size(90, 17);
            numDuration.Location = new Point(330, 39);
            numDuration.Size     = new Size(80, 30);

            lblPrice.Location = new Point(580, 15);
            lblPrice.Size     = new Size(200, 17);
            numPrice.Location = new Point(580, 39);
            numPrice.Size     = new Size(200, 30);

            lblCoach = NewFieldLabel("COACH", 170, 150);
            cmbCoach = NewFieldCombo(170, 150);

            lblDurationPicker = NewFieldLabel("DURATION", 420, 150);
            numLength = new NumericUpDown
            {
                Font     = new Font("Segoe UI", 10F),
                Location = new Point(420, 39),
                Size     = new Size(60, 30),
                Minimum  = 1,
                Maximum  = 999,
                Value    = PlanDuration.Default.Value
            };
            cmbLengthUnit = NewFieldCombo(485, 85);
            cmbLengthUnit.Items.AddRange(PlanDuration.Units);
            cmbLengthUnit.SelectedItem = PlanDuration.Default.Unit;

            panelInputs.Controls.Add(lblCoach);
            panelInputs.Controls.Add(cmbCoach);
            panelInputs.Controls.Add(lblDurationPicker);
            panelInputs.Controls.Add(numLength);
            panelInputs.Controls.Add(cmbLengthUnit);

            LoadCoaches();
        }

        private Label NewFieldLabel(string text, int x, int width) => new Label
        {
            Text      = text,
            Font      = new Font("Segoe UI", 8F, FontStyle.Bold),
            ForeColor = Color.FromArgb(100, 100, 100),
            Location  = new Point(x, 15),
            Size      = new Size(width, 17)
        };

        private ComboBox NewFieldCombo(int x, int width) => new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Font          = new Font("Segoe UI", 10F),
            Location      = new Point(x, 39),
            Size          = new Size(width, 30)
        };

        // The duration the manager typed, e.g. 10 + Month or 1 + Year.
        private PlanDuration SelectedDuration =>
            new PlanDuration((int)numLength.Value, cmbLengthUnit.SelectedItem as string);

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

                // DurationValue/Unit stay bound (CellClick reads them) but are shown
                // as the single readable "Duration" column instead. Label is the long
                // dropdown caption - it only truncates here, so hide it too.
                foreach (var col in new[] { "PlanID", "CoachID", "DurationValue", "DurationUnit", "Duration", "Label" })
                    if (dgvPlans.Columns.Contains(col))
                        dgvPlans.Columns[col].Visible = false;

                if (dgvPlans.Columns.Contains("CoachName"))
                    dgvPlans.Columns["CoachName"].HeaderText = "Coach";
                if (dgvPlans.Columns.Contains("DurationLabel"))
                    dgvPlans.Columns["DurationLabel"].HeaderText = "Duration";
                if (dgvPlans.Columns.Contains("PlanName"))
                    dgvPlans.Columns["PlanName"].HeaderText = "Plan";
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
                var d = SelectedDuration;
                DatabaseHelper.AddPlan(new MembershipPlan
                {
                    CoachID       = coachID,
                    PlanName      = txtPlanName.Text.Trim(),
                    Sessions      = (int)numDuration.Value,
                    Price         = numPrice.Value,
                    DurationValue = d.Value,
                    DurationUnit  = d.Unit
                });
                MessageBox.Show(
                    $"Plan added.\n\n{(int)numDuration.Value} sessions, valid {d.Label.ToLower()} from the day a member buys it.",
                    "Plan Added", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                var d = SelectedDuration;
                DatabaseHelper.UpdatePlan(new MembershipPlan
                {
                    PlanID        = selectedPlanID,
                    CoachID       = coachID,
                    PlanName      = txtPlanName.Text.Trim(),
                    Sessions      = (int)numDuration.Value,
                    Price         = numPrice.Value,
                    DurationValue = d.Value,
                    DurationUnit  = d.Unit
                });
                MessageBox.Show(
                    "Plan updated successfully!\n\n"
                    + "Members already on this plan keep the expiry date they were given - "
                    + $"the new duration ({d.Label.ToLower()}) applies from their next renewal.",
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

                var length = PlanDuration.From(
                    Convert.ToInt32(row.Cells["DurationValue"].Value),
                    row.Cells["DurationUnit"].Value?.ToString());
                numLength.Value = Math.Min(length.Value, numLength.Maximum);
                cmbLengthUnit.SelectedItem = length.Unit;
            }
        }

        private void ClearFields()
        {
            selectedPlanID   = 0;
            txtPlanName.Clear();
            numDuration.Value = 8;
            numPrice.Value    = 0;
            numLength.Value = PlanDuration.Default.Value;
            cmbLengthUnit.SelectedItem = PlanDuration.Default.Unit;
        }
    }
}
