using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Vedia.API.Models;

namespace Vedia.API.Services
{
    public class WordService
    {
        public IMongoCollection<Word> Words { get; }
        
        public WordService(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetSection("MongoDB")["ConnectionString"]);
            var database = client.GetDatabase(configuration.GetSection("MongoDB")["DatabaseName"]);
            Words = database.GetCollection<Word>("Words");
        }
    }
}