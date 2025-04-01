namespace SoliGest.Server.Models
{
    public class Holidays
    {
        public int Id { get; set; }
        public DateOnly HolidayStart { get; set; }
        public DateOnly HolidayEnd { get; set; }

        public string UserId { get; set; }
    }
}
