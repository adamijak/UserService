using MongoDB.Bson;

namespace Api;

public class User
{
    public ObjectId Id { get; set; }
    public string? Email { get; set; }
    public string? FullName { get; set; }
    public DateTime BirthDate { get; set; }
}