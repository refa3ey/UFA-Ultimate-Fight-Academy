using System.Drawing;

namespace GYM_Desktop_app.Helpers
{
    public static class Theme
    {
        public static readonly Color Primary        = Color.FromArgb(242, 101, 34);
        public static readonly Color PrimaryDark    = Color.FromArgb(26, 26, 26);
        public static readonly Color Accent         = Color.White;
        public static readonly Color TextDark       = Color.FromArgb(33, 33, 33);
        public static readonly Color TextGray       = Color.FromArgb(120, 120, 120);
        public static readonly Color Background     = Color.White;
        public static readonly Color BackgroundLight= Color.FromArgb(245, 247, 250);
        public static readonly Color BorderLight    = Color.FromArgb(200, 200, 200);
        public static readonly Color Danger         = Color.FromArgb(220, 53, 69);
        public static readonly Color DangerDark     = Color.FromArgb(180, 30, 45);
        public static readonly Color Success        = Color.FromArgb(40, 167, 69);
        public static readonly Color SuccessDark    = Color.FromArgb(30, 130, 55);
        public static readonly Color Warning        = Color.FromArgb(255, 152, 0);
        public static readonly Color SidebarBg     = Color.FromArgb(18, 18, 18);

        public static Font Regular(float size = 10f)
            => new Font("Segoe UI", size, FontStyle.Regular);

        public static Font Bold(float size = 10f)
            => new Font("Segoe UI", size, FontStyle.Bold);
    }
}
