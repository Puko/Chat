namespace ChatApp.Http;

public interface IResponseMessageInterceptor
{
    void Intercept(HttpResponseMessage response);
}
