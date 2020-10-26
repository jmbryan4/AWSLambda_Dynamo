using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.Core;
using AWSLambda_Dynamo.Models;
using Microsoft.Extensions.DependencyInjection;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace AWSLambda_Dynamo
{
    public class Function
    {
        private ServiceCollection _serviceCollection;
        private readonly AmazonDynamoDBConfig _awsDynamoDBConfig;

        public Function()
        {
            _awsDynamoDBConfig = new AmazonDynamoDBConfig
            {
                ServiceURL = "http://localhost:4566",
                UseHttp = true
            };
            ConfigureServices();
        }

        /// <summary>
        /// Entrypoint for Lambda
        /// </summary>
        /// <example>
        /// { "personId": 1, "firstName": "Hello", "lastName": "World", "state": "AL" }
        /// </example>
        /// <param name="input"></param>
        /// <param name="context"></param>
        public async Task<List<Person>> FunctionHandler(Person input, ILambdaContext context)
        {
            Console.WriteLine("Running Function");
            await DynamoDBFactory.CreateAsync(_awsDynamoDBConfig);
            using ServiceProvider serviceProvider = _serviceCollection.BuildServiceProvider();
            var repo = serviceProvider.GetRequiredService<IRepository>();
            int personId = await repo.AddPerson(input);
            var response = await repo.ScanForPeopleUsingFirstName(input.FirstName);
            return response;
        }

        private void ConfigureServices()
        {
            Console.WriteLine("Registering Dependencies");
            _serviceCollection = new ServiceCollection();
            _serviceCollection.AddSingleton<IDynamoDBContext>(_ => new DynamoDBContext(new AmazonDynamoDBClient(_awsDynamoDBConfig)));
            _serviceCollection.AddTransient<IRepository, Repository>();
        }
    }
}
