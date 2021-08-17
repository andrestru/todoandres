using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using todoandres.common.Model;
using todoandres.functions.function;
using todoandres.test.Helpers;
using Xunit;

namespace todoandres.test.Test
{
    public class TodoApiTest
    {
        private readonly ILogger ilogger = TestFactory.createlogger();

        [Fact]
        public async void CreateTodo_Should_Return200()
        {
            //Arrange (prepara la prueba)
            MockCloudTableTodos mock = new MockCloudTableTodos(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            todo todoRequest = TestFactory.GetTodoRequest();
            DefaultHttpRequest request = TestFactory.createHttpRequest(todoRequest);

            //Act(ejecutar como tal)
            IActionResult response = await todoApi.createTodo(request, mock, ilogger);

            //Assert (verificar el resultado correcto)
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }


        [Fact]
        public async void UpdateTodo_Should_Return200()
        {
            //Arrange (prepara la prueba)
            MockCloudTableTodos mock = new MockCloudTableTodos(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            todo todoRequest = TestFactory.GetTodoRequest();
            Guid TodoId = Guid.NewGuid();
            DefaultHttpRequest request = TestFactory.createHttpRequest(TodoId, todoRequest);

            //Act(ejecutar como tal)
            IActionResult response = await todoApi.updateTodo(request, mock, TodoId.ToString(), ilogger);

            //Assert (verificar el resultado correcto)
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }
    }
}
