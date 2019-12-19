using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TodoWebProjekt.Controllers;
using TodoWebProjekt.Repository;
using TodoWebProjekt.Test.Helper;
using TodoWebProjekt.ViewModel;
using Xunit;

namespace TodoWebProjekt.Test
{
    public class UnitTestControllerDetail
    {
        private readonly IAuthorizationService _authorizationService = new TestAuthorizationService();

        [Fact]
        public async Task Task_GetPostById_MatchResult()
        {
            //Arrange
            var mockRepo = new Mock<ITodoRepository>();
            mockRepo.Setup(repo => repo.GetFileTaskViewModel(0))
                .ReturnsAsync(TodoControllerHelper.GetFileTaskViewModel());
            using var controller = new TodoController(mockRepo.Object, _authorizationService);
            int? id = 0;

            //Act
            var result = await controller.Details(id);

            //Assert
            var partialViewResult = Assert.IsType<PartialViewResult>(result);
            var model = Assert.IsAssignableFrom<FileTaskViewModel>(partialViewResult.ViewData.Model);

            Assert.Equal("Test Eintrag", model.Task.Title);
            Assert.Equal("Ein Eintrag für einen kleinen Test", model.Task.Description);
        }

        [Fact]
        public async Task Task_GetPostById_Return_BadRequestResult()
        {
            //Arrange
            var mockRepo = new Mock<ITodoRepository>();
            mockRepo.Setup(repo => repo.GetFileTaskViewModel(null))
                .ReturnsAsync(TodoControllerHelper.GetFileTaskViewModel());
            using var controller = new TodoController(mockRepo.Object, _authorizationService);

            //Act
            var data = await controller.Details(null);

            //Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(data);
            Assert.Equal("The ID is null!", badRequestResult.Value);
        }

        [Fact]
        public async Task Task_GetTodoById_Return_NotFoundResult()
        {
            //Arrange
            var mockRepo = new Mock<ITodoRepository>();
            mockRepo.Setup(repo => repo.GetFileTaskViewModel(3))
                .ReturnsAsync((FileTaskViewModel) null);
            using var controller = new TodoController(mockRepo.Object, _authorizationService);
            var id = 3;

            //Act
            var result = await controller.Details(id);

            //Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Task_GetTodoById_Return_PartialViewResult()
        {
            //Arrange
            var mockRepo = new Mock<ITodoRepository>();
            mockRepo.Setup(repo => repo.GetFileTaskViewModel(0))
                .ReturnsAsync(TodoControllerHelper.GetFileTaskViewModel());
            using var controller = new TodoController(mockRepo.Object, _authorizationService);

            //Act
            var result = await controller.Details(It.IsAny<int>());

            //Assert
           var partialViewResult = Assert.IsType<PartialViewResult>(result);
            Assert.Equal("_DetailsModal", partialViewResult.ViewName);
        }
    }
}