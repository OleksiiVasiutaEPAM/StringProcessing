namespace DataProcessingService.Business.Contracts.Services;

public interface IProcessingService
{
    /// <summary>
    /// Process an input string to the count plus base64 result
    /// </summary>
    /// <param name="input">Input string</param>
    /// <returns>Result</returns>
    string ProcessAsync(string input);
}