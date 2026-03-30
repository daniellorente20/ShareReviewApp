namespace ShareReviewApp.Exceptions;

public class DuplicateReviewException : Exception
{
    public DuplicateReviewException(string message) : base(message) { }
}
