using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using GYM_Desktop_app.Database;
using GYM_Desktop_app.Helpers;
using GYM_Desktop_app.Models;

namespace GYM_Desktop_app.Forms
{
    public partial class AdminDashboard : Form
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(int l, int t, int r, int b, int w, int h);

        private bool  _dragging;
        private Point _dragStart;
        private User  _currentUser;

        private Chart chartRevenue;
        private Chart chartGrowth;
        private Chart chartPlans;
        private Chart chartHours;
        private Label lblNoRevData;
        private Label lblNoGrowthData;
        private Label lblNoPlansData;
        private Label lblNoHoursData;

        // Colour palette for doughnut slices
        private static readonly Color[] SlicePalette =
        {
            Theme.Primary,
            Theme.Success,
            Theme.Warning,
            Color.FromArgb(52,  152, 219),
            Color.FromArgb(155, 89,  182),
            Color.FromArgb(231, 76,  60)
        };

        public AdminDashboard(User user)
        {
            InitializeComponent();
            _currentUser    = user;
            lblWelcome.Text = $"Welcome, {user.Username}!";
        }

        private void AdminDashboard_Load(object sender, EventArgs e)
        {
            Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));
            ApplyGridStyle(dgvRecentMembers);
            InitCharts();
            RefreshAll();
            timerRefresh.Start();
        }

        // ── Chart initialisation ──────────────────────────────────────────────

        private void InitCharts()
        {
            chartRevenue = BuildBaseChart();
            chartGrowth  = BuildBaseChart();
            chartPlans   = BuildBaseChart();
            chartHours   = BuildBaseChart();

            ConfigureRevenueChart();
            ConfigureGrowthChart();
            ConfigurePlansChart();
            ConfigureHoursChart();

            AddChartToPanel(panelChartRevCard,    chartRevenue, "Revenue (Last 12 Months)",  out lblNoRevData);
            AddChartToPanel(panelChartGrowthCard, chartGrowth,  "Member Growth",              out lblNoGrowthData);
            AddChartToPanel(panelChartPlansCard,  chartPlans,   "Plan Distribution",          out lblNoPlansData);
            AddChartToPanel(panelChartHoursCard,  chartHours,   "Sessions (Last 7 Days)",     out lblNoHoursData);
        }

        private Chart BuildBaseChart()
        {
            var chart = new Chart();
            chart.BackColor          = Color.White;
            chart.BorderlineWidth    = 0;
            chart.BorderlineDashStyle = ChartDashStyle.NotSet;
            chart.BorderSkin.SkinStyle = BorderSkinStyle.None;

            var ca = new ChartArea("Main");
            ca.BackColor               = Color.White;
            ca.AxisX.MajorGrid.LineColor = Color.Transparent;
            ca.AxisY.MajorGrid.LineColor = Color.FromArgb(238, 238, 238);
            ca.AxisX.LineColor           = Color.FromArgb(210, 210, 210);
            ca.AxisY.LineColor           = Color.Transparent;
            ca.AxisX.LabelStyle.Font     = new Font("Segoe UI", 7.5f);
            ca.AxisY.LabelStyle.Font     = new Font("Segoe UI", 7.5f);
            ca.AxisX.LabelStyle.ForeColor = Color.FromArgb(130, 130, 130);
            ca.AxisY.LabelStyle.ForeColor = Color.FromArgb(130, 130, 130);
            ca.AxisX.MajorTickMark.Enabled = false;
            ca.AxisY.MajorTickMark.Enabled = false;
            ca.BorderWidth  = 0;
            ca.BorderColor  = Color.Transparent;
            ca.ShadowOffset = 0;
            chart.ChartAreas.Add(ca);

            return chart;
        }

        private void ConfigureRevenueChart()
        {
            var s = new Series("Revenue") { ChartType = SeriesChartType.Line };
            s.Color              = Theme.Primary;
            s.BorderWidth        = 3;
            s.MarkerStyle        = MarkerStyle.Circle;
            s.MarkerSize         = 7;
            s.MarkerColor        = Theme.Primary;
            s.MarkerBorderColor  = Color.White;
            s.MarkerBorderWidth  = 2;
            s.ToolTip            = "#VALX\n#VAL{N0} EGP";
            chartRevenue.Series.Add(s);
            chartRevenue.ChartAreas["Main"].AxisY.LabelStyle.Format = "{0:N0} EGP";
        }

        private void ConfigureGrowthChart()
        {
            var s = new Series("Growth") { ChartType = SeriesChartType.SplineArea };
            s.Color       = Color.FromArgb(55, Theme.Primary.R, Theme.Primary.G, Theme.Primary.B);
            s.BorderColor = Theme.Primary;
            s.BorderWidth = 3;
            s.ToolTip     = "#VALX\n#VAL members";
            chartGrowth.Series.Add(s);
            chartGrowth.ChartAreas["Main"].AxisY.Minimum = 0;
        }

        private void ConfigurePlansChart()
        {
            var s = new Series("Plans") { ChartType = SeriesChartType.Doughnut };
            s["DoughnutRadius"]    = "40";
            s["PieStartAngle"]     = "270";
            s.IsValueShownAsLabel  = true;
            s.LabelFormat          = "#";
            s.Font                 = new Font("Segoe UI", 8f, FontStyle.Bold);
            s.ToolTip              = "#VALX\n#VAL members";
            chartPlans.Series.Add(s);

            // Hide axes — not needed for doughnut
            var ca = chartPlans.ChartAreas["Main"];
            ca.AxisX.LabelStyle.ForeColor = Color.Transparent;
            ca.AxisY.LabelStyle.ForeColor = Color.Transparent;
            ca.AxisX.LineColor            = Color.Transparent;
            ca.AxisY.MajorGrid.LineColor  = Color.Transparent;
            ca.InnerPlotPosition          = new ElementPosition(5, 5, 60, 90);

            var legend = new Legend("Plans");
            legend.Font      = new Font("Segoe UI", 8f);
            legend.ForeColor = Color.FromArgb(80, 80, 80);
            legend.BackColor = Color.Transparent;
            legend.Docking   = Docking.Right;
            legend.Alignment = System.Drawing.StringAlignment.Center;
            chartPlans.Legends.Add(legend);
            chartPlans.Series["Plans"].Legend           = "Plans";
            chartPlans.Series["Plans"].IsVisibleInLegend = true;
        }

        private void ConfigureHoursChart()
        {
            var s = new Series("Hours") { ChartType = SeriesChartType.Column };
            s.Color           = Theme.Primary;
            s["PointWidth"]   = "0.6";
            s.ToolTip         = "#VALX\n#VAL sessions";
            chartHours.Series.Add(s);
        }

        private void AddChartToPanel(Guna.UI2.WinForms.Guna2Panel panel, Chart chart,
                                      string title, out Label noDataLabel)
        {
            // Title label
            var lbl = new Label
            {
                Text      = title,
                Font      = new Font("Segoe UI", 11f, FontStyle.Bold),
                ForeColor = Theme.TextDark,
                BackColor = Color.Transparent,
                Location  = new Point(14, 10),
                Size      = new Size(panel.Width - 28, 24),
                AutoSize  = false
            };
            panel.Controls.Add(lbl);

            // Chart control
            int chartTop = 40;
            int chartH   = panel.Height - chartTop - 6;
            chart.Location = new Point(4, chartTop);
            chart.Size     = new Size(panel.Width - 8, chartH);
            chart.Anchor   = AnchorStyles.Top | AnchorStyles.Left |
                             AnchorStyles.Right | AnchorStyles.Bottom;
            panel.Controls.Add(chart);

            // "No data" overlay
            var noData = new Label
            {
                Text      = "No data yet",
                Font      = new Font("Segoe UI", 10f),
                ForeColor = Color.FromArgb(175, 175, 175),
                BackColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Location  = new Point(0, chartTop),
                Size      = new Size(panel.Width, chartH),
                Visible   = false
            };
            panel.Controls.Add(noData);
            noData.BringToFront();

            noDataLabel = noData;
        }

        // ── Chart data loading ────────────────────────────────────────────────

        private void LoadCharts()
        {
            LoadRevenueChart();
            LoadGrowthChart();
            LoadPlansChart();
            LoadHoursChart();
        }

        private void LoadRevenueChart()
        {
            try
            {
                var data = DatabaseHelper.GetMonthlyRevenue(12);
                var s    = chartRevenue.Series["Revenue"];
                s.Points.Clear();

                if (data.Count == 0)
                {
                    chartRevenue.Visible = false;
                    lblNoRevData.Visible = true;
                    return;
                }
                chartRevenue.Visible = true;
                lblNoRevData.Visible = false;

                foreach (var (month, total) in data)
                    s.Points.AddXY(month, (double)total);

                var ca = chartRevenue.ChartAreas["Main"];
                ca.AxisX.LabelStyle.Angle = -35;
                ca.AxisX.Interval         = 1;
            }
            catch { }
        }

        private void LoadGrowthChart()
        {
            try
            {
                var data = DatabaseHelper.GetMemberGrowth(12);
                var s    = chartGrowth.Series["Growth"];
                s.Points.Clear();

                bool hasData = data.Count > 0 && data.Any(d => d.Count > 0);
                if (!hasData)
                {
                    chartGrowth.Visible      = false;
                    lblNoGrowthData.Visible  = true;
                    return;
                }
                chartGrowth.Visible     = true;
                lblNoGrowthData.Visible = false;

                foreach (var (month, count) in data)
                    s.Points.AddXY(month, count);

                var ca = chartGrowth.ChartAreas["Main"];
                ca.AxisX.LabelStyle.Angle = -35;
                ca.AxisX.Interval         = 1;
            }
            catch { }
        }

        private void LoadPlansChart()
        {
            try
            {
                var data = DatabaseHelper.GetPlanDistribution();
                var s    = chartPlans.Series["Plans"];
                s.Points.Clear();

                if (data.Count == 0)
                {
                    chartPlans.Visible      = false;
                    lblNoPlansData.Visible  = true;
                    return;
                }
                chartPlans.Visible     = true;
                lblNoPlansData.Visible = false;

                for (int i = 0; i < data.Count; i++)
                {
                    var dp = new DataPoint();
                    dp.SetValueXY(data[i].PlanName, data[i].Count);
                    dp.Color       = SlicePalette[i % SlicePalette.Length];
                    dp.LegendText  = $"{data[i].PlanName} ({data[i].Count})";
                    s.Points.Add(dp);
                }
            }
            catch { }
        }

        private void LoadHoursChart()
        {
            try
            {
                var data = DatabaseHelper.GetSessionsPerDay(7);
                var s    = chartHours.Series["Hours"];
                s.Points.Clear();

                bool hasData = data.Any(d => d.Count > 0);
                if (!hasData)
                {
                    chartHours.Visible      = false;
                    lblNoHoursData.Visible  = true;
                    return;
                }
                chartHours.Visible     = true;
                lblNoHoursData.Visible = false;

                int maxVal = data.Max(d => d.Count);
                foreach (var (day, count) in data)
                {
                    var dp = new DataPoint();
                    dp.SetValueXY(day, count);
                    dp.Color = (maxVal > 0 && count == maxVal) ? Theme.Warning : Theme.Primary;
                    s.Points.Add(dp);
                }
            }
            catch { }
        }

        // ── Stats cards ───────────────────────────────────────────────────────

        private void LoadStats()
        {
            try
            {
                var st = DatabaseHelper.GetDashboardStats();
                lblCardMembersVal.Text = st.totalMembers.ToString();
                lblCardActiveVal.Text  = st.activeMembers.ToString();
                lblCardRevenueVal.Text = st.monthRevenue.ToString("N0") + " EGP";
                lblCardNewVal.Text     = st.weekSessions.ToString();
            }
            catch { }
        }

        // ── Recent members table ──────────────────────────────────────────────

        private void LoadRecentMembers()
        {
            try
            {
                var members = DatabaseHelper.GetAllMembers()
                    .OrderByDescending(m => m.JoinDate)
                    .Take(10)
                    .ToList();
                dgvRecentMembers.DataSource = null;
                dgvRecentMembers.DataSource = members;
            }
            catch { }
        }

        // ── Refresh ───────────────────────────────────────────────────────────

        private void RefreshAll()
        {
            LoadStats();
            LoadCharts();
            LoadRecentMembers();
        }

        private void timerRefresh_Tick(object sender, EventArgs e) => RefreshAll();

        private void btnRefresh_Click(object sender, EventArgs e) => RefreshAll();

        // ── Grid styling ──────────────────────────────────────────────────────

        private void ApplyGridStyle(DataGridView dgv)
        {
            dgv.BackgroundColor                           = Color.White;
            dgv.BorderStyle                               = BorderStyle.None;
            dgv.GridColor                                 = Color.FromArgb(230, 230, 230);
            dgv.RowHeadersVisible                         = false;
            dgv.AllowUserToAddRows                        = false;
            dgv.SelectionMode                             = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect                               = false;
            dgv.ReadOnly                                  = true;
            dgv.EnableHeadersVisualStyles                 = false;
            dgv.RowTemplate.Height                        = 38;
            dgv.DefaultCellStyle.Font                     = new Font("Segoe UI", 9.5f);
            dgv.DefaultCellStyle.SelectionBackColor       = Theme.Primary;
            dgv.DefaultCellStyle.SelectionForeColor       = Color.White;
            dgv.DefaultCellStyle.Padding                  = new Padding(5, 0, 5, 0);
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 247, 250);
            dgv.ColumnHeadersHeight                       = 40;
            dgv.ColumnHeadersDefaultCellStyle.BackColor   = Theme.Primary;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor   = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font        = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.Padding     = new Padding(5, 0, 5, 0);
        }

        // ── Drag ─────────────────────────────────────────────────────────────

        private void DragPanel_MouseDown(object sender, MouseEventArgs e)
        {
            _dragging  = true;
            _dragStart = e.Location;
        }

        private void DragPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (_dragging)
                Location = new Point(Location.X + e.X - _dragStart.X,
                                     Location.Y + e.Y - _dragStart.Y);
        }

        private void DragPanel_MouseUp(object sender, MouseEventArgs e) => _dragging = false;

        // ── Navigation buttons ────────────────────────────────────────────────

        private void btnManageMembers_Click(object sender, EventArgs e)
            => new ManageMembers().ShowDialog();

        private void btnManagePlans_Click(object sender, EventArgs e)
            => new ManagePlans().ShowDialog();

        private void btnManageTrainers_Click(object sender, EventArgs e)
            => new ManageTrainers().ShowDialog();

        private void btnPayments_Click(object sender, EventArgs e)
            => new PaymentForm().ShowDialog();

        private void btnReports_Click(object sender, EventArgs e)
            => new ReportsForm().ShowDialog();

        private void btnChangePassword_Click(object sender, EventArgs e)
            => new ChangePasswordForm(_currentUser).ShowDialog();

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Hide();
            var login = new LoginForm();
            login.FormClosed += (s, args) => this.Close();
            login.Show();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            this.Hide();
            var login = new LoginForm();
            login.FormClosed += (s, args) => this.Close();
            login.Show();
        }
    }
}
