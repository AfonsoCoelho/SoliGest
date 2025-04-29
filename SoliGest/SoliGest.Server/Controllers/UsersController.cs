using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SendGrid.Helpers.Mail;
using SoliGest.Server.Data;
using SoliGest.Server.Models;
using SoliGest.Server.Services;

namespace SoliGest.Server.Controllers
{
    /// <summary>
    /// Controlador que gere operações relacionadas com utilizadores, incluindo registo, autenticação,
    /// recuperação de palavra-passe e gestão de perfil.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly SoliGestServerContext _context;
        private readonly IUserService _userService;

        /// <summary>
        /// Construtor do controlador de utilizadores.
        /// </summary>
        /// <param name="userManager">Serviço do ASP.NET Identity para gestão de utilizadores.</param>
        /// <param name="configuration">Configurações da aplicação (JWT, URLs, etc.).</param>
        /// <param name="emailService">Serviço para envio de emails.</param>
        /// <param name="context">Contexto da base de dados SoliGest.</param>
        /// <param name="userService">Serviço customizado para operações adicionais com utilizadores.</param>
        public UsersController(
            UserManager<User> userManager,
            IConfiguration configuration,
            IEmailService emailService,
            SoliGestServerContext context,
            IUserService userService)
        {
            _userManager = userManager;
            _configuration = configuration;
            _emailService = emailService;
            _context = context;
            _userService = userService;
        }

        /// <summary>
        /// Regista um novo utilizador com as credenciais fornecidas.
        /// </summary>
        /// <param name="model">Dados de registo do utilizador.</param>
        /// <returns>Resultado da operação: sucesso ou lista de erros.</returns>
        [HttpPost("signup")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationModel model)
        {
            var userExists = await _userManager.FindByEmailAsync(model.Email);
            if (userExists != null)
                return BadRequest(new { message = "User already exists." });

            var user = new User
            {
                UserName = model.Email,
                Email = model.Email,
                Name = model.Name,
                BirthDate = model.BirthDate,
                PhoneNumber = model.PhoneNumber,
                Address1 = model.Address1,
                Address2 = model.Address2,
                Role = model.Role,
                DayOff = model.DayOff,
                StartHoliday = model.StartHoliday,
                EndHoliday = model.EndHoliday
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return Ok(new { message = "User registered successfully." });
            }

            return BadRequest(result.Errors);
        }

        /// <summary>
        /// Autentica o utilizador e gera um token JWT.
        /// </summary>
        /// <param name="model">Dados de login (email e password).</param>
        /// <returns>Token JWT e data de expiração.</returns>
        [HttpPost("signin")]
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

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("Name", user.Name),
                new Claim("Email", user.Email),
                new Claim("UserId", user.Id)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: creds);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new
            {
                token = tokenString,
                expiration = token.ValidTo
            });
        }

        /// <summary>
        /// Redefine a palavra-passe de um utilizador com base no token enviado.
        /// </summary>
        /// <param name="model">Dados para redefinição de password.</param>
        /// <returns>Resultado da operação.</returns>
        [HttpPost("reset-password")]
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

        /// <summary>
        /// Inicia o processo de recuperação de palavra-passe enviando um email ao utilizador.
        /// </summary>
        /// <param name="model">Email do utilizador para recuperação.</param>
        /// <returns>Mensagem de resultado.</returns>
        [HttpPost("forgot-password")]
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
            catch (Exception)
            {
                return StatusCode(500, new { message = "Ocorreu um erro ao tentar processar o teu pedido." });
            }
        }

        /// <summary>
        /// Atualiza as propriedades de um utilizador existente.
        /// </summary>
        /// <param name="model">Dados atualizados do utilizador.</param>
        /// <returns>Resultado da operação de atualização.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPerson([FromBody] UserUpdateModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (model.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, model.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    return BadRequest(new { message = "Unexpected error when trying to set phone number." });
                }
            }

            var email = await _userManager.GetEmailAsync(user);
            if (model.Email != email)
            {
                var setEmailResult = await _userManager.SetEmailAsync(user, model.Email);
                if (!setEmailResult.Succeeded)
                {
                    return BadRequest(new { message = "Unexpected error when trying to set email." });
                }
            }

            user.Name = model.Name;
            user.Address1 = model.Address1;
            user.Address2 = model.Address2;
            user.BirthDate = model.BirthDate;
            user.Role = model.Role;
            user.DayOff = model.DayOff;
            user.StartHoliday = model.StartHoliday;
            user.EndHoliday = model.EndHoliday;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", result.Errors.First().Description);
                return BadRequest();
            }

            return Ok(new { message = "Utilizador atualizado com sucesso!" });
        }

        /// <summary>
        /// Verifica a existência de um utilizador pelo ID.
        /// </summary>
        /// <param name="id">ID do utilizador.</param>
        /// <returns>True se existir, false caso contrário.</returns>
        private bool UserExists(string id)
        {
            return _context.Users.Any(e => e.Id.Equals(id));
        }

        /// <summary>
        /// Obtém um utilizador pelo ID.
        /// </summary>
        /// <param name="id">ID do utilizador.</param>
        /// <returns>Objeto <see cref="User"/> se encontrado.</returns>
        [HttpGet("by-id/{id}")]
        public async Task<ActionResult<User>> GetPerson(string id)
        {
            try
            {
                var person = await _context.Users.FindAsync(id);
                if (person != null)
                {
                    return person;
                }
                else
                {
                    return NotFound();
                }
            }
            catch
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Obtém um utilizador pelo email.
        /// </summary>
        /// <param name="email">Email do utilizador.</param>
        /// <returns>Objeto <see cref="User"/> se encontrado.</returns>
        [HttpGet("by-email/{email}")]
        public async Task<ActionResult<User>> GetUserByEmail(string email)
        {
            try
            {
                var user = await _userService.GetUserByEmail(email);
                return Ok(user);
            }
            catch
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Elimina um utilizador pelo ID.
        /// </summary>
        /// <param name="id">ID do utilizador.
        /// </param>
        /// <returns>Resultado da operação.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePerson(string id)
        {
            try
            {
                var person = await _context.Users.FindAsync(id);
                _context.Users.Remove(person);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Cria um utilizador diretamente no contexto.
        /// </summary>
        /// <param name="user">Objeto <see cref="User"/> a ser criado.</param>
        /// <returns>Objeto criado com status 201.</returns>
        [HttpPost]
        public async Task<IActionResult> PostPerson(User user)
        {
            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return CreatedAtAction("GetPerson", new { id = user.Id }, user);
            }
            catch
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Define um utilizador como ativo.
        /// </summary>
        /// <param name="userId">ID do utilizador.</param>
        /// <returns>Usuário atualizado.</returns>
        [HttpPut("set-user-as-active/{userId}")]
        public async Task<IActionResult> SetUserAsActive(string userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return BadRequest($"User with id {userId} not found!");
                }
                user.isActive = true;
                await _context.SaveChangesAsync();
                return CreatedAtAction("GetPerson", new { id = user.Id }, user);
            }
            catch
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Define um utilizador como inativo.
        /// </summary>
        /// <param name="userId">ID do utilizador.</param>
        /// <returns>Usuário atualizado.</returns>
        [HttpPut("set-user-as-inactive/{userId}")]
        public async Task<IActionResult> SetUserAsInactive(string userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return BadRequest($"User with id {userId} not found!");
                }
                user.isActive = false;
                await _context.SaveChangesAsync();
                return CreatedAtAction("GetPerson", new { id = user.Id }, user);
            }
            catch
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Atualiza a localização geográfica do utilizador.
        /// </summary>
        /// <param name="userId">ID do utilizador.</param>
        /// <param name="latitude">Latitude GPS.</param>
        /// <param name="longitude">Longitude GPS.</param>
        /// <returns>Usuário atualizado.</returns>
        [HttpPut("update-location/{userId}")]
        public async Task<IActionResult> UpdateUserLocation(string userId, double latitude, double longitude)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return BadRequest($"User with id {userId} not found!");
                }
                user.Latitude = latitude;
                user.Longitude = longitude;
                await _context.SaveChangesAsync();
                return CreatedAtAction("GetPerson", new { id = user.Id }, user);
            }
            catch
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Guarda uma imagem de perfil para o utilizador.
        /// </summary>
        /// <param name="userId">ID do utilizador.</param>
        /// <param name="file">Ficheiro enviado contendo a imagem.</param>
        /// <returns>Caminho relativo da imagem guardada.</returns>
        [HttpPut("save-profile-picture/{userId}")]
        public async Task<string> SaveProfilePicture(string userId, IFormFile file)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + '_' + userId + "_" + file.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            var user = await _context.Users.FindAsync(userId);
            user.ProfilePictureUrl = uniqueFileName;
            await _context.SaveChangesAsync();

            return Path.Combine("uploads", uniqueFileName);
        }

        /// <summary>
        /// Obtém a lista de todos os utilizadores.
        /// </summary>
        /// <returns>Lista de objetos <see cref="User"/>.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetPerson()
        {
            return await _context.Users.ToListAsync();
        }
    }

    /// <summary>
    /// Modelo de login para autenticação de utilizadores.
    /// </summary>
    public class UserLoginModel
    {
        /// <summary>
        /// Email do utilizador.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Password do utilizador.
        /// </summary>
        public string Password { get; set; }
    }

    /// <summary>
    /// Modelo de registo de novo utilizador.
    /// </summary>
    public class UserRegistrationModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string BirthDate { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; }
        public string DayOff { get; set; }
        public string StartHoliday { get; set; }
        public string EndHoliday { get; set; }
    }

    /// <summary>
    /// Modelo para atualizar dados de um utilizador existente.
    /// </summary>
    public class UserUpdateModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string BirthDate { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; }
        public string DayOff { get; set; }
        public string StartHoliday { get; set; }
        public string EndHoliday { get; set; }
    }

    /// <summary>
    /// Modelo para redefinição de palavra-passe.
    /// </summary>
    public class UserResetPasswordModel
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public string NewPassword { get; set; }
    }

    /// <summary>
    /// Modelo para pedido de recuperação de palavra-passe.
    /// </summary>
    public class ForgotPasswordModel
    {
        public string Email { get; set; }
    }
}
