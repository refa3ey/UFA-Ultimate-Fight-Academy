namespace GYM_Desktop_app.Forms
{
    partial class AdminDashboard
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components           = new System.ComponentModel.Container();
            this.panelSidebar         = new Guna.UI2.WinForms.Guna2Panel();
            this.lblBrand             = new System.Windows.Forms.Label();
            this.lblBrandSub          = new System.Windows.Forms.Label();
            this.panelDivider         = new System.Windows.Forms.Panel();
            this.btnManageMembers     = new Guna.UI2.WinForms.Guna2Button();
            this.btnManagePlans       = new Guna.UI2.WinForms.Guna2Button();
            this.btnManageTrainers    = new Guna.UI2.WinForms.Guna2Button();
            this.btnPayments          = new Guna.UI2.WinForms.Guna2Button();
            this.btnReports           = new Guna.UI2.WinForms.Guna2Button();
            this.btnChangePassword    = new Guna.UI2.WinForms.Guna2Button();
            this.btnLogout            = new Guna.UI2.WinForms.Guna2Button();
            this.panelTopBar          = new Guna.UI2.WinForms.Guna2Panel();
            this.lblPageTitle         = new System.Windows.Forms.Label();
            this.lblWelcome           = new System.Windows.Forms.Label();
            this.btnRefresh           = new Guna.UI2.WinForms.Guna2Button();
            this.btnClose             = new Guna.UI2.WinForms.Guna2Button();
            this.cardMembers          = new Guna.UI2.WinForms.Guna2Panel();
            this.lblCardMembers       = new System.Windows.Forms.Label();
            this.lblCardMembersVal    = new System.Windows.Forms.Label();
            this.cardActive           = new Guna.UI2.WinForms.Guna2Panel();
            this.lblCardActive        = new System.Windows.Forms.Label();
            this.lblCardActiveVal     = new System.Windows.Forms.Label();
            this.cardRevenue          = new Guna.UI2.WinForms.Guna2Panel();
            this.lblCardRevenue       = new System.Windows.Forms.Label();
            this.lblCardRevenueVal    = new System.Windows.Forms.Label();
            this.cardNew              = new Guna.UI2.WinForms.Guna2Panel();
            this.lblCardNew           = new System.Windows.Forms.Label();
            this.lblCardNewVal        = new System.Windows.Forms.Label();
            this.panelChartRevCard    = new Guna.UI2.WinForms.Guna2Panel();
            this.panelChartGrowthCard = new Guna.UI2.WinForms.Guna2Panel();
            this.panelChartPlansCard  = new Guna.UI2.WinForms.Guna2Panel();
            this.panelChartHoursCard  = new Guna.UI2.WinForms.Guna2Panel();
            this.lblRecentTitle       = new System.Windows.Forms.Label();
            this.dgvRecentMembers     = new System.Windows.Forms.DataGridView();
            this.timerRefresh         = new System.Windows.Forms.Timer(this.components);
            this.panelSidebar.SuspendLayout();
            this.panelTopBar.SuspendLayout();
            this.cardMembers.SuspendLayout();
            this.cardActive.SuspendLayout();
            this.cardRevenue.SuspendLayout();
            this.cardNew.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRecentMembers)).BeginInit();
            this.SuspendLayout();

            // ── panelSidebar ──────────────────────────────────────────────────
            this.panelSidebar.Controls.Add(this.lblBrand);
            this.panelSidebar.Controls.Add(this.lblBrandSub);
            this.panelSidebar.Controls.Add(this.panelDivider);
            this.panelSidebar.Controls.Add(this.btnManageMembers);
            this.panelSidebar.Controls.Add(this.btnManagePlans);
            this.panelSidebar.Controls.Add(this.btnManageTrainers);
            this.panelSidebar.Controls.Add(this.btnPayments);
            this.panelSidebar.Controls.Add(this.btnReports);
            this.panelSidebar.Controls.Add(this.btnChangePassword);
            this.panelSidebar.Controls.Add(this.btnLogout);
            this.panelSidebar.FillColor  = System.Drawing.Color.FromArgb(18, 18, 18);
            this.panelSidebar.Location   = new System.Drawing.Point(0, 0);
            this.panelSidebar.Name       = "panelSidebar";
            this.panelSidebar.Size       = new System.Drawing.Size(240, 900);
            this.panelSidebar.TabIndex   = 0;
            this.panelSidebar.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DragPanel_MouseDown);
            this.panelSidebar.MouseMove += new System.Windows.Forms.MouseEventHandler(this.DragPanel_MouseMove);
            this.panelSidebar.MouseUp   += new System.Windows.Forms.MouseEventHandler(this.DragPanel_MouseUp);

            // lblBrand
            this.lblBrand.BackColor  = System.Drawing.Color.Transparent;
            this.lblBrand.Font       = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblBrand.ForeColor  = System.Drawing.Color.White;
            this.lblBrand.Location   = new System.Drawing.Point(0, 28);
            this.lblBrand.Name       = "lblBrand";
            this.lblBrand.Size       = new System.Drawing.Size(240, 40);
            this.lblBrand.TabIndex   = 0;
            this.lblBrand.Text       = "UFA";
            this.lblBrand.TextAlign  = System.Drawing.ContentAlignment.MiddleCenter;

            // lblBrandSub
            this.lblBrandSub.BackColor  = System.Drawing.Color.Transparent;
            this.lblBrandSub.Font       = new System.Drawing.Font("Segoe UI", 8F);
            this.lblBrandSub.ForeColor  = System.Drawing.Color.FromArgb(150, 200, 200);
            this.lblBrandSub.Location   = new System.Drawing.Point(0, 66);
            this.lblBrandSub.Name       = "lblBrandSub";
            this.lblBrandSub.Size       = new System.Drawing.Size(240, 18);
            this.lblBrandSub.TabIndex   = 1;
            this.lblBrandSub.Text       = "Ultimate Fight Academy";
            this.lblBrandSub.TextAlign  = System.Drawing.ContentAlignment.MiddleCenter;

            // panelDivider
            this.panelDivider.BackColor = System.Drawing.Color.FromArgb(80, 130, 135);
            this.panelDivider.Location  = new System.Drawing.Point(20, 93);
            this.panelDivider.Name      = "panelDivider";
            this.panelDivider.Size      = new System.Drawing.Size(200, 1);
            this.panelDivider.TabIndex  = 2;

            // btnManageMembers
            this.btnManageMembers.BorderRadius             = 8;
            this.btnManageMembers.FillColor                = System.Drawing.Color.FromArgb(18, 18, 18);
            this.btnManageMembers.Font                     = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnManageMembers.ForeColor                = System.Drawing.Color.White;
            this.btnManageMembers.HoverState.FillColor     = System.Drawing.Color.FromArgb(242, 101, 34);
            this.btnManageMembers.HoverState.ForeColor     = System.Drawing.Color.White;
            this.btnManageMembers.Location                 = new System.Drawing.Point(10, 105);
            this.btnManageMembers.Name                     = "btnManageMembers";
            this.btnManageMembers.Size                     = new System.Drawing.Size(220, 50);
            this.btnManageMembers.TabIndex                 = 3;
            this.btnManageMembers.Text                     = "  Members";
            this.btnManageMembers.TextAlign                = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnManageMembers.Click                   += new System.EventHandler(this.btnManageMembers_Click);

            // btnManagePlans
            this.btnManagePlans.BorderRadius             = 8;
            this.btnManagePlans.FillColor                = System.Drawing.Color.FromArgb(18, 18, 18);
            this.btnManagePlans.Font                     = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnManagePlans.ForeColor                = System.Drawing.Color.White;
            this.btnManagePlans.HoverState.FillColor     = System.Drawing.Color.FromArgb(242, 101, 34);
            this.btnManagePlans.HoverState.ForeColor     = System.Drawing.Color.White;
            this.btnManagePlans.Location                 = new System.Drawing.Point(10, 165);
            this.btnManagePlans.Name                     = "btnManagePlans";
            this.btnManagePlans.Size                     = new System.Drawing.Size(220, 50);
            this.btnManagePlans.TabIndex                 = 6;
            this.btnManagePlans.Text                     = "  Plans";
            this.btnManagePlans.TextAlign                = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnManagePlans.Click                   += new System.EventHandler(this.btnManagePlans_Click);

            // btnManageTrainers
            this.btnManageTrainers.BorderRadius             = 8;
            this.btnManageTrainers.FillColor                = System.Drawing.Color.FromArgb(18, 18, 18);
            this.btnManageTrainers.Font                     = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnManageTrainers.ForeColor                = System.Drawing.Color.White;
            this.btnManageTrainers.HoverState.FillColor     = System.Drawing.Color.FromArgb(242, 101, 34);
            this.btnManageTrainers.HoverState.ForeColor     = System.Drawing.Color.White;
            this.btnManageTrainers.Location                 = new System.Drawing.Point(10, 225);
            this.btnManageTrainers.Name                     = "btnManageTrainers";
            this.btnManageTrainers.Size                     = new System.Drawing.Size(220, 50);
            this.btnManageTrainers.TabIndex                 = 7;
            this.btnManageTrainers.Text                     = "  Coaches";
            this.btnManageTrainers.TextAlign                = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnManageTrainers.Click                   += new System.EventHandler(this.btnManageTrainers_Click);

            // btnPayments
            this.btnPayments.BorderRadius             = 8;
            this.btnPayments.FillColor                = System.Drawing.Color.FromArgb(18, 18, 18);
            this.btnPayments.Font                     = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnPayments.ForeColor                = System.Drawing.Color.White;
            this.btnPayments.HoverState.FillColor     = System.Drawing.Color.FromArgb(242, 101, 34);
            this.btnPayments.HoverState.ForeColor     = System.Drawing.Color.White;
            this.btnPayments.Location                 = new System.Drawing.Point(10, 285);
            this.btnPayments.Name                     = "btnPayments";
            this.btnPayments.Size                     = new System.Drawing.Size(220, 50);
            this.btnPayments.TabIndex                 = 8;
            this.btnPayments.Text                     = "  Payments";
            this.btnPayments.TextAlign                = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnPayments.Click                   += new System.EventHandler(this.btnPayments_Click);

            // btnReports
            this.btnReports.BorderRadius             = 8;
            this.btnReports.FillColor                = System.Drawing.Color.FromArgb(18, 18, 18);
            this.btnReports.Font                     = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnReports.ForeColor                = System.Drawing.Color.White;
            this.btnReports.HoverState.FillColor     = System.Drawing.Color.FromArgb(242, 101, 34);
            this.btnReports.HoverState.ForeColor     = System.Drawing.Color.White;
            this.btnReports.Location                 = new System.Drawing.Point(10, 345);
            this.btnReports.Name                     = "btnReports";
            this.btnReports.Size                     = new System.Drawing.Size(220, 50);
            this.btnReports.TabIndex                 = 9;
            this.btnReports.Text                     = "  Reports";
            this.btnReports.TextAlign                = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnReports.Click                   += new System.EventHandler(this.btnReports_Click);

            // btnChangePassword
            this.btnChangePassword.BorderRadius             = 8;
            this.btnChangePassword.FillColor                = System.Drawing.Color.FromArgb(18, 18, 18);
            this.btnChangePassword.Font                     = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnChangePassword.ForeColor                = System.Drawing.Color.White;
            this.btnChangePassword.HoverState.FillColor     = System.Drawing.Color.FromArgb(242, 101, 34);
            this.btnChangePassword.HoverState.ForeColor     = System.Drawing.Color.White;
            this.btnChangePassword.Location                 = new System.Drawing.Point(10, 405);
            this.btnChangePassword.Name                     = "btnChangePassword";
            this.btnChangePassword.Size                     = new System.Drawing.Size(220, 50);
            this.btnChangePassword.TabIndex                 = 10;
            this.btnChangePassword.Text                     = "  Change Password";
            this.btnChangePassword.TextAlign                = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnChangePassword.Click                   += new System.EventHandler(this.btnChangePassword_Click);

            // btnLogout
            this.btnLogout.BorderRadius             = 8;
            this.btnLogout.FillColor                = System.Drawing.Color.FromArgb(220, 53, 69);
            this.btnLogout.Font                     = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnLogout.ForeColor                = System.Drawing.Color.White;
            this.btnLogout.Location                 = new System.Drawing.Point(20, 820);
            this.btnLogout.Name                     = "btnLogout";
            this.btnLogout.Size                     = new System.Drawing.Size(200, 45);
            this.btnLogout.TabIndex                 = 11;
            this.btnLogout.Text                     = "  Logout";
            this.btnLogout.TextAlign                = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnLogout.Click                   += new System.EventHandler(this.btnLogout_Click);

            // ── panelTopBar ───────────────────────────────────────────────────
            this.panelTopBar.Controls.Add(this.lblPageTitle);
            this.panelTopBar.Controls.Add(this.lblWelcome);
            this.panelTopBar.Controls.Add(this.btnRefresh);
            this.panelTopBar.Controls.Add(this.btnClose);
            this.panelTopBar.FillColor = System.Drawing.Color.White;
            this.panelTopBar.Location  = new System.Drawing.Point(240, 0);
            this.panelTopBar.Name      = "panelTopBar";
            this.panelTopBar.Size      = new System.Drawing.Size(960, 65);
            this.panelTopBar.TabIndex  = 1;

            // lblPageTitle
            this.lblPageTitle.Font      = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblPageTitle.ForeColor = System.Drawing.Color.FromArgb(33, 33, 33);
            this.lblPageTitle.Location  = new System.Drawing.Point(25, 15);
            this.lblPageTitle.Name      = "lblPageTitle";
            this.lblPageTitle.Size      = new System.Drawing.Size(300, 35);
            this.lblPageTitle.TabIndex  = 0;
            this.lblPageTitle.Text      = "Analytics Dashboard";

            // lblWelcome
            this.lblWelcome.Font      = new System.Drawing.Font("Segoe UI", 10F);
            this.lblWelcome.ForeColor = System.Drawing.Color.FromArgb(120, 120, 120);
            this.lblWelcome.Location  = new System.Drawing.Point(480, 20);
            this.lblWelcome.Name      = "lblWelcome";
            this.lblWelcome.Size      = new System.Drawing.Size(290, 25);
            this.lblWelcome.TabIndex  = 1;
            this.lblWelcome.Text      = "Welcome, Admin!";
            this.lblWelcome.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            // btnRefresh
            this.btnRefresh.BorderRadius             = 20;
            this.btnRefresh.FillColor                = System.Drawing.Color.FromArgb(242, 101, 34);
            this.btnRefresh.Font                     = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnRefresh.ForeColor                = System.Drawing.Color.White;
            this.btnRefresh.HoverState.FillColor     = System.Drawing.Color.FromArgb(26, 26, 26);
            this.btnRefresh.HoverState.ForeColor     = System.Drawing.Color.White;
            this.btnRefresh.Location                 = new System.Drawing.Point(786, 17);
            this.btnRefresh.Name                     = "btnRefresh";
            this.btnRefresh.Size                     = new System.Drawing.Size(110, 30);
            this.btnRefresh.TabIndex                 = 2;
            this.btnRefresh.Text                     = "↻  Refresh";
            this.btnRefresh.Click                   += new System.EventHandler(this.btnRefresh_Click);

            // btnClose
            this.btnClose.BorderRadius             = 20;
            this.btnClose.FillColor                = System.Drawing.Color.FromArgb(220, 53, 69);
            this.btnClose.Font                     = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor                = System.Drawing.Color.White;
            this.btnClose.Location                 = new System.Drawing.Point(920, 18);
            this.btnClose.Name                     = "btnClose";
            this.btnClose.Size                     = new System.Drawing.Size(28, 28);
            this.btnClose.TabIndex                 = 3;
            this.btnClose.Text                     = "✕";
            this.btnClose.Click                   += new System.EventHandler(this.btnClose_Click);

            // ── Stat cards (y=75, h=105) ──────────────────────────────────────
            // cardMembers
            this.cardMembers.BorderColor     = System.Drawing.Color.FromArgb(230, 230, 230);
            this.cardMembers.BorderRadius    = 12;
            this.cardMembers.BorderThickness = 1;
            this.cardMembers.Controls.Add(this.lblCardMembers);
            this.cardMembers.Controls.Add(this.lblCardMembersVal);
            this.cardMembers.FillColor       = System.Drawing.Color.White;
            this.cardMembers.Location        = new System.Drawing.Point(258, 75);
            this.cardMembers.Name            = "cardMembers";
            this.cardMembers.Size            = new System.Drawing.Size(212, 105);
            this.cardMembers.TabIndex        = 2;

            this.lblCardMembers.Font      = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.lblCardMembers.ForeColor = System.Drawing.Color.FromArgb(120, 120, 120);
            this.lblCardMembers.Location  = new System.Drawing.Point(15, 15);
            this.lblCardMembers.Name      = "lblCardMembers";
            this.lblCardMembers.Size      = new System.Drawing.Size(185, 18);
            this.lblCardMembers.TabIndex  = 0;
            this.lblCardMembers.Text      = "TOTAL MEMBERS";

            this.lblCardMembersVal.Font      = new System.Drawing.Font("Segoe UI", 26F, System.Drawing.FontStyle.Bold);
            this.lblCardMembersVal.ForeColor = System.Drawing.Color.FromArgb(242, 101, 34);
            this.lblCardMembersVal.Location  = new System.Drawing.Point(12, 35);
            this.lblCardMembersVal.Name      = "lblCardMembersVal";
            this.lblCardMembersVal.Size      = new System.Drawing.Size(185, 55);
            this.lblCardMembersVal.TabIndex  = 1;
            this.lblCardMembersVal.Text      = "0";

            // cardActive
            this.cardActive.BorderColor     = System.Drawing.Color.FromArgb(230, 230, 230);
            this.cardActive.BorderRadius    = 12;
            this.cardActive.BorderThickness = 1;
            this.cardActive.Controls.Add(this.lblCardActive);
            this.cardActive.Controls.Add(this.lblCardActiveVal);
            this.cardActive.FillColor       = System.Drawing.Color.White;
            this.cardActive.Location        = new System.Drawing.Point(480, 75);
            this.cardActive.Name            = "cardActive";
            this.cardActive.Size            = new System.Drawing.Size(212, 105);
            this.cardActive.TabIndex        = 3;

            this.lblCardActive.Font      = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.lblCardActive.ForeColor = System.Drawing.Color.FromArgb(120, 120, 120);
            this.lblCardActive.Location  = new System.Drawing.Point(15, 15);
            this.lblCardActive.Name      = "lblCardActive";
            this.lblCardActive.Size      = new System.Drawing.Size(185, 18);
            this.lblCardActive.TabIndex  = 0;
            this.lblCardActive.Text      = "ACTIVE MEMBERS";

            this.lblCardActiveVal.Font      = new System.Drawing.Font("Segoe UI", 26F, System.Drawing.FontStyle.Bold);
            this.lblCardActiveVal.ForeColor = System.Drawing.Color.FromArgb(40, 167, 69);
            this.lblCardActiveVal.Location  = new System.Drawing.Point(12, 35);
            this.lblCardActiveVal.Name      = "lblCardActiveVal";
            this.lblCardActiveVal.Size      = new System.Drawing.Size(185, 55);
            this.lblCardActiveVal.TabIndex  = 1;
            this.lblCardActiveVal.Text      = "0";

            // cardRevenue
            this.cardRevenue.BorderColor     = System.Drawing.Color.FromArgb(230, 230, 230);
            this.cardRevenue.BorderRadius    = 12;
            this.cardRevenue.BorderThickness = 1;
            this.cardRevenue.Controls.Add(this.lblCardRevenue);
            this.cardRevenue.Controls.Add(this.lblCardRevenueVal);
            this.cardRevenue.FillColor       = System.Drawing.Color.White;
            this.cardRevenue.Location        = new System.Drawing.Point(702, 75);
            this.cardRevenue.Name            = "cardRevenue";
            this.cardRevenue.Size            = new System.Drawing.Size(212, 105);
            this.cardRevenue.TabIndex        = 4;

            this.lblCardRevenue.Font      = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.lblCardRevenue.ForeColor = System.Drawing.Color.FromArgb(120, 120, 120);
            this.lblCardRevenue.Location  = new System.Drawing.Point(15, 15);
            this.lblCardRevenue.Name      = "lblCardRevenue";
            this.lblCardRevenue.Size      = new System.Drawing.Size(185, 18);
            this.lblCardRevenue.TabIndex  = 0;
            this.lblCardRevenue.Text      = "REVENUE (MONTH)";

            this.lblCardRevenueVal.Font      = new System.Drawing.Font("Segoe UI", 26F, System.Drawing.FontStyle.Bold);
            this.lblCardRevenueVal.ForeColor = System.Drawing.Color.FromArgb(255, 152, 0);
            this.lblCardRevenueVal.Location  = new System.Drawing.Point(12, 35);
            this.lblCardRevenueVal.Name      = "lblCardRevenueVal";
            this.lblCardRevenueVal.Size      = new System.Drawing.Size(185, 55);
            this.lblCardRevenueVal.TabIndex  = 1;
            this.lblCardRevenueVal.Text      = "$0";

            // cardNew  (now shows check-ins this week)
            this.cardNew.BorderColor     = System.Drawing.Color.FromArgb(230, 230, 230);
            this.cardNew.BorderRadius    = 12;
            this.cardNew.BorderThickness = 1;
            this.cardNew.Controls.Add(this.lblCardNew);
            this.cardNew.Controls.Add(this.lblCardNewVal);
            this.cardNew.FillColor       = System.Drawing.Color.White;
            this.cardNew.Location        = new System.Drawing.Point(924, 75);
            this.cardNew.Name            = "cardNew";
            this.cardNew.Size            = new System.Drawing.Size(212, 105);
            this.cardNew.TabIndex        = 5;

            this.lblCardNew.Font      = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.lblCardNew.ForeColor = System.Drawing.Color.FromArgb(120, 120, 120);
            this.lblCardNew.Location  = new System.Drawing.Point(15, 15);
            this.lblCardNew.Name      = "lblCardNew";
            this.lblCardNew.Size      = new System.Drawing.Size(185, 18);
            this.lblCardNew.TabIndex  = 0;
            this.lblCardNew.Text      = "CHECK-INS (WEEK)";

            this.lblCardNewVal.Font      = new System.Drawing.Font("Segoe UI", 26F, System.Drawing.FontStyle.Bold);
            this.lblCardNewVal.ForeColor = System.Drawing.Color.FromArgb(52, 152, 219);
            this.lblCardNewVal.Location  = new System.Drawing.Point(12, 35);
            this.lblCardNewVal.Name      = "lblCardNewVal";
            this.lblCardNewVal.Size      = new System.Drawing.Size(185, 55);
            this.lblCardNewVal.TabIndex  = 1;
            this.lblCardNewVal.Text      = "0";

            // ── Chart card panels (row 1, y=195) ─────────────────────────────
            this.panelChartRevCard.BorderColor     = System.Drawing.Color.FromArgb(225, 225, 225);
            this.panelChartRevCard.BorderRadius    = 12;
            this.panelChartRevCard.BorderThickness = 1;
            this.panelChartRevCard.FillColor       = System.Drawing.Color.White;
            this.panelChartRevCard.Location        = new System.Drawing.Point(258, 195);
            this.panelChartRevCard.Name            = "panelChartRevCard";
            this.panelChartRevCard.Size            = new System.Drawing.Size(457, 235);
            this.panelChartRevCard.TabIndex        = 6;

            this.panelChartGrowthCard.BorderColor     = System.Drawing.Color.FromArgb(225, 225, 225);
            this.panelChartGrowthCard.BorderRadius    = 12;
            this.panelChartGrowthCard.BorderThickness = 1;
            this.panelChartGrowthCard.FillColor       = System.Drawing.Color.White;
            this.panelChartGrowthCard.Location        = new System.Drawing.Point(730, 195);
            this.panelChartGrowthCard.Name            = "panelChartGrowthCard";
            this.panelChartGrowthCard.Size            = new System.Drawing.Size(457, 235);
            this.panelChartGrowthCard.TabIndex        = 7;

            // ── Chart card panels (row 2, y=445) ─────────────────────────────
            this.panelChartPlansCard.BorderColor     = System.Drawing.Color.FromArgb(225, 225, 225);
            this.panelChartPlansCard.BorderRadius    = 12;
            this.panelChartPlansCard.BorderThickness = 1;
            this.panelChartPlansCard.FillColor       = System.Drawing.Color.White;
            this.panelChartPlansCard.Location        = new System.Drawing.Point(258, 445);
            this.panelChartPlansCard.Name            = "panelChartPlansCard";
            this.panelChartPlansCard.Size            = new System.Drawing.Size(457, 235);
            this.panelChartPlansCard.TabIndex        = 8;

            this.panelChartHoursCard.BorderColor     = System.Drawing.Color.FromArgb(225, 225, 225);
            this.panelChartHoursCard.BorderRadius    = 12;
            this.panelChartHoursCard.BorderThickness = 1;
            this.panelChartHoursCard.FillColor       = System.Drawing.Color.White;
            this.panelChartHoursCard.Location        = new System.Drawing.Point(730, 445);
            this.panelChartHoursCard.Name            = "panelChartHoursCard";
            this.panelChartHoursCard.Size            = new System.Drawing.Size(457, 235);
            this.panelChartHoursCard.TabIndex        = 9;

            // ── Recent Members ────────────────────────────────────────────────
            this.lblRecentTitle.Font      = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblRecentTitle.ForeColor = System.Drawing.Color.FromArgb(33, 33, 33);
            this.lblRecentTitle.Location  = new System.Drawing.Point(258, 695);
            this.lblRecentTitle.Name      = "lblRecentTitle";
            this.lblRecentTitle.Size      = new System.Drawing.Size(300, 28);
            this.lblRecentTitle.TabIndex  = 10;
            this.lblRecentTitle.Text      = "Recent Members";

            this.dgvRecentMembers.AllowUserToAddRows  = false;
            this.dgvRecentMembers.BackgroundColor     = System.Drawing.Color.White;
            this.dgvRecentMembers.BorderStyle         = System.Windows.Forms.BorderStyle.None;
            this.dgvRecentMembers.ColumnHeadersHeight = 29;
            this.dgvRecentMembers.EnableHeadersVisualStyles = false;
            this.dgvRecentMembers.GridColor           = System.Drawing.Color.FromArgb(230, 230, 230);
            this.dgvRecentMembers.Location            = new System.Drawing.Point(258, 726);
            this.dgvRecentMembers.MultiSelect         = false;
            this.dgvRecentMembers.Name                = "dgvRecentMembers";
            this.dgvRecentMembers.ReadOnly            = true;
            this.dgvRecentMembers.RowHeadersVisible   = false;
            this.dgvRecentMembers.RowHeadersWidth     = 51;
            this.dgvRecentMembers.SelectionMode       = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvRecentMembers.Size                = new System.Drawing.Size(929, 158);
            this.dgvRecentMembers.TabIndex            = 20;

            // ── timerRefresh ──────────────────────────────────────────────────
            this.timerRefresh.Interval = 300000; // 5 minutes
            this.timerRefresh.Tick    += new System.EventHandler(this.timerRefresh_Tick);

            // ── Form ──────────────────────────────────────────────────────────
            this.BackColor    = System.Drawing.Color.FromArgb(245, 247, 250);
            this.ClientSize   = new System.Drawing.Size(1200, 900);
            this.Controls.Add(this.panelSidebar);
            this.Controls.Add(this.panelTopBar);
            this.Controls.Add(this.cardMembers);
            this.Controls.Add(this.cardActive);
            this.Controls.Add(this.cardRevenue);
            this.Controls.Add(this.cardNew);
            this.Controls.Add(this.panelChartRevCard);
            this.Controls.Add(this.panelChartGrowthCard);
            this.Controls.Add(this.panelChartPlansCard);
            this.Controls.Add(this.panelChartHoursCard);
            this.Controls.Add(this.lblRecentTitle);
            this.Controls.Add(this.dgvRecentMembers);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox     = false;
            this.Name            = "AdminDashboard";
            this.StartPosition   = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text            = "Admin Dashboard";
            this.Load           += new System.EventHandler(this.AdminDashboard_Load);
            this.panelSidebar.ResumeLayout(false);
            this.panelTopBar.ResumeLayout(false);
            this.cardMembers.ResumeLayout(false);
            this.cardActive.ResumeLayout(false);
            this.cardRevenue.ResumeLayout(false);
            this.cardNew.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvRecentMembers)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private Guna.UI2.WinForms.Guna2Panel     panelSidebar;
        private System.Windows.Forms.Label       lblBrand;
        private System.Windows.Forms.Label       lblBrandSub;
        private System.Windows.Forms.Panel       panelDivider;
        private Guna.UI2.WinForms.Guna2Button    btnManageMembers;
        private Guna.UI2.WinForms.Guna2Button    btnManagePlans;
        private Guna.UI2.WinForms.Guna2Button    btnManageTrainers;
        private Guna.UI2.WinForms.Guna2Button    btnPayments;
        private Guna.UI2.WinForms.Guna2Button    btnReports;
        private Guna.UI2.WinForms.Guna2Button    btnChangePassword;
        private Guna.UI2.WinForms.Guna2Button    btnLogout;
        private Guna.UI2.WinForms.Guna2Panel     panelTopBar;
        private System.Windows.Forms.Label       lblPageTitle;
        private System.Windows.Forms.Label       lblWelcome;
        private Guna.UI2.WinForms.Guna2Button    btnRefresh;
        private Guna.UI2.WinForms.Guna2Button    btnClose;
        private Guna.UI2.WinForms.Guna2Panel     cardMembers;
        private System.Windows.Forms.Label       lblCardMembers;
        private System.Windows.Forms.Label       lblCardMembersVal;
        private Guna.UI2.WinForms.Guna2Panel     cardActive;
        private System.Windows.Forms.Label       lblCardActive;
        private System.Windows.Forms.Label       lblCardActiveVal;
        private Guna.UI2.WinForms.Guna2Panel     cardRevenue;
        private System.Windows.Forms.Label       lblCardRevenue;
        private System.Windows.Forms.Label       lblCardRevenueVal;
        private Guna.UI2.WinForms.Guna2Panel     cardNew;
        private System.Windows.Forms.Label       lblCardNew;
        private System.Windows.Forms.Label       lblCardNewVal;
        private Guna.UI2.WinForms.Guna2Panel     panelChartRevCard;
        private Guna.UI2.WinForms.Guna2Panel     panelChartGrowthCard;
        private Guna.UI2.WinForms.Guna2Panel     panelChartPlansCard;
        private Guna.UI2.WinForms.Guna2Panel     panelChartHoursCard;
        private System.Windows.Forms.Label       lblRecentTitle;
        private System.Windows.Forms.DataGridView dgvRecentMembers;
        private System.Windows.Forms.Timer        timerRefresh;
    }
}
