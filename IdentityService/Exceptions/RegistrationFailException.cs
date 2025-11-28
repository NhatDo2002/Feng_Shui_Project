namespace Exceptions
{
    public class RegistrationFailException(IEnumerable<string> errorDescriptions): Exception($"Đăng ký thất bại với những lỗi sau: {string.Join(Environment.NewLine, errorDescriptions)}");
}