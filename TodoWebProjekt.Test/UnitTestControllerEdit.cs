using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TodoWebProjekt.Controllers;
using TodoWebProjekt.Models;
using TodoWebProjekt.Repository;
using TodoWebProjekt.Test.Helper;
using TodoWebProjekt.ViewModel;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace TodoWebProjekt.Test
{
    public class UnitTestControllerEdit
    {
        private readonly IAuthorizationService _authorizationService = new TestAuthorizationService();

        [Fact]
        public async Task Task_GetEdit_Return_BadRequest_NoFileTaskViewModel()
        {
            //Arrange

            var mockRepo = new Mock<ITodoRepository>();

            mockRepo.Setup(repo => repo.GetEditView(It.IsAny<int>()))
                .Returns((FileTaskViewModel) null);

            using var controller = new TodoController(mockRepo.Object, _authorizationService);

            //Act
            var data = await controller.Edit(It.IsAny<int>());

            //Assert
            Assert.IsType<BadRequestResult>(data);
        }

        [Fact]
        public async Task Task_GetEdit_Return_RedirectToActionResult_NotAuthorized_User()
        {
            //Arrange

            var mockRepo = new Mock<ITodoRepository>();
            var fileTaskViewModel = TodoControllerHelper.GetFileTaskViewModel();
            var user = TodoControllerHelper.GetTestFakeUser();

            mockRepo.Setup(repo => repo.GetEditView(It.IsAny<int>()))
                .Returns(fileTaskViewModel);


            using var controller = new TodoController(mockRepo.Object, _authorizationService)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext {User = user}
                }
            };

            //Act
            var data = await controller.Edit(It.IsAny<int>());

            //Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(data);
            Assert.Equal("Account", redirectToActionResult.ControllerName);
            Assert.Equal("AccessDenied", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Task_GetEdit_Return_ViewResult_Authorized_Admin()
        {
            //Arrange

            var mockRepo = new Mock<ITodoRepository>();
            var fileTaskViewModel = TodoControllerHelper.GetFileTaskViewModel();
            var user = TodoControllerHelper.GetTestAdmin();

            mockRepo.Setup(repo => repo.GetEditView(It.IsAny<int>()))
                .Returns(fileTaskViewModel);


            using var controller = new TodoController(mockRepo.Object, _authorizationService)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext {User = user}
                }
            };

            //Act
            var data = await controller.Edit(It.IsAny<int>());

            //Assert
            Assert.IsType<ViewResult>(data);
        }

        [Fact]
        public async Task Task_GetEdit_Return_ViewResult_Authorized_User()
        {
            //Arrange

            var mockRepo = new Mock<ITodoRepository>();
            var fileTaskViewModel = TodoControllerHelper.GetFileTaskViewModel();
            var user = TodoControllerHelper.GetTestUser();

            mockRepo.Setup(repo => repo.GetEditView(It.IsAny<int>()))
                .Returns(fileTaskViewModel);


            using var controller = new TodoController(mockRepo.Object, _authorizationService)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext {User = user}
                }
            };

            //Act
            var data = await controller.Edit(It.IsAny<int>());

            //Assert
            Assert.IsType<ViewResult>(data);
        }

        [Fact]
        public async Task Task_PostEdit_Return_BadRequest_ModelStateIsInavalid()
        {
            // Arrange
            var mockRepo = new Mock<ITodoRepository>();
            mockRepo.Setup(repo => repo.Update(It.IsAny<FileTaskViewModel>()))
                .ReturnsAsync(1 /*Success save to DataBase*/);

            using var controller = new TodoController(mockRepo.Object, _authorizationService);
            controller.ModelState.AddModelError("Title", "Required");

            // Act
            var result = await controller.Edit(TodoControllerHelper.GetFileTaskViewModel());


            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badRequestResult.Value);
        }

        [Fact]
        public async Task Task_PostEdit_Return_BadRequest_UpdateFailed()
        {
            // Arrange
            var mockRepo = new Mock<ITodoRepository>();
            mockRepo.Setup(repo => repo.Update(It.IsAny<FileTaskViewModel>()))
                .ReturnsAsync(0 /* Update Failed */);

            using var controller = new TodoController(mockRepo.Object, _authorizationService);

            // Act
            var result = await controller.Edit(TodoControllerHelper.GetFileTaskViewModel());

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task Task_PostEdit_Return_BadRequest_UpdateImageFailed()
        {
            // Arrange
            var mockRepo = new Mock<ITodoRepository>();
            mockRepo.Setup(repo => repo.AddFile(It.IsAny<File>()))
                .Returns(0 /* Add Failed */);

            using var controller = new TodoController(mockRepo.Object, _authorizationService);

            // Act
            var result = await controller.Edit(TodoControllerHelper.GetFileTaskViewModel());

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task Task_PostEdit_Return_RedirectToActionResult_ModelStateIsValid()
        {
            // Arrange
            var mockRepo = new Mock<ITodoRepository>();
            mockRepo.Setup(repo => repo.Update(It.IsAny<FileTaskViewModel>()))
                .ReturnsAsync(1);
            using var controller = new TodoController(mockRepo.Object, _authorizationService);

            // Act
            var result = await controller.Edit(TodoControllerHelper.GetFileTaskViewModel());

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }
    }
}