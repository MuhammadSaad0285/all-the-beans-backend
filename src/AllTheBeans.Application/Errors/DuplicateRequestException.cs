namespace AllTheBeans.Application.Errors;

public class DuplicateRequestException : Exception
{
    public DuplicateRequestException(string message) : base(message) { }
}
