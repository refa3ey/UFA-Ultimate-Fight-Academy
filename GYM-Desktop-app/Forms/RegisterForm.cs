using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using GYM_Desktop_app.Models;
using GYM_Desktop_app.Database;

namespace GYM_Desktop_app.Forms
{
    public partial class RegisterForm : Form
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(int l, int t, int r, int b, int w, int h);

        private bool _dragging;
        private System.Drawing.Point _dragStart;

        public RegisterForm()
        {
            InitializeComponent();
            LoadPlans();
        }

        private void RegisterForm_Load(object sender, EventArgs e)
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

        private void LoadPlans()
        {
            try
            {
                var plans = DatabaseHelper.GetAllPlans();
                cmbPlan.DataSource    = plans;
                cmbPlan.DisplayMember = "Label";
                cmbPlan.ValueMember   = "PlanID";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading plans: " + ex.Message);
            }
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text) ||
                string.IsNullOrWhiteSpace(txtUsername.Text) ||
                string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Please fill in all required fields.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (txtPassword.Text != txtConfirmPassword.Text)
            {
                MessageBox.Show("Passwords do not match!", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cmbPlan.SelectedValue == null)
            {
                MessageBox.Show("Please select a membership plan.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var selectedPlan     = (MembershipPlan)cmbPlan.SelectedItem;

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

                DatabaseHelper.AddMember(member, txtUsername.Text.Trim(), txtPassword.Text.Trim());

                MessageBox.Show("Registration successful! You can now login.", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Registration failed: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e) => this.Close();

        private void chkShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            char pwdChar                   = chkShowPassword.Checked ? '\0' : '*';
            txtPassword.PasswordChar       = pwdChar;
            txtConfirmPassword.PasswordChar = pwdChar;
        }
    }
}
