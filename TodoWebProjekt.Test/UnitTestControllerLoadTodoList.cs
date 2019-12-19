using System.Collections.Generic;
using System.Security.Claims;
using TodoWebProjekt.Models;
using TodoWebProjekt.Authorization;
using Microsoft.AspNetCore.Authorization;
using Xunit;
using Moq;
using TodoWebProjekt.Repository;
using System.Linq;
using TodoWebProjekt.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using TodoWebProjekt.Test.Helper;
using TodoWebProjekt.ViewModel;

namespace TodoWebProjekt.Test
{
    public class UnitTestControllerLoadTodoList
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

            var sessionMock = new Mock<ISession>();
            var key = "CurrentPageSize";
            var value = new byte[]
            {
                9 >> 24,
                0xFF & (9 >> 16),
                0xFF & (9 >> 8),
                0xFF & 9
            };
            sessionMock.Setup(s => s.Set(key, It.IsAny<byte[]>()))
                .Callback<string, byte[]>((k, v) => value = v);

            sessionMock.Setup(_ => _.TryGetValue(key, out value))
                .Returns(true);

            mockRepo.Setup(repo => repo.GetAll(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(dataQueryable);

            using var controller = new TodoController(mockRepo.Object, _authorizationService)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = user, Session = sessionMock.Object }
                }
            };

            //Act
            var result = controller.LoadTodoList("", "", "", null, 0);

            //Assert
            var viewResult = Assert.IsType<PartialViewResult>(result);
            var model = Assert.IsAssignableFrom<IndexViewModel>(viewResult.ViewData.Model);

            Assert.Equal(3, model.Length);
        }
    }
}
