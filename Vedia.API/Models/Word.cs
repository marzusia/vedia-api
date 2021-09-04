using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Vedia.API.Models
{
    public class Definition
    {
        public string Field { get; set; }
        public string Gloss { get; set; }
    }

    public class Etymology
    {
        public string Source { get; set; }
        public string Etymon { get; set; }
        public string EtymonRoot { get; set; }
        public string Link { get; set; }
        public string Notes { get; set; }
    }
    
    public class Sense
    {
        public string PartOfSpeech { get; set; }
        public string Class { get; set; }
        public IEnumerable<Definition> Definitions { get; set; }
        public Etymology Etymology { get; set; }
        public string Notes { get; set; }
    }

    public class Word
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Headword { get; set; }
        public string Pronunciation { get; set; }
        public IEnumerable<Sense> Senses { get; set; }
    }
}