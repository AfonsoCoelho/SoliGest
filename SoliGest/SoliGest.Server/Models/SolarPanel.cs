using System.ComponentModel.DataAnnotations;

namespace SoliGest.Server.Models
{
    public class SolarPanel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "É obrigatório indicar um número de telemóvel.")]
        [Display(Name = "Telemóvel")]
        public int PhoneNumber { get; set; }

        [Required(ErrorMessage = "É obrigatório indicar um email.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "É obrigatório indicar uma morada.")]
        [Display(Name = "Morada")]
        public Address Address { get; set; }
    }
}
