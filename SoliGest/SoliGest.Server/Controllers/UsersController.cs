using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Shared;
using SendGrid.Helpers.Mail;
using SoliGest.Server.Data;
//using SoliGest.Server.Migrations;
using SoliGest.Server.Models;
using SoliGest.Server.Services;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace SoliGest.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly SoliGestServerContext _context;

        public UsersController(UserManager<User> userManager, IConfiguration configuration, IEmailService emailService, SoliGestServerContext context)
        {
            _userManager = userManager;
            _configuration = configuration;
            _emailService = emailService;
            _context = context;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationModel model)
        {
            var userExists = await _userManager.FindByEmailAsync(model.Email);
            if (userExists != null)
                return BadRequest(new { message = "User already exists." });

            var user = new User { UserName = model.Email, Email = model.Email, Name = model.Name, BirthDate = model.BirthDate, PhoneNumber = model.PhoneNumber, Address1 = model.Address1, Address2 = model.Address2, Role = model.Role, DayOff = model.DayOff, StartHoliday = model.StartHoliday, EndHoliday = model.EndHoliday };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return Ok(new { message = "User registered successfully." });
            }

            return BadRequest(result.Errors);
        }

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
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ocorreu um erro ao tentar processar o teu pedido." });
            }
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPerson([FromBody] UserUpdateModel model)
        {
            //var user = new User { UserName = model.Email, Email = model.Email, Name = model.Name, BirthDate = model.BirthDate, PhoneNumber = model.PhoneNumber, Address1 = model.Address1, Address2 = model.Address2 };

            //var result = await _userManager.UpdateAsync(user);

            //if (result.Succeeded)
            //{
            //    return Ok(new { message = "Utilizador atualizado com sucesso!" });
            //}
            //else
            //{
            //    return BadRequest(result.Errors);
            //}
            //if (ModelState.IsValid)
            //{
            //    var user = await _userManager.FindByEmailAsync(model.Email);
            //    if (user == null)
            //    {
            //        return NotFound();
            //    }

            //    user.UserName = model.Email;
            //    user.Email = model.Email;
            //    user.Name = model.Name;
            //    user.PhoneNumber = model.PhoneNumber;

            //    var result = await _userManager.UpdateAsync(user);

            //    if (!result.Succeeded)
            //    {
            //        ModelState.AddModelError("", result.Errors.First().Description);
            //        return BadRequest();
            //    }

            //    //return RedirectToAction("Index");
            //    return NoContent();
            //}

            //ModelState.AddModelError("", "Something failed.");
            //return Ok();

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

            //await _signInManager.RefreshSignInAsync(user);

            return Ok(new { message = "Utilizador atualizado com sucesso!" });
        }

        private bool UserExists(string id)
        {
            return _context.Users.Any(e => e.Id.Equals(id));
        }

        // GET: api/People/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetPerson(string id)
        {
            try
            {
                var person = await _context.Users.FindAsync(id);

                return person;
            } 
            catch
            {
                return NotFound();
            }
        }

        // DELETE: api/People/5
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

        // POST: api/People
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
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

        // GET: api/People
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetPerson()
        {
            return await _context.Users.ToListAsync();
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
        public string BirthDate { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; }
        public string DayOff { get; set; }
        public string StartHoliday { get; set; }
        public string EndHoliday { get; set; }
    }

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
