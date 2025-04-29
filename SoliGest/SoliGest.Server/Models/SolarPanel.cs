using System.ComponentModel.DataAnnotations;

namespace SoliGest.Server.Models
{
    /// <summary>
    /// Representa um painel solar no sistema.
    /// </summary>
    public class SolarPanel
    {
        /// <summary>
        /// Identificador único do painel solar.
        /// </summary>
        /// <remarks>Este campo é utilizado para identificar de forma única o painel solar no sistema.</remarks>
        public int Id { get; set; }

        /// <summary>
        /// Nome do painel solar.
        /// </summary>
        /// <remarks>Este campo contém o nome ou a identificação do painel solar, que pode ser usado para facilitar a busca e referência.</remarks>
        public string Name { get; set; }

        /// <summary>
        /// Prioridade associada ao painel solar.
        /// </summary>
        /// <remarks>Este campo define a prioridade do painel solar, o que pode ser usado para indicar a urgência ou importância em algum processo.</remarks>
        public string Priority { get; set; }

        /// <summary>
        /// Status atual do painel solar.
        /// </summary>
        /// <remarks>Este campo contém o status atual do painel solar, como "Ativo", "Inativo", "Em manutenção", etc.</remarks>
        public string Status { get; set; }

        /// <summary>
        /// Classe de estilo associada ao status do painel solar.
        /// </summary>
        /// <remarks>Este campo é utilizado para associar uma classe CSS para representar visualmente o status do painel solar.</remarks>
        public string StatusClass { get; set; }

        /// <summary>
        /// Latitude de localização do painel solar.
        /// </summary>
        /// <remarks>Este campo contém a latitude geográfica do painel solar para fins de localização em mapas.</remarks>
        public double Latitude { get; set; }

        /// <summary>
        /// Longitude de localização do painel solar.
        /// </summary>
        /// <remarks>Este campo contém a longitude geográfica do painel solar para fins de localização em mapas.</remarks>
        public double Longitude { get; set; }

        /// <summary>
        /// Descrição do painel solar.
        /// </summary>
        /// <remarks>Este campo pode conter uma descrição detalhada do painel solar, como características técnicas ou informações adicionais.</remarks>
        public string Description { get; set; }

        /// <summary>
        /// Número de telemóvel associado ao painel solar.
        /// </summary>
        /// <remarks>Este campo contém o número de telefone associado ao painel solar para contatos de emergência ou manutenção.</remarks>
        [Required(ErrorMessage = "É obrigatório indicar um número de telemóvel.")]
        [Display(Name = "Telemóvel")]
        public int PhoneNumber { get; set; }

        /// <summary>
        /// Endereço de email associado ao painel solar.
        /// </summary>
        /// <remarks>Este campo contém o endereço de e-mail para comunicação relacionada ao painel solar.</remarks>
        [Required(ErrorMessage = "É obrigatório indicar um email.")]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        /// Endereço físico do painel solar.
        /// </summary>
        /// <remarks>Este campo contém o endereço físico onde o painel solar está localizado.</remarks>
        [Required(ErrorMessage = "É obrigatório indicar uma morada.")]
        [Display(Name = "Morada")]
        public string Address { get; set; }
    }
}
