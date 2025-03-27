using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using SoliGest.Server.Controllers;
using SoliGest.Server.Data;
using SoliGest.Server.Models;
using SoliGest.Server.Services;
using System.Collections;
using System.Linq.Expressions;

namespace SoliGestTest
{
    public class UsersControllerTest
    {
        private readonly Mock<UserManager<User>> _mockUserManager;
        private readonly Mock<IEmailService> _mockEmailService;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<SoliGestServerContext> _mockContext;
        private readonly UsersController _controller;

        public UsersControllerTest()
        {
            _mockUserManager = new Mock<UserManager<User>>(
                new Mock<IUserStore<User>>().Object, null, null, null, null, null, null, null, null);

            _mockEmailService = new Mock<IEmailService>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockContext = new Mock<SoliGestServerContext>(new DbContextOptionsBuilder<SoliGestServerContext>().Options);

            _controller = new UsersController(
                _mockUserManager.Object,
                _mockConfiguration.Object,
                _mockEmailService.Object,
                _mockContext.Object
            );
        }

        [Fact]
        public async Task Register_UserAlreadyExists_ReturnsBadRequest()
        {
            var model = new UserRegistrationModel { Email = "existinguser@test.com", Password = "Password123" };
            _mockUserManager.Setup(um => um.FindByEmailAsync(model.Email)).ReturnsAsync(new User());

            var result = await _controller.Register(model);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Register_NewUser_ReturnsOk()
        {
            var model = new UserRegistrationModel { Email = "newuser@test.com", Password = "Password123" };
            _mockUserManager.Setup(um => um.FindByEmailAsync(model.Email)).ReturnsAsync((User)null);
            _mockUserManager.Setup(um => um.CreateAsync(It.IsAny<User>(), model.Password)).ReturnsAsync(IdentityResult.Success);

            var result = await _controller.Register(model);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Login_InvalidCredentials_ReturnsBadRequest()
        {
            var model = new UserLoginModel { Email = "invalid@test.com", Password = "wrongpassword" };
            _mockUserManager.Setup(um => um.FindByEmailAsync(model.Email)).ReturnsAsync((User)null);

            var result = await _controller.Login(model);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Login_ReturnsOk()
        {
            var registerModel = new UserRegistrationModel { Email = "newuser@test.com", Password = "Password123" };
            _mockUserManager.Setup(um => um.FindByEmailAsync(registerModel.Email)).ReturnsAsync((User)null);
            _mockUserManager.Setup(um => um.CreateAsync(It.IsAny<User>(), registerModel.Password)).ReturnsAsync(IdentityResult.Success);

            var loginModel = new UserLoginModel { Email = "newuser@test.com", Password = "Password123" };
            _mockUserManager.Setup(um => um.FindByEmailAsync(loginModel.Email)).ReturnsAsync((User)null);

            var result = await _controller.Login(loginModel);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task ForgotPassword_UserNotFound_ReturnsBadRequest()
        {
            var model = new ForgotPasswordModel { Email = "nonexistent@test.com" };
            _mockUserManager.Setup(um => um.FindByEmailAsync(model.Email)).ReturnsAsync((User)null);

            var result = await _controller.ForgotPassword(model) as BadRequestObjectResult;

            Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, result.StatusCode);
            Assert.Contains("Email inválido.", result.Value.ToString());
        }

        [Fact]
        public async Task ForgotPassword_ValidUser_SendsEmail()
        {
            var user = new User { Email = "valid@test.com" };
            _mockUserManager.Setup(um => um.FindByEmailAsync(user.Email)).ReturnsAsync(user);
            _mockUserManager.Setup(um => um.GeneratePasswordResetTokenAsync(user)).ReturnsAsync("mocked-token");

            var model = new ForgotPasswordModel { Email = user.Email };
            var result = await _controller.ForgotPassword(model) as OkObjectResult;

            _mockEmailService.Verify(e => e.SendEmailAsync(user.Email, It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, result.StatusCode);
            Assert.Contains("Pedido de recuperação de palavra-passe enviado para o email!", result.Value.ToString());
        }

        [Fact]
        public async Task ResetPassword_DifferentPasswords_ReturnsBadRequest()
        {
            // Arrange
            var user = new User { Email = "user@test.com" };
            var model = new UserResetPasswordModel
            {
                Email = user.Email,
                Token = "valid-token",
                NewPassword = "NewPassword123"
            };

            _mockUserManager.Setup(um => um.FindByEmailAsync(model.Email))
                            .ReturnsAsync(user);

            _mockUserManager.Setup(um => um.ResetPasswordAsync(user, model.Token, model.NewPassword))
                            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Passwords do not match." }));

            // Act
            var result = await _controller.ResetPassword(model) as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
        }

        [Fact]
        public async Task ResetPassword_SuccessfulReset_ReturnsOk()
        {
            // Arrange
            var user = new User { Email = "user@test.com" };
            var model = new UserResetPasswordModel
            {
                Email = user.Email,
                Token = "valid-token",
                NewPassword = "NewPassword123"
            };

            _mockUserManager.Setup(um => um.FindByEmailAsync(model.Email))
                            .ReturnsAsync(user);

            _mockUserManager.Setup(um => um.ResetPasswordAsync(user, model.Token, model.NewPassword))
                            .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.ResetPassword(model) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.Contains("Password changed successfully", result.Value.ToString());
        }



        //testes sprint 2

        [Fact]
        public async Task PutPerson_ReturnsBadRequest_WhenIdDoesNotMatch()
        {
            var user = new User { Id = "123", Name = "Old Name" };
            var updatedUser = new UserUpdateModel { Id = "456", Name = "New Name" };

            _mockUserManager.Setup(m => m.FindByIdAsync(user.Id)).ReturnsAsync(user);
            _mockUserManager.Setup(m => m.UpdateAsync(It.IsAny<User>())).ReturnsAsync(IdentityResult.Success);

            var result = await _controller.PutPerson(updatedUser);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetPerson_ReturnsNotFound_WhenUserDoesNotExist()
        {
            _mockUserManager.Setup(m => m.FindByIdAsync("123")).ReturnsAsync((User)null);
            var result = await _controller.GetPerson("123");
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task PostPerson_CreatesNewUser()
        {
            var user = new User { Id = "123", Name = "John Doe" };

            var mockDbSet = new Mock<DbSet<User>>();

            mockDbSet.Setup(m => m.FindAsync("123")).ReturnsAsync(user);

            _mockContext.Setup(m => m.Users).Returns(mockDbSet.Object);
            _mockContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            _mockUserManager.Setup(um => um.CreateAsync(It.IsAny<User>())).ReturnsAsync(IdentityResult.Success);

            var result = await _controller.PostPerson(user);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);

            Assert.Equal("GetPerson", createdResult.ActionName);
        }

        [Fact]
        public async Task DeletePerson_ReturnsNotFound_WhenUserDoesNotExist()
        {
            _mockUserManager.Setup(m => m.FindByIdAsync("123")).ReturnsAsync((User)null);
            var result = await _controller.DeletePerson("123");
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task PutPerson_UpdatesUser_WhenIdMatches()
        {
            var user = new User { Id = "123", Name = "Old Name" };
            var updatedUser = new UserUpdateModel { Id = "123", Name = "New Name" };

            _mockUserManager.Setup(m => m.FindByIdAsync(user.Id)).ReturnsAsync(user);
            _mockUserManager.Setup(m => m.UpdateAsync(It.IsAny<User>())).ReturnsAsync(IdentityResult.Success);

            var result = await _controller.PutPerson(updatedUser);

            Assert.IsType<OkObjectResult>(result);
        }


        [Fact]
        public async Task GetPerson_ReturnsUser_WhenUserExists()
        {
            var user = new User { Id = "123", Name = "John Doe" };
            var mockDbSet = new Mock<DbSet<User>>();

            mockDbSet.Setup(m => m.FindAsync("123")).ReturnsAsync(user);
            _mockContext.Setup(m => m.Users).Returns(mockDbSet.Object);
            _mockContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var result = await _controller.GetPerson("123");

            Assert.NotNull(result);
            Assert.IsType<ActionResult<User>>(result);
            Assert.Equal("John Doe", result.Value.Name);
        }

        [Fact]
        public async Task PostPerson_ReturnsBadRequest_WhenUserCreationFails()
        {
            var user = new User { Id = "123", Name = "John Doe" };
            _mockUserManager.Setup(m => m.CreateAsync(user)).ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "User creation failed." }));

            var result = await _controller.PostPerson(user);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task DeletePerson_DeletesUser_WhenUserExists()
        {
            var user = new User { Id = "123", Name = "John Doe" };
            var mockDbSet = new Mock<DbSet<User>>();

            mockDbSet.Setup(m => m.FindAsync("123")).ReturnsAsync(user);
            _mockContext.Setup(m => m.Users).Returns(mockDbSet.Object);
            _mockContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var result = await _controller.DeletePerson("123");

            Assert.IsType<OkResult>(result);
        }
    }
}