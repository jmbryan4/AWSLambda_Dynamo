using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using AWSLambda_Dynamo.Models;

namespace AWSLambda_Dynamo
{
    /// <summary>
    /// Factory for seeding a DynamoDB (for Dev environment & testing)
    /// </summary>
    public static class DynamoDBFactory
    {
        /// <summary>
        /// Create DynamoDB tables for testing
        /// </summary>
        /// <param name="config">AmazonDynamoDBConfig</param>
        public static async Task CreateAsync(AmazonDynamoDBConfig config)
        {
            using var client = new AmazonDynamoDBClient(config);

            var tableResponse = await client.ListTablesAsync();
            if (!tableResponse.TableNames.Contains("People"))
            {
                Console.WriteLine("Creating Table People");
                // Create our table if it doesn't exist
                await client.CreateTableAsync(new CreateTableRequest
                {
                    TableName = "People",
                    ProvisionedThroughput = new ProvisionedThroughput
                    {
                        ReadCapacityUnits = 3,
                        WriteCapacityUnits = 2
                    },
                    KeySchema = new List<KeySchemaElement>
                    {
                        new KeySchemaElement
                        {
                            AttributeName = nameof(Person.PersonId),
                            KeyType = KeyType.HASH
                        }
                    },
                    AttributeDefinitions = new List<AttributeDefinition>
                    {
                        new AttributeDefinition
                        {
                            AttributeName = nameof(Person.PersonId),
                            AttributeType = ScalarAttributeType.N
                        }
                    }
                });

                await VerifyTableIsReady(client, "People");
            }
        }

        private static async Task VerifyTableIsReady(AmazonDynamoDBClient client, string tableName)
        {
            bool isTableAvailable = false;
            do
            {
                Thread.Sleep(2000);
                try
                {
                    Console.WriteLine($"Checking if Table: {tableName} is ready for use.");
                    var tableStatus = await client.DescribeTableAsync(tableName);
                    isTableAvailable = tableStatus.Table.TableStatus == TableStatus.ACTIVE;
                }
                catch (ResourceNotFoundException)
                {
                    // DescribeTable is eventually consistent. So you might get resource not found. 
                }
            } while (!isTableAvailable);
        }
    }
}
