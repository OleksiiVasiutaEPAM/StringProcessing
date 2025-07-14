namespace DataProcessingService.Exceptions;

public class ValidationFailedException(IDictionary<string, string[]> errors) 
    : Exception("Validation failed")
{
    public IDictionary<string, string[]> Errors { get; } = errors;
}