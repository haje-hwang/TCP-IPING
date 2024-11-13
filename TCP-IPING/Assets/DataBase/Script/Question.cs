using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

[BsonIgnoreExtraElements]
public struct Question
{
    [BsonElement("id")]
    public int id { get; set; }

    [BsonElement("question")]
    public string question { get; set; }

    [BsonElement("options")]
    public List<string> options { get; set; }

    [BsonElement("answer")]
    public int answer { get; set; }

    [BsonElement("category")]
    public string category { get; set; }

    [BsonElement("difficulty")]
    public string difficulty { get; set; }
}
