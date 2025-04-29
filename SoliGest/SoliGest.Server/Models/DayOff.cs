namespace SoliGest.Server.Models
{
    /// <summary>
    /// Representa um dia de folga de um usuário no sistema.
    /// </summary>
    public class DayOff
    {
        /// <summary>
        /// Identificador único do registro de folga.
        /// </summary>
        /// <remarks>Este campo é utilizado para identificar o registro de folga de forma única no sistema.</remarks>
        public int Id { get; set; }

        /// <summary>
        /// Data do dia de folga.
        /// </summary>
        /// <remarks>Este campo representa o dia específico em que o usuário está de folga.</remarks>
        public DateOnly Day { get; set; }

        /// <summary>
        /// Identificador do usuário associado à folga.
        /// </summary>
        /// <remarks>Este campo relaciona o dia de folga a um usuário específico no sistema.</remarks>
        public string UserId { get; set; }
    }
}
