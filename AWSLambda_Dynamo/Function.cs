using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.Core;
using AWSLambda_Dynamo.Models;
using Microsoft.Extensions.DependencyInjection;
using static AWSLambda_Dynamo.Constants;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace AWSLambda_Dynamo
{
    public class Function
    {
        private readonly string _environment = Environment.GetEnvironmentVariable(EnvironmentVariables.Environment) ?? Environments.Sandbox;
        private readonly bool _createTables;
        private ServiceCollection _serviceCollection;
        private readonly AmazonDynamoDBConfig _awsDynamoDBConfig;

        public Function()
        {
            _createTables = Environment.GetEnvironmentVariable(EnvironmentVariables.CreateDynamoDBTables) == "True";
            _awsDynamoDBConfig = GetDynamoDBConfig();
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
            if (_createTables)
            {
                await DynamoDBFactory.CreateAsync(_awsDynamoDBConfig);
            }
            using ServiceProvider serviceProvider = _serviceCollection.BuildServiceProvider();
            var repo = serviceProvider.GetRequiredService<IRepository>();
            int personId = await repo.AddPerson(input);
            var response = await repo.ScanForPeopleUsingFirstName(input.FirstName);
            return response;
        }

        private AmazonDynamoDBConfig GetDynamoDBConfig()
        {
            var config = new AmazonDynamoDBConfig();
            if (_environment == Environments.Development)
            {
                config.ServiceURL = "http://localhost:4566";
                config.UseHttp = true;
            }
            return config;
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
