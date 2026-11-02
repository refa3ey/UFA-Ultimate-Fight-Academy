using System;
using System.Drawing;
using System.Drawing.Printing;
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
        private ComboBox cmbPlan;   // pick the plan being paid for (replaces the amount field)

        public PaymentForm()
        {
            InitializeComponent();
            SetupPlanCombo();
            LoadMembers();
            if (cmbMethod.Items.Count > 0)
                cmbMethod.SelectedIndex = 0;
            SyncPlanToMember();
        }

        // Replace the amount box with a Plan dropdown; the amount = the plan's price.
        private void SetupPlanCombo()
        {
            lblAmount.Text    = "PLAN";
            numAmount.Visible = false;

            cmbPlan = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font          = new Font("Segoe UI", 10F),
                Location      = numAmount.Location,
                Size          = numAmount.Size
            };
            panelCard.Controls.Add(cmbPlan);
            cmbPlan.BringToFront();

            try
            {
                cmbPlan.DataSource    = null;
                cmbPlan.DisplayMember = "Label";   // "Coach - Plan (N sessions, price EGP)"
                cmbPlan.ValueMember   = "PlanID";
                cmbPlan.DataSource    = DatabaseHelper.GetAllPlans();
            }
            catch (Exception ex) { MessageBox.Show("Error loading plans: " + ex.Message); }

            cmbMember.SelectedIndexChanged += (s, e) => SyncPlanToMember();
        }

        // When a member is chosen, preselect the plan they are currently on.
        private void SyncPlanToMember()
        {
            var m = cmbMember.SelectedItem as Member;
            if (m == null || cmbPlan == null || cmbPlan.Items.Count == 0) return;
            if (m.PlanID > 0)
                try { cmbPlan.SelectedValue = m.PlanID; } catch { }
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
                cmbMember.DataSource    = null;
                cmbMember.DisplayMember = "Name";
                cmbMember.ValueMember   = "MemberID";
                cmbMember.DataSource    = DatabaseHelper.GetAllMembers();
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
            var selectedPlan = cmbPlan?.SelectedItem as MembershipPlan;
            if (selectedPlan == null)
            {
                MessageBox.Show("Please select a plan.");
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
                    PlanID   = selectedPlan.PlanID,
                    Amount   = selectedPlan.Price,
                    Date     = dtpDate.Value,
                    Method   = cmbMethod.SelectedItem.ToString()
                };

                var selectedMember = (Member)cmbMember.SelectedItem;
                DatabaseHelper.AddPayment(payment);

                string planResult = DatabaseHelper.ApplyPlanPayment(payment.MemberID, selectedPlan);
                dtpDate.Value = DateTime.Now;

                // Cleared so the page is ready for the next payment, and pressing Save
                // again can't record the same payment twice.
                cmbMember.SelectedIndex = -1;

                var result = MessageBox.Show(this,
                    $"{selectedMember.Name} paid {selectedPlan.Price:0} EGP for " +
                    $"{selectedPlan.PlanName} (Coach {selectedPlan.CoachName}).\n\n{planResult}\n\nPrint a receipt now?",
                    "Payment Recorded", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (result == DialogResult.Yes)
                {
                    string planLine = $"{selectedPlan.PlanName}  -  {selectedPlan.CoachName}";
                    try
                    {
                        if (PrinterSettings.InstalledPrinters.Count > 0)
                            PrintReceipt(payment, selectedMember.Name, planLine);
                        else
                            SaveReceiptAsPdf(payment, selectedMember.Name, planLine);
                    }
                    catch (Exception printEx)
                    {
                        MessageBox.Show(this, "Could not print the receipt: " + printEx.Message,
                            "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void PrintReceipt(Payment payment, string memberName, string planLine)
        {
            using (var document = BuildReceipt(payment, memberName, planLine))
            using (var dialog = new PrintDialog { Document = document, UseEXDialog = true })
                if (dialog.ShowDialog(this) == DialogResult.OK)
                    document.Print();
        }

        // For a PC with no printer installed.
        private void SaveReceiptAsPdf(Payment payment, string memberName, string planLine)
        {
            string path = ExportDialog.PromptForPDFPath(
                $"Receipt_{memberName.Replace(" ", "_")}_{payment.Date:yyyyMMdd}.pdf");
            if (path == null) return;
            ExportHelper.ExportPaymentReceiptToPDF(path, payment, memberName, planLine);
            ExportHelper.OpenFile(path);
        }

        private static PrintDocument BuildReceipt(Payment payment, string memberName, string planLine)
        {
            var document = new PrintDocument { DocumentName = "UFA Receipt" };
            document.PrintPage += (s, e) =>
            {
                using (var title = new Font("Segoe UI", 20F, FontStyle.Bold))
                using (var heading = new Font("Segoe UI", 12F, FontStyle.Bold))
                using (var text = new Font("Segoe UI", 12F))
                {
                    float left = e.MarginBounds.Left, y = e.MarginBounds.Top;
                    e.Graphics.DrawString("Ultimate Fight Academy", title, Brushes.Black, left, y);
                    y += 45;
                    e.Graphics.DrawString("Payment Receipt", heading, Brushes.Black, left, y);
                    y += 40;
                    e.Graphics.DrawLine(Pens.Black, left, y, e.MarginBounds.Right, y);
                    y += 15;
                    foreach (var (label, value) in new[]
                    {
                        ("Member", memberName),
                        ("Plan / Coach", planLine),
                        ("Amount", $"{payment.Amount:N0} EGP"),
                        ("Date", payment.Date.ToString("MMM dd, yyyy")),
                        ("Paid by", payment.Method)
                    })
                    {
                        e.Graphics.DrawString(label, heading, Brushes.Black, left, y);
                        e.Graphics.DrawString(value, text, Brushes.Black, left + 150, y);
                        y += 32;
                    }
                }
            };
            return document;
        }
    }
}
