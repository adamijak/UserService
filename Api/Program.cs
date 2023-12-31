using Api;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Runtime.CompilerServices;
using Api.Validators;
using FluentValidation;
using MongoDB.Bson;

[assembly:InternalsVisibleTo("ApiTest")]

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IValidator<UserDto>, UserDtoValidator>();
builder.Services.AddScoped<IValidator<UserNoIdDto>, UserNoIdDtoValidator>();

builder.Logging.AddConsole();

var mongoClient = new MongoClient( "mongodb://db:27017");
builder.Services.AddSingleton(mongoClient.GetDatabase("user-service-db").GetCollection<User>("users"));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/users", async ([FromServices] IMongoCollection<User> users, [FromServices] ILogger<User> logger) =>
{
   logger.LogInformation("GET /users");
   return (await users.Find(_ => true).ToListAsync()).Select(u => u.ToUserDto());
}).Produces<IEnumerable<UserDto>>();

app.MapPost("/users", async ([FromServices] IMongoCollection<User> users, [FromServices] ILogger<User> logger, [FromServices] IValidator<UserNoIdDto> validator, UserNoIdDto user) =>
{
   logger.LogInformation("POST /users");
   var validation = await validator.ValidateAsync(user);
   if (!validation.IsValid)
   {
      var errors = validation.Errors.Select(e => e.ErrorMessage);
      return Results.BadRequest(new ErrorResponse
      {
         Errors = errors
      });
   }

   var value = user.ToUser(new ObjectId());
   await users.InsertOneAsync(value);
   
   return Results.Ok(value.ToUserDto());
}).Produces<UserDto>()
   .ProducesProblem(StatusCodes.Status400BadRequest);

app.MapGet("/users/{id}", async ([FromServices] IMongoCollection<User> users, [FromServices] ILogger<User> logger, string id) =>
{
   logger.LogInformation("GET /users/{Id}", id);
   
   
   var user = await users.Find(u => u.Id == new ObjectId(id)).FirstOrDefaultAsync();
   return user is null ? Results.NotFound() : Results.Ok(user.ToUserDto());
}).Produces<UserDto>()
   .ProducesProblem(StatusCodes.Status404NotFound);

app.MapPut("/users/{id}", async ([FromServices] IMongoCollection<User> users, [FromServices] ILogger<User> logger,
      string? id, UserNoIdDto user) =>
   {
      logger.LogInformation("PUT /users/{Id}", id);

      var value = user.ToUser(new ObjectId(id));
      var result = await users.ReplaceOneAsync(u => u.Id == new ObjectId(id), value);
      
      return result.MatchedCount == 1 ? Results.Ok(value.ToUserDto()) : Results.NotFound();
   }).Produces<UserDto>()
   .ProducesProblem(StatusCodes.Status404NotFound);

app.MapDelete("/users/{id}", async ([FromServices] IMongoCollection<User> users, [FromServices] ILogger<User> logger, string id) =>
{
   logger.LogInformation("DELETE /users/{Id}", id);
   var result = await users.DeleteOneAsync(u => u.Id == new ObjectId(id));
   return result.DeletedCount == 1 ? Results.Ok() : Results.NotFound();
}).ProducesProblem(StatusCodes.Status404NotFound);

app.Run();