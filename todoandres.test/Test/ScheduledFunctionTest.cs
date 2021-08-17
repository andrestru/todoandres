using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using todoandres.functions.function;
using todoandres.test.Helpers;
using Xunit;

namespace todoandres.test.Test
{
    public class ScheduledFunctionTest
    {
  

        [Fact]
        public void ScheduledFunction_should_log_Message()
        {
            MockCloudTableTodos mock = new MockCloudTableTodos(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            ListLogger listLogger =  (ListLogger)TestFactory.createlogger(LoggerTypes.List);

            //Act
            ScheduledFunction.Run(null, mock, listLogger);
            string message = listLogger.Logs[0];

            //Assert
            Assert.Contains("Deleting completed", message);  //por el metodo de otra clase
        }
    }
}
