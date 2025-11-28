namespace Exceptions
{
    public class UserAlreadyExistException(string email): Exception($"Email {email} này đã tồn tại");
}