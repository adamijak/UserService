using System.Net;
using System.Net.Http.Json;
using Api;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ApiTest.IntegrationTests;

[TestClass]
public class UserTests
{

    private static IMongoCollection<User> users;
    private static HttpClient httpClient;
    
    [ClassInitialize]
    public static void ClassInitialize(TestContext testContext)
    {
        var mongoClient = new MongoClient( "mongodb://db:27017");
        users = mongoClient.GetDatabase("user-service-db").GetCollection<User>("users");
        
        httpClient = new HttpClient();
    }
    
    [TestMethod]
    public async Task PostTest()
    {
        var response = await httpClient.PostAsJsonAsync("http://api/users", new UserNoIdDto()
        {
            Email = "david@email.com",
            BirthDate = new DateTime(2000, 01, 03),
            FullName = "David Small",
        });
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        var userResponse = await response.Content.ReadFromJsonAsync<UserDto>();
        
        var user = await users.Find(u => u.Id == new ObjectId(userResponse.Id)).FirstOrDefaultAsync();
        Assert.IsNotNull(user);
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        httpClient.Dispose();
    }
}