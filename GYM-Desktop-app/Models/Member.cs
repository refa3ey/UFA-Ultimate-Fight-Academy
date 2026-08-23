using System;
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

        // Coach + session-plan the member signed up for
        public int CoachID { get; set; }
        public int PlanID { get; set; }
        public int SessionsTotal { get; set; }
        public int SessionsRemaining { get; set; }

        // Display-only (filled from joins)
        public string CoachName { get; set; }
        public string PlanName { get; set; }
    }
}
