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
    public class UnitTestControllerDelete
    {
        private readonly IAuthorizationService _authorizationService = new TestAuthorizationService();

        [Fact]
        public async Task Task_Delete_Return_BadRequest_DeleteFailed()
        {
            //Arrange

            var mockRepo = new Mock<ITodoRepository>();
            var fileTaskViewModel = TodoControllerHelper.GetFileTaskViewModel();
            var user = TodoControllerHelper.GetTestUser();

            mockRepo.Setup(repo => repo.GetFileTaskViewModel(It.IsAny<int>()))
                .ReturnsAsync(fileTaskViewModel);

            mockRepo.Setup(repo => repo.Delete(It.IsAny<int>()))
                .ReturnsAsync(0 /* Delete Failed */);


            using var controller = new TodoController(mockRepo.Object, _authorizationService)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext {User = user}
                }
            };

            //Act
            var result = await controller.Delete(It.IsAny<int>());

            //Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Delete dosn't work", badRequestResult.Value);
        }

        [Fact]
        public async Task Task_Delete_Return_BadRequest_IdIsNull()
        {
            //Arrange

            var mockRepo = new Mock<ITodoRepository>();

            using var controller = new TodoController(mockRepo.Object, _authorizationService);

            //Act
            var result = await controller.Delete(null);

            //Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Id is null", badRequestResult.Value);
        }

        [Fact]
        public async Task Task_Delete_Return_BadRequest_NoFileTaskViewModel()
        {
            //Arrange

            var mockRepo = new Mock<ITodoRepository>();
            FileTaskViewModel fileTaskViewModel = null;

            mockRepo.Setup(repo => repo.GetFileTaskViewModel(It.IsAny<int>()))
                .ReturnsAsync(fileTaskViewModel);

            using var controller = new TodoController(mockRepo.Object, _authorizationService);

            //Act
            var result = await controller.Delete(It.IsAny<int>());

            //Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Wrong id", badRequestResult.Value);
        }

        [Fact]
        public async Task Task_Delete_Return_RedirectToActionResult_Authorized_Admin()
        {
            //Arrange

            var mockRepo = new Mock<ITodoRepository>();
            var fileTaskViewModel = TodoControllerHelper.GetFileTaskViewModel();
            var user = TodoControllerHelper.GetTestAdmin();

            mockRepo.Setup(repo => repo.GetFileTaskViewModel(It.IsAny<int>()))
                .ReturnsAsync(fileTaskViewModel);

            mockRepo.Setup(repo => repo.Delete(It.IsAny<int>()))
                .ReturnsAsync(1 /* Delete Success */);


            using var controller = new TodoController(mockRepo.Object, _authorizationService)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext {User = user}
                }
            };

            //Act
            var result = await controller.Delete(It.IsAny<int>());

            //Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("LoadTodoList", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Task_Delete_Return_RedirectToActionResult_NotAuthorized_User()
        {
            //Arrange

            var mockRepo = new Mock<ITodoRepository>();
            var fileTaskViewModel = TodoControllerHelper.GetFileTaskViewModel();
            var user = TodoControllerHelper.GetTestFakeUser();

            mockRepo.Setup(repo => repo.GetFileTaskViewModel(It.IsAny<int>()))
                .ReturnsAsync(fileTaskViewModel);

            mockRepo.Setup(repo => repo.Delete(It.IsAny<int>()))
                .ReturnsAsync(1 /* Delete Success */);


            using var controller = new TodoController(mockRepo.Object, _authorizationService)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext {User = user}
                }
            };

            //Act
            var result = await controller.Delete(It.IsAny<int>());

            //Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Account", redirectToActionResult.ControllerName);
            Assert.Equal("AccountError", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Task_Delete_Return_PartialViewResult_Authorized_User()
        {
            //Arrange

            var mockRepo = new Mock<ITodoRepository>();
            var fileTaskViewModel = TodoControllerHelper.GetFileTaskViewModel();
            var user = TodoControllerHelper.GetTestUser();

            mockRepo.Setup(repo => repo.GetFileTaskViewModel(It.IsAny<int>()))
                .ReturnsAsync(fileTaskViewModel);

            mockRepo.Setup(repo => repo.Delete(It.IsAny<int>()))
                .ReturnsAsync(1 /* Delete Success */);


            using var controller = new TodoController(mockRepo.Object, _authorizationService)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext {User = user}
                }
            };

            //Act
            var result = await controller.Delete(It.IsAny<int>());

            //Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("LoadTodoList", redirectToActionResult.ActionName);
        }
    }
}