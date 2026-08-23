using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Drawing.Text;
using System.Windows.Forms;
using GYM_Desktop_app.Database;
using GYM_Desktop_app.Helpers;
using GYM_Desktop_app.Models;

namespace GYM_Desktop_app.Forms
{
    public partial class MemberCardPrintForm : Form
    {
        private Member _member;
        private Bitmap _cardBitmap;

        private bool _dragging;
        private Point _dragStart;

        public MemberCardPrintForm(Member member)
        {
            _member = member;
            InitializeComponent();
        }

        private void MemberCardPrintForm_Load(object sender, EventArgs e)
        {
            _cardBitmap = RenderCard(_member);
            picCardPreview.Image = _cardBitmap;
        }

        // ===== DRAG =====
        private void pnlTitleBar_MouseDown(object sender, MouseEventArgs e)
        {
            _dragging = true; _dragStart = e.Location;
        }
        private void pnlTitleBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (_dragging)
                Location = new Point(Location.X + e.X - _dragStart.X, Location.Y + e.Y - _dragStart.Y);
        }
        private void pnlTitleBar_MouseUp(object sender, MouseEventArgs e) => _dragging = false;

        // ===== CARD RENDER =====
        private Bitmap RenderCard(Member m)
        {
            int cw = 856, ch = 540;
            var bmp = new Bitmap(cw, ch);
            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

                // Background
                g.Clear(Color.White);

                // Top stripe
                using (var b = new SolidBrush(Color.FromArgb(26, 26, 26)))
                    g.FillRectangle(b, 0, 0, cw, 80);

                using (var f = new Font("Segoe UI", 28, FontStyle.Bold))
                    g.DrawString("UFA", f, Brushes.White, 28, 16);

                // Member ID badge in top-right of stripe
                using (var f = new Font("Segoe UI", 13, FontStyle.Bold))
                    g.DrawString($"MBR-{m.MemberID:D5}", f, Brushes.White, cw - 220, 26);

                // Bottom stripe
                using (var b = new SolidBrush(Color.FromArgb(26, 26, 26)))
                    g.FillRectangle(b, 0, 510, cw, 30);

                string bottomText = "Show this card at the entrance";
                using (var f = new Font("Segoe UI", 9))
                {
                    var sz = g.MeasureString(bottomText, f);
                    g.DrawString(bottomText, f, Brushes.White, (cw - sz.Width) / 2, 517);
                }

                // Photo placeholder
                using (var b = new SolidBrush(Color.FromArgb(210, 210, 210)))
                    g.FillRectangle(b, 20, 100, 170, 250);
                using (var f = new Font("Segoe UI", 11))
                {
                    g.DrawString("PHOTO", f, Brushes.Gray, 62, 215);
                }

                // Right side: member info
                int rx = 210;
                using (var f = new Font("Segoe UI", 22, FontStyle.Bold))
                    g.DrawString(m.Name, f, new SolidBrush(Color.FromArgb(33, 33, 33)), rx, 100);

                string planName = "Member";
                try { planName = DatabaseHelper.GetPlanNameByID(m.PlanID); } catch { }

                using (var f = new Font("Segoe UI", 13))
                    g.DrawString(planName, f, new SolidBrush(Color.FromArgb(100, 100, 100)), rx, 140);

                using (var f = new Font("Segoe UI", 13))
                    g.DrawString($"Sessions left: {m.SessionsRemaining} / {m.SessionsTotal}", f,
                        new SolidBrush(Color.FromArgb(60, 60, 60)), rx, 170);

                using (var f = new Font("Courier New", 15, FontStyle.Bold))
                    g.DrawString($"MBR-{m.MemberID:D5}", f,
                        new SolidBrush(Color.FromArgb(26, 26, 26)), rx, 208);

                // Decorative accent line
                using (var pen = new Pen(Color.FromArgb(242, 101, 34), 2))
                    g.DrawLine(pen, rx, 255, rx + 360, 255);

                // QR code bottom-right
                try
                {
                    using (var qr = QRHelper.GenerateMemberQR(m.MemberID, 4))
                        g.DrawImage(qr, cw - 175, 340, 155, 155);
                }
                catch { }
            }
            return bmp;
        }

        // ===== BUTTONS =====
        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (_cardBitmap == null) return;
            var doc = new PrintDocument();
            doc.PrintPage += (s, pe) =>
            {
                var bounds = pe.MarginBounds;
                float scale = Math.Min((float)bounds.Width / _cardBitmap.Width,
                                       (float)bounds.Height / _cardBitmap.Height);
                int dw = (int)(_cardBitmap.Width * scale);
                int dh = (int)(_cardBitmap.Height * scale);
                int dx = bounds.Left + (bounds.Width - dw) / 2;
                int dy = bounds.Top  + (bounds.Height - dh) / 2;
                pe.Graphics.DrawImage(_cardBitmap, dx, dy, dw, dh);
            };

            using (var dlg = new PrintDialog { Document = doc })
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                    doc.Print();
            }
        }

        private void btnSaveImage_Click(object sender, EventArgs e)
        {
            if (_cardBitmap == null) return;
            using (var dlg = new SaveFileDialog())
            {
                dlg.Title    = "Save Member Card";
                dlg.Filter   = "PNG Image|*.png";
                dlg.FileName = $"MemberCard-MBR-{_member.MemberID}.png";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    _cardBitmap.Save(dlg.FileName, System.Drawing.Imaging.ImageFormat.Png);
                    MessageBox.Show("Card saved successfully!", "Saved",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            _cardBitmap?.Dispose();
        }
    }
}
