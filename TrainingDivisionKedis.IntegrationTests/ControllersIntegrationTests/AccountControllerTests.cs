using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TrainingDivisionKedis.BLL.Contracts;
using TrainingDivisionKedis.BLL.DTO.User;
using TrainingDivisionKedis.BLL.Services;
using TrainingDivisionKedis.Controllers;
using TrainingDivisionKedis.DAL.ApplicationDbContext;
using TrainingDivisionKedis.Extensions.Alerts;
using TrainingDivisionKedis.ViewModels.Account;
using Xunit;

namespace TrainingDivisionKedis.Tests.ControllersIntegrationTests
{
    public class AccountControllerTests
    {
        AccountController _sut;

        public AccountControllerTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
               .UseSqlServer("Server=E7450-PC;Database=KEDIS;Trusted_Connection=True;")
               .Options;
            var contextFactory = new AppDbContextFactory(options);
            var userService = new TeacherUserService(contextFactory);
            _sut = new MockAccountController(userService);
        }

        [Fact]
        public async Task Login_ShouldReturnARedirect()
        {          
            // Act
            var viewModel = new LoginViewModel { Login = "ramatov", Password = "123456" };
            var response = await _sut.Login(viewModel);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(response);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Login_ShouldReturnModelWithError()
        {
            // Act
            var viewModel = new LoginViewModel { Login = "abcdef", Password = "123456" };
            var response = await _sut.Login(viewModel);

            // Assert
            var result = Assert.IsType<ViewResult>(response);
            Assert.Equal(ComparableObject.Convert(viewModel), ComparableObject.Convert(result.ViewData.Model));
            Assert.True(result.ViewData.ModelState.ErrorCount > 0);
        }
        
        [Fact]
        public void ChangePassword_ShouldReturnView()
        {
            // Act           
            var response = _sut.ChangePassword();

            // Assert
            var result = Assert.IsType<ViewResult>(response);
        }

        [Fact]
        public async Task ChangePassword_ShouldReturnARedirect()
        {
            // Arrange
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "John Doe"),
                new Claim(ClaimTypes.NameIdentifier, "1307"),
                new Claim(ClaimTypes.Role, "директор")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = claimsPrincipal
                }
            };
            _sut.ControllerContext = context;

            // Act
            var request = new ChangeUserPasswordRequest { OldPassword = "123456", NewPassword = "1234567" };
            var response = await _sut.ChangePassword(request);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(response);
            Assert.Equal("Login", redirectToActionResult.ActionName);

            // Reload
            request.OldPassword = request.NewPassword;
            request.NewPassword = "123456";
            await _sut.ChangePassword(request);
        }

        [Fact]
        public async Task ChangePassword_ShouldReturnDangerAlert()
        {
            // Arrange
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "John Doe"),
                new Claim(ClaimTypes.NameIdentifier, "0"),
                new Claim(ClaimTypes.Role, "директор")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = claimsPrincipal
                }
            };
            _sut.ControllerContext = context;

            // Act
            var request = new ChangeUserPasswordRequest { OldPassword = "123456", NewPassword = "1234567" };
            var response = await _sut.ChangePassword(request);

            // Assert
            var result = Assert.IsType<AlertDecoratorResult>(response);
            Assert.Equal("danger", result.Type);
        }

        [Fact]
        public void ChangeLogin_ShouldReturnView()
        {
            // Arrange
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "John Doe"),
                new Claim(ClaimTypes.NameIdentifier, "1307"),
                new Claim(ClaimTypes.Role, "директор")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = claimsPrincipal
                }
            };
            _sut.ControllerContext = context;

            // Act           
            var response = _sut.ChangeLogin();

            // Assert
            var result = Assert.IsType<ViewResult>(response);
            Assert.IsType<ChangeLoginViewModel>(result.Model);
        }

        [Fact]
        public async Task ChangeLogin_ShouldReturnARedirect()
        {
            // Arrange
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "John Doe"),
                new Claim(ClaimTypes.NameIdentifier, "1307"),
                new Claim(ClaimTypes.Role, "директор")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = claimsPrincipal
                }
            };
            _sut.ControllerContext = context;

            // Act
            var request = new ChangeUserLoginRequest { NewLogin = "new_login" };
            var response = await _sut.ChangeLogin(request);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(response);
            Assert.Equal("Login", redirectToActionResult.ActionName);

            // Reload
            request.NewLogin = "ramatov";
            await _sut.ChangeLogin(request);
        }

        [Fact]
        public async Task ChangeLogin_ShouldReturnDangerAlert()
        {
            // Arrange
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "John Doe"),
                new Claim(ClaimTypes.NameIdentifier, "0"),
                new Claim(ClaimTypes.Role, "директор")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = claimsPrincipal
                }
            };
            _sut.ControllerContext = context;

            // Act
            var request = new ChangeUserLoginRequest { NewLogin = "new_login" };
            var response = await _sut.ChangeLogin(request);

            // Assert
            var result = Assert.IsType<AlertDecoratorResult>(response);
            Assert.Equal("danger", result.Type);
        }
    }

    class MockAccountController : AccountController
    {
        public MockAccountController(IUserService userService) : base(userService)
        {
        }

        public override async Task<IActionResult> Logout()
        {
            return await Task.FromResult(RedirectToAction("Index", "Home"));
        }

        protected override async Task Authenticate(UserDto user)
        {
            try
            {
                await base.Authenticate(user);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

}
