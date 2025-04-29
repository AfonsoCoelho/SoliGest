using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace SoliGest.Server.Models
{
    public class User : IdentityUser
    {
        public string? ProfilePictureUrl { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        [Display(Name = "Nome")]
        public string Name { get; set; }

        [Required(ErrorMessage = "A data de nascimento é obrigatória.")]
        [Display(Name = "Data de nascimento")]
        public string BirthDate { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Role { get; set; }

        [Display(Name = "Dia de folga")]
        public string DayOff { get; set; }
        public string StartHoliday { get; set; }
        public string EndHoliday { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public bool? isActive { get; set; } = false;

    }
}
