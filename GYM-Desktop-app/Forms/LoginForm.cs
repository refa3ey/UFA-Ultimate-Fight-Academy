using GYM_Desktop_app.Models;
using GYM_Desktop_app.Database;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace GYM_Desktop_app.Forms
{
    public partial class LoginForm : Form
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(int l, int t, int r, int b, int w, int h);

        private bool _dragging;
        private System.Drawing.Point _dragStart;

        public LoginForm()
        {
            InitializeComponent();
            AddLogo();
        }

        private void AddLogo()
        {
            try
            {
                string path = System.IO.Path.Combine(Application.StartupPath, "Resources", "logo.png");
                if (!System.IO.File.Exists(path)) return;
                var pic = new PictureBox
                {
                    Image     = System.Drawing.Image.FromFile(path),
                    SizeMode  = PictureBoxSizeMode.Zoom,
                    Size      = new System.Drawing.Size(150, 150),
                    Location  = new System.Drawing.Point(110, 28),
                    BackColor = System.Drawing.Color.Transparent
                };
                panelLeft.Controls.Add(pic);
                pic.BringToFront();
                lblTagline.Text = "Ultimate Fight Academy";
            }
            catch { }
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));
        }

        private void DragPanel_MouseDown(object sender, MouseEventArgs e)
        {
            _dragging = true;
            _dragStart = e.Location;
        }

        private void DragPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (_dragging)
                Location = new System.Drawing.Point(Location.X + e.X - _dragStart.X, Location.Y + e.Y - _dragStart.Y);
        }

        private void DragPanel_MouseUp(object sender, MouseEventArgs e) => _dragging = false;

        private void btnClose_Click(object sender, EventArgs e) => this.Close();

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter username and password.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                User user = DatabaseHelper.ValidateUser(username, password);

                if (user != null)
                {
                    this.Hide();
                    if (user.Role == "Admin")
                    {
                        AdminDashboard adminForm = new AdminDashboard(user);
                        adminForm.FormClosed += (s, args) => this.Close();
                        adminForm.Show();
                    }
                    else
                    {
                        MemberDashboard memberForm = new MemberDashboard(user);
                        memberForm.FormClosed += (s, args) => this.Close();
                        memberForm.Show();
                    }
                }
                else
                {
                    MessageBox.Show("Invalid username or password.", "Login Failed",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database error: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            RegisterForm registerForm = new RegisterForm();
            registerForm.ShowDialog();
        }
    }
}
