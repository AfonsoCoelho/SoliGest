namespace SoliGest.Server.Models
{
    /// <summary>
    /// Representa um período de férias de um usuário no sistema.
    /// </summary>
    public class Holidays
    {
        /// <summary>
        /// Identificador único do registro de férias.
        /// </summary>
        /// <remarks>Este campo é utilizado para identificar o registro de férias de forma única no sistema.</remarks>
        public int Id { get; set; }

        /// <summary>
        /// Data de início das férias.
        /// </summary>
        /// <remarks>Este campo representa a data em que o usuário inicia suas férias.</remarks>
        public DateOnly HolidayStart { get; set; }

        /// <summary>
        /// Data de término das férias.
        /// </summary>
        /// <remarks>Este campo representa a data em que as férias do usuário terminam.</remarks>
        public DateOnly HolidayEnd { get; set; }

        /// <summary>
        /// Identificador do usuário associado às férias.
        /// </summary>
        /// <remarks>Este campo relaciona o período de férias a um usuário específico no sistema.</remarks>
        public string UserId { get; set; }
    }
}
