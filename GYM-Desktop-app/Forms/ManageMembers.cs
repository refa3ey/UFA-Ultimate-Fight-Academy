using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using GYM_Desktop_app.Database;
using GYM_Desktop_app.Helpers;
using GYM_Desktop_app.Models;

namespace GYM_Desktop_app.Forms
{
    public partial class ManageMembers : Form
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(int l, int t, int r, int b, int w, int h);

        private bool _dragging;
        private Point _dragStart;
        private int selectedMemberID = 0;
        private Member _selectedMember = null;
        private List<Member> _allMembers = new List<Member>();

        private Guna.UI2.WinForms.Guna2Button btnUseSession;
        private Guna.UI2.WinForms.Guna2Button btnRenew;

        public ManageMembers()
        {
            InitializeComponent();
            SetupSessionButtons();
            LoadPlans();
            LoadMembers();
        }

        // Staff actions on the selected member: use a session, or renew (refill sessions)
        private void SetupSessionButtons()
        {
            var parent = btnClear?.Parent ?? (Control)this;

            btnUseSession = new Guna.UI2.WinForms.Guna2Button
            {
                BorderRadius = 8,
                FillColor    = Color.FromArgb(60, 60, 60),
                ForeColor    = Color.White,
                Font         = new Font("Segoe UI", 10F, FontStyle.Bold),
                Text         = "  Use a Session (-1)",
                TextAlign    = HorizontalAlignment.Left,
                Size         = new Size(200, 40),
                Location     = new Point(20, 220)
            };
            btnUseSession.Click += BtnUseSession_Click;

            btnRenew = new Guna.UI2.WinForms.Guna2Button
            {
                BorderRadius = 8,
                FillColor    = Color.FromArgb(242, 101, 34),
                ForeColor    = Color.White,
                Font         = new Font("Segoe UI", 10F, FontStyle.Bold),
                Text         = "  Renew Plan (refill sessions)",
                TextAlign    = HorizontalAlignment.Left,
                Size         = new Size(255, 40),
                Location     = new Point(235, 220)
            };
            btnRenew.Click += BtnRenew_Click;

            parent.Controls.Add(btnUseSession);
            parent.Controls.Add(btnRenew);
            btnUseSession.BringToFront();
            btnRenew.BringToFront();
        }

        private void BtnRenew_Click(object sender, EventArgs e)
        {
            if (selectedMemberID == 0)
            {
                MessageBox.Show("Select a member from the table first.");
                return;
            }
            var plan = cmbPlan.SelectedItem as MembershipPlan;
            if (plan == null)
            {
                MessageBox.Show("Choose the plan to renew with (in the Plan dropdown).");
                return;
            }
            // Spell out what the renewal will actually do - each case differs.
            // Only the plan with this coach is touched.
            var current = DatabaseHelper.GetMemberPlanFor(selectedMemberID, plan.CoachID);
            bool monthIsOver = current == null || current.IsExpired;
            int leftover = current?.SessionsRemaining ?? 0;

            string prompt =
                $"Renew this member with \"{plan.PlanName}\" from {plan.CoachName}  ({plan.Sessions} sessions)?\n\n";

            if (current == null)
            {
                prompt += $"They have no plan with {plan.CoachName}, so this adds a new one for {plan.Duration.Label.ToLower()}:\n"
                        + $"  •  {plan.Sessions} sessions\n"
                        + $"  •  Valid until {DatabaseHelper.ComputeExpiry(DateTime.Now, plan):MMM dd, yyyy}\n";
            }
            else if (monthIsOver)
            {
                prompt += $"Their plan has finished, so this starts a NEW {plan.Duration.Label.ToLower()}:\n"
                        + $"  •  Sessions reset to {plan.Sessions}\n"
                        + $"  •  Valid until {DatabaseHelper.ComputeExpiry(DateTime.Now, plan):MMM dd, yyyy}\n";
                if (leftover > 0)
                    prompt += $"\nWARNING: {leftover} unused session(s) from the finished plan will be LOST.";
            }
            else
            {
                prompt += $"Their plan is still running, so this tops them up by {plan.Duration.Label.ToLower()}:\n"
                        + $"  •  Sessions {leftover}  ->  {leftover + plan.Sessions}\n"
                        + $"  •  Valid until {plan.Duration.AddTo(current.PlanExpiry):MMM dd, yyyy}\n";
            }

            prompt += "\n\nSaving a payment on the Payment page already renews the plan. "
                    + "If this member just paid there, choose No.";

            if (MessageBox.Show(prompt, "Renew Plan",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            int newRemaining = DatabaseHelper.RenewPlan(
                selectedMemberID, plan.PlanID, out bool wasReset, out DateTime newExpiry);

            if (newRemaining < 0)
            {
                MessageBox.Show("Could not renew (member or plan not found).",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show(
                $"Renewed.\n\nSessions: {newRemaining}\nValid until: {newExpiry:MMM dd, yyyy}"
                + (wasReset ? $"\n\n({leftover} unused session(s) from the finished plan were dropped.)" : ""),
                "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadMembers();
        }

        private void BtnUseSession_Click(object sender, EventArgs e)
        {
            if (selectedMemberID == 0)
            {
                MessageBox.Show("Select a member from the table first.");
                return;
            }
            if (_selectedMember == null || _selectedMember.MemberPlanID == 0)
            {
                MessageBox.Show("This person has no plan. Use Pay One Session on the dashboard, or subscribe them to a plan.",
                    "No Plan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // The row you clicked decides which coach the session comes off.
            var m = DatabaseHelper.GetMemberPlanRow(_selectedMember.MemberPlanID);
            if (m == null)
            {
                MessageBox.Show("Plan not found. Refresh and try again.");
                return;
            }
            if (m.IsExpired)
            {
                MessageBox.Show(
                    $"This member's plan expired on {m.PlanExpiry:MMM dd, yyyy}"
                    + (m.SessionsRemaining > 0
                        ? $" with {m.SessionsRemaining} session(s) unused.\n\nThe plan period is over, so those sessions no longer count."
                        : ".")
                    + "\n\nRenew their plan to start a new period.",
                    "Plan Expired", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (m.SessionsRemaining <= 0)
            {
                MessageBox.Show("This member has no sessions left. Renew their plan first.",
                    "No Sessions", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (MessageBox.Show(
                    $"Mark one session with {m.CoachName ?? "this coach"} as used for {m.Name}?\n\n"
                    + $"{m.PlanName}: {m.SessionsRemaining} left  ->  {m.SessionsRemaining - 1} left", "Use Session",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            int remaining = DatabaseHelper.UseSession(m.MemberPlanID, out string error);
            if (error != null)
            {
                MessageBox.Show(error, "Cannot Use Session",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                LoadMembers();
                return;
            }
            MessageBox.Show(
                $"Session recorded with {m.CoachName ?? "this coach"}. {remaining} session(s) left, valid until {m.PlanExpiry:MMM dd, yyyy}.",
                "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadMembers();
        }

        private void ManageMembers_Load(object sender, EventArgs e)
        {
            Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 16, 16));
            ApplyGridStyle(dgvMembers);
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

        // ===== GRID STYLING =====
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

        // ===== DATA LOADING =====
        private void LoadPlans()
        {
            try
            {
                var plans             = DatabaseHelper.GetAllPlans();
                cmbPlan.DataSource    = plans;
                cmbPlan.DisplayMember = "Label";
                cmbPlan.ValueMember   = "PlanID";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading plans: " + ex.Message);
            }
        }

        private void LoadMembers()
        {
            try
            {
                _allMembers = DatabaseHelper.GetMemberPlanRows();
                ApplyFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading members: " + ex.Message);
            }
        }

        // ===== SEARCH & FILTER =====
        private void ApplyFilters()
        {
            IEnumerable<Member> filtered = _allMembers;

            string search = txtSearch.Text.Trim();
            if (!string.IsNullOrWhiteSpace(search))
            {
                filtered = filtered.Where(m =>
                    (m.Name    ?? "").IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (m.Phone   ?? "").IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (m.Address ?? "").IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            switch (cmbFilter.SelectedIndex)
            {
                case 1: // Active - month still running AND sessions left
                    filtered = filtered.Where(m => m.CanTrain);
                    break;
                case 2: // Expired - the month is over
                    filtered = filtered.Where(m => m.IsExpired);
                    break;
                case 3: // Expiring soon - near the end of the plan's own period
                    filtered = filtered.Where(m => m.IsExpiringSoon);
                    break;
                case 4: // Out of sessions, but the period has not ended yet
                    filtered = filtered.Where(m => m.PlanID > 0 && !m.IsExpired && m.SessionsRemaining <= 0);
                    break;
                case 5: // Low sessions
                    filtered = filtered.Where(m => !m.IsExpired &&
                                                   m.SessionsRemaining > 0 &&
                                                   m.SessionsRemaining <= 2);
                    break;
            }

            var list = filtered.ToList();
            dgvMembers.DataSource = null;
            dgvMembers.DataSource = list;
            lblResultCount.Text = $"Showing {list.Select(m => m.MemberID).Distinct().Count()} of "
                                + $"{_allMembers.Select(m => m.MemberID).Distinct().Count()} members";
        }

        private void txtSearch_TextChanged(object sender, EventArgs e) => ApplyFilters();

        private void cmbFilter_SelectedIndexChanged(object sender, EventArgs e) => ApplyFilters();

        private void btnRefresh_Click(object sender, EventArgs e) => LoadMembers();

        // ===== ROW COLORING =====
        private void dgvMembers_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var list = dgvMembers.DataSource as List<Member>;
            if (list == null || e.RowIndex >= list.Count) return;

            var m = list[e.RowIndex];

            // Walk-ins have no plan: no colour, and a blank expiry date.
            if (m.PlanID == 0)
            {
                if (dgvMembers.Columns[e.ColumnIndex].Name == "PlanExpiry")
                {
                    e.Value = "";
                    e.FormattingApplied = true;
                }
                return;
            }

            // Red = cannot train (month over, or out of sessions).
            // Yellow = still usable but running out of days or sessions.
            if (m.IsExpired || m.SessionsRemaining <= 0)
                e.CellStyle.BackColor = Color.FromArgb(255, 230, 230);
            else if (m.IsExpiringSoon || m.SessionsRemaining <= 2)
                e.CellStyle.BackColor = Color.FromArgb(255, 248, 220);
        }

        // ===== COLUMN POLISH =====
        private void dgvMembers_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (dgvMembers.Columns.Count == 0) return;

            foreach (var col in new[] { "UserID", "CoachID", "PlanID", "PlanStart",
                                        "IsExpired", "CanTrain", "IsExpiringSoon", "ExpiryWarningDays" })
                if (dgvMembers.Columns.Contains(col))
                    dgvMembers.Columns[col].Visible = false;

            var headers = new Dictionary<string, string>
            {
                { "MemberID",          "ID"        },
                { "Name",              "Name"      },
                { "Phone",             "Phone"     },
                { "Age",               "Age"       },
                { "Address",           "Address"   },
                { "JoinDate",          "Joined"    },
                { "CoachName",         "Coach"     },
                { "PlanName",          "Plan"      },
                { "SessionsTotal",     "Total"     },
                { "SessionsRemaining", "Left"      },
                { "PlanExpiry",        "Expires"   },
                { "DaysLeft",          "Days Left" },
                { "Status",            "Status"    }
            };

            foreach (var kv in headers)
                if (dgvMembers.Columns.Contains(kv.Key))
                    dgvMembers.Columns[kv.Key].HeaderText = kv.Value;

            foreach (var col in new[] { "JoinDate", "PlanExpiry" })
                if (dgvMembers.Columns.Contains(col))
                    dgvMembers.Columns[col].DefaultCellStyle.Format = "MMM dd, yyyy";

            // A member's plans read left to right next to their name.
            int position = 0;
            foreach (var col in new[] { "MemberID", "Name", "CoachName", "PlanName", "SessionsRemaining",
                                        "SessionsTotal", "PlanExpiry", "Status" })
                if (dgvMembers.Columns.Contains(col))
                    dgvMembers.Columns[col].DisplayIndex = position++;
        }

        // ===== CRUD HANDLERS =====
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs()) return;

            try
            {
                string username = Microsoft.VisualBasic.Interaction.InputBox(
                    "Enter username for the new member:", "Username", "");
                if (string.IsNullOrWhiteSpace(username))
                {
                    MessageBox.Show("Username is required.");
                    return;
                }

                string password = Microsoft.VisualBasic.Interaction.InputBox(
                    "Enter password for the new member:", "Password", "");
                if (string.IsNullOrWhiteSpace(password))
                {
                    MessageBox.Show("Password is required.");
                    return;
                }

                var selectedPlan = (MembershipPlan)cmbPlan.SelectedItem;
                var member = new Member
                {
                    Name              = txtName.Text.Trim(),
                    Phone             = txtPhone.Text.Trim(),
                    Age               = (int)numAge.Value,
                    Address           = txtAddress.Text.Trim(),
                    JoinDate          = DateTime.Now,
                    CoachID           = selectedPlan.CoachID,
                    PlanID            = selectedPlan.PlanID,
                    SessionsTotal     = selectedPlan.Sessions,
                    SessionsRemaining = selectedPlan.Sessions
                };

                DatabaseHelper.AddMember(member, username, password);
                MessageBox.Show("Member added successfully!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadMembers();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (selectedMemberID == 0)
            {
                MessageBox.Show("Please select a member from the table first.");
                return;
            }

            bool walkIn = _selectedMember != null && _selectedMember.PlanID == 0;
            if (!ValidateInputs(planRequired: false)) return;

            try
            {
                var selectedPlan = cmbPlan.SelectedItem as MembershipPlan;
                var member = new Member
                {
                    MemberID          = selectedMemberID,
                    Name              = txtName.Text.Trim(),
                    Phone             = txtPhone.Text.Trim(),
                    Age               = (int)numAge.Value,
                    Address           = txtAddress.Text.Trim()
                };

                // Choosing a different plan on a row changes that plan only.
                bool planChanged = !walkIn && _selectedMember != null && selectedPlan != null
                                   && selectedPlan.PlanID != _selectedMember.PlanID;
                if (planChanged && !DatabaseHelper.ChangeMemberPlan(_selectedMember.MemberPlanID, selectedMemberID, selectedPlan))
                {
                    MessageBox.Show($"This member already has a plan with {selectedPlan.CoachName}.\n\n"
                                    + "Select that row instead, or record a payment for the new plan.",
                        "Plan Not Changed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DatabaseHelper.UpdateMember(member);

                // A walk-in with a plan chosen is subscribing: this starts the plan.
                if (walkIn && selectedPlan != null)
                {
                    int sessions = DatabaseHelper.RenewPlan(
                        selectedMemberID, selectedPlan.PlanID, out bool _, out DateTime expiry);
                    MessageBox.Show(
                        $"{member.Name} is now subscribed to {selectedPlan.PlanName}.\n\n"
                        + $"Sessions: {sessions}\nValid until: {expiry:MMM dd, yyyy}",
                        "Subscribed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                    MessageBox.Show("Member updated successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadMembers();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedMemberID == 0)
            {
                MessageBox.Show("Please select a member to delete.");
                return;
            }

            var result = MessageBox.Show("Delete this member and ALL their plans?\n\nYou can bring them back from Deleted Records.",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    DatabaseHelper.DeleteMember(selectedMemberID);
                    MessageBox.Show("Member deleted.\n\nYou can bring them back from Deleted Records.");
                    LoadMembers();
                    ClearFields();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void btnClear_Click(object sender, EventArgs e) => ClearFields();

        private void dgvMembers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var row          = dgvMembers.Rows[e.RowIndex];
                selectedMemberID = Convert.ToInt32(row.Cells["MemberID"].Value);
                txtName.Text     = row.Cells["Name"].Value.ToString();
                txtPhone.Text    = row.Cells["Phone"].Value?.ToString() ?? "";
                // Walk-ins have no age on file (0), which the box can't show.
                int age          = Convert.ToInt32(row.Cells["Age"].Value);
                numAge.Value     = age < numAge.Minimum ? 18 : Math.Min(age, numAge.Maximum);
                txtAddress.Text  = row.Cells["Address"].Value?.ToString() ?? "";

                int planID = Convert.ToInt32(row.Cells["PlanID"].Value);
                var plans  = (List<MembershipPlan>)cmbPlan.DataSource;
                var plan   = plans.FirstOrDefault(p => p.PlanID == planID);
                if (plan != null)
                    cmbPlan.SelectedItem = plan;
                else
                    cmbPlan.SelectedIndex = -1;   // walk-in, or a plan that was deleted

                _selectedMember = row.DataBoundItem as Member;

                try
                {
                    var old = picMemberQR.Image;
                    picMemberQR.Image = QRHelper.GenerateMemberQR(selectedMemberID, 4);
                    old?.Dispose();
                }
                catch { }
            }
        }

        private void btnPrintCard_Click(object sender, EventArgs e)
        {
            if (_selectedMember == null)
            {
                MessageBox.Show("Please select a member from the table first.");
                return;
            }
            new MemberCardPrintForm(_selectedMember).ShowDialog();
        }

        private void btnExportMembers_Click(object sender, EventArgs e)
        {
            if (_allMembers == null || _allMembers.Count == 0)
            {
                MessageBox.Show("No members to export.", "Export",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string path = ExportDialog.PromptForExcelPath(
                $"Members_{DateTime.Now:yyyyMMdd}.xlsx");
            if (path == null) return;
            try
            {
                ExportHelper.ExportMembersToExcel(path, _allMembers);
                if (MessageBox.Show("Excel file saved.\n\nOpen it now?", "Export Complete",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    ExportHelper.OpenFile(path);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Export failed: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateInputs(bool planRequired = true)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Name is required.");
                return false;
            }
            if (planRequired && cmbPlan.SelectedItem == null)
            {
                MessageBox.Show("Please select a plan.");
                return false;
            }
            return true;
        }

        private void ClearFields()
        {
            selectedMemberID = 0;
            _selectedMember  = null;
            txtName.Clear();
            txtPhone.Clear();
            numAge.Value = 18;
            txtAddress.Clear();
            if (cmbPlan.Items.Count > 0)
                cmbPlan.SelectedIndex = 0;
            var old = picMemberQR.Image;
            picMemberQR.Image = null;
            old?.Dispose();
        }
    }
}
