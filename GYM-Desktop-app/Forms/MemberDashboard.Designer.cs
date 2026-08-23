namespace GYM_Desktop_app.Forms
{
    partial class MemberDashboard
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
            this.panelSidebar = new Guna.UI2.WinForms.Guna2Panel();
            this.lblBrand = new System.Windows.Forms.Label();
            this.lblBrandSub = new System.Windows.Forms.Label();
            this.btnViewSchedule = new Guna.UI2.WinForms.Guna2Button();
            this.btnChangePassword = new Guna.UI2.WinForms.Guna2Button();
            this.btnSelfCheckIn = new Guna.UI2.WinForms.Guna2Button();
            this.btnLogout = new Guna.UI2.WinForms.Guna2Button();
            this.pnlMyQR = new Guna.UI2.WinForms.Guna2Panel();
            this.lblQRTitle = new System.Windows.Forms.Label();
            this.lblQRSubtitle = new System.Windows.Forms.Label();
            this.picMyQR = new System.Windows.Forms.PictureBox();
            this.lblMemberCode = new System.Windows.Forms.Label();
            this.btnSaveQR = new Guna.UI2.WinForms.Guna2Button();
            this.btnPrintCard = new Guna.UI2.WinForms.Guna2Button();
            this.panelTopBar = new Guna.UI2.WinForms.Guna2Panel();
            this.lblWelcome = new System.Windows.Forms.Label();
            this.btnClose = new Guna.UI2.WinForms.Guna2Button();
            this.panelInfoCard = new Guna.UI2.WinForms.Guna2Panel();
            this.lblInfoTitle = new System.Windows.Forms.Label();
            this.lblPlanLabel = new System.Windows.Forms.Label();
            this.lblPlan = new System.Windows.Forms.Label();
            this.lblExpiryLabel = new System.Windows.Forms.Label();
            this.lblExpiry = new System.Windows.Forms.Label();
            this.lblStatusLabel = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.panelSidebar.SuspendLayout();
            this.panelTopBar.SuspendLayout();
            this.panelInfoCard.SuspendLayout();
            this.pnlMyQR.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picMyQR)).BeginInit();
            this.SuspendLayout();
            // 
            // panelSidebar
            // 
            this.panelSidebar.Controls.Add(this.lblBrand);
            this.panelSidebar.Controls.Add(this.lblBrandSub);
            this.panelSidebar.Controls.Add(this.btnViewSchedule);
            this.panelSidebar.Controls.Add(this.btnChangePassword);
            this.panelSidebar.Controls.Add(this.btnLogout);
            this.panelSidebar.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(18)))), ((int)(((byte)(18)))));
            this.panelSidebar.Location = new System.Drawing.Point(0, 0);
            this.panelSidebar.Name = "panelSidebar";
            this.panelSidebar.Size = new System.Drawing.Size(240, 700);
            this.panelSidebar.TabIndex = 0;
            this.panelSidebar.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DragPanel_MouseDown);
            this.panelSidebar.MouseMove += new System.Windows.Forms.MouseEventHandler(this.DragPanel_MouseMove);
            this.panelSidebar.MouseUp += new System.Windows.Forms.MouseEventHandler(this.DragPanel_MouseUp);
            // 
            // lblBrand
            // 
            this.lblBrand.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblBrand.ForeColor = System.Drawing.Color.Teal;
            this.lblBrand.Location = new System.Drawing.Point(0, 28);
            this.lblBrand.Name = "lblBrand";
            this.lblBrand.Size = new System.Drawing.Size(240, 40);
            this.lblBrand.TabIndex = 0;
            this.lblBrand.Text = "UFA";
            this.lblBrand.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblBrandSub
            // 
            this.lblBrandSub.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblBrandSub.ForeColor = System.Drawing.Color.Teal;
            this.lblBrandSub.Location = new System.Drawing.Point(0, 66);
            this.lblBrandSub.Name = "lblBrandSub";
            this.lblBrandSub.Size = new System.Drawing.Size(240, 18);
            this.lblBrandSub.TabIndex = 1;
            this.lblBrandSub.Text = "Member Portal";
            this.lblBrandSub.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnViewSchedule
            // 
            this.btnViewSchedule.BorderRadius = 8;
            this.btnViewSchedule.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(18)))), ((int)(((byte)(18)))));
            this.btnViewSchedule.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnViewSchedule.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.btnViewSchedule.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(101)))), ((int)(((byte)(34)))));
            this.btnViewSchedule.HoverState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.btnViewSchedule.Location = new System.Drawing.Point(10, 110);
            this.btnViewSchedule.Name = "btnViewSchedule";
            this.btnViewSchedule.Size = new System.Drawing.Size(220, 50);
            this.btnViewSchedule.TabIndex = 2;
            this.btnViewSchedule.Text = "  Workout Schedule";
            this.btnViewSchedule.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnViewSchedule.Click += new System.EventHandler(this.btnViewSchedule_Click);
            //
            // btnChangePassword
            //
            this.btnChangePassword.BorderRadius = 8;
            this.btnChangePassword.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(18)))), ((int)(((byte)(18)))));
            this.btnChangePassword.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnChangePassword.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.btnChangePassword.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(101)))), ((int)(((byte)(34)))));
            this.btnChangePassword.HoverState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.btnChangePassword.Location = new System.Drawing.Point(10, 170);
            this.btnChangePassword.Name = "btnChangePassword";
            this.btnChangePassword.Size = new System.Drawing.Size(220, 50);
            this.btnChangePassword.TabIndex = 3;
            this.btnChangePassword.Text = "  Change Password";
            this.btnChangePassword.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnChangePassword.Click += new System.EventHandler(this.btnChangePassword_Click);
            //
            // btnSelfCheckIn
            //
            this.btnSelfCheckIn.BorderRadius         = 12;
            this.btnSelfCheckIn.FillColor            = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(167)))), ((int)(((byte)(69)))));
            this.btnSelfCheckIn.Font                 = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.btnSelfCheckIn.ForeColor            = System.Drawing.Color.White;
            this.btnSelfCheckIn.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(130)))), ((int)(((byte)(55)))));
            this.btnSelfCheckIn.Location             = new System.Drawing.Point(270, 78);
            this.btnSelfCheckIn.Name                 = "btnSelfCheckIn";
            this.btnSelfCheckIn.Size                 = new System.Drawing.Size(715, 65);
            this.btnSelfCheckIn.TabIndex             = 4;
            this.btnSelfCheckIn.Text                 = "Check In / Check Out";
            this.btnSelfCheckIn.Click               += new System.EventHandler(this.btnSelfCheckIn_Click);
            //
            // btnLogout
            // 
            this.btnLogout.BorderRadius = 8;
            this.btnLogout.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(53)))), ((int)(((byte)(69)))));
            this.btnLogout.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnLogout.ForeColor = System.Drawing.Color.White;
            this.btnLogout.Location = new System.Drawing.Point(20, 630);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(200, 45);
            this.btnLogout.TabIndex = 5;
            this.btnLogout.Text = "  Logout";
            this.btnLogout.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);
            // 
            // panelTopBar
            // 
            this.panelTopBar.Controls.Add(this.lblWelcome);
            this.panelTopBar.Controls.Add(this.btnClose);
            this.panelTopBar.FillColor = System.Drawing.Color.White;
            this.panelTopBar.Location = new System.Drawing.Point(241, 0);
            this.panelTopBar.Name = "panelTopBar";
            this.panelTopBar.Size = new System.Drawing.Size(754, 68);
            this.panelTopBar.TabIndex = 1;
            // 
            // lblWelcome
            // 
            this.lblWelcome.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblWelcome.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.lblWelcome.Location = new System.Drawing.Point(25, 15);
            this.lblWelcome.Name = "lblWelcome";
            this.lblWelcome.Size = new System.Drawing.Size(600, 35);
            this.lblWelcome.TabIndex = 0;
            this.lblWelcome.Text = "Welcome!";
            // 
            // btnClose
            // 
            this.btnClose.BorderRadius = 20;
            this.btnClose.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(53)))), ((int)(((byte)(69)))));
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(720, 18);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(28, 28);
            this.btnClose.TabIndex = 0;
            this.btnClose.Text = "✕";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // panelInfoCard
            // 
            this.panelInfoCard.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.panelInfoCard.BorderRadius = 12;
            this.panelInfoCard.BorderThickness = 1;
            this.panelInfoCard.Controls.Add(this.lblInfoTitle);
            this.panelInfoCard.Controls.Add(this.lblPlanLabel);
            this.panelInfoCard.Controls.Add(this.lblPlan);
            this.panelInfoCard.Controls.Add(this.lblExpiryLabel);
            this.panelInfoCard.Controls.Add(this.lblExpiry);
            this.panelInfoCard.Controls.Add(this.lblStatusLabel);
            this.panelInfoCard.Controls.Add(this.lblStatus);
            this.panelInfoCard.FillColor = System.Drawing.Color.White;
            this.panelInfoCard.Location = new System.Drawing.Point(270, 153);
            this.panelInfoCard.Name = "panelInfoCard";
            this.panelInfoCard.Size = new System.Drawing.Size(700, 220);
            this.panelInfoCard.TabIndex = 2;
            // 
            // lblInfoTitle
            // 
            this.lblInfoTitle.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            this.lblInfoTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.lblInfoTitle.Location = new System.Drawing.Point(25, 18);
            this.lblInfoTitle.Name = "lblInfoTitle";
            this.lblInfoTitle.Size = new System.Drawing.Size(650, 30);
            this.lblInfoTitle.TabIndex = 0;
            this.lblInfoTitle.Text = "Membership Information";
            // 
            // lblPlanLabel
            // 
            this.lblPlanLabel.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblPlanLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.lblPlanLabel.Location = new System.Drawing.Point(25, 65);
            this.lblPlanLabel.Name = "lblPlanLabel";
            this.lblPlanLabel.Size = new System.Drawing.Size(165, 28);
            this.lblPlanLabel.TabIndex = 1;
            this.lblPlanLabel.Text = "Membership Plan";
            this.lblPlanLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblPlan
            // 
            this.lblPlan.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblPlan.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.lblPlan.Location = new System.Drawing.Point(200, 65);
            this.lblPlan.Name = "lblPlan";
            this.lblPlan.Size = new System.Drawing.Size(430, 28);
            this.lblPlan.TabIndex = 2;
            this.lblPlan.Text = "—";
            this.lblPlan.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblExpiryLabel
            // 
            this.lblExpiryLabel.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblExpiryLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.lblExpiryLabel.Location = new System.Drawing.Point(25, 115);
            this.lblExpiryLabel.Name = "lblExpiryLabel";
            this.lblExpiryLabel.Size = new System.Drawing.Size(165, 28);
            this.lblExpiryLabel.TabIndex = 3;
            this.lblExpiryLabel.Text = "Expiry Date";
            this.lblExpiryLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblExpiry
            // 
            this.lblExpiry.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblExpiry.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.lblExpiry.Location = new System.Drawing.Point(200, 115);
            this.lblExpiry.Name = "lblExpiry";
            this.lblExpiry.Size = new System.Drawing.Size(430, 28);
            this.lblExpiry.TabIndex = 4;
            this.lblExpiry.Text = "—";
            this.lblExpiry.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblStatusLabel
            // 
            this.lblStatusLabel.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblStatusLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.lblStatusLabel.Location = new System.Drawing.Point(25, 165);
            this.lblStatusLabel.Name = "lblStatusLabel";
            this.lblStatusLabel.Size = new System.Drawing.Size(165, 28);
            this.lblStatusLabel.TabIndex = 5;
            this.lblStatusLabel.Text = "Status";
            this.lblStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblStatus
            // 
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.lblStatus.Location = new System.Drawing.Point(200, 165);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(430, 28);
            this.lblStatus.TabIndex = 6;
            this.lblStatus.Text = "—";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // pnlMyQR
            //
            this.pnlMyQR.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.pnlMyQR.BorderRadius = 12;
            this.pnlMyQR.BorderThickness = 1;
            this.pnlMyQR.Controls.Add(this.lblQRTitle);
            this.pnlMyQR.Controls.Add(this.lblQRSubtitle);
            this.pnlMyQR.Controls.Add(this.picMyQR);
            this.pnlMyQR.Controls.Add(this.lblMemberCode);
            this.pnlMyQR.Controls.Add(this.btnSaveQR);
            this.pnlMyQR.Controls.Add(this.btnPrintCard);
            this.pnlMyQR.FillColor = System.Drawing.Color.White;
            this.pnlMyQR.Location = new System.Drawing.Point(270, 388);
            this.pnlMyQR.Name = "pnlMyQR";
            this.pnlMyQR.Size = new System.Drawing.Size(700, 290);
            this.pnlMyQR.TabIndex = 5;
            //
            // lblQRTitle
            //
            this.lblQRTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblQRTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.lblQRTitle.Location = new System.Drawing.Point(20, 15);
            this.lblQRTitle.Name = "lblQRTitle";
            this.lblQRTitle.Size = new System.Drawing.Size(400, 28);
            this.lblQRTitle.TabIndex = 0;
            this.lblQRTitle.Text = "Your Member QR Code";
            //
            // lblQRSubtitle
            //
            this.lblQRSubtitle.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblQRSubtitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.lblQRSubtitle.Location = new System.Drawing.Point(20, 47);
            this.lblQRSubtitle.Name = "lblQRSubtitle";
            this.lblQRSubtitle.Size = new System.Drawing.Size(650, 22);
            this.lblQRSubtitle.TabIndex = 1;
            this.lblQRSubtitle.Text = "Show this at the gym entrance to check in";
            //
            // picMyQR
            //
            this.picMyQR.BackColor = System.Drawing.Color.White;
            this.picMyQR.Location = new System.Drawing.Point(20, 78);
            this.picMyQR.Name = "picMyQR";
            this.picMyQR.Size = new System.Drawing.Size(200, 200);
            this.picMyQR.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picMyQR.TabIndex = 2;
            this.picMyQR.TabStop = false;
            //
            // lblMemberCode
            //
            this.lblMemberCode.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            this.lblMemberCode.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(101)))), ((int)(((byte)(34)))));
            this.lblMemberCode.Location = new System.Drawing.Point(240, 78);
            this.lblMemberCode.Name = "lblMemberCode";
            this.lblMemberCode.Size = new System.Drawing.Size(430, 32);
            this.lblMemberCode.TabIndex = 3;
            this.lblMemberCode.Text = "Code: MBR-...";
            //
            // btnSaveQR
            //
            this.btnSaveQR.BorderRadius = 8;
            this.btnSaveQR.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(101)))), ((int)(((byte)(34)))));
            this.btnSaveQR.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnSaveQR.ForeColor = System.Drawing.Color.White;
            this.btnSaveQR.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(26)))), ((int)(((byte)(26)))));
            this.btnSaveQR.Location = new System.Drawing.Point(240, 126);
            this.btnSaveQR.Name = "btnSaveQR";
            this.btnSaveQR.Size = new System.Drawing.Size(200, 42);
            this.btnSaveQR.TabIndex = 4;
            this.btnSaveQR.Text = "Save as Image";
            this.btnSaveQR.Click += new System.EventHandler(this.btnSaveQR_Click);
            //
            // btnPrintCard
            //
            this.btnPrintCard.BorderRadius = 8;
            this.btnPrintCard.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(152)))), ((int)(((byte)(0)))));
            this.btnPrintCard.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnPrintCard.ForeColor = System.Drawing.Color.White;
            this.btnPrintCard.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(130)))), ((int)(((byte)(0)))));
            this.btnPrintCard.Location = new System.Drawing.Point(240, 180);
            this.btnPrintCard.Name = "btnPrintCard";
            this.btnPrintCard.Size = new System.Drawing.Size(200, 42);
            this.btnPrintCard.TabIndex = 5;
            this.btnPrintCard.Text = "Print Member Card";
            this.btnPrintCard.Click += new System.EventHandler(this.btnPrintCard_Click);
            //
            // MemberDashboard
            //
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.ClientSize = new System.Drawing.Size(1000, 700);
            this.Controls.Add(this.panelSidebar);
            this.Controls.Add(this.panelTopBar);
            this.Controls.Add(this.btnSelfCheckIn);
            this.Controls.Add(this.panelInfoCard);
            this.Controls.Add(this.pnlMyQR);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.Name = "MemberDashboard";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Member Dashboard";
            this.Load += new System.EventHandler(this.MemberDashboard_Load);
            this.panelSidebar.ResumeLayout(false);
            this.panelTopBar.ResumeLayout(false);
            this.panelInfoCard.ResumeLayout(false);
            this.pnlMyQR.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picMyQR)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Guna.UI2.WinForms.Guna2Panel  panelSidebar;
        private System.Windows.Forms.Label    lblBrand;
        private System.Windows.Forms.Label    lblBrandSub;
        private Guna.UI2.WinForms.Guna2Button btnViewSchedule;
        private Guna.UI2.WinForms.Guna2Button btnChangePassword;
        private Guna.UI2.WinForms.Guna2Button btnSelfCheckIn;
        private Guna.UI2.WinForms.Guna2Button btnLogout;
        private Guna.UI2.WinForms.Guna2Panel  pnlMyQR;
        private System.Windows.Forms.Label    lblQRTitle;
        private System.Windows.Forms.Label    lblQRSubtitle;
        private System.Windows.Forms.PictureBox picMyQR;
        private System.Windows.Forms.Label    lblMemberCode;
        private Guna.UI2.WinForms.Guna2Button btnSaveQR;
        private Guna.UI2.WinForms.Guna2Button btnPrintCard;
        private Guna.UI2.WinForms.Guna2Panel  panelTopBar;
        private System.Windows.Forms.Label    lblWelcome;
        private Guna.UI2.WinForms.Guna2Button btnClose;
        private Guna.UI2.WinForms.Guna2Panel  panelInfoCard;
        private System.Windows.Forms.Label    lblInfoTitle;
        private System.Windows.Forms.Label    lblPlanLabel;
        private System.Windows.Forms.Label    lblPlan;
        private System.Windows.Forms.Label    lblExpiryLabel;
        private System.Windows.Forms.Label    lblExpiry;
        private System.Windows.Forms.Label    lblStatusLabel;
        private System.Windows.Forms.Label    lblStatus;
    }
}
