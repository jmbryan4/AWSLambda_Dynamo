using Amazon.DynamoDBv2.DataModel;

namespace AWSLambda_Dynamo.Models
{
    public class Person
    {
        public Person() { }

        public Person(PersonDynamoDb personDynamoDb)
        {
            PersonId = personDynamoDb.PersonId;
            State = personDynamoDb.State;
            FirstName = personDynamoDb.FirstName;
            LastName = personDynamoDb.LastName;
        }

        public int PersonId { get; set; }
        public string State { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    [DynamoDBTable("People")]
    public class PersonDynamoDb
    {
        public PersonDynamoDb() { }
        public PersonDynamoDb(Person person)
        {
            PersonId = person.PersonId;
            State = person.State;
            FirstName = person.FirstName;
            LastName = person.LastName;
        }

        [DynamoDBHashKey]
        public int PersonId { get; set; }
        public string State { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
