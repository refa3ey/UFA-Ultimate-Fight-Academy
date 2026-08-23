namespace GYM_Desktop_app.Models
{
    // A session package that belongs to a specific coach
    // e.g. Coach Bilal -> "Plan 1" 8 sessions 1000 EGP
    public class MembershipPlan
    {
        public int PlanID { get; set; }
        public int CoachID { get; set; }
        public string PlanName { get; set; }
        public int Sessions { get; set; }
        public decimal Price { get; set; }

        // Display-only
        public string CoachName { get; set; }

        // Combined label for dropdowns: "Coach Bilal — Plan 1 (8 sessions, 1000 EGP)"
        public string Label =>
            $"{(string.IsNullOrEmpty(CoachName) ? "" : CoachName + " — ")}{PlanName}  ({Sessions} sessions, {Price:0} EGP)";

        public override string ToString() => Label;
    }
}
