using System.ComponentModel.DataAnnotations;
using DataProcessingService.Business.Contracts.Services;
using DataProcessingService.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace DataProcessingService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProcessingController(IStringProcessingService processingService) : ControllerBase
{
    [HttpGet("stream")]
    public async Task Stream([FromQuery, Required] string input, CancellationToken cancellationToken)
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
    
        await Response.WriteAsync($"event: metadata\ndata: {result.Length}\n\n", cancellationToken);
        await Response.Body.FlushAsync(cancellationToken);

        foreach (var ch in result)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await Task.Delay(Random.Shared.Next(1000, 5000), cancellationToken);
            await Response.WriteAsync($"data: {ch}\n\n", cancellationToken);
            await Response.Body.FlushAsync(cancellationToken);
        }
    }
}