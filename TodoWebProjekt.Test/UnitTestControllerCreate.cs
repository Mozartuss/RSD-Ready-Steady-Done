using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TodoWebProjekt.Controllers;
using TodoWebProjekt.Repository;
using TodoWebProjekt.Test.Helper;
using TodoWebProjekt.ViewModel;
using Xunit;

namespace TodoWebProjekt.Test
{
    public class UnitTestControllerCreate
    {
        private readonly IAuthorizationService _authorizationService = new TestAuthorizationService();

        [Fact]
        public void Task_GetCreate_Return_RedirectToActionResult_NotAuthorized()
        {
            //Arrange
            ClaimsPrincipal user = null;
            var mockRepo = new Mock<ITodoRepository>();

            mockRepo.Setup(repo => repo.GetPossibleAssignUsers(It.IsAny<string>()))
                .Returns(TodoControllerHelper.GetTestUserSelectList());

            using var controller = new TodoController(mockRepo.Object, _authorizationService)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext {User = user}
                }
            };

            //Act
            var result = controller.Create();

            //Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Account", redirectToActionResult.ControllerName);
            Assert.Equal("AccessDenied", redirectToActionResult.ActionName);
        }

        [Fact]
        public void Task_GetCreate_Return_ViewResult_Authorized()
        {
            //Arrange
            var user = TodoControllerHelper.GetTestUser();
            var mockRepo = new Mock<ITodoRepository>();

            mockRepo.Setup(repo => repo.GetPossibleAssignUsers(It.IsAny<string>()))
                .Returns(TodoControllerHelper.GetTestUserSelectList());

            using var controller = new TodoController(mockRepo.Object, _authorizationService)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext {User = user}
                }
            };

            //Act
            var result = controller.Create();

            //Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Task_PostCreate_Return_BadRequest_ModelStateIsInavalid()
        {
            // Arrange
            var mockRepo = new Mock<ITodoRepository>();

            mockRepo.Setup(repo => repo.AddFileTask(It.IsAny<FileTaskViewModel>()))
                .ReturnsAsync(1 /*Success save to DataBase*/);
            using var controller = new TodoController(mockRepo.Object, _authorizationService);

            controller.ModelState.AddModelError("Title", "Required");

            // Act
            var result = await controller.Create(TodoControllerHelper.GetFileTaskViewModel());

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badRequestResult.Value);
        }

        [Fact]
        public async Task Task_PostCreate_Return_BadRequest_ModelStateIsValid()
        {
            // Arrange
            var user = new Mock<ClaimsPrincipal>();
            var mockRepo = new Mock<ITodoRepository>();
            mockRepo.Setup(repo => repo.AddFileTask(It.IsAny<FileTaskViewModel>()))
                .ReturnsAsync(10);
            using var controller = new TodoController(mockRepo.Object, _authorizationService)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext {User = user.Object}
                }
            };

            // Act
            var result = await controller.Create(TodoControllerHelper.GetFileTaskViewModel());

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }
    }
}