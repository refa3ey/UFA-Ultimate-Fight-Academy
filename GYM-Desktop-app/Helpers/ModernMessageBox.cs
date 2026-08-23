using System.Windows.Forms;

namespace GYM_Desktop_app.Helpers
{
    public static class ModernMessageBox
    {
        public static void Info(string message, string title = "Information")
            => MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);

        public static void Error(string message, string title = "Error")
            => MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);

        public static void Warning(string message, string title = "Warning")
            => MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);

        public static bool Confirm(string message, string title = "Confirm")
            => MessageBox.Show(message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
    }
}
