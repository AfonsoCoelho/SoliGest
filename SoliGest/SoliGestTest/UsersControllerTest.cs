using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using SoliGest.Server.Controllers;
using SoliGest.Server.Data;
using SoliGest.Server.Models;
using System.Collections;
using System.Linq.Expressions;

namespace SoliGestTest
{
    public class UsersControllerTest
    {
        private readonly Mock<UserManager<User>> _mockUserManager;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly UsersController _controller;

        public UsersControllerTest()
        {
            _mockUserManager = new Mock<UserManager<User>>(
                new Mock<IUserStore<User>>().Object, null, null, null, null, null, null, null, null);

            //_mockEmailService = new Mock<IEmailService>();
            _mockConfiguration = new Mock<IConfiguration>();

            _controller = new UsersController(
                _mockUserManager.Object,
                _mockConfiguration.Object
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
    }
}