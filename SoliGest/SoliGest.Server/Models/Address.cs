using System.ComponentModel.DataAnnotations;

namespace SoliGest.Server.Models
{
    /// <summary>
    /// Representa um endereço, incluindo detalhes como linha de endereço, código postal, localidade, cidade e país.
    /// </summary>
    public class Address
    {
        /// <summary>
        /// Identificador único do endereço.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Linha 1 do endereço, que é obrigatória.
        /// </summary>
        /// <remarks>Este campo deve conter pelo menos uma linha de endereço.</remarks>
        [Required(ErrorMessage = "É obrigatório a morada possuir pelo menos uma linha.")]
        [Display(Name = "Linha 1")]
        public string Line1 { get; set; }

        /// <summary>
        /// Linha 2 do endereço, que é opcional.
        /// </summary>
        [Display(Name = "Linha 2")]
        public string Line2 { get; set; }

        /// <summary>
        /// O código postal do endereço, que é obrigatório.
        /// </summary>
        /// <remarks>Este campo deve ser preenchido com o código postal da localização.</remarks>
        [Required(ErrorMessage = "É obrigatório indicar um código postal.")]
        [Display(Name = "Código Postal")]
        public string PostalCode { get; set; }

        /// <summary>
        /// A localidade do endereço, que é obrigatória.
        /// </summary>
        /// <remarks>Este campo deve indicar a localidade onde o endereço está situado.</remarks>
        [Required(ErrorMessage = "É obrigatório indicar uma localidade.")]
        [Display(Name = "Localidade")]
        public string Locality { get; set; }

        /// <summary>
        /// A cidade do endereço, que é obrigatória.
        /// </summary>
        /// <remarks>Este campo deve indicar a cidade onde o endereço está situado.</remarks>
        [Required(ErrorMessage = "É obrigatório indicar uma cidade.")]
        [Display(Name = "Cidade")]
        public string City { get; set; }

        /// <summary>
        /// O país do endereço, que é obrigatório.
        /// </summary>
        /// <remarks>Este campo deve indicar o país onde o endereço está situado.</remarks>
        [Required(ErrorMessage = "É obrigatório indicar um país.")]
        [Display(Name = "País")]
        public string Country { get; set; }
    }
}
