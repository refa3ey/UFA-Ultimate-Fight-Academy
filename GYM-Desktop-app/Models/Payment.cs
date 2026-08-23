using System;
namespace GYM_Desktop_app.Models
{
    public class Payment
    {
        public int PaymentID { get; set; }
        public int MemberID { get; set; }
        public int PlanID { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Method { get; set; }
    }
}