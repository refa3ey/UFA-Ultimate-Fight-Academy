using System;

namespace GYM_Desktop_app.Models
{
    // How long a plan stays valid once a member buys or renews it.
    //
    // Stored as a number + a unit rather than a plain day count so that
    // "1 Month" means a real calendar month (Jan 31 -> Feb 28), not 30 days.
    public class PlanDuration
    {
        public const string Week  = "Week";
        public const string Month = "Month";
        public const string Year  = "Year";

        // The units offered next to the number in the Manage Coach Plans screen.
        public static readonly string[] Units = { Week, Month, Year };

        public int Value { get; }
        public string Unit { get; }

        public PlanDuration(int value, string unit)
        {
            Value = value < 1 ? 1 : value;
            Unit  = string.Equals(unit, Week, StringComparison.OrdinalIgnoreCase) ? Week
                  : string.Equals(unit, Year, StringComparison.OrdinalIgnoreCase) ? Year
                  : Month;
        }

        // "1 Week", "2 Weeks", "1 Month" ...
        public string Label => Value == 1 ? $"1 {Unit}" : $"{Value} {Unit}s";

        // Short form for tight columns: "1w", "2mo", "1y"
        public string ShortLabel => Unit == Week ? $"{Value}w" : Unit == Year ? $"{Value}y" : $"{Value}mo";

        // End of the window that opens on `start`. The member may still train ON
        // the expiry day itself - the plan goes stale the day after.
        public DateTime AddTo(DateTime start) =>
            Unit == Week ? start.Date.AddDays(7 * Value)
          : Unit == Year ? start.Date.AddYears(Value)
                         : start.Date.AddMonths(Value);

        // Used for plans saved before durations existed, and as the form default.
        public static PlanDuration Default => new PlanDuration(1, Month);

        public static PlanDuration From(int value, string unit) => new PlanDuration(value, unit);

        public override string ToString() => Label;

        public override bool Equals(object obj) =>
            obj is PlanDuration d && d.Value == Value && d.Unit == Unit;

        public override int GetHashCode() => Value * 31 + Unit.GetHashCode();
    }
}
