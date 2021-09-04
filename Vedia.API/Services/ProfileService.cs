using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Vedia.API.Models;

namespace Vedia.API.Services
{
    public class ProfileService
    {
        public IMongoCollection<Profile> Profiles { get; }

        public ProfileService(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetSection("MongoDB")["ConnectionString"]);
            var database = client.GetDatabase(configuration.GetSection("MongoDB")["DatabaseName"]);
            Profiles = database.GetCollection<Profile>("Profiles");
        }
    }
}