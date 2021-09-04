using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using Vedia.API.Models;

namespace Vedia.API.Services.Runnable
{
    public class DatabaseSetupService : IHostedService
    {
        private readonly IMongoDatabase _database;

        public DatabaseSetupService(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetSection("MongoDB")["ConnectionString"]);
            _database = client.GetDatabase(configuration.GetSection("MongoDB")["DatabaseName"]);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // unique index on Profile.Username and on Profile.Email
            var profiles = _database.GetCollection<Profile>("Profiles");
            var profileIndexUsernameDefinition = Builders<Profile>.IndexKeys.Ascending(p => p.Username);
            var profileIndexEmailDefinition = Builders<Profile>.IndexKeys.Ascending(p => p.Email);
            await profiles.Indexes.CreateOneAsync(
                new CreateIndexModel<Profile>(profileIndexUsernameDefinition, new CreateIndexOptions {Unique = true}),
                cancellationToken: cancellationToken);
            await profiles.Indexes.CreateOneAsync(
                new CreateIndexModel<Profile>(profileIndexEmailDefinition, new CreateIndexOptions {Unique = true}),
                cancellationToken: cancellationToken);
            
            // unique index on Word.Headword
            var words = _database.GetCollection<Word>("Words");
            var wordIndexDefinition = Builders<Word>.IndexKeys.Ascending(w => w.Headword);
            await words.Indexes.CreateOneAsync(
                new CreateIndexModel<Word>(wordIndexDefinition, new CreateIndexOptions {Unique = true}),
                cancellationToken: cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}