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

        public static void ExportPaymentsToExcel(string filePath, DataTable dt)
        {
            using (var pkg = new ExcelPackage())
            {
                var ws = pkg.Workbook.Worksheets.Add("Payments");
                int cols = dt.Columns.Count;

                // Title
                ws.Cells[1, 1].Value = "UFA — Payment Report";
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
                        cell.Value = dt.Rows[r][c];
                        if (r % 2 == 1)
                        {
                            cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            cell.Style.Fill.BackgroundColor.SetColor(LightGray);
                        }
                        string colName = dt.Columns[c].ColumnName;
                        if (colName == "Amount")
                            cell.Style.Numberformat.Format = "#,##0\" EGP\"";
                        else if (colName.Contains("Date"))
                            cell.Style.Numberformat.Format = "mmm dd, yyyy";
                    }
                }

                // Total row
                if (dt.Columns.Contains("Amount"))
                {
                    int amtCol  = dt.Columns["Amount"].Ordinal + 1;
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
                ws.Cells[1, 1, 1, 8].Merge = true;

                ws.Cells[2, 1].Value = $"Exported: {DateTime.Now:MMM dd, yyyy}   |   {members.Count} members";
                ws.Cells[2, 1].Style.Font.Color.SetColor(Color.Gray);
                ws.Cells[2, 1, 2, 8].Merge = true;

                string[] headers = { "ID", "Name", "Phone", "Age", "Coach", "Joined", "Sessions Left", "Status" };
                for (int i = 0; i < headers.Length; i++)
                {
                    var cell = ws.Cells[4, i + 1];
                    cell.Value = headers[i];
                    cell.Style.Font.Bold = true;
                    cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    cell.Style.Fill.BackgroundColor.SetColor(TealMid);
                    cell.Style.Font.Color.SetColor(Color.White);
                }

                var now = DateTime.Now;
                for (int r = 0; r < members.Count; r++)
                {
                    var m   = members[r];
                    int row = 5 + r;

                    bool active   = m.SessionsRemaining > 0;
                    bool expiring = active && m.SessionsRemaining <= 2;
                    string status = active ? (expiring ? "Low Sessions" : "Active") : "Finished";

                    Color rowBg = expiring  ? Color.FromArgb(255, 248, 220)
                                 : !active  ? Color.FromArgb(255, 230, 230)
                                 : r % 2 == 1 ? LightGray : Color.White;

                    ws.Cells[row, 1].Value = m.MemberID;
                    ws.Cells[row, 2].Value = m.Name;
                    ws.Cells[row, 3].Value = m.Phone;
                    ws.Cells[row, 4].Value = m.Age;
                    ws.Cells[row, 5].Value = m.CoachName;
                    ws.Cells[row, 6].Value = m.JoinDate;
                    ws.Cells[row, 6].Style.Numberformat.Format = "mmm dd, yyyy";
                    ws.Cells[row, 7].Value = $"{m.SessionsRemaining} / {m.SessionsTotal}";
                    ws.Cells[row, 8].Value = status;

                    Color statusFg = expiring  ? Color.FromArgb(200, 100, 0)
                                   : !active   ? Color.FromArgb(180, 20, 30)
                                   : Color.FromArgb(20, 120, 50);
                    ws.Cells[row, 8].Style.Font.Color.SetColor(statusFg);
                    ws.Cells[row, 8].Style.Font.Bold = true;

                    for (int c = 1; c <= 8; c++)
                    {
                        ws.Cells[row, c].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        ws.Cells[row, c].Style.Fill.BackgroundColor.SetColor(rowBg);
                    }
                }

                for (int c = 1; c <= 8; c++)
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

        // =====================================================================
        // UTIL
        // =====================================================================

        public static void OpenFile(string path)
        {
            try { Process.Start(path); } catch { }
        }
    }
}
