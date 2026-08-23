using System.Windows.Forms;

namespace GYM_Desktop_app.Helpers
{
    public static class ExportDialog
    {
        public static string PromptForExcelPath(string defaultFileName)
        {
            using (var dlg = new SaveFileDialog())
            {
                dlg.Title    = "Save Excel Export";
                dlg.Filter   = "Excel Workbook|*.xlsx";
                dlg.FileName = defaultFileName;
                return dlg.ShowDialog() == DialogResult.OK ? dlg.FileName : null;
            }
        }

        public static string PromptForPDFPath(string defaultFileName)
        {
            using (var dlg = new SaveFileDialog())
            {
                dlg.Title    = "Save PDF";
                dlg.Filter   = "PDF Document|*.pdf";
                dlg.FileName = defaultFileName;
                return dlg.ShowDialog() == DialogResult.OK ? dlg.FileName : null;
            }
        }
    }
}
