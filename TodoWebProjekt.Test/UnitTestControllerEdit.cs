using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using TodoWebProjekt.Controllers;
using TodoWebProjekt.Helper;
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
                .Returns((FileTaskViewModel)null);

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
                    HttpContext = new DefaultHttpContext { User = user }
                }
            };

            //Act
            var data = await controller.Edit(It.IsAny<int>());

            //Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(data);
            Assert.Equal("Account", redirectToActionResult.ControllerName);
            Assert.Equal("AccountError", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Task_GetEdit_Return_PartialViewResult_Authorized_Admin()
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
                    HttpContext = new DefaultHttpContext { User = user }
                }
            };

            //Act
            var data = await controller.Edit(It.IsAny<int>());

            //Assert
            var partialView = Assert.IsType<PartialViewResult>(data);
            Assert.Equal("_EditModal", partialView.ViewName);
            Assert.IsType<FileTaskViewModel>(partialView.Model);
        }

        [Fact]
        public async Task Task_GetEdit_Return_PartialViewResult_Authorized_User()
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
                    HttpContext = new DefaultHttpContext { User = user }
                }
            };

            //Act
            var data = await controller.Edit(It.IsAny<int>());

            //Assert
            var partialView = Assert.IsType<PartialViewResult>(data);
            Assert.Equal("_EditModal", partialView.ViewName);
            Assert.IsType<FileTaskViewModel>(partialView.Model);
        }

        [Fact]
        public async Task Task_PostEdit_Return_Failure_ModelStateIsInavalid()
        {
            // Arrange
            var mockRepo = new Mock<ITodoRepository>();
            mockRepo.Setup(repo => repo.Update(It.IsAny<FileTaskViewModel>()))
                .ReturnsAsync(1 /*Success save to DataBase*/);

            using var controller = new TodoController(mockRepo.Object, _authorizationService);
            controller.ModelState.AddModelError("Title", "Too Long");

            // Act
            var result = await controller.Edit(TodoControllerHelper.GetFileTaskViewModel());

            // Assert
            JsonResult jsonResult = Assert.IsType<JsonResult>(result);
            JsonErrorModel json = (JsonErrorModel)jsonResult.Value;

            Assert.Equal("failure", json.status);
            Assert.Equal("Too Long", json.formErrors.FirstOrDefault().errors.FirstOrDefault());
            Assert.Equal("Title", json.formErrors.FirstOrDefault().key);
        }

        [Fact]
        public async Task Task_PostEdit_Return_Failure_ModelStateIsValid_NullModel()
        {
            // Arrange
            var mockRepo = new Mock<ITodoRepository>();
            mockRepo.Setup(repo => repo.Update(It.IsAny<FileTaskViewModel>()))
                .ReturnsAsync(1 /*Success save to DataBase*/);

            using var controller = new TodoController(mockRepo.Object, _authorizationService);

            // Act
            var result = await controller.Edit((FileTaskViewModel)null);

            // Assert
            JsonResult jsonResult = Assert.IsType<JsonResult>(result);
            JsonErrorModel json = (JsonErrorModel)jsonResult.Value;

            Assert.Equal("failure", json.status);
            Assert.Equal("Sorry, your changes have been lost.", json.formErrors.FirstOrDefault().errors.FirstOrDefault());
            Assert.Equal("Task", json.formErrors.FirstOrDefault().key);
        }


        [Fact]
        public async Task Task_PostEdit_Return_Failure_UpdateFailed()
        {
            // Arrange
            var mockRepo = new Mock<ITodoRepository>();
            mockRepo.Setup(repo => repo.Update(It.IsAny<FileTaskViewModel>()))
                .ReturnsAsync(-1 /* Update Failed */);
            mockRepo.Setup(repo => repo.AddFile(It.IsAny<File>()))
                .Returns(EntityState.Added);

            using var controller = new TodoController(mockRepo.Object, _authorizationService);

            // Act
            var result = await controller.Edit(TodoControllerHelper.GetFileTaskViewModel());

            // Assert
            JsonResult jsonResult = Assert.IsType<JsonResult>(result);
            JsonErrorModel json = (JsonErrorModel)jsonResult.Value;

            Assert.Equal("failure", json.status);
            Assert.Equal("Failed to update the task!", json.formErrors.FirstOrDefault().errors.FirstOrDefault());
            Assert.Equal("Task", json.formErrors.FirstOrDefault().key);
        }

        [Fact]
        public async Task Task_PostEdit_Return_Failure_UpdateImageFailed()
        {
            // Arrange
            var mockRepo = new Mock<ITodoRepository>();
            mockRepo.Setup(repo => repo.AddFile(It.IsAny<File>()))
                .Returns(EntityState.Detached);

            using var controller = new TodoController(mockRepo.Object, _authorizationService);

            // Act
            var result = await controller.Edit(TodoControllerHelper.GetFileTaskViewModel());

            // Assert
            JsonResult jsonResult = Assert.IsType<JsonResult>(result);
            JsonErrorModel json = (JsonErrorModel)jsonResult.Value;

            Assert.Equal("failure", json.status);
            Assert.Equal("Image uploade failed.", json.formErrors.FirstOrDefault().errors.FirstOrDefault());
            Assert.Equal("UploadeImage", json.formErrors.FirstOrDefault().key);
        }

        [Fact]
        public async Task Task_PostEdit_Return_Success_ModelStateIsValid()
        {
            // Arrange
            var mockRepo = new Mock<ITodoRepository>();
            mockRepo.Setup(repo => repo.AddFile(It.IsAny<File>()))
                .Returns(EntityState.Added);
            mockRepo.Setup(repo => repo.Update(It.IsAny<FileTaskViewModel>()))
                .ReturnsAsync(1);
            using var controller = new TodoController(mockRepo.Object, _authorizationService);

            // Act
            var result = await controller.Edit(TodoControllerHelper.GetFileTaskViewModel());

            // Assert
            JsonResult jsonResult = Assert.IsType<JsonResult>(result);
            JsonErrorModel json = (JsonErrorModel)jsonResult.Value;

            Assert.Equal("success", json.status);
        }

        [Fact]
        public async Task Task_PostEdit_Return_Failure_ModelStateIsValid_WrongFile()
        {
            // Arrange
            var mockRepo = new Mock<ITodoRepository>();
            mockRepo.Setup(repo => repo.Update(It.IsAny<FileTaskViewModel>()))
                .ReturnsAsync(1);
            using var controller = new TodoController(mockRepo.Object, _authorizationService);

            // Act
            var result = await controller.Edit(TodoControllerHelper.GetWrongFileWithFileTaskViewModel());

            // Assert
            // Assert
            JsonResult jsonResult = Assert.IsType<JsonResult>(result);
            JsonErrorModel json = (JsonErrorModel)jsonResult.Value;

            Assert.Equal("failure", json.status);
            Assert.Equal("This file is not an image", json.formErrors.FirstOrDefault().errors.FirstOrDefault());
            Assert.Equal("UploadeImage", json.formErrors.FirstOrDefault().key);
        }

        [Fact]
        public async Task Task_PostEdit_Return_Failure_ModelStateIsValid_EmptyImage()
        {
            // Arrange
            var mockRepo = new Mock<ITodoRepository>();
            mockRepo.Setup(repo => repo.DeleteFile(It.IsAny<File>()))
                .Returns(false);
            using var controller = new TodoController(mockRepo.Object, _authorizationService);

            // Act
            var result = await controller.Edit(TodoControllerHelper.GetEmptyFileWithFileTaskViewModel());

            // Assert
            // Assert
            JsonResult jsonResult = Assert.IsType<JsonResult>(result);
            JsonErrorModel json = (JsonErrorModel)jsonResult.Value;

            Assert.Equal("failure", json.status);
            Assert.Equal("Not able to delete the Image.", json.formErrors.FirstOrDefault().errors.FirstOrDefault());
            Assert.Equal("UploadeImage", json.formErrors.FirstOrDefault().key);
        }
    }
}