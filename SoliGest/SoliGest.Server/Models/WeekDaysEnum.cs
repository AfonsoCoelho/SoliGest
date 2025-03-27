using System.ComponentModel;

namespace SoliGest.Server.Models
{
    public enum WeekDaysEnum
    {
        [Description("Monday")]
        Monday,
        Tuesday,
        Wednesday,
        Thursday,
        Friday,
        [Description("Saturday")]
        Saturday,
        Sunday
    }
}
