namespace Chat.App.Application.Exceptions;

public sealed class ChatAppException : Exception
{
    public string ErrorCode { get; }

    public ChatAppException(string errorCode, string message)
        : base(message)
    {
        ErrorCode = errorCode;
    }

    public ChatAppException(string errorCode, string message, Exception innerException)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
    }
}
