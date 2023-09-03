using MongoDB.Bson;

namespace Api;

public static class UserMappingExtensions
{
    public static UserDto ToUserDto(this User user)
    {
        return new UserDto
        {
            Id = user.Id.ToString(),
            Email = user.Email,
            BirthDate = user.BirthDate,
            FullName = user.FullName,
        };
    }
    
    public static User ToUser(this UserDto user)
    {
        return new User
        {
            Id = new ObjectId(user.Id),
            Email = user.Email,
            BirthDate = user.BirthDate,
            FullName = user.FullName,
        };
    }
    
    public static UserNoIdDto ToUserNoIdDto(this User user)
    {
        return new UserNoIdDto()
        {
            Email = user.Email,
            BirthDate = user.BirthDate,
            FullName = user.FullName,
        };
    }
    
    public static User ToUser(this UserNoIdDto user, ObjectId id)
    {
        return new User
        {
            Id = id,
            Email = user.Email,
            BirthDate = user.BirthDate,
            FullName = user.FullName,
        };
    }
}