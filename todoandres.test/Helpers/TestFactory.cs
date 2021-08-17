using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using System;
using System.IO;
using todoandres.common.Model;
using todoandres.functions.Entities;

namespace todoandres.test.Helpers
{
    public class TestFactory
    {

        public static TodoEntity GetTodoEntity()
        {
            return new TodoEntity
            {
                ETag = "*",
                PartitionKey = "TODO",
                RowKey = Guid.NewGuid().ToString(),
                CreatedTime = DateTime.UtcNow,
                IsCompleted = false,
                TaskDescription = "Task: kill the humans."
            };
        }

        public static DefaultHttpRequest createHttpRequest(Guid Todoid, todo todoRequest)
        {
            string request = JsonConvert.SerializeObject(todoRequest);
            DefaultHttpRequest httpRequest = new DefaultHttpRequest(new DefaultHttpContext())
            {
                Body = GenerateStreamFromString(request),
                Path = $"/{Todoid}"
            };
            return httpRequest;
        }

        public static DefaultHttpRequest createHttpRequest(Guid Todoid)
        {
            DefaultHttpRequest httpRequest = new DefaultHttpRequest(new DefaultHttpContext())
            {
                Path = $"/{Todoid}"
            };
            return httpRequest;
        }

        public static DefaultHttpRequest createHttpRequest(todo todoRequest)
        {
            string request = JsonConvert.SerializeObject(todoRequest);
            DefaultHttpRequest httpRequest = new DefaultHttpRequest(new DefaultHttpContext())
            {
                Body = GenerateStreamFromString(request),
            };
            return httpRequest;
        }

        public static DefaultHttpRequest createHttpRequest()
        {
            DefaultHttpRequest httpRequest = new DefaultHttpRequest(new DefaultHttpContext());
            return httpRequest;
        }

        public static todo GetTodoRequest()
        {
            return new todo
            {
                CreatedTime = DateTime.UtcNow,
                IsCompleted = false,
                TaskDescription = "Try to conquer the world."

            };
        }


        public static Stream GenerateStreamFromString(string stringToConvert)
        {
            MemoryStream memoryStream = new MemoryStream();
            StreamWriter streamWriter = new StreamWriter(memoryStream);
            streamWriter.Write(stringToConvert);
            streamWriter.Flush();
            memoryStream.Position = 0;
            return memoryStream;
        }


        //este metodo si va de acuerdo a la logica

        public static ILogger createlogger(LoggerTypes type = LoggerTypes.Null)
        {
            ILogger logger;

            if (type == LoggerTypes.List)
            {
                logger = new ListLogger();
            }
            else
            {
                logger = NullLoggerFactory.Instance.CreateLogger("Null Logger");
            }
            return logger;
        }

    }
}
