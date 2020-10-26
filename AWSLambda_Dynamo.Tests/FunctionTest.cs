using System;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.Lambda.TestUtilities;
using AWSLambda_Dynamo.Models;
using Xunit;
using static AWSLambda_Dynamo.Constants;

namespace AWSLambda_Dynamo.Tests
{
    public class FunctionTest
    {
        public FunctionTest()
        {
            Environment.SetEnvironmentVariable(EnvironmentVariables.Environment, Environments.Development);
            Environment.SetEnvironmentVariable(EnvironmentVariables.CreateDynamoDBTables, "True");
        }

        [Fact]
        public async Task TestFunction()
        {
            var function = new Function();
            var context = new TestLambdaContext();
            var input = new Person
            {
                PersonId = 1,
                FirstName = "Unit",
                LastName = "Test",
                State = "UT"
            };

            var result = await function.FunctionHandler(input, context);

            Assert.Contains(JsonSerializer.Serialize(input), JsonSerializer.Serialize(result));
        }
    }
}
