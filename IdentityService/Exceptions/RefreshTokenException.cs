namespace Exceptions
{
    public class RefreshTokenException(string message): Exception($"Có lỗi: {message}");
}