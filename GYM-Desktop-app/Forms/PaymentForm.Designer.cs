namespace GYM_Desktop_app.Forms
{
    partial class PaymentForm
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
            this.panelTopBar = new Guna.UI2.WinForms.Guna2Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.btnClose = new Guna.UI2.WinForms.Guna2Button();
            this.panelCard = new Guna.UI2.WinForms.Guna2Panel();
            this.lblMember = new System.Windows.Forms.Label();
            this.cmbMember = new System.Windows.Forms.ComboBox();
            this.lblAmount = new System.Windows.Forms.Label();
            this.numAmount = new System.Windows.Forms.NumericUpDown();
            this.lblDate = new System.Windows.Forms.Label();
            this.dtpDate = new System.Windows.Forms.DateTimePicker();
            this.lblMethod = new System.Windows.Forms.Label();
            this.cmbMethod = new System.Windows.Forms.ComboBox();
            this.btnSave = new Guna.UI2.WinForms.Guna2Button();
            this.panelTopBar.SuspendLayout();
            this.panelCard.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numAmount)).BeginInit();
            this.SuspendLayout();
            // 
            // panelTopBar
            // 
            this.panelTopBar.Controls.Add(this.lblTitle);
            this.panelTopBar.Controls.Add(this.btnClose);
            this.panelTopBar.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(26)))), ((int)(((byte)(26)))));
            this.panelTopBar.Location = new System.Drawing.Point(0, 0);
            this.panelTopBar.Name = "panelTopBar";
            this.panelTopBar.Size = new System.Drawing.Size(580, 55);
            this.panelTopBar.TabIndex = 0;
            this.panelTopBar.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DragPanel_MouseDown);
            this.panelTopBar.MouseMove += new System.Windows.Forms.MouseEventHandler(this.DragPanel_MouseMove);
            this.panelTopBar.MouseUp += new System.Windows.Forms.MouseEventHandler(this.DragPanel_MouseUp);
            // 
            // lblTitle
            // 
            this.lblTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(20, 12);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(350, 30);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Record Payment";
            // 
            // btnClose
            // 
            this.btnClose.BorderRadius = 20;
            this.btnClose.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(53)))), ((int)(((byte)(69)))));
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(540, 13);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(28, 28);
            this.btnClose.TabIndex = 0;
            this.btnClose.Text = "✕";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // panelCard
            // 
            this.panelCard.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.panelCard.BorderRadius = 12;
            this.panelCard.BorderThickness = 1;
            this.panelCard.Controls.Add(this.lblMember);
            this.panelCard.Controls.Add(this.cmbMember);
            this.panelCard.Controls.Add(this.lblAmount);
            this.panelCard.Controls.Add(this.numAmount);
            this.panelCard.Controls.Add(this.lblDate);
            this.panelCard.Controls.Add(this.dtpDate);
            this.panelCard.Controls.Add(this.lblMethod);
            this.panelCard.Controls.Add(this.cmbMethod);
            this.panelCard.Controls.Add(this.btnSave);
            this.panelCard.FillColor = System.Drawing.Color.White;
            this.panelCard.Location = new System.Drawing.Point(15, 70);
            this.panelCard.Name = "panelCard";
            this.panelCard.Size = new System.Drawing.Size(550, 360);
            this.panelCard.TabIndex = 1;
            // 
            // lblMember
            // 
            this.lblMember.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.lblMember.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.lblMember.Location = new System.Drawing.Point(20, 20);
            this.lblMember.Name = "lblMember";
            this.lblMember.Size = new System.Drawing.Size(510, 17);
            this.lblMember.TabIndex = 0;
            this.lblMember.Text = "SELECT MEMBER";
            // 
            // cmbMember
            // 
            this.cmbMember.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMember.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cmbMember.Location = new System.Drawing.Point(20, 40);
            this.cmbMember.Name = "cmbMember";
            this.cmbMember.Size = new System.Drawing.Size(510, 31);
            this.cmbMember.TabIndex = 1;
            // 
            // lblAmount
            // 
            this.lblAmount.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.lblAmount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.lblAmount.Location = new System.Drawing.Point(20, 90);
            this.lblAmount.Name = "lblAmount";
            this.lblAmount.Size = new System.Drawing.Size(510, 17);
            this.lblAmount.TabIndex = 2;
            this.lblAmount.Text = "AMOUNT (EGP)";
            // 
            // numAmount
            // 
            this.numAmount.DecimalPlaces = 2;
            this.numAmount.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.numAmount.Increment = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numAmount.Location = new System.Drawing.Point(20, 110);
            this.numAmount.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.numAmount.Name = "numAmount";
            this.numAmount.Size = new System.Drawing.Size(510, 30);
            this.numAmount.TabIndex = 3;
            // 
            // lblDate
            // 
            this.lblDate.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.lblDate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.lblDate.Location = new System.Drawing.Point(20, 165);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(510, 17);
            this.lblDate.TabIndex = 4;
            this.lblDate.Text = "PAYMENT DATE";
            // 
            // dtpDate
            // 
            this.dtpDate.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.dtpDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDate.Location = new System.Drawing.Point(20, 185);
            this.dtpDate.Name = "dtpDate";
            this.dtpDate.Size = new System.Drawing.Size(510, 30);
            this.dtpDate.TabIndex = 5;
            // 
            // lblMethod
            // 
            this.lblMethod.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.lblMethod.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.lblMethod.Location = new System.Drawing.Point(20, 238);
            this.lblMethod.Name = "lblMethod";
            this.lblMethod.Size = new System.Drawing.Size(510, 17);
            this.lblMethod.TabIndex = 6;
            this.lblMethod.Text = "PAYMENT METHOD";
            // 
            // cmbMethod
            // 
            this.cmbMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMethod.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cmbMethod.Items.AddRange(new object[] {
            "Cash",
            "Vodafone Cash",
            "InstaPay"});
            this.cmbMethod.Location = new System.Drawing.Point(20, 258);
            this.cmbMethod.Name = "cmbMethod";
            this.cmbMethod.Size = new System.Drawing.Size(510, 31);
            this.cmbMethod.TabIndex = 7;
            // 
            // btnSave
            // 
            this.btnSave.BorderRadius = 8;
            this.btnSave.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(167)))), ((int)(((byte)(69)))));
            this.btnSave.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.Location = new System.Drawing.Point(20, 305);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(510, 45);
            this.btnSave.TabIndex = 8;
            this.btnSave.Text = "Save Payment";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // PaymentForm
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.ClientSize = new System.Drawing.Size(580, 450);
            this.Controls.Add(this.panelTopBar);
            this.Controls.Add(this.panelCard);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.Name = "PaymentForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Record Payment";
            this.Load += new System.EventHandler(this.PaymentForm_Load);
            this.panelTopBar.ResumeLayout(false);
            this.panelCard.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numAmount)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Guna.UI2.WinForms.Guna2Panel    panelTopBar;
        private System.Windows.Forms.Label      lblTitle;
        private Guna.UI2.WinForms.Guna2Button   btnClose;
        private Guna.UI2.WinForms.Guna2Panel    panelCard;
        private System.Windows.Forms.Label      lblMember;
        private System.Windows.Forms.ComboBox   cmbMember;
        private System.Windows.Forms.Label      lblAmount;
        private System.Windows.Forms.NumericUpDown numAmount;
        private System.Windows.Forms.Label      lblDate;
        private System.Windows.Forms.DateTimePicker dtpDate;
        private System.Windows.Forms.Label      lblMethod;
        private System.Windows.Forms.ComboBox   cmbMethod;
        private Guna.UI2.WinForms.Guna2Button   btnSave;
    }
}
