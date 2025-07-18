namespace DataProcessingService.Business.Contracts.Services;

public interface IStringProcessingService
{
    /// <summary>
    /// Process an input string to the count plus base64 result
    /// </summary>
    /// <param name="input">Input string</param>
    /// <returns>Result</returns>
    string Process(string input);
}