using Business.Models;

namespace WebApi.Helpers;

public interface IJwtHelper
{
    string GetAccessToken(User user, DateTime expires);
}
