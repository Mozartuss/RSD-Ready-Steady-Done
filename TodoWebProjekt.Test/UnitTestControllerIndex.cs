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

        [Fact]
        public void Task_GetTodo_Return_ViewResult()
        {
            //Arrange

            var mockRepo = new Mock<ITodoRepository>();
            var controller = new TodoController(mockRepo.Object, _authorizationService);

            //Act
            var result = controller.Index();

            //Assert
            Assert.IsType<ViewResult>(result);
        }
    }
}