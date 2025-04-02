using System.ComponentModel.DataAnnotations;

namespace SoliGest.Server.Models
{
    public class SolarPanel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }
        public string StatusClass { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Description { get; set; }

        [Required(ErrorMessage = "É obrigatório indicar um número de telemóvel.")]
        [Display(Name = "Telemóvel")]
        public int PhoneNumber { get; set; }

        [Required(ErrorMessage = "É obrigatório indicar um email.")]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "É obrigatório indicar uma morada.")]
        [Display(Name = "Morada")]
        public string Address { get; set; }
    }
}
