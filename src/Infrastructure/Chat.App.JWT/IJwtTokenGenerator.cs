namespace Chat.App.JWT;

public interface IJwtTokenGenerator
{
    JwtToken Generate(Guid userId, string username, string role);
}
