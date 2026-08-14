namespace AssignFlow.Utils.Exceptions;

public class NotFoundException : AppException
{
    public NotFoundException(string message = "The requested resource was not found.") : base(message)
    {
    }

    public override int StatusCode => 404;
}
