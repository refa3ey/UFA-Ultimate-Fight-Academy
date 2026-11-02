using System;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using GYM_Desktop_app.Database;

namespace GYM_Desktop_app.Forms
{
    // Lists deleted members and coaches, and brings them back.
    public class DeletedRecordsForm : Form
    {
        private bool  _dragging;
        private Point _dragStart;

        private readonly DataGridView dgvMembers = new DataGridView();
        private readonly DataGridView dgvCoaches = new DataGridView();

        public DeletedRecordsForm()
        {
            BuildUi();
            LoadData();
        }

        private void BuildUi()
        {
            Text            = "Deleted Records";
            FormBorderStyle = FormBorderStyle.None;
            StartPosition   = FormStartPosition.CenterScreen;
            ClientSize      = new Size(860, 600);
            BackColor       = Color.FromArgb(245, 247, 250);

            var top = new Guna2Panel { FillColor = Color.FromArgb(26, 26, 26), Location = new Point(0, 0), Size = new Size(860, 55) };
            top.MouseDown += (s, e) => { _dragging = true; _dragStart = e.Location; };
            top.MouseMove += (s, e) => { if (_dragging) Location = new Point(Location.X + e.X - _dragStart.X, Location.Y + e.Y - _dragStart.Y); };
            top.MouseUp   += (s, e) => _dragging = false;
            top.Controls.Add(new Label { Text = "Deleted Records", Font = new Font("Segoe UI", 14F, FontStyle.Bold), ForeColor = Color.White, BackColor = Color.Transparent, Location = new Point(20, 12), Size = new Size(400, 30) });
            var close = new Guna2Button { Text = "✕", BorderRadius = 14, FillColor = Color.FromArgb(220, 53, 69), Font = new Font("Segoe UI", 10F, FontStyle.Bold), ForeColor = Color.White, Location = new Point(820, 13), Size = new Size(28, 28) };
            close.Click += (s, e) => Close();
            top.Controls.Add(close);

            var tabs = new TabControl { Location = new Point(10, 62), Size = new Size(840, 528), Font = new Font("Segoe UI", 10F) };
            tabs.TabPages.Add(BuildTab("Members", dgvMembers, RestoreMember));
            tabs.TabPages.Add(BuildTab("Coaches", dgvCoaches, RestoreCoach));

            Controls.Add(top);
            Controls.Add(tabs);
        }

        private TabPage BuildTab(string title, DataGridView grid, EventHandler restore)
        {
            var tab = new TabPage(title) { BackColor = BackColor };
            grid.Location = new Point(10, 10);
            grid.Size     = new Size(810, 425);
            ProductsForm.StyleGrid(grid);
            tab.Controls.Add(grid);
            var btn = new Guna2Button { Text = "Restore Selected", BorderRadius = 8, FillColor = Color.FromArgb(40, 167, 69), Font = new Font("Segoe UI", 10F, FontStyle.Bold), ForeColor = Color.White, Location = new Point(620, 445), Size = new Size(200, 40) };
            btn.Click += restore;
            tab.Controls.Add(btn);
            return tab;
        }

        private void LoadData()
        {
            try
            {
                dgvMembers.DataSource = DatabaseHelper.GetDeletedMembers();
                dgvCoaches.DataSource = DatabaseHelper.GetDeletedCoaches();
                dgvMembers.Columns["ArchiveID"].Visible = false;
                dgvCoaches.Columns["ArchiveID"].Visible = false;
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void RestoreMember(object sender, EventArgs e)
        {
            if (dgvMembers.SelectedRows.Count == 0) { MessageBox.Show("Select a member first."); return; }
            var row = dgvMembers.SelectedRows[0];
            try
            {
                int oldId = Convert.ToInt32(row.Cells["Old ID"].Value);
                int newId = DatabaseHelper.RestoreMember(Convert.ToInt32(row.Cells["ArchiveID"].Value), out string username);
                string message = $"{row.Cells["Name"].Value} is back with ID {newId}, with their payment history.";
                if (newId != oldId)
                    message += $"\n\nTheir old ID {oldId} is used by someone else now, so print them a new card.";
                if (!string.IsNullOrEmpty(username))
                    message += $"\n\nUsername: {username}";
                MessageBox.Show(message, "Restored", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Cannot Restore", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void RestoreCoach(object sender, EventArgs e)
        {
            if (dgvCoaches.SelectedRows.Count == 0) { MessageBox.Show("Select a coach first."); return; }
            var row = dgvCoaches.SelectedRows[0];
            try
            {
                int newId = DatabaseHelper.RestoreCoach(Convert.ToInt32(row.Cells["ArchiveID"].Value));
                MessageBox.Show(
                    $"{row.Cells["Name"].Value} is back with ID {newId}, with their plans and payment history.\n\n"
                    + "Members who had no coach since the delete are back with this coach.",
                    "Restored", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }
    }
}
