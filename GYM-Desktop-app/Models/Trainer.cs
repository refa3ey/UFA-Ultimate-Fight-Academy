namespace GYM_Desktop_app.Models
{
    // Represents a Coach (kept the type name Trainer to limit churn)
    public class Trainer
    {
        public int TrainerID { get; set; }
        public string Name { get; set; }
        public string Specialty { get; set; }
        public string Phone { get; set; }

        public override string ToString() => Name;
    }
}
