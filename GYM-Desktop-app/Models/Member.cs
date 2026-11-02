using System;
using System.ComponentModel;
namespace GYM_Desktop_app.Models
{
    public class Member
    {
        public int MemberID { get; set; }
        public int UserID { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public int Age { get; set; }
        public string Address { get; set; }
        public DateTime JoinDate { get; set; }

        // Which of the member's plans this object shows (0 = they have no plan).
        [Browsable(false)]
        public int MemberPlanID { get; set; }

        // Coach + session-plan the member signed up for
        public int CoachID { get; set; }
        public int PlanID { get; set; }
        public int SessionsTotal { get; set; }
        public int SessionsRemaining { get; set; }

        // The 1-month window the current plan is valid for.
        public DateTime PlanStart { get; set; }
        public DateTime PlanExpiry { get; set; }

        // Display-only (filled from joins)
        public string CoachName { get; set; }
        public string PlanName { get; set; }

        // ===== PLAN WINDOW (computed - never stored) =====

        // The month is over. True even if sessions are still left over:
        // an unused session dies with the month, the member must renew.
        public bool IsExpired =>
            PlanExpiry != default(DateTime) && DateTime.Now.Date > PlanExpiry.Date;

        // Days left in the month (0 on the last day, negative once expired).
        public int DaysLeft =>
            PlanExpiry == default(DateTime)
                ? 0
                : (int)(PlanExpiry.Date - DateTime.Now.Date).TotalDays;

        // Can this member train right now? Needs BOTH time and sessions.
        public bool CanTrain => !IsExpired && SessionsRemaining > 0;

        // How many days before expiry to start warning. Scaled to the plan's own
        // length (a quarter of it, 2-7 days) so a 1-week plan isn't flagged
        // "expiring soon" the moment it is sold, the way a flat 7 days would.
        public int ExpiryWarningDays
        {
            get
            {
                if (PlanStart == default(DateTime) || PlanExpiry == default(DateTime)) return 7;
                int total = (int)(PlanExpiry.Date - PlanStart.Date).TotalDays;
                if (total <= 0) return 2;
                return Math.Max(2, Math.Min(7, (int)Math.Round(total * 0.25)));
            }
        }

        public bool IsExpiringSoon => PlanID > 0 && !IsExpired && DaysLeft <= ExpiryWarningDays;

        public string Status =>
              PlanID == 0             ? "Walk-in"
            : IsExpired               ? "Expired"
            : SessionsRemaining <= 0  ? "No Sessions"
            : IsExpiringSoon          ? "Expiring Soon"
            : SessionsRemaining <= 2  ? "Low Sessions"
            :                           "Active";
    }
}
