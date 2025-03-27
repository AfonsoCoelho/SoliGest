using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace SoliGest.Server.Models
{
    public class User : IdentityUser
    {
        [Required(ErrorMessage = "O nome é obrigatório.")]
        [Display(Name = "Nome")]
        public string Name { get; set; }

        [Required(ErrorMessage = "A data de nascimento é obrigatória.")]
        [Display(Name = "Data de nascimento")]
        public string BirthDate { get; set; }

        public string Address1 { get; set; }
        public string Address2 { get; set; }

        public List<DayOff> MonthlyDaysOff { get; set; } = new List<DayOff>();
        public List<Holidays> YearHolidays { get; set; } = new List<Holidays>();
        
        [Display(Name = "Dia de folga.")]
        public WeekDaysEnum DayOff { get; set; }
    }

}
