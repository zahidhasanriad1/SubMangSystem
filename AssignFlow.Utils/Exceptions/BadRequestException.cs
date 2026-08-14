namespace AssignFlow.Utils.Exceptions;

public class BadRequestException : AppException
{
    public BadRequestException(string message) : base(message)
    {
    }

    public override int StatusCode => 400;
}
