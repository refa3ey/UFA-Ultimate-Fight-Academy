using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using GYM_Desktop_app.Database;
using GYM_Desktop_app.Helpers;
using GYM_Desktop_app.Models;

namespace GYM_Desktop_app.Forms
{
    public partial class PaymentForm : Form
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(int l, int t, int r, int b, int w, int h);

        private bool _dragging;
        private Point _dragStart;

        private Label lblPlanInfo;

        public PaymentForm()
        {
            InitializeComponent();
            SetupPlanInfo();
            LoadMembers();
            if (cmbMethod.Items.Count > 0)
                cmbMethod.SelectedIndex = 0;
            UpdatePlanInfo();
        }

        // Shows which coach + plan the selected member is paying for, and prefills the amount
        private void SetupPlanInfo()
        {
            lblPlanInfo = new Label
            {
                Font      = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(242, 101, 34),
                Location  = new Point(20, 73),
                Size      = new Size(510, 16),
                Text      = ""
            };
            panelCard.Controls.Add(lblPlanInfo);
            lblPlanInfo.BringToFront();
            cmbMember.SelectedIndexChanged += (s, e) => UpdatePlanInfo();
        }

        private void UpdatePlanInfo()
        {
            var m = cmbMember.SelectedItem as Member;
            if (m == null) { if (lblPlanInfo != null) lblPlanInfo.Text = ""; return; }
            decimal price = 0m;
            try { var pl = DatabaseHelper.GetPlanById(m.PlanID); if (pl != null) price = pl.Price; } catch { }
            string coach = string.IsNullOrEmpty(m.CoachName) ? "-" : m.CoachName;
            string plan  = string.IsNullOrEmpty(m.PlanName) ? "-" : m.PlanName;
            lblPlanInfo.Text = $"Coach: {coach}    •    Plan: {plan}    •    {price:0} EGP";
            if (price > 0) numAmount.Value = Math.Min(price, numAmount.Maximum);
        }

        private void PaymentForm_Load(object sender, EventArgs e)
        {
            Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 16, 16));
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

        private void LoadMembers()
        {
            try
            {
                var members          = DatabaseHelper.GetAllMembers();
                cmbMember.DataSource    = members;
                cmbMember.DisplayMember = "Name";
                cmbMember.ValueMember   = "MemberID";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading members: " + ex.Message);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cmbMember.SelectedValue == null)
            {
                MessageBox.Show("Please select a member.");
                return;
            }

            if (numAmount.Value <= 0)
            {
                MessageBox.Show("Amount must be greater than zero.");
                return;
            }

            if (cmbMethod.SelectedItem == null)
            {
                MessageBox.Show("Please select a payment method.");
                return;
            }

            try
            {
                var payment = new Payment
                {
                    MemberID = Convert.ToInt32(cmbMember.SelectedValue),
                    Amount   = numAmount.Value,
                    Date     = dtpDate.Value,
                    Method   = cmbMethod.SelectedItem.ToString()
                };

                var selectedMember = (Member)cmbMember.SelectedItem;
                DatabaseHelper.AddPayment(payment);

                numAmount.Value = 0;
                dtpDate.Value   = DateTime.Now;

                var result = MessageBox.Show("Payment recorded successfully!\n\nPrint a receipt now?",
                    "Success", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (result == DialogResult.Yes)
                {
                    string pdfPath = ExportDialog.PromptForPDFPath(
                        $"Receipt_{selectedMember.Name.Replace(" ", "_")}_{payment.Date:yyyyMMdd}.pdf");
                    if (pdfPath != null)
                    {
                        try
                        {
                            string planLine = $"{(selectedMember.PlanName ?? "-")}  —  Coach {(selectedMember.CoachName ?? "-")}";
                            ExportHelper.ExportPaymentReceiptToPDF(pdfPath, payment, selectedMember.Name, planLine);
                            ExportHelper.OpenFile(pdfPath);
                        }
                        catch (Exception pdfEx)
                        {
                            MessageBox.Show("Could not generate receipt: " + pdfEx.Message,
                                "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}
