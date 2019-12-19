using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;
using TodoWebProjekt.Controllers;
using TodoWebProjekt.Email;
using TodoWebProjekt.Models;
using TodoWebProjekt.Test.Helper;
using TodoWebProjekt.ViewModel;
using Xunit;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace TodoWebProjekt.Test
{
    public class UnitTestAccountControllerSingIn
    {
        [Fact]
        public async System.Threading.Tasks.Task Task_Login_Return_BadRequest_Invalid()
        {
            // Arrange
            using var controller = new AccountController(
                AccountConttrollerHelper.GetMockUserManager().Object,
                AccountConttrollerHelper.GetMockSignInManager().Object,
                AccountConttrollerHelper.GetMockRoleManager().Object,
            new Mock<IEmailSender>().Object,
            new Mock<TODOContext>().Object);
            controller.ModelState.AddModelError("Email", "Required");

            // Act
            var result = await controller.Login(It.IsAny<LoginViewModel>());

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.False(viewResult.ViewData.ModelState.IsValid);
        }

        [Fact]
        public async System.Threading.Tasks.Task Task_Login_Return_BadRequest_InvalidLoginAttempt()
        {
            // Arrange
            using var controller = new AccountController(AccountConttrollerHelper.GetMockUserManager().Object,
                AccountConttrollerHelper.GetMockSignInManager().Object,
                AccountConttrollerHelper.GetMockRoleManager().Object,
            new Mock<IEmailSender>().Object,
            new Mock<TODOContext>().Object);

            // Act
            var result = await controller.Login((LoginViewModel)null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            KeyValuePair<string, ModelStateEntry> errorMessage = viewResult.ViewData.ModelState.FirstOrDefault();

            Assert.Equal("Invalid Login Attempt", errorMessage.Value.Errors[0].ErrorMessage);

        }

        [Fact]
        public async System.Threading.Tasks.Task Task_Login_Return_RedirectToAction_LoginSuccess()
        {
            // Arrange
            var mockSignInManager = AccountConttrollerHelper.GetMockSignInManager();
            mockSignInManager
                .Setup(option =>
                    option.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(),
                        It.IsAny<bool>())).ReturnsAsync(SignInResult.Success);
            using var controller = new AccountController(AccountConttrollerHelper.GetMockUserManager().Object,
                mockSignInManager.Object, AccountConttrollerHelper.GetMockRoleManager().Object,
            new Mock<IEmailSender>().Object,
            new Mock<TODOContext>().Object);
            // Act
            var result = await controller.Login(new LoginViewModel());

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }
    }
}