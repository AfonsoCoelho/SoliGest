namespace SoliGest.Server.Models
{
    /// <summary>
    /// Representa um contato no sistema, com informações básicas como o identificador e o nome.
    /// </summary>
    public class Contact
    {
        /// <summary>
        /// Identificador único do contato.
        /// </summary>
        /// <remarks>Este campo é utilizado para identificar o contato de forma única no sistema.</remarks>
        public string Id { get; set; }

        /// <summary>
        /// Nome do contato.
        /// </summary>
        /// <remarks>Este campo armazena o nome completo ou identificador do contato.</remarks>
        public string Name { get; set; }
    }
}
