using Api;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Runtime.CompilerServices;
using Api.Validators;
using FluentValidation;

[assembly:InternalsVisibleTo("ApiTest")]

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddValidatorsFromAssemblyContaining<UserValidator>();

builder.Logging.AddConsole();

var mongoClient = new MongoClient( "mongodb://db:27017");
builder.Services.AddSingleton(mongoClient.GetDatabase("user-service-db").GetCollection<User>("users"));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/users", async ([FromServices] IMongoCollection<User> users, [FromServices] ILogger<User> logger) =>
{
   logger.LogInformation("GET /users");
   return await users.Find(_ => true).ToListAsync();
});

app.MapPost("/users", async ([FromServices] IMongoCollection<User> users, [FromServices] ILogger<User> logger, [FromServices] UserValidator validator, User user) =>
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
   
   await users.InsertOneAsync(user);
   return Results.Ok();
});

app.MapGet("/users/{id}", async ([FromServices] IMongoCollection<User> users, [FromServices] ILogger<User> logger, string id) =>
{
   logger.LogInformation("GET /users/{Id}", id);
   
   var user = await users.Find(u => u.Id == id).FirstOrDefaultAsync();
   return user is null ? Results.NotFound() : Results.Ok(user);
});

app.MapPut("/users/{id}", async ([FromServices] IMongoCollection<User> users, [FromServices] ILogger<User> logger, string? id, User user) =>
{
   logger.LogInformation("PUT /users/{Id}", id);
   if (user.Id is not null)
   {
      return Results.BadRequest(new ErrorResponse
      {
         Errors = new []{"Can not change user id"},
      });
   }

   user.Id = id;
   var result = await users.ReplaceOneAsync(u => u.Id == id, user );
   return result.MatchedCount == 1 ? Results.Ok() : Results.NotFound();
});

app.MapDelete("/users/{id}", async ([FromServices] IMongoCollection<User> users, [FromServices] ILogger<User> logger, string id) =>
{
   logger.LogInformation("DELETE /users/{Id}", id);
   var result = await users.DeleteOneAsync(u => u.Id == id);
   return result.DeletedCount == 1 ? Results.Ok() : Results.NotFound();
});

app.Run();