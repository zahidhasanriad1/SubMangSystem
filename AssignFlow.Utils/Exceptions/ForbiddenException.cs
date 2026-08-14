namespace AssignFlow.Utils.Exceptions;

public class ForbiddenException : AppException
{
    public ForbiddenException(string message = "You are not allowed to perform this action.") : base(message)
    {
    }

    public override int StatusCode => 403;
}
