namespace Chat.App.Wpf.Services;

public sealed class ServiceResult<T>
{
    public T? Value { get; init; }

    public string? Error { get; init; }

    public bool IsSuccess => Error is null;

    public static ServiceResult<T> Ok(T value) => new() { Value = value };

    public static ServiceResult<T> Fail(string error) => new() { Error = error };
}
