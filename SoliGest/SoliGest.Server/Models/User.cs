using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace SoliGest.Server.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome do utilizador é obrigatório.")]
        [Display(Name = "Nome de utilizador")]
        public string Username { get; set; }

        [Required(ErrorMessage = "A palavra-passe é obrigatório.")]
        [Display(Name = "Palavra-passe")]
        public string Password { get; set; }

        [Required(ErrorMessage = "A data de nascimento é obrigatória.")]
        [Display(Name = "Data de nascimento")]
        public DateOnly BirthDate { get; set; }

        [Required(ErrorMessage = "O email é obrigatório.")]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "O número de telemóvel é obrigatório.")]
        [Display(Name = "Telemóvel")]
        public int PhoneNumber { get; set; }

        [Required(ErrorMessage = "A morada é obrigatória.")]
        [Display(Name = "Morada")]
        public Address Address { get; set; }
    }
}
