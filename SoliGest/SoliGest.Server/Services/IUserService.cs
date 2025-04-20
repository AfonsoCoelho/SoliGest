using Microsoft.AspNetCore.Mvc;
using SoliGest.Server.Models;

namespace SoliGest.Server.Services
{
    public interface IUserService
    {
        Task<User> GetUserByEmail(string email);
    }
}
