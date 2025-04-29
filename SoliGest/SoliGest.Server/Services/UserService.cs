using Microsoft.EntityFrameworkCore;
using SoliGest.Server.Data;
using SoliGest.Server.Models;

namespace SoliGest.Server.Services
{
    /// <summary>
    /// Serviço responsável pela gestão de usuários.
    /// Este serviço permite consultar um usuário pelo seu email.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly SoliGestServerContext _context;

        /// <summary>
        /// Construtor da classe UserService.
        /// Inicializa o contexto do banco de dados para acesso aos dados de usuários.
        /// </summary>
        /// <param name="context">O contexto do banco de dados.</param>
        public UserService(SoliGestServerContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtém um usuário pelo seu endereço de email.
        /// </summary>
        /// <param name="email">O email do usuário a ser consultado.</param>
        /// <returns>O usuário encontrado ou lança uma exceção caso o usuário não seja encontrado.</returns>
        /// <exception cref="Exception">Lança exceção se o usuário com o email fornecido não for encontrado.</exception>
        public async Task<User> GetUserByEmail(string email)
        {
            // Tenta encontrar o usuário com o email fornecido.
            var person = await _context.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));

            if (person == null)
                throw new Exception("O utilizador com o email fornecido não foi encontrado.");

            return person;
        }
    }
}
