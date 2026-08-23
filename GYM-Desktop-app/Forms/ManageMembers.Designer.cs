namespace GYM_Desktop_app.Forms
{
    partial class ManageMembers
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
            this.panelTopBar    = new Guna.UI2.WinForms.Guna2Panel();
            this.lblTitle       = new System.Windows.Forms.Label();
            this.btnClose       = new Guna.UI2.WinForms.Guna2Button();
            this.panelSearch    = new Guna.UI2.WinForms.Guna2Panel();
            this.txtSearch      = new Guna.UI2.WinForms.Guna2TextBox();
            this.cmbFilter      = new Guna.UI2.WinForms.Guna2ComboBox();
            this.btnRefresh        = new Guna.UI2.WinForms.Guna2Button();
            this.btnExportMembers  = new Guna.UI2.WinForms.Guna2Button();
            this.lblResultCount    = new System.Windows.Forms.Label();
            this.dgvMembers     = new System.Windows.Forms.DataGridView();
            this.panelInputs    = new Guna.UI2.WinForms.Guna2Panel();
            this.lblName        = new System.Windows.Forms.Label();
            this.txtName        = new Guna.UI2.WinForms.Guna2TextBox();
            this.lblPhone       = new System.Windows.Forms.Label();
            this.txtPhone       = new Guna.UI2.WinForms.Guna2TextBox();
            this.lblAge         = new System.Windows.Forms.Label();
            this.numAge         = new System.Windows.Forms.NumericUpDown();
            this.lblAddress     = new System.Windows.Forms.Label();
            this.txtAddress     = new Guna.UI2.WinForms.Guna2TextBox();
            this.lblPlan        = new System.Windows.Forms.Label();
            this.cmbPlan        = new System.Windows.Forms.ComboBox();
            this.btnAdd         = new Guna.UI2.WinForms.Guna2Button();
            this.btnUpdate      = new Guna.UI2.WinForms.Guna2Button();
            this.btnDelete      = new Guna.UI2.WinForms.Guna2Button();
            this.btnClear       = new Guna.UI2.WinForms.Guna2Button();
            this.lblMemberQRLabel = new System.Windows.Forms.Label();
            this.picMemberQR    = new System.Windows.Forms.PictureBox();
            this.btnPrintCard   = new Guna.UI2.WinForms.Guna2Button();
            this.panelTopBar.SuspendLayout();
            this.panelSearch.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMembers)).BeginInit();
            this.panelInputs.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numAge)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picMemberQR)).BeginInit();
            this.SuspendLayout();
            //
            // panelTopBar
            //
            this.panelTopBar.Controls.Add(this.lblTitle);
            this.panelTopBar.Controls.Add(this.btnClose);
            this.panelTopBar.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(26)))), ((int)(((byte)(26)))));
            this.panelTopBar.Location = new System.Drawing.Point(0, 0);
            this.panelTopBar.Name = "panelTopBar";
            this.panelTopBar.Size = new System.Drawing.Size(1100, 55);
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
            this.lblTitle.Size = new System.Drawing.Size(400, 30);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Manage Members";
            //
            // btnClose
            //
            this.btnClose.BorderRadius = 20;
            this.btnClose.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(53)))), ((int)(((byte)(69)))));
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(1060, 13);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(28, 28);
            this.btnClose.TabIndex = 0;
            this.btnClose.Text = "✕";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            //
            // panelSearch
            //
            this.panelSearch.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.panelSearch.BorderRadius = 12;
            this.panelSearch.BorderThickness = 1;
            this.panelSearch.Controls.Add(this.txtSearch);
            this.panelSearch.Controls.Add(this.cmbFilter);
            this.panelSearch.Controls.Add(this.btnRefresh);
            this.panelSearch.Controls.Add(this.btnExportMembers);
            this.panelSearch.Controls.Add(this.lblResultCount);
            this.panelSearch.FillColor = System.Drawing.Color.White;
            this.panelSearch.Location = new System.Drawing.Point(10, 63);
            this.panelSearch.Name = "panelSearch";
            this.panelSearch.Size = new System.Drawing.Size(1080, 85);
            this.panelSearch.TabIndex = 1;
            //
            // txtSearch
            //
            this.txtSearch.BorderRadius = 8;
            this.txtSearch.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(101)))), ((int)(((byte)(34)))));
            this.txtSearch.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtSearch.Location = new System.Drawing.Point(12, 12);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.PlaceholderText = "Search by name, phone, or address...";
            this.txtSearch.Size = new System.Drawing.Size(400, 40);
            this.txtSearch.TabIndex = 0;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            //
            // cmbFilter
            //
            this.cmbFilter.BorderRadius = 8;
            this.cmbFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFilter.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(101)))), ((int)(((byte)(34)))));
            this.cmbFilter.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cmbFilter.Items.AddRange(new object[] {
                "All Members",
                "Active",
                "Expired",
                "Expiring Soon (7 days)"});
            this.cmbFilter.Location = new System.Drawing.Point(422, 12);
            this.cmbFilter.Name = "cmbFilter";
            this.cmbFilter.Size = new System.Drawing.Size(200, 40);
            this.cmbFilter.TabIndex = 1;
            this.cmbFilter.SelectedIndex = 0;
            this.cmbFilter.SelectedIndexChanged += new System.EventHandler(this.cmbFilter_SelectedIndexChanged);
            //
            // btnRefresh
            //
            this.btnRefresh.BorderRadius = 8;
            this.btnRefresh.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(167)))), ((int)(((byte)(69)))));
            this.btnRefresh.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnRefresh.ForeColor = System.Drawing.Color.White;
            this.btnRefresh.Location = new System.Drawing.Point(632, 12);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(120, 40);
            this.btnRefresh.TabIndex = 2;
            this.btnRefresh.Text = "↻  Refresh";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            //
            // btnExportMembers
            //
            this.btnExportMembers.BorderRadius         = 8;
            this.btnExportMembers.FillColor            = System.Drawing.Color.FromArgb(40, 167, 69);
            this.btnExportMembers.Font                 = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnExportMembers.ForeColor            = System.Drawing.Color.White;
            this.btnExportMembers.HoverState.FillColor = System.Drawing.Color.FromArgb(30, 130, 55);
            this.btnExportMembers.Location             = new System.Drawing.Point(762, 12);
            this.btnExportMembers.Name                 = "btnExportMembers";
            this.btnExportMembers.Size                 = new System.Drawing.Size(155, 40);
            this.btnExportMembers.TabIndex             = 3;
            this.btnExportMembers.Text                 = "Export Excel";
            this.btnExportMembers.Click               += new System.EventHandler(this.btnExportMembers_Click);
            //
            // lblResultCount
            //
            this.lblResultCount.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblResultCount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.lblResultCount.Location = new System.Drawing.Point(12, 60);
            this.lblResultCount.Name = "lblResultCount";
            this.lblResultCount.Size = new System.Drawing.Size(500, 18);
            this.lblResultCount.TabIndex = 3;
            this.lblResultCount.Text = "Showing 0 of 0 members";
            //
            // dgvMembers
            //
            this.dgvMembers.AllowUserToAddRows = false;
            this.dgvMembers.BackgroundColor = System.Drawing.Color.White;
            this.dgvMembers.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvMembers.ColumnHeadersHeight = 29;
            this.dgvMembers.EnableHeadersVisualStyles = false;
            this.dgvMembers.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.dgvMembers.Location = new System.Drawing.Point(10, 158);
            this.dgvMembers.MultiSelect = false;
            this.dgvMembers.Name = "dgvMembers";
            this.dgvMembers.ReadOnly = true;
            this.dgvMembers.RowHeadersVisible = false;
            this.dgvMembers.RowHeadersWidth = 51;
            this.dgvMembers.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvMembers.Size = new System.Drawing.Size(1080, 280);
            this.dgvMembers.TabIndex = 2;
            this.dgvMembers.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvMembers_CellClick);
            this.dgvMembers.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvMembers_CellFormatting);
            this.dgvMembers.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.dgvMembers_DataBindingComplete);
            //
            // panelInputs
            //
            this.panelInputs.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.panelInputs.BorderRadius = 12;
            this.panelInputs.BorderThickness = 1;
            this.panelInputs.Controls.Add(this.lblName);
            this.panelInputs.Controls.Add(this.txtName);
            this.panelInputs.Controls.Add(this.lblPhone);
            this.panelInputs.Controls.Add(this.txtPhone);
            this.panelInputs.Controls.Add(this.lblAge);
            this.panelInputs.Controls.Add(this.numAge);
            this.panelInputs.Controls.Add(this.lblAddress);
            this.panelInputs.Controls.Add(this.txtAddress);
            this.panelInputs.Controls.Add(this.lblPlan);
            this.panelInputs.Controls.Add(this.cmbPlan);
            this.panelInputs.Controls.Add(this.btnAdd);
            this.panelInputs.Controls.Add(this.btnUpdate);
            this.panelInputs.Controls.Add(this.btnDelete);
            this.panelInputs.Controls.Add(this.btnClear);
            this.panelInputs.Controls.Add(this.lblMemberQRLabel);
            this.panelInputs.Controls.Add(this.picMemberQR);
            this.panelInputs.Controls.Add(this.btnPrintCard);
            this.panelInputs.FillColor = System.Drawing.Color.White;
            this.panelInputs.Location = new System.Drawing.Point(10, 450);
            this.panelInputs.Name = "panelInputs";
            this.panelInputs.Size = new System.Drawing.Size(1080, 220);
            this.panelInputs.TabIndex = 3;
            //
            // lblName
            //
            this.lblName.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.lblName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.lblName.Location = new System.Drawing.Point(20, 15);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(260, 17);
            this.lblName.TabIndex = 0;
            this.lblName.Text = "FULL NAME";
            //
            // txtName
            //
            this.txtName.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.txtName.BorderRadius = 8;
            this.txtName.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtName.DefaultText = "";
            this.txtName.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(101)))), ((int)(((byte)(34)))));
            this.txtName.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.txtName.Location = new System.Drawing.Point(20, 35);
            this.txtName.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.txtName.Name = "txtName";
            this.txtName.PlaceholderText = "";
            this.txtName.SelectedText = "";
            this.txtName.Size = new System.Drawing.Size(260, 38);
            this.txtName.TabIndex = 1;
            //
            // lblPhone
            //
            this.lblPhone.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.lblPhone.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.lblPhone.Location = new System.Drawing.Point(300, 15);
            this.lblPhone.Name = "lblPhone";
            this.lblPhone.Size = new System.Drawing.Size(260, 17);
            this.lblPhone.TabIndex = 2;
            this.lblPhone.Text = "PHONE";
            //
            // txtPhone
            //
            this.txtPhone.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.txtPhone.BorderRadius = 8;
            this.txtPhone.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtPhone.DefaultText = "";
            this.txtPhone.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(101)))), ((int)(((byte)(34)))));
            this.txtPhone.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtPhone.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.txtPhone.Location = new System.Drawing.Point(300, 35);
            this.txtPhone.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.txtPhone.Name = "txtPhone";
            this.txtPhone.PlaceholderText = "";
            this.txtPhone.SelectedText = "";
            this.txtPhone.Size = new System.Drawing.Size(205, 38);
            this.txtPhone.TabIndex = 3;
            //
            // lblAge
            //
            this.lblAge.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.lblAge.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.lblAge.Location = new System.Drawing.Point(530, 15);
            this.lblAge.Name = "lblAge";
            this.lblAge.Size = new System.Drawing.Size(260, 17);
            this.lblAge.TabIndex = 4;
            this.lblAge.Text = "AGE";
            //
            // numAge
            //
            this.numAge.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.numAge.Location = new System.Drawing.Point(530, 35);
            this.numAge.Minimum = new decimal(new int[] { 10, 0, 0, 0 });
            this.numAge.Name = "numAge";
            this.numAge.Size = new System.Drawing.Size(80, 30);
            this.numAge.TabIndex = 5;
            this.numAge.Value = new decimal(new int[] { 18, 0, 0, 0 });
            //
            // lblAddress
            //
            this.lblAddress.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.lblAddress.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.lblAddress.Location = new System.Drawing.Point(20, 90);
            this.lblAddress.Name = "lblAddress";
            this.lblAddress.Size = new System.Drawing.Size(260, 17);
            this.lblAddress.TabIndex = 6;
            this.lblAddress.Text = "ADDRESS";
            //
            // txtAddress
            //
            this.txtAddress.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.txtAddress.BorderRadius = 8;
            this.txtAddress.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtAddress.DefaultText = "";
            this.txtAddress.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(101)))), ((int)(((byte)(34)))));
            this.txtAddress.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtAddress.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.txtAddress.Location = new System.Drawing.Point(20, 110);
            this.txtAddress.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.PlaceholderText = "";
            this.txtAddress.SelectedText = "";
            this.txtAddress.Size = new System.Drawing.Size(455, 38);
            this.txtAddress.TabIndex = 7;
            //
            // lblPlan
            //
            this.lblPlan.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.lblPlan.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.lblPlan.Location = new System.Drawing.Point(500, 90);
            this.lblPlan.Name = "lblPlan";
            this.lblPlan.Size = new System.Drawing.Size(260, 17);
            this.lblPlan.TabIndex = 8;
            this.lblPlan.Text = "PLAN";
            //
            // cmbPlan
            //
            this.cmbPlan.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPlan.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cmbPlan.Location = new System.Drawing.Point(500, 110);
            this.cmbPlan.Name = "cmbPlan";
            this.cmbPlan.Size = new System.Drawing.Size(280, 31);
            this.cmbPlan.TabIndex = 9;
            //
            // btnAdd
            //
            this.btnAdd.BorderRadius = 8;
            this.btnAdd.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(167)))), ((int)(((byte)(69)))));
            this.btnAdd.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnAdd.ForeColor = System.Drawing.Color.White;
            this.btnAdd.Location = new System.Drawing.Point(20, 165);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(148, 40);
            this.btnAdd.TabIndex = 10;
            this.btnAdd.Text = "  Add Member";
            this.btnAdd.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            //
            // btnUpdate
            //
            this.btnUpdate.BorderRadius = 8;
            this.btnUpdate.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(152)))), ((int)(((byte)(0)))));
            this.btnUpdate.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnUpdate.ForeColor = System.Drawing.Color.White;
            this.btnUpdate.Location = new System.Drawing.Point(180, 165);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(148, 40);
            this.btnUpdate.TabIndex = 11;
            this.btnUpdate.Text = "  Update";
            this.btnUpdate.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            //
            // btnDelete
            //
            this.btnDelete.BorderRadius = 8;
            this.btnDelete.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(53)))), ((int)(((byte)(69)))));
            this.btnDelete.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnDelete.ForeColor = System.Drawing.Color.White;
            this.btnDelete.Location = new System.Drawing.Point(340, 165);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(148, 40);
            this.btnDelete.TabIndex = 12;
            this.btnDelete.Text = "  Delete";
            this.btnDelete.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            //
            // btnClear
            //
            this.btnClear.BorderRadius = 8;
            this.btnClear.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.btnClear.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnClear.ForeColor = System.Drawing.Color.White;
            this.btnClear.Location = new System.Drawing.Point(500, 165);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(148, 40);
            this.btnClear.TabIndex = 13;
            this.btnClear.Text = "  Clear Fields";
            this.btnClear.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            //
            // lblMemberQRLabel
            //
            this.lblMemberQRLabel.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.lblMemberQRLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.lblMemberQRLabel.Location = new System.Drawing.Point(810, 12);
            this.lblMemberQRLabel.Name = "lblMemberQRLabel";
            this.lblMemberQRLabel.Size = new System.Drawing.Size(240, 17);
            this.lblMemberQRLabel.TabIndex = 14;
            this.lblMemberQRLabel.Text = "MEMBER QR CODE";
            this.lblMemberQRLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // picMemberQR
            //
            this.picMemberQR.BackColor = System.Drawing.Color.White;
            this.picMemberQR.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picMemberQR.Location = new System.Drawing.Point(855, 32);
            this.picMemberQR.Name = "picMemberQR";
            this.picMemberQR.Size = new System.Drawing.Size(130, 130);
            this.picMemberQR.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picMemberQR.TabIndex = 15;
            this.picMemberQR.TabStop = false;
            //
            // btnPrintCard
            //
            this.btnPrintCard.BorderRadius = 8;
            this.btnPrintCard.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(152)))), ((int)(((byte)(0)))));
            this.btnPrintCard.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnPrintCard.ForeColor = System.Drawing.Color.White;
            this.btnPrintCard.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(130)))), ((int)(((byte)(0)))));
            this.btnPrintCard.Location = new System.Drawing.Point(830, 170);
            this.btnPrintCard.Name = "btnPrintCard";
            this.btnPrintCard.Size = new System.Drawing.Size(200, 38);
            this.btnPrintCard.TabIndex = 16;
            this.btnPrintCard.Text = "Print Member Card";
            this.btnPrintCard.Click += new System.EventHandler(this.btnPrintCard_Click);
            //
            // ManageMembers
            //
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.ClientSize = new System.Drawing.Size(1100, 682);
            this.Controls.Add(this.panelTopBar);
            this.Controls.Add(this.panelSearch);
            this.Controls.Add(this.dgvMembers);
            this.Controls.Add(this.panelInputs);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.Name = "ManageMembers";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Manage Members";
            this.Load += new System.EventHandler(this.ManageMembers_Load);
            this.panelTopBar.ResumeLayout(false);
            this.panelSearch.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvMembers)).EndInit();
            this.panelInputs.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numAge)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picMemberQR)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private Guna.UI2.WinForms.Guna2Panel      panelTopBar;
        private System.Windows.Forms.Label         lblTitle;
        private Guna.UI2.WinForms.Guna2Button      btnClose;
        private Guna.UI2.WinForms.Guna2Panel       panelSearch;
        private Guna.UI2.WinForms.Guna2TextBox     txtSearch;
        private Guna.UI2.WinForms.Guna2ComboBox    cmbFilter;
        private Guna.UI2.WinForms.Guna2Button      btnRefresh;
        private Guna.UI2.WinForms.Guna2Button      btnExportMembers;
        private System.Windows.Forms.Label         lblResultCount;
        private System.Windows.Forms.DataGridView  dgvMembers;
        private Guna.UI2.WinForms.Guna2Panel       panelInputs;
        private System.Windows.Forms.Label         lblName;
        private System.Windows.Forms.Label         lblPhone;
        private System.Windows.Forms.Label         lblAge;
        private System.Windows.Forms.Label         lblAddress;
        private System.Windows.Forms.Label         lblPlan;
        private Guna.UI2.WinForms.Guna2TextBox     txtName;
        private Guna.UI2.WinForms.Guna2TextBox     txtPhone;
        private System.Windows.Forms.NumericUpDown numAge;
        private Guna.UI2.WinForms.Guna2TextBox     txtAddress;
        private System.Windows.Forms.ComboBox      cmbPlan;
        private Guna.UI2.WinForms.Guna2Button      btnAdd;
        private Guna.UI2.WinForms.Guna2Button      btnUpdate;
        private Guna.UI2.WinForms.Guna2Button      btnDelete;
        private Guna.UI2.WinForms.Guna2Button      btnClear;
        private System.Windows.Forms.Label         lblMemberQRLabel;
        private System.Windows.Forms.PictureBox    picMemberQR;
        private Guna.UI2.WinForms.Guna2Button      btnPrintCard;
    }
}
