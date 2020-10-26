using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using AWSLambda_Dynamo.Models;

namespace AWSLambda_Dynamo
{
    public class Repository : IRepository
    {
        private readonly IDynamoDBContext _context;
        public Repository(IDynamoDBContext dynamoDBContext)
        {
            _context = dynamoDBContext;
        }

        public async Task<List<Person>> ScanForPeopleUsingFirstName(string firstName)
        {
            var conditions = new List<ScanCondition>
            {
                new ScanCondition(nameof(PersonDynamoDb.FirstName), ScanOperator.Equal, firstName)
            };
            var allDocs = await _context.ScanAsync<PersonDynamoDb>(conditions).GetRemainingAsync();
            var people = allDocs.ConvertAll(x => new Person(x));
            return people;
        }

        public async Task<Person> GetPerson(int personId)
        {
            var personDynamoDb = await _context.LoadAsync<PersonDynamoDb>(personId);
            var person = new Person(personDynamoDb);
            return person;
        }

        public async Task<int> AddPerson(Person person)
        {
            var personDynamoDb = new PersonDynamoDb(person);
            await _context.SaveAsync(personDynamoDb);
            return personDynamoDb.PersonId;
        }
    }

    public interface IRepository
    {
        Task<List<Person>> ScanForPeopleUsingFirstName(string firstName);
        Task<Person> GetPerson(int personId);
        Task<int> AddPerson(Person person);
    }
}
