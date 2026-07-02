namespace Chat.App.JWT;

public interface IRefreshTokenGenerator
{
    string Generate();
    string Hash(string token);
}
