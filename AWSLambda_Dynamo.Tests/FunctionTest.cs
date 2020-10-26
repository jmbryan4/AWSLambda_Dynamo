using System.Text.Json;
using System.Threading.Tasks;
using Amazon.Lambda.TestUtilities;
using AWSLambda_Dynamo.Models;
using Xunit;

namespace AWSLambda_Dynamo.Tests
{
    public class FunctionTest
    {
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
