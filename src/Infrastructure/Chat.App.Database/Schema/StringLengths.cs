namespace Chat.App.Database.Schema;

public static class StringLengths
{
    public const int UserName = 50;
    public const int UserPasswordHash = 512;
    public const int UserRole = 20;
    public const int ConversationType = 20;
    public const int MessageContent = 4000;
    public const int RefreshTokenHash = 64;
}
