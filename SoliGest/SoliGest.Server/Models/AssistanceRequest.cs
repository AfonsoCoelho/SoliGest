using System.ComponentModel.DataAnnotations;

namespace SoliGest.Server.Models
{
    /// <summary>
    /// Representa um pedido de assistência técnica relacionado a um painel solar, incluindo informações sobre a data do pedido, a prioridade, o status e a descrição do problema.
    /// </summary>
    public class AssistanceRequest
    {
        /// <summary>
        /// Identificador único do pedido de assistência.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// A data e hora do pedido de assistência, que é obrigatória.
        /// </summary>
        /// <remarks>Este campo deve indicar quando o pedido foi feito.</remarks>
        [Required(ErrorMessage = "É obrigatório indicar uma hora de pedido.")]
        [Display(Name = "Hora de pedido")]
        public string RequestDate { get; set; }

        /// <summary>
        /// A prioridade do pedido de assistência.
        /// </summary>
        /// <remarks>Este campo pode ser preenchido com valores como "Alta", "Média", "Baixa".</remarks>
        public string Priority { get; set; }

        /// <summary>
        /// O status atual do pedido de assistência.
        /// </summary>
        /// <remarks>Exemplos de valores: "Em andamento", "Concluído", "Pendente".</remarks>
        public string Status { get; set; }

        /// <summary>
        /// A classe CSS associada ao status para formatação visual.
        /// </summary>
        /// <remarks>Este campo é utilizado para definir o estilo do status na interface.</remarks>
        public string StatusClass { get; set; }

        /// <summary>
        /// A data e hora de resolução do pedido de assistência.
        /// </summary>
        /// <remarks>Este campo é opcional e é preenchido quando o pedido de assistência é resolvido.</remarks>
        [Display(Name = "Hora de resolução")]
        public string ResolutionDate { get; set; }

        /// <summary>
        /// A descrição do problema ou avaria do painel solar.
        /// </summary>
        /// <remarks>Este campo é usado para fornecer detalhes sobre o problema que requer assistência.</remarks>
        [Display(Name = "Descrição da avaria")]
        public string Description { get; set; }

        /// <summary>
        /// O painel solar associado a este pedido de assistência, que é obrigatório.
        /// </summary>
        /// <remarks>Este campo é utilizado para vincular o pedido de assistência a um painel solar específico.</remarks>
        [Required(ErrorMessage = "É obrigatório indicar um painel solar.")]
        [Display(Name = "Painel Solar")]
        public required SolarPanel SolarPanel { get; set; }

        /// <summary>
        /// O usuário designado para resolver o pedido de assistência.
        /// </summary>
        /// <remarks>Este campo é opcional e pode estar vazio se o pedido não tiver um técnico atribuído.</remarks>
        public User? AssignedUser { get; set; }
    }
}
