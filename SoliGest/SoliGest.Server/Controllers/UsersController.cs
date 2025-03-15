using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using SoliGest.Server.Models;
using SoliGest.Server.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace SoliGest.Server.Controllers
{
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly SendGridEmailService _emailService;

        public UsersController(UserManager<User> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
            _emailService = new SendGridEmailService(_configuration);
        }

        [HttpPost("api/signup")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationModel model)
        {
            var userExists = await _userManager.FindByEmailAsync(model.Email);
            if (userExists != null)
                return BadRequest(new { message = "User already exists." });

            var user = new User { UserName = model.Email, Email = model.Email, Name = model.Name, BirthDate = model.BirthDate };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return Ok(new { message = "User registered successfully." });
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("api/signin")]
        public async Task<IActionResult> Login([FromBody] UserLoginModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return BadRequest(new { message = "Invalid email or password." });
            }
            var passwordValid = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!passwordValid)
            {
                return BadRequest(new { message = "Invalid email or password." });
            }

            // Gerar os claims do utilizador
            var claims = new List<Claim>
            {
                new Claim("Name", user.Name),
                new Claim("Email", user.Email),
                new Claim("UserId", user.Id)
            };

            // Gerar o token JWT
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1), // Definir o tempo de expiração
                signingCredentials: creds);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            // Retornar o token ao cliente
            return Ok(new
            {
                token = tokenString,
                expiration = token.ValidTo
            });
        }

        [HttpPost("api/reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] UserResetPasswordModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
                return BadRequest(new { message = "Invalid Email." });

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);

            if (result.Succeeded)
            {
                return Ok(new { message = "Password changed successfully." });
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("api/forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordModel model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                    return BadRequest(new { message = "Email inválido." });

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var encodedToken = WebUtility.UrlEncode(token);

                var resetLink = $"{_configuration["Frontend:BaseUrl"]}/changepw?email={user.Email}&token={encodedToken}";

                await _emailService.SendEmailAsync(
                    user.Email!,
                    "Pedido de reposição de Palavra-passe",
                    $"<p>Olá {user.Name},</p>" +
                    $"<p>Esqueceste-te da tua palavra-passe? Não faz mal, acontece a todos! Carrega no link abaixo para continuar:</p>" +
                    $"<p><a href='{resetLink}'>Configurar nova palavra-passe</a></p>" +
                    $"<p>Se não fizeste este pedido, pedimos que ignores este email e tenhas atenção a qualquer atividade suspeita.</p>"
                );

                return Ok(new { message = "Pedido de recuperação de palavra-passe enviado para o email!" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] {ex.Message}");
                return StatusCode(500, new { message = "Ocorreu um erro ao tentar processar o teu pedido." });
            }
            
        }
    }

    public class UserLoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class UserRegistrationModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateOnly BirthDate { get; set; }
    }

    public class UserResetPasswordModel
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public string NewPassword { get; set; }
    }
  
    public class ForgotPasswordModel
    {
        public string Email { get; set; }
    }
}
