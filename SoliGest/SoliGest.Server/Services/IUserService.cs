using Microsoft.AspNetCore.Mvc;
using SoliGest.Server.Models;

namespace SoliGest.Server.Services
{
    /// <summary>
    /// Interface para o serviço de usuários.
    /// Define as operações relacionadas à manipulação e consulta de usuários no sistema.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Obtém um usuário com base no email.
        /// </summary>
        /// <param name="email">O email do usuário a ser consultado.</param>
        /// <returns>Uma tarefa assíncrona que retorna o usuário encontrado.</returns>
        Task<User> GetUserByEmail(string email);
    }
}
