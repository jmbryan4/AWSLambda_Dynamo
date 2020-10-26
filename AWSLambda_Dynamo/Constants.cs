namespace AWSLambda_Dynamo
{
    public static class Constants
    {
        public static class EnvironmentVariables
        {
            public const string Environment = "ENVIRONMENT";
            public const string CreateDynamoDBTables = nameof(CreateDynamoDBTables);
        }

        public static class Environments
        {
            public const string Development = nameof(Development);
            public const string Sandbox = nameof(Sandbox);
            public const string Production = nameof(Production);
        }
    }
}
