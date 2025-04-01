namespace SoliGest.Server.Models
{
    public class DayOff
    {
        public int Id { get; set; }
        public DateOnly Day { get; set; }

        public string UserId { get; set; }
    }
}
