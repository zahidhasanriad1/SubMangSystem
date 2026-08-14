namespace AssignFlow.Utils.Exceptions;

public class ConflictException : AppException
{
    public ConflictException(string message) : base(message)
    {
    }

    public override int StatusCode => 409;
}
