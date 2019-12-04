using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TodoWebProjekt.Authorization;
using TodoWebProjekt.Controllers;
using TodoWebProjekt.Models;
using TodoWebProjekt.Repository;
using TodoWebProjekt.Test.Helper;
using TodoWebProjekt.ViewModel;
using Xunit;

namespace TodoWebProjekt.Test
{
    public class UnitTestControllerIndex
    {
        private readonly IAuthorizationService _authorizationService = new TestAuthorizationService();

        private List<Task> GetTestTaskList()
        {
            return new List<Task>
            {
                new Task
                {
                    Title = "Test Eintrag", Description = "Ein Eintrag für einen kleinen Test",
                    UserId = "90e309f8-da89-4d21-82f7-297bd0a2f378", TaskId = 0
                },
                new Task
                {
                    Title = "Test Eintrag 2", Description = "Ein Eintrag für einen kleinen Test 2",
                    UserId = "ddc052bc-70e5-462e-a071-b932ba54909e", TaskId = 1
                },
                new Task
                {
                    Title = "Test Eintrag 3", Description = "Ein Eintrag für einen kleinen Test 3",
                    UserId = "ddc052bc-70e5-462e-a071-b932ba54909e", TaskId = 2
                }
            };
        }

        private ClaimsPrincipal GetTestUser()
        {
            return new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "Random Name"),
                new Claim(ClaimTypes.NameIdentifier, "90e309f8-da89-4d21-82f7-297bd0a2f378"),
                new Claim(ClaimTypes.Role, Constants.UserRole),
                new Claim("custom-claim", "example claim value")
            }, "mock"));
        }

        [Fact]
        public void Task_GetTodo_Return_Length()
        {
            //Arrange
            var mockRepo = new Mock<ITodoRepository>();
            var data = GetTestTaskList();
            var user = GetTestUser();

            var dataQueryable = data.AsQueryable();

            mockRepo.Setup(repo => repo.GetAll(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(dataQueryable);

            using var controller = new TodoController(mockRepo.Object, _authorizationService)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext {User = user}
                }
            };

            //Act
            var result = controller.Index();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IndexViewModel>(viewResult.ViewData.Model);

            Assert.Equal(3, model.Length);
        }

        [Fact]
        public void Task_GetTodo_Return_ViewResult()
        {
            //Arrange
            var mockRepo = new Mock<ITodoRepository>();
            var data = GetTestTaskList();
            var user = GetTestUser();

            var dataQueryable = data.AsQueryable();

            mockRepo.Setup(repo => repo.GetAll(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(dataQueryable);

            using var controller = new TodoController(mockRepo.Object, _authorizationService)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext {User = user}
                }
            };

            //Act
            var result = controller.Index();

            //Assert
            Assert.IsType<ViewResult>(result);
        }
    }
}