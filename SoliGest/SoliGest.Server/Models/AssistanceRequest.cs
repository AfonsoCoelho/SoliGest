using System.ComponentModel.DataAnnotations;

namespace SoliGest.Server.Models
{
    public class AssistanceRequest
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "É obrigatório indicar uma hora de pedido.")]
        [Display(Name = "Hora de pedido")]
        public string RequestDate { get; set; }

        [Display(Name = "Hora de resolução")]
        public string ResolutionDate { get; set; }

        [Display(Name = "Descrição da avaria")]
        public string Description { get; set; }

        [Required(ErrorMessage = "É obrigatório indicar um painel solar.")]
        [Display(Name = "Painel Solar")]
        public required SolarPanel SolarPanel { get; set; }
    }
}
