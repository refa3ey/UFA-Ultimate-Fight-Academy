using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using GYM_Desktop_app.Database;
using GYM_Desktop_app.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace GYM_Desktop_app.Helpers
{
    public static class ExportHelper
    {
        static readonly Color TealMid   = Color.FromArgb(242, 101, 34);
        static readonly Color TealDark  = Color.FromArgb(26, 26, 26);
        static readonly Color LightGray = Color.FromArgb(245, 247, 250);

        // =====================================================================
        // EXCEL
        // =====================================================================

        public static void ExportPaymentsToExcel(string filePath, DataTable dt, string title = "Payment Report")
        {
            using (var pkg = new ExcelPackage())
            {
                var ws = pkg.Workbook.Worksheets.Add("Payments");
                int cols = dt.Columns.Count;

                // Title
                ws.Cells[1, 1].Value = "UFA — " + title;
                ws.Cells[1, 1].Style.Font.Size = 16;
                ws.Cells[1, 1].Style.Font.Bold = true;
                ws.Cells[1, 1, 1, cols].Merge = true;

                ws.Cells[2, 1].Value = $"Generated: {DateTime.Now:MMM dd, yyyy HH:mm}";
                ws.Cells[2, 1].Style.Font.Color.SetColor(Color.Gray);
                ws.Cells[2, 1, 2, cols].Merge = true;

                // Header row
                const int HR = 4;
                for (int c = 0; c < cols; c++)
                {
                    var cell = ws.Cells[HR, c + 1];
                    cell.Value = dt.Columns[c].ColumnName;
                    cell.Style.Font.Bold = true;
                    cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    cell.Style.Fill.BackgroundColor.SetColor(TealMid);
                    cell.Style.Font.Color.SetColor(Color.White);
                }

                // Data rows
                for (int r = 0; r < dt.Rows.Count; r++)
                {
                    int row = HR + 1 + r;
                    for (int c = 0; c < cols; c++)
                    {
                        var cell = ws.Cells[row, c + 1];
                        cell.Value = dt.Rows[r][c] == DBNull.Value ? null : dt.Rows[r][c];
                        if (r % 2 == 1)
                        {
                            cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            cell.Style.Fill.BackgroundColor.SetColor(LightGray);
                        }
                        string colName = dt.Columns[c].ColumnName;
                        if (colName == "Amount" || colName == "Coach Total" || colName == "Total" || colName == "UnitPrice")
                            cell.Style.Numberformat.Format = "#,##0\" EGP\"";
                        else if (colName.Contains("Date"))
                            cell.Style.Numberformat.Format = "mmm dd, yyyy";
                    }
                }

                // Total row
                string sumColumn = dt.Columns.Contains("Amount") ? "Amount"
                                 : dt.Columns.Contains("Total")  ? "Total" : null;
                if (sumColumn != null)
                {
                    int amtCol  = dt.Columns[sumColumn].Ordinal + 1;
                    int totalRow = HR + dt.Rows.Count + 2;
                    ws.Cells[totalRow, amtCol - 1].Value = "TOTAL";
                    ws.Cells[totalRow, amtCol - 1].Style.Font.Bold = true;
                    ws.Cells[totalRow, amtCol].Formula =
                        $"SUM({ws.Cells[HR + 1, amtCol].Address}:{ws.Cells[HR + dt.Rows.Count, amtCol].Address})";
                    ws.Cells[totalRow, amtCol].Style.Numberformat.Format = "#,##0\" EGP\"";
                    ws.Cells[totalRow, amtCol].Style.Font.Bold = true;
                    ws.Cells[totalRow, amtCol].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[totalRow, amtCol].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(220, 250, 220));
                }

                for (int c = 1; c <= cols; c++)
                    ws.Column(c).AutoFit();
                ws.View.FreezePanes(HR + 1, 1);
                pkg.SaveAs(new FileInfo(filePath));
            }
        }

        public static void ExportMembersToExcel(string filePath, List<Member> members)
        {
            using (var pkg = new ExcelPackage())
            {
                var ws = pkg.Workbook.Worksheets.Add("Members");

                ws.Cells[1, 1].Value = "UFA — Members List";
                ws.Cells[1, 1].Style.Font.Size = 16;
                ws.Cells[1, 1].Style.Font.Bold = true;
                ws.Cells[1, 1, 1, 9].Merge = true;

                ws.Cells[2, 1].Value = $"Exported: {DateTime.Now:MMM dd, yyyy}   |   {members.Count} members";
                ws.Cells[2, 1].Style.Font.Color.SetColor(Color.Gray);
                ws.Cells[2, 1, 2, 9].Merge = true;

                string[] headers = { "ID", "Name", "Phone", "Age", "Coach", "Joined", "Expires", "Sessions Left", "Status" };
                for (int i = 0; i < headers.Length; i++)
                {
                    var cell = ws.Cells[4, i + 1];
                    cell.Value = headers[i];
                    cell.Style.Font.Bold = true;
                    cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    cell.Style.Fill.BackgroundColor.SetColor(TealMid);
                    cell.Style.Font.Color.SetColor(Color.White);
                }

                for (int r = 0; r < members.Count; r++)
                {
                    var m   = members[r];
                    int row = 5 + r;

                    // Blocked = cannot train: the month is over, or no sessions left.
                    bool blocked  = !m.CanTrain;
                    bool warning  = !blocked && (m.IsExpiringSoon || m.SessionsRemaining <= 2);

                    Color rowBg = blocked    ? Color.FromArgb(255, 230, 230)
                                : warning    ? Color.FromArgb(255, 248, 220)
                                : r % 2 == 1 ? LightGray : Color.White;

                    ws.Cells[row, 1].Value = m.MemberID;
                    ws.Cells[row, 2].Value = m.Name;
                    ws.Cells[row, 3].Value = m.Phone;
                    ws.Cells[row, 4].Value = m.Age;
                    ws.Cells[row, 5].Value = m.CoachName;
                    ws.Cells[row, 6].Value = m.JoinDate;
                    ws.Cells[row, 6].Style.Numberformat.Format = "mmm dd, yyyy";
                    if (m.PlanExpiry != default(DateTime))
                    {
                        ws.Cells[row, 7].Value = m.PlanExpiry;
                        ws.Cells[row, 7].Style.Numberformat.Format = "mmm dd, yyyy";
                    }
                    ws.Cells[row, 8].Value = $"{m.SessionsRemaining} / {m.SessionsTotal}";
                    ws.Cells[row, 9].Value = m.Status;

                    Color statusFg = blocked ? Color.FromArgb(180, 20, 30)
                                   : warning ? Color.FromArgb(200, 100, 0)
                                   : Color.FromArgb(20, 120, 50);
                    ws.Cells[row, 9].Style.Font.Color.SetColor(statusFg);
                    ws.Cells[row, 9].Style.Font.Bold = true;

                    for (int c = 1; c <= 9; c++)
                    {
                        ws.Cells[row, c].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        ws.Cells[row, c].Style.Fill.BackgroundColor.SetColor(rowBg);
                    }
                }

                for (int c = 1; c <= 9; c++)
                    ws.Column(c).AutoFit();
                ws.View.FreezePanes(5, 1);
                pkg.SaveAs(new FileInfo(filePath));
            }
        }

        // =====================================================================
        // PDF
        // =====================================================================

        public static void ExportPaymentReceiptToPDF(string filePath, Payment payment,
                                                      string memberName, string planName)
        {
            var doc  = new PdfDocument();
            doc.Info.Title  = $"Receipt — {memberName}";
            doc.Info.Author = "UFA";

            var page = doc.AddPage();
            page.Width  = new XUnit(595);
            page.Height = new XUnit(842);

            var gfx = XGraphics.FromPdfPage(page);
            double pw = page.Width.Point;

            var fntTitle  = new XFont("Arial", 22, XFontStyle.Bold);
            var fntSub    = new XFont("Arial", 16, XFontStyle.Bold);
            var fntBold   = new XFont("Arial", 11, XFontStyle.Bold);
            var fntReg    = new XFont("Arial", 11, XFontStyle.Regular);
            var fntSmall  = new XFont("Arial", 10, XFontStyle.Regular);
            var fntItalic = new XFont("Arial", 10, XFontStyle.Italic);
            var fntMono   = new XFont("Arial", 14, XFontStyle.Bold);

            XBrush brushTeal  = new XSolidBrush(XColor.FromArgb(26, 26, 26));
            XBrush brushMidTeal = new XSolidBrush(XColor.FromArgb(242, 101, 34));
            XBrush brushDark  = new XSolidBrush(XColor.FromArgb(33, 33, 33));
            XBrush brushGray  = new XSolidBrush(XColor.FromArgb(100, 100, 100));
            XBrush brushLight = new XSolidBrush(XColor.FromArgb(245, 247, 250));

            // ---- Header bar ----
            gfx.DrawRectangle(brushTeal, 0, 0, pw, 80);
            gfx.DrawString("UFA", fntTitle, XBrushes.White,
                new XRect(0, 0, pw, 80), XStringFormats.Center);

            // ---- Receipt title ----
            gfx.DrawString("Payment Receipt", fntSub, brushDark,
                new XRect(0, 100, pw, 30), XStringFormats.TopCenter);

            // ---- Divider ----
            gfx.DrawLine(new XPen(XColor.FromArgb(200, 200, 200)), 50, 142, pw - 50, 142);

            // ---- Bill To (left) ----
            double y = 162;
            gfx.DrawString("BILL TO", fntSmall, brushGray, new XRect(60, y, 250, 18), XStringFormats.TopLeft);
            y += 20;
            gfx.DrawString(memberName, new XFont("Arial", 13, XFontStyle.Bold), brushDark,
                new XRect(60, y, 250, 22), XStringFormats.TopLeft);
            y += 22;
            gfx.DrawString(planName, fntReg, brushGray,
                new XRect(60, y, 250, 20), XStringFormats.TopLeft);

            // ---- Meta (right) ----
            gfx.DrawString("DATE", fntSmall, brushGray,
                new XRect(pw - 230, 162, 180, 18), XStringFormats.TopLeft);
            gfx.DrawString(payment.Date.ToString("MMM dd, yyyy"), fntReg, brushDark,
                new XRect(pw - 230, 182, 180, 20), XStringFormats.TopLeft);

            gfx.DrawString("METHOD", fntSmall, brushGray,
                new XRect(pw - 230, 212, 180, 18), XStringFormats.TopLeft);
            gfx.DrawString(payment.Method, fntReg, brushDark,
                new XRect(pw - 230, 232, 180, 20), XStringFormats.TopLeft);

            // ---- Table ----
            double tableY = 290;
            double tableW = pw - 100;

            // Header
            gfx.DrawRectangle(brushMidTeal, 50, tableY, tableW, 34);
            gfx.DrawString("Description", fntBold, XBrushes.White,
                new XRect(65, tableY, tableW - 120, 34), XStringFormats.CenterLeft);
            gfx.DrawString("Amount", fntBold, XBrushes.White,
                new XRect(50, tableY, tableW - 10, 34), XStringFormats.CenterRight);

            // Row
            double rowY = tableY + 34;
            gfx.DrawRectangle(new XSolidBrush(XColor.FromArgb(245, 247, 250)), 50, rowY, tableW, 38);
            gfx.DrawString("Membership Payment", fntReg, brushDark,
                new XRect(65, rowY, tableW - 120, 38), XStringFormats.CenterLeft);
            gfx.DrawString(payment.Amount.ToString("#,##0\" EGP\""), fntReg, brushDark,
                new XRect(50, rowY, tableW - 10, 38), XStringFormats.CenterRight);

            // Total
            double totalY = rowY + 38;
            gfx.DrawLine(new XPen(XColor.FromArgb(242, 101, 34), 1.5), 50, totalY, pw - 50, totalY);
            totalY += 10;
            gfx.DrawString("TOTAL", fntBold, brushTeal,
                new XRect(50, totalY, tableW - 100, 28), XStringFormats.CenterRight);
            gfx.DrawString(payment.Amount.ToString("#,##0\" EGP\""), fntMono, brushTeal,
                new XRect(50, totalY, tableW - 10, 28), XStringFormats.CenterRight);

            // ---- Footer ----
            double fY = page.Height.Point - 80;
            gfx.DrawLine(new XPen(XColor.FromArgb(200, 200, 200)), 50, fY, pw - 50, fY);
            gfx.DrawString("Thank you for your payment!", fntItalic, brushGray,
                new XRect(0, fY + 12, pw, 24), XStringFormats.TopCenter);
            gfx.DrawString("UFA — Ultimate Fight Academy", fntSmall, brushGray,
                new XRect(0, fY + 40, pw, 20), XStringFormats.TopCenter);

            doc.Save(filePath);
        }

        public static void ExportMonthlyReportToPDF(string filePath, DateTime month, DatabaseHelper.MonthlyReport report)
        {
            var doc = new PdfDocument();
            doc.Info.Title  = $"UFA Monthly Report - {month:MMMM yyyy}";
            doc.Info.Author = "UFA";

            var unicode  = new XPdfFontOptions(PdfFontEncoding.Unicode);
            var fntTitle = new XFont("Arial", 20, XFontStyle.Bold, unicode);
            var fntHead  = new XFont("Arial", 13, XFontStyle.Bold, unicode);
            var fntBold  = new XFont("Arial", 10, XFontStyle.Bold, unicode);
            var fntReg   = new XFont("Arial", 10, XFontStyle.Regular, unicode);
            var fntBig   = new XFont("Arial", 15, XFontStyle.Bold, unicode);
            var fntSmall = new XFont("Arial", 8.5, XFontStyle.Regular, unicode);

            XBrush black  = new XSolidBrush(XColor.FromArgb(26, 26, 26));
            XBrush orange = new XSolidBrush(XColor.FromArgb(242, 101, 34));
            XBrush dark   = new XSolidBrush(XColor.FromArgb(33, 33, 33));
            XBrush gray   = new XSolidBrush(XColor.FromArgb(100, 100, 100));
            XBrush light  = new XSolidBrush(XColor.FromArgb(245, 247, 250));

            const double Margin = 40, RowH = 24, PageW = 595, PageH = 842;
            double width = PageW - 2 * Margin;
            XGraphics gfx = null;
            double y = 0;

            void NewPage()
            {
                gfx?.Dispose();
                var page = doc.AddPage();
                page.Width  = new XUnit(PageW);
                page.Height = new XUnit(PageH);
                gfx = XGraphics.FromPdfPage(page);
                gfx.DrawString($"UFA - Ultimate Fight Academy  |  {month:MMMM yyyy}", fntSmall, gray,
                    new XRect(0, PageH - 30, PageW, 16), XStringFormats.TopCenter);
                y = Margin;
            }

            // Columns: first is left-aligned text, the rest right-aligned numbers.
            void Table(string title, DataTable table, string[] columns, string[] headers, double[] widths, int moneyFrom, string empty)
            {
                if (y + 34 + RowH * 2 > PageH - 50) NewPage();
                gfx.DrawString(title, fntHead, dark, new XRect(Margin, y, width, 22), XStringFormats.TopLeft);
                y += 24;
                if (table.Rows.Count == 0)
                {
                    gfx.DrawString(empty, fntReg, gray, new XRect(Margin, y, width, 18), XStringFormats.TopLeft);
                    y += 34;
                    return;
                }
                void Header()
                {
                    gfx.DrawRectangle(orange, Margin, y, width, RowH);
                    double hx = Margin;
                    for (int c = 0; c < headers.Length; c++)
                    {
                        gfx.DrawString(headers[c], fntBold, XBrushes.White, new XRect(hx + 8, y, widths[c] - 16, RowH),
                            c == 0 ? XStringFormats.CenterLeft : XStringFormats.CenterRight);
                        hx += widths[c];
                    }
                    y += RowH;
                }
                Header();
                for (int r = 0; r < table.Rows.Count; r++)
                {
                    if (y + RowH > PageH - 50) { NewPage(); Header(); }
                    if (r % 2 == 1) gfx.DrawRectangle(light, Margin, y, width, RowH);
                    double cx = Margin;
                    for (int c = 0; c < columns.Length; c++)
                    {
                        object v = table.Rows[r][columns[c]];
                        string text = c == 0 ? v.ToString()
                                    : c >= moneyFrom ? $"{Convert.ToDecimal(v):N0} EGP"
                                    : Convert.ToDecimal(v).ToString("N0");
                        gfx.DrawString(text, fntReg, dark, new XRect(cx + 8, y, widths[c] - 16, RowH),
                            c == 0 ? XStringFormats.CenterLeft : XStringFormats.CenterRight);
                        cx += widths[c];
                    }
                    y += RowH;
                }
                y += 22;
            }

            NewPage();

            // ---- Header ----
            gfx.DrawRectangle(black, 0, 0, PageW, 86);
            double textX = Margin;
            string logoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "logo.png");
            if (File.Exists(logoPath))
                using (var logo = XImage.FromFile(logoPath))
                {
                    double h = 62, w = h * logo.PixelWidth / logo.PixelHeight;
                    gfx.DrawImage(logo, Margin, 12, w, h);
                    textX += w + 16;
                }
            gfx.DrawString("Monthly Report", fntTitle, XBrushes.White, new XRect(textX, 18, 400, 28), XStringFormats.TopLeft);
            gfx.DrawString(month.ToString("MMMM yyyy"), fntHead, orange, new XRect(textX, 50, 400, 22), XStringFormats.TopLeft);
            y = 104;

            // ---- Headline numbers ----
            decimal total = report.Memberships + report.Products;
            var cards = new[]
            {
                ("Total Revenue", $"{total:N0} EGP"),
                ("Memberships", $"{report.Memberships:N0} EGP"),
                ("Products", $"{report.Products:N0} EGP"),
                ("New Members", report.NewMembers.ToString()),
                ("Sessions Attended", report.Sessions.ToString()),
                ("Free First Sessions", report.FreeSessions.ToString())
            };
            double cardW = (width - 20) / 3, cardH = 62;
            for (int i = 0; i < cards.Length; i++)
            {
                double cx = Margin + (i % 3) * (cardW + 10), cy = y + (i / 3) * (cardH + 10);
                gfx.DrawRectangle(light, cx, cy, cardW, cardH);
                gfx.DrawRectangle(orange, cx, cy, 4, cardH);
                gfx.DrawString(cards[i].Item1, fntSmall, gray, new XRect(cx + 14, cy + 10, cardW - 20, 14), XStringFormats.TopLeft);
                gfx.DrawString(cards[i].Item2, fntBig, dark, new XRect(cx + 14, cy + 28, cardW - 20, 24), XStringFormats.TopLeft);
            }
            y += 2 * (cardH + 10) + 14;

            // ---- Top coach ----
            if (report.Coaches.Rows.Count > 0 && Convert.ToDecimal(report.Coaches.Rows[0]["Revenue"]) > 0)
            {
                gfx.DrawString($"Top coach: {report.Coaches.Rows[0]["Coach"]}  -  {Convert.ToDecimal(report.Coaches.Rows[0]["Revenue"]):N0} EGP",
                    fntHead, orange, new XRect(Margin, y, width, 22), XStringFormats.TopLeft);
                y += 32;
            }

            Table("Coaches", report.Coaches,
                new[] { "Coach", "Members", "Payments", "Sessions", "Revenue" },
                new[] { "Coach", "Paying Members", "Payments", "Sessions", "Revenue" },
                new[] { width - 340, 100, 70, 70, 100 }, 4, "No coach activity this month.");
            Table("Best-Selling Products", report.TopProducts,
                new[] { "Product", "Sold", "Revenue" },
                new[] { "Product", "Sold", "Revenue" },
                new[] { width - 190, 80, 110 }, 2, "No products sold this month.");
            Table("Money by Payment Method", report.Methods,
                new[] { "Method", "Revenue" },
                new[] { "Method", "Revenue" },
                new[] { width - 110, 110 }, 1, "No money received this month.");

            gfx.Dispose();
            doc.Save(filePath);
        }

        // =====================================================================
        // UTIL
        // =====================================================================

        public static void OpenFile(string path)
        {
            try { Process.Start(path); } catch { }
        }
    }
}
