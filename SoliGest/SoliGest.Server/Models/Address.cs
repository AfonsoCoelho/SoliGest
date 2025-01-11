using System.ComponentModel.DataAnnotations;

namespace SoliGest.Server.Models
{
    public class Address
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome do projeto é obrigatório.")]
        [Display(Name = "Nome do Projeto")]
        public string Line1 { get; set; }

        [Display(Name = "Descrição")]
        public string Line2 { get; set; }

        [Required(ErrorMessage = "A localização é obrigatória.")]
        [Display(Name = "Localização")]
        public string PostalCode { get; set; }

        [Required(ErrorMessage = "A capacidade é obrigatória.")]
        [Range(1, int.MaxValue, ErrorMessage = "A capacidade deve ser um número positivo.")]
        [Display(Name = "Capacidade")]
        public string Locality { get; set; }

        // Propriedade de navegação para os voluntários associados ao projeto
        [Display(Name = "Voluntários")]
        public string City { get; set; }

        public string Country { get; set; }
    }
}
