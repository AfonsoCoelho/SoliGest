
using Microsoft.EntityFrameworkCore;
using SoliGest.Server.Data;
using SoliGest.Server.Models;

namespace SoliGest.Server.Services
{
    public class UserService : IUserService
    {
        private readonly SoliGestServerContext _context;

        public UserService(SoliGestServerContext context)
        {
            _context = context;
        }

        public async Task<User> GetUserByEmail(string email)
        {
            var person = await _context.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));

            if (person == null)
                throw new Exception("O utilizador com o email fornecido não foi encontrado.");

            return person;
        }
    }
}
