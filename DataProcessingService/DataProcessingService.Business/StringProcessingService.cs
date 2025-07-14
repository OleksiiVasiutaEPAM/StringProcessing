using System.Text;
using DataProcessingService.Business.Contracts.Services;

namespace DataProcessingService.Business;

/// <summary>
/// String processing service
/// </summary>
public class StringProcessingService : IStringProcessingService
{
    /// <inheritdoc />
    public string Process(string input) => input switch
    {
        null or "" => string.Empty,
        _ => $"{CountChars(input)}/{ToBase64(input)}"
    };

    private static string CountChars(string input) =>
        string.Concat(input
            .GroupBy(c => c)
            .OrderBy(g => g.Key)
            .Select(g => $"{g.Key}{g.Count()}"));

    private static string ToBase64(string input) =>
        Convert.ToBase64String(Encoding.UTF8.GetBytes(input));
}