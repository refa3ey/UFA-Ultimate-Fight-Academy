namespace GYM_Desktop_app.Forms
{
    partial class MemberCardPrintForm
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
            this.pnlTitleBar     = new Guna.UI2.WinForms.Guna2Panel();
            this.lblCardTitle    = new System.Windows.Forms.Label();
            this.btnCloseForm    = new Guna.UI2.WinForms.Guna2Button();
            this.picCardPreview  = new System.Windows.Forms.PictureBox();
            this.btnPrint        = new Guna.UI2.WinForms.Guna2Button();
            this.btnSaveImage    = new Guna.UI2.WinForms.Guna2Button();
            this.btnClose        = new Guna.UI2.WinForms.Guna2Button();
            this.pnlTitleBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picCardPreview)).BeginInit();
            this.SuspendLayout();
            //
            // pnlTitleBar
            //
            this.pnlTitleBar.Controls.Add(this.lblCardTitle);
            this.pnlTitleBar.Controls.Add(this.btnCloseForm);
            this.pnlTitleBar.FillColor = System.Drawing.Color.FromArgb(26, 26, 26);
            this.pnlTitleBar.Location = new System.Drawing.Point(0, 0);
            this.pnlTitleBar.Name = "pnlTitleBar";
            this.pnlTitleBar.Size = new System.Drawing.Size(900, 50);
            this.pnlTitleBar.TabIndex = 0;
            this.pnlTitleBar.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnlTitleBar_MouseDown);
            this.pnlTitleBar.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pnlTitleBar_MouseMove);
            this.pnlTitleBar.MouseUp   += new System.Windows.Forms.MouseEventHandler(this.pnlTitleBar_MouseUp);
            //
            // lblCardTitle
            //
            this.lblCardTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblCardTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblCardTitle.ForeColor = System.Drawing.Color.White;
            this.lblCardTitle.Location = new System.Drawing.Point(20, 10);
            this.lblCardTitle.Name = "lblCardTitle";
            this.lblCardTitle.Size = new System.Drawing.Size(400, 30);
            this.lblCardTitle.TabIndex = 0;
            this.lblCardTitle.Text = "Print Member Card";
            //
            // btnCloseForm
            //
            this.btnCloseForm.BorderRadius = 20;
            this.btnCloseForm.FillColor = System.Drawing.Color.FromArgb(220, 53, 69);
            this.btnCloseForm.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnCloseForm.ForeColor = System.Drawing.Color.White;
            this.btnCloseForm.Location = new System.Drawing.Point(860, 11);
            this.btnCloseForm.Name = "btnCloseForm";
            this.btnCloseForm.Size = new System.Drawing.Size(28, 28);
            this.btnCloseForm.TabIndex = 1;
            this.btnCloseForm.Text = "✕";
            this.btnCloseForm.Click += new System.EventHandler(this.btnClose_Click);
            //
            // picCardPreview
            //
            this.picCardPreview.BackColor = System.Drawing.Color.White;
            this.picCardPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picCardPreview.Location = new System.Drawing.Point(30, 58);
            this.picCardPreview.Name = "picCardPreview";
            this.picCardPreview.Size = new System.Drawing.Size(840, 528);
            this.picCardPreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picCardPreview.TabIndex = 1;
            this.picCardPreview.TabStop = false;
            //
            // btnPrint
            //
            this.btnPrint.BorderRadius = 8;
            this.btnPrint.FillColor = System.Drawing.Color.FromArgb(242, 101, 34);
            this.btnPrint.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnPrint.ForeColor = System.Drawing.Color.White;
            this.btnPrint.HoverState.FillColor = System.Drawing.Color.FromArgb(26, 26, 26);
            this.btnPrint.Location = new System.Drawing.Point(185, 600);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(150, 42);
            this.btnPrint.TabIndex = 2;
            this.btnPrint.Text = "Print";
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            //
            // btnSaveImage
            //
            this.btnSaveImage.BorderRadius = 8;
            this.btnSaveImage.FillColor = System.Drawing.Color.FromArgb(40, 167, 69);
            this.btnSaveImage.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnSaveImage.ForeColor = System.Drawing.Color.White;
            this.btnSaveImage.HoverState.FillColor = System.Drawing.Color.FromArgb(30, 130, 55);
            this.btnSaveImage.Location = new System.Drawing.Point(350, 600);
            this.btnSaveImage.Name = "btnSaveImage";
            this.btnSaveImage.Size = new System.Drawing.Size(200, 42);
            this.btnSaveImage.TabIndex = 3;
            this.btnSaveImage.Text = "Save as Image";
            this.btnSaveImage.Click += new System.EventHandler(this.btnSaveImage_Click);
            //
            // btnClose
            //
            this.btnClose.BorderRadius = 8;
            this.btnClose.FillColor = System.Drawing.Color.FromArgb(120, 120, 120);
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.HoverState.FillColor = System.Drawing.Color.FromArgb(90, 90, 90);
            this.btnClose.Location = new System.Drawing.Point(565, 600);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(150, 42);
            this.btnClose.TabIndex = 4;
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            //
            // MemberCardPrintForm
            //
            this.BackColor = System.Drawing.Color.FromArgb(245, 247, 250);
            this.ClientSize = new System.Drawing.Size(900, 660);
            this.Controls.Add(this.pnlTitleBar);
            this.Controls.Add(this.picCardPreview);
            this.Controls.Add(this.btnPrint);
            this.Controls.Add(this.btnSaveImage);
            this.Controls.Add(this.btnClose);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "MemberCardPrintForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Print Member Card";
            this.Load += new System.EventHandler(this.MemberCardPrintForm_Load);
            this.pnlTitleBar.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picCardPreview)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private Guna.UI2.WinForms.Guna2Panel   pnlTitleBar;
        private System.Windows.Forms.Label     lblCardTitle;
        private Guna.UI2.WinForms.Guna2Button  btnCloseForm;
        private System.Windows.Forms.PictureBox picCardPreview;
        private Guna.UI2.WinForms.Guna2Button  btnPrint;
        private Guna.UI2.WinForms.Guna2Button  btnSaveImage;
        private Guna.UI2.WinForms.Guna2Button  btnClose;
    }
}
