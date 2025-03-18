using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        private readonly UsersController _controller;

        public UsersControllerTest()
        {
            _mockUserManager = new Mock<UserManager<User>>(
                new Mock<IUserStore<User>>().Object, null, null, null, null, null, null, null, null);

            _mockEmailService = new Mock<IEmailService>();
            _mockConfiguration = new Mock<IConfiguration>();

            _controller = new UsersController(
                _mockUserManager.Object,
                _mockConfiguration.Object,
                _mockEmailService.Object
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

            var result = await _controller.ForgotPassword(model);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task ForgotPassword_ValidUser_SendsEmail()
        {
            var user = new User { Email = "valid@test.com" };
            _mockUserManager.Setup(um => um.FindByEmailAsync(user.Email)).ReturnsAsync(user);
            _mockUserManager.Setup(um => um.GeneratePasswordResetTokenAsync(user)).ReturnsAsync("mocked-token");

            var model = new ForgotPasswordModel { Email = user.Email };
            var result = await _controller.ForgotPassword(model);

            _mockEmailService.Verify(e => e.SendEmailAsync(user.Email, It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            Assert.IsType<OkObjectResult>(result);
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
    }
}