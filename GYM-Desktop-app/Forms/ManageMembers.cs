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

        public ManageMembers()
        {
            InitializeComponent();
            SetupUseSessionButton();
            LoadPlans();
            LoadMembers();
        }

        // Staff action: mark one training session as used for the selected member
        private void SetupUseSessionButton()
        {
            btnUseSession = new Guna.UI2.WinForms.Guna2Button
            {
                BorderRadius = 8,
                FillColor    = Color.FromArgb(242, 101, 34),
                ForeColor    = Color.White,
                Font         = new Font("Segoe UI", 10F, FontStyle.Bold),
                Text         = "  Use Session",
                TextAlign    = HorizontalAlignment.Left,
                Size         = new Size(148, 40),
                Location     = new Point(660, 165)
            };
            btnUseSession.Click += BtnUseSession_Click;
            if (btnClear?.Parent != null) btnClear.Parent.Controls.Add(btnUseSession);
            else this.Controls.Add(btnUseSession);
            btnUseSession.BringToFront();
        }

        private void BtnUseSession_Click(object sender, EventArgs e)
        {
            if (selectedMemberID == 0)
            {
                MessageBox.Show("Select a member from the table first.");
                return;
            }
            if (!DatabaseHelper.HasSessionsLeft(selectedMemberID))
            {
                MessageBox.Show("This member has no sessions left. Renew their plan first.",
                    "No Sessions", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (MessageBox.Show("Mark one session as used for this member?", "Use Session",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            int remaining = DatabaseHelper.UseSession(selectedMemberID);
            MessageBox.Show(remaining >= 0
                    ? $"Session recorded. {remaining} session(s) left."
                    : "No sessions left.",
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
                _allMembers = DatabaseHelper.GetAllMembers();
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
                case 1: // Active (has sessions left)
                    filtered = filtered.Where(m => m.SessionsRemaining > 0);
                    break;
                case 2: // Finished (no sessions left)
                    filtered = filtered.Where(m => m.SessionsRemaining <= 0);
                    break;
                case 3: // Low sessions
                    filtered = filtered.Where(m => m.SessionsRemaining > 0 &&
                                                   m.SessionsRemaining <= 2);
                    break;
            }

            var list = filtered.ToList();
            dgvMembers.DataSource = null;
            dgvMembers.DataSource = list;
            lblResultCount.Text = $"Showing {list.Count} of {_allMembers.Count} members";
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

            if (m.SessionsRemaining <= 0)
                e.CellStyle.BackColor = Color.FromArgb(255, 230, 230);
            else if (m.SessionsRemaining <= 2)
                e.CellStyle.BackColor = Color.FromArgb(255, 248, 220);
        }

        // ===== COLUMN POLISH =====
        private void dgvMembers_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (dgvMembers.Columns.Count == 0) return;

            foreach (var col in new[] { "UserID", "CoachID", "PlanID" })
                if (dgvMembers.Columns.Contains(col))
                    dgvMembers.Columns[col].Visible = false;

            var headers = new Dictionary<string, string>
            {
                { "MemberID",          "ID"      },
                { "Name",              "Name"    },
                { "Phone",             "Phone"   },
                { "Age",               "Age"     },
                { "Address",           "Address" },
                { "JoinDate",          "Joined"  },
                { "CoachName",         "Coach"   },
                { "PlanName",          "Plan"    },
                { "SessionsTotal",     "Total"   },
                { "SessionsRemaining", "Left"    }
            };

            foreach (var kv in headers)
                if (dgvMembers.Columns.Contains(kv.Key))
                    dgvMembers.Columns[kv.Key].HeaderText = kv.Value;

            if (dgvMembers.Columns.Contains("JoinDate"))
                dgvMembers.Columns["JoinDate"].DefaultCellStyle.Format = "MMM dd, yyyy";
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

            if (!ValidateInputs()) return;

            try
            {
                var selectedPlan = (MembershipPlan)cmbPlan.SelectedItem;
                int remaining = _selectedMember != null ? _selectedMember.SessionsRemaining : selectedPlan.Sessions;
                var member = new Member
                {
                    MemberID          = selectedMemberID,
                    Name              = txtName.Text.Trim(),
                    Phone             = txtPhone.Text.Trim(),
                    Age               = (int)numAge.Value,
                    Address           = txtAddress.Text.Trim(),
                    CoachID           = selectedPlan.CoachID,
                    PlanID            = selectedPlan.PlanID,
                    SessionsTotal     = selectedPlan.Sessions,
                    SessionsRemaining = remaining
                };

                DatabaseHelper.UpdateMember(member);
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

            var result = MessageBox.Show("Are you sure you want to delete this member?",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    DatabaseHelper.DeleteMember(selectedMemberID);
                    MessageBox.Show("Member deleted.");
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
                numAge.Value     = Convert.ToInt32(row.Cells["Age"].Value);
                txtAddress.Text  = row.Cells["Address"].Value?.ToString() ?? "";

                int planID = Convert.ToInt32(row.Cells["PlanID"].Value);
                var plans  = (List<MembershipPlan>)cmbPlan.DataSource;
                var plan   = plans.FirstOrDefault(p => p.PlanID == planID);
                if (plan != null)
                    cmbPlan.SelectedItem = plan;

                _selectedMember = (dgvMembers.DataSource as List<Member>)
                    ?.FirstOrDefault(m => m.MemberID == selectedMemberID);

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

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Name is required.");
                return false;
            }
            if (cmbPlan.SelectedItem == null)
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
