namespace GYM_Desktop_app.Models
{
    // A session package that belongs to a specific coach
    // e.g. Coach Bilal -> "Plan 1" 8 sessions 1000 EGP valid 2 weeks
    public class MembershipPlan
    {
        public int PlanID { get; set; }
        public int CoachID { get; set; }
        public string PlanName { get; set; }
        public int Sessions { get; set; }
        public decimal Price { get; set; }

        // How long this plan stays valid - set per plan by the manager.
        public int DurationValue { get; set; } = 1;
        public string DurationUnit { get; set; } = PlanDuration.Month;

        // Display-only
        public string CoachName { get; set; }

        public PlanDuration Duration => new PlanDuration(DurationValue, DurationUnit);

        // Shown as its own column in the Manage Coach Plans grid.
        public string DurationLabel => Duration.Label;

        // Combined label for dropdowns:
        // "Coach Bilal — Plan 1 (8 sessions, 2 Weeks, 1000 EGP)"
        public string Label =>
            $"{(string.IsNullOrEmpty(CoachName) ? "" : CoachName + " — ")}{PlanName}" +
            $"  ({Sessions} sessions, {Duration.Label}, {Price:0} EGP)";

        public override string ToString() => Label;
    }
}
