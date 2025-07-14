using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using DataProcessingService.Business.Contracts.Services;
using DataProcessingService.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace DataProcessingService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProcessingController(IStringProcessingService processingService) : ControllerBase
{
    [HttpGet("stream")]
    public async IAsyncEnumerable<string> Stream([FromQuery, Required] string input, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(e => e.Value?.Errors?.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                );

            throw new ValidationFailedException(errors);
        }
        
        Response.ContentType = "text/event-stream";
        
        var result = processingService.Process(input);

        yield return $"event: metadata\ndata: {result.Length}\n\n";

        foreach (var ch in result)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await Task.Delay(Random.Shared.Next(1000, 5000), cancellationToken);
            yield return $"data: {ch}\n\n";
        }
    }
}