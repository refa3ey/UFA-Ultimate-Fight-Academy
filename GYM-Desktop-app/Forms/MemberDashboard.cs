using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using GYM_Desktop_app.Database;
using GYM_Desktop_app.Helpers;
using GYM_Desktop_app.Models;

namespace GYM_Desktop_app.Forms
{
    public partial class MemberDashboard : Form
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(int l, int t, int r, int b, int w, int h);

        private bool _dragging;
        private Point _dragStart;
        private User _currentUser;
        private Member _member;

        public MemberDashboard(User user)
        {
            InitializeComponent();
            _currentUser    = user;
            lblWelcome.Text = $"Welcome, {user.Username}!";
            LoadMemberInfo();
        }

        private void MemberDashboard_Load(object sender, EventArgs e)
        {
            Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));
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

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Hide();
            LoginForm login = new LoginForm();
            login.FormClosed += (s, args) => this.Close();
            login.Show();
        }

        private void LoadMemberInfo()
        {
            try
            {
                _member = DatabaseHelper.GetMemberByUserID(_currentUser.UserID);
                if (_member == null) return;

                string coach = string.IsNullOrEmpty(_member.CoachName) ? "" : _member.CoachName + "  •  ";
                lblPlan.Text   = coach + (string.IsNullOrEmpty(_member.PlanName) ? "—" : _member.PlanName);
                lblExpiry.Text = _member.PlanExpiry == default(DateTime)
                    ? $"{_member.SessionsRemaining} / {_member.SessionsTotal} sessions left"
                    : $"{_member.SessionsRemaining} / {_member.SessionsTotal} sessions  •  "
                      + (_member.IsExpired
                            ? $"expired {_member.PlanExpiry:MMM dd, yyyy}"
                            : $"valid until {_member.PlanExpiry:MMM dd, yyyy}");

                bool active  = _member.CanTrain;
                bool warning = active && (_member.IsExpiringSoon || _member.SessionsRemaining <= 2);

                lblStatus.Text      = _member.Status;
                lblStatus.ForeColor = !active ? Color.FromArgb(220, 53, 69)   // red
                                    : warning ? Color.FromArgb(242, 101, 34)  // orange
                                    : Color.FromArgb(40, 167, 69);            // green

                btnSelfCheckIn.Text = active
                    ? $"Sessions remaining:  {_member.SessionsRemaining}"
                    : "Renew your plan";

                var qr = QRHelper.GenerateMemberQR(_member.MemberID);
                picMyQR.Image      = qr;
                lblMemberCode.Text = $"Code: MBR-{_member.MemberID}";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("LoadMemberInfo: " + ex.Message);
            }
        }

        private void btnSaveQR_Click(object sender, EventArgs e)
        {
            if (picMyQR.Image == null) return;
            using (var dlg = new SaveFileDialog())
            {
                dlg.Title    = "Save QR Code";
                dlg.Filter   = "PNG Image|*.png";
                dlg.FileName = $"MBR-{_member?.MemberID}-QR.png";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    picMyQR.Image.Save(dlg.FileName, System.Drawing.Imaging.ImageFormat.Png);
                    MessageBox.Show("QR code saved successfully!", "Saved",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void btnPrintCard_Click(object sender, EventArgs e)
        {
            if (_member == null)
            {
                MessageBox.Show("Member data not loaded yet. Please wait.");
                return;
            }
            new MemberCardPrintForm(_member).ShowDialog();
        }

        private void btnChangePassword_Click(object sender, EventArgs e)
            => new ChangePasswordForm(_currentUser).ShowDialog();

        private void btnSelfCheckIn_Click(object sender, EventArgs e)
        {
            if (_member == null) return;
            string plans = "";
            foreach (var p in DatabaseHelper.GetMemberPlans(_member.MemberID))
                plans += $"{p.CoachName ?? "-"}  -  {p.PlanName ?? "-"}\n"
                       + $"Sessions remaining: {p.SessionsRemaining} of {p.SessionsTotal}\n"
                       + (p.PlanExpiry == default(DateTime) ? "" :
                            p.IsExpired
                                ? $"Plan expired: {p.PlanExpiry:MMM dd, yyyy}  -  please renew\n"
                                : $"Valid until: {p.PlanExpiry:MMM dd, yyyy}  ({p.DaysLeft} day(s) left)\n")
                       + "\n";
            MessageBox.Show(
                plans +
                "Sessions are recorded by the front desk when you train.\n" +
                "Unused sessions do not carry over once the plan expires.",
                "My Sessions", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnViewSchedule_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Your workout schedule:\n\n" +
                "Monday: Cardio (30 min)\n" +
                "Wednesday: Strength Training\n" +
                "Friday: Yoga & Stretching",
                "Workout Schedule", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            this.Hide();
            LoginForm login = new LoginForm();
            login.FormClosed += (s, args) => this.Close();
            login.Show();
        }
    }
}
