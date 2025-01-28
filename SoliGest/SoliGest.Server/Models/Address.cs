using System.ComponentModel.DataAnnotations;

namespace SoliGest.Server.Models
{
    public class Address
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "É obrigatório a morada possuir pelo menos uma linha.")]
        [Display(Name = "Linha 1")]
        public string Line1 { get; set; }

        [Display(Name = "Linha 2")]
        public string Line2 { get; set; }

        [Required(ErrorMessage = "É obrigatório indicar um código postal.")]
        [Display(Name = "Código Postal")]
        public string PostalCode { get; set; }

        [Required(ErrorMessage = "É obrigatório indicar uma localidade.")]
        [Display(Name = "Localidade")]
        public string Locality { get; set; }

        [Required(ErrorMessage = "É obrigatório indicar uma cidade.")]
        [Display(Name = "Localidade")]
        public string City { get; set; }

        [Required(ErrorMessage = "É obrigatório indicar um país.")]
        [Display(Name = "País")]
        public string Country { get; set; }
    }
}
