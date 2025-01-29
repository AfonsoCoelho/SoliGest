using System.ComponentModel.DataAnnotations;

namespace SoliGest.Server.Models
{
    public class AssistanceRequest
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "É obrigatório indicar uma hora de pedido.")]
        [Display(Name = "Hora de pedido")]
        public DateTime RequestDate { get; set; }

        [Required(ErrorMessage = "É obrigatório indicar uma hora de resolução.")]
        [Display(Name = "Hora de resolução")]
        public DateTime ResolutionDate { get; set; }

        [Required(ErrorMessage = "É obrigatório indicar um painel solar.")]
        [Display(Name = "Painel Solar")]
        public SolarPanel SolarPanel { get; set; }
    }
}
