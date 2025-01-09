using System.ComponentModel.DataAnnotations;

namespace SoliGest.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome do projeto é obrigatório.")]
        [Display(Name = "Nome do Projeto")]
        public string Username { get; set; }

        [Display(Name = "Descrição")]
        public string Password { get; set; }

        [Required(ErrorMessage = "A localização é obrigatória.")]
        [Display(Name = "Localização")]
        public DateOnly BirthDate { get; set; }

        [Required(ErrorMessage = "A capacidade é obrigatória.")]
        [Range(1, int.MaxValue, ErrorMessage = "A capacidade deve ser um número positivo.")]
        [Display(Name = "Capacidade")]
        public string Email { get; set; }

        // Propriedade de navegação para os voluntários associados ao projeto
        [Display(Name = "Voluntários")]
        public int PhoneNumber { get; set; }

        public Address Address { get; set; }
    }
}
