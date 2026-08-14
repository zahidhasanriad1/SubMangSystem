namespace AssignFlow.Utils.Exceptions;

public class UnauthorizedException : AppException
{
    public UnauthorizedException(string message = "Authentication failed.") : base(message)
    {
    }

    public override int StatusCode => 401;
}
