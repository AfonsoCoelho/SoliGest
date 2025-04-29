using System.ComponentModel;

namespace SoliGest.Server.Models
{
    /// <summary>
    /// Enumeração que representa os dias da semana.
    /// </summary>
    public enum WeekDaysEnum
    {
        /// <summary>
        /// Representa o dia da semana segunda-feira.
        /// </summary>
        [Description("Monday")]
        Monday,

        /// <summary>
        /// Representa o dia da semana terça-feira.
        /// </summary>
        Tuesday,

        /// <summary>
        /// Representa o dia da semana quarta-feira.
        /// </summary>
        Wednesday,

        /// <summary>
        /// Representa o dia da semana quinta-feira.
        /// </summary>
        Thursday,

        /// <summary>
        /// Representa o dia da semana sexta-feira.
        /// </summary>
        Friday,

        /// <summary>
        /// Representa o dia da semana sábado.
        /// </summary>
        [Description("Saturday")]
        Saturday,

        /// <summary>
        /// Representa o dia da semana domingo.
        /// </summary>
        Sunday
    }
}
