using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using todoandres.common.Model;
using todoandres.common.Response;
using todoandres.functions.Entities;

namespace todoandres.functions.function
{
    public static class todoApi
    {
        [FunctionName(nameof(createTodo))]
        public static async Task<IActionResult> createTodo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            [Table("todo", Connection = "AzureWebJobsStorage")] CloudTable todoTable,
            ILogger log)
        {

            log.LogInformation("Recieved a new todo.");



            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            todo todo = JsonConvert.DeserializeObject<todo>(requestBody);

            if (string.IsNullOrEmpty(todo?.TaskDescription))
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSuccess = false,
                    Message = "The request must have a TaskDescription."
                });
            }

            TodoEntity todoEntity = new TodoEntity
            {

                CreatedTime = DateTime.UtcNow,
                ETag = "*",
                IsCompleted = false,
                PartitionKey = "TODO",  //nombre de la tabla
                RowKey = Guid.NewGuid().ToString(),
                TaskDescription = todo.TaskDescription
            };

            TableOperation addOperation = TableOperation.Insert(todoEntity);
            await todoTable.ExecuteAsync(addOperation);
            string message = "New todo stored in table";
            log.LogInformation(message);


            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = todoEntity
            });

        }

        [FunctionName(nameof(updateTodo))]
        public static async Task<IActionResult> updateTodo(
           [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "todo/{id}")] HttpRequest req,
           [Table("todo", Connection = "AzureWebJobsStorage")] CloudTable todoTable,
           string id,
           ILogger log)
        {

            log.LogInformation($"Update for todo: {id}, received.");


            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            todo todo = JsonConvert.DeserializeObject<todo>(requestBody);

            //validate todo id
            TableOperation findid = TableOperation.Retrieve<TodoEntity>("TODO", id);
            TableResult findresult = await todoTable.ExecuteAsync(findid);

            if (findresult.Result == null)
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSuccess = false,
                    Message = "Todo not found."
                });
            }

            //Update todo
            TodoEntity todoEntity = (TodoEntity)findresult.Result;
            todoEntity.IsCompleted = todo.IsCompleted;
            if (!string.IsNullOrEmpty(todo.TaskDescription))
            {
                todoEntity.TaskDescription = todo.TaskDescription;
            }

            TableOperation updateOperation = TableOperation.Replace(todoEntity);
            await todoTable.ExecuteAsync(updateOperation);
            string message = $"Todo: {id} updated in table";
            log.LogInformation(message);


            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = todoEntity
            });

        }

        [FunctionName(nameof(getAllTodo))]
        public static async Task<IActionResult> getAllTodo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todo")] HttpRequest req,
            [Table("todo", Connection = "AzureWebJobsStorage")] CloudTable todoTable,
            ILogger log)
        {

            log.LogInformation("get all todo received.");

            //no tiene parametros, elimino todo
            TableQuery<TodoEntity> tableQuery = new TableQuery<TodoEntity>();
            TableQuerySegment<TodoEntity> todoquerys = await todoTable.ExecuteQuerySegmentedAsync(tableQuery, null);


            string message = "Retrieved all todos.";
            log.LogInformation(message);


            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = todoquerys
            });

        }

        [FunctionName(nameof(getById))]
        public static async Task<IActionResult> getById(
          [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todo/{id}")] HttpRequest req,
          [Table("todo", "TODO", "{id}", Connection = "AzureWebJobsStorage")] TodoEntity TodoEntity,
          string id,
          ILogger log)
        {

            log.LogInformation($"get todo by id: {id} received.");

            if (TodoEntity == null)
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSuccess = false,
                    Message = "Todo not found."
                });
            }

            string message = $"Todo: {TodoEntity.RowKey}, retrieved.";
            log.LogInformation(message);


            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = TodoEntity
            });

        }

        [FunctionName(nameof(deletebyId))]
        public static async Task<IActionResult> deletebyId(
           [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "todo/{id}")] HttpRequest req,
           [Table("todo", "TODO", "{id}", Connection = "AzureWebJobsStorage")] TodoEntity TodoEntity,
           [Table("todo", Connection = "AzureWebJobsStorage")] CloudTable todoTable,
           string id,
           ILogger log)
        {

            log.LogInformation($"Delete todo: {id}, received.");

            if (TodoEntity == null)
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSuccess = false,
                    Message = "Todo not found."
                });
            }

            await todoTable.ExecuteAsync(TableOperation.Delete(TodoEntity));
            string message = $"Todo: {TodoEntity.RowKey}, deleted.";
            log.LogInformation(message);


            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = TodoEntity
            });

        }
    }

}
