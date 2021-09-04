using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Vedia.API.Models
{
    public class Profile
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Username { get; set; }
        [JsonIgnore]
        public string Hash { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public List<string> Roles { get; set; }
        public DateTime Created { get; set; }
    }
}