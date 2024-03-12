using Business.Models;

namespace Infrastructure.Mappers;

public static class UserMapper
{
    public static User MapFromDbResult(Dictionary<string, object> data)
    {
        ArgumentNullException.ThrowIfNull(data);

        return new User
        {
            Id = (Guid)data["id"],
            Name = (string)data["name"],
            Email = (string)data["email"],
            PasswordHash = (string)data["passwordhash"]
        };
    }
}
