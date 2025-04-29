using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace SoliGest.Server.Models
{
    /// <summary>
    /// Representa um utilizador no sistema.
    /// Esta classe herda de <see cref="IdentityUser"/> e contém informações adicionais específicas para o usuário.
    /// </summary>
    public class User : IdentityUser
    {
        /// <summary>
        /// URL da imagem de perfil do usuário.
        /// </summary>
        /// <remarks>Este campo armazena o caminho ou URL da imagem de perfil do usuário.</remarks>
        public string? ProfilePictureUrl { get; set; }

        /// <summary>
        /// Nome completo do usuário.
        /// </summary>
        /// <remarks>Este campo armazena o nome do usuário, sendo obrigatório para o cadastro.</remarks>
        [Required(ErrorMessage = "O nome é obrigatório.")]
        [Display(Name = "Nome")]
        public string Name { get; set; }

        /// <summary>
        /// Data de nascimento do usuário.
        /// </summary>
        /// <remarks>Este campo armazena a data de nascimento do usuário, sendo obrigatória para o cadastro.</remarks>
        [Required(ErrorMessage = "A data de nascimento é obrigatória.")]
        [Display(Name = "Data de nascimento")]
        public string BirthDate { get; set; }

        /// <summary>
        /// Endereço da linha 1 do usuário.
        /// </summary>
        /// <remarks>Este campo armazena o primeiro endereço do usuário.</remarks>
        public string Address1 { get; set; }

        /// <summary>
        /// Endereço da linha 2 do usuário.
        /// </summary>
        /// <remarks>Este campo armazena o segundo endereço do usuário, caso exista.</remarks>
        public string Address2 { get; set; }

        /// <summary>
        /// Cargo ou função do usuário no sistema.
        /// </summary>
        /// <remarks>Este campo define o papel ou função do usuário (e.g., "Supervisor", "Técnico").</remarks>
        public string Role { get; set; }

        /// <summary>
        /// Dia de folga do usuário.
        /// </summary>
        /// <remarks>Este campo especifica o dia da semana que o usuário tira folga.</remarks>
        [Display(Name = "Dia de folga")]
        public string DayOff { get; set; }

        /// <summary>
        /// Data de início das férias do usuário.
        /// </summary>
        /// <remarks>Este campo indica a data em que o usuário começa suas férias.</remarks>
        public string StartHoliday { get; set; }

        /// <summary>
        /// Data de fim das férias do usuário.
        /// </summary>
        /// <remarks>Este campo indica a data em que as férias do usuário terminam.</remarks>
        public string EndHoliday { get; set; }

        /// <summary>
        /// Coleção de conversas associadas ao usuário.
        /// </summary>
        /// <remarks>Este campo armazena todas as conversas em que o usuário está envolvido.</remarks>
        public ICollection<Conversation> Conversations { get; set; }

        /// <summary>
        /// Coleção de mensagens enviadas pelo usuário.
        /// </summary>
        /// <remarks>Este campo armazena todas as mensagens enviadas pelo usuário.</remarks>
        public ICollection<Message> MessagesSent { get; set; }

        /// <summary>
        /// Coleção de mensagens recebidas pelo usuário.
        /// </summary>
        /// <remarks>Este campo armazena todas as mensagens recebidas pelo usuário.</remarks>
        public ICollection<Message> MessagesReceived { get; set; }

        /// <summary>
        /// Latitude da localização do usuário.
        /// </summary>
        /// <remarks>Este campo contém a latitude geográfica do usuário, usada para localização.</remarks>
        public double? Latitude { get; set; }

        /// <summary>
        /// Longitude da localização do usuário.
        /// </summary>
        /// <remarks>Este campo contém a longitude geográfica do usuário, usada para localização.</remarks>
        public double? Longitude { get; set; }

        /// <summary>
        /// Indica se o usuário está ativo no sistema.
        /// </summary>
        /// <remarks>Este campo armazena um valor booleano que indica se o usuário está ativo no sistema.</remarks>
        public bool? isActive { get; set; } = false;
    }
}
