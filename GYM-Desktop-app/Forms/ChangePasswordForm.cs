using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using GYM_Desktop_app.Models;
using GYM_Desktop_app.Database;

namespace GYM_Desktop_app.Forms
{
    public partial class ChangePasswordForm : Form
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(int l, int t, int r, int b, int w, int h);

        private bool _dragging;
        private Point _dragStart;
        private readonly int _userId;

        public ChangePasswordForm(User user)
        {
            InitializeComponent();
            _userId = user.UserID;
        }

        private void ChangePasswordForm_Load(object sender, EventArgs e)
        {
            Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));
        }

        private void panelTopBar_MouseDown(object sender, MouseEventArgs e)
        {
            _dragging = true;
            _dragStart = e.Location;
        }

        private void panelTopBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (_dragging)
                Location = new Point(Location.X + e.X - _dragStart.X, Location.Y + e.Y - _dragStart.Y);
        }

        private void panelTopBar_MouseUp(object sender, MouseEventArgs e) => _dragging = false;

        private void btnClose_Click(object sender, EventArgs e) => this.Close();

        private void btnCancel_Click(object sender, EventArgs e) => this.Close();

        private void btnSave_Click(object sender, EventArgs e)
        {
            string current = txtCurrentPassword.Text;
            string newPwd  = txtNewPassword.Text;
            string confirm = txtConfirmPassword.Text;

            if (string.IsNullOrWhiteSpace(current) || string.IsNullOrWhiteSpace(newPwd) || string.IsNullOrWhiteSpace(confirm))
            {
                Helpers.ModernMessageBox.Error("Please fill in all fields.", "Validation");
                return;
            }

            if (newPwd.Length < 6)
            {
                Helpers.ModernMessageBox.Error("New password must be at least 6 characters.", "Validation");
                return;
            }

            if (newPwd != confirm)
            {
                Helpers.ModernMessageBox.Error("New password and confirmation do not match.", "Validation");
                return;
            }

            // Fetch stored password and verify current password
            string storedHash = GetStoredPassword(_userId);
            if (storedHash == null || (!DatabaseHelper.VerifyPassword(current, storedHash) && storedHash != current))
            {
                Helpers.ModernMessageBox.Error("Current password is incorrect.", "Error");
                return;
            }

            DatabaseHelper.UpdateUserPassword(_userId, DatabaseHelper.HashPassword(newPwd));
            Helpers.ModernMessageBox.Info("Password changed successfully!", "Success");
            this.Close();
        }

        private string GetStoredPassword(int userId)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string sql = "SELECT Password FROM Users WHERE UserID=@id";
                    using (var cmd = new System.Data.SQLite.SQLiteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", userId);
                        var result = cmd.ExecuteScalar();
                        return result?.ToString();
                    }
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
