using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TodoWebProjekt.Controllers;
using TodoWebProjekt.Email;
using TodoWebProjekt.Test.Helper;
using TodoWebProjekt.ViewModel;
using Xunit;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace TodoWebProjekt.Test
{
    public class UnitTestAccountControllerSingIn
    {
        [Fact]
        public async Task Task_Login_Return_BadRequest_Invalid()
        {
            // Arrange
            using var controller = new AccountController(AccountConttrollerHelper.GetMockUserManager().Object,
                AccountConttrollerHelper.GetMockSignInManager().Object,
                AccountConttrollerHelper.GetMockRoleManager().Object,
            new Mock<IEmailSender>().Object);
            controller.ModelState.AddModelError("Email", "Required");

            // Act
            var result = await controller.Login(It.IsAny<LoginViewModel>());

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badRequestResult.Value);
        }

        [Fact]
        public void Task_Login_Return_BadRequest_InvalidLoginAttempt()
        {
            // Arrange
            using var controller = new AccountController(AccountConttrollerHelper.GetMockUserManager().Object,
                AccountConttrollerHelper.GetMockSignInManager().Object,
                AccountConttrollerHelper.GetMockRoleManager().Object,
            new Mock<IEmailSender>().Object);

            // Act
            var result = controller.Login((string)null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var valueSerializable = (SerializableError)badRequestResult.Value;
            var errorMessageDict = valueSerializable.FirstOrDefault();

            if (errorMessageDict.Value is string[] errorMessage)
            {
                Assert.Equal("Invalid Login Attempt", errorMessage.GetValue(0));
            }
        }

        [Fact]
        public async Task Task_Login_Return_RedirectToAction_LoginSuccess()
        {
            // Arrange
            var mockSignInManager = AccountConttrollerHelper.GetMockSignInManager();
            mockSignInManager
                .Setup(option =>
                    option.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(),
                        It.IsAny<bool>())).ReturnsAsync(SignInResult.Success);
            using var controller = new AccountController(AccountConttrollerHelper.GetMockUserManager().Object,
                mockSignInManager.Object, AccountConttrollerHelper.GetMockRoleManager().Object,
            new Mock<IEmailSender>().Object);

            // Act
            var result = await controller.Login(new LoginViewModel());

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }
    }
}