using System.Collections;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json.Linq;
using TodoWebProjekt.Controllers;
using TodoWebProjekt.Helper;
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
        public void Task_GetCreate_Return_PartialViewResult_Authorized()
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
                    HttpContext = new DefaultHttpContext { User = user }
                }
            };

            //Act
            var result = controller.Create();

            //Assert
            var partialView = Assert.IsType<PartialViewResult>(result);
            Assert.Equal("_CreateModal", partialView.ViewName);
            Assert.IsType<FileTaskViewModel>(partialView.Model);
        }

        [Fact]
        public async Task Task_PostCreate_Return_Failure_ModelStateIsInavalid()
        {
            // Arrange
            var mockRepo = new Mock<ITodoRepository>();

            mockRepo.Setup(repo => repo.AddFileTask(It.IsAny<FileTaskViewModel>()))
                .ReturnsAsync(1 /*Success save to DataBase*/);
            using var controller = new TodoController(mockRepo.Object, _authorizationService);

            controller.ModelState.AddModelError("Title", "Too Long");

            // Act
            var result = await controller.Create(TodoControllerHelper.GetFileTaskViewModel()) as JsonResult;

            // Assert
            JsonResult jsonResult = Assert.IsType<JsonResult>(result);
            JsonErrorModel json = (JsonErrorModel)jsonResult.Value;

            Assert.Equal("failure", json.status);
            Assert.Equal("Too Long", json.formErrors.FirstOrDefault().errors.FirstOrDefault());
            Assert.Equal("Title", json.formErrors.FirstOrDefault().key);


        }

        [Fact]
        public async Task Task_PostCreate_Return_Success_ModelStateIsValid()
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
                    HttpContext = new DefaultHttpContext { User = user.Object }
                }
            };

            // Act
            var result = await controller.Create(TodoControllerHelper.GetFileTaskViewModel());

            // Assert
            JsonResult jsonResult = Assert.IsType<JsonResult>(result);
            JsonErrorModel json = (JsonErrorModel)jsonResult.Value;

            Assert.Equal("success", json.status);
        }

        [Fact]
        public async Task Task_PostCreate_Return_Failure_ModelStateIsValid()
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
                    HttpContext = new DefaultHttpContext { User = user.Object }
                }
            };

            // Act
            var result = await controller.Create(TodoControllerHelper.GetWrongFileWithFileTaskViewModel());

            // Assert
            JsonResult jsonResult = Assert.IsType<JsonResult>(result);
            JsonErrorModel json = (JsonErrorModel)jsonResult.Value;

            Assert.Equal("failure", json.status);
            Assert.Equal("This file is not an image", json.formErrors.FirstOrDefault().errors.FirstOrDefault());
            Assert.Equal("UploadeImage", json.formErrors.FirstOrDefault().key);
        }
    }
}