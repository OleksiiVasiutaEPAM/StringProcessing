using System.Text;
using DataProcessingService.Business.Contracts.Services;

namespace DataProcessingService.Business;

/// <summary>
/// String processing service
/// </summary>
public class StringProcessingService : IStringProcessingService
{
    /// <inheritdoc />
    public string ProcessAsync(string input)
    {
        var counts = input.GroupBy(c => c)
            .OrderBy(g => g.Key)
            .Select(g => $"{g.Key}{g.Count()}");
        
        var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(input));

        return $"{string.Join("", counts)}/{base64}";
    }
}