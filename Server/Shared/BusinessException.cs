namespace Server.Shared;

public class BusinessException : Exception
{
    public BusinessException(string message)
        : base(message)
    {
    }

    public BusinessException()
    {
    }

    public BusinessException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
