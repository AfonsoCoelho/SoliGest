namespace SoliGest.Server.Models
{
    public class AssistanceRequest
    {
        public int Id { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime ResolutionDate { get; set; }
        public SolarPanel SolarPanel { get; set; }
    }
}
